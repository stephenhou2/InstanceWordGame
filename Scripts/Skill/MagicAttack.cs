using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class MagicAttack : Skill {
		
		public int hurtBase = 50;


		public override void AffectAgents (BattleAgentController self, BattleAgentController enemy)
		{

			//原始魔法伤害值
			int originalDamage = (int)(hurtBase * skillLevel * enemy.agent.magicalHurtScaler);

			//抵消魔抗作用后的实际伤害值
			int actualDamage = (int)(originalDamage / (1 + magicResistSeed * enemy.agent.manaResist) + 0.5f);

			enemy.agent.health -= actualDamage;

			string tintStr = string.Format("<color=blue>{0}</color>",actualDamage);

			enemy.PlayHurtTextAnim(tintStr,TintTextType.None);

			if(enemy.agent.health < 0){
				enemy.agent.health = 0;
			}

		}

	}
}
