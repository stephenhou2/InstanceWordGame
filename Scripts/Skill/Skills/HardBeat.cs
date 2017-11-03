using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class HardBeat : Skill {

		public float duration = 2.0f;

		private BattleAgentController baCtr;

		void Awake(){
			isPassive = true;
			skillType = SkillType.Passive;
			baseNum = 0.01f;
			skillName = "重击";
			skillDescription = string.Format ("<color=orange>{0}*技能等级</color>%的概率将对方击晕，持续<color=orange>{1}s</color>",(int)(baseNum * 100),duration);
		}

		public override void AffectAgents (BattleAgentController self, BattleAgentController enemy)
		{
			baCtr = enemy;
			self.attackTriggerCallBacks.Add (HardBeatEnemy);
		}

		private void HardBeatEnemy(){

			float hardBeatChance = baseNum * skillLevel;

			if (isEffective (hardBeatChance)) {
				
				CancelInvoke ("EndHardBeat");

				baCtr.PlayRoleAnim (enemyAnimName, 0, null);

				baCtr.SetEffectAnim (enemyEffectName, true);

				Invoke ("EndHardBeat", duration);
			}
		}

		private void EndHardBeat(){
			baCtr.SetEffectAnim (enemyEffectName, false);
			baCtr.Fight ();
		}

	}
}