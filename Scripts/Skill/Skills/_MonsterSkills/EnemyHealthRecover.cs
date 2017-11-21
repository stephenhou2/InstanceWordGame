using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	// 角色死亡时对方恢复一定血量
	public class EnemyHealthRecover : PassiveSkill {

		public int enemyHealthGainBase;// 对方回复的血量值

		void Awake(){
			this.skillType = SkillType.Passive;
		}

		protected override void ExcuteSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			enemy.agent.health += enemyHealthGainBase * skillLevel;
		}

		
	}
}
