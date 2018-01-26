using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Transform = UnityEngine.Transform;
using DragonBones;


namespace WordJourney
{

	public delegate void SkillCallBack(BattleAgentController selfBaCtr,BattleAgentController enemyBaCtr);

	public class TriggeredSkillExcutor{
		public TriggeredSkill triggeredSkill;
		public SkillEffectTarget triggerSource;
		public SkillCallBack triggeredCallback;

		public TriggeredSkillExcutor(TriggeredSkill ts,SkillEffectTarget source,SkillCallBack callBack){
			this.triggeredSkill = ts;
			this.triggerSource = source;
			this.triggeredCallback = callBack;
		}
	}

	public abstract class BattleAgentController : MonoBehaviour {

		// 控制的角色
		[HideInInspector]public Agent agent;

		public AgentPropertyCalculator propertyCalculator;

		private ExploreUICotroller expUICtr;

		public BattleAgentController enemy;

		public bool isIdle;

		// 进入战斗前触发事件回调队列
		public List<TriggeredSkillExcutor> beforeFightTriggerExcutors = new List<TriggeredSkillExcutor>();

		// 角色攻击触发事件回调队列
		public List<TriggeredSkillExcutor> attackTriggerExcutors = new List<TriggeredSkillExcutor>();

		// 角色攻击命中触发的时间回调队列
		public List<TriggeredSkillExcutor> hitTriggerExcutors = new List<TriggeredSkillExcutor>();

		// 角色被攻击触发事件回调队列
		public List<TriggeredSkillExcutor> beAttackedTriggerExcutors = new List<TriggeredSkillExcutor>();

		// 角色被攻击命中触发的事件回调队列
		public List<TriggeredSkillExcutor> beHitTriggerExcutors = new List<TriggeredSkillExcutor>();

		// 战斗结束触发事件回调队列
		public List<TriggeredSkillExcutor> fightEndTriggerExcutors = new List<TriggeredSkillExcutor>();


		// 碰撞检测层
		public LayerMask collosionLayer;

		// 碰撞检测包围盒
		public BoxCollider2D boxCollider;

		// 激活的龙骨状态模型
		protected GameObject modelActive;

		// 自动攻击协程
		public IEnumerator attackCoroutine;
		// 等待角色动画结束协程
		public IEnumerator waitRoleAnimEndCoroutine;
		// 等待技能动画结束协程
		public List<IEnumerator> allSkillEffectReuseCoroutines = new List<IEnumerator>();

		protected ActiveSkill currentSkill;


		public List<ActiveSkill> activeSkills= new List<ActiveSkill>();

		[HideInInspector]public bool isAttackActionFinish;

		// 骨骼动画控制器
		protected UnityArmatureComponent armatureCom{
			get{
				return modelActive.GetComponent<UnityArmatureComponent> ();
			}
		}


		protected Transform mExploreManager;
		protected Transform exploreManager{
			get{
				if (mExploreManager == null) {
					mExploreManager = TransformManager.FindTransform ("ExploreManager");
				}
				return mExploreManager;
			}
		}


		public List<string> currentTriggeredEffectAnim = new List<string>();

		public MyTowards towards;

		public Transform effectAnimContainer;




		protected virtual void Awake(){

			boxCollider = GetComponent <BoxCollider2D> ();

			ListenerDelegate<EventObject> keyFrameListener = KeyFrameMessage;

			if (gameObject.tag == "monster") {
				armatureCom.AddEventListener (DragonBones.EventObject.FRAME_EVENT, keyFrameListener);
			} else if (gameObject.tag == "Player") {
				UnityArmatureComponent playerArmature = transform.Find ("PlayerSide").GetComponent<UnityArmatureComponent> ();
				playerArmature.AddEventListener(DragonBones.EventObject.FRAME_EVENT, keyFrameListener);
			}

			isIdle = true;

			propertyCalculator = new AgentPropertyCalculator ();

		}

