using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
public class Paralysis : StateSkillEffect {

	public override void AffectAgents (Agent self, List<Agent> friends, Agent targetEnemy, List<Agent> enemies, int skillLevel, TriggerType triggerType, int attachedInfo)
	{
		bool isParalysis = isEffective (this.scaler * skillLevel);
		if (isParalysis) {
			self.agility = (int)(0.75f * self.agility);
		}
	}
}
}
