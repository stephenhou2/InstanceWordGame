using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace WordJourney
{
	public class Player : Agent {

		private static volatile Player mPlayerSingleton;

		private static object objectLock = new System.Object();

		// 玩家角色单例
		public static Player mainPlayer{
			get{
				if (mPlayerSingleton == null) {
					lock (objectLock) {
						Player[] existPlayers = GameObject.FindObjectsOfType<Player>();
						if (existPlayers != null) {
							for (int i = 0; i < existPlayers.Length; i++) {
								Destroy (existPlayers [i].gameObject);
							}
						}

						ResourceLoader playerLoader = ResourceLoader.CreateNewResourceLoader ();

						ResourceManager.Instance.LoadAssetsWithBundlePath<GameObject> (playerLoader, CommonData.mainStaticBundleName, () => {
							mPlayerSingleton = playerLoader.gos[0].GetComponent<Player> ();
							mPlayerSingleton.transform.SetParent (null);
							mPlayerSingleton.ResetBattleAgentProperties (true);
						},true,"Player");

						DontDestroyOnLoad (mPlayerSingleton);
					}
				} 
				return mPlayerSingleton;
			}

		}
			

		[SerializeField]private int[] mCharactersCount;
		public int[] charactersCount{

			get{
				if (mCharactersCount == null || mCharactersCount.Length == 0) {
					mCharactersCount = new int[26];
					#warning 这里测试用，暂时初始化每个字母初始有10个，后面去掉
					for(int i = 0;i<mCharactersCount.Length;i++){
						mCharactersCount[i] = 10;
					}
				}
				return mCharactersCount;
			}

			set{
				mCharactersCount = value;
			}

		}
			
		public int experience;//玩家经验值

		// 每次升级所需要的经验值
		public int upgradeExprience{
			get{
				return 50 * agentLevel * (agentLevel + 1);
			}
		}

		public List<ActiveSkill> equipedActiveSkills = new List<ActiveSkill>();

		public List<Skill> allLearnedSkills = new List<Skill>();//技能数组

		public float skillCoolenInterval;// 技能的冷却时间

		public int skillPointsLeft;

		public List<Material> allMaterialsInBag = new List<Material>();

		public List<Equipment> allEquipmentsInBag = new List<Equipment> ();
		public List<Consumables> allConsumablesInBag = new List<Consumables> ();
		public List<FuseStone> allFuseStonesInBag = new List<FuseStone>();
		public List<TaskItem> allTaskItemsInBag = new List<TaskItem>();
//		public List<CharacterFragment> allCharacterFragmentsInBag = new List<CharacterFragment> ();
		public List<Formula> allFormulasInBag = new List<Formula>();//所有背包中的配方


		public int maxUnlockLevelIndex;

		public int currentLevelIndex;

		public List<ConsumablesEffectState> allConsumablesEffectStates = new List<ConsumablesEffectState> ();


		public void SetUpPlayerWithPlayerData(PlayerData playerData){

			if (playerData == null) {
				return;
			}

			this.agentName = playerData.agentName;
			this.agentIconName = playerData.agentIconName;
			this.agentLevel = playerData.agentLevel;
			this.isActive = playerData.isActive;

			this.originalMaxHealth = playerData.originalMaxHealth;
			this.originalMaxMana = playerData.originalMaxMana;
			this.originalAttack = playerData.originalAttack;
			this.originalAttackSpeed = playerData.originalAttackSpeed;
			this.originalArmor = playerData.originalArmor;
			this.originalManaResist = playerData.originalManaResist;
			this.originalCrit = playerData.originalCrit;
			this.originalDodge = playerData.originalDodge;
			this.originalHealth = playerData.originalHealth;
			this.originalMana = playerData.originalMana;


			this.attack = playerData.attack;//攻击力
			this.attackSpeed = playerData.attackSpeed;//攻速
			this.armor = playerData.armor;//护甲
			this.manaResist = playerData.manaResist;//魔抗
			this.dodge = playerData.dodge;//闪避
			this.crit = playerData.crit;//暴击
			this.maxHealth = playerData.maxHealth;//最大生命值
			this.maxMana = playerData.maxMana;//最大魔法值
			this.health = playerData.health;//生命
			this.mana = playerData.mana;//魔法

			this.charactersCount = playerData.charactersCount;

			this.allMaterialsInBag = playerData.allMaterialsInBag;
			this.allEquipmentsInBag = playerData.allEquipmentsInBag;
			this.allConsumablesInBag = playerData.allConsumablesInBag;
			this.allFuseStonesInBag = playerData.allFuseStonesInBag;
			this.allTaskItemsInBag = playerData.allTaskItemsInBag;
//			this.allCharacterFragmentsInBag = playerData.allCharacterFragmentsInBag;
			this.allFormulasInBag = playerData.allFormulasInBag;

			this.maxUnlockLevelIndex = playerData.maxUnlockLevelIndex;
			this.currentLevelIndex = playerData.currentLevelIndex;


			for (int i = 0; i < playerData.allLearnedSkillInfo.Count; i++) {

				SkillInfo skillInfo = playerData.allLearnedSkillInfo [i];

				Skill skill = Skill.LoadSkillFromWithSkillInfo (skillInfo);

				allLearnedSkills.Add (skill);

				if (skill is ActiveSkill) {
					equipedActiveSkills.Add (skill as ActiveSkill);
				}

			}

			this.skillPointsLeft = playerData.skillPointsLeft;

		}


		public void EquipEquipment(Equipment equipment){

			Equipment equipedEquipmentOfSelectType = allEquipedEquipments.Find (delegate(Equipment obj) {
				return obj.equipmentType == equipment.equipmentType;
			});

			if (equipedEquipmentOfSelectType != null) {

				equipedEquipmentOfSelectType.equiped = false;
				allEquipedEquipments.Remove (equipedEquipmentOfSelectType);

			}

			equipment.equiped = true;

			allEquipedEquipments.Add(equipment);

			ResetBattleAgentProperties (false);

			ResetPropertiesByConsumablesEffectState ();

		}

		/// <summary>
		/// 根据玩家身上有的消耗品对应的状态重置玩家属性(仅在玩家更换装备后使用该方法）
		/// </summary>
		private void ResetPropertiesByConsumablesEffectState(){

			for (int i = 0; i < allConsumablesEffectStates.Count; i++) {

				Consumables consumables = allConsumablesEffectStates [i].consumables;

				attack = (int)(attack * (1 + consumables.attackGain));
				attackSpeed = (int)(attackSpeed * (1 + consumables.attackSpeedGain));
				armor = (int)(armor * (1 + consumables.armorGain));
				manaResist = (int)(manaResist * (1 + consumables.manaResistGain));
				dodge = (int)(dodge * (1 + consumables.dodgeGain));
				crit = (int)(crit * (1 + consumables.critGain));

				physicalHurtScaler = 1 + consumables.physicalHurtScaler;
				magicalHurtScaler = 1 + consumables.magicHurtScaler;

			}

		}
			
		public void LevelUpIfExperienceEnough(){

			if (experience >= upgradeExprience) {
		
				agentLevel++;

				// 全属性+1，血量+10，魔法+5
				originalAttack += 1;
				originalAttackSpeed += 1;
				originalArmor += 1;
				originalManaResist += 1;
				originalDodge += 1;
				originalCrit += 1;
				originalMaxHealth += 10;
				originalMaxMana += 5;

				skillPointsLeft++;

				ResetBattleAgentProperties (true);//升级后更新玩家状态，玩家血量和魔法值回满
			}

		}

		public void UpdateValidActionType(){

			// 如果技能还在冷却中或者玩家气力值小于技能消耗的气力值，则相应按钮不可用
			for (int i = 0;i < equipedActiveSkills.Count;i++) {

				ActiveSkill s = equipedActiveSkills [i];
				// 如果是冷却中的技能
				if (mana > s.manaConsume) {
					s.isAvalible = true;
				}
			}
		}



		// 获取玩家已学习的技能
		public Skill GetPlayerLearnedSkill(int skillId){
			Skill s = null;
			s = allLearnedSkills.Find (delegate(Skill obj) {
				return obj.skillId == skillId;	
			});
			return s;
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

			switch(item.itemType){
			case ItemType.Equipment:
				allEquipmentsInBag.Add (item as Equipment);
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
				}
				break;
			case ItemType.FuseStone:
				allFuseStonesInBag.Add (item as FuseStone);
				break;
			case ItemType.Task:
				allTaskItemsInBag.Add (item as TaskItem);
				break;
			case ItemType.Material:
				AddMaterial (item as Material);
				break;
			case ItemType.Formula:

				Formula formula = item as Formula;

				// 如果背包中已经有这种配方，则不添加到背包中
				for (int i = 0; i < allFormulasInBag.Count; i++) {
					Formula formulaInBag = allFormulasInBag [i];
					if (formulaInBag.formulaType == FormulaType.Equipment && formulaInBag.itemOrSkillId == formula.itemOrSkillId) {
						return;
					}
				}

				formula.GetItemModelUnlock ();

				allFormulasInBag.Add (formula);

				break;
			}
				
		}

		public void RemoveItem(Item item){
			switch(item.itemType){
			case ItemType.Equipment:
				Equipment equipment = item as Equipment;
				allEquipmentsInBag.Remove (equipment);
				if (equipment.equiped) {
					allEquipedEquipments.Remove (equipment);
				}
				break;
				// 如果是消耗品，且背包中已经存在该消耗品，则只合并数量
			case ItemType.Consumables:
				Consumables consumablesInBag = allConsumablesInBag.Find (delegate(Consumables obj) {
					return obj.itemId == item.itemId;	
				});
				consumablesInBag.itemCount -= item.itemCount;
				if (consumablesInBag.itemCount <= 0) {
					allConsumablesInBag.Remove (consumablesInBag);
				}
				break;
			case ItemType.FuseStone:
				allFuseStonesInBag.Remove (item as FuseStone);
				break;
			case ItemType.Task:
				allTaskItemsInBag.Remove (item as TaskItem);
				break;
			case ItemType.Material:
				RemoveMaterial (item as Material);
				break;
			}
		}

		/// <summary>
		/// Adds the material.
		/// </summary>
		/// <param name="material">Material.</param>
		private void AddMaterial(Material material){

			Material materialInBag = allMaterialsInBag.Find(delegate(Material obj){
				return obj.itemId == material.itemId;
			});

			if (materialInBag != null) {
				// 如果玩家背包中存在对应材料 ＋＝ 材料数量
				materialInBag.itemCount += material.itemCount;		
			}else{
				// 如果玩家背包中不存在对应材料，则背包中添加该材料
				Player.mainPlayer.allMaterialsInBag.Add(material);
			} 
		}

		public Material GetMaterialInBagWithId(int materialId){
			return allMaterialsInBag.Find(delegate(Material obj) {
				return obj.itemId == materialId;
			});
		}

		private void RemoveMaterial(Material material){

			Material materialInBag = allMaterialsInBag.Find (delegate(Material obj) {
				return obj.itemId == material.itemId;
			});

			materialInBag.itemCount -= material.itemCount;

			if (materialInBag.itemCount <= 0) {
				allMaterialsInBag.Remove (materialInBag);
			}
				
		}


		/// <summary>
		/// 分解材料
		/// </summary>
		/// <returns>分解后获得的字母碎片</returns>
		public List<char> GetCharactersFromItem(Item item,int resolveCount){

			// 分解后得到的字母碎片
			List<char> charactersReturn = new List<char> ();

			// 每分解一个物品可以获得的字母碎片数量
			int charactersReturnCount = 1;

			// 物品英文名称转换为char数组
			char[] charArray = item.itemNameInEnglish.ToCharArray ();

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

					charactersCount [i]++;

//					CharacterFragment cf = allCharacterFragmentsInBag.Find (delegate(CharacterFragment obj) {
//						return obj.itemName == character.ToString ();
//					});
//
//					if (cf == null) {
//						
//						cf = new CharacterFragment (character, 0);
//
//						allCharacterFragmentsInBag.Add (cf);
//					}
//
//					cf.itemCount++;

					charactersReturn.Add (character);
				}
			}

			// 被分解的物品减去分解数量，如果数量<=0,从背包中删除物品
			RemoveItem(item);
		

			return charactersReturn;

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
		/// 从单词的字母组成中随机返回一个字母
		/// </summary>
		/// <returns>The random characters.</returns>
		private char ReturnRandomCharacters(ref List<char> charList){

			int charIndex = (int)Random.Range (0, charList.Count - float.Epsilon);

			char character = charList [charIndex];

			charList.RemoveAt (charIndex);

			return character;

		}
			


		/// <summary>
		/// 玩家死亡时随机丢失一种 装备／消耗品／材料
		/// </summary>
		/// <returns>The item when die.</returns>
		public Item LostItemWhenDie(){

			// 判断玩家背包中是否有字母碎片
			bool noCharacterFragmentInBag = true;

			for (int i = 0; i < charactersCount.Length; i++) {
				if (charactersCount [i] != 0) {
					noCharacterFragmentInBag = false;
				}
			}

			// 如果玩家背包中没有可丢失的物品，返回null
			if (allEquipmentsInBag.Count == 0 && allConsumablesInBag.Count == 0 && allMaterialsInBag.Count == 0 && noCharacterFragmentInBag) {
				return null;
			}

			// 随机一种丢失物品类型（0:装备，1:消耗品，2:材料，3:字母碎片）
			ItemType lostItemType = (ItemType)Random.Range (0, 3);

			Item lostItem = null;

			int lostCount = 0;

			switch (lostItemType) {
			case ItemType.Equipment:
				
				if (allEquipmentsInBag.Count == 0) {
					return LostItemWhenDie ();
				}

				int lostEquipmentIndex = Random.Range (0, allEquipmentsInBag.Count);

				lostItem = allEquipmentsInBag [lostEquipmentIndex];

				RemoveItem (lostItem);

				return lostItem;

			case ItemType.Consumables:
				
				if (allConsumablesInBag.Count == 0) {
					return LostItemWhenDie ();
				}

				int lostConsumbablesIndex = Random.Range (0, allConsumablesInBag.Count);

				// 获得背包中丢失的物品
				Consumables lostConsumables = allConsumablesInBag [lostConsumbablesIndex];

				// 获得丢失数量（20%概率返回最大值*0.2，70%概率返回最大值* 0.5，10%概率返回最大值；最小返回1，最大返回5）
				lostCount = MyRandom (lostConsumables.itemCount);

				// 获取物品实际数量
				lostConsumables.itemCount -= lostCount;

				// 如果物品数量==0，从背包中移除
				if (lostConsumables.itemCount == 0) {
					allConsumablesInBag.Remove (lostConsumables);
				}

				lostItem = new Consumables (lostConsumables, lostCount);

				return lostItem;

			case ItemType.Material:
				
				if (allMaterialsInBag.Count == 0) {
					return LostItemWhenDie ();
				}

				int lostMaterialIndex = Random.Range (0, allMaterialsInBag.Count);

				Material lostMaterial = allMaterialsInBag [lostMaterialIndex];

				lostCount = MyRandom (lostMaterial.itemCount);

				lostMaterial.itemCount -= lostCount;

				if (lostMaterial.itemCount == 0) {
					allMaterialsInBag.Remove (lostMaterial);
				}

				lostItem = new Material (lostMaterial, lostCount);

				return lostItem;

			case ItemType.CharacterFragment:
				
				if (noCharacterFragmentInBag) {
					return LostItemWhenDie ();
				}

				List<int> notEmptyCharacterIndexs = new List<int> ();

				// 将背包中字母碎片数量不为0的字母单独拉出来一个表
				for (int i = 0; i < charactersCount.Length; i++) {
					if (charactersCount [i] > 0) {
						notEmptyCharacterIndexs.Add (i);
					}
				}
				// 随机获取一个数量不为0的字母碎片序号（在新拉出来的表中）
				int randomIndex = Random.Range (0, notEmptyCharacterIndexs.Count);

				// 获取对应的真实字母
				char character = (char)(notEmptyCharacterIndexs [randomIndex] + CommonData.aInASCII);

				// 获取对应的字母碎片数量
				int characterCount = charactersCount [notEmptyCharacterIndexs [randomIndex]];

				lostCount = MyRandom (characterCount);

				charactersCount [notEmptyCharacterIndexs [randomIndex]] -= lostCount;

				lostItem = new CharacterFragment (character, characterCount) as Item;

				// 返回字母碎片
				return lostItem;
			}

			return null;
		}


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



		public int attack;//攻击力
		public int attackSpeed;//攻速
		public int armor;//护甲
		public int manaResist;//魔抗
		public int dodge;//敏捷
		public int crit;//暴击
		public int maxHealth;//最大生命值
		public int maxMana;//最大魔法值
		public int health;//生命
		public int mana;//魔法

		public int[] charactersCount;//剩余的字母碎片信息

		public List<Material> allMaterialsInBag;//背包中所有材料信息
		public List<Equipment> allEquipmentsInBag;//背包中所有装备信息
		public List<Consumables> allConsumablesInBag;//背包中所有消耗品信息
		public List<FuseStone> allFuseStonesInBag;//背包中所有融合石信息
		public List<TaskItem> allTaskItemsInBag;//背包中所有任务物品信息