		public virtual void SetSortingOrder(int order){
			armatureCom.sortingOrder = order;
		}

		public abstract void InitFightTextDirectionTowards (Vector3 position);

		protected ActiveSkill InteligentAttackSkill(){

			float seed = Random.Range (0, 1f);

			float max = 0;

			for (int i = 0; i < activeSkills.Count; i++) {

				max += activeSkills [i].probability;

				if (seed <= max) {
					return activeSkills [i];
				}

			}

			return null;

		}


		public void SetUpPropertyCalculator(){
			propertyCalculator.enemy = enemy;
			propertyCalculator.maxHealth = agent.maxHealth;
			propertyCalculator.health = agent.health;
			propertyCalculator.mana = agent.mana;
			propertyCalculator.attack = agent.attack;
			propertyCalculator.attackSpeed = agent.attackSpeed;
			propertyCalculator.hit = agent.hit;
			propertyCalculator.armor = agent.armor;
			propertyCalculator.magicResist = agent.magicResist;
			propertyCalculator.dodge = agent.dodge;
			propertyCalculator.crit = agent.crit;
			propertyCalculator.physicalHurtScaler = agent.physicalHurtScaler;
			propertyCalculator.magicalHurtScaler = agent.magicalHurtScaler;
			propertyCalculator.self = this;
			propertyCalculator.critHurtScaler = agent.critHurtScaler;
			propertyCalculator.dodgeFixScaler = agent.dodgeFixScaler;
			propertyCalculator.critFixScaler = agent.critFixScaler;
		}

		protected void KeyFrameMessage<T>(string key,T eventObject){

			EventObject frameObject = eventObject as EventObject;

			if (frameObject.name == "hit") {
				AgentExcuteHitEffect ();
//				Debug.LogFormat ("hit---{0}", agent.name);
			} else {
				Debug.LogError ("事件帧消息名称必须是hit");
			}

		}

		protected abstract void AgentExcuteHitEffect ();


		public abstract void TowardsLeft();
		public abstract void TowardsRight();

		/// <summary>
		/// 角色攻击间隔计时器
		/// </summary>
		/// <param name="skill">计时结束后使用的技能</param>
		protected IEnumerator InvokeAttack(ActiveSkill skill){

			float timePassed = 0;

			while (timePassed < agent.attackInterval) {

				timePassed += Time.deltaTime;

				yield return null;

			}

			UseSkill (skill);

		}

		public abstract bool CheckFightEnd ();

		public void PlayShakeAnim(){
			#warning 受击动画逻辑上有冲突，暂时先不使用受击动画
//			PlayRoleAnim ("hit", 1, null);
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

			float deltaX = 0.2f;

			float backwardSpeed = deltaX / backwardTime;
			float forwardSpeed = deltaX / forwardTime;

			Vector3 originPos = modelActive.transform.position;

			int directionSeed = armatureCom.armature.flipX ? -1 : 1;

			Vector3 targetPos = new Vector3 (modelActive.transform.position.x - deltaX * directionSeed, transform.position.y);

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
//			if (armatureCom.animation.lastAnimationName == "die") {
//				return;
//			}

			isIdle = animName == "wait";

			// 如果还有等待上个角色动作结束的协程存在，则结束该协程
			if (waitRoleAnimEndCoroutine != null) {
				StopCoroutine (waitRoleAnimEndCoroutine);
			}
				

			// 播放新的角色动画
			armatureCom.animation.Play (animName,playTimes);
				
			// 如果有角色动画结束后要执行的回调，则开启一个新的等待角色动画结束的协程，等待角色动画结束后执行回调
			if (cb != null) {
				waitRoleAnimEndCoroutine = ExcuteCallBackAtEndOfRoleAnim (cb);
				StartCoroutine(waitRoleAnimEndCoroutine);
			}
		}

