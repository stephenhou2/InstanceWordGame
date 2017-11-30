using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class Thunder : ActiveSkill {

//		private int mMagicBase = 50;
//		public int magicBase{
//			get{ return mMagicBase; }
//			set{
//				mMagicBase = value;
//				skillDescription = string.Format ("对敌方造成<color=orange>{0}*技能等级</color>点魔法伤害", mMagicBase);
//			}
//		}

		public int magicHurtBase;
		public float magicHurtScalerBase;


		void Awake(){
			skillType = SkillType.Magical;
			skillName = "天雷";
			skillDescription = string.Format ("对敌方造成<color=orange>{0}+{1}*技能等级%*最大魔法值</color>点魔法伤害",magicHurtBase,(int)(magicHurtScalerBase * 100));
		}
			
		protected override void ExcuteSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{

			self.agent.mana -= this.manaConsume;

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
