using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class Vision : Skill {

		void Awake(){
			isPassive = true;
			baseNum = 0.05f;
			skillName = "幻境";
			skillDescription = string.Format ("提升<color=orange>{0}*技能等级</color>的闪避", (int)(baseNum * 100));
		}


		public override void AffectAgents (BattleAgentController self, BattleAgentController enemy)
		{
			self.agent.dodgeGainScaler = baseNum * skillLevel;
		}

	}
}