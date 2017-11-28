using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class IceExplosion : ActiveSkill {

		public string stateName;

		public int attackSpeedDecreaseBase;

		public float magicHurtScalerBase;

		public int magicHurtBase;

		public bool removeWhenQuitFight;

		void Awake(){
			skillType = SkillType.Magical;
			skillName = "冰爆";
			skillDescription = string.Format ("对敌方造成<color=orange>{0}+{1}*技能等级%*最大魔法值</color>点魔法伤害,并较少对方<color=orange>{2}*技能等级%的攻速直至战斗结束(可叠加)",magicHurtBase, (int)(magicHurtScalerBase * 100), attackSpeedDecreaseBase);
		}

		protected override void ExcuteSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{

			self.agent.mana -= manaConsume;

			// 首次触发时添加状态，并执行状态效果
			if (!enemy.CheckStateExist (stateName)) {
				SkillState state = new SkillState (this, stateName, removeWhenQuitFight, null);
				enemy.states.Add (state);
				enemy.agent.attackSpeed -= attackSpeedDecreaseBase;
			}

			// 如果可以叠加，每次使用该技能都会减对方攻速
			if (canOverlay) {
				enemy.agent.attackSpeed -= attackSpeedDecreaseBase;
			} 

			if (enemy.agent.attackSpeed < 0) {
				enemy.agent.attackSpeed = 0;
			}

			//原始魔法伤害值
			int originalDamage = (int)((self.agent.maxMana * magicHurtScalerBase * skillLevel + magicHurtBase) * (1 + enemy.agent.magicalHurtScaler));

			//抵消魔抗作用后的实际伤害值
			int actualDamage = (int)(originalDamage / (1 + magicResistSeed * enemy.agent.manaResist) + 0.5f);

			enemy.agent.health -= actualDamage;

			string tintStr = string.Format("<color=blue>{0}</color>",actualDamage);

			enemy.PlayHurtTextAnim(tintStr,TintTextType.None);

			// 计算反弹伤害
			if (enemy.agent.reflectScaler > 0) {

				int reflectDamage = (int)(actualDamage * enemy.agent.reflectScaler);

				self.agent.health -= reflectDamage;

				string hurtStr = string.Format ("<color=red>{0}</color>", reflectDamage);

				self.PlayHurtTextAnim (hurtStr, TintTextType.None);
			}

		}

	}
}
