using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class MagicStrengthen : Skill {

		void Awake(){
			isPassive = true;
			skillType = SkillType.Passive;
			baseNum = 0.1f;
			skillName = "魔法强化";
			skillDescription = string.Format("提升<color=orange>{0}*技能等级%</color>的魔法伤害",(int)(baseNum*100));
		}

		public override void AffectAgents (BattleAgentController self, BattleAgentController enemy)
		{
			self.agent.magicalHurtScaler = baseNum * skillLevel;
			self.agent.ResetBattleAgentProperties (false);
		}


	}
}
