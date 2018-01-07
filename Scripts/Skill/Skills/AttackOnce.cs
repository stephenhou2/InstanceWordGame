using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class AttackOnce : ActiveSkill {

//		private float critScaler = 1.0f;

		private float dodgeSeed = 0.0035f; //计算闪避时的种子数



		protected override void ExcuteNoneTriggeredSkillLogic(BattleAgentController self, BattleAgentController enemy){

//			TintTextType tintTextType = TintTextType.None;

			// 执行攻击触发事件回调
			foreach (TriggeredSkillExcutor excutor in self.attackTriggerExcutors) {
				switch (excutor.effectTarget) {
				case SkillEffectTarget.Self:
					excutor.triggeredCallback (self, enemy);
					break;
				case SkillEffectTarget.Enemy:
					excutor.triggeredCallback (enemy, self);
					break;
				}
			}

			// 敌方执行被攻击触发事件回调
			foreach (TriggeredSkillExcutor excutor in enemy.beAttackedTriggerExcutors) {
				switch (excutor.effectTarget) {
				case SkillEffectTarget.Self:
					excutor.triggeredCallback (enemy, self);
					break;
				case SkillEffectTarget.Enemy:
					excutor.triggeredCallback (self, enemy);
					break;
				}
			}
				
			//计算对方闪避率(敌方的基础闪避率 - 己方的闪避修正)
			int enemyDodge = enemy.propertyCalculator.dodge;
			float dodgeProbability = dodgeSeed * enemyDodge / (1 + dodgeSeed * enemyDodge) - self.propertyCalculator.dodgeFixScaler;
			//判断对方是否闪避成功
			if(isEffective(dodgeProbability)){
				enemy.propertyCalculator.specialAttackResult = SpecialAttackResult.Miss;
				enemy.AddFightTextToQueue (string.Empty, SpecialAttackResult.Miss);
//				tintTextType = TintTextType.Miss;
//				enemy.PlayHurtTextAnim(string.Empty,tintTextType);
				return;
			}

			//原始物理伤害值

			int oriPhysicalHurt = self.propertyCalculator.attack;
			self.propertyCalculator.physicalHurtFromNomalAttack = oriPhysicalHurt;
//			Debug.LogFormat ("{0}普通攻击造成{1}物理伤害", self.agent.agentName, oriPhysicalHurt);


			// 执行己方攻击命中的回调
			foreach (TriggeredSkillExcutor excutor in self.hitTriggerExcutors) {
				switch (excutor.effectTarget) {
				case SkillEffectTarget.Self:
					excutor.triggeredCallback (enemy, self);
					break;
				case SkillEffectTarget.Enemy:
					excutor.triggeredCallback (self, enemy);
					break;
				}
			}

			// 执行敌方被击中的回调
			foreach (TriggeredSkillExcutor excutor in enemy.beHitTriggerExcutors) {
				switch (excutor.effectTarget) {
				case SkillEffectTarget.Self:
					excutor.triggeredCallback (enemy, self);
					break;
				case SkillEffectTarget.Enemy:
					excutor.triggeredCallback (self, enemy);
					break;
				}
			}

			self.propertyCalculator.CalculateAttackHurt ();
			enemy.propertyCalculator.CalculateAttackHurt ();

			self.propertyCalculator.CalculateAgentHealth ();
			enemy.propertyCalculator.CalculateAgentHealth ();

			self.UpdateFightStatus ();
			enemy.UpdateFightStatus ();

			self.propertyCalculator.ResetAllHurt ();
			enemy.propertyCalculator.ResetAllHurt ();

			self.UpdateStatusPlane ();
			enemy.UpdateStatusPlane ();

			enemy.PlayShakeAnim ();

//			//计算己方的暴击率（己方的基础暴击率 - 敌方的暴久修正）
//			int selfCrit = self.propertyCalculator.crit;
//			float critProbability = critSeed * selfCrit / (1 + critSeed * selfCrit) - enemy.agent.dodgeFixScaler;
//
//			// 判断是否打出暴击
//			bool isCrit = isEffective (critProbability);
//
//			if (isCrit) {
//				critScaler = self.agent.critHurtScaler;
//				tintTextType = TintTextType.Crit;
//			}


			//原始物理伤害值
//			int originalPhysicalDamage = (int)(self.agent.attack * (1 + self.agent.physicalHurtScaler) * critScaler);
			//抵消护甲后的物理伤害值
//			int physicalDamageAfterArmor = (int)(originalPhysicalDamage / (1 + armorSeed * enemy.agent.armor) );
			//抵消的物理伤害值
//			int physicalDamageOffset = originalPhysicalDamage - physicalDamageAfterArmor;

			//原始魔法伤害值
//			int originalMagicalDamage = (int)(self.agent.maxMana * self.agent.attachMagicHurtScaler + 0.9f);
			//抵消魔抗后的魔法伤害值
//			int magicalDamageAfterResist = (int)(originalMagicalDamage / (1 + magicResistSeed * enemy.agent.magicResist) + 0.9f);
//			int magicalDamageOffset = originalMagicalDamage - magicalDamageAfterResist;

			// 攻击之后将暴击伤害率重新设定为1
//			critScaler = 1.0f;

			// 计算反弹伤害
//			if (enemy.agent.reflectScaler > 0) {
//
//				int reflectDamage = (int)((physicalDamageOffset + magicalDamageOffset) * enemy.agent.reflectScaler);
//
//				self.agent.health -= reflectDamage;
//
//				string hurtStr = string.Format ("<color=red>{0}</color>", reflectDamage);
//
//				self.PlayHurtTextAnim (hurtStr, TintTextType.None);
//			}

//			int actualPhysicalDamage = (int)(physicalDamageAfterArmor * (1 - enemy.agent.decreaseHurtScaler));
//
//			int actualMagicalDamage = (int)(magicalDamageAfterResist * (1 - enemy.agent.decreaseHurtScaler));

//			actualDamage = (int)(actualDamage * (1 - enemy.agent.decreaseHurtScaler));

//			enemy.agent.health -= (actualPhysicalDamage + actualMagicalDamage);
//
//			if (actualPhysicalDamage > 0) {
//
//				string physicalHurtStr = string.Format ("<color=red>{0}</color>", actualPhysicalDamage);
//
//				enemy.PlayHurtTextAnim (physicalHurtStr, tintTextType);
//			}
//
//			if (actualMagicalDamage > 0) {
//
//				string magicalHurtStr = string.Format ("<color=blue>{0}</color>", actualMagicalDamage);
//
//				enemy.PlayHurtTextAnim (magicalHurtStr, TintTextType.None, 0.2f);
//
//			}
//
//			if(self.agent.healthAbsorbScalser > 0){
//
//				int healthGain = (int)(actualPhysicalDamage * self.agent.healthAbsorbScalser);
//
//				self.agent.health += healthGain;
//
//				string gainStr = string.Format("<color=green>{0}</color>",healthGain);
//
//				self.PlayGainTextAnim(gainStr);
//			}
				
//			foreach (SkillCallBack attackFinishCallBack in self.attackFinishTriggerCallBacks) {
//				attackFinishCallBack (self,enemy);
//			}

//			enemy.PlayShakeAnim ();

		}

	}
}
