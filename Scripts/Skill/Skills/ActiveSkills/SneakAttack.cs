using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace WordJourney
{
	public class SneakAttack : ActiveSkill {

		public int hurtBase;

		public float hurtScalerBase;

		public float healthAbsorbScaler;

		void Awake(){
			skillType = SkillType.Physical;
			skillName = "背刺";
			skillDescription = string.Format ("对敌方造成额外<color=orange>{0}+{1}*技能等级*攻击力点</color>的物理伤害,并将实际伤害的<color=orange>{2}%</color>转化为自身生命值(不可闪避)", hurtBase, (int)(hurtScalerBase * 100),(int)(healthAbsorbScaler * 100));

		}

		protected override void ExcuteSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{

			self.agent.mana -= manaConsume;

			int originalDamage = (int)((self.agent.attack * (hurtScalerBase * skillLevel + 1) + hurtBase) * (1 + self.agent.physicalHurtScaler));

			//抵消护甲作用后的实际伤害值
			int actualDamage = (int)(originalDamage / (1 + armorSeed * enemy.agent.armor) + 1f);

			enemy.agent.health -= actualDamage;

			string tintStr = string.Format("<color=red>{0}</color>",actualDamage);

			Debug.LogFormat ("造成{0}点伤害", tintStr);

			enemy.PlayHurtTextAnim(tintStr,TintTextType.None);

			int healthAbsorb = (int)(actualDamage * healthAbsorbScaler + 0.5f);

			self.agent.health += healthAbsorb;

			string gainStr = string.Format ("<color=green>+{0}</color>",healthAbsorb);

			Debug.LogFormat ("吸血{0}", gainStr);

			self.PlayGainTextAnim (gainStr);

			// 计算反弹伤害
			if (enemy.agent.reflectScaler > 0) {

				int reflectDamage = (int)((actualDamage - originalDamage) * enemy.agent.reflectScaler);

				self.agent.health -= reflectDamage;

				string hurtStr = string.Format ("<color=red>{0}</color>", reflectDamage);

				self.PlayHurtTextAnim (hurtStr, TintTextType.None);
			}

		}
	}
}