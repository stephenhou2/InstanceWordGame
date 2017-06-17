using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treat : BaseSkillEffect {

	public override void AffectAgents (BattleAgent self, List<BattleAgent> friends, BattleAgent targetEnemy, List<BattleAgent> enemies, int skillLevel, TriggerType triggerType, int attachedInfo)
	{
		self.health += (int)(this.scaler * skillLevel * self.magic);
		if (self.health >= self.maxHealth) {
			self.health = self.maxHealth;
		}
	}
}
