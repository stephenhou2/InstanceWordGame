using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
public class WeaponExpert : StateSkillEffect {

	public override void AffectAgents (Agent self, List<Agent> friends, Agent targetEnemy, List<Agent> enemies, int skillLevel, TriggerType triggerType, int attachedInfo)
	{
		self.attack = (int)((1 + this.scaler) * self.attack);
	}
}
}
