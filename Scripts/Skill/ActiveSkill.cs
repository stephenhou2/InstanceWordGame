using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public abstract class ActiveSkill : Skill {

		public string selfRoleAnimName;

		public float probability;

		public void SetEffectAnims(BattleAgentController self,BattleAgentController enemy){
			if (selfEffectAnimName != string.Empty) {
				self.SetEffectAnim (selfEffectAnimName, null);
			}
			if (enemyEffectAnimName != string.Empty) {
				enemy.SetEffectAnim (enemyEffectAnimName, null);
			}
		}

	}


}
