using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnermyController : MonoBehaviour
{

	// 目标位置
	public Transform TargetTransform;
	// 总血量
	int HP = 2;
	// 导航组件
	NavMeshAgent NavMashAgent;
	// 动画组件
	Animator Animator;

	void Start ()
	{
		// 初始化变量
		NavMashAgent = GetComponent<NavMeshAgent> ();
		NavMashAgent.SetDestination (TargetTransform.transform.position);

		Animator = GetComponent<Animator> ();
	}

	void Update ()
	{
		// 如果已死亡则直接返回
		if (HP <= 0) {
			return;
		}

		// 获取当前动画信息
		AnimatorStateInfo stateInfo = Animator.GetCurrentAnimatorStateInfo (0);

		// 判断当前的动画状态是什么，并进行相应的处理
		if (stateInfo.fullPathHash == Animator.StringToHash ("Base Layer.Run") && !Animator.IsInTransition (0)) {
			
			Animator.SetBool ("Run", false);

			// 玩家有移动则重新检查，目前不会移动
			if (Vector3.Distance (TargetTransform.transform.position, NavMashAgent.destination) > 1) {
				NavMashAgent.SetDestination (TargetTransform.transform.position);
			}

			// 进入攻击距离则跳转到攻击动画，否则继续跑动
			if (NavMashAgent.remainingDistance < 3) {
				Animator.SetBool ("Attack", true);
			} else {
				Animator.SetBool ("Run", true);
			}
		}

		if (stateInfo.fullPathHash == Animator.StringToHash ("Base Layer.Attack") && !Animator.IsInTransition (0)) {
			
			Animator.SetBool ("Attack", false);

			// 玩家有移动则重新检查，目前不会移动
			if (Vector3.Distance (TargetTransform.transform.position, NavMashAgent.destination) > 1) {
				NavMashAgent.SetDestination (TargetTransform.transform.position);
			}

			// 进入攻击距离则跳转到攻击动画，否则继续跑动
			if (NavMashAgent.remainingDistance < 3) {
				Animator.SetBool ("Attack", true);
				NavMashAgent.Stop ();
			} else {
				Animator.SetBool ("Run", true);
			}
		}

		if (stateInfo.fullPathHash == Animator.StringToHash ("Base Layer.Damage") && !Animator.IsInTransition (0)) {

			Animator.SetBool ("Attack", false);
			Animator.SetBool ("Run", false);

			// 玩家有移动则重新检查，目前不会移动
			if (Vector3.Distance (TargetTransform.transform.position, NavMashAgent.destination) > 1) {
				NavMashAgent.SetDestination (TargetTransform.transform.position);
			}

			// 进入攻击距离则跳转到攻击动画，否则继续跑动
			if (NavMashAgent.remainingDistance < 3) {
				Animator.SetBool ("Attack", true);
				NavMashAgent.Stop ();
			} else {
				Animator.SetBool ("Run", true);
			}
		}
	}

	// 当被枪击中时调用
	public void UnderAttack ()
	{
		// 扣血并判断是否死亡，如果死亡则直接跳到死亡动画，否则跳到受伤动画
		HP--;

		if (HP <= 0) {
			Animator.Play ("Death");
			Destroy (GetComponent<Collider> ());
			Destroy (GetComponent<NavMeshAgent> ());
		} else {
			Animator.Play ("Damage");
		}
	}

	// 怪物攻击由动画事件调用
	public void Attack ()
	{
		Manager.Instance.UnderAttack ();
	}

}
