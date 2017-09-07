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



		//*****初始信息********//
		public int originalMaxHealth;
		public int originalMaxMana;
		public int originalHealth;
		public int originalMana;
		public int originalAttack;
		public int originalAttackSpeed;
		public int originalCrit;
		public int originaldodge;
		public int originalarmour;
		public int originalManaResist;
		//*****初始信息********//

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
		public int armour;//护甲
		public int manaResist;//魔抗
		public int crit;//暴击


		public float reflectScaler;
		public float decreaseHurtScaler;

		public int magicBase;

		private float mAttackSpeedGainScaler;
		public float attackSpeedGainScaler{
			get{ return mAttackSpeedGainScaler; }
			set{
				mAttackSpeedGainScaler = value;
				ResetBattleAgentProperties ();
			}
		}

		private float mArmourGainScaler;
		public float armourGainScaler{
			get{ return mArmourGainScaler; }
			set{
				mArmourGainScaler = value; 
				ResetBattleAgentProperties ();
			}
				
		}

		private float mManaResistGainScaler;
		public float manaResistGainScaler{
			get{ return mManaResistGainScaler; }
			set{
				mManaResistGainScaler = value;
				ResetBattleAgentProperties ();
			}
		}

		private float mCritGainScalser;
		public float critGainScaler{
			get{ return mCritGainScalser; }
			set{
				mCritGainScalser = value;
				ResetBattleAgentProperties ();
			}
		}

		private float mDodgeGainScaler;
		public float dodgeGainScaler{
			get{ return mDodgeGainScaler; }
			set{
				mDodgeGainScaler = value;
				ResetBattleAgentProperties ();
			}
		}

//		public int healthGainScaler;//力量对最大血量的加成系数

		public ValidActionType validActionType = ValidActionType.All;// 有效的行动类型

		public float physicalHurtScaler;//物理伤害系数

		public float magicalHurtScaler;//魔法伤害系数

		public float critScaler;//暴击伤害系数

		public float healthAbsorbScalser;//回血比例

		public List<Skill> equipedSkills = new List<Skill>();//技能数组

		private List<Equipment> mAllEquipedEquipments = new List<Equipment>();
		public List<Equipment> allEquipedEquipments{
			get{
				if (mAllEquipedEquipments.Count == 0) {
					mAllEquipedEquipments.AddRange (new Equipment[3]{ null, null, null});
				}
				return mAllEquipedEquipments;
			}
			set{
				mAllEquipedEquipments = value;
			}
		}


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

		public List<Item> allItems = new List<Item> (); // 所有物品

//		[HideInInspector]public Item healthBottle;
//
//		[HideInInspector]public Item manaBottle;
//
//		[HideInInspector]public Item antiDebuffBottle;

//		public List<StateSkillEffect> states = new List<StateSkillEffect>();//状态数组

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

			critScaler = 1.0f;//暴击伤害系数

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
//			this.originalarmour = ba.originalarmour;
//			this.originalManaResist = ba.originalManaResist;

			this.maxHealth = ba.maxHealth;
			this.maxMana = ba.maxMana;

			this.health = ba.health;

			this.attack = ba.attack;//攻击力
			this.attackSpeed = ba.attackSpeed;//攻速
			this.mana = ba.mana;//魔法
			this.dodge = ba.dodge;//敏捷
			this.armour = ba.armour;//护甲
			this.manaResist = ba.manaResist;//魔抗
			this.crit = ba.crit;//暴击

			this.equipedSkills = ba.equipedSkills;

			this.allEquipedEquipments = ba.allEquipedEquipments;

			this.allItems = ba.allItems;

		}

//		public void CopyAgentStatus(BattleAgentModel ba){
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
//			this.originalarmour = ba.originalarmour;
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
//			this.armour = ba.armour;//护甲
//			this.manaResist = ba.manaResist;//魔抗
//			this.crit = ba.crit;//暴击
//
//			this.isActive = ba.isActive;
//
//		}

