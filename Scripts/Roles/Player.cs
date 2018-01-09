﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace WordJourney
{
	public class Player : Agent {

		private static volatile Player mPlayerSingleton;

//		private static object objectLock = new System.Object();

		// 玩家角色单例
		public static Player mainPlayer{
			get{
				if (mPlayerSingleton == null) {
//					lock (objectLock) {
//						Player[] existPlayers = GameObject.FindObjectsOfType<Player>();
//						if (existPlayers != null) {
//							for (int i = 0; i < existPlayers.Length; i++) {
//								Destroy (existPlayers [i].gameObject);
//							}
//						}
//
//						ResourceLoader playerLoader = ResourceLoader.CreateNewResourceLoader ();
//
//						ResourceManager.Instance.LoadAssetsWithBundlePath<GameObject> (playerLoader, CommonData.mainStaticBundleName, () => {
//							mPlayerSingleton = playerLoader.gos[0].GetComponent<Player> ();
//							mPlayerSingleton.transform.SetParent (null);
//							mPlayerSingleton.ResetBattleAgentProperties (true);
//						},"Player");

						mPlayerSingleton = TransformManager.FindTransform ("Player").GetComponent<Player>();

						DontDestroyOnLoad (mPlayerSingleton);
//					}
				} 
				return mPlayerSingleton;
			}

		}
			
			
		public int experience;//玩家经验值

		public int totalCoins;//玩家金币数量

		// 每次升级所需要的经验值
		public int upgradeExprience{
			get{
				return 50 * agentLevel * (agentLevel + 1);
			}
		}


		public List<Item> allItemsInBag = new List<Item>();
		public List<Consumables> allConsumablesInBag = new List<Consumables> ();
//		public List<FuseStone> allFuseStonesInBag = new List<FuseStone>();
//		public List<TaskItem> allTaskItemsInBag = new List<TaskItem>();
		public List<Formula> allFormulasInBag = new List<Formula>();//所有背包中的配方

//		public Consumables[] consumablesEquiped;

		public int maxUnlockLevelIndex;

		public int currentLevelIndex;

		public void SetUpPlayerWithPlayerData(PlayerData playerData){

			if (playerData == null) {
				return;
			}

			this.agentName = playerData.agentName;
			this.agentIconName = playerData.agentIconName;
			this.agentLevel = playerData.agentLevel;
			this.isActive = false;
//			this.isActive = playerData.isActive;

			this.originalMaxHealth = playerData.originalMaxHealth;
			this.originalMana = playerData.originalMana;
			this.originalAttack = playerData.originalAttack;
			this.originalAttackSpeed = playerData.originalAttackSpeed;
			this.originalArmor = playerData.originalArmor;
			this.originalMagicResist = playerData.originalManaResist;
			this.originalCrit = playerData.originalCrit;
			this.originalDodge = playerData.originalDodge;
			this.originalHealth = playerData.originalHealth;
			this.originalMana = playerData.originalMana;

			this.originalPhysicalHurtScaler = 1.0f;
			this.originalMagicalHurtScaler = 1.0f;
			this.originalCritHurtScaler = 1.5f;

//			this.attack = playerData.attack;//攻击力
//			this.attackSpeed = playerData.attackSpeed;//攻速
//			this.armor = playerData.armor;//护甲
//			this.magicResist = playerData.manaResist;//魔抗
//			this.dodge = playerData.dodge;//闪避
//			this.crit = playerData.crit;//暴击
//			this.maxHealth = playerData.maxHealth;//最大生命值
//			this.mana = playerData.mana;//法强
//			this.health = playerData.health;//生命
//			this.mana = playerData.mana;//魔法

			this.charactersCount = playerData.charactersCount;

//			this.allMaterialsInBag = playerData.allMaterialsInBag;
			this.allEquipmentsInBag = playerData.allEquipmentsInBag;
			this.allEquipedEquipments = playerData.allEquipedEquipments;
			this.allConsumablesInBag = playerData.allConsumablesInBag;
//			this.allFuseStonesInBag = playerData.allFuseStonesInBag;
//			this.allTaskItemsInBag = playerData.allTaskItemsInBag;
//			this.allCharacterFragmentsInBag = playerData.allCharacterFragmentsInBag;
			this.allFormulasInBag = playerData.allFormulasInBag;

			this.maxUnlockLevelIndex = playerData.maxUnlockLevelIndex;
			this.currentLevelIndex = playerData.currentLevelIndex;

			this.attachedEquipmentSkills.Clear ();
			this.attachedConsumablesSkills.Clear ();

			ResetBattleAgentProperties (false);

			for (int i = 0; i < playerData.allEquipedEquipments.Length; i++) {

				Equipment equipment = playerData.allEquipedEquipments [i];

				if (equipment.itemId > 0) {

					for (int j = 0; j < equipment.attachedSkillInfos.Length; j++) {
						
						TriggeredSkill attachedSkill = SkillGenerator.Instance.GenerateTriggeredSkill (equipment, equipment.attachedSkillInfos [j], triggeredSkillsContainer);

						equipment.attachedSkills.Add (attachedSkill);

						attachedEquipmentSkills.Add (attachedSkill);
					}
				}

			}



//			this.skillPointsLeft = playerData.skillPointsLeft;

		}

//		public override void ResetBattleAgentProperties (bool toOriginalState = false)
//		{
//			PropertyChange changeByEquipment = base.ResetBattleAgentProperties (toOriginalState);
//			ResetPropertiesByConsumablesEffectState ();
//
//		}


		/// <summary>
		/// 角色卸下装备
		/// </summary>
		/// <param name="equipment">Equipment.</param>
		/// <param name="equipmentIndexInPanel">Equipment index in panel.</param>
		public PropertyChange UnloadEquipment(Equipment equipment,int equipmentIndexInPanel){

			equipment.equiped = false;

			Debug.LogFormat ("卸下装备{0}/{1}", equipmentIndexInPanel,allEquipedEquipments.Length);

			if (equipment.itemId < 0) {
				return new PropertyChange();
			}

			allEquipmentsInBag.Add (equipment);
			allItemsInBag.Add (equipment);

			for (int i = 0; i < equipment.attachedSkills.Count; i++) {
				TriggeredSkill attachedSkill = equipment.attachedSkills [i];
				attachedEquipmentSkills.Remove (attachedSkill);
				equipment.attachedSkills.RemoveAt (i);
				Destroy (attachedSkill.gameObject);
			}


			Equipment emptyEquipment = new Equipment ();

			allEquipedEquipments [equipmentIndexInPanel] = emptyEquipment;

//			equipmentDragControl.item = emptyEquipment;

			return ResetBattleAgentProperties (false);

		}



		/// <summary>
		/// 角色穿上装备
		/// </summary>
		/// <param name="equipment">Equipment.</param>
		/// <param name="equipmentIndexInPanel">Equipment index in panel.</param>
		public PropertyChange EquipEquipment(Equipment equipment,int equipmentIndexInPanel){

			equipment.equiped = true;

			Debug.LogFormat ("穿上装备{0}", equipmentIndexInPanel);

			allEquipedEquipments [equipmentIndexInPanel] = equipment;

//			equipmentDragControl.item = equipment;

			for (int i = 0; i < equipment.attachedSkillInfos.Length; i++) {
				TriggeredSkill attachedSkill = SkillGenerator.Instance.GenerateTriggeredSkill (equipment, equipment.attachedSkillInfos [i],triggeredSkillsContainer);
				equipment.attachedSkills.Add (attachedSkill);
				attachedEquipmentSkills.Add (attachedSkill);
				attachedSkill.transform.SetParent (triggeredSkillsContainer);
			}

			allItemsInBag.Remove (equipment);
			allEquipmentsInBag.Remove (equipment);

			return ResetBattleAgentProperties (false);

		}


		public Agent.PropertyChange UseMedicines(Consumables consumables){

			if (consumables.attachedSkillInfos.Length > 0) {
				for (int i = 0; i < consumables.attachedSkillInfos.Length; i++) {
					SkillInfo si = consumables.attachedSkillInfos [i];
					ConsumablesSkill cs = SkillGenerator.Instance.GenerateConsumablesSkill (consumables, si, consumablesSkillsContainer);
					cs.AffectAgents (battleAgentCtr, null);
				}
				RemoveItem (consumables);
			}

			Debug.LogFormat ("{0}使用了{1}", agentName, consumables.itemName);

			return ResetBattleAgentProperties (false);

		}



			
		public void LevelUpIfExperienceEnough(){

			if (experience >= upgradeExprience) {
		
				agentLevel++;

				// 全属性+1，血量+10，魔法+5
				originalAttack += 1;
				originalAttackSpeed += 1;
				originalArmor += 1;
				originalMagicResist += 1;
				originalDodge += 1;
				originalCrit += 1;
				originalMaxHealth += 10;
				originalMana += 5;

//				skillPointsLeft++;

				ResetBattleAgentProperties (true);//升级后更新玩家状态，玩家血量和魔法值回满
			}

		}

		/// <summary>
		/// 检查物品是否已经被玩家解锁
		/// </summary>
		/// <returns><c>true</c>, if item unlocked was checked, <c>false</c> otherwise.</returns>
		/// <param name="item">Item.</param>
		public bool CheckItemUnlocked(int itemId){

			for (int i = 0; i < allFormulasInBag.Count; i++) {
				Formula formula = allFormulasInBag [i];
				if (formula.unlocked && formula.unlockedItemId == itemId) {
					return true;
				}
			}

			return false;

		}


		/// <summary>
		/// 用户获得字母碎片
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="count">Count.</param>
		public void AddCharacterFragment(char character,int count){

			int characterIndex = (int)(character) - CommonData.aInASCII;

			charactersCount [characterIndex] += count;

		}

		/// <summary>
		/// 用户损失字母碎片
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="count">Count.</param>
		public void RemoveCharacterFragment(char character,int count){
			
			int characterIndex = (int)(character) - CommonData.aInASCII;

			int characterCount = charactersCount [characterIndex];

			if (characterCount < count) {
				charactersCount [characterIndex] = 0;
			} else {
				charactersCount [characterIndex] -= count;
			}
		}

		/// <summary>
		/// 添加物品到背包中
		/// </summary>
		/// <param name="item">Item.</param>
		public void AddItem(Item item){

			if (item == null) {
				string error = "添加的物品为null";
				Debug.LogError (error);
				return;
			}

			switch(item.itemType){
			case ItemType.Equipment:
				for (int i = 0; i < item.itemCount; i++) {
					Equipment equipment = Item.NewItemWith (item, 1) as Equipment;
					allEquipmentsInBag.Add (equipment);
					allItemsInBag.Add (equipment);
				}
				break;
			// 如果是消耗品，且背包中已经存在该消耗品，则只合并数量
			case ItemType.Consumables:
				Consumables consumablesInBag = allConsumablesInBag.Find (delegate(Consumables obj) {
					return obj.itemId == item.itemId;	
				});
				if (consumablesInBag != null) {
					consumablesInBag.itemCount += item.itemCount;
				} else {
					consumablesInBag = new Consumables (item as Consumables, item.itemCount);
					allConsumablesInBag.Add (item as Consumables);
					allItemsInBag.Add (item);
				}
				break;
//			case ItemType.FuseStone:
//				allFuseStonesInBag.Add (item as FuseStone);
//				allItemsInBag.Add (item);
//				break;
//			case ItemType.Task:
//				allTaskItemsInBag.Add (item as TaskItem);
//				allItemsInBag.Add (item);
//				break;
			case ItemType.Formula:
				allFormulasInBag.Add (item as Formula);
				break;
			case ItemType.CharacterFragment:
				CharacterFragment characterFragment = item as CharacterFragment;
				int characterIndex = (int)(characterFragment.character) - CommonData.aInASCII;
				charactersCount [characterIndex]++;
				break;
			}
				
		}

		public void RemoveItem(Item item){
			switch(item.itemType){
			case ItemType.Equipment:
				Equipment equipment = allEquipmentsInBag.Find(delegate(Equipment obj) {
					return obj.itemId == item.itemId;
				});
	
				if (equipment.equiped) {
					for (int i = 0; i < allEquipedEquipments.Length; i++) {
						if (allEquipedEquipments [i] == equipment) {
							allEquipedEquipments [i] = new Equipment();
						}
					}
				}

				allEquipmentsInBag.Remove (equipment);
				allItemsInBag.Remove (equipment);
//				TransformManager.FindTransform ("BagCanvas").GetComponent<BagViewController> ().RemoveItem (item);
				break;
				// 如果是消耗品，且背包中已经存在该消耗品，则只合并数量
			case ItemType.Consumables:
				Consumables consumablesInBag = allConsumablesInBag.Find (delegate(Consumables obj) {
					return obj.itemId == item.itemId;	
				});
				consumablesInBag.itemCount -= item.itemCount;
				if (consumablesInBag.itemCount <= 0) {
					allConsumablesInBag.Remove (consumablesInBag);
					allItemsInBag.Remove (consumablesInBag);
//					TransformManager.FindTransform ("BagCanvas").GetComponent<BagViewController> ().RemoveItem (item);
				}
				break;
//			case ItemType.FuseStone:
//				allFuseStonesInBag.Remove (item as FuseStone);
//				allItemsInBag.Remove (item);
////				TransformManager.FindTransform ("BagCanvas").GetComponent<BagViewController> ().RemoveItem (item);
//				break;
//			case ItemType.Task:
//				allTaskItemsInBag.Remove (item as TaskItem);
//				allItemsInBag.Remove (item);
////				TransformManager.FindTransform ("BagCanvas").GetComponent<BagViewController> ().RemoveItem (item);
//				break;
			case ItemType.CharacterFragment:
				CharacterFragment characterFragment = item as CharacterFragment;
				int characterIndex = (int)(characterFragment.character) - CommonData.aInASCII;
				if (charactersCount [characterIndex] > 0) {
					charactersCount [characterIndex]--;
				}
				break;
			}
//			return removeFromBag;
		}

		/// <summary>
		/// Adds the material.
		/// </summary>
		/// <param name="material">Material.</param>
//		private void AddMaterial(Material material){
//
//			Material materialInBag = allMaterialsInBag.Find(delegate(Material obj){
//				return obj.itemId == material.itemId;
//			});
//
//			if (materialInBag != null) {
//				// 如果玩家背包中存在对应材料 ＋＝ 材料数量
//				materialInBag.itemCount += material.itemCount;		
//			}else{
//				// 如果玩家背包中不存在对应材料，则背包中添加该材料
//				Player.mainPlayer.allMaterialsInBag.Add(material);
//			} 
//		}

//		public Material GetMaterialInBagWithId(int materialId){
//			return allMaterialsInBag.Find(delegate(Material obj) {
//				return obj.itemId == materialId;
//			});
//		}

//		private void RemoveMaterial(Material material){
//
//			Material materialInBag = allMaterialsInBag.Find (delegate(Material obj) {
//				return obj.itemId == material.itemId;
//			});
//
//			materialInBag.itemCount -= material.itemCount;
//
//			if (materialInBag.itemCount <= 0) {
//				allMaterialsInBag.Remove (materialInBag);
//			}
//				
//		}






		/// <summary>
		/// Checks the unsufficient characters.
		/// </summary>
		/// <returns>The unsufficient characters.</returns>
		/// <param name="itemNameInEnglish">Item name in english.</param>
		public List<char> CheckUnsufficientCharacters(string itemNameInEnglish){

			char[] charactersArray = itemNameInEnglish.ToCharArray ();

			int[] charactersNeed = new int[26];

			List<char> unsufficientCharacters = new List<char> ();

			foreach (char c in charactersArray) {
				int index = (int)c - CommonData.aInASCII;
				charactersNeed [index]++;
			}

			// 判断玩家字母碎片是否足够
			for(int i = 0;i<charactersNeed.Length;i++){

				if (charactersNeed [i] > Player.mainPlayer.charactersCount[i]) {

					char c = (char)(i + CommonData.aInASCII);

					unsufficientCharacters.Add (c);

				}

			}

			return unsufficientCharacters;

		}


			


		/// <summary>
		/// 玩家死亡时随机丢失一种 装备／消耗品／材料
		/// </summary>
		/// <returns>The item when die.</returns>
//		public Item LostItemWhenDie(){
//
//			// 判断玩家背包中是否有字母碎片
//			bool noCharacterFragmentInBag = true;
//
//			for (int i = 0; i < charactersCount.Length; i++) {
//				if (charactersCount [i] != 0) {
//					noCharacterFragmentInBag = false;
//				}
//			}
//
//			// 如果玩家背包中没有可丢失的物品，返回null
////			if (allEquipmentsInBag.Count == 0 && allConsumablesInBag.Count == 0 && allMaterialsInBag.Count == 0 && noCharacterFragmentInBag) {
////				return null;
////			}
//
//			// 随机一种丢失物品类型（0:装备，1:消耗品，2:材料，3:字母碎片）
//			ItemType lostItemType = (ItemType)Random.Range (0, 3);
//
//			Item lostItem = null;
//
//			int lostCount = 0;
//
//			switch (lostItemType) {
//			case ItemType.Equipment:
//				
//				if (allEquipmentsInBag.Count == 0) {
//					return LostItemWhenDie ();
//				}
//
//				int lostEquipmentIndex = Random.Range (0, allEquipmentsInBag.Count);
//
//				lostItem = allEquipmentsInBag [lostEquipmentIndex];
//
//				RemoveItem (lostItem);
//
//				return lostItem;
//
//			case ItemType.Consumables:
//				
//				if (allConsumablesInBag.Count == 0) {
//					return LostItemWhenDie ();
//				}
//
//				int lostConsumbablesIndex = Random.Range (0, allConsumablesInBag.Count);
//
//				// 获得背包中丢失的物品
//				Consumables lostConsumables = allConsumablesInBag [lostConsumbablesIndex];
//
//				// 获得丢失数量（20%概率返回最大值*0.2，70%概率返回最大值* 0.5，10%概率返回最大值；最小返回1，最大返回5）
//				lostCount = MyRandom (lostConsumables.itemCount);
//
//				// 获取物品实际数量
//				lostConsumables.itemCount -= lostCount;
//
//				// 如果物品数量==0，从背包中移除
//				if (lostConsumables.itemCount == 0) {
//					allConsumablesInBag.Remove (lostConsumables);
//				}
//
//				lostItem = new Consumables (lostConsumables, lostCount);
//
//				return lostItem;
//
//			case ItemType.CharacterFragment:
//				
//				if (noCharacterFragmentInBag) {
//					return LostItemWhenDie ();
//				}
//
//				List<int> notEmptyCharacterIndexs = new List<int> ();
//
//				// 将背包中字母碎片数量不为0的字母单独拉出来一个表
//				for (int i = 0; i < charactersCount.Length; i++) {
//					if (charactersCount [i] > 0) {
//						notEmptyCharacterIndexs.Add (i);
//					}
//				}
//				// 随机获取一个数量不为0的字母碎片序号（在新拉出来的表中）
//				int randomIndex = Random.Range (0, notEmptyCharacterIndexs.Count);
//
//				// 获取对应的真实字母
//				char character = (char)(notEmptyCharacterIndexs [randomIndex] + CommonData.aInASCII);
//
//				// 获取对应的字母碎片数量
//				int characterCount = charactersCount [notEmptyCharacterIndexs [randomIndex]];
//
//				lostCount = MyRandom (characterCount);
//
//				charactersCount [notEmptyCharacterIndexs [randomIndex]] -= lostCount;
//
//				lostItem = new CharacterFragment (character, characterCount) as Item;
//
//				// 返回字母碎片
//				return lostItem;
//			}
//
//			return null;
//		}


		/// <summary>
		/// 随机返回数量，20%概率返回最大值*0.2，70%概率返回最大值* 0.5，10%概率返回最大值
		/// 最小返回1，最大返回5
		/// </summary>
		/// <returns>The random.</returns>
		/// <param name="max">Max.</param>
		private int MyRandom(int max){

			int seed = 0;

			int returnNum = 0;

			seed = Random.Range (1, 10);

			if (seed <= 2) {
				returnNum = (int)(max * 0.2f);
			} else if (seed <= 9) {
				returnNum = (int)(max * 0.5f);
			}else{
				returnNum = max;
			}

			if (returnNum < 1) {
				returnNum = 1;
			}

			if (returnNum > 5) {
				returnNum = 5;
			}

			return returnNum;

		}

	}



	[System.Serializable]
	public class PlayerData{

		public string agentName;

		public string agentIconName;

		public bool isActive = true;

		public int agentLevel;

		//*****初始信息********//
		public int originalMaxHealth;
		public int originalMaxMana;
		public int originalHealth;
		public int originalMana;
		public int originalAttack;
		public int originalAttackSpeed;
		public int originalArmor;
		public int originalManaResist;
		public int originalDodge;
		public int originalCrit;
		//*****初始信息********//



//		public int attack;//攻击力
//		public int attackSpeed;//攻速
//		public int armor;//护甲
//		public int manaResist;//魔抗
//		public int dodge;//敏捷
//		public int crit;//暴击
//		public int maxHealth;//最大生命值
//		public int maxMana;//最大魔法值
//		public int health;//生命
//		public int mana;//魔法

		public int[] charactersCount = new int[26];//剩余的字母碎片信息

		public List<Material> allMaterialsInBag;//背包中所有材料信息
		public List<Equipment> allEquipmentsInBag;//背包中所有装备信息
		public Equipment[] allEquipedEquipments;//已装备的所有装备信息
		public List<Consumables> allConsumablesInBag;//背包中所有消耗品信息
		public List<FuseStone> allFuseStonesInBag;//背包中所有融合石信息
		public List<TaskItem> allTaskItemsInBag;//背包中所有任务物品信息
//		public List<CharacterFragment> allCharacterFragmentsInBag;//背包中所有的字母碎片
		public List<Formula> allFormulasInBag = new List<Formula>();//所有背包中的配方

		public int maxUnlockLevelIndex;//最大解锁关卡序号
		public int currentLevelIndex;//当前所在关卡序号

//		public List<SkillInfo> allEquipedActiveSkillInfo = new List<SkillInfo> ();//所有已装备的主动技能信息
//		public List<SkillInfo> allLearnedSkillInfo = new List<SkillInfo>();//所有已学习的技能信息

//		public int skillPointsLeft;//剩余可用技能点

		public PlayerData(Player player){

			this.agentName = player.agentName;
			this.agentIconName = player.agentIconName;
			this.agentLevel = player.agentLevel;
			this.isActive = player.isActive;

			this.originalMaxHealth = player.originalMaxHealth;
			this.originalMaxMana = player.originalMana;
			this.originalAttack = player.originalAttack;
			this.originalAttackSpeed = player.originalAttackSpeed;
			this.originalArmor = player.originalArmor;
			this.originalManaResist = player.originalMagicResist;
			this.originalCrit = player.originalCrit;
			this.originalDodge = player.originalDodge;
			this.originalHealth = player.originalHealth;
			this.originalMana = player.originalMana;

//			this.ori

//			this.attack = player.attack;//攻击力
//			this.attackSpeed = player.attackSpeed;//攻速
//			this.armor = player.armor;//护甲
//			this.manaResist = player.magicResist;//魔抗
//			this.dodge = player.dodge;//闪避
//			this.crit = player.crit;//暴击
//			this.maxHealth = player.maxHealth;//最大生命值
//			this.maxMana = player.mana;//最大魔法值
//			this.health = player.health;//生命
//			this.mana = player.mana;//魔法

			this.charactersCount = player.charactersCount;

			this.allEquipmentsInBag = player.allEquipmentsInBag;
			this.allEquipedEquipments = player.allEquipedEquipments;
			this.allConsumablesInBag = player.allConsumablesInBag;
//			this.allFuseStonesInBag = player.allFuseStonesInBag;
//			this.allTaskItemsInBag = player.allTaskItemsInBag;
			this.allFormulasInBag = player.allFormulasInBag;

			this.maxUnlockLevelIndex = player.maxUnlockLevelIndex;
			this.currentLevelIndex = player.currentLevelIndex;

		}

	}


}
