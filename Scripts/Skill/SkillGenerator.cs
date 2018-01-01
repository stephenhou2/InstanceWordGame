using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class SkillGenerator:SingletonMono<SkillGenerator> {

//		private Transform mAllSkillsContainer;
//		public Transform allSkillsContainer{
//			get{
//				if (mAllSkillsContainer == null) {
//					mAllSkillsContainer = TransformManager.FindOrCreateTransform ("AllSkillsContainer");
//				}
//				return mAllSkillsContainer;
//			}
//		}

//		public Transform allSkillsContainer;

		public ConsumablesSkill GenerateConsumablesSkill(Item item,SkillInfo skillInfo,Transform skillContainer){

			ConsumablesSkill newSkill = null;

			if (skillInfo.excuteOnce) {
				ConsumablesSkill cs = GameManager.Instance.gameDataCenter.allSkills.Find (delegate(Skill obj) {
					return obj.name == "ConsumablesPropertyChangeOnce";
				}) as ConsumablesSkill;
				newSkill = GameObject.Instantiate (cs.gameObject, skillContainer).GetComponent<ConsumablesSkill> ();
				newSkill.gameObject.name = string.Format ("{0}-{1}", item.itemName, "ConsumablesPropertyChangeOnce");
			} else {
				ConsumablesSkill cs = GameManager.Instance.gameDataCenter.allSkills.Find (delegate(Skill obj) {
					return obj.name == "ConsumablesPropertyChangeDurative";
				}) as ConsumablesSkill;
				newSkill = GameObject.Instantiate (cs.gameObject, skillContainer).GetComponent<ConsumablesSkill> ();
				newSkill.gameObject.name = string.Format ("{0}-{1}", item.itemName, "ConsumablesPropertyChangeDurative");
			}

			string propertyName = "";

			switch (skillInfo.skillType) {
			case MySkillType.MaxHealth:
				newSkill.propertyType = PropertyType.MaxHealth;
				propertyName = "MaxHealth";
				break;
			case MySkillType.Health:
				newSkill.propertyType = PropertyType.Health;
				propertyName = "Health";
				break;
			case MySkillType.Mana:
				newSkill.propertyType = PropertyType.Mana;
				propertyName = "Mana";
				break;
			case MySkillType.Attack:
				newSkill.propertyType = PropertyType.Attack;
				propertyName = "Attack";
				break;
			case MySkillType.AttackSpeed:
				newSkill.propertyType = PropertyType.AttackSpeed;
				propertyName = "AttackSpeed";
				break;
			case MySkillType.Armor:
				newSkill.propertyType = PropertyType.Armor;
				propertyName = "Armor";
				break;
			case MySkillType.MagicResist:
				newSkill.propertyType = PropertyType.MagicResist;
				propertyName = "MagicResist";
				break;
			case MySkillType.Dodge:
				newSkill.propertyType = PropertyType.Dodge;
				propertyName = "Dodge";
				break;
			case MySkillType.Crit:
				newSkill.propertyType = PropertyType.Crit;
				propertyName = "Crit";
				break;
			case MySkillType.Hit:
				newSkill.propertyType = PropertyType.Hit;
				propertyName = "Hit";
				break;
			case MySkillType.WholeProperty:
				newSkill.propertyType = PropertyType.WholeProperty;
				propertyName = "WholeProperty";
				break;
			}
				
			newSkill.gameObject.name = newSkill.gameObject.name + "-" + propertyName;

			newSkill.selfEffectAnimName = skillInfo.selfEffectAnimName;
			newSkill.enemyEffectAnimName = skillInfo.enemyEffectAnimName;
			newSkill.statusName = skillInfo.statusName;
			newSkill.skillSourceValue = skillInfo.skillSourceValue;
			newSkill.duration = skillInfo.duration;
			newSkill.canOverlay = skillInfo.canOverlay;

			return newSkill;
		}


		public TriggeredSkill GenerateTriggeredSkill(Item item,SkillInfo skillInfo,Transform skillContainer){


			TriggeredSkill newSkill = null;

//			public float skillSourceValue;//技能数据输入源
//			public float duration;//状态持续事件

			switch (skillInfo.skillType) {
			case MySkillType.AttachMagicalHurt:
				
				TriggeredSkill attachMagicalHurt = GameManager.Instance.gameDataCenter.allSkills.Find (delegate(Skill obj) {
					return obj.skillName == "AttachMagicalHurt";
				}) as TriggeredSkill;

				newSkill = GameObject.Instantiate (attachMagicalHurt.gameObject, skillContainer).GetComponent<TriggeredSkill> ();

				newSkill.gameObject.name = string.Format ("{0}-{1}", item.itemName, "AttachMagicalHurt");

				break;
			case MySkillType.HealthAbsorb:

				TriggeredSkill healthAbsorb = GameManager.Instance.gameDataCenter.allSkills.Find (delegate(Skill obj) {
					return obj.skillName == "HealthAbsorb";
				}) as TriggeredSkill;

				newSkill = GameObject.Instantiate (healthAbsorb.gameObject, skillContainer).GetComponent<TriggeredSkill> ();

				newSkill.gameObject.name = string.Format ("{0}-{1}", item.itemName, "HealthAbsorb");

				break;

			case MySkillType.Fizzy:
				
				TriggeredSkill fizzy = GameManager.Instance.gameDataCenter.allSkills.Find (delegate(Skill obj) {
					return obj.skillName == "Fizzy";
				}) as TriggeredSkill;

				newSkill = GameObject.Instantiate (fizzy.gameObject, skillContainer).GetComponent<TriggeredSkill> ();

				newSkill.gameObject.name = string.Format ("{0}-{1}", item.itemName, "Fizzy");

				break;
			case MySkillType.ReflectHurt:
				TriggeredSkill reflectHurt = GameManager.Instance.gameDataCenter.allSkills.Find (delegate(Skill obj) {
					return obj.skillName == "ReflectHurt";
				}) as TriggeredSkill;

				newSkill = GameObject.Instantiate (reflectHurt.gameObject, skillContainer).GetComponent<TriggeredSkill> ();

				newSkill.gameObject.name = string.Format ("{0}-{1}", item.itemName, "ReflectHurt");

				break;
			case MySkillType.Health:
			case MySkillType.Mana:
			case MySkillType.Attack:
			case MySkillType.AttackSpeed:
			case MySkillType.Armor:
			case MySkillType.MagicResist:
			case MySkillType.Dodge:
			case MySkillType.Crit:
			case MySkillType.Hit:
			case MySkillType.WholeProperty:
				newSkill = GetPropertyOnceOrDurativeChangeSkill (item,skillInfo,skillContainer);
				break;
			case MySkillType.None:
				Debug.LogError (string.Format ("物品{0}缺少技能类型信息，无法生成技能", item.itemName));
				break;

			}


			switch (skillInfo.triggeredCondition) {
			case TriggeredCondition.BeforeFight:
				newSkill.beforeFightTriggerInfo.triggerSource = skillInfo.triggerSource;
				newSkill.beforeFightTriggerInfo.triggerTarget = skillInfo.triggerTarget;
				newSkill.beforeFightTriggerInfo.triggered = true;
				break;
			case TriggeredCondition.Attack:
				newSkill.attackTriggerInfo.triggerSource = skillInfo.triggerSource;
				newSkill.attackTriggerInfo.triggerTarget = skillInfo.triggerTarget;
				newSkill.attackTriggerInfo.triggered = true;
				break;
			case TriggeredCondition.Hit:
				newSkill.hitTriggerInfo.triggerSource = skillInfo.triggerSource;
				newSkill.hitTriggerInfo.triggerTarget = skillInfo.triggerTarget;
				newSkill.hitTriggerInfo.triggered = true;
				break;
			case TriggeredCondition.BeAttacked:
				newSkill.beAttackedTriggerInfo.triggerSource = skillInfo.triggerSource;
				newSkill.beAttackedTriggerInfo.triggerTarget = skillInfo.triggerTarget;
				newSkill.beAttackedTriggerInfo.triggered = true;
				break;
			case TriggeredCondition.BeHit:
				newSkill.beHitTriggerInfo.triggerSource = skillInfo.triggerSource;
				newSkill.beHitTriggerInfo.triggerTarget = skillInfo.triggerTarget;
				newSkill.beHitTriggerInfo.triggered = true;
				break;
			case TriggeredCondition.None:

				break;
			}



			newSkill.hurtType = skillInfo.hurtType;

			newSkill.selfEffectAnimName = skillInfo.selfEffectAnimName;
			newSkill.enemyEffectAnimName = skillInfo.enemyEffectAnimName;


			newSkill.statusName = skillInfo.statusName;

			newSkill.triggeredProbability = skillInfo.triggeredProbability;
			newSkill.skillSourceValue = skillInfo.skillSourceValue;
			newSkill.duration = skillInfo.duration;
			newSkill.canOverlay = skillInfo.canOverlay;



			return newSkill;
		}
			
		private TriggeredSkill GetPropertyOnceOrDurativeChangeSkill(Item item,SkillInfo skillInfo,Transform skillContainer){

			TriggeredSkill skill = null;

			if (skillInfo.excuteOnce) {
				PropertyChangeOnce propertyChangeOnce = GameManager.Instance.gameDataCenter.allSkills.Find (delegate(Skill obj) {
					return obj.skillName == "PropertyChangeOnce";
				}) as PropertyChangeOnce;

				string propertyName = "";

				switch (skillInfo.skillType) {
				case MySkillType.MaxHealth:
					propertyChangeOnce.propertyType = PropertyType.MaxHealth;
					propertyName = "MaxHealth";
					break;
				case MySkillType.Health:
					propertyChangeOnce.propertyType = PropertyType.Health;
					propertyName = "Health";
					break;
				case MySkillType.Mana:
					propertyChangeOnce.propertyType = PropertyType.Mana;
					propertyName = "Mana";
					break;
				case MySkillType.Attack:
					propertyChangeOnce.propertyType = PropertyType.Attack;
					propertyName = "Attack";
					break;
				case MySkillType.AttackSpeed:
					propertyChangeOnce.propertyType = PropertyType.AttackSpeed;
					propertyName = "AttackSpeed";
					break;
				case MySkillType.Armor:
					propertyChangeOnce.propertyType = PropertyType.Armor;
					propertyName = "Armor";
					break;
				case MySkillType.MagicResist:
					propertyChangeOnce.propertyType = PropertyType.MagicResist;
					propertyName = "MagicResist";
					break;
				case MySkillType.Dodge:
					propertyChangeOnce.propertyType = PropertyType.Dodge;
					propertyName = "Dodge";
					break;
				case MySkillType.Crit:
					propertyChangeOnce.propertyType = PropertyType.Crit;
					propertyName = "Crit";
					break;
				case MySkillType.Hit:
					propertyChangeOnce.propertyType = PropertyType.Hit;
					propertyName = "Hit";
					break;
				case MySkillType.WholeProperty:
					propertyChangeOnce.propertyType = PropertyType.WholeProperty;
					propertyName = "WholeProperty";
					break;
				}

				skill = GameObject.Instantiate (propertyChangeOnce.gameObject, skillContainer).GetComponent<TriggeredSkill> ();

				skill.gameObject.name = string.Format ("{0}-{1}-{2}", item.itemName, "PropertyChangeOnce",propertyName);

			} else {
				PropertyChangeDurative durativeChange = GameManager.Instance.gameDataCenter.allSkills.Find (delegate(Skill obj) {
					return obj.skillName == "PropertyChangeDurative";
				}) as PropertyChangeDurative;

				string propertyName = "";

				switch (skillInfo.skillType) {
				case MySkillType.MaxHealth:
					durativeChange.propertyType = PropertyType.MaxHealth;
					propertyName = "MaxHealth";
				break;
				case MySkillType.Health:
					durativeChange.propertyType = PropertyType.Health;
					propertyName = "Health";
					break;
				case MySkillType.Mana:
					durativeChange.propertyType = PropertyType.Mana;
					propertyName = "Mana";
					break;
				case MySkillType.Attack:
					durativeChange.propertyType = PropertyType.Attack;
					propertyName = "Attack";
					break;
				case MySkillType.AttackSpeed:
					durativeChange.propertyType = PropertyType.AttackSpeed;
					propertyName = "AttackSpeed";
					break;
				case MySkillType.Armor:
					durativeChange.propertyType = PropertyType.Armor;
					propertyName = "Armor";
					break;
				case MySkillType.MagicResist:
					durativeChange.propertyType = PropertyType.MagicResist;
					propertyName = "MagicResist";
					break;
				case MySkillType.Dodge:
					durativeChange.propertyType = PropertyType.Dodge;
					propertyName = "Dodge";
					break;
				case MySkillType.Crit:
					durativeChange.propertyType = PropertyType.Crit;
					propertyName = "Crit";
					break;
				case MySkillType.Hit:
					durativeChange.propertyType = PropertyType.Hit;
					propertyName = "Hit";
					break;
				case MySkillType.WholeProperty:
					durativeChange.propertyType = PropertyType.WholeProperty;
					propertyName = "WholeProperty";
					break;
				}

				skill = GameObject.Instantiate (durativeChange.gameObject, skillContainer).GetComponent<TriggeredSkill> ();
			
				skill.gameObject.name = string.Format ("{0}-{1}-{2}", item.itemName, "PropertyChangeDurative",propertyName);
			}



			return skill;

		}
	}
}
