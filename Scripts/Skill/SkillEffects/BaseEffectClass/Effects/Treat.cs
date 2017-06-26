using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treat : BaseSkillEffect {

	public override void AffectAgents (BattleAgent self, List<BattleAgent> friends, BattleAgent targetEnemy, List<BattleAgent> enemies, int skillLevel, TriggerType triggerType, int attachedInfo)
	{
		int healthIncreased = (int)(this.scaler * skillLevel * self.magic);
		self.health += healthIncreased;
		self.baView.PlayHurtHUDAnim ("<color=green>  +" + healthIncreased + "</color>");
		if (self.health >= self.maxHealth) {
			self.health = self.maxHealth;
		}
	}
}
