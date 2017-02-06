using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
	// Manager 单例
	public static Manager Instance;
	// 当前玩家 HP
	public int CurrentHP = 100;
	// 添加传送门
	public GameObject Portals;
	// 游戏重玩提示
	public GameObject Replay;

	void Awake ()
	{
		// 实现单例
		if (Instance == null) {
			Instance = this;
		} else {
			Debug.LogError ("Only One Manager is Allowed");
		}
	}

	// 受到怪物攻击
	public void UnderAttack ()
	{
		
		CurrentHP--;

		// 如果血量小于0，游戏结束
		if (CurrentHP < 0) {
			EndGame ();
		}
	}

	void EndGame ()
	{
		Destroy (Portals);
		Replay.SetActive (true);
	}

	public void ReStartGame ()
	{
		SceneManager.LoadScene ("FPS");
	}
}
