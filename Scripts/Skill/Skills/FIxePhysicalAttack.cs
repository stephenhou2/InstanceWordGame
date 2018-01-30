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
			for(int i = 0;i < self.attackTriggerExcutors.Count; i++) {
				TriggeredSkillExcutor excutor = self.attackTriggerExcutors[i];
				switch (excutor.triggerSource) {
				case SkillEffectTarget.Self:
					excutor.triggeredCallback (self, enemy);
					break;
				case SkillEffectTarget.Enemy:
					excutor.triggeredCallback (enemy, self);
					break;
				}
			}

			// 敌方执行被攻击触发事件回调
			for(int i = 0; i<enemy.beAttackedTriggerExcutors.Count; i++) {
				TriggeredSkillExcutor excutor = enemy.beAttackedTriggerExcutors[i];
				switch (excutor.triggerSource) {
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

			enemy.PlayShakeAnim ();

			//原始物理伤害值
			self.propertyCalculator.physicalHurtToEnemy += skillSourceValue;

			SetEffectAnims (self, enemy);

			// 执行己方攻击命中的回调
			for(int i = 0;i<self.hitTriggerExcutors.Count;i++) {
				TriggeredSkillExcutor excutor = self.hitTriggerExcutors[i];
				switch (excutor.triggerSource) {
				case SkillEffectTarget.Self:
					excutor.triggeredCallback (self, enemy);
					break;
				case SkillEffectTarget.Enemy:
					excutor.triggeredCallback (enemy, self);
					break;
				}
			}
				

			// 执行敌方被击中的回调
			for(int i = 0;i < enemy.beHitTriggerExcutors.Count; i++) {
				TriggeredSkillExcutor excutor = enemy.beHitTriggerExcutors[i];
				switch (excutor.triggerSource) {
				case SkillEffectTarget.Self:
					excutor.triggeredCallback (enemy, self);
					break;
				case SkillEffectTarget.Enemy:
					excutor.triggeredCallback (self, enemy);
					break;
				}
			}


			self.propertyCalculator.CalculateAgentHealth ();
			enemy.propertyCalculator.CalculateAgentHealth ();


			self.UpdateFightStatus ();
			enemy.UpdateFightStatus ();

			self.UpdateStatusPlane ();
			enemy.UpdateStatusPlane ();

			self.propertyCalculator.ResetAllHurt ();
			enemy.propertyCalculator.ResetAllHurt ();

		

		}

	}
}
