using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
public class PowerOfElement : StateSkillEffect {

	public override void AffectAgents (Agent self, List<Agent> friends, Agent targetEnemy, List<Agent> enemies, int skillLevel, TriggerType triggerType, int attachedInfo)
	{
		self.magic = (int)((1 + this.scaler * skillLevel) * self.magic);
	}
}
}
