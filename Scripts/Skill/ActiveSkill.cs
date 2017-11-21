using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public abstract class ActiveSkill : Skill {


		public int baseManaConsume;//基础魔法消耗

		public int manaConsumeGain;//魔法消耗增长值

		//技能的魔法消耗
		public int manaConsume{
			get{
				return baseManaConsume + (skillLevel / 5) * manaConsumeGain;
			}
		}

		/// <summary>
		/// 技能作用效果
		/// </summary>
		/// <param name="self">Self.</param>
		/// <param name="enemy">Enemy.</param>
		/// <param name="skillLevel">Skill level.</param>
		public sealed override void AffectAgents(BattleAgentController self, BattleAgentController enemy){
			ExcuteSkillLogic (self, enemy);
		}

	}
}
