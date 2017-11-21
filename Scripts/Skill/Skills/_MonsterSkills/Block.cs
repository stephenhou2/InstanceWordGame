using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	// 格挡这个被动加在攻击方身上
	public class Block : PassiveSkill {

		public float decreasePhysicalHurtDecreaseScaler;

		private float originalPhysicalHurtScaler;

		public float probabilityBase;

		void Awake(){
			this.skillType = SkillType.Passive;
		}
			
		protected override void ExcuteSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			originalPhysicalHurtScaler = enemy.agent.physicalHurtScaler;
		}

		protected override void AttackTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			if(!isEffective(probabilityBase * skillLevel)){
				return;
			}

			enemy.agent.physicalHurtScaler *= (1 - decreasePhysicalHurtDecreaseScaler * skillLevel);
		}

		protected override void AttackFinishTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			enemy.agent.physicalHurtScaler = originalPhysicalHurtScaler;
		}


	}
}
