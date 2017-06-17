using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponExpert : StateSkillEffect {

	public override void AffectAgents (BattleAgent self, List<BattleAgent> friends, BattleAgent targetEnemy, List<BattleAgent> enemies, int skillLevel, TriggerType triggerType, int attachedInfo)
	{
		self.attack = (int)((1 + this.scaler) * self.attack);
	}
}
