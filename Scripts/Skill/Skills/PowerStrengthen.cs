using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class PowerStrengthen : Skill {

		void Awake(){
			isPassive = true;
			skillType = SkillType.Passive;
			baseNum = 0.05f;
			skillName = "力量强化";
			skillDescription = string.Format("提升<color=orange>{0}*技能等级%</color>的物理伤害",(int)(baseNum*100));

		}

		public override void AffectAgents (BattleAgentController self, BattleAgentController enemy)
		{
			Debug.Log (self.agent);
			self.agent.physicalHurtScaler = baseNum * skillLevel;
		}
	}

}
