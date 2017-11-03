using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace WordJourney
{
	public class SneakAttack : Skill {

		public int hurtScaler;

		public int absorbBase;

		void Awake(){
			isPassive = false;
			skillType = SkillType.Physical;
			baseNum = 0.04f;
			hurtScaler = 2;
			absorbBase = 10;
			skillName = "背刺";
			skillDescription = string.Format ("对敌方造成<color=orange>{0}*攻击力</color>点物理伤害,并将实际伤害的<color=orange>{1}%</color>转化为自身生命值",hurtScaler,absorbBase + (int)(baseNum * skillLevel));
			selfAnimName = "stand";
		}

		public override void AffectAgents (BattleAgentController self, BattleAgentController enemy)
		{

			Debug.Log(string.Format("使用了{0}",skillName));

			int originalDamage = (int)(self.agent.attack * hurtScaler * (1 + self.agent.physicalHurtScaler) + 0.5f);

			//抵消护甲作用后的实际伤害值
			int actualDamage = (int)(originalDamage / (1 + armorSeed * enemy.agent.armor) + 1f);

			enemy.agent.health -= actualDamage;

			if(enemy.agent.health < 0){
				enemy.agent.health = 0;
			}

			string tintStr = string.Format("<color=red>{0}</color>",actualDamage);

			Debug.LogFormat ("造成{0}点伤害", tintStr);

			enemy.PlayHurtTextAnim(tintStr,TintTextType.None);

			int healthAbsorb = (int)(actualDamage * (absorbBase / 100f + baseNum * skillLevel) + 1f);

			self.agent.health += healthAbsorb;

			if (self.agent.health > self.agent.maxHealth) {
				self.agent.health = self.agent.maxHealth;
			}

			string gainStr = string.Format ("<color=green>+{0}</color>",healthAbsorb);

			Debug.LogFormat ("吸血{0}", gainStr);

			self.PlayGainTextAnim (gainStr);

		}
	}
}