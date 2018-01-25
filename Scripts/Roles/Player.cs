using System.Collections;
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


		public List<Item> allItemsInBag = new List<Item>();//背包中要显示的所有物品（已穿戴的装备和已解锁的卷轴将会从这个表中删除）
		public List<Consumables> allConsumablesInBag = new List<Consumables> ();
//		public List<FuseStone> allFuseStonesInBag = new List<FuseStone>();
//		public List<TaskItem> allTaskItemsInBag = new List<TaskItem>();
		public List<UnlockScroll> allUnlockScrollsInBag = new List<UnlockScroll>();//所有背包中的解锁卷轴
		public List<CraftingRecipe> allCraftingRecipesInBag = new List<CraftingRecipe>();//所有背包中的合成配方

		public int maxUnlockLevelIndex;

		public int currentLevelIndex;

		public void SetUpPlayerWithPlayerData(PlayerData playerData){

			if (playerData == null) {
				return;
			}

			this.agentName = playerData.agentName;
//			this.agentIconName = playerData.agentIconName;
			this.agentLevel = playerData.agentLevel;
//			this.isActive = false;

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


			this.charactersCount = playerData.charactersCount;

			this.allEquipmentsInBag = playerData.allEquipmentsInBag;
			this.allEquipedEquipments = playerData.allEquipedEquipments;
			this.allConsumablesInBag = playerData.allConsumablesInBag;
			this.allItemsInBag = playerData.allItemsInBag;
			this.allUnlockScrollsInBag = playerData.allUnlockScrollsInBag;
			this.allCraftingRecipesInBag = playerData.allCraftRecipesInBag;


			this.maxUnlockLevelIndex = playerData.maxUnlockLevelIndex;
//			this.currentLevelIndex = playerData.currentLevelIndex;

			this.totalCoins = playerData.totalCoins;
			this.experience = playerData.experience;

			this.attachedTriggeredSkills.Clear ();
			this.attachedConsumablesSkills.Clear ();
			this.allStatus.Clear ();

			ResetBattleAgentProperties (true);


			allItemsInBag = new List<Item> ();

			for (int i = 0; i < allEquipmentsInBag.Count; i++) {
				if (!allEquipmentsInBag [i].equiped) {
					allItemsInBag.Add (allEquipmentsInBag [i]);
				}
			}

			for (int i = 0; i < allConsumablesInBag.Count; i++) {
				allItemsInBag.Add(allConsumablesInBag[i]);
			}

			for (int i = 0; i < allUnlockScrollsInBag.Count; i++) {
				if (!allUnlockScrollsInBag [i].unlocked) {
					allItemsInBag.Add (allUnlockScrollsInBag [i]);
				}
			}

			for (int i = 0; i < allCraftingRecipesInBag.Count; i++) {
				allItemsInBag.Add(allCraftingRecipesInBag[i]);
			}

			for(int i = 0;i<triggeredSkillsContainer.childCount;i++) {
				Destroy (triggeredSkillsContainer.GetChild (i).gameObject);
			}

			for (int i = 0; i < allEquipedEquipments.Length; i++) {

				Equipment equipment = allEquipedEquipments [i];

				if (equipment.itemId > 0) {

					for (int j = 0; j < equipment.attachedSkillInfos.Length; j++) {

						TriggeredSkill attachedSkill = SkillGenerator.Instance.GenerateTriggeredSkill (equipment, equipment.attachedSkillInfos [j], triggeredSkillsContainer);

						equipment.attachedSkills.Add (attachedSkill);

						Debug.LogFormat ("{0}-{1}", equipment.itemName, attachedSkill.name);

						attachedTriggeredSkills.Add (attachedSkill);
					}
				}

			}


		}



		/// <summary>
		/// 角色卸下装备
		/// </summary>
		/// <param name="equipment">Equipment.</param>
		/// <param name="equipmentIndexInPanel">Equipment index in panel.</param>
		public PropertyChange UnloadEquipment(Equipment equipment,int equipmentIndexInPanel,int indexInBag = -1){

			SoundManager.Instance.PlayAudioClip ("UI/sfx_UI_Equipment");

			equipment.equiped = false;

			Debug.LogFormat ("卸下装备{0}/{1}", equipmentIndexInPanel,allEquipedEquipments.Length);

			if (equipment.itemId < 0) {
				return new PropertyChange();
			}

			if (indexInBag == -1) {
				allItemsInBag.Add (equipment);
			} else {
				allItemsInBag.Insert (indexInBag, equipment);
			}

			for (int i = 0; i < equipment.attachedSkills.Count; i++) {
				TriggeredSkill attachedSkill = equipment.attachedSkills [i];
				attachedTriggeredSkills.Remove (attachedSkill);
				if (!(battleAgentCtr as BattlePlayerController).isInFight) {
					equipment.attachedSkills.Remove (attachedSkill);
					Destroy (attachedSkill.gameObject);
					i--;
				}
			}
				
			Equipment emptyEquipment = new Equipment ();

			allEquipedEquipments [equipmentIndexInPanel] = emptyEquipment;

			PropertyChange pc = ResetBattleAgentProperties (false);

			Transform exploreManager = TransformManager.FindTransform ("ExploreManager");
			if (exploreManager != null) {
				ExploreManager manager = exploreManager.GetComponent<ExploreManager> ();
				manager.UpdatePlayerPropertyCalculator ();
				manager.UpdateTriggeredCallBacks ();
				manager.UpdatePlayerStatusPlane ();
			}


			return pc;

		}


		public void DestroyEquipmentInBagAttachedSkills(){

			for (int j = 0; j < allEquipmentsInBag.Count; j++) {
				Equipment equipment = allEquipmentsInBag [j];
				if (!equipment.equiped && equipment.attachedSkills.Count > 0) {
					for (int i = 0; i < equipment.attachedSkills.Count; i++) {
						TriggeredSkill attachedSkill = equipment.attachedSkills [i];
						equipment.attachedSkills.RemoveAt (i);
						Destroy (attachedSkill.gameObject);
						i--;
					}
				}
			}

		}


		/// <summary>
		/// 角色穿上装备
		/// </summary>
		/// <param name="equipment">Equipment.</param>
		/// <param name="equipmentIndexInPanel">Equipment index in panel.</param>
		public PropertyChange EquipEquipment(Equipment equipment,int equipmentIndexInPanel){

			SoundManager.Instance.PlayAudioClip ("UI/sfx_UI_Equipment");

			equipment.equiped = true;

			Debug.LogFormat ("穿上装备{0}", equipmentIndexInPanel);

			allEquipedEquipments [equipmentIndexInPanel] = equipment;

//			equipmentDragControl.item = equipment;

			if (equipment.attachedSkills.Count == 0) {
				for (int i = 0; i < equipment.attachedSkillInfos.Length; i++) {
					TriggeredSkill attachedSkill = SkillGenerator.Instance.GenerateTriggeredSkill (equipment, equipment.attachedSkillInfos [i], triggeredSkillsContainer);
					equipment.attachedSkills.Add (attachedSkill);
					attachedTriggeredSkills.Add (attachedSkill);
					attachedSkill.transform.SetParent (triggeredSkillsContainer);
				}
			} else {
				for (int i = 0; i < equipment.attachedSkills.Count; i++) {
					TriggeredSkill attachedSkill = equipment.attachedSkills [i];
					attachedTriggeredSkills.Add (attachedSkill);
				}
			}

			allItemsInBag.Remove (equipment);

			PropertyChange pc = ResetBattleAgentProperties (false);

			Transform exploreManager = TransformManager.FindTransform ("ExploreManager");
			if (exploreManager != null) {
				ExploreManager manager = exploreManager.GetComponent<ExploreManager> ();
				manager.UpdatePlayerPropertyCalculator ();
				manager.UpdateTriggeredCallBacks ();
				manager.UpdatePlayerStatusPlane ();
			}

			return pc;

		}

		public Equipment GetEquipedEquipment(int itemId){
			Equipment equipedEquipment = null;
			for (int i = 0; i < allEquipedEquipments.Length; i++) {
				if (allEquipedEquipments [i].itemId == itemId) {
					equipedEquipment = allEquipedEquipments [i];
					return  equipedEquipment;
				}
			}
			return equipedEquipment;
		}

		public int GetItemIndexInBag(Item item){

			int index = -1;

			for (int i = 0; i < allItemsInBag.Count; i++) {
				if (allItemsInBag [i] == item) {
					index = i;
					return index;
				}
			}

			return index;
		}


		public Agent.PropertyChange UseMedicines(Consumables consumables){

			if (consumables.attachedSkillInfos.Length > 0) {
				for (int i = 0; i < consumables.attachedSkillInfos.Length; i++) {
					SkillInfo si = consumables.attachedSkillInfos [i];
					ConsumablesSkill cs = SkillGenerator.Instance.GenerateConsumablesSkill (consumables, si, consumablesSkillsContainer);
					cs.AffectAgents (battleAgentCtr, null);
				}
				RemoveItem (consumables,1);
			}

			Debug.LogFormat ("{0}使用了{1}", agentName, consumables.itemName);

			return ResetBattleAgentProperties (false);

		}

	
		public bool LevelUpIfExperienceEnough(){

			bool levelUp = false;

			if (experience >= upgradeExprience) {
		
				agentLevel++;

				// 全属性+1，血量+10
				originalAttack += 1;
				originalHit += 1;
				originalArmor += 1;
				originalMagicResist += 1;
				originalDodge += 1;
				originalCrit += 1;
				originalMaxHealth += 10;
				originalMana += 1;

				ResetBattleAgentProperties (false);//升级后更新玩家状态，玩家血量和魔法值回满

				levelUp = true;
			}
			return levelUp;
		}

		/// <summary>
		/// 检查物品是否已经被玩家解锁
		/// </summary>
		/// <returns><c>true</c>, if item unlocked was checked, <c>false</c> otherwise.</returns>
		/// <param name="item">Item.</param>
		public bool CheckItemUnlocked(int itemId){

			for (int i = 0; i < allUnlockScrollsInBag.Count; i++) {
				UnlockScroll unlockScroll = allUnlockScrollsInBag [i];
				if (unlockScroll.unlocked && unlockScroll.unlockedItemId == itemId) {
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
			case ItemType.UnlockScroll:
				UnlockScroll unlockScroll = item as UnlockScroll;
				allUnlockScrollsInBag.Add (unlockScroll);
				allItemsInBag.Add (unlockScroll);
				break;
			case ItemType.CraftingRecipes:
				allItemsInBag.Add (item);
				allCraftingRecipesInBag.Add (item as CraftingRecipe);
				break;
			case ItemType.CharacterFragment:
				CharacterFragment characterFragment = item as CharacterFragment;
				int characterIndex = (int)(characterFragment.character) - CommonData.aInASCII;
				charactersCount [characterIndex]++;
				break;
			}
				
		}
			

		public void RemoveItem(Item item,int removeCount){

			switch(item.itemType){
			case ItemType.Equipment:
				
				Equipment equipment = allEquipmentsInBag.Find(delegate(Equipment obj) {
					return obj == item;
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
					return obj == item;	
				});

				consumablesInBag.itemCount -= removeCount;

				if (consumablesInBag.itemCount <= 0) {
					allConsumablesInBag.Remove (consumablesInBag);
					allItemsInBag.Remove (consumablesInBag);
//					TransformManager.FindTransform ("BagCanvas").GetComponent<BagViewController> ().RemoveItem (item);
				}
				break;
			case ItemType.UnlockScroll:
				UnlockScroll unlockScroll = allUnlockScrollsInBag.Find (delegate(UnlockScroll obj) {
					return obj == item;	
				});
				if (!unlockScroll.unlocked) {
					allUnlockScrollsInBag.Remove (unlockScroll);
				} 
				allItemsInBag.Remove (unlockScroll);
				break;
			case ItemType.CraftingRecipes:
				allItemsInBag.Remove (item);
				allCraftingRecipesInBag.Remove (item as CraftingRecipe);
				break;
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
		/// 分解物品
		/// </summary>
		/// <returns>分解后获得的字母碎片</returns>
		public List<char> ResolveItemAndGetCharacters(Item item,int resolveCount){

			SoundManager.Instance.PlayAudioClip ("UI/sfx_UI_Resolve");

			// 分解后得到的字母碎片
			List<char> charactersReturn = new List<char> ();

			// 物品英文名称转换为char数组
			char[] charArray = item.itemNameInEnglish.ToCharArray ();

			if (charArray.Length == 0) {
				charArray = CommonData.wholeAlphabet;
			}

			// 每分解一个物品可以获得的字母碎片数量(解锁卷轴返回对应单词的所有字母，其余物品返回单词中的一个字母）
			int charactersReturnCount = item.itemType == ItemType.UnlockScroll ? charArray.Length : 1;

			// char数组转换为可以进行增减操作的list
			List<char> charList = new List<char> ();

			for (int i = 0; i < charArray.Length; i++) {
				charList.Add (charArray [i]);
			}

			// 分解物品，背包中的字母碎片数量增加
			for (int j = 0; j < resolveCount; j++) {

				for (int i = 0; i < charactersReturnCount; i++) {

					char character = ReturnRandomCharacters (ref charList);

					int characterIndex = (int)character - CommonData.aInASCII;

					charactersCount [characterIndex]++;

					charactersReturn.Add (character);
				}
			}

			// 被分解的物品减去分解数量，如果数量<=0,从背包中删除物品
			RemoveItem(item,resolveCount);

			return charactersReturn;

		}

		/// <summary>
		/// 从单词的字母组成中随机返回一个字母
		/// </summary>
		/// <returns>The random characters.</returns>
		private char ReturnRandomCharacters(ref List<char> charList){

			int charIndex = Random.Range (0, charList.Count);

			char character = charList [charIndex];

			charList.RemoveAt (charIndex);

			return character;

		}




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



		public int[] charactersCount = new int[26];//剩余的字母碎片信息

//		public List<Material> allMaterialsInBag;//背包中所有材料信息
		public List<Equipment> allEquipmentsInBag;//背包中所有装备信息
		public Equipment[] allEquipedEquipments;//已装备的所有装备信息
		public List<Consumables> allConsumablesInBag;//背包中所有消耗品信息
		public List<Item> allItemsInBag;
		public List<UnlockScroll> allUnlockScrollsInBag;
		public List<CraftingRecipe> allCraftRecipesInBag;
//		public List<FuseStone> allFuseStonesInBag;//背包中所有融合石信息
//		public List<TaskItem> allTaskItemsInBag;//背包中所有任务物品信息
//		public List<CharacterFragment> allCharacterFragmentsInBag;//背包中所有的字母碎片

		public int maxUnlockLevelIndex;//最大解锁关卡序号
//		public int currentLevelIndex;//当前所在关卡序号

		public int experience;//人物经验值
		public int totalCoins;//人物金币数量


		public PlayerData(Player player){

			this.agentName = player.agentName;
//			this.agentIconName = player.agentIconName;
			this.agentLevel = player.agentLevel;
//			this.isActive = player.isActive;

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


			this.charactersCount = player.charactersCount;

			this.allItemsInBag = player.allItemsInBag;
			this.allEquipmentsInBag = player.allEquipmentsInBag;
			this.allEquipedEquipments = player.allEquipedEquipments;
			this.allConsumablesInBag = player.allConsumablesInBag;
			this.allUnlockScrollsInBag = player.allUnlockScrollsInBag;
			this.allCraftRecipesInBag = player.allCraftingRecipesInBag;

			this.maxUnlockLevelIndex = player.maxUnlockLevelIndex;
//			this.currentLevelIndex = player.currentLevelIndex;

			this.totalCoins = player.totalCoins;
			this.experience = player.experience;

			ClearAllEquipmentAttachedSkills ();

		}

		private void ClearAllEquipmentAttachedSkills(){
			for (int i = 0; i < allEquipedEquipments.Length; i++) {
				allEquipedEquipments [i].attachedSkills.Clear ();
			}
		}

	}


}
