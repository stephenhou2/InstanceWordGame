using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace WordJourney
{
public class BloodThirsty : BaseSkillEffect {

	public override void AffectAgents (Agent self, List<Agent> friends, Agent targetEnemy, List<Agent> enemies, int skillLevel, TriggerType triggerType, int attachedInfo)
	{
		self.healthAbsorbScalser = this.scaler * skillLevel * self.attack;
	}

}
}
