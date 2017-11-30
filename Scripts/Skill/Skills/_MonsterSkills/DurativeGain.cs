﻿using System.Collections;
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
	public class DurativeGain : TriggeredPassiveSkill {

		public int singleGain;

		public float duration;

		public float probabilityBase;

		public SkillEffectTarget effectTarget;

		public PropertyType propertyType;

		public bool removeWhenQuitFight;

		public string stateSpriteName;

		private int originalProperty;

		private BattleAgentController affectedBattleAgent;

		private IEnumerator effectCoroutine;

		private List<IEnumerator> effectCoroutines = new List<IEnumerator>();

		private SkillState state;

		private float effectDelay = 0.2f;

		protected override void Awake ()
		{
			base.Awake ();
			originalProperty = 0;
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

			// 如果技能效果可叠加，或者首次触发时添加状态，并执行状态效果
			if (canOverlay || !affectedBattleAgent.CheckStateExist (stateName)) {

				// 创建持续性状态的执行协程
				effectCoroutine = ExcuteAgentGain (affectedBattleAgent);

				// 创建对应的状态
				state = new SkillState (stateName,stateSpriteName);

				// 作用对象身上添加状态
				affectedBattleAgent.AddState (state);

				// 开启持续性状态的协程
				StartCoroutine (effectCoroutine);

				// 将该协程添加到所有协程表中
				effectCoroutines.Add (effectCoroutine);
			}


		}

		protected override void BeAttackedTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{
			// 判断被动是否生效
			if (!isEffective (probabilityBase * skillLevel)) {
				return;
			}

			// 播放技能对应的玩家技能特效动画
			SetEffectAnims(attackTriggerInfo,self,enemy);

			// 如果技能效果可叠加，或者首次触发时添加状态，并执行状态效果
			if (canOverlay || !affectedBattleAgent.CheckStateExist (stateName)) {
				
				// 创建持续性状态的执行协程
				effectCoroutine = ExcuteAgentGain (affectedBattleAgent);

				// 创建对应的状态
				state = new SkillState (stateName,stateSpriteName);

				// 作用对象身上添加状态
				affectedBattleAgent.AddState (state);

				// 开启持续性状态的协程
				StartCoroutine (effectCoroutine);

				// 将该协程添加到所有协程表中
				effectCoroutines.Add (effectCoroutine);
			}

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

			effectCoroutines.Remove (effectCoroutine);

			ba.RemoveState (state);
		}

		protected override void FightEndTriggerCallBack (BattleAgentController self, BattleAgentController enemy)
		{

			if (removeWhenQuitFight) {
				for (int i = 0; i < effectCoroutines.Count; i++) {
					StopCoroutine (effectCoroutines [i]);
				}
			}

			// 如果状态增长是攻击，攻速，护甲，抗性，闪避，暴击，在状态结束后将属性重置为初始值
			if (propertyType == PropertyType.Health || propertyType == PropertyType.Mana) {
				return;
			}

			affectedBattleAgent.agent.AgentPropertySetToValue (propertyType, originalProperty);



		}

	}
}
