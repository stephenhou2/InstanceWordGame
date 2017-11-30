using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Transform = UnityEngine.Transform;
using DragonBones;


namespace WordJourney
{

	public delegate void SkillCallBack(BattleAgentController selfBaCtr,BattleAgentController enemyBaCtr);

	public abstract class BattleAgentController : MonoBehaviour {

		// 控制的角色
		[HideInInspector]public Agent agent;



		// 角色攻击触发的技能
//		public List<Skill> attackTriggerSkill = new List<Skill> ();
//
//		// 角色被攻击触发的技能
//		public List<Skill> beAttackedTriggerSkill = new List<Skill>();
//
//		// 角色死亡触发的技能
//		public List<Skill> agentDieTriggerSkill = new List<Skill> ();
//
//		// 角色完成一次攻击触发事件回调队列
//		public List<SkillCallBack> attackFinishTriggerCallBacks = new List<SkillCallBack> ();

		// 进入战斗前触发事件回调队列
		public List<SkillCallBack> beforeFightTriggerCallBacks = new List<SkillCallBack>();

		// 角色攻击触发事件回调队列
		public List<SkillCallBack> attackTriggerCallBacks = new List<SkillCallBack>();

		// 角色完成一次攻击触发事件回调队列
		public List<SkillCallBack> attackFinishTriggerCallBacks = new List<SkillCallBack> ();

		// 角色被攻击触发事件回调队列
		public List<SkillCallBack> beAttackedTriggerCallBacks = new List<SkillCallBack>();

		// 战斗结束触发事件回调队列
		public List<SkillCallBack> fightEndTriggerCallBacks = new List<SkillCallBack>();

		// 角色身上的所有状态
		public List<SkillState> states = new List<SkillState>();

//		// 角色身上的所有状态名称
//		public List<string> states = new List<string>();
//		// 角色身上所有状态协程
//		public List<IEnumerator> stateEffectCoroutines = new List<IEnumerator> ();

		// 碰撞检测层
		public LayerMask collosionLayer;

		// 碰撞检测包围盒
		protected BoxCollider2D boxCollider;
	
		// 激活的龙骨状态模型
		protected GameObject modelActive;

		// 自动攻击协程
		public IEnumerator attackCoroutine;
		// 等待角色动画结束协程
		public IEnumerator waitRoleAnimEndCoroutine;
		// 等待技能动画结束协程
		public IEnumerator waitEffectAnimEndCoroutine;

		protected Skill currentSkill;

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

		// 技能特效信息类
		protected class SkillEffectInfo{

			// 特效游戏体的transform
			public Transform skillEffectTrans;
			// 特效名称
			public string triggerName;

			public SkillEffectInfo(string triggerName,Transform skillEffectTrans){
				this.skillEffectTrans = skillEffectTrans;
				this.triggerName = triggerName;
			}
		}

		// 角色身上仍然在播放中的技能特效
		protected Dictionary<string,SkillEffectInfo> skillEffectDic = new Dictionary<string,SkillEffectInfo> ();

		public List<string> currentTriggeredEffectAnim = new List<string>();


		protected virtual void Awake(){

			boxCollider = GetComponent <BoxCollider2D> ();

			ListenerDelegate<EventObject> keyFrameListener = KeyFrameMessage;

			if (gameObject.tag == "monster") {
				armatureCom.AddEventListener (DragonBones.EventObject.FRAME_EVENT, keyFrameListener);
			} else if (gameObject.tag == "Player") {
				UnityArmatureComponent playerArmature = transform.Find ("PlayerSide").GetComponent<UnityArmatureComponent> ();
				playerArmature.AddEventListener(DragonBones.EventObject.FRAME_EVENT, keyFrameListener);
			}

		}


		protected void KeyFrameMessage<T>(string key,T eventObject){

			EventObject frameObject = eventObject as EventObject;

			if (frameObject.name == "hit") {
				AgentExcuteHitEffect ();

			} else {
				Debug.LogError ("事件帧消息名称必须是hit");
			}

		}

		protected abstract void AgentExcuteHitEffect ();


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

		public void PlayShakeAnim(){
			StartCoroutine ("PlayAgentShake");
		}

		/// <summary>
		/// 角色被攻击时的抖动动画
		/// </summary>
		/// <returns>The agent shake.</returns>
		private IEnumerator PlayAgentShake(){

			float backwardTime = 0.1f;
			float forwardTime = 0.1f;

			float timer = 0f;

			float deltaX = 0.1f;

			float backwardSpeed = deltaX / backwardTime;
			float forwardSpeed = deltaX / forwardTime;

			Vector3 originPos = modelActive.transform.position;

			Vector3 targetPos = new Vector3 (modelActive.transform.position.x - deltaX * modelActive.transform.localScale.x, transform.position.y);

			while (timer < backwardTime) {

				modelActive.transform.position = Vector3.MoveTowards (modelActive.transform.position, targetPos, backwardSpeed * Time.deltaTime);

				timer += Time.deltaTime;

				yield return null;
			}

			timer = 0f;

			while (timer < forwardTime) {

				modelActive.transform.position = Vector3.MoveTowards (modelActive.transform.position, originPos, forwardSpeed * Time.deltaTime);

				timer += Time.deltaTime;

				yield return null;

			}


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
				waitRoleAnimEndCoroutine = ExcuteCallBackAtEndOfRoleAnim (cb);
//				waitRoleAnimEndCoroutine = StartCoroutine ("ExcuteCallBackAtEndOfRoleAnim", cb);
				StartCoroutine(waitRoleAnimEndCoroutine);
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

				waitEffectAnimEndCoroutine = AddSkillEffectToPoolAfterAnimEnd (effectInfo);

				StartCoroutine (waitEffectAnimEndCoroutine);

//				waitEffectAnimEndCoroutine = StartCoroutine ("AddSkillEffectToPoolAfterAnimEnd", effectInfo);


			}
		}
			

