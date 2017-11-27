using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class AttackOnce : Skill {

		private float critScaler = 1.0f;

		public override void AffectAgents (BattleAgentController self, BattleAgentController enemy){
			ExcuteSkillLogic (self, enemy);
		}

		protected override void ExcuteSkillLogic(BattleAgentController self, BattleAgentController enemy){

			TintTextType tintTextType = TintTextType.None;
				
			//计算对方闪避率(敌方的基础闪避率 - 己方的闪避修正)
			float dodge = dodgeSeed * enemy.agent.dodge / (1 + dodgeSeed * enemy.agent.dodge) - self.agent.dodgeFixScaler;
			//判断对方是否闪避成功
			if(isEffective(dodge)){
				tintTextType = TintTextType.Miss;
				enemy.PlayHurtTextAnim(string.Empty,tintTextType);
				return;
			}


			// 执行攻击触发事件回调
			foreach (SkillCallBack cb in self.attackTriggerCallBacks) {
				cb (self,enemy);
			}

			// 敌方执行被攻击触发事件回调
			foreach (SkillCallBack cb in enemy.beAttackedTriggerCallBacks) {
				cb (enemy,self);
			}


			//计算己方的暴击率（己方的基础暴击率 - 敌方的暴久修正）
			float crit = critSeed * self.agent.crit / (1 + critSeed * self.agent.crit) - enemy.agent.dodgeFixScaler;

			//是否打出暴击
			bool isCrit = isEffective (crit);

			if (isCrit) {
				critScaler = self.agent.critHurtScaler;
				tintTextType = TintTextType.Crit;
			}

			//原始物理伤害值
			int originalPhysicalDamage = (int)(self.agent.attack * (1 + self.agent.physicalHurtScaler) * critScaler);
			//抵消护甲后的物理伤害值
			int physicalDamageAfterArmor = (int)(originalPhysicalDamage / (1 + armorSeed * enemy.agent.armor) );
			//抵消的物理伤害值
			int physicalDamageOffset = originalPhysicalDamage - physicalDamageAfterArmor;

			//原始魔法伤害值
			int originalMagicalDamage = (int)(self.agent.maxMana * self.agent.attachMagicHurtScaler + 0.9f);
			//抵消魔抗后的魔法伤害值
			int magicalDamageAfterResist = (int)(originalMagicalDamage / (1 + magicResistSeed * enemy.agent.manaResist) + 0.9f);

			int magicalDamageOffset = originalMagicalDamage - magicalDamageAfterResist;

			// 攻击之后将暴击伤害率重新设定为1
			critScaler = 1.0f;

			// 计算反弹伤害
			if (enemy.agent.reflectScaler > 0) {

				int reflectDamage = (int)((physicalDamageOffset + magicalDamageOffset) * enemy.agent.reflectScaler);

				self.agent.health -= reflectDamage;

				string hurtStr = string.Format ("<color=red>{0}</color>", reflectDamage);

				self.PlayHurtTextAnim (hurtStr, TintTextType.None);
			}

			int actualPhysicalDamage = (int)(physicalDamageAfterArmor * (1 - enemy.agent.decreaseHurtScaler));

			int actualMagicalDamage = (int)(magicalDamageAfterResist * (1 - enemy.agent.decreaseHurtScaler));

//			actualDamage = (int)(actualDamage * (1 - enemy.agent.decreaseHurtScaler));

			enemy.agent.health -= (actualPhysicalDamage + actualMagicalDamage);

			if (actualPhysicalDamage > 0) {

				string physicalHurtStr = string.Format ("<color=red>{0}</color>", actualPhysicalDamage);

				enemy.PlayHurtTextAnim (physicalHurtStr, tintTextType);
			}

			if (actualMagicalDamage > 0) {

				string magicalHurtStr = string.Format ("<color=blue>{0}</color>", actualMagicalDamage);

				enemy.PlayHurtTextAnim (magicalHurtStr, TintTextType.None, 0.2f);

			}

			if(self.agent.healthAbsorbScalser > 0){

				int healthGain = (int)(actualPhysicalDamage * self.agent.healthAbsorbScalser);

				self.agent.health += healthGain;

				string gainStr = string.Format("<color=green>{0}</color>",healthGain);

				self.PlayGainTextAnim(gainStr);
			}
				
			foreach (SkillCallBack attackFinishCallBack in self.attackFinishTriggerCallBacks) {
				attackFinishCallBack (self,enemy);
			}

			enemy.PlayShakeAnim ();

		}




	}
}
