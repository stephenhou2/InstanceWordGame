using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodThirsty : BaseSkillEffect {

	public override void AffectAgents (BattleAgent self, List<BattleAgent> friends, BattleAgent targetEnemy, List<BattleAgent> enemies, int skillLevel, TriggerType triggerType, int attachedInfo)
	{
		self.healthAbsorbScalser = this.scaler * skillLevel * self.attack;
	}

}
