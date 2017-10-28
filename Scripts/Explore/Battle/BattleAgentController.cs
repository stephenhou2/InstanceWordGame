using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Transform = UnityEngine.Transform;
using DragonBones;


namespace WordJourney
{
	public abstract class BattleAgentController : MonoBehaviour {

		// 控制的角色
		[HideInInspector]public Agent agent;

		// 物理攻击技能（角色自带）
		public Skill physicalAttack;

		// 角色攻击触发事件回调队列
		public List<CallBack> attackTriggerCallBacks = new List<CallBack>();

		// 角色被攻击触发事件回调队列
		public List<CallBack> beAttackedTriggerCallBacks = new List<CallBack>();

		// 碰撞检测层
		public LayerMask collosionLayer;			//Layer on which collision will be checked.

		// 碰撞检测包围盒
		protected BoxCollider2D boxCollider;
	
		protected GameObject modelActive;

		// 骨骼动画控制器
		protected UnityArmatureComponent armatureCom{
			get{
				return modelActive.GetComponent<UnityArmatureComponent> ();
			}
		}

		// 攻击特效动画控制器
		protected Animator mEffectAnimaor;
		protected Animator effectAnimator{
			get{
				if (mEffectAnimaor == null) {
					mEffectAnimaor = modelActive.GetComponentInChildren<Animator> ();
				}
				return mEffectAnimaor;
			}
		}



		protected virtual void Awake(){

			boxCollider = GetComponent <BoxCollider2D> ();
		}


		/// <summary>
		/// 角色攻击间隔计时器
		/// </summary>
		/// <param name="skill">计时结束后使用的技能</param>
		protected IEnumerator InvokeAttack(Skill skill){

			float timePassed = 0;

			while (timePassed < agent.attackInterval) {

				timePassed += Time.deltaTime;

				yield return null;

			}

			UseSkill (skill);

		}

		/// <summary>
		/// 播放动画方法.
		/// </summary>
		/// <param name="animName">播放的动画名称</param>
		/// <param name="playTimes">播放次数 [-1: 使用动画数据默认值, 0: 无限循环播放, [1~N]: 循环播放 N 次]</param>
		/// <param name="cb">动画完成回调.</param>
		public virtual void PlayRoleAnim(string animName,int playTimes,CallBack cb = null){
			armatureCom.animation.Play (animName,playTimes);
			if (cb != null) {
				StartCoroutine ("ExcuteCallBackAtEndOfAnim", cb);
			}

		}

		/// <summary>
		/// 设置角色特效动画，bool型触发器
		/// </summary>
		/// <param name="animName">触发器名称</param>
		/// <param name="arg">bool型参数</param>
		public void SetEffectAnim(string triggerName,bool arg){
			
			if (triggerName != string.Empty) {
				effectAnimator.SetBool (triggerName, arg);
			}
		}

		/// <summary>
		/// 设置角色特效动画，string 型触发器
		/// </summary>
		/// <param name="animName">触发器名称</param>
		public void SetEffectAnim(string triggerName){
			if(triggerName != string.Empty){
				effectAnimator.SetTrigger(triggerName);
			}
		}

		/// <summary>
		/// 设置角色特效动画，int 型触发器
		/// </summary>
		/// <param name="triggerName">触发器名称</param>
		/// <param name="arg">int 型参数</param>
		public void SetEffectAnim(string triggerName,int arg){
			if (triggerName != string.Empty) {
				effectAnimator.SetInteger (triggerName, arg);
			}
		}

		/// <summary>
		/// 伤害文本动画
		/// </summary>
		/// <param name="hurtStr">Hurt string.</param>
		/// <param name="tintTextType">伤害类型 [TintTextType.Crit:暴击伤害 ,TintTextType.Miss: 伤害闪避]</param>
		public abstract void PlayHurtTextAnim (string hurtStr, TintTextType tintTextType);

		/// <summary>
		/// 血量提升文本动画
		/// </summary>
		/// <param name="gainStr">Gain string.</param>
		public abstract void PlayGainTextAnim(string gainStr);


		protected abstract void UseSkill (Skill skill);

		/// <summary>
		/// 角色战斗逻辑
		/// </summary>
		public abstract void Fight ();


		/// <summary>
		/// 等待角色动画完成后执行回调
		/// </summary>
		/// <returns>The call back at end of animation.</returns>
		/// <param name="cb">Cb.</param>
		private IEnumerator ExcuteCallBackAtEndOfAnim(CallBack cb){
			yield return new WaitUntil (() => armatureCom.animation.isCompleted);
			cb ();
		}
	}
}
