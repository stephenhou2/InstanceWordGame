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
					for(int i = 0;i<mCharactersCount.Length;i++){
						mCharactersCount[i] = 10;
					}
				}
				return mCharactersCount;
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

			#warning 这里用来玩家信息初始化

		}




		private void InitPlayerFromLocalData(){

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
		public List<char> ResolveMaterial(Material material,int resolveCount){

			//分解后得到的字母碎片
			List<char> charactersReturn = new List<char> ();

			int charactersReturnCount = 1;

			char[] charArray = material.itemNameInEnglish.ToCharArray ();

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

			material.itemCount -= resolveCount;

			if (material.itemCount <= 0) {
				allMaterialsInBag.Remove (material);
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
}
