using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class WeaponExpert : Skill {

		void Awake(){
			isPassive = true;
			baseNum = 0.05f;
			skillName = "武器精通";
			skillDescription = string.Format ("提升<color=orange>{0}*技能等级%</color>暴击", (int)(baseNum * 100));
		}

		public override void AffectAgents (BattleAgentController self, BattleAgentController enemy)
		{
			self.agent.SetBasePropertyGainScalers (0, 0, 0, 0, 0, baseNum * skillLevel, 0, 0);
		}

	}
}
