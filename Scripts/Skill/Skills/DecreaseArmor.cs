using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class DecreaseArmor : Skill {


		public float hurtScaler;

		void Awake(){
			isPassive = false;
			skillType = SkillType.Physical;
			baseNum = 0.05f;
			skillName = "破甲";
			hurtScaler = 2f;
			skillDescription = string.Format("造成<color=orange>{0}*攻击力</color>的物理伤害,并减少对方<color=orange>{1}＊技能等级％</color>的护甲直至战斗结束(可叠加)",hurtScaler,(int)(baseNum*100));

		}

		public override void AffectAgents (BattleAgentController self, BattleAgentController enemy)
		{
			enemy.agent.armor = (int)(enemy.agent.armor * (1 - baseNum * skillLevel));

//			enemy.PlaySkillEffect (skillName,state);

			int originalDamage = (int)(self.agent.attack * hurtScaler * self.agent.physicalHurtScaler + 0.5f);

			//抵消护甲作用后的实际伤害值
			int actualDamage = (int)(originalDamage / (1 + armorSeed * enemy.agent.armor) + 0.5f);

			enemy.agent.health -= actualDamage;

			if(enemy.agent.health < 0){
				enemy.agent.health = 0;
			}

			string tintStr = string.Format("<color=red>{0}</color>",actualDamage);

			enemy.PlayHurtTextAnim(tintStr,TintTextType.None);


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
		}

	}
}