		/// <summary>
		/// 设置角色特效动画，bool型触发器,适用于被动触发的技能
		/// </summary>
		/// <param name="animName">触发器名称</param>
		/// <param name="arg">bool型参数</param>
//		public void SetEffectAnim(string triggerName,bool arg){
//
//			if (triggerName != string.Empty) {
//
//				Transform skillEffect = null;
//
//				Animator skillEffectAnim = null;
//
//				SkillEffectInfo effectInfo = null;
//
//				if (skillEffectDic.ContainsKey (triggerName)) {
//
//					skillEffectAnim = skillEffectDic [triggerName].skillEffectTrans.GetComponent<Animator> ();
//
//					effectInfo = skillEffectDic [triggerName];
//
//				} else {
//
//					skillEffect = exploreManager.GetComponent<MapGenerator> ().GetSkillEffect (transform);
//
//					effectInfo = new SkillEffectInfo (triggerName, skillEffect);
//
//					skillEffectDic.Add (triggerName, effectInfo);
//
//					skillEffectAnim = skillEffect.GetComponent<Animator> ();
//
//				}
//
//				skillEffectAnim.SetBool (triggerName, arg);
//
//				if (!arg) {
//					exploreManager.GetComponent<MapGenerator> ().AddSkillEffectToPool (effectInfo.skillEffectTrans);
//				}
//			}
//		}

		/// <summary>
		/// 设置角色特效动画，trigger 型触发器
		/// </summary>
		/// <param name="animName">触发器名称</param>
		public void SetEffectAnim(string triggerName,CallBack cb = null){

			if(triggerName != string.Empty){

				Debug.LogFormat ("{0}触发技能特效{1}", agent.agentName, triggerName);

//				IEnumerator playEffectAnimCoroutine = LatelyPlayEffectAnim (triggerName, cb);
//
//				StartCoroutine (playEffectAnimCoroutine);
//
//			}
//		}
//
//		private IEnumerator LatelyPlayEffectAnim(string triggerName,CallBack cb){
//
//			yield return new WaitUntil (() => Time.timeScale == 1);
//
//			yield return null;

			Transform skillEffect = null;
			Animator skillEffectAnim = null;

			skillEffect = exploreManager.GetComponent<MapGenerator> ().GetEffectAnim (transform);

			skillEffectAnim = skillEffect.GetComponent<Animator> ();

			skillEffectAnim.transform.SetParent (effectAnimContainer);

			skillEffectAnim.SetTrigger (triggerName);

			IEnumerator skillEffectReuseCoroutine = AddSkillEffectToPoolAfterAnimEnd (skillEffect.transform,cb);

			allSkillEffectReuseCoroutines.Add (skillEffectReuseCoroutine);

			StartCoroutine (skillEffectReuseCoroutine);

		}
		}


		/// <summary>
		/// 技能特效动画结束后将特效显示器重置后（带SkillEffectAnimtor的游戏体）加入缓存池
		/// </summary>
		/// <returns>The skill effect to pool after animation end.</returns>
		/// <param name="effectInfo">Effect info.</param>
		protected IEnumerator AddSkillEffectToPoolAfterAnimEnd(Transform skillEffectTrans,CallBack cb){

			yield return null;

			Animator animator = skillEffectTrans.GetComponent<Animator> ();

			AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo (0);

//			Debug.Log (stateInfo.IsName ("Empty") + "----------------------");

			while (stateInfo.normalizedTime < 1) {
//				Debug.Log (stateInfo.normalizedTime);
				yield return null;
				stateInfo = animator.GetCurrentAnimatorStateInfo (0);
			}
//			yield return new WaitForSeconds (stateInfo.length);

			animator.SetTrigger ("Empty");

			yield return null;

			exploreManager.GetComponent<MapGenerator> ().AddEffectAnimToPool (animator.transform);

			if (cb != null) {
				cb ();
			}

//			skillEffectDic.Remove (effectInfo.triggerName);

//			animator.ResetTrigger (triggerName);

			Debug.LogFormat ("{0}回收技能特效", agent.agentName);

		}



