using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defence : StateSkillEffect {
	public override void AffectAgents (BattleAgent self, List<BattleAgent> friends, BattleAgent targetEnemy, List<BattleAgent> enemies, int skillLevel, TriggerType triggerType, int attachedInfo)
	{
		self.hurtScaler = this.scaler;
		self.strength += 3;
		if (self.strength > self.maxStrength) {
			self.strength = self.maxStrength;
		}
	}
}
