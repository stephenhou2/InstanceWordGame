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
		public Skill defaultSkill;

		// 角色攻击触发事件回调队列
		public List<CallBack> attackTriggerCallBacks = new List<CallBack>();

		// 角色被攻击触发事件回调队列
		public List<CallBack> beAttackedTriggerCallBacks = new List<CallBack>();

		// 碰撞检测层
		public LayerMask collosionLayer;

		// 碰撞检测包围盒
		protected BoxCollider2D boxCollider;
	
		// 激活的龙骨状态
		protected GameObject modelActive;

		// 自动攻击协程
		public Coroutine attackCoroutine;
		// 等待角色动画结束协程
		public Coroutine waitRoleAnimEndCoroutine;
		// 等待技能动画结束协程
		public Coroutine waitEffectAnimEndCoroutine;

		// 骨骼动画控制器
		protected UnityArmatureComponent armatureCom{
			get{
				return modelActive.GetComponent<UnityArmatureComponent> ();
			}
		}

		// 攻击特效动画控制器缓存池
		protected Transform mExploreManager;
		protected Transform exploreManager{
			get{
				if (mExploreManager == null) {
					mExploreManager = TransformManager.FindTransform ("ExploreManager");
				}
				return mExploreManager;
			}
		}

		protected class SkillEffectInfo{
			public Transform skillEffectTrans;
			public string triggerName;

			public SkillEffectInfo(string triggerName,Transform skillEffectTrans){
				this.skillEffectTrans = skillEffectTrans;
				this.triggerName = triggerName;
			}
		}

		protected Dictionary<string,SkillEffectInfo> skillEffectDic = new Dictionary<string,SkillEffectInfo> ();


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
		/// 播放角色动画方法.
		/// </summary>
		/// <param name="animName">播放的动画名称</param>
		/// <param name="playTimes">播放次数 [-1: 使用动画数据默认值, 0: 无限循环播放, [1~N]: 循环播放 N 次]</param>
		/// <param name="cb">动画完成回调.</param>
		public void PlayRoleAnim (string animName, int playTimes, CallBack cb)
		{
			// 播放新的角色动画
			armatureCom.animation.Play (animName,playTimes);

			// 如果还有等待上个角色动作结束的协程存在，则结束该协程
			if (waitRoleAnimEndCoroutine != null) {
				StopCoroutine (waitRoleAnimEndCoroutine);
			}

			// 如果有角色动画结束后要执行的回调，则开启一个新的等待角色动画结束的协程，等待角色动画结束后执行回调
			if (cb != null) {
				waitRoleAnimEndCoroutine = StartCoroutine ("ExcuteCallBackAtEndOfRoleAnim", cb);
			}
		}

		/// <summary>
		/// 设置角色特效动画，bool型触发器,适用于被动触发的技能
		/// </summary>
		/// <param name="animName">触发器名称</param>
		/// <param name="arg">bool型参数</param>
		public void SetEffectAnim(string triggerName,bool arg){
			
			if (triggerName != string.Empty) {
				
				Transform skillEffect = null;

				Animator skillEffectAnim = null;

				SkillEffectInfo effectInfo = null;

				if (skillEffectDic.ContainsKey (triggerName)) {
					
					skillEffectAnim = skillEffectDic [triggerName].skillEffectTrans.GetComponent<Animator> ();

					effectInfo = skillEffectDic [triggerName];
				
				} else {

					skillEffect = exploreManager.GetComponent<MapGenerator> ().GetSkillEffect (this.transform);

					effectInfo = new SkillEffectInfo (triggerName, skillEffect);

					skillEffectDic.Add (triggerName, effectInfo);

					skillEffect.localPosition = Vector3.zero;
					skillEffect.localRotation = Quaternion.identity;
					skillEffect.localScale = Vector3.one;

					skillEffectAnim = skillEffect.GetComponent<Animator> ();
				
				}

				skillEffectAnim.SetBool (triggerName, arg);

				if (!arg) {
					exploreManager.GetComponent<MapGenerator> ().AddSkillEffectToPool (effectInfo.skillEffectTrans);
				}
			}
		}

		/// <summary>
		/// 设置角色特效动画，string 型触发器
		/// </summary>
		/// <param name="animName">触发器名称</param>
		public void SetEffectAnim(string triggerName){
			
			if(triggerName != string.Empty){

				Transform skillEffect = null;
				Animator skillEffectAnim = null;
				SkillEffectInfo effectInfo = null;

				if (skillEffectDic.ContainsKey (triggerName)) {
					
					skillEffectAnim = skillEffectDic [triggerName].skillEffectTrans.GetComponent<Animator> ();

					effectInfo = skillEffectDic [triggerName];

					if (waitEffectAnimEndCoroutine != null) {
						StopCoroutine (waitEffectAnimEndCoroutine);
					}

				} else {

					skillEffect = exploreManager.GetComponent<MapGenerator> ().GetSkillEffect (this.transform);

					effectInfo = new SkillEffectInfo (triggerName, skillEffect);

					skillEffectDic.Add (triggerName, effectInfo);

					skillEffect.localPosition = Vector3.zero;
					skillEffect.localRotation = Quaternion.identity;
					skillEffect.localScale = Vector3.one;

					skillEffectAnim = skillEffect.GetComponent<Animator> ();

				}

				skillEffectAnim.SetTrigger (triggerName);

//				if (gameObject.activeInHierarchy) {
				waitEffectAnimEndCoroutine = StartCoroutine ("AddSkillEffectToPoolAfterAnimEnd", effectInfo);
//				}

			}
		}
			

		/// <summary>
		/// 技能特效动画结束后将特效显示器重置后（带SkillEffectAnimtor的游戏体）加入缓存池
		/// </summary>
		/// <returns>The skill effect to pool after animation end.</returns>
		/// <param name="effectInfo">Effect info.</param>
		protected IEnumerator AddSkillEffectToPoolAfterAnimEnd(SkillEffectInfo effectInfo){

			Animator animator = effectInfo.skillEffectTrans.GetComponent<Animator> ();

			AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo (0);

			float time = 0;

			while (time < stateInfo.length) {
				yield return null;
				time += Time.deltaTime;

			}

			animator.SetTrigger ("Empty");

			exploreManager.GetComponent<MapGenerator> ().AddSkillEffectToPool (animator.transform);

			skillEffectDic.Remove (effectInfo.triggerName);

		}

		protected void CollectSkillEffectsToPool(){

			string[] keys = new string[skillEffectDic.Keys.Count];

			int i = 0;

			IDictionaryEnumerator enumerator = skillEffectDic.GetEnumerator ();

			while (enumerator.MoveNext ()) {

				keys [i] = enumerator.Key as string;

				i++;

			}

			for (int j = 0; j < keys.Length; j++) {

				string key = keys [j];

				Transform effectTrans = skillEffectDic[key].skillEffectTrans;

				effectTrans.GetComponent<Animator> ().SetTrigger ("Empty");

				exploreManager.GetComponent<MapGenerator> ().AddSkillEffectToPool (effectTrans);

				skillEffectDic.Remove (key);

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
		/// 角色默认的战斗方法
		/// </summary>
		public abstract void Fight ();





		/// <summary>
		/// 等待角色动画完成后执行回调
		/// </summary>
		/// <returns>The call back at end of animation.</returns>
		/// <param name="cb">Cb.</param>
		protected IEnumerator ExcuteCallBackAtEndOfRoleAnim(CallBack cb){
			yield return new WaitUntil (() => armatureCom.animation.isCompleted);
			cb ();
		}
	}
}
