using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class WeaponExpert : TalentSkill {

		public float critScalerBase;

		void Awake(){
			skillType = SkillType.Passive;
			skillName = "武器精通";
			skillDescription = string.Format ("提升<color=orange>{0}*技能等级%</color>暴击", (int)(critScalerBase * 100));
		}

		protected override void ExcuteSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			if (levelChanged) {
				self.agent.SetBasePropertyGainScalers (0, 0, 0, 0, 0, critScalerBase * skillLevel, 0, 0);
				self.agent.ResetBattleAgentProperties (false);
				levelChanged = false;
			}
		}

	}
}
