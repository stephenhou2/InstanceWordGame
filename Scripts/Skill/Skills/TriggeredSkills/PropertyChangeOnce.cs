using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{


	public class PropertyChangeOnce : TriggeredSkill {

//		private float originalProperty;//记录受影响方的原始属性值，战斗结束后重置该属性
		private float propertyChange;// 记录受影响方属性变化值，如果效果时长有限制（duration>0)，则效果结束后重置该属性

//		public float timer;//技能效果计时器

		private BattleAgentController affectedAgent;

		public PropertyType propertyType;

		protected IEnumerator ResetPropertyCoroutine;


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

			// 先记录增长前的属性状态
//			originalProperty = affectedAgent.propertyCalculator.GetAgentPropertyWithType(propertyType);

		}


		protected override void BeforeFightTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			ExcuteTriggeredSkillLogic (beforeFightTriggerInfo,self, enemy);
		}

		protected override void AttackTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			ExcuteTriggeredSkillLogic (attackTriggerInfo,self, enemy);
		}

		protected override void HitTriggerCallBack(BattleAgentController self,BattleAgentController enemy){
			ExcuteTriggeredSkillLogic (hitTriggerInfo,self, enemy);
		}

		protected override void BeAttackedTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			ExcuteTriggeredSkillLogic (beAttackedTriggerInfo,self, enemy);
		}

		protected override void BeHitTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			ExcuteTriggeredSkillLogic (beHitTriggerInfo,self, enemy);
		}
			
//		protected override void AttackFinishTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
//		{
//			CheckSkillEffectiveAndSetAnim (self, enemy);
//		}

		protected override void ExcuteTriggeredSkillLogic(TriggerInfo triggerInfo, BattleAgentController self, BattleAgentController enemy){

			bool skillEffective = isEffective (triggeredProbability);

			if (!skillEffective) {
				return;
			}

			SetEffectAnims (triggerInfo, self, enemy);

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

			// 执行技能
			Excute ();

			// 播放技能对应的技能特效动画
//			SetEffectAnims(triggerInfo,self,enemy);
		}

		/// <summary>
		/// 执行技能，如果技能有时效性，则在时效结束时重置人物相应属性
		/// </summary>
		private void Excute(){
			affectedAgent.propertyCalculator.AgentPropertyChange (propertyType, skillSourceValue);
			propertyChange += skillSourceValue;
			if (duration > 0) {
				ResetPropertyCoroutine = ResetPropertyWhenTimeOut (affectedAgent, propertyType, duration);
				StartCoroutine (ResetPropertyCoroutine);
			}
			affectedAgent.propertyCalculator.AddSkill<TriggeredSkill> (this);
		}

		/// <summary>
		/// 时效结束时重置人物相应属性
		/// </summary>
		/// <returns>The property when time out.</returns>
		/// <param name="affectedAgent">Affected agent.</param>
		/// <param name="propertyType">Property type.</param>
		/// <param name="duration">Duration.</param>
		private IEnumerator ResetPropertyWhenTimeOut(BattleAgentController affectedAgent,PropertyType propertyType,float duration){
			yield return new WaitForSeconds (duration);
			affectedAgent.propertyCalculator.InstantPropertyChange (affectedAgent,propertyType, -skillSourceValue);
			affectedAgent.propertyCalculator.RemoveAttachedSkill<TriggeredSkill> (this);
			affectedAgent = null;

		}

		/// <summary>
		/// 取消技能效果
		/// </summary>
		/// <returns><c>true</c> if this instance cancel skill effect; otherwise, <c>false</c>.</returns>
		public override void CancelSkillEffect ()
		{
			if (ResetPropertyCoroutine != null) {
				StopCoroutine (ResetPropertyCoroutine);
			}
			affectedAgent.propertyCalculator.RemoveAttachedSkill<TriggeredSkill> (this);

			if (propertyType == PropertyType.Health) {
				return;
			}

			affectedAgent.propertyCalculator.InstantPropertyChange (affectedAgent ,propertyType, -skillSourceValue);

		}

		/// <summary>
		/// Fights the end trigger call back.
		/// </summary>
		protected override void FightEndTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			


			if (ResetPropertyCoroutine != null) {
				StopCoroutine (ResetPropertyCoroutine);
				ResetPropertyCoroutine = null;
			}



			if (propertyType == PropertyType.Health) {
				affectedAgent = null;
				return;
			}


//			affectedAgent.propertyCalculator.AgentPropertySetToValue (propertyType, originalProperty);
			affectedAgent.propertyCalculator.AgentPropertyChange (propertyType, -propertyChange);
			affectedAgent.propertyCalculator.RemoveAttachedSkill<TriggeredSkill> (this);

			affectedAgent = null;

		}



		
	}
}
