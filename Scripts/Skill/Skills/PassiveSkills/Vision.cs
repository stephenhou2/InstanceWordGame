using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class Vision : TalentSkill {

		public float dodgeScalerBase;

		protected override void Awake ()
		{
			base.Awake ();
			skillName = "幻境";
			skillDescription = string.Format ("提升<color=orange>{0}*技能等级</color>的闪避", (int)(dodgeScalerBase * 100));
		}


		protected override void ExcuteSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			self.agent.SetBasePropertyGainScalers (0, 0, 0, 0, dodgeScalerBase * skillLevel, 0, 0, 0);
			self.agent.ResetBattleAgentProperties (false);
		}

	}
}