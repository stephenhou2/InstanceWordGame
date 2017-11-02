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
			

		public List<Skill> allLearnedSkills = new List<Skill>();

		public int skillPointsLeft;

		public List<Material> allMaterialsInBag = new List<Material>();

		public List<Equipment> allEquipmentsInBag = new List<Equipment> ();
		public List<Consumables> allConsumablesInBag = new List<Consumables> ();
		public List<FuseStone> allFuseStonesInBag = new List<FuseStone>();
		public List<TaskItem> allTaskItemsInBag = new List<TaskItem>();



		public override void Awake(){

			base.Awake ();

			InitPlayerFromLocalData ();
		}




		private void InitPlayerFromLocalData(){

			string playerDataPath = string.Format ("{0}/{1}", CommonData.persistDataPath, "PlayerData.json");

			PlayerData playerData = DataHandler.LoadDataToSingleModelWithPath<PlayerData> (playerDataPath);

			if (playerData != null) {
				LoadDataFromModel (playerData);
			}

		}

		private void LoadDataFromModel(PlayerData playerData){

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

			GameManager.Instance.maxUnlockChapterIndex = playerData.maxUnlockChapterIndex;


			for (int i = 0; i < playerData.allLearnedSkillInfo.Count; i++) {

				SkillInfo skillInfo = playerData.allLearnedSkillInfo [i];

				Skill skill = Skill.LoadSkillFromWithSkillInfo (skillInfo);

				allLearnedSkills.Add (skill);

			}

			this.skillPointsLeft = playerData.skillPointsLeft;

		}

		public void PlayerLevelUp(){

			agentLevel++;

			originalAttack += 1;
			originalAttackSpeed += 1;
			originalArmor += 1;
			originalManaResist += 1;
			originalDodge += 1;
			originalCrit += 1;
			originalMaxHealth += 10;
			originalMaxMana += 5;

			ResetBattleAgentProperties (true);

		}

		public void UpdateValidActionType(){

			// 如果技能还在冷却中或者玩家气力值小于技能消耗的气力值，则相应按钮不可用
			for (int i = 0;i < equipedSkills.Count;i++) {

				Skill s = equipedSkills [i];
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

		public void AddItem(Item item){

			switch(item.itemType){
			case ItemType.Equipment:
				allEquipmentsInBag.Add (item as Equipment);
				break;
			// 如果是消耗品，且背包中已经存在该消耗品，则只合并数量
			case ItemType.Consumables:
				Consumables comsumablesInBag = allConsumablesInBag.Find (delegate(Consumables obj) {
					return obj.itemId == item.itemId;	
				});
				if (comsumablesInBag != null) {
					comsumablesInBag.itemCount += item.itemCount;
				} 
				allConsumablesInBag.Add (item as Consumables);
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
			}
				
		}

		/// <summary>
		/// 分解材料
		/// </summary>
		/// <returns>分解后获得的字母碎片</returns>
		public List<char> GetCharactersFromItem(Item item,int resolveCount){

			//分解后得到的字母碎片
			List<char> charactersReturn = new List<char> ();

			int charactersReturnCount = 1;

			char[] charArray = item.itemNameInEnglish.ToCharArray ();

			List<char> charList = new List<char> ();

			for (int i = 0; i < charArray.Length; i++) {
				charList.Add (charArray [i]);
			}

			for (int j = 0; j < resolveCount; j++) {

				for (int i = 0; i < charactersReturnCount; i++) {

					char character = ReturnRandomCharacters (ref charList);

					int charIndex = (int)character - CommonData.aInASCII;

					charactersCount [charIndex]++;

					charactersReturn.Add (character);
				}
			}

			item.itemCount -= resolveCount;

			if (item.itemCount <= 0) {
				switch (item.itemType) {
				case ItemType.Material:
					allMaterialsInBag.Remove (item as Material);
					break;
				case ItemType.FuseStone:
					allFuseStonesInBag.Remove (item as FuseStone);
					break;
				}

			}

			return charactersReturn;

		}

//		public void ArrangeItems(List<Item> items){
//
//			for (int i = 0; i < items.Count; i++) {
//
//				Item item = allItems [i];
//
//				if (item.itemType == ItemType.Consumables) {
//
//					for (int j = i + 1; j < allItems.Count; j++) {
//
//						Item itemBackwards = allItems [j];
//
//						if (item.itemId == itemBackwards.itemId) {
//
//							item.itemCount += itemBackwards.itemCount;
//
//							allItems.Remove (itemBackwards);
//
//							j--;
//
//						}
//					}
//				}
//			}
//		}


		public void AddMaterial(Material material,int materialCount){
			material.itemCount = materialCount;
			AddMaterial (material);
		}

		public void RemoveMaterials(List<Material> materials){
			
			for(int i = 0;i<materials.Count;i++){

				Material material = materials [i];

				Material materialInBag = allMaterialsInBag.Find (delegate(Material obj) {
					return obj.itemId == material.itemId;
				});

				materialInBag.itemCount -= material.itemCount;

				if (materialInBag.itemCount <= 0) {
					allMaterialsInBag.Remove (materialInBag);
				}

			}

		}

		/// <summary>
		/// Adds the material.
		/// </summary>
		/// <param name="material">Material.</param>
		public void AddMaterial(Material material){

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

		public int maxUnlockChapterIndex;//最大解锁关卡序号

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

			this.maxUnlockChapterIndex = GameManager.Instance.maxUnlockChapterIndex;


			for (int i = 0; i < player.allLearnedSkills.Count; i++) {

				SkillInfo skillInfo = new SkillInfo (player.allLearnedSkills [i]);

				allLearnedSkillInfo.Add (skillInfo);

			}

			this.skillPointsLeft = player.skillPointsLeft;

		}

	}


}
