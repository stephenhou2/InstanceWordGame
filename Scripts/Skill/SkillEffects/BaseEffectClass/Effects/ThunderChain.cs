using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderChain : BaseSkillEffect {

	public override void AffectAgents (BattleAgent self, List<BattleAgent> friends, BattleAgent targetEnemy, List<BattleAgent> enemies, int skillLevel, TriggerType triggerType, int attachedInfo)
	{
		Debug.Log (self.agentName +  "使用了雷霆锁链");



		foreach (Monster enemy in enemies) {
			//计算对方闪避率
			float dodge = seed * enemy.agility / (1 + seed * enemy.agility);
			//判断对方是否闪避成功
			if (isEffective (dodge)) {
				Debug.Log ("enemy dodge your attack");
				//目标触发闪避成功效果
				enemy.OnTrigger (enemies, self, friends, TriggerType.Dodge, 0);
				enemy.baView.PlayHurtHUDAnim ("<color=gray>miss</color>");
				return;
			}

			// 对方未闪避成功，判断自己是否打出了暴击
			bool isCrit = isEffective (seed * self.crit / (1 + seed * self.crit));

			if (isCrit) {
				self.critScaler = 2.0f;
			}	

			//原始魔法伤害值
			int originalDamage = (int)((scaler * skillLevel + 1) * self.magic * enemy.hurtScaler * self.critScaler);

			Debug.Log ("original damage" + originalDamage);

			//抵消魔抗作用后的实际伤害值
			int actualDamage = (int)(originalDamage / (1 + seed * enemy.magicResist) + 0.5f);


			Debug.Log ("actual damage" + actualDamage);
			//抵消的伤害值
			int DamageOffset = originalDamage - actualDamage;

			//己方触发命中效果
			self.OnTrigger (friends, targetEnemy, enemies, TriggerType.MagicalHit, 0);
			//目标触发被击中效果
			enemy.OnTrigger (enemies, self, friends, TriggerType.BeMagicalHit, DamageOffset);

			if (self.critScaler == 2.0f) {
				enemy.baView.PlayHurtHUDAnim ("<color=red>暴击 -" + actualDamage + "</color>");
			} else {
				enemy.baView.PlayHurtHUDAnim ("<color=red>-" + actualDamage + "</color>");
			}


			enemy.health -= actualDamage;


			enemy.OnTrigger (enemies, self, friends, TriggerType.MagicalHit, 0);


			self.critScaler = 1.0f;

			if (enemy.health < 0) {
				enemy.health = 0;
			}

			Debug.Log ("target health:" + enemy.health);

			int healthAbsorb = (int)(actualDamage * self.healthAbsorbScalser);
			if (healthAbsorb > 0) {
				enemy.baView.PlayHurtHUDAnim ("<color=green>    +" + actualDamage + "</color>");
			}

			self.health += healthAbsorb;
			if (self.health >= self.maxHealth) {
				self.health = self.maxHealth;
			}

		}
	}
}
