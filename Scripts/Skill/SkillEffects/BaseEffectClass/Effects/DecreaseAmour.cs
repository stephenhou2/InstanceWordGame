using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecreaseAmour : StateSkillEffect {

	public override void AffectAgents (BattleAgent self, List<BattleAgent> friends, BattleAgent targetEnemy, List<BattleAgent> enemies, int skillLevel, TriggerType triggerType, int attachedInfo)
	{
		self.amour = (int)(self.amour * (1 - this.scaler * skillLevel));
	}

}