		/// <summary>
		/// 伤害文本动画
		/// </summary>
		/// <param name="hurtStr">Hurt string.</param>
		/// <param name="tintTextType">伤害类型 [TintTextType.Crit:暴击伤害 ,TintTextType.Miss: 伤害闪避]</param>
		public abstract void AddFightTextToQueue (string hurtStr, SpecialAttackResult specialAttackType);


		/// <summary>
		/// 使用技能
		/// </summary>
		/// <param name="skill">Skill.</param>
		protected abstract void UseSkill (ActiveSkill skill);

		/// <summary>
		/// 角色默认的战斗方法
		/// </summary>
		public abstract void Fight ();

		public void UpdateFightStatus (){
			agent.ResetPropertiesWithPropertyCalculator (propertyCalculator);
			if (enemy != null) {
				if (enemy.propertyCalculator.physicalHurtToEnemy > 0) {
					string physicalHurt = string.Format ("<color=red>{0}</color>", enemy.propertyCalculator.physicalHurtToEnemy.ToString ());
					AddFightTextToQueue (physicalHurt, SpecialAttackResult.None);
				}
				if (enemy.propertyCalculator.magicalHurtToEnemy > 0) {
					string magicalHurt = string.Format ("<color=blue>{0}</color>", enemy.propertyCalculator.magicalHurtToEnemy.ToString ());
					AddFightTextToQueue (magicalHurt, SpecialAttackResult.None);
				}
			}

		}

		public void ResetAgent(){
			StopCoroutinesWhenFightEnd ();
			this.armatureCom.animation.Stop ();
			CancelInvoke ();
		}

		public abstract void AgentDie ();

		public virtual void StopCoroutinesWhenFightEnd (){
			
			if (attackCoroutine != null) {
				StopCoroutine (attackCoroutine);
			}

			if (waitRoleAnimEndCoroutine != null) {
				StopCoroutine (waitRoleAnimEndCoroutine);
			}

			for (int i = 0; i < allSkillEffectReuseCoroutines.Count; i++) {
				IEnumerator skillEffectReuseCoroutine = allSkillEffectReuseCoroutines [i];
				if (skillEffectReuseCoroutine != null) {
					StopCoroutine (skillEffectReuseCoroutine);
				}

			}

			AllEffectAnimsInfoPool ();

			allSkillEffectReuseCoroutines.Clear ();

			StopCoroutine ("PlayAgentShake");

//			modelActive.transform.localPosition = Vector3.zero;
		}

		private void AllEffectAnimsInfoPool(){

			for (int i = 0; i < effectAnimContainer.childCount; i++) {

				Transform effectAnim = effectAnimContainer.GetChild (i);

				exploreManager.GetComponent<MapGenerator> ().AddEffectAnimToPool (effectAnim);

				i--;

			}

		}

		public abstract void UpdateStatusPlane ();

		public void ExcuteBeforeFightSkillCallBacks(BattleAgentController enemy){
			for (int i = 0; i < beforeFightTriggerExcutors.Count; i++) {
				SkillCallBack cb = beforeFightTriggerExcutors [i].triggeredCallback;
				cb (this, enemy);
			}
		}

		public void ExcuteAttackSkillCallBacks(BattleAgentController enemy){
			for (int i = 0; i < attackTriggerExcutors.Count; i++) {
				SkillCallBack cb = attackTriggerExcutors [i].triggeredCallback;
				cb (this, enemy);
			}
		}

		public void ExcuteHitSkillCallBacks(BattleAgentController enemy){
			for (int i = 0; i < beHitTriggerExcutors.Count; i++) {
				SkillCallBack cb = beHitTriggerExcutors [i].triggeredCallback;
				cb (this, enemy);
			}
		}

