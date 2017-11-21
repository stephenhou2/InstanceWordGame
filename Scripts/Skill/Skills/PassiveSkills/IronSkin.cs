using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class IronSkin : PassiveSkill {

		public float armorScalerBase;

		void Awake(){
			skillType = SkillType.Passive;
			skillName = "钢铁皮肤";
			skillDescription = string.Format ("提升<color=orange>{0}*技能等级%</color>的护甲",(int)(armorScalerBase * 100));
		}

		protected override void ExcuteSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			if (levelChanged) {
				self.agent.SetBasePropertyGainScalers (0, 0, armorScalerBase * skillLevel, 0, 0, 0, 0, 0);
				self.agent.ResetBattleAgentProperties (false);
				levelChanged = false;
			}
		}

	}
}