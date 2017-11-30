using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class ElementBody : TalentSkill {

		public float magicResistScalerBase;

		protected override void Awake ()
		{
			base.Awake ();
			skillName = "元素体质";
			skillDescription = string.Format("提升<color=orange>{0}*技能等级%</color>抗性",(int)(magicResistScalerBase * 100));
		}

		protected override void ExcuteSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			self.agent.SetBasePropertyGainScalers (0, 0, 0, magicResistScalerBase * skillLevel, 0, 0, 0, 0);
			self.agent.ResetBattleAgentProperties (false);
		}
	}
}