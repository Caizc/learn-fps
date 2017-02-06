using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunShoot : MonoBehaviour
{

	public Animator Animator;
	public Transform GunPoint;
	public GameObject Spark;
	public TextMesh textMesh;
	public AudioClip Fire;
	public AudioClip Reload;
	AudioSource AudioSource;
	bool isReloading = false;
	int maxBullet = 12;
	int currentBullet;
	SteamVR_TrackedController SteamVR_TrackedController;

	void Start ()
	{
		// 注册手柄事件监听
		SteamVR_TrackedController = GetComponent<SteamVR_TrackedController> ();
		SteamVR_TrackedController.TriggerClicked += TriggerClicked;
		SteamVR_TrackedController.Gripped += Gripped;

		currentBullet = maxBullet;
		AudioSource = GetComponent<AudioSource> ();
	}

	// 扣下扳机的开枪逻辑
	void TriggerClicked (object sender, ClickedEventArgs e)
	{
		if (isReloading) {
			return;
		}

		if (currentBullet > 0) {
			currentBullet--;
			textMesh.text = currentBullet.ToString ();
		} else {
			return;
		}

		// 播放开枪声音
		AudioSource.PlayOneShot (Fire);
		Animator.Play ("PistolAnimation");

		Debug.DrawRay (GunPoint.position, GunPoint.up * 100, Color.red, 0.02f);

		Ray raycast = new Ray (GunPoint.position, GunPoint.up);
		RaycastHit hit;

		// 根据 Layer 来判断是否有物体击中
		LayerMask layer = 1 << (LayerMask.NameToLayer ("Enermy"));
		bool bHit = Physics.Raycast (raycast, out hit, 10000, layer.value);

		Debug.Log (bHit.ToString());

		// 击中怪物扣血逻辑
		if (bHit) {

			Debug.Log (hit.collider.gameObject);

			EnermyController ec = hit.collider.gameObject.GetComponent<EnermyController> ();
			if (ec != null) {
				ec.UnderAttack ();
				GameObject go = GameObject.Instantiate (Spark);
				go.transform.position = hit.point;
				Destroy (go, 3);
			} else {
				Manager.Instance.ReStartGame ();
			}
		}
	}

	// 握住手柄的逻辑
	void Gripped (object sender, ClickedEventArgs e)
	{
		if (isReloading) {
			return;
		}

		isReloading = true;
		Invoke ("ReloadFinished", 1);

		AudioSource.PlayOneShot (Reload);
	}

	// 换弹结束逻辑
	void ReloadFinished ()
	{
		isReloading = false;
		currentBullet = maxBullet;
		textMesh.text = currentBullet.ToString ();
	}

	void Update ()
	{
		
	}
}
