using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class MagicForbidden : PassiveSkill {

		protected override void ExcuteSkillLogic (BattleAgentController self, BattleAgentController enemy){

		}

		protected override void BeforeFightTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			// 获取玩家所有的主动技能
			List<ActiveSkill> enemyActiveSkills = (enemy.agent as Player).equipedActiveSkills;

			for (int i = 0; i < enemyActiveSkills.Count; i++) {
				ActiveSkill activeSkill = enemyActiveSkills [i];
				activeSkill.isAvalible = false;
			}
		}

		protected override void FightEndTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			// 获取玩家所有的主动技能
			List<ActiveSkill> enemyActiveSkills = (enemy.agent as Player).equipedActiveSkills;

			for (int i = 0; i < enemyActiveSkills.Count; i++) {
				ActiveSkill activeSkill = enemyActiveSkills [i];
				activeSkill.isAvalible = activeSkill.manaConsume <= enemy.agent.mana;
			}
		}

	}
}
