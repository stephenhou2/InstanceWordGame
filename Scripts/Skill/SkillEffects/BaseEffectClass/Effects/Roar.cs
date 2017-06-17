using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roar : StateSkillEffect {

	public override void AffectAgents (BattleAgent self, List<BattleAgent> friends, BattleAgent targetEnemy, List<BattleAgent> enemies, int skillLevel, TriggerType triggerType, int attachedInfo)
	{
		self.crit = (int)(self.crit * (1 + this.scaler * skillLevel));
	}

}
