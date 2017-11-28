using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	public class StrengthenWhenEndangered : TriggeredPassiveSkill {

		public float endangeredHealthScaler;
		public int endangeredAttack;
		public int endangeredAttackSpeed;
		public bool removeWhenQuitFight;

		void Awake(){
			this.skillType = SkillType.Passive;
		}

		protected override void ExcuteSkillLogic (BattleAgentController self, BattleAgentController enemy){
		
		}

		protected override void BeAttackedTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			if (!self.CheckStateExist (stateName)) {
				SkillState state = new SkillState (this, stateName, removeWhenQuitFight, null);
				self.states.Add (state);
			}

			if (self.agent.health / self.agent.maxHealth <= endangeredHealthScaler) {
				self.agent.attack = endangeredAttack;
				self.agent.attackSpeed = endangeredAttackSpeed;
			}


		}

	}
}
