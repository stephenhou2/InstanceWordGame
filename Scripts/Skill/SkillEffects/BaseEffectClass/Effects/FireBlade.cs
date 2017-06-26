using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBlade : BaseSkillEffect {

	public override void AffectAgents (BattleAgent self, List<BattleAgent> friends, BattleAgent targetEnemy, List<BattleAgent> enemies, int skillLevel, TriggerType triggerType, int attachedInfo)
	{
		Debug.Log (self.agentName +  "使用了火焰斩");

		//计算对方闪避率
		float dodge = seed * targetEnemy.agility / (1 + seed * targetEnemy.agility);
		//判断对方是否闪避成功
		if (isEffective (dodge)) {
			Debug.Log ("enemy dodge your attack");
			//目标触发闪避成功效果
			targetEnemy.OnTrigger (enemies,self,friends,TriggerType.Dodge, 0);
			targetEnemy.baView.PlayHurtHUDAnim ("<color=gray>miss</color>");
			return;
		}

		// 对方未闪避成功，判断自己是否打出了暴击
		bool isCrit = isEffective (seed * self.crit / (1 + seed * self.crit));

		if (isCrit) {
			self.critScaler = 2.0f;
		}

		//原始魔法伤害值
		int originalMagicalDamage = (int)((scaler * skillLevel + 1) * self.magic * targetEnemy.hurtScaler * self.critScaler);

		Debug.Log("original damage" + originalMagicalDamage);

		//抵消魔抗作用后的实际伤害值
		int actualMagicalDamage = (int)(originalMagicalDamage / (1 + seed * targetEnemy.magicResist) + 0.5f);

		//原始物理伤害值
		int originalPhysicalDamage = (int)((scaler * skillLevel + 1) * self.attack * targetEnemy.hurtScaler * self.critScaler);

		Debug.Log("original damage" + originalPhysicalDamage);

		//抵消护甲作用后的实际伤害值
		int actualPhysicalDamage = (int)(originalPhysicalDamage / (1 + seed * targetEnemy.amour) + 0.5f);

		Debug.Log("actual damage" + actualPhysicalDamage);

		//抵消的伤害值
		int DamageOffset = originalMagicalDamage + originalPhysicalDamage - actualMagicalDamage - actualPhysicalDamage;

		//己方触发命中效果
		self.OnTrigger (friends,targetEnemy,enemies,TriggerType.DisorderHit, 0);
		//目标触发被击中效果
		targetEnemy.OnTrigger (enemies,self,friends,TriggerType.BeDisorderHit, DamageOffset);

		int actualDamage = actualMagicalDamage + actualPhysicalDamage;

		if (self.critScaler == 2.0f) {
			targetEnemy.baView.PlayHurtHUDAnim ("<color=red>暴击 -" + actualDamage + "</color>");
		} else {
			targetEnemy.baView.PlayHurtHUDAnim ("<color=red>-" + actualDamage + "</color>");
		}

		targetEnemy.health -= (actualMagicalDamage + actualPhysicalDamage);

		self.critScaler = 1.0f;

		if(targetEnemy.health < 0){
			targetEnemy.health = 0;
		}
		Debug.Log("target health:" + targetEnemy.health);

		self.health += (int)((actualMagicalDamage + actualPhysicalDamage) * self.healthAbsorbScalser);

		if(self.health > self.maxHealth){
			self.health = self.maxHealth;
		}
	}
}
