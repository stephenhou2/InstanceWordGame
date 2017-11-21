using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class MagicAttachedAttack : PassiveSkill {

		public int attachMagicHurtBase;

		public float attachMagicHurtScalerBase;

		void Awake(){
			this.skillType = SkillType.Passive;
			this.skillName = "攻击附魔";
			this.skillDescription = string.Format ("普通攻击附带<color=orange>{0}+{1}%*最大魔法值</color>点的魔法伤害",(int)(attachMagicHurtBase * 100),(int)(attachMagicHurtScalerBase * 100));
		}

		protected override void ExcuteSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			if (levelChanged) {
				self.agent.attachMagicHurtScaler = attachMagicHurtBase + attachMagicHurtScalerBase * skillLevel;
				self.agent.ResetBattleAgentProperties (false);
				levelChanged = false;
			}
		}

	}
}
