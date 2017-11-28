using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class ArmorPenetration : ActiveSkill {

		public int hurtBase;

		public float hurtScalerBase;

		public int armorDecreaseBase;

		public string stateName;

		public bool removeWhenQuitFight;

		void Awake(){
			skillType = SkillType.Physical;
			skillDescription = string.Format("造成额外<color=orange>{0}+{1}*技能等级*攻击力点</color>的物理伤害,并减少对方<color=orange>{2}点</color>护甲直至战斗结束(可叠加)", hurtBase, (int)(hurtScalerBase*100),armorDecreaseBase);
		}

		protected override void ExcuteSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{

			self.agent.mana -= manaConsume;

			// 首次触发时添加状态，并执行状态效果
			if (!enemy.CheckStateExist (stateName)) {
				
				SkillState state = new SkillState (this, stateName, removeWhenQuitFight, null);

				enemy.states.Add (state);

				enemy.agent.armor -= armorDecreaseBase;
			}

			// 如果可以叠加，每次使用该技能都会减对方护甲，如果不能叠加，则首次使用该技能时减对方护甲
			if (canOverlay) {
				enemy.agent.armor -= armorDecreaseBase;
			}
				
			if (enemy.agent.armor < 0) {
				enemy.agent.armor = 0;
			}
				
			int originalDamage = (int)((self.agent.attack * (hurtScalerBase * skillLevel + 1) + hurtBase) * (1 + self.agent.physicalHurtScaler));

			//抵消护甲作用后的实际伤害值
			int actualDamage = (int)(originalDamage / (1 + armorSeed * enemy.agent.armor) + 0.5f);

			enemy.agent.health -= actualDamage;


			string tintStr = string.Format("<color=red>{0}</color>",actualDamage);

			enemy.PlayHurtTextAnim(tintStr,TintTextType.None);


			int damageOffset = originalDamage - actualDamage;

			if (enemy.agent.reflectScaler > 0) {

				int reflectDamage = (int)(damageOffset * enemy.agent.reflectScaler);

				self.agent.health -= reflectDamage;

				string hurtStr = string.Format ("<color=red>{0}</color>", reflectDamage);

				self.PlayHurtTextAnim (hurtStr, TintTextType.None);
			}



		}

	}
}
