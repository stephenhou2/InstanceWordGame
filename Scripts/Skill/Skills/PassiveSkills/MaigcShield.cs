using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class MaigcShield : PassiveSkill {

		public int duration;

		public float decreaseHurtScaler;

		public float probabilityBase;

		private Coroutine magicShieldCoroutine;

		void Awake(){
			skillType = SkillType.Passive;
			skillName = "魔法盾";
			skillDescription = string.Format ("受到攻击时有<color=orange>{0}*技能等级%</color>的概率产生一个魔法盾,魔法盾存期间可以减少<color=orange>{1}%</color>的伤害，持续时间<color=orange>{2}s</color>",(int)(probabilityBase * 100),decreaseHurtScaler,duration);
			selfEffectName = "MagicShield";
		}

		protected override void ExcuteSkillLogic (BattleAgentController self, BattleAgentController enemy){
		
		}

		protected override void BeAttackedTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			float createShieldProbability = probabilityBase * skillLevel;

			if (isEffective (createShieldProbability)) {

				StopCoroutine (magicShieldCoroutine);

				self.SetEffectAnim (selfEffectName, true);

				self.agent.decreaseHurtScaler = decreaseHurtScaler;

				magicShieldCoroutine = StartCoroutine ("EndMagicShield",self);
			}

		}

		private IEnumerator EndMagicShield(BattleAgentController targetBa){
			yield return new WaitForSeconds (duration);
			targetBa.SetEffectAnim (selfEffectName, false);
			targetBa.agent.decreaseHurtScaler = 0;
		}

	}
}
