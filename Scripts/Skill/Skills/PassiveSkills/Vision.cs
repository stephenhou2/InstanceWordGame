using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class Vision : PassiveSkill {

		public float dodgeScalerBase;

		void Awake(){
			skillType = SkillType.Passive;
			skillName = "幻境";
			skillDescription = string.Format ("提升<color=orange>{0}*技能等级</color>的闪避", (int)(dodgeScalerBase * 100));
		}


		protected override void ExcuteSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			if (levelChanged) {
				self.agent.SetBasePropertyGainScalers (0, 0, 0, 0, dodgeScalerBase * skillLevel, 0, 0, 0);
				self.agent.ResetBattleAgentProperties (false);
				levelChanged = false;
			}
		}

	}
}