using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
		
	public class PropertyChangeDurative : TriggeredSkill {


		public SkillEffectTarget effectTarget;

		public PropertyType propertyType;

		// 默认退出战斗时，技能效果仍会持续性触发直至结束
//		private bool removeWhenQuitFight = false;

//		private float originalProperty;//记录受影响方的原始属性值，战斗结束后重置该属性
		private float propertyChange;// 记录受影响方属性变化值，如果效果时长有限制（duration>0)，则效果结束后重置该属性

		private BattleAgentController affectedAgent;

		private IEnumerator effectCoroutine;

//		private List<IEnumerator> effectCoroutines = new List<IEnumerator>();

//		private SkillState state;

//		void Awake ()
//		{
//			originalProperty = 0;
//		}
			

		protected override void ExcuteNoneTriggeredSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{

			propertyChange = 0;
			// 首次触发前记录受影响者
			if (beforeFightTriggerInfo.triggered) {
				affectedAgent = GetAffectedBattleAgent (beforeFightTriggerInfo, self, enemy);
			} else if (attackTriggerInfo.triggered) {
				affectedAgent = GetAffectedBattleAgent (attackTriggerInfo, self, enemy);
			}else if (hitTriggerInfo.triggered) {
				affectedAgent = GetAffectedBattleAgent (hitTriggerInfo, self, enemy);
			}else if (beAttackedTriggerInfo.triggered) {
				affectedAgent = GetAffectedBattleAgent (beAttackedTriggerInfo, self, enemy);
			}else if (beHitTriggerInfo.triggered) {
				affectedAgent = GetAffectedBattleAgent (beHitTriggerInfo, self, enemy);
			}
//			originalProperty = affectedAgent.propertyCalculator.GetAgentPropertyWithType (propertyType);

		}

		protected override void BeforeFightTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			ExcuteTriggeredSkillLogic (beforeFightTriggerInfo,self, enemy);
		}

		protected override void AttackTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			ExcuteTriggeredSkillLogic (attackTriggerInfo,self, enemy);
		}

		protected override void HitTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			ExcuteTriggeredSkillLogic (hitTriggerInfo,self, enemy);
		}

		protected override void BeAttackedTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			ExcuteTriggeredSkillLogic (beAttackedTriggerInfo,self, enemy);
		}

//		protected override void AttackFinishTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
//		{
//			CheckSkillEffectiveAndSetAnim (self, enemy);
//		}
			

		protected override void BeHitTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			ExcuteTriggeredSkillLogic (beHitTriggerInfo,self, enemy);
		}

		protected override void FightEndTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			if (effectCoroutine != null) {
				StopCoroutine (effectCoroutine);
				effectCoroutine = null;
			}

			// 如果状态变化是攻击，攻速，护甲，抗性，闪避，暴击，魔法，在状态结束后将属性重置为初始值
			if (propertyType == PropertyType.Health) {
				return;
			}

//			affectedAgent.propertyCalculator.AgentPropertySetToValue (propertyType, originalProperty);
			affectedAgent.propertyCalculator.AgentPropertyChange (propertyType, -skillSourceValue);
			affectedAgent.propertyCalculator.RemoveTriggeredSkill (this);
			affectedAgent = null;

		}

		protected override void ExcuteTriggeredSkillLogic(TriggerInfo triggerInfo,BattleAgentController self, BattleAgentController enemy){

			// 判断被动是否生效
			if (!isEffective (triggeredProbability)) {
				return;
			}

			// 如果没有状态名称，则默认不是触发状态，直接执行技能
			if (statusName == "") {
				Excute ();
				return;
			}

			List<TriggeredSkill> sameStatusSkills = affectedAgent.propertyCalculator.GetTriggeredSkillsWithSameStatus (statusName);

			// 如果技能效果不可叠加，则被影响人身上原有的同种状态技能效果全部取消，并从战斗结算器中移除这些技能
			if (!canOverlay) {
				for (int i = 0; i < sameStatusSkills.Count; i++) {
					TriggeredSkill ts = sameStatusSkills [i];
					ts.CancelSkillEffect ();
				}
			}

			Excute ();

		}


		private IEnumerator ExcuteAgentPropertyChange(BattleAgentController ba){

			float timer = 0f;

			while (timer < duration) {

				ba.propertyCalculator.InstantPropertyChange (ba, propertyType, skillSourceValue);

				propertyChange += skillSourceValue;

				timer += 1f;

				yield return new WaitForSeconds (1.0f);

			}

			ba.propertyCalculator.RemoveTriggeredSkill (this);
		}

		public override void CancelSkillEffect ()
		{
			if (effectCoroutine != null) {
				StopCoroutine (effectCoroutine);
			}

			affectedAgent.propertyCalculator.RemoveTriggeredSkill (this);
			affectedAgent = null;

			if (propertyType == PropertyType.Health) {
				return;
			}
			affectedAgent.propertyCalculator.AgentPropertyChange (propertyType, -propertyChange);

		}

		private void Excute(){

			// 创建持续性状态的执行协程
			effectCoroutine = ExcuteAgentPropertyChange (affectedAgent);
			// 开启持续性状态的协程
			StartCoroutine (effectCoroutine);

			affectedAgent.propertyCalculator.AddSkill<TriggeredSkill> (this);

		}

	}
}
