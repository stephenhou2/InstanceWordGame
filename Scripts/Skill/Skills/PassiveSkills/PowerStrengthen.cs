using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class PowerStrengthen : PassiveSkill {

		public float attackScalerBase;

		void Awake(){
			skillType = SkillType.Passive;
			skillName = "力量强化";
			skillDescription = string.Format("提升<color=orange>{0}*技能等级%</color>的攻击力",(int)(attackScalerBase*100));

		}

		protected override void ExcuteSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			if (levelChanged) {
				self.agent.SetBasePropertyGainScalers (attackScalerBase * skillLevel, 0, 0, 0, 0, 0, 0, 0);
				self.agent.ResetBattleAgentProperties (false);
				levelChanged = false;
			}
		}
	}

}