//		public List<CharacterFragment> allCharacterFragmentsInBag;//背包中所有的字母碎片
		public List<Formula> allFormulasInBag = new List<Formula>();//所有背包中的配方

		public int maxUnlockLevelIndex;//最大解锁关卡序号
		public int currentLevelIndex;//当前所在关卡序号

//		public List<SkillInfo> allEquipedActiveSkillInfo = new List<SkillInfo> ();//所有已装备的主动技能信息
		public List<SkillInfo> allLearnedSkillInfo = new List<SkillInfo>();//所有已学习的技能信息

		public int skillPointsLeft;//剩余可用技能点

		public PlayerData(Player player){

			this.agentName = player.agentName;
			this.agentIconName = player.agentIconName;
			this.agentLevel = player.agentLevel;
			this.isActive = player.isActive;

			this.originalMaxHealth = player.originalMaxHealth;
			this.originalMaxMana = player.originalMaxMana;
			this.originalAttack = player.originalAttack;
			this.originalAttackSpeed = player.originalAttackSpeed;
			this.originalArmor = player.originalArmor;
			this.originalManaResist = player.originalManaResist;
			this.originalCrit = player.originalCrit;
			this.originalDodge = player.originalDodge;
			this.originalHealth = player.originalHealth;
			this.originalMana = player.originalMana;


			this.attack = player.attack;//攻击力
			this.attackSpeed = player.attackSpeed;//攻速
			this.armor = player.armor;//护甲
			this.manaResist = player.manaResist;//魔抗
			this.dodge = player.dodge;//闪避
			this.crit = player.crit;//暴击
			this.maxHealth = player.maxHealth;//最大生命值
			this.maxMana = player.maxMana;//最大魔法值
			this.health = player.health;//生命
			this.mana = player.mana;//魔法



			this.charactersCount = player.charactersCount;

			this.allMaterialsInBag = player.allMaterialsInBag;
			this.allEquipmentsInBag = player.allEquipmentsInBag;
			this.allConsumablesInBag = player.allConsumablesInBag;
			this.allFuseStonesInBag = player.allFuseStonesInBag;
			this.allTaskItemsInBag = player.allTaskItemsInBag;
//			this.allCharacterFragmentsInBag = player.allCharacterFragmentsInBag;
			this.allFormulasInBag = player.allFormulasInBag;

			this.maxUnlockLevelIndex = player.maxUnlockLevelIndex;
			this.currentLevelIndex = player.currentLevelIndex;

			for (int i = 0; i < player.allLearnedSkills.Count; i++) {

				SkillInfo skillInfo = new SkillInfo (player.allLearnedSkills [i]);

				allLearnedSkillInfo.Add (skillInfo);

			}

			this.skillPointsLeft = player.skillPointsLeft;

		}

	}


}
