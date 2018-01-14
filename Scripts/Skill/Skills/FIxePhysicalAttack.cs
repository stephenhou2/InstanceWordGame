using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	public class FIxePhysicalAttack : ActiveSkill {

		private float dodgeSeed = 0.0035f; //计算闪避时的种子数
		public int skillSourceValue;//魔法伤害值


		protected override void ExcuteNoneTriggeredSkillLogic(BattleAgentController self, BattleAgentController enemy){
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
			if (isEffective (dodgeProbability)) {
				enemy.propertyCalculator.specialAttackResult = SpecialAttackResult.Miss;
				enemy.AddFightTextToQueue (string.Empty, SpecialAttackResult.Miss);
				return;
			}

			//原始魔法伤害值
			self.propertyCalculator.physicalHurtFromNomalAttack += skillSourceValue;
//			Debug.LogFormat ("{0}魔法攻击造成{1}魔法伤害", self.agent.agentName, oriPhysicalHurt);


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

		}

	}
}
