using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class BrambleShield : TalentSkill {

		public float reflectScalerBase;

		protected override void Awake ()
		{
			base.Awake ();
			skillName = "荆棘护甲";
			skillDescription = string.Format("将护甲和魔抗所抵消伤害的<color=orange>{0}*技能等级%</color>反弹给敌方",(int)(reflectScalerBase * 100));
		}


		protected override void ExcuteSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			self.agent.reflectScaler = reflectScalerBase * skillLevel;
			self.agent.ResetBattleAgentProperties (false);
		}

	}
}