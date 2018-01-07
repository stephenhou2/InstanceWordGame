using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public abstract class ConsumablesSkill : Skill {

		public string statusName;//状态名称

		public float skillSourceValue;//技能结算数据源

		public PropertyType propertyType;

		public bool canOverlay;// 技能效果是否可以叠加

		//技能持续事件或状态持续时间
		//单次型技能代表技能对应状态的持续时间（如 ： 减少10点护甲，【持续】90秒）
		//连续型技能代表技能持续时间(如 ：每秒损失10点生命，【持续】3秒）
		public float duration;


		protected void SetEffectAnims(BattleAgentController self){
			// 播放技能对应的己方（释放技能方）技能特效动画
			if (selfEffectAnimName != string.Empty) {
				self.SetEffectAnim (selfEffectAnimName);
			}
		}

		protected abstract void ExcuteConsumablesSkillLogic(BattleAgentController self);

		public abstract void CancelSkillEffect (BattleAgentController self);

	}
}
