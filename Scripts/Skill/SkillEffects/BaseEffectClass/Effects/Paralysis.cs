using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paralysis : StateSkillEffect {

	public override void AffectAgents (BattleAgent self, List<BattleAgent> friends, BattleAgent targetEnemy, List<BattleAgent> enemies, int skillLevel, TriggerType triggerType, int attachedInfo)
	{
		bool isParalysis = isEffective (this.scaler * skillLevel);
		if (isParalysis) {
			self.agility = (int)(0.75f * self.agility);
		}
	}
}
