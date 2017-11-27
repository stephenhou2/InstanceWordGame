using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

//	public enum DurativeGainType{
//		Health,
//		Mana,
//		Attack,
//		AttackSpeed,
//		Armor,
//		MagicResist,
//		Dodge,
//		Crit
//	}
		
	// 持续加／减血量或者魔量
	public class DurativeGain : PassiveSkill {

		public int singleGain;

		public float duration;

		public float probabilityBase;

		public SkillEffectTarget effectTarget;

		private BattleAgentController affectedBattleAgent;

		public SkillCallBack attackTriggerCallBack;

		public PropertyType propertyType;

		private int originalProperty;

		private Coroutine effectCoroutine;

		private float effectDelay = 0.2f;

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
			// 记录增长前的属性状态
			affectedBattleAgent.agent.GetAgentPropertyWithType (propertyType);

		}

		protected override void AttackTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			// 判断被动是否生效
			if (!isEffective (probabilityBase * skillLevel)) {
				return;
			}

			// 播放技能对应的玩家技能特效动画
			SetEffectAnims(attackTriggerInfo,self,enemy);

			// 如果技能效果不可叠加，并且作用对象身上已经有该技能效果，则直接返回
			if (!canOverlay && affectedBattleAgent.stateEffectCoroutines.Contains (effectCoroutine)) {
//				StopCoroutine (effectCoroutine);
//				affectedBattleAgent.stateEffectCoroutines.Remove (effectCoroutine);
				return;
			}

			effectCoroutine = StartCoroutine ("ExcuteAgentGain", affectedBattleAgent);

			affectedBattleAgent.stateEffectCoroutines.Add (effectCoroutine);
		}

		protected override void BeAttackedTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			// 判断被动是否生效
			if (!isEffective (probabilityBase * skillLevel)) {
				return;
			}

			// 播放技能对应的玩家技能特效动画
			SetEffectAnims(beAttackedTriggerInfo,self,enemy);
				

			if (!canOverlay && affectedBattleAgent.stateEffectCoroutines.Contains (effectCoroutine)) {
//				StopCoroutine (effectCoroutine);
//				affectedBattleAgent.stateEffectCoroutines.Remove (effectCoroutine);
				return;
			}

			effectCoroutine = StartCoroutine ("ExcuteAgentGain", affectedBattleAgent);

			affectedBattleAgent.stateEffectCoroutines.Add (effectCoroutine);
		}
			

		private IEnumerator ExcuteAgentGain(BattleAgentController ba){

			float timer = 0f;

			while (timer < duration) {

				ba.agent.AgentPropertyGain (propertyType, singleGain);

				string tintStr = string.Empty;

				if (propertyType == PropertyType.Health) {
					if (singleGain > 0) {
						tintStr = string.Format ("<color=green>+{0}</color>", singleGain);
						affectedBattleAgent.PlayGainTextAnim (tintStr);
					} else {
						tintStr = string.Format ("<color=red>{0}</color>", singleGain);
						affectedBattleAgent.PlayGainTextAnim (tintStr,effectDelay);
					}
				} else if (propertyType == PropertyType.Mana) {
					if (singleGain > 0) {
						tintStr = string.Format ("<color=blue>+{0}</color>", singleGain);
						affectedBattleAgent.PlayGainTextAnim (tintStr);
					} else {
						tintStr = string.Format ("<color=blue>{0}</color>", singleGain);
						affectedBattleAgent.PlayGainTextAnim (tintStr,effectDelay);
					}
				}

				timer += 1f;

				yield return new WaitForSeconds (1.0f);

			}

			ba.states.Remove (stateName);

			ba.stateEffectCoroutines.Remove (effectCoroutine);


		}

		protected override void FightEndTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			// 如果状态增长是攻击，攻速，护甲，抗性，闪避，暴击，在状态结束后将属性重置为初始值
			if (propertyType == PropertyType.Health || propertyType == PropertyType.Mana) {
				return;
			}

			affectedBattleAgent.agent.AgentPropertySetToValue (propertyType, originalProperty);

		}

	}
}