//		//添加状态 
//		public void AddState(StateSkillEffect sse){
//			states.Add (sse);
//			ResetBattleAgentProperties (false,false);
//		}
//		//删除状态
//		public void RemoveState(StateSkillEffect sse){
//			for(int i = 0;i<states.Count;i++){
//				if (sse.effectName == states[i].effectName) {
//					states.RemoveAt(i);
//					Destroy (sse);
//					ResetBattleAgentProperties (false,false);
//					return;
//				}
//			}
//		}



		private void ResetPropertiesByEquipment(Equipment equipment){

			if (equipment.itemName == null) {
				return;
			}

//			health += equipment.healthGain;
//			mana += equipment.manaGain;

			attack += equipment.attackGain;

			attackSpeed = (int)((attackSpeed + equipment.attackSpeedGain) * (1 + attackSpeedGainScaler));
			crit = (int)((crit + equipment.critGain) * (1 + critGainScaler));
			armour = (int)((armour + equipment.armourGain) * (1 + armourGainScaler));
			manaResist = (int)((manaResist + equipment.manaResistGain) * (1 + manaResistGainScaler));
			dodge = (int)((dodge + equipment.dodgeGain) * (1 + dodgeGainScaler));

		}

		// 仅根据物品重新计人物的属性，其余属性重置为初始状态
		public void ResetBattleAgentProperties (bool toOriginalState = false)
		{
			// 所有属性重置为初始值
			attack = originalAttack;
			mana = originalMana;
			crit = originalCrit;
			armour = originalarmour;
			manaResist = originalManaResist;
			dodge = originaldodge;

			// 根据装备更新属性
			foreach (Equipment equipment in allEquipedEquipments) {
				if (equipment != null) {
					ResetPropertiesByEquipment (equipment);
				}
			}


			critScaler = 1.0f;//暴击伤害系数
			healthAbsorbScalser = 0f;//吸血比例
			attackTime = 1;

			if (toOriginalState) {
				validActionType = ValidActionType.All;
				health = maxHealth;
				mana = maxMana;

			}

		}


		//添加状态 
//		public void AddState(StateSkillEffect sse){
//			states.Add (sse);
//			ResetBattleAgentProperties (false,false);
//		}
//		//删除状态
//		public void RemoveState(StateSkillEffect sse){
//			for(int i = 0;i<states.Count;i++){
//				if (sse.effectName == states[i].effectName) {
//					states.RemoveAt(i);
//					Destroy (sse);
//					ResetBattleAgentProperties (false,false);
//					return;
//				}
//			}
//		}


//		public void AgentDie(CallBack cb){
//			baController.AgentDieAnim (cb);
//		}

		public override string ToString ()
		{
			return string.Format ("[agent]:" + agentName +
				"\n[attack]:" + attack + 
				"\n[mana]:" + mana +
				"\n[crit]:" + crit +
				"\n[armour]:" + armour +
				"\n[manaResist]:" + manaResist +
				"\n[agiglity]:" + dodge +
				"\n[maxHealth]:" + maxHealth +
				"\n[maxMana]:" + maxMana);
		}
	}



	[System.Serializable]
	public class BattleAgentModel{

		public string agentName;

		public string agentIconName;

		public bool isActive = true;

		public int agentLevel;

		//*****初始信息********//
		public int originalMaxHealth;
		public int originalMaxStrength;
		public int originalHealth;
		public int originalStrength;
		public int originalAttack;
		public int originalPower;
		public int originalMana;
		public int originalCrit;
		public int originaldodge;
		public int originalarmour;
		public int originalManaResist;
		//*****初始信息********//

		public int maxHealth;//最大血量
		public int maxStrength;//最大气力值

		public int health;
		public int strength;

		public int attack;//攻击力
		public int power;//力量
		public int mana;//魔法
		public int dodge;//敏捷
		public int armour;//护甲
		public int manaResist;//魔抗
		public int crit;//暴击


	}
}
