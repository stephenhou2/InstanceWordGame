using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class HealthSource : TalentSkill {

		public float healthScalerBase;

		void Awake(){
			skillType = SkillType.Passive;
			skillName = "生命之源";
			skillDescription = string.Format("提升<color=orange>{0}*技能等级%</color>的生命值",(int)(healthScalerBase*100));
		}

		protected override void ExcuteSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			if (levelChanged) {
				self.agent.SetBasePropertyGainScalers (0, 0, 0, 0, 0, 0, healthScalerBase * skillLevel, 0);
				self.agent.ResetBattleAgentProperties (false);
				levelChanged = false;
			}
		}

	}
}
