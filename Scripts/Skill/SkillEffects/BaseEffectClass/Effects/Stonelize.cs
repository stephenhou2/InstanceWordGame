using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
public class Stonelize : StateSkillEffect {
	public override void AffectAgents (Agent self, List<Agent> friends, Agent targetEnemy, List<Agent> enemies, int skillLevel, TriggerType triggerType, int attachedInfo)
	{
		self.validActionType = ValidActionType.None;
//		self.amour = (int)((1.0f - this.scaler * skillLevel) * self.amour);
//		self.magicResist = (int)((1.0f - this.scaler * skillLevel) * self.magicResist);
	}
}
}
