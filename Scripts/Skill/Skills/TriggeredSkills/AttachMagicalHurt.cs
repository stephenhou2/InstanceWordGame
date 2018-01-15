using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	public class AttachMagicalHurt : TriggeredSkill {


		protected override void BeforeFightTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			ExcuteTriggeredSkillLogic (beforeFightTriggerInfo, self, enemy);
		}

		protected override void AttackTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			ExcuteTriggeredSkillLogic (attackTriggerInfo, self, enemy);
		}

		protected override void HitTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			ExcuteTriggeredSkillLogic (hitTriggerInfo, self, enemy);
		}

		protected override void BeAttackedTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			ExcuteTriggeredSkillLogic (beAttackedTriggerInfo, self, enemy);
		}

		protected override void BeHitTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			ExcuteTriggeredSkillLogic (beHitTriggerInfo, self, enemy);
		}

		protected override void ExcuteTriggeredSkillLogic (TriggerInfo triggerInfo, BattleAgentController self, BattleAgentController enemy)
		{
			if(isEffective(triggeredProbability)){
				BattleAgentController skillUser = GetSkillUser (triggerInfo, self, enemy);
				int mana = skillUser.propertyCalculator.mana;
				int attachedMagicalHurt = (int)(skillSourceValue * mana);
				skillUser.propertyCalculator.magicalHurtToEnemy += attachedMagicalHurt;
				SetEffectAnims (triggerInfo, self, enemy);
			}
		}


	}
}
