using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	public abstract class Skill:MonoBehaviour {

		public string skillName;// 技能名称

		public string sfxName;//音效名称

		public int skillId;

		public string skillIconName;

		public string skillDescription;





		public bool canOverlay;// 技能效果是否可以叠加


//		public string selfRoleAnimName;
//		public string enemyRoleAnimName;

		public string selfEffectAnimName;
		public string enemyEffectAnimName;

//		public string audioClipName;

//		public bool unlocked;

		/// <summary>
		/// 技能作用效果
		/// </summary>
		/// <param name="self">Self.</param>
		/// <param name="enemy">Enemy.</param>
		/// <param name="skillLevel">Skill level.</param>
		public virtual void AffectAgents (BattleAgentController self, BattleAgentController enemy){
			ExcuteNoneTriggeredSkillLogic (self, enemy);
		}

		/// <summary>
		/// 非触发型技能（如普通攻击）的逻辑 和 触发型技能的非触发逻辑（如添加触发回调前先记录一些角色数据） 写在这个方法里
		/// </summary>
		/// <param name="self">Self.</param>
		/// <param name="enemy">Enemy.</param>
		protected virtual void ExcuteNoneTriggeredSkillLogic (BattleAgentController self, BattleAgentController enemy){

		}


		// 判断概率性技能是否生效
		protected bool isEffective(float chance){
			float randomNum = Random.Range (0, 100)/100f;
			return randomNum <= chance;
		}


		public override string ToString ()
		{
			return string.Format ("[Skill]" + "\n[SkillName]:" + skillName);
		}

	}


	public enum MySkillType{
		None,
		MaxHealth,
		Health,
		Mana,
		Attack,
		AttackSpeed,
		Armor,
		MagicResist,
		Dodge,
		Crit,
		Hit,
		PhysicalHurtScaler,
		MagicalHurtScaler,
		CritHurtScaler,
		WholeProperty,
		HealthAbsorb,
		AttachMagicalHurt,
		Fizzy,
		ReflectHurt
	}


	public enum HurtType{
		None,
		Physical,
		Magical,
	}

	public enum TriggeredCondition{
		None,
		BeforeFight,
		Attack,
		Hit,
		BeAttacked,
		BeHit
	}

	public enum SkillEffectTarget
	{
		Self,
		Enemy
	}



	[System.Serializable]
	public class SkillInfo{

		public MySkillType skillType;//技能类型

		public HurtType hurtType;

		public SkillEffectTarget triggerSource;//触发源

		public TriggeredCondition triggeredCondition;//触发时机

		public SkillEffectTarget triggerTarget;//触发效果作用对象

		public string selfEffectAnimName;//己方技能特效名称
		public string enemyEffectAnimName;//敌方技能特效名称

		public string statusName;//状态名称

		public float triggeredProbability;//触发概率

		public bool canOverlay;//是否可以叠加

		public float skillSourceValue;//技能数据输入源

		public bool excuteOnce;//是否是单次型技能

		public float duration;//状态持续事件

	}



}
	

