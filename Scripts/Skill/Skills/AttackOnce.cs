using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class AttackOnce : Skill {

		void Awake(){
			isPassive = false;
		}

		public override void AffectAgents(BattleAgentController self, BattleAgentController enemy){

			TintTextType tintTextType = TintTextType.None;

			//计算对方闪避率(敌方的基础闪避率 - 己方的闪避修正)
			float dodge = dodgeSeed * enemy.agent.dodge / (1 + dodgeSeed * enemy.agent.dodge) - self.agent.dodgeFixScaler;
			//判断对方是否闪避成功
			if(isEffective(dodge)){
				tintTextType = TintTextType.Miss;
				enemy.PlayHurtTextAnim(string.Empty,tintTextType);
				return;
			}
				
			//计算己方的暴击率（己方的基础暴击率 - 敌方的暴久修正）
			float crit = critSeed * self.agent.crit / (1 + critSeed * self.agent.crit) - enemy.agent.dodgeFixScaler;

			//是否打出暴击
			bool isCrit = isEffective (crit);

			if (isCrit) {
				#warning 这里暴击倍率暂时固定为2被暴击
				self.agent.critHurtScaler = 2.0f;
				tintTextType = TintTextType.Crit;
			}

			//原始物理伤害值
			int originalDamage = (int)(self.agent.attack * (1 + self.agent.physicalHurtScaler) * self.agent.critHurtScaler);

			// 攻击之后将暴击伤害率重新设定为1
			self.agent.critHurtScaler = 1.0f;

			//抵消护甲作用后的实际伤害值
			int actualDamage = (int)(originalDamage / (1 + armorSeed * enemy.agent.armor) + 0.5f);

			int damageOffset = originalDamage - actualDamage;

			if (enemy.agent.reflectScaler > 0) {

				int reflectDamage = (int)(damageOffset * enemy.agent.reflectScaler);

				self.agent.health -= reflectDamage;

				if (self.agent.health < 0) {
					self.agent.health = 0;
				}

				string hurtStr = string.Format ("<color=red>{0}</color>", reflectDamage);

				self.PlayHurtTextAnim (hurtStr, TintTextType.None);
			}

			actualDamage = (int)(actualDamage * (1 - enemy.agent.decreaseHurtScaler));

			enemy.agent.health -= actualDamage;

			if(enemy.agent.health < 0){
				enemy.agent.health = 0;
			}

			string tintStr = string.Format("<color=red>{0}</color>",actualDamage);

			enemy.PlayHurtTextAnim(tintStr,tintTextType);

			Debug.LogFormat ("伤害值{0}/{1}", tintStr,self.name);

			if(self.agent.healthAbsorbScalser > 0){

				int healthGain = (int)(actualDamage * self.agent.healthAbsorbScalser);

				self.agent.health += healthGain;

				if(self.agent.health > self.agent.maxHealth){
					self.agent.health = self.agent.maxHealth;
				}

				string gainStr = string.Format("<color=green>{0}</color>",healthGain);

				self.PlayGainTextAnim(gainStr);
			}

			foreach (CallBack cb in self.attackTriggerCallBacks) {
				cb ();
			}

			foreach (CallBack cb in enemy.beAttackedTriggerCallBacks) {
				cb ();
			}

		}

	}
}
