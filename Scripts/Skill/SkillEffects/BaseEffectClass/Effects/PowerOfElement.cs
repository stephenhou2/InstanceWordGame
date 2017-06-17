using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerOfElement : StateSkillEffect {

	public override void AffectAgents (BattleAgent self, List<BattleAgent> friends, BattleAgent targetEnemy, List<BattleAgent> enemies, int skillLevel, TriggerType triggerType, int attachedInfo)
	{
		self.magic = (int)((1 + this.scaler * skillLevel) * self.magic);
	}
}
