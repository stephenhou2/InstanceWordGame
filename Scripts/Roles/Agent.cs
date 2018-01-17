using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace WordJourney
{

	public enum PropertyType{
		MaxHealth,
		Health,
		Mana,
		Attack,
		Hit,
		AttackSpeed,
		Armor,
		MagicResist,
		Dodge,
		Crit,
		PhysicalHurtScaler,
		MagicalHurtScaler,
		CritHurtScaler,
		WholeProperty
	}

	public abstract class Agent : MonoBehaviour {

		public string agentName;
//		public string agentIconName;

//		public bool isActive;

		public int agentLevel;

		protected BattleAgentController mBattleAgentController;
		protected BattleAgentController battleAgentCtr{
			get{
				if (mBattleAgentController == null) {
					mBattleAgentController = GetComponent<BattleAgentController> ();
				}
				if (mBattleAgentController == null) {
					mBattleAgentController = transform.Find ("BattlePlayer").GetComponent<BattleAgentController> ();
				}
				return mBattleAgentController;
			}
		}


		//*****人物基础信息(无装备，无状态加成时的人物属性)********//
		public int originalMaxHealth;//基础最大生命值
		public int originalHealth;//基础生命
		public int originalMana;//基础法强
		public int originalAttack;//基础攻击
		public int originalAttackSpeed;//基础攻速
		public int originalArmor;//基础护甲
		public int originalMagicResist;//基础抗性
		public int originalDodge;//基础闪避
		public int originalCrit;//基础暴击
		public int originalHit;//基础命中
		public float originalPhysicalHurtScaler = 1f;//基础物理伤害系数
		public float originalMagicalHurtScaler = 1f;//基础魔法伤害系数
		public float originalCritHurtScaler = 1.5f;//基础暴击系数
		//*****人物基础信息(无装备，无状态加成时的人物属性)********//

		//********人物只考虑装备加成的属性信息*********//
		protected int maxHealthWithEquipment;
		protected int healthWithEquipment;
		protected int manaWithEquipment;
		protected int attackWithEquipment;
		protected int attackSpeedWithEquipment;
		protected int armorWithEquipment;
		protected int magicResistWithEquipment;
		protected int dodgeWithEquipment;
		protected int critWithEquipment;
		protected int hitWithEquipment;
//		protected float physicalHurtScalerWithEquipment;
//		protected float magicalHurtScalerWithEquipment;
//		protected float critHurtScalerWithEquipment;
		//********人物只考虑装备加成的属性信息*********//


		//********人物最终的实际属性信息*********//
		private int mMaxHealth;//实际最大血量
		private int mHealth;//实际生命
		private int mMana;//实际法强
		private int mAttack;//实际攻击力
		private int mAttackSpeed;//实际攻速
		private int mArmor;//实际护甲
		private int mMagicResist;//实际抗性
		private int mDodge;//实际闪避
		private int mCrit;//实际暴击
		private int mHit;//实际命中
		//********人物最终的实际属性信息*********//


		public int maxHealth{
			get{ return mMaxHealth; }
			set{ mMaxHealth = value > 0 ? value : 0;}
		}
		public int health{
			get{ return mHealth; }
			set{ 
				if (value >= 0) {
					mHealth = value >= maxHealth ? maxHealth : value;
				} else {
					mHealth = 0;
				}
			}
		}
		public int mana{
			get{ return mMana; }
			set{ mMana = value > 0 ? value : 0;}
		}
		public int attack{
			get{ return mAttack; }
			set{ mAttack = value > 0 ? value : 0;}
		}
		public int attackSpeed{
			get{ return mAttackSpeed; }
			set{ mAttackSpeed = value > 0 ? value : 0; }
		}
		public int armor{
			get{ return mArmor; }
			set{ mArmor = value > 0 ? value : 0; }
		}
		public int magicResist{
			get{ return mMagicResist; }
			set{ mMagicResist = value > 0 ? value : 0; }
		}
		public int dodge {
			get{ return mDodge; }
			set{ mDodge = value > 0 ? value : 0; }
		}
		public int crit{
			get{ return mCrit; }
			set{ mCrit = value > 0 ? value : 0; }
		}
		public int hit{
			get{ return mHit; }
			set{ mHit = value > 0 ? value : 0; }
		}


		//***********其他事件造成的属性数值增益（消耗品增益，特殊事件增益，技能增益）*************//
		protected int maxHealthChangeFromOther;
		protected int healthChangeFromOther;
		protected int manaChangeFromOther;
		protected int attackChangeFromOther;
		protected int hitChangeFromOther;
		protected int attackSpeedChangeFromOther;
		protected int armorChangeFromOther;
		protected int magicResistChangeFromOther;
		protected int dodgeChangeFromOther;
		protected int critChangeFromOther; 
		//***********其他事件造成的属性数值增益（消耗品增益，特殊事件增益，技能增益）*************//

		//***********其他事件造成的属性加成比例（消耗品加成，特殊事件加成，技能加成）*************//
		protected float maxHealthChangeScalerFromOther;
		protected float healthChangeScalerFromOther;
		protected float manaChangeScalerFromOther;
		protected float attackChangeScalerFromOther;
		protected float hitChangeScalerFromOther;
		protected float attackSpeedChangeScalerFromOther;
		protected float armorChangeScalerFromOther;
		protected float magicResistChangeScalerFromOther;
		protected float dodgeChangeScalerFromOther;
		protected float critChangeScalerFromOther; 
		protected float physicalHurtChangeScalerFromOther;
		protected float magicalHurtChangeScalerFromOther;
		protected float critHurtChangeScalerFromOther;
		//***********其他事件造成的属性加成比例（消耗品加成，特殊事件加成，技能加成）*************//



		//***********只考虑武器加成的基础属性加成比例*************//
		protected float maxHealthGainScalerFromEq;
		protected float manaGainScalerFromEq;
		protected float attackGainScalerFromEq;
		protected float hitGainScalerFromEq;
		protected float attackSpeedGainScalerFromEq;
		protected float armorGainScalerFromEq;
		protected float magicResistGainScalerFromEq;
		protected float dodgeGainScalerFromEq;
		protected float critGainScalerFromEq;
		//***********只考虑武器加成的基础属性加成比例*************//

		public float dodgeFixScaler;//闪避修正系数
		public float critFixScaler;//暴击修正系数


//		[HideInInspector]public float physicalHurtScaler = 1.0f;//物理伤害系数
//
//		[HideInInspector]public float magicalHurtScaler = 1.0f;//魔法伤害系数
//
//		[HideInInspector]public float critHurtScaler = 1.5f;//暴击伤害倍率

		public float physicalHurtScaler = 1.0f;//物理伤害系数

		public float magicalHurtScaler = 1.0f;//魔法伤害系数

		public float critHurtScaler = 1.5f;//暴击伤害倍率


		private Transform mTriggeredSkillsContainer;
		public Transform triggeredSkillsContainer{
			get{
				if (mTriggeredSkillsContainer == null) {
					mTriggeredSkillsContainer = transform.Find ("AttachedTriggeredSkills");
				}
				return mTriggeredSkillsContainer;
			}
		}

		private Transform mConsumbalesSkillsContainer;
		public Transform consumablesSkillsContainer{
			get{
				if (mConsumbalesSkillsContainer == null) {
					mConsumbalesSkillsContainer = transform.Find ("AttachedConsumablesSkills");
				}
				return mConsumbalesSkillsContainer;
			}
		}

		public List<Equipment> allEquipmentsInBag = new List<Equipment> ();

		public Equipment[] allEquipedEquipments;



		public List<TriggeredSkill> attachedTriggeredSkills = new List<TriggeredSkill>();
		public List<ConsumablesSkill> attachedConsumablesSkills = new List<ConsumablesSkill> ();

		public List<string> allStatus = new List<string> ();

		[SerializeField]private int[] mCharactersCount;
		public int[] charactersCount{
			get{
				if (mCharactersCount == null || mCharactersCount.Length == 0) {
					mCharactersCount = new int[26];
				}
				return mCharactersCount;
			}

			set{
				mCharactersCount = value;
			}

		}


		// 攻击间隔
		public float attackInterval{
			get{
				if (attackSpeed > 240) {
					return 0.2f;
				}
				return (1f - attackSpeed / 200f);
			}
		}
			

		public virtual void Awake(){

//			isActive = true; // 角色初始化后默认可以行动

			mHealth = maxHealth;

			ResetBattleAgentProperties (true);

//			mMana = maxMana;

		}

		public struct PropertyChange
		{
			public int maxHealthChangeFromEq;
			public int hitChangeFromEq;
			public int attackChangeFromEq;
			public int attackSpeedChangeFromEq;
			public int manaChangeFromEq;
			public int armorChangeFromEq;
			public int magicResistChangeFromEq;
			public int dodgeChangeFromEq;
			public int critChangeFromEq;
			public int maxHealthChangeFromOther;
			public int hitChangeFromOther;
			public int attackChangeFromOther;
			public int attackSpeedChangeFromOther;
			public int manaChangeFromOther;
			public int armorChangeFromOther;
			public int magicResistChangeFromOther;
			public int dodgeChangeFromOther;
			public int critChangeFromOther;

		

			public PropertyChange(int maxHealthChangeFromEq,int hitChangeFromEq,int attackChangeFromEq,
				int manaChangeFromEq,int attackSpeedChangeFromEq,int armorChangeFromEq,
				int magicResistChangeFromEq,int dodgeChangeFromEq,int critChangeFromEq,
				int maxHealthChangeFromOther,int hitChangeFromOther,int attackChangeFromOther,
				int manaChangeFromOther,int attackSpeedChangeFromOther,int armorChangeFromOther,
				int magicResistChangeFromOther,int dodgeChangeFromOther,int critChangeFromOther){

				this.maxHealthChangeFromEq = maxHealthChangeFromEq;
				this.hitChangeFromEq = hitChangeFromEq;
				this.attackChangeFromEq = attackChangeFromEq;
				this.attackSpeedChangeFromEq = attackSpeedChangeFromEq;
				this.manaChangeFromEq = manaChangeFromEq;
				this.armorChangeFromEq = armorChangeFromEq;
				this.magicResistChangeFromEq = magicResistChangeFromEq;
				this.dodgeChangeFromEq = dodgeChangeFromEq;
				this.critChangeFromEq = critChangeFromEq;

				this.maxHealthChangeFromOther = maxHealthChangeFromOther;
				this.hitChangeFromOther = hitChangeFromOther;
				this.attackChangeFromOther = attackChangeFromOther;
				this.attackSpeedChangeFromOther = attackSpeedChangeFromOther;
				this.manaChangeFromOther = manaChangeFromOther;
				this.armorChangeFromOther = armorChangeFromOther;
				this.magicResistChangeFromOther = magicResistChangeFromOther;
				this.dodgeChangeFromOther = dodgeChangeFromOther;
				this.critChangeFromOther = critChangeFromOther;
			}

			public static PropertyChange MergeTwoPropertyChange(PropertyChange arg1,PropertyChange arg2){
				PropertyChange mergedPropertyChange = new PropertyChange ();
				mergedPropertyChange.maxHealthChangeFromEq = arg1.maxHealthChangeFromEq + arg2.maxHealthChangeFromEq;
				mergedPropertyChange.hitChangeFromEq = arg1.hitChangeFromEq + arg2.hitChangeFromEq;
				mergedPropertyChange.attackChangeFromEq = arg1.attackChangeFromEq + arg2.attackChangeFromEq;
				mergedPropertyChange.attackSpeedChangeFromEq = arg1.attackSpeedChangeFromEq  + arg2.attackSpeedChangeFromEq;
				mergedPropertyChange.manaChangeFromEq = arg1.manaChangeFromEq + arg2.manaChangeFromEq;
				mergedPropertyChange.armorChangeFromEq = arg1.armorChangeFromEq + arg2.armorChangeFromEq;
				mergedPropertyChange.magicResistChangeFromEq = arg1.magicResistChangeFromEq + arg2.magicResistChangeFromEq;
				mergedPropertyChange.dodgeChangeFromEq = arg1.dodgeChangeFromEq  + arg2.dodgeChangeFromEq;
				mergedPropertyChange.critChangeFromEq = arg1.critChangeFromEq + arg2.critChangeFromEq;

				mergedPropertyChange.maxHealthChangeFromOther = arg1.maxHealthChangeFromOther + arg2.maxHealthChangeFromOther;
				mergedPropertyChange.hitChangeFromOther = arg1.hitChangeFromOther + arg2.hitChangeFromOther;
				mergedPropertyChange.attackChangeFromOther = arg1.attackChangeFromOther + arg2.attackChangeFromOther;
				mergedPropertyChange.attackSpeedChangeFromOther = arg1.attackSpeedChangeFromOther + arg2.attackSpeedChangeFromOther;
				mergedPropertyChange.manaChangeFromOther = arg1.manaChangeFromOther + arg2.manaChangeFromOther;
				mergedPropertyChange.armorChangeFromOther = arg1.armorChangeFromOther + arg2.armorChangeFromOther;
				mergedPropertyChange.magicResistChangeFromOther = arg1.magicResistChangeFromOther + arg2.magicResistChangeFromOther;
				mergedPropertyChange.dodgeChangeFromOther = arg1.dodgeChangeFromOther + arg2.dodgeChangeFromOther;
				mergedPropertyChange.critChangeFromOther = arg1.critChangeFromOther + arg2.critChangeFromOther;
			
				return mergedPropertyChange;
			}

		}




		public void ResetPropertiesWithPropertyCalculator(AgentPropertyCalculator cal){
			maxHealth = cal.maxHealth;
			health = cal.health;
			mana = cal.mana;
			attack = cal.attack;
			attackSpeed = cal.attackSpeed;
			hit = cal.hit;
			armor = cal.armor;
			magicResist = cal.magicResist;
			dodge = cal.dodge;
			crit = cal.crit;
			physicalHurtScaler = cal.physicalHurtScaler;
			magicalHurtScaler = cal.magicalHurtScaler;
			critHurtScaler = cal.critHurtScaler;

//			allStatus.Clear ();
//
//			for (int i = 0; i < cal.triggeredSkills.Count; i++) {
//				string status = cal.triggeredSkills [i].statusName;
//				if (!allStatus.Contains (status)) {
//					allStatus.Add (status);
//				}
//			}
//
//			for (int i = 0; i < cal.consumablesSkills.Count; i++) {
//				string status = cal.consumablesSkills [i].statusName;
//				if (!allStatus.Contains (status)) {
//					allStatus.Add (status);
//				}
//			}

		}

		/// <summary>
		/// 根据装备更新人物属性&属性加成
		/// </summary>
		/// <param name="equipment">Equipment.</param>
		private void ResetPropertiesByEquipment(Equipment equipment){

			if (equipment.itemId < 0 || equipment.itemName == null) {
				return;
			}
			// 装备的该项属性>=1时，更新人物的基础属性
			// 装备的该项属性处于0～1之间时，说明是属性加成，更新人物的属性加成
			if (equipment.healthGain >= 1) {
				maxHealthWithEquipment += (int)equipment.healthGain;
				healthWithEquipment += (int)(equipment.healthGain / maxHealth * health);
			} else if (equipment.healthGain > 0) {
				maxHealthGainScalerFromEq += equipment.healthGain;
			}
			if (equipment.manaGain >= 1) {
				manaWithEquipment += (int)equipment.manaGain;
			} else if (equipment.manaGain > 0) {
				manaGainScalerFromEq += equipment.manaGain;
			}
			if (equipment.attackGain >= 1) {
				attackWithEquipment += (int)equipment.attackGain;
			} else if (equipment.attackGain > 0){
				attackGainScalerFromEq += equipment.attackGain;
			}
			if (equipment.attackSpeedGain >= 1) {
				attackSpeedWithEquipment += (int)equipment.attackSpeedGain;
			} else if (equipment.attackSpeedGain > 0) {
				attackSpeedGainScalerFromEq += equipment.attackSpeedGain;
			}
			if (equipment.hitGain >= 1) {
				hitWithEquipment += (int)equipment.hitGain;
			} else if (equipment.hitGain > 0) {
				hitGainScalerFromEq += equipment.hitGain;
			}
			if (equipment.armorGain >= 1) {
				armorWithEquipment += (int)equipment.armorGain;
			} else if (equipment.armorGain > 0) {
				armorGainScalerFromEq += equipment.armorGain;
			}
			if (equipment.magicResistGain >= 1) {
				magicResistWithEquipment += (int)equipment.magicResistGain;
			} else if (equipment.magicResistGain > 0) {
				magicResistGainScalerFromEq += equipment.magicResistGain;
			}
			if (equipment.dodgeGain >= 1) {
				dodgeWithEquipment += (int)equipment.dodgeGain;
			} else if (equipment.dodgeGain > 0) {
				dodgeGainScalerFromEq += equipment.dodgeGain;
			}
			if (equipment.critGain >= 1) {
				critWithEquipment += (int)equipment.critGain;
			} else if (equipment.critGain > 0) {
				critGainScalerFromEq += equipment.critGain;
			}
			if (equipment.physicalHurtScalerGain > 0) {
				physicalHurtScaler += equipment.physicalHurtScalerGain;
			}
			if (equipment.magicalHurtScalerGain > 0) {
				magicalHurtScaler += equipment.magicalHurtScalerGain;
			}
			if (equipment.critHurtScalerGain > 0) {
				critHurtScaler += equipment.critHurtScalerGain;
			}

			if (equipment.wholePropertyGain >= 1) {
				maxHealthWithEquipment += (int)equipment.wholePropertyGain;
				healthWithEquipment += (int)(equipment.wholePropertyGain / maxHealth * equipment.wholePropertyGain);
				manaWithEquipment += (int)(equipment.wholePropertyGain);
				attackWithEquipment += (int)(equipment.wholePropertyGain);
//				attackSpeedWithEquipment += (int)(equipment.wholePropertyGain);
				hitWithEquipment += (int)(equipment.wholePropertyGain);
				armorWithEquipment += (int)(equipment.wholePropertyGain);
				magicResistWithEquipment += (int)(equipment.wholePropertyGain);
				dodgeWithEquipment += (int)(equipment.wholePropertyGain);
				critWithEquipment += (int)(equipment.wholePropertyGain);
			}else if (equipment.wholePropertyGain > 0){
				maxHealthGainScalerFromEq += equipment.wholePropertyGain;
				manaGainScalerFromEq += equipment.wholePropertyGain;
				attackGainScalerFromEq += equipment.wholePropertyGain;
				hitGainScalerFromEq += equipment.wholePropertyGain;
				armorGainScalerFromEq += equipment.wholePropertyGain;
				magicResistGainScalerFromEq += equipment.wholePropertyGain;
				dodgeGainScalerFromEq += equipment.wholePropertyGain;
				critGainScalerFromEq += equipment.wholePropertyGain;
			
			}

		}



		// 仅根据物品重新计人物的属性，其余属性重置为初始状态
		public virtual PropertyChange ResetBattleAgentProperties (bool toOriginalState = false)
		{
			
			// 记录原来人物只佩戴装备时的属性信息
			int mMaxHealthWithEq = maxHealthWithEquipment;
			int mHitWithEq = hitWithEquipment;
			int mAttackWithEq = attackWithEquipment;
			int mManaWithEq = manaWithEquipment;
			int mAttackSpeedWithEq = attackSpeedWithEquipment;
			int mArmorWithEq = armorWithEquipment;
			int mMagicResistWithEq = magicResistWithEquipment;
			int mDodgeWithEq = dodgeWithEquipment;
			int mCritWithEq = critWithEquipment;

			// 记录人物原来的真实属性
//			int mMaxHealth = maxHealth;
//			int mHit = hit;
//			int mAttack = attack;
//			int mMana = mana;
//			int mAttackSpeed = attackSpeed;
//			int mArmor = armor;
//			int mMagicResist = magicResist;
//			int mDodge = dodge;
//			int mCrit = crit;


			// 以人物基础属性为出发点重新开始计算
			maxHealthWithEquipment = originalMaxHealth;
			healthWithEquipment = health;
			manaWithEquipment = originalMana;
			attackWithEquipment = originalAttack;
			attackSpeedWithEquipment = originalAttackSpeed;
			hitWithEquipment = originalHit;
			armorWithEquipment = originalArmor;
			magicResistWithEquipment = originalMagicResist;
			dodgeWithEquipment = originalDodge;
			critWithEquipment = originalCrit;
			physicalHurtScaler = originalPhysicalHurtScaler;
			magicalHurtScaler = originalMagicalHurtScaler;
			critHurtScaler = originalCritHurtScaler;

			// 根据装备计算人物的总的基础属性和总的各项属性加成比例
			foreach (Equipment equipment in allEquipedEquipments) {
				if (equipment.itemId >= 0) {
					ResetPropertiesByEquipment (equipment);
				}
			}

//			AgentPropertyCalculator propertyCalculator = battleAgentCtr.propertyCalculator;


			// 根据装备加成重新计算人物穿上装备之后的属性
			maxHealthWithEquipment = (int)(maxHealthWithEquipment * (1 + maxHealthGainScalerFromEq));
			healthWithEquipment = (int)(healthWithEquipment * (1 + maxHealthGainScalerFromEq));
			manaWithEquipment = (int)(manaWithEquipment * (1 + manaGainScalerFromEq));
			attackWithEquipment = (int)(attackWithEquipment * (1 + attackGainScalerFromEq));
			attackSpeedWithEquipment = (int)(attackSpeedWithEquipment * (1 + attackSpeedGainScalerFromEq)); 
			hitWithEquipment = (int)(hitWithEquipment * (1 + hitGainScalerFromEq));
			armorWithEquipment = (int)(armorWithEquipment * (1 + armorGainScalerFromEq));
			magicResistWithEquipment = (int)(magicResistWithEquipment * (1 + magicResistGainScalerFromEq));
			dodgeWithEquipment = (int)(dodgeWithEquipment * (1 + dodgeGainScalerFromEq));
			critWithEquipment = (int)(critWithEquipment * (1 + critGainScalerFromEq));

			// 根据其他状态加成重新计算人物最终属性
			maxHealth = (int)((maxHealthWithEquipment + maxHealthChangeFromOther) * (1 + maxHealthChangeScalerFromOther));
			health = (int)((healthWithEquipment + healthChangeFromOther) * (1 + healthChangeScalerFromOther));
			mana = (int)((manaWithEquipment + manaChangeFromOther) * (1 + manaChangeScalerFromOther));
			attack = (int)((attackWithEquipment + attackChangeFromOther) * (1 + attackChangeScalerFromOther));
			attackSpeed = (int)((originalAttackSpeed + attackSpeedChangeFromOther) * (1 + attackSpeedChangeScalerFromOther));
			hit = (int)((hitWithEquipment + hitChangeFromOther) * (1 + hitChangeScalerFromOther));
			armor = (int)((armorWithEquipment + armorChangeFromOther) * (1 + armorChangeScalerFromOther));
			magicResist = (int)((magicResistWithEquipment + magicResistChangeFromOther) * (1 + magicResistChangeScalerFromOther));
			dodge = (int)((dodgeWithEquipment + dodgeChangeFromOther) * (1 + dodgeChangeScalerFromOther));
			crit = (int)((critWithEquipment + critChangeFromOther) * (1 + critChangeScalerFromOther));

			if (toOriginalState) {
				health = maxHealth;
			}

			// 计算人物更换装备造成的属性变化
			int maxHealthChangeFromEq = maxHealthWithEquipment - mMaxHealthWithEq;
			int hitChangeFromEq = hitWithEquipment - mHitWithEq;
			int attackChangeFromEq = attackWithEquipment - mAttackWithEq;
			int attackSpeedChangeFromEq = attackSpeedWithEquipment - mAttackSpeedWithEq;
			int manaChangeFromEq = manaWithEquipment - mManaWithEq;
			int armorChangeFromEq = armorWithEquipment - mArmorWithEq;
			int magicResistChangeFromEq = magicResistWithEquipment - mMagicResistWithEq;
			int dodgeChangeFromEq = dodgeWithEquipment - mDodgeWithEq;
			int critChangeFromEq = critWithEquipment - mCritWithEq;

			// 计算人物其他属性
			int finalMaxHealthChangeFromOther = maxHealth - maxHealthWithEquipment;
			int finalHitChangeFromOther = hit - hitWithEquipment;
			int finalAttackChangeFromOther = attack - attackWithEquipment;
			int finalAttackSpeedChangeFromOther = mAttackSpeed- attackSpeedWithEquipment;
			int finalManaChangeFromOther = mana  - manaWithEquipment;
			int finalArmorChangeFromOther = armor  - armorWithEquipment;
			int finalMagicResistChangeFromOther = magicResist  - magicResistWithEquipment;
			int finalDodgeChangeFromOther = dodge  - dodgeWithEquipment;
			int finalCritChangeFromOther = crit  - critWithEquipment;



			return new PropertyChange (maxHealthChangeFromEq, hitChangeFromEq, attackChangeFromEq,
				manaChangeFromEq, attackSpeedChangeFromEq, armorChangeFromEq, magicResistChangeFromEq,
				dodgeChangeFromEq, critChangeFromEq,
				finalMaxHealthChangeFromOther,finalHitChangeFromOther,finalAttackChangeFromOther,
				finalManaChangeFromOther,finalManaChangeFromOther,finalArmorChangeFromOther,
				finalMagicResistChangeFromOther,finalDodgeChangeFromOther,finalCritChangeFromOther);


		}

		/// <summary>
		/// 装备以外的物品，事件，技能造成的属性变化,并更新对象属性
		/// </summary>
		public void AddPropertyChangeFromOther(PropertyType propertyType, float change)
		{
			switch(propertyType){
			case PropertyType.MaxHealth:
				if (change < -1 || change > 1) {
					maxHealthChangeFromOther += (int)change;
				} else {
					maxHealthChangeScalerFromOther += change;
				}
				break;
//			case PropertyType.Health:
//				if (change < -1 || change > 1) {
//					healthChangeFromOther += (int)change;
//				} else {
//					healthChangeScalerFromOther += change;
//				}
//				break;
			case PropertyType.Mana:
				if (change < -1 || change > 1) {
					manaChangeFromOther += (int)change;
				} else {
					manaChangeScalerFromOther += change;
				}
				break;
			case PropertyType.Attack:
				if (change < -1 || change > 1) {
					attackChangeFromOther += (int)change;
				} else {
					attackChangeScalerFromOther += change;
				}
				break;
			case PropertyType.AttackSpeed:
				if (change < -1 || change > 1) {
					attackChangeFromOther += (int)change;
				} else {
					attackChangeScalerFromOther += change;
				}
				break;
			case PropertyType.Hit:
				if (change < -1 || change > 1) {
					hitChangeFromOther += (int)change;
				} else {
					hitChangeScalerFromOther += change;
				}
				break;
			case PropertyType.Armor:
				if (change < -1 || change > 1) {
					armorChangeFromOther += (int)change;
				} else {
					armorChangeScalerFromOther += change;
				}
				break;
			case PropertyType.MagicResist:
				if (change < -1 || change > 1) {
					magicResistChangeFromOther += (int)change;
				} else {
					magicResistChangeScalerFromOther += change;
				}
				break;
			case PropertyType.Dodge:
				if (change < -1 || change > 1) {
					dodgeChangeFromOther += (int)change;
				} else {
					dodgeChangeScalerFromOther += change;
				}
				break;
			case PropertyType.Crit:
				if (change < -1 || change > 1) {
					critChangeFromOther += (int)change;
				} else {
					critChangeScalerFromOther += change;
				}
				break;
			case PropertyType.PhysicalHurtScaler:
				physicalHurtChangeScalerFromOther += change;
				break;
			case PropertyType.MagicalHurtScaler:
				magicalHurtChangeScalerFromOther += change;
				break;
			case PropertyType.CritHurtScaler:
				critHurtChangeScalerFromOther += change;
				break;
			case PropertyType.WholeProperty:
				if (change > -1 && change < 1) {
					maxHealthChangeFromOther += (int)change;
					healthChangeFromOther += (int)change;
					manaChangeFromOther += (int)change;
					attackChangeFromOther += (int)change;
					attackSpeedChangeFromOther += (int)change;
					armorChangeFromOther += (int)change;
					magicResistChangeFromOther += (int)change;
					dodgeChangeFromOther += (int)change;
					critChangeFromOther += (int)change;
					hitChangeFromOther += (int)change;
				} else {
					maxHealthChangeScalerFromOther += change;
					manaChangeScalerFromOther += change;
					attackChangeScalerFromOther += change;
					attackSpeedChangeScalerFromOther += change;
					armorChangeScalerFromOther += change;
					magicResistChangeScalerFromOther += change;
					dodgeChangeScalerFromOther += change;
					critChangeScalerFromOther += change;
					hitChangeScalerFromOther += change;
				}
				break;
			}


		}


		public void AddPropertyChangeFromOther(int maxHealthChangeFromOther,
			int hitChangeFromOther,int attackChangeFromOther,int attackSpeedChangeFromOther,
			int manaChangeFromOther,int armorChangeFromOther,int magicResistChangeFromOther,
			int dodgeChangeFromOther,int critChangeFromOther,
			float maxHealthChangeScalerFromOther,float hitChangeScalerFromOther,
			float attackChangeScalerFromOther,float attackSpeedChangeScalerFromOther,float manaChangeScalerFromOther,
			float armorChangeScalerFromOther,float magicResistChangeScalerFromOther,float dodgeChangeScalerFromOther,
			float critChangeScalerFromOther,float physicalHurtChangeScalerFromOther,
			float magicalHurtChangeScalerFromOther,float critHurtChangeScalerFromOther){

			this.maxHealthChangeFromOther += maxHealthChangeFromOther;
			this.hitChangeFromOther += hitChangeFromOther;
			this.attackChangeFromOther += attackChangeFromOther;
			this.attackSpeedChangeFromOther += attackSpeedChangeFromOther;
			this.manaChangeFromOther += manaChangeFromOther;
			this.armorChangeFromOther += armorChangeFromOther;
			this.magicResistChangeFromOther += magicResistChangeFromOther;
			this.dodgeChangeFromOther += dodgeChangeFromOther;
			this.critChangeFromOther += critChangeFromOther;

			this.maxHealthChangeScalerFromOther += maxHealthChangeScalerFromOther;
			this.hitChangeScalerFromOther += hitChangeScalerFromOther;
			this.manaChangeScalerFromOther += manaChangeScalerFromOther;
			this.attackChangeScalerFromOther += attackChangeScalerFromOther;
			this.attackSpeedChangeScalerFromOther += attackSpeedChangeScalerFromOther;
			this.armorChangeScalerFromOther += armorChangeScalerFromOther;
			this.magicResistChangeScalerFromOther += magicResistChangeScalerFromOther;
			this.dodgeChangeScalerFromOther += dodgeChangeScalerFromOther;
			this.critChangeScalerFromOther += critChangeScalerFromOther;
			this.physicalHurtChangeScalerFromOther += physicalHurtChangeScalerFromOther;
			this.magicalHurtChangeScalerFromOther += magicalHurtChangeScalerFromOther;
			this.critHurtChangeScalerFromOther += critHurtChangeScalerFromOther;
				


		}


//		public List<string> triggeredStatusList = new List<string> ();
//
//		public List<string> consumablesStatusList = new List<string>();
//
//
//		public void RemoveConsumablesStatus(string statusName){
//
//			if (!CheckConsumablesStatusExist(statusName)) {
//				string error = string.Format ("{0}身上没有名为{1}的状态", agentName, statusName);
//				Debug.LogError (error);
//			}
//
//			consumablesStatusList.Remove (statusName);
//
//		}
//
//		public void AddConsumablesStatus(string statusName){
//
//			if (CheckConsumablesStatusExist (statusName)) {
//				return;
//			}
//
//			consumablesStatusList.Add (statusName);
//
//		}
//
//		public bool CheckConsumablesStatusExist(string statuseName){
//			return consumablesStatusList.Contains (statuseName);
//		}
//
//		public void ClearAllStatus(){
//			consumablesStatusList.Clear ();
//			triggeredStatusList.Clear ();
//		}




		/// <summary>
		/// 属性变更
		/// </summary>
		/// <param name="propertyType">Property type.</param>
		/// <param name="gain">Gain.</param>
		public void AgentPropertyChange(PropertyType propertyType,float change){

			switch (propertyType) {
			case PropertyType.MaxHealth:
				if (change > -1 && change < 1) {
					maxHealth = (int)(maxHealth * (1 + change));
					health = (int)(health * (1 + change));
				} else {
					maxHealth += (int)change;
					health += (int)(health * change / maxHealth);
				}
				break;
			case PropertyType.Health:
				if (change > -1 && change < 1) {
					health = (int)(health * (1 + change));
				} else {
					health += (int)(change);
				}
				break;
			case PropertyType.Mana:
				if (change > -1 && change < 1) {
					mana = (int)(mana * (1 + change));
				} else {
					mana += (int)change;
				}
				break;
			case PropertyType.Attack:
				if (change > -1 && change < 1) {
					attack = (int)(attack * (1 + change));
				} else {
					attack += (int)change;
				}
				break;
			case PropertyType.AttackSpeed:
				if (change > -1 && change < 1) {
					attackSpeed = (int)(attackSpeed * (1 + change));
				} else {
					attackSpeed += (int)change;
				}
				break;
			case PropertyType.Armor:
				if (change > -1 && change < 1) {
					armor = (int)(armor * (1 + change));
				} else {
					armor += (int)change;
				}
				break;
			case PropertyType.MagicResist:
				if (change > -1 && change < 1) {
					magicResist = (int)(magicResist * (1 + change));
				} else {
					magicResist += (int)change;
				}
				break;
			case PropertyType.Dodge:
				if (change > -1 && change < 1) {
					dodge = (int)(dodge * (1 + change));
				} else {
					dodge += (int)change;
				}
				break;
			case PropertyType.Crit:
				if (change > -1 && change < 1) {
					crit = (int)(crit * (1 + change));
				} else {
					crit += (int)change;
				}
				break;
			case PropertyType.PhysicalHurtScaler:
				physicalHurtScaler += change;
				break;
			case PropertyType.MagicalHurtScaler:
				magicalHurtScaler += change;
				break;
			case PropertyType.CritHurtScaler:
				critHurtScaler += change;
				break;
			case PropertyType.WholeProperty:
				if (change > -1 && change < 1) {
					maxHealth = (int)(maxHealth * (1 + change));
					mana = (int)(mana * (1 + change));
					attack = (int)(attack * (1 + change));
					attackSpeed = (int)(attackSpeed * (1 + change));
					armor = (int)(armor * (1 + change));
					magicResist = (int)(magicResist * (1 + change));
					dodge = (int)(dodge * (1 + change));
					crit = (int)(crit * (1 + change));
					hit = (int)(hit * (1 + change));
				} else {
					maxHealth += (int)maxHealth;
					mana += (int)change;
					attack += (int)change;
					attackSpeed += (int)change;
					armor += (int)change;
					magicResist += (int)change;
					dodge += (int)change;
					crit += (int)change;
					hit += (int)change;
				}
				break;
			}

		}

		public void AgentPropertySetToValue(PropertyType propertyType,float propertyValue){

			switch (propertyType) {
			case PropertyType.MaxHealth:
				maxHealth = (int)propertyValue;
				break;
			case PropertyType.Mana:
				mana = (int)propertyValue;
				break;
			case PropertyType.Attack:
				attack = (int)propertyValue;
				break;
			case PropertyType.AttackSpeed:
				attackSpeed = (int)propertyValue;
				break;
			case PropertyType.Hit:
				hit = (int)propertyValue;
				break;
			case PropertyType.Armor:
				armor = (int)propertyValue;
				break;
			case PropertyType.MagicResist:
				magicResist = (int)propertyValue;
				break;
			case PropertyType.Dodge:
				dodge = (int)propertyValue;
				break;
			case PropertyType.Crit:
				crit = (int)propertyValue;
				break;
			case PropertyType.PhysicalHurtScaler:
				physicalHurtScaler = propertyValue;
				break;
			case PropertyType.MagicalHurtScaler:
				magicalHurtScaler = propertyValue;
				break;
			case PropertyType.CritHurtScaler:
				critHurtScaler = propertyValue;
				break;

			}

		}

		public float GetAgentPropertyWithType(PropertyType propertyType){

			float propertyValue = 0;

			switch (propertyType) {
			case PropertyType.MaxHealth:
				propertyValue = maxHealth;
				break;
			case PropertyType.Health:
				propertyValue = health;
				break;
			case PropertyType.Mana:
				propertyValue = mana;
				break;
			case PropertyType.Attack:
				propertyValue = attack;
				break;
			case PropertyType.AttackSpeed:
				propertyValue = attackSpeed;
				break;
			case PropertyType.Hit:
				propertyValue = hit;
				break;
			case PropertyType.Armor:
				propertyValue = armor;
				break;
			case PropertyType.MagicResist:
				propertyValue = magicResist;
				break;
			case PropertyType.Dodge:
				propertyValue = dodge;
				break;
			case PropertyType.Crit:
				propertyValue = crit;
				break;
			case PropertyType.PhysicalHurtScaler:
				propertyValue = physicalHurtScaler;
				break;
			case PropertyType.MagicalHurtScaler:
				propertyValue = magicalHurtScaler;
				break;
			case PropertyType.CritHurtScaler:
				propertyValue = critHurtScaler;
				break;
			}

			return propertyValue;
		}





		public override string ToString ()
		{
			return string.Format ("[agent]:" + agentName +
				"\n[attack]:" + attack + 
				"\n[mana]:" + mana +
				"\n[crit]:" + crit +
				"\n[armor]:" + armor +
				"\n[manaResist]:" + magicResist +
				"\n[agiglity]:" + dodge +
				"\n[maxHealth]:" + maxHealth +
				"\n[mana]:" + mana);
		}
	}




}
