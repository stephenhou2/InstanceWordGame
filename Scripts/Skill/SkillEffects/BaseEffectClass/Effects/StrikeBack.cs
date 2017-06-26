using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikeBack : StateSkillEffect {
	public override void AffectAgents (BattleAgent self, List<BattleAgent> friends, BattleAgent targetEnemy, List<BattleAgent> enemies, int skillLevel, TriggerType triggerType, int attachedInfo)
	{

		bool strikeBack = isEffective (this.scaler * skillLevel);

		if (strikeBack && (triggerType == TriggerType.BePhysicalHit || triggerType == TriggerType.BeMagicalHit)) {

			Debug.Log (self.agentName + "使用了反击");

			float dodge = seed * targetEnemy.agility / (1 + seed * targetEnemy.agility);

			//判断对方是否闪避成功
			if (isEffective (dodge)) {
				Debug.Log ("enemy dodge your attack");
				//目标触发闪避成功效果
				targetEnemy.OnTrigger (enemies,self,friends,TriggerType.Dodge, 0);
				targetEnemy.baView.PlayHurtHUDAnim ("<color=gray>miss</color>");
				return;
			}

			bool isCrit = isEffective (seed * self.crit / (1 + seed * self.crit));

			if (isCrit) {
				self.critScaler = 2.0f;
			}

			//原始物理伤害值
			int originalDamage = (int)(self.attack * targetEnemy.hurtScaler * self.critScaler);

			//抵消护甲作用后的实际伤害值
			int actualDamage = (int)(originalDamage / (1 + seed * targetEnemy.amour));

			//抵消的伤害值
			int DamageOffset = originalDamage - actualDamage;

			//己方触发命中效果
			self.OnTrigger (friends,targetEnemy,enemies,TriggerType.PhysicalHit, 0);
			//目标触发被击中效果
			targetEnemy.OnTrigger (enemies,self,friends,TriggerType.BePhysicalHit, DamageOffset);

			if (self.critScaler == 2.0f) {
				targetEnemy.baView.PlayHurtHUDAnim ("<color=red>暴击 -" + actualDamage + "</color>");
			} else {
				targetEnemy.baView.PlayHurtHUDAnim ("<color=red>-" + actualDamage + "</color>");
			}

			targetEnemy.health -= actualDamage;

			if (targetEnemy.health < 0) {
				targetEnemy.health = 0;
			}

			self.health += (int)(actualDamage * self.healthAbsorbScalser);

			if(self.health > self.maxHealth){
				self.health = self.maxHealth;
			}

			self.critScaler = 1.0f;
		}
	}
}
