using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	// 天赋技能类
	public abstract class TalentSkill : PassiveSkill {

		protected virtual void Awake(){
			this.skillType = SkillType.TalentPassive;
			this.canOverlay = false;
		}


		/// <summary>
		/// 天赋技能技能效果;只有在技能升级时执行技能效果，技能效果为永久改变角色某项属性
		/// </summary>
		/// <param name="self">Self.</param>
		/// <param name="enemy">Enemy.</param>
		public override void AffectAgents (BattleAgentController self, BattleAgentController enemy)
		{
			if (levelChanged) {
				ExcuteSkillLogic (self, enemy);
				levelChanged = false;
			}
		}

	}
}
