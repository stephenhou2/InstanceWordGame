using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stonelize : StateSkillEffect {
	public override void AffectAgents (BattleAgent self, List<BattleAgent> friends, BattleAgent targetEnemy, List<BattleAgent> enemies, int skillLevel, TriggerType triggerType, int attachedInfo)
	{
		self.validActionType = ValidActionType.None;
//		self.amour = (int)((1.0f - this.scaler * skillLevel) * self.amour);
//		self.magicResist = (int)((1.0f - this.scaler * skillLevel) * self.magicResist);
	}
}
