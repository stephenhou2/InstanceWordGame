using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class IronSkin : Skill {

		void Awake(){
			isPassive = true;
			skillType = SkillType.Passive;
			baseNum = 0.05f;
			skillName = "钢铁皮肤";
			skillDescription = string.Format ("提升<color=orange>{0}*技能等级%</color>的护甲",(int)(1+baseNum * 100));
		}

		public override void AffectAgents (BattleAgentController self, BattleAgentController enemy)
		{
			self.agent.SetBasePropertyGainScalers (0, 0, baseNum * skillLevel, 0, 0, 0, 0, 0);
		}

	}
}