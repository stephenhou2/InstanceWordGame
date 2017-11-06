using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class BrambleShield : Skill {

		void Awake(){	
			isPassive = true;
			skillType = SkillType.Passive;
			baseNum = 0.1f;
			skillName = "荆棘护甲";
			skillDescription = string.Format("反弹<color=orange>{0}*技能等级%</color>护甲和魔抗所抵消的伤害",(int)(baseNum * 100));
		}

		public override void AffectAgents (BattleAgentController self, BattleAgentController enemy)
		{
			self.agent.reflectScaler = baseNum * skillLevel;
			self.agent.ResetBattleAgentProperties (false);
		}
	}
}