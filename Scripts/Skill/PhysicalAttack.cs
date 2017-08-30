using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class PhysicalAttack : Skill {


		public override void AffectAgents(BattleAgentController self, BattleAgentController enemy){

			TintTextType tintTextType = TintTextType.None;

			//计算对方闪避率
			float dodge = dodgeSeed * enemy.agent.agility / (1 + dodgeSeed * enemy.agent.agility);
			//判断对方是否闪避成功
			if(isEffective(dodge)){
				tintTextType = TintTextType.Miss;
				enemy.PlayHurtTextAnim(string.Empty,tintTextType);
				return;
			}



			//是否打出暴击
			bool isCrit = isEffective (critSeed * self.agent.crit / (1 + critSeed * self.agent.crit));

			if (isCrit) {
				self.agent.critScaler = 2.0f;
				tintTextType = TintTextType.Crit;
			}

			//原始物理伤害值
			int originalDamage = (int)(self.agent.attack * enemy.agent.physicalHurtScaler * self.agent.critScaler);

			//抵消护甲作用后的实际伤害值
			int actualDamage = (int)(originalDamage / (1 + amourSeed * enemy.agent.amour) + 0.5f);

			enemy.agent.health -= actualDamage;

			string tintStr = string.Format("<color=red>{0}</color>",actualDamage);

			enemy.PlayHurtTextAnim(tintStr,tintTextType);

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

		}

	}
}
