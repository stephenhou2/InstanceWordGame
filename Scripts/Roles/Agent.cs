using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace WordJourney
{
	public abstract class Agent : MonoBehaviour {

		public string agentName;
		public string agentIconName;

		public bool isActive;

		public int agentLevel;



		//*****人物基础信息********//
		public int originalAttack;
		public int originalAttackSpeed;
		public int originalArmor;
		public int originalManaResist;
		public int originalDodge;
		public int originalCrit;
		public int originalMaxHealth;
		public int originalMaxMana;
		public int originalHealth;
		public int originalMana;
		//*****人物基础信息*******//




		public int maxHealth;//最大血量
		public int maxMana;//最大魔法值


		private int mHealth;
		public int health{
			get{ return mHealth; }
			set{ 
				if (value >= 0) {
					mHealth = value;
				} else {
					mHealth = 0;
				}
			}
		}

		private int mMana;
		public int mana{
			get{ return mMana; }
			set{
				if (value >= 0) {
					mMana = value;
				} else {
					mMana = 0;
				}
			}
		}

		public int attack;//攻击力
		public int attackSpeed;//攻速
		public int dodge;//敏捷
		public int armor;//护甲
		public int manaResist;//魔抗
		public int crit;//暴击



		//*****基础属性加成比例，只由技能影响*******//
		private float baseMaxHealthGainScaler;
		private float baseMaxManaGainScaler;
		private float baseAttackGainScaler;
		private float baseAttackSpeedGainScaler;
		private float baseArmorGainScaler;
		private float baseManaResistGainScaler;
		private float baseDodgeGainScaler;
		private float baseCritGainScaler; 
		//*****基础属性加成比例，只由技能影响*******//


		public float maxHealthGainScaler;
		public float maxManaGainScaler;
		public float attackGainScaler;
		public float attackSpeedGainScaler;
		public float armorGainScaler;
		public float manaResistGainScaler;
		public float dodgeGainScaler;
		public float critGainScaler;


		public ValidActionType validActionType = ValidActionType.All;// 有效的行动类型

		public float physicalHurtScaler;//物理伤害系数

		public float magicalHurtScaler;//魔法伤害系数

		public float critHurtScaler;//暴击伤害系数

		public float healthAbsorbScalser;//回血比例

		public float hardBeatChance;//打出重击的基础概率

		public float reflectScaler;//荆棘护甲反弹伤害比例

		public float decreaseHurtScaler;//魔法盾减伤比例



		public List<Skill> equipedSkills = new List<Skill>();//技能数组

		public List<Equipment> allEquipedEquipments = new List<Equipment>();

//		private List<Equipment> mAllEquipedEquipments = new List<Equipment>();
//		public List<Equipment> allEquipedEquipments{
//			get{
//				if (mAllEquipedEquipments.Count == 0) {
//					mAllEquipedEquipments.AddRange (new Equipment[3]{ null, null, null});
//				}
//				return mAllEquipedEquipments;
//			}
//			set{
//				mAllEquipedEquipments = value;
//			}
//		}


//		private List<Consumable> mAllEquipedConsumables = new List<Consumable>();
//		public List<Consumable> allEquipedConsumables{
//			get{
//				if (mAllEquipedConsumables.Count == 0) {
//					mAllEquipedConsumables.AddRange(new Consumable[3]{null,null,null});
//				}
//				return mAllEquipedConsumables;
//			}
//			set{
//				mAllEquipedConsumables = value;
//			}
//		}
//			
//		private List<Equipment> mAllEquipedEquipments = new List<Equipment>();
//		public List<Equipment> allEquipedEquipments{
//			get{
//				if (mAllEquipedEquipments.Count == 0) {
//					mAllEquipedEquipments.AddRange(new Equipment[3]{null,null,null});
//				}
//				return mAllEquipedEquipments;
//			}
//			set{
//				mAllEquipedEquipments = value;
//			}
//		}

//		public List<Item> allItems = new List<Item> (); // 所有物品


		public int attackTime;//攻击次数

		public float attackInterval{
			get{

				float ai = 1f / (1 + 0.01f * attackSpeed);
				int tempt = (int)(ai * 100);
				return tempt/100f;

			}
		}





		public virtual void Awake(){

			isActive = true; // 角色初始化后默认可以行动

			validActionType = ValidActionType.All;// 有效的行动类型

			critHurtScaler = 1.0f;//暴击伤害系数

			healthAbsorbScalser = 0f;//回血比例

			attackTime = 1;//攻击次数

			mHealth = maxHealth;
			mMana = maxMana;

		}


		public void CopyAgentStatus(Agent ba){

//			this.originalMaxHealth = ba.originalMaxHealth;
//			this.originalMaxStrength = ba.originalMaxStrength;
//			this.originalHealth = ba.originalHealth;
//			this.originalStrength = ba.originalStrength;
//			this.originalAttack = ba.originalAttack;
//			this.originalPower = ba.originalPower;
//			this.originalMana = ba.originalMana;
//			this.originalCrit = ba.originalCrit;
//			this.originaldodge = ba.originaldodge;
//			this.originalarmor = ba.originalarmor;
//			this.originalManaResist = ba.originalManaResist;

//			this.maxHealth = ba.maxHealth;
//			this.maxMana = ba.maxMana;
//
//			this.health = ba.health;
//
//			this.attack = ba.attack;//攻击力
//			this.attackSpeed = ba.attackSpeed;//攻速
//			this.mana = ba.mana;//魔法
//			this.dodge = ba.dodge;//敏捷
//			this.armor = ba.armor;//护甲
//			this.manaResist = ba.manaResist;//魔抗
//			this.crit = ba.crit;//暴击
//
//			this.equipedSkills = ba.equipedSkills;

//			this. = ba.allEquipedEquipments;


		}

//		public void CopyAgentStatus(PlayerData ba){
//
//			this.agentIconName = ba.agentIconName;
//
//			this.originalMaxHealth = ba.originalMaxHealth;
//			this.originalMaxStrength = ba.originalMaxStrength;
//			this.originalHealth = ba.originalHealth;
//			this.originalStrength = ba.originalStrength;
//			this.originalAttack = ba.originalAttack;
//			this.originalPower = ba.originalPower;
//			this.originalMana = ba.originalMana;
//			this.originalCrit = ba.originalCrit;
//			this.originaldodge = ba.originaldodge;
//			this.originalarmor = ba.originalarmor;
//			this.originalManaResist = ba.originalManaResist;
//
//			this.maxHealth = ba.maxHealth;
//			this.maxStrength = ba.maxStrength;
//			this.health = ba.health;
//
//			this.strength = ba.strength;
//
//
//			this.attack = ba.attack;//攻击力
//			this.power = ba.power;//力量
//			this.mana = ba.mana;//魔法
//			this.dodge = ba.dodge;//敏捷
//			this.armor = ba.armor;//护甲
//			this.manaResist = ba.manaResist;//魔抗
//			this.crit = ba.crit;//暴击
//
//			this.isActive = ba.isActive;
//
//		}



		/// <summary>
		/// 根据装备更新人物属性&属性加成
		/// </summary>
		/// <param name="equipment">Equipment.</param>
		private void ResetPropertiesByEquipment(Equipment equipment){

			if (equipment.itemName == null) {
				return;
			}

			// 装备的该项属性>=1时，更新人物的基础属性
			// 装备的该项属性处于0～1之间时，说明是属性加成，更新人物的属性加成
			if (equipment.attackGain >= 1) {
				attack += (int)equipment.attackGain;
			} else if (equipment.attackGain > 0){
				attackGainScaler += equipment.attackGain;
			}

			if (equipment.attackSpeedGain >= 1) {
				attackSpeed += (int)equipment.attackSpeedGain;
			} else if (equipment.attackSpeedGain > 0) {
				attackSpeedGainScaler += equipment.attackSpeedGain;
			}

			if (equipment.armorGain >= 1) {
				armor += (int)equipment.armorGain;
			} else if (equipment.armorGain > 0) {
				armorGainScaler += equipment.armorGain;
			}

			if (equipment.manaResistGain >= 1) {
				manaResist += (int)equipment.manaResistGain;
			} else if (equipment.manaResistGain > 0) {
				manaResistGainScaler += equipment.manaResistGain;
			}

			if (equipment.dodgeGain >= 1) {
				dodge += (int)equipment.dodgeGain;
			} else if (equipment.dodgeGain > 0) {
				dodgeGainScaler += equipment.dodgeGain;
			}

			if (equipment.critGain >= 1) {
				crit += (int)equipment.critGain;
			} else if (equipment.critGain > 0) {
				critGainScaler += equipment.critGain;
			}

			if (equipment.healthGain >= 1) {
				maxHealth += (int)equipment.healthGain;
			} else if (equipment.healthGain > 0) {
				maxHealthGainScaler += equipment.healthGain;
			}

			if (equipment.manaGain >= 1) {
				maxMana += (int)equipment.manaGain;
			} else if (equipment.manaGain > 0) {
				maxManaGainScaler += equipment.manaGain;
			}

			for (int i = 0; i < equipment.attachedProperties.Count; i++) {
				equipment.attachedProperties [i].RebuildPropertiesOf (this);
			}


		}

		// 仅根据物品重新计人物的属性，其余属性重置为初始状态
		public void ResetBattleAgentProperties (bool toOriginalState = false)
		{
			// 属性重置为没有任何装备时的基础属性
			attack = originalAttack;
			attackSpeed = originalAttackSpeed;
			crit = originalCrit;
			armor = originalArmor;
			manaResist = originalManaResist;
			dodge = originalDodge;
			maxHealth = originalMaxHealth;
			maxMana = originalMaxMana;

			// 属性加成重置为只考虑技能时的属性加成
			maxHealthGainScaler = baseMaxHealthGainScaler;
			maxManaGainScaler = baseMaxManaGainScaler;
			attackGainScaler = baseAttackGainScaler;
			attackSpeedGainScaler = baseAttackSpeedGainScaler;
			armorGainScaler = baseArmorGainScaler;
			manaResistGainScaler = baseManaResistGainScaler;
			dodgeGainScaler = baseDodgeGainScaler;
			critGainScaler = baseCritGainScaler;

			// 根据装备计算人物的总的基础属性和总的各项属性加成比例
			foreach (Equipment equipment in allEquipedEquipments) {
				if (equipment != null) {
					ResetPropertiesByEquipment (equipment);
				}
			}

			// 根据属性加成重新计算人物真实属性
			attack = (int)(attack * (1 + attackGainScaler));
			attackSpeed = (int)(attackSpeed * (1 + attackSpeedGainScaler));
			armor = (int)(armor * (1 + armorGainScaler));
			manaResist = (int)(manaResist * (1 + manaResistGainScaler));
			dodge = (int)(dodge * (1 + dodgeGainScaler));
			crit = (int)(crit * (1 + critGainScaler));
			maxHealth = (int)(maxHealth * (1 + maxHealthGainScaler));
			maxMana = (int)(maxMana * (1 + maxManaGainScaler));


		

			critHurtScaler = 1.0f;//暴击伤害系数
			healthAbsorbScalser = 0f;//吸血比例
			attackTime = 1;

			if (toOriginalState) {
				validActionType = ValidActionType.All;
				health = maxHealth;
				mana = maxMana;

			}

		}

		/// <summary>
		/// 由技能设置基础属性加成比例,并更新对象属性
		/// </summary>
		/// <param name="baseAttackGainScaler">基础攻击加成比例.</param>
		/// <param name="baseAttackSpeedGainScaler">基础攻速加成比例.</param>
		/// <param name="baseArmorGainScaler">基础护甲加成比例.</param>
		/// <param name="baseManaResistGainScaler">基础抗性加成比例.</param>
		/// <param name="baseDodgeGainScaler">基础闪避加成比例.</param>
		/// <param name="baseCritGainScaler">基础暴击加成比例.</param>
		/// <param name="baseMaxHealthGainScaler">基础最大血量加成比例.</param>
		/// <param name="baseMaxManaGainScaler">基础最大魔法加成比例.</param>
		public void SetBasePropertyGainScalers(float baseAttackGainScaler,float baseAttackSpeedGainScaler,
			float baseArmorGainScaler,float baseManaResistGainScaler,float baseDodgeGainScaler,
			float baseCritGainScaler,float baseMaxHealthGainScaler,float baseMaxManaGainScaler){

			if (baseAttackGainScaler > 0) {
				this.baseAttackGainScaler = baseAttackGainScaler;
			}
			if (baseAttackSpeedGainScaler > 0) {
				this.baseAttackSpeedGainScaler = baseAttackSpeedGainScaler;
			}
			if (baseArmorGainScaler > 0) {
				this.baseArmorGainScaler = baseArmorGainScaler;
			}
			if (baseManaResistGainScaler > 0) {
				this.baseManaResistGainScaler = baseManaResistGainScaler;
			}
			if (baseDodgeGainScaler > 0) {
				this.baseDodgeGainScaler = baseDodgeGainScaler;
			}
			if (baseCritGainScaler > 0) {
				this.baseCritGainScaler = baseCritGainScaler;
			}
			if (baseMaxHealthGainScaler > 0) {
				this.baseMaxHealthGainScaler = baseMaxHealthGainScaler;
			}
			if (baseManaResistGainScaler > 0) {
				this.baseMaxManaGainScaler = baseMaxManaGainScaler;
			}

			ResetBattleAgentProperties (false);

		}


		public override string ToString ()
		{
			return string.Format ("[agent]:" + agentName +
				"\n[attack]:" + attack + 
				"\n[mana]:" + mana +
				"\n[crit]:" + crit +
				"\n[armor]:" + armor +
				"\n[manaResist]:" + manaResist +
				"\n[agiglity]:" + dodge +
				"\n[maxHealth]:" + maxHealth +
				"\n[maxMana]:" + maxMana);
		}
	}




}
