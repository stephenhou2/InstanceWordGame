using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class AgentPropertyGain : PassiveSkill {

		private int originalProperty;// 记录己方原始属性值，战斗结束后重置

		public float probabilityBase;

		public int gainBase;// 属性增加值

		private BattleAgentController affectedBattleAgent;

		public PropertyType propertyType;

		public SkillEffectTarget effectTarget;

		void Awake(){
			this.skillType = SkillType.Passive;
		}

		protected override void ExcuteSkillLogic (BattleAgentController self, BattleAgentController enemy)
		{
			switch (effectTarget) {
			case SkillEffectTarget.Self:
				affectedBattleAgent = self;
				break;
			case SkillEffectTarget.Enemy:
				affectedBattleAgent = enemy;
				break;
			}

			// 先记录增长前的属性状态
			originalProperty = affectedBattleAgent.agent.GetAgentPropertyWithType(propertyType);

		}


		protected override void AttackTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			bool skillEffective = isEffective (probabilityBase * skillLevel);

			if (!skillEffective) {
				return;
			}

			ExcuteAgentPropertyGain ();


		}

		protected override void BeAttackedTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			bool skillEffective = isEffective (probabilityBase * skillLevel);

			if (!skillEffective) {
				return;
			}

			ExcuteAgentPropertyGain ();

			// 播放技能对应的玩家技能特效动画
			if (enemyEffectName != string.Empty) {
				affectedBattleAgent.SetEffectAnim (enemyEffectName);
			}

		}

		private void ExcuteAgentPropertyGain(){

//			bool skillEffective = isEffective (probabilityBase * skillLevel);
//
//			if (!skillEffective) {
//				return;
//			}

			if (!affectedBattleAgent.states.Contains (this.stateName)) {

				affectedBattleAgent.states.Add (this.stateName);

				affectedBattleAgent.agent.AgentPropertyGain (propertyType, gainBase);

				string tintStr = string.Empty;

				if (propertyType == PropertyType.Health) {
					if (gainBase > 0) {
						tintStr = string.Format ("<color=green>+{0}</color>", gainBase);
						affectedBattleAgent.PlayGainTextAnim (tintStr);
					} else {
						tintStr = string.Format ("<color=red>{0}</color>", gainBase);
						affectedBattleAgent.PlayHurtTextAnim (tintStr, TintTextType.None);
					}
				} else if (propertyType == PropertyType.Mana) {
					if (gainBase > 0) {
						tintStr = string.Format ("<color=blue>+{0}</color>", gainBase);
						affectedBattleAgent.PlayGainTextAnim (tintStr);
					} else {
						tintStr = string.Format ("<color=blue>{0}</color>", gainBase);
						affectedBattleAgent.PlayGainTextAnim (tintStr);
					}
				}

			} else if (canOverlay) {

				affectedBattleAgent.agent.AgentPropertyGain (propertyType, gainBase);

				string tintStr = string.Empty;

				if (propertyType == PropertyType.Health) {
					if (gainBase > 0) {
						tintStr = string.Format ("<color=green>+{0}</color>", gainBase);
						affectedBattleAgent.PlayGainTextAnim (tintStr);
					} else {
						tintStr = string.Format ("<color=red>{0}</color>", gainBase);
						affectedBattleAgent.PlayHurtTextAnim (tintStr, TintTextType.None);
					}
				} else if (propertyType == PropertyType.Mana) {
					if (gainBase > 0) {
						tintStr = string.Format ("<color=blue>+{0}</color>", gainBase);
						affectedBattleAgent.PlayGainTextAnim (tintStr);
					} else {
						tintStr = string.Format ("<color=blue>{0}</color>", gainBase);
						affectedBattleAgent.PlayGainTextAnim (tintStr);
					}
				}

			}

		}

		/// <summary>
		/// Fights the end trigger call back.
		/// </summary>
		protected override void FightEndTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{

			if (propertyType == PropertyType.Health || propertyType == PropertyType.Mana) {
				return;
			}

			affectedBattleAgent.agent.AgentPropertySetToValue (propertyType, originalProperty);

			affectedBattleAgent.states.Remove(this.stateName);
		}
		
	}
}
