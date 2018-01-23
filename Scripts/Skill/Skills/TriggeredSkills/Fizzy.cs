using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class Fizzy: TriggeredSkill {

//		public float duration;
//
//		public float probability;

		private Coroutine fizzyCoroutine;

		private BattleAgentController affectedAgent;
	

		protected override void BeforeFightTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			ExcuteTriggeredSkillLogic (beforeFightTriggerInfo, self, enemy);
		}

		protected override void AttackTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			ExcuteTriggeredSkillLogic (attackTriggerInfo, self, enemy);
		}

		protected override void HitTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			ExcuteTriggeredSkillLogic (hitTriggerInfo, self, enemy);
		}

		protected override void BeAttackedTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			ExcuteTriggeredSkillLogic (beAttackedTriggerInfo, self, enemy);
		}

		protected override void BeHitTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			ExcuteTriggeredSkillLogic (beHitTriggerInfo, self, enemy);
		}

		protected override void ExcuteTriggeredSkillLogic(TriggerInfo triggerInfo, BattleAgentController self,BattleAgentController enemy){

			if (isEffective (triggeredProbability)) {

				List<TriggeredSkill> fizzySkills = enemy.propertyCalculator.GetTriggeredSkillsWithSameStatus (statusName);

				affectedAgent = GetAffectedBattleAgent (triggerInfo, self, enemy);

				if (fizzySkills.Count > 0) {
					for (int i = 0; i < fizzySkills.Count; i++) {
						TriggeredSkill ts = fizzySkills [i];
						ts.CancelSkillEffect (ts != this);
					}
				} else {
					affectedAgent.propertyCalculator.SkillTriggered<TriggeredSkill> (this);
				}


				affectedAgent.PlayRoleAnim ("stun", 0, null);
				affectedAgent.isAttackActionFinish = false;

				fizzyCoroutine = StartCoroutine ("FizzyForDuration",affectedAgent);

				SetEffectAnims (triggerInfo, self, enemy);
			}

		}
			
		private IEnumerator FizzyForDuration(BattleAgentController affectedAgent){
			yield return new WaitForSeconds (duration);
			affectedAgent.isAttackActionFinish = true;
			affectedAgent.propertyCalculator.RemoveAttachedSkill<TriggeredSkill> (this);
			affectedAgent.Fight ();
		}

		protected override void FightEndTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			CancelSkillEffect (true);
		}

		public override void CancelSkillEffect (bool removeSkill)
		{
			if (fizzyCoroutine != null) {
				StopCoroutine (fizzyCoroutine);

			}

			if (affectedAgent != null && removeSkill) {
				affectedAgent.propertyCalculator.RemoveAttachedSkill<TriggeredSkill> (this);
			}

		}

	}
}