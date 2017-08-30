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
		public int originalAgility;
		public int originalAmour;
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
		public int agility;//敏捷
		public int amour;//护甲
		public int manaResist;//魔抗
		public int crit;//暴击


//		public int healthGainScaler;//力量对最大血量的加成系数

		public ValidActionType validActionType = ValidActionType.All;// 有效的行动类型

		public float physicalHurtScaler;//物理伤害系数

		public float magicalHurtScaler;//魔法伤害系数

		public float critScaler;//暴击伤害系数

		public float healthAbsorbScalser;//回血比例

		public List<Skill> equipedSkills = new List<Skill>();//技能数组

		private List<Item> mAllEquipedItems = new List<Item>();
		public List<Item> allEquipedItems{
			get{
				if (mAllEquipedItems.Count == 0) {
					mAllEquipedItems.AddRange (new Item[6]{ null, null, null, null, null, null });
				}
				return mAllEquipedItems;
			}
			set{
				mAllEquipedItems = value;
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

//			healthGainScaler = 1;//力量对最大血量的加成系数

			validActionType = ValidActionType.All;// 有效的行动类型

			physicalHurtScaler = 1.0f;//伤害系数

			magicalHurtScaler = 1.0f;

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
//			this.originalAgility = ba.originalAgility;
//			this.originalAmour = ba.originalAmour;
//			this.originalManaResist = ba.originalManaResist;

			this.maxHealth = ba.maxHealth;
			this.maxMana = ba.maxMana;

			this.health = ba.health;

			this.attack = ba.attack;//攻击力
			this.attackSpeed = ba.attackSpeed;//攻速
			this.mana = ba.mana;//魔法
			this.agility = ba.agility;//敏捷
			this.amour = ba.amour;//护甲
			this.manaResist = ba.manaResist;//魔抗
			this.crit = ba.crit;//暴击

			this.equipedSkills = ba.equipedSkills;

			this.allEquipedItems = ba.allEquipedItems;

			this.allItems = ba.allItems;

//			foreach (StateSkillEffect state in ba.states) {
//				state.transform.SetParent (this.transform.FindChild ("States").transform);
//			}
//			ba.states.Clear ();
//			this.states = ba.states;

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
//			this.originalAgility = ba.originalAgility;
//			this.originalAmour = ba.originalAmour;
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
//			this.agility = ba.agility;//敏捷
//			this.amour = ba.amour;//护甲
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



		private void ResetPropertiesByEquipment(Item equipment){

			if (equipment.itemName == null) {
				return;
			}

			attack += equipment.attackGain;
			mana += equipment.manaGain;
			crit += equipment.critGain;
			amour += equipment.amourGain;
			manaResist += equipment.manaResistGain;
			agility += equipment.agilityGain;

		}

		// 仅根据物品重新计人物的属性，其余属性重置为初始状态
		public void ResetBattleAgentProperties (bool toOriginalState,bool firstEnterBattleOrQuitBattle)
		{
			// 所有属性重置为初始值
			attack = originalAttack;
			mana = originalMana;
			crit = originalCrit;
			amour = originalAmour;
			manaResist = originalManaResist;
			agility = originalAgility;

			// 根据装备更新属性

			foreach (Item item in allEquipedItems) {
				if (item != null && item.itemType != ItemType.Consumables) {
					ResetPropertiesByEquipment (item);
				}
			}

//			maxHealth = originalMaxHealth + healthGainScaler * power;
//			maxMana = originalMaxMana + (int)( * power);

			physicalHurtScaler = 1.0f;// 物理伤害系数
			magicalHurtScaler = 1.0f;// 魔法伤害系数
			critScaler = 1.0f;//暴击伤害系数
			healthAbsorbScalser = 0f;//吸血比例
			attackTime = 1;

			if (toOriginalState) {
				validActionType = ValidActionType.All;
				health = maxHealth;
				mana = maxMana;

				foreach (Skill s in equipedSkills) {
					s.isAvalible = true;
//					foreach (BaseSkillEffect bse in s.skillEffects) {
//						bse.actionCount = 0;
//					}
				}
			}

			if (firstEnterBattleOrQuitBattle) {
				validActionType = ValidActionType.All;
				foreach (Skill s in equipedSkills) {
					s.isAvalible = true;
//					foreach (BaseSkillEffect bse in s.skillEffects) {
//						bse.actionCount = 0;
//					}
				}

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
				"\n[amour]:" + amour +
				"\n[manaResist]:" + manaResist +
				"\n[agiglity]:" + agility +
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
		public int originalAgility;
		public int originalAmour;
		public int originalManaResist;
		//*****初始信息********//

		public int maxHealth;//最大血量
		public int maxStrength;//最大气力值

		public int health;
		public int strength;

		public int attack;//攻击力
		public int power;//力量
		public int mana;//魔法
		public int agility;//敏捷
		public int amour;//护甲
		public int manaResist;//魔抗
		public int crit;//暴击


	}
}
