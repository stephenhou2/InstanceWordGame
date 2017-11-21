using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	public class StrengthenWhenEndangered : PassiveSkill {

		public float endangeredHealthScaler;
		public int endangeredAttack;
		public int endangeredAttackSpeed;

		void Awake(){
			this.skillType = SkillType.Passive;
		}

		protected override void ExcuteSkillLogic (BattleAgentController self, BattleAgentController enemy){
		
		}

		protected override void BeAttackedTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			if (self.states.Contains (stateName)) {
				return;
			}
			if (self.agent.health / self.agent.maxHealth <= endangeredHealthScaler) {
				self.agent.attack = endangeredAttack;
				self.agent.attackSpeed = endangeredAttackSpeed;
				self.states.Add (stateName);
			}
		}

	}
}