		/// <summary>
		/// 技能特效动画结束后将特效显示器重置后（带SkillEffectAnimtor的游戏体）加入缓存池
		/// </summary>
		/// <returns>The skill effect to pool after animation end.</returns>
		/// <param name="effectInfo">Effect info.</param>
		protected IEnumerator AddSkillEffectToPoolAfterAnimEnd(SkillEffectInfo effectInfo){

			yield return null;

			Animator animator = effectInfo.skillEffectTrans.GetComponent<Animator> ();

			AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo (0);

			while (stateInfo.normalizedTime < 1.0f) {
				yield return null;
				stateInfo = animator.GetCurrentAnimatorStateInfo (0);
			}

			exploreManager.GetComponent<MapGenerator> ().AddSkillEffectToPool (animator.transform);

			skillEffectDic.Remove (effectInfo.triggerName);

		}

		/// <summary>
		/// 回收技能特效组件
		/// </summary>
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

		private struct TintTextAnimInfo
		{
			public string tintTextStr;
			public TintTextType tintTextType;
			public float delay;

			public TintTextAnimInfo(string tintTextStr,TintTextType tintTextType,float delay){
				this.tintTextStr = tintTextStr;
				this.tintTextType = tintTextType;
				this.delay = delay;
			}
		}

		/// <summary>
		/// 延迟一段事件后播放文本动画
		/// </summary>
		/// <param name="hurtStr">Hurt string.</param>
		/// <param name="tintTextType">Tint text type.</param>
		/// <param name="delay">Delay.</param>
		public void PlayHurtTextAnim(string hurtStr,TintTextType tintTextType,float delay){
			TintTextAnimInfo info = new TintTextAnimInfo(hurtStr, tintTextType, delay);
			StartCoroutine ("LatelyPlayHurtTextAnim", info);
		}

		private IEnumerator LatelyPlayHurtTextAnim(TintTextAnimInfo info){

			yield return new WaitForSeconds (info.delay);

			PlayHurtTextAnim (info.tintTextStr, info.tintTextType);

		}

		/// <summary>
		/// 血量提升文本动画
		/// </summary>
		/// <param name="gainStr">Gain string.</param>
		public abstract void PlayGainTextAnim(string gainStr);

		public void PlayGainTextAnim(string gainStr,float delay){
			TintTextAnimInfo info = new TintTextAnimInfo(gainStr, TintTextType.None, delay);
			StartCoroutine ("LatelyPlayGainTextAnim", info);
		}

		private IEnumerator LatelyPlayGainTextAnim(TintTextAnimInfo info){
			
			yield return new WaitForSeconds (info.delay);

			PlayGainTextAnim (info.tintTextStr);
		}

		/// <summary>
		/// 使用技能
		/// </summary>
		/// <param name="skill">Skill.</param>
		protected abstract void UseSkill (Skill skill);

		/// <summary>
		/// 角色默认的战斗方法
		/// </summary>
		public abstract void Fight ();

		public abstract void AgentDie ();


		public void ExcuteBeforeFightSkillCallBacks(BattleAgentController enemy){
			for (int i = 0; i < beforeFightTriggerCallBacks.Count; i++) {
				SkillCallBack cb = beforeFightTriggerCallBacks [i];
				cb (this, enemy);
			}
		}

		public void ExcuteAttackSkillCallBacks(BattleAgentController enemy){
			for (int i = 0; i < attackTriggerCallBacks.Count; i++) {
				SkillCallBack cb = attackTriggerCallBacks [i];
				cb (this, enemy);
			}
		}

		public void ExcuteBeAttackedSkillCallBacks(BattleAgentController enemy){
			for (int i = 0; i < beAttackedTriggerCallBacks.Count; i++) {
				SkillCallBack cb = beAttackedTriggerCallBacks [i];
				cb (this, enemy);
			}
		}

		public void ExcuteAttackFinishSkillCallBacks(BattleAgentController enemy){
			for (int i = 0; i < attackFinishTriggerCallBacks.Count; i++) {
				SkillCallBack cb = attackFinishTriggerCallBacks [i];
				cb (this, enemy);
			}
		}

		public void ExcuteFightEndCallBacks(BattleAgentController enemy){
			for (int i = 0; i < fightEndTriggerCallBacks.Count; i++) {
				SkillCallBack cb = fightEndTriggerCallBacks [i];
				cb (this, enemy);
			}
		}

		/// <summary>
		/// 添加技能状态到角色身上
		/// </summary>
		/// <param name="state">State.</param>
		public void AddState(SkillState state){
			states.Add (state);

		}


		/// <summary>
		/// 删除角色身上的技能状态
		/// </summary>
		/// <param name="state">State.</param>
		public void RemoveState(SkillState state){
			states.Remove (state);

		}

		public bool CheckStateExist(string stateName){
			bool result = false;
			for (int i = 0; i < states.Count; i++) {
				if (states [i].stateName == stateName) {
					result = true;
					break;
				}
			}
			return result;
		}

		/// <summary>
		/// 清除角色身上所有的非持续性效果状态
		/// </summary>
		public void ClearAllEffectStatesAndSkillCallBacks(){

			beforeFightTriggerCallBacks.Clear ();
			attackTriggerCallBacks.Clear ();
			attackFinishTriggerCallBacks.Clear ();
			beAttackedTriggerCallBacks.Clear ();
			fightEndTriggerCallBacks.Clear ();

			states.Clear ();
		}

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
