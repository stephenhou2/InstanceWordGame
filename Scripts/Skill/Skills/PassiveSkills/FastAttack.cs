using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class FastAttack : TalentSkill{

		public float attackSpeedScalerBase;

		protected override void Awake ()
		{
			base.Awake ();
			skillName = "速击";
			skillDescription = string.Format("提升<color=orange>{0}*技能等级%</color>的攻速",(int)(attackSpeedScalerBase * 100));
		}

		protected override void ExcuteSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			self.agent.attackSpeed = (int)(self.agent.attackSpeed * (1 + attackSpeedScalerBase * skillLevel));
			self.agent.ResetBattleAgentProperties (false);
		}


	}
}