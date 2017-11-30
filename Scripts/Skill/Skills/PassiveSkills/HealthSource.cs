using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class HealthSource : TalentSkill {

		public float healthScalerBase;

		protected override void Awake ()
		{
			base.Awake ();
			skillName = "生命之源";
			skillDescription = string.Format("提升<color=orange>{0}*技能等级%</color>的生命值",(int)(healthScalerBase*100));
		}

		protected override void ExcuteSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			self.agent.SetBasePropertyGainScalers (0, 0, 0, 0, 0, 0, healthScalerBase * skillLevel, 0);
			self.agent.ResetBattleAgentProperties (false);
		}

	}
}
