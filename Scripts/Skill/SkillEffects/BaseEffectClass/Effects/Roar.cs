using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
public class Roar : StateSkillEffect {

	public override void AffectAgents (Agent self, List<Agent> friends, Agent targetEnemy, List<Agent> enemies, int skillLevel, TriggerType triggerType, int attachedInfo)
	{
		self.crit = (int)(self.crit * (1 + this.scaler * skillLevel));
	}

}
}
