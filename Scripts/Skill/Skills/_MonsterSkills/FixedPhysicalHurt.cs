using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class FixedPhysicalHurt : ActiveSkill {


		public int fixedHurt;

		void Awake(){
//			this.isPassive = false;
			this.skillType = SkillType.Physical;
		}

		protected override void ExcuteSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{

			// 执行攻击触发事件回调
			foreach (SkillCallBack cb in self.attackTriggerCallBacks) {
				cb (self,enemy);
			}

			// 敌方执行被攻击触发事件回调
			foreach (SkillCallBack cb in enemy.beAttackedTriggerCallBacks) {
				cb (self,enemy);
			}


			//抵消护甲后的物理伤害值
			int physicalDamageAfterArmor = (int)(fixedHurt / (1 + armorSeed * enemy.agent.armor) );
			//抵消的物理伤害值
			int physicalDamageOffset = fixedHurt - physicalDamageAfterArmor;

			if (enemy.agent.reflectScaler > 0) {

				int reflectDamage = (int)(physicalDamageOffset * enemy.agent.reflectScaler);

				self.agent.health -= reflectDamage;

				string hurtStr = string.Format ("<color=red>{0}</color>", reflectDamage);

				self.PlayHurtTextAnim (hurtStr, TintTextType.None);
			}

			int actualPhysicalDamage = (int)(physicalDamageAfterArmor * (1 - enemy.agent.decreaseHurtScaler));

			enemy.agent.health -= actualPhysicalDamage;

			string tintStr = string.Format("<color=red>{0}</color>",actualPhysicalDamage);

			enemy.PlayHurtTextAnim(tintStr,TintTextType.None);

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
