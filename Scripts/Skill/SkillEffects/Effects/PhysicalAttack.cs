using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class PhysicalAttack : BaseSkillEffect {

		public override void AffectAgents (BattleAgentController self, BattleAgentController enemy, int skillLevel, TriggerType triggerType)
		{
			int attackCount = 0;

			Debug.Log (self.agent.agentName +  "使用了普通攻击");

			do {
				attackCount++;
				//计算对方闪避率
				float dodge = seed * enemy.agent.agility / (1 + seed * enemy.agent.agility);
				//判断对方是否闪避成功
				if (isEffective (dodge)) {
					Debug.Log (enemy.agent.agentName + "成功躲避了攻击");
					//目标触发闪避成功效果
					enemy.OnTrigger (self,TriggerType.Dodge);
//					enemy.baView.PlayHurtHUDAnim ("<color=gray>miss</color>");
					return;
				}

				//是否打出暴击
				bool isCrit = isEffective (seed * self.agent.crit / (1 + seed * self.agent.crit));

				if (isCrit) {
					self.agent.critScaler = 2.0f;
				}

				//原始物理伤害值
				int originalDamage = (int)(self.agent.attack * enemy.agent.hurtScaler * self.agent.critScaler);

				//抵消护甲作用后的实际伤害值
				int actualDamage = (int)(originalDamage / (1 + seed * enemy.agent.amour) + 0.5f);

				//抵消的伤害值
				int DamageOffset = originalDamage - actualDamage;

				//己方触发命中效果
				self.OnTrigger (enemy,TriggerType.PhysicalHit);
				//目标触发被击中效果
				enemy.OnTrigger (self,TriggerType.BePhysicalHit);

				enemy.agent.health -= actualDamage;

				string tintStr = string.Format("<color=red>{0}</color>",actualDamage);

				enemy.PlayHurtTextAnim(tintStr);

				// 攻击之后将暴击伤害率重新设定为1
				self.agent.critScaler = 1.0f;

				if(enemy.agent.health < 0){
					enemy.agent.health = 0;
				}

				if(self.agent.healthAbsorbScalser > 0){

					int healthGain = (int)(actualDamage * self.agent.healthAbsorbScalser);

					self.agent.health += healthGain;

					if(self.agent.health > self.agent.maxHealth){
						self.agent.health = self.agent.maxHealth;
					}

					string gainStr = string.Format("<color=green>{0}</color>",healthGain);

					self.PlayGainTextAnim(gainStr);
				}




			} while(attackCount < self.agent.attackTime);

		}
	}
}
