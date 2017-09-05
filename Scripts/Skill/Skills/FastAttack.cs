using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class FastAttack : Skill	{

//		private BattleAgentController baCtr;
		private int originAttackSpeed;

		void Awake(){
			isPassive = true;
			baseNum = 0.05f;
			skillName = "速击";
			skillDescription = string.Format("提升<color=orange>{0}*技能等级%</color>的攻速",(int)(baseNum * 100));
		}

		public override void AffectAgents (BattleAgentController self, BattleAgentController enemy)
		{
//			baCtr = self;
//			originAttackSpeed = self.agent.attackSpeed;
			self.agent.attackSpeed = (int)(self.agent.attackSpeed * (1 + baseNum * skillLevel));
//			self.PlaySkillEffect (skillName);
//			Invoke ("EndFastAttack", duration);
		}
	
//		private void EndFastAttack(){
//			baCtr.agent.attackSpeed = originAttackSpeed;
//			baCtr.EndSkillEffect (skillName);
//		}

	}
}