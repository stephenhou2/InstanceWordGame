using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class Thunder : Skill {

		private int mMagicBase = 50;
		public int magicBase{
			get{ return mMagicBase; }
			set{
				mMagicBase = value;
				skillDescription = string.Format ("对敌方造成<color=orange>{0}*技能等级</color>点魔法伤害", mMagicBase);
			}
		}

		void Awake(){
			isPassive = false;
			skillType = SkillType.Magic;
			skillName = "天雷";
			skillDescription = string.Format ("对敌方造成<color=orange>{0}*技能等级</color>点魔法伤害", mMagicBase);
			enemyEffectName = "Thunder";
			coolenInterval = 2f;
		}
			
		public override void AffectAgents (BattleAgentController self, BattleAgentController enemy)
		{

			self.agent.mana -= this.manaConsume;

			//原始魔法伤害值
			int originalDamage = (int)(self.agent.magicBase * skillLevel * (1 + enemy.agent.magicalHurtScaler));

			//抵消魔抗作用后的实际伤害值
			int actualDamage = (int)(originalDamage / (1 + magicResistSeed * enemy.agent.manaResist) + 0.5f);

			enemy.agent.health -= actualDamage;

			if(enemy.agent.health < 0){
				enemy.agent.health = 0;
			}

			string tintStr = string.Format("<color=blue>{0}</color>",actualDamage);

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
