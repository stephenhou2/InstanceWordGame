using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class PowerStrengthen : TalentSkill {

		public float attackScalerBase;

		protected override void Awake ()
		{
			base.Awake ();
			skillName = "力量强化";
			skillDescription = string.Format("提升<color=orange>{0}*技能等级%</color>的攻击力",(int)(attackScalerBase*100));

		}

		protected override void ExcuteSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			self.agent.SetBasePropertyGainScalers (attackScalerBase * skillLevel, 0, 0, 0, 0, 0, 0, 0);
			self.agent.ResetBattleAgentProperties (false);
		}
	}

}
