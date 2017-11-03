using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class MaigcShield : Skill {

		public int duration;
		public float decreaseHurtScaler;

		private BattleAgentController baCtr;

		void Awake(){
			isPassive = true;
			skillType = SkillType.Passive;
			duration = 2;
			decreaseHurtScaler = 0.5f;
			baseNum = 0.5f;
			skillName = "魔法盾";
			skillDescription = string.Format ("受到攻击时有<color=orange>{0}*技能等级%</color>的概率产生一个魔法盾,魔法盾存期间可以减少<color=orange>{1}%</color>的伤害，持续时间<color=orange>{2}s</color>",(int)(baseNum * 100),decreaseHurtScaler,duration);
			selfEffectName = "MagicShield";
		}

		public override void AffectAgents (BattleAgentController self, BattleAgentController enemy)
		{
			baCtr = self;
			self.beAttackedTriggerCallBacks.Add (CreateMagicShield);
		}

		private void CreateMagicShield(){

			float createShieldChance = baseNum * skillLevel;

			if (isEffective (createShieldChance)) {

				CancelInvoke ("EndMagicShield");

				baCtr.SetEffectAnim (selfEffectName, true);

				baCtr.agent.decreaseHurtScaler = decreaseHurtScaler;

				Invoke ("EndMagicShield", duration);
			}

		}

		private void EndMagicShield(){
			baCtr.SetEffectAnim (selfEffectName, false);
			baCtr.agent.decreaseHurtScaler = 0;
		}

	}
}
