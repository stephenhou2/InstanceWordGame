using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class HardBeat : TriggeredPassiveSkill {

		public float duration;

		public float probabilityBase;

		private Coroutine hardBeatCoroutine;

		void Awake(){
			skillType = SkillType.Passive;
			skillName = "重击";
			skillDescription = string.Format ("<color=orange>{0}*技能等级</color>%的概率将对方击晕，持续<color=orange>{1}s</color>",(int)(probabilityBase * 100),duration);
		}

		protected override void ExcuteSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			if (levelChanged) {
				self.agent.hardBeatProbability = probabilityBase * skillLevel;
				self.agent.ResetBattleAgentProperties (false);
				levelChanged = false;
			}
		}

		protected override void AttackTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{

			if (isEffective (self.agent.hardBeatProbability)) {
				
				StopCoroutine (hardBeatCoroutine);

				enemy.PlayRoleAnim (enemyAnimName, 0, null);

				enemy.SetEffectAnim (enemyEffectName, true);

				hardBeatCoroutine = StartCoroutine ("EndHardBeat",enemy);
			}
		}

		private IEnumerator EndHardBeat(BattleAgentController targetBa){
			yield return new WaitForSeconds (duration);
			targetBa.SetEffectAnim (enemyEffectName, false);
			targetBa.Fight ();
		}

	}
}