using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	public class ConsumablesPropertyChangeDurative : ConsumablesSkill {

		private float propertyChange;// 记录己方属性变化值，战斗结束后重置



		private IEnumerator effectCoroutine;

//		private List<IEnumerator> effectCoroutines = new List<IEnumerator>();

		protected override void ExcuteNoneTriggeredSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			propertyChange = 0;

			ExcuteConsumablesSkillLogic (self);
		}

		protected override void ExcuteConsumablesSkillLogic (BattleAgentController self)
		{

			// 如果没有状态名称，则默认不是触发状态，直接执行技能
			if (statusName == "") {
				Excute (self);
				return;
			}

			List<TriggeredSkill> sameStatusSkills = self.propertyCalculator.GetTriggeredSkillsWithSameStatus (statusName);



			// 如果技能效果不可叠加，则被影响人身上原有的同种状态技能效果全部取消，并从战斗结算器中移除这些技能
			if (!canOverlay) {
				for (int i = 0; i < sameStatusSkills.Count; i++) {
					TriggeredSkill ts = sameStatusSkills [i];
					ts.CancelSkillEffect ();
				}
			}

			Excute (self);
		}


		private void Excute(BattleAgentController self){

			// 创建持续性状态的执行协程
			effectCoroutine = ExcuteAgentPropertyChange (self);
			// 开启持续性状态的协程
			StartCoroutine (effectCoroutine);

			self.propertyCalculator.AddSkill<ConsumablesSkill> (this);

		}

		private IEnumerator ExcuteAgentPropertyChange(BattleAgentController self){

			float timer = 0f;

			while (timer < duration) {

				self.InitFightTextDirectionTowards (self.transform.position);

				self.propertyCalculator.InstantPropertyChange (self, propertyType, skillSourceValue, false);

				propertyChange += skillSourceValue;

				timer += 1f;

				yield return new WaitForSeconds (1.0f);

			}

			self.propertyCalculator.RemoveTriggeredSkill (this);
			self = null;
			Destroy (this.gameObject);
		}

		public override void CancelSkillEffect (BattleAgentController self)
		{
			if (effectCoroutine != null) {
				StopCoroutine (effectCoroutine);
			}

			self.propertyCalculator.RemoveTriggeredSkill (this);
			self = null;
			Destroy (this.gameObject);

			if (propertyType == PropertyType.Health) {
				return;
			}
			self.propertyCalculator.AgentPropertyChange (propertyType, -propertyChange,false);

		}
	}
}
