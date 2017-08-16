using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
public class Treat : BaseSkillEffect {

	public override void AffectAgents (Agent self, List<Agent> friends, Agent targetEnemy, List<Agent> enemies, int skillLevel, TriggerType triggerType, int attachedInfo)
	{
		int healthIncreased = (int)(this.scaler * skillLevel * self.magic);
		self.health += healthIncreased;
			self.baController.PlayHurtHUDAnim ("<color=green>  +" + healthIncreased + "</color>");
		if (self.health >= self.maxHealth) {
			self.health = self.maxHealth;
		}
	}
}
}