		public void ExcuteBeAttackedSkillCallBacks(BattleAgentController enemy){
			for (int i = 0; i < beAttackedTriggerExcutors.Count; i++) {
				SkillCallBack cb = beAttackedTriggerExcutors [i].triggeredCallback;
				cb (this, enemy);
			}
		}

		public void ExcuteBeHitSkillCallBacks(BattleAgentController enemy){
			for (int i = 0; i < beHitTriggerExcutors.Count; i++) {
				SkillCallBack cb = beHitTriggerExcutors [i].triggeredCallback;
				cb (this, enemy);
			}
		}
			

		public void ExcuteFightEndCallBacks(BattleAgentController enemy){
			for (int i = 0; i < fightEndTriggerExcutors.Count; i++) {
				SkillCallBack cb = fightEndTriggerExcutors [i].triggeredCallback;
				cb (this, enemy);
			}
		}

		public void RemoveTriggeredSkillEffect(){
			agent.AddPropertyChangeFromOther (
				-propertyCalculator.maxHealthChangeFromTriggeredSkill,
				-propertyCalculator.hitChangeFromTriggeredSkill,
				-propertyCalculator.attackChangeFromTriggeredSkill,
				-propertyCalculator.attackSpeedChangeFromTriggeredSkill,
				-propertyCalculator.manaChangeFromTriggeredSkill,
				-propertyCalculator.armorChangeFromTriggeredSkill,
				-propertyCalculator.magicResistChangeFromTriggeredSkill,
				-propertyCalculator.dodgeChangeFromTriggeredSkill,
				-propertyCalculator.critChangeFromTriggeredSkill,
				-propertyCalculator.maxHealthChangeScalerFromTriggeredSkill,
				-propertyCalculator.hitChangeScalerFromTriggeredSkill,
				-propertyCalculator.attackChangeScalerFromTriggeredSkill,
				-propertyCalculator.attackSpeedChangeScalerFromTriggeredSkill,
				-propertyCalculator.manaChangeScalerFromTriggeredSkill,
				-propertyCalculator.armorChangeScalerFromTriggeredSkill,
				-propertyCalculator.magicResistChangeScalerFromTriggeredSkill,
				-propertyCalculator.dodgeChangeScalerFromTriggeredSkill,
				-propertyCalculator.critChangeScalerFromTriggeredSkill,
				-propertyCalculator.physicalHurtScalerChangeFromTriggeredSkill,
				-propertyCalculator.magicalHurtScalerChangeFromTriggeredSkill,
				-propertyCalculator.critChangeScalerFromTriggeredSkill);

		}

		public void ClearAllSkillCallBacks(){

			beforeFightTriggerExcutors.Clear ();
			attackTriggerExcutors.Clear ();
			hitTriggerExcutors.Clear ();
			beAttackedTriggerExcutors.Clear ();
			beHitTriggerExcutors.Clear ();
			fightEndTriggerExcutors.Clear ();

		}


		/// <summary>
		/// 清除角色身上所有的战斗回调和触发型技能效果
		/// </summary>
		public void ClearAllEffectStatesAndSkillCallBacks(){

			ClearAllSkillCallBacks ();

			propertyCalculator.ClearSkillsOfType<TriggeredSkill> ();
		}

		/// <summary>
		/// 等待角色动画完成后执行回调
		/// </summary>
		/// <returns>The call back at end of animation.</returns>
		/// <param name="cb">Cb.</param>
		protected IEnumerator ExcuteCallBackAtEndOfRoleAnim(CallBack cb){
//			while (!armatureCom.animation.isCompleted) {
//				Debug.LogFormat ("agent:{0},animName:{1},playtime:{2}", agent.agentName,armatureCom.animation.lastAnimationName, armatureCom.animation.lastAnimationState.currentTime);
//				yield return null;
//			}
			yield return new WaitUntil (() => armatureCom.animation.isCompleted);
			cb ();
		}



	}
}