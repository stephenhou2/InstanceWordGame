using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class HealthSource : Skill {

		void Awake(){
			isPassive = true;
			skillType = SkillType.Passive;
			baseNum = 0.2f;
			skillName = "生命之源";
			skillDescription = string.Format("提升<color=orange>{0}*技能等级%</color>的生命值",(int)(baseNum*100));
		}

		public override void AffectAgents (BattleAgentController self, BattleAgentController enemy)
		{
			self.agent.SetBasePropertyGainScalers (0, 0, 0, 0, 0, 0, baseNum * skillLevel, 0);
			self.agent.ResetBattleAgentProperties (false);
		}

	}
}
