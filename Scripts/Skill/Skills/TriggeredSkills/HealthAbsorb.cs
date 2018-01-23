using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class HealthAbsorb : TriggeredSkill {

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



		protected override void ExcuteTriggeredSkillLogic(TriggerInfo triggerInfo, BattleAgentController self,BattleAgentController enemy){


			if (isEffective (triggeredProbability)) {

				BattleAgentController skillUser = GetSkillUser (triggerInfo, self, enemy);

				int physicalHurt = skillUser.propertyCalculator.physicalHurtToEnemy;

				int healthAbsorb = (int)(skillSourceValue * physicalHurt);

				skillUser.propertyCalculator.InstantPropertyChange (skillUser, PropertyType.Health, healthAbsorb, true);

//				skillUser.propertyCalculator.healthAbsorb += healthAbsorb;

				SetEffectAnims (triggerInfo, self, enemy);

			}

		}

	}
}
