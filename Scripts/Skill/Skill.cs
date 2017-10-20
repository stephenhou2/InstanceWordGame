﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public abstract class Skill:MonoBehaviour {

		public string skillName;// 技能名称

		public string skillEngName;

		public string sfxName;//音效名称

		public int skillId;

		public string skillIconName;

		public string skillDescription;

//		public BaseSkillEffect[] skillEffects;// 技能效果数组

		public int manaConsume;//技能的魔法消耗

		public float coolenInterval;// 技能的冷却时间

		public int skillLevel;// 技能等级

		public bool isAvalible = true;

		public int unlockAgentLevel;

//		public bool unlocked;

		public float dodgeSeed = 0.01f; //计算闪避时的种子数

		public float critSeed = 0.01f; //计算暴击时的种子数

		public float armorSeed = 0.01f; //计算护甲抵消伤害的种子数

		public float magicResistSeed = 0.01f; //计算魔抗抵消伤害的种子数

		public float baseNum;

		public bool isPassive;

		public string selfAnimName;
		public string enemyAnimName;

		public string selfEffectName;
		public string enemyEffectName;


		/// <summary>
		/// 技能作用效果
		/// </summary>
		/// <param name="self">Self.</param>
		/// <param name="enemy">Enemy.</param>
		/// <param name="skillLevel">Skill level.</param>
		public abstract void AffectAgents(BattleAgentController self, BattleAgentController enemy);
//		{
//			for (int i = 0; i < skillEffects.Length; i++) {
//
//				BaseSkillEffect bse = skillEffects [i];
//
//				// 如果是状态类效果，将状态添加到对象身上
//				if (bse.isStateEffect) {
//
//					StateSkillEffect sse = bse as StateSkillEffect;
//
//					BattleAgentStatesManager.AddStateCopyToBattleAgents (self, enemy, sse, skillLevel, effectDuration);
//
//					continue;
//				}
//
//				// 如果不是状态类效果
//				bse.AffectAgents (self, enemy, skillLevel, TriggerType.None);
//
//			}
//		}

		// 判断概率性技能是否生效
		protected bool isEffective(float chance){
			float randomNum = Random.Range (0, 100)/100f;
			return randomNum <= chance;
		}

		public override string ToString ()
		{
	//		return string.Format ("[Skill]" + "\n[SkillName]:" + skillName + "\n[manaConsume]:" + manaConsume + "\n[ActionConsume]:" + actionConsume + "\n[effect1]:" + skillEffects[0].effectName + "\n[effect2]:" + skillEffects[1].effectName);
			return string.Format ("[Skill]" + "\n[SkillName]:" + skillName + "\n[manaConsume]:" + manaConsume);
		}

	}
}
	

