using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
public class DecreaseAmour : StateSkillEffect {

	public override void AffectAgents (Agent self, List<Agent> friends, Agent targetEnemy, List<Agent> enemies, int skillLevel, TriggerType triggerType, int attachedInfo)
	{
		self.amour = (int)(self.amour * (1 - this.scaler * skillLevel));
	}

}
}
