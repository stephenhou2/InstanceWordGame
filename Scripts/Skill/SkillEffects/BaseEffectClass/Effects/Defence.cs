using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
public class Defence : StateSkillEffect {
	public override void AffectAgents (Agent self, List<Agent> friends, Agent targetEnemy, List<Agent> enemies, int skillLevel, TriggerType triggerType, int attachedInfo)
	{
		self.hurtScaler = this.scaler;
		self.strength += 3;
		if (self.strength > self.maxStrength) {
			self.strength = self.maxStrength;
		}
	}
}
}
