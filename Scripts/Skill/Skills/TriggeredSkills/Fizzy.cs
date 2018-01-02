using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class Fizzy: TriggeredSkill {

//		public float duration;
//
//		public float probability;

		private Coroutine hardBeatCoroutine;
	

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
				if (hardBeatCoroutine != null) {
					StopCoroutine (hardBeatCoroutine);
				}

				BattleAgentController affectedAgent = GetAffectedBattleAgent (triggerInfo, self, enemy);

				affectedAgent.PlayRoleAnim ("stun", 0, null);

				affectedAgent.SetEffectAnim (enemyEffectAnimName, true);

				hardBeatCoroutine = StartCoroutine ("FizzyForDuration",affectedAgent);
			}

		}
			
		private IEnumerator FizzyForDuration(BattleAgentController affectedAgent){
			yield return new WaitForSeconds (duration);
			affectedAgent.SetEffectAnim (enemyEffectAnimName, false);
			affectedAgent.Fight ();
		}

		protected override void FightEndTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			StopCoroutine (hardBeatCoroutine);
		}

		public override void CancelSkillEffect ()
		{
			StopCoroutine (hardBeatCoroutine);
		}

	}
}