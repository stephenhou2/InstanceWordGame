using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class ConsumablesPropertyChangeOnce : ConsumablesSkill {

		private float propertyChange;// 记录己方属性变化值，战斗结束后重置

//		public PropertyType propertyType;

		private IEnumerator ResetPropertyCoroutine;


		protected override void ExcuteNoneTriggeredSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			// 先记录增长前的属性状态
			propertyChange = 0;

			ExcuteConsumablesSkillLogic (self);

		}

		protected override void ExcuteConsumablesSkillLogic(BattleAgentController self){

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

			// 执行技能
			Excute (self);

		}

		private void Excute(BattleAgentController self){

			self.propertyCalculator.AgentPropertyChange (propertyType, skillSourceValue,false);

			propertyChange += skillSourceValue;

			if (duration > 0) {
				self.propertyCalculator.AddSkill<ConsumablesSkill> (this);
				ResetPropertyCoroutine = ResetPropertyWhenTimeOut (self, propertyType, duration);
				StartCoroutine (ResetPropertyCoroutine);
			} else {
				self = null;
				Destroy (this.gameObject);
			}



		}
			

		/// <summary>
		/// 取消技能效果
		/// </summary>
		/// <returns><c>true</c> if this instance cancel skill effect; otherwise, <c>false</c>.</returns>
		public override void CancelSkillEffect (BattleAgentController self)
		{
			if (ResetPropertyCoroutine != null) {
				StopCoroutine (ResetPropertyCoroutine);
			}

			self.propertyCalculator.RemoveTriggeredSkill (this);

			if (propertyType == PropertyType.Health) {
				return;
			}

			self.propertyCalculator.AgentPropertyChange (propertyType, -skillSourceValue);
			self = null;
			Destroy (this.gameObject);

		}

		private IEnumerator ResetPropertyWhenTimeOut(BattleAgentController self,PropertyType propertyType,float duration){
			yield return new WaitForSeconds (duration);
			self.propertyCalculator.InstantPropertyChange (self,propertyType, -skillSourceValue,false);
			self.propertyCalculator.RemoveTriggeredSkill (this);
			self = null;
			Destroy (this.gameObject);
		}
			

	}
}
