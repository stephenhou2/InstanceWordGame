using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class FireExplosion : Skill {

		private int mMagicBase = 30;
		public int magicBase{
			get{ return mMagicBase; }
			set{
				mMagicBase = value;
				skillDescription = string.Format ("对敌方造成<color=orange>{0}*技能等级</color>点魔法伤害,并较少对方<color=orange>{1}*技能等级%的攻速直至战斗结束(可叠加)", magicBase, baseNum);
			}
		}

		void Awake(){
			isPassive = false;
			skillType = SkillType.Magic;
			baseNum = 0.05f;
			skillName = "炎爆";
			skillDescription = string.Format ("对敌方造成<color=orange>{0}*技能等级</color>点魔法伤害,并较少对方<color=orange>{1}*技能等级%的攻速直至战斗结束(可叠加)", magicBase, baseNum);
		}

		public override void AffectAgents (BattleAgentController self, BattleAgentController enemy)
		{

			enemy.agent.attackSpeed = (int)(enemy.agent.attackSpeed * (1 - baseNum * skillLevel)); 

//			enemy.PlaySkillEffect (skillName,state);

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
