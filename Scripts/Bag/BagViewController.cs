using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class BagViewController : MonoBehaviour {

		public BagView bagView;

		public Item currentSelectItem;

		private ExploreManager mExploreManager;
		private ExploreManager exploreManager{
			get{
				if (mExploreManager == null) {
					mExploreManager = TransformManager.FindTransform ("ExploreManager").GetComponent<ExploreManager>();
				}
				return mExploreManager;
			}
		}

		void Awake(){

			#warning forTest init some equipments for test
			if (Player.mainPlayer.allEquipmentsInBag.Count == 0) {
				for (int i = 0; i < 3; i++) {

//					Debug.Log (GameManager.Instance.gameDataCenter.allItemModels.Count);

					ItemModel im = GameManager.Instance.gameDataCenter.allItemModels.Find (delegate (ItemModel obj) {
						return obj.itemId == i;
					});

					Equipment e = new Equipment (im);

					Player.mainPlayer.AddItem (e);
				}

				for (int i = 100; i < 120; i++) {
					ItemModel im = GameManager.Instance.gameDataCenter.allItemModels.Find (delegate(ItemModel obj) {
						return obj.itemId == i;
					});

					Consumables c = new Consumables (im,1);

					Player.mainPlayer.AddItem (c);
				}
			}

//			for (int i = 0; i < 114; i++) {
//
//				Material m = GameManager.Instance.gameDataCenter.allMaterials.Find (delegate(Material obj) {
//					return obj.itemId == 1000 + i;
//				});
//
//				Material material = new Material (m, 2);
//
//				Player.mainPlayer.allMaterialsInBag.Add (material);
//
//			}


//			ItemModel key = GameManager.Instance.gameDataCenter.allItemModels.Find (delegate(ItemModel obj) {
//				return obj.itemId == 513;
//			});
//
//			ItemModel pickAxe = GameManager.Instance.gameDataCenter.allItemModels.Find (delegate(ItemModel obj) {
//				return obj.itemId == 512;
//			});
//
//			Player.mainPlayer.AddItem (new Consumables (key, 5));
//			Player.mainPlayer.AddItem (new Consumables (pickAxe, 5));


		}

		public void SetUpBagView(bool setVisible){
			StartCoroutine ("SetUpViewAfterDataReady",setVisible);
		}

		private IEnumerator SetUpViewAfterDataReady(bool setVisible){

			bool dataReady = false;

			while (!dataReady) {
				dataReady = GameManager.Instance.gameDataCenter.CheckDatasReady (new GameDataCenter.GameDataType[] {
					GameDataCenter.GameDataType.UISprites,
					GameDataCenter.GameDataType.ItemModels,
					GameDataCenter.GameDataType.ItemSprites,
					GameDataCenter.GameDataType.Skills
				});
				yield return null;
			}

			bagView.SetUpBagView (setVisible);

		}



		/// <summary>
		/// 在物品详细信息页点击了装备按钮（装备）
		/// </summary>
		public void OnEquipButtonClick(){

			for (int i = 0; i < Player.mainPlayer.allEquipedEquipments.Length; i++) {

				Equipment equipment = Player.mainPlayer.allEquipedEquipments [i];

				if (equipment.itemId < 0) {
					Agent.PropertyChange propertyChange = Player.mainPlayer.EquipEquipment (currentSelectItem as Equipment, i);
					bagView.SetUpEquipedEquipmentsPlane ();
					bagView.SetUpPlayerStatusPlane (propertyChange);
					bagView.RemoveItemInBag(equipment);
					bagView.QuitItemDetailHUD ();
					bagView.SetUpEquipedEquipmentsPlane ();
					return;
				}
			}

			bagView.SetUpTintHUD ("装备栏已满");

		}

		/// <summary>
		/// 在物品详细信息页点击了卸下按钮（装备）
		/// </summary>
		public void OnUnloadButtonClick(){

			for (int i = 0; i < Player.mainPlayer.allEquipedEquipments.Length; i++) {
				if (currentSelectItem == Player.mainPlayer.allEquipedEquipments [i]) {
					Agent.PropertyChange propertyChange = Player.mainPlayer.UnloadEquipment (currentSelectItem as Equipment,i);
					bagView.SetUpEquipedEquipmentsPlane ();
					bagView.SetUpPlayerStatusPlane (propertyChange);
					bagView.AddBagItem (currentSelectItem);
					bagView.QuitItemDetailHUD ();
					return;
				}
			}

		}


		/// <summary>
		/// 在物品详细信息页点击了使用按钮（消耗品）
		/// </summary>
		public void OnUseButtonClick(){

			bool consumblesUsedInExploreScene = false;

			Consumables consumables = currentSelectItem as Consumables;

			Agent.PropertyChange propertyChange = new Agent.PropertyChange();

			switch (consumables.itemName) {
			case "药剂":
			case "草药":
			case "蓝莓":
			case "菠菜":
			case "香蕉":
			case "菠萝":
			case "南瓜":
			case "葡萄":
			case "柠檬":
				propertyChange = Player.mainPlayer.UseMedicines (consumables);
				consumblesUsedInExploreScene = false;
				bagView.SetUpPlayerStatusPlane (propertyChange);
				break;  
			case "卷轴":
				consumblesUsedInExploreScene = false;
				break;
			case "锄头":
			case "锯子":
			case "镰刀":
			case "钥匙":
			case "树苗":
			case "火把":
			case "水":
			case "地板":
				consumblesUsedInExploreScene = true;
				break;
			}
				
			bagView.QuitItemDetailHUD ();

			if (consumblesUsedInExploreScene) {
				OnQuitBagPlaneButtonClick ();
				exploreManager.clickForConsumablesPos = true;
				exploreManager.ShowConsumablesValidPointTintAround (consumables);
			}

		}
	
		/// <summary>
		/// 在物品详细信息页点击了分解按钮
		/// </summary>
		public void OnResolveButtonClick(){
			bagView.ShowQueryResolveHUD ();
		}


		/// <summary>
		/// 在分解确认页点击了确认按钮
		/// </summary>
		public void OnConfirmResolveButtonClick(){
			ResolveAndGetCharacters (currentSelectItem);
			bagView.QuitQueryResolveHUD ();
		}

		/// <summary>
		/// 在分解确认页点击了取消按钮
		/// </summary>
		public void OnCancelResolveButtonClick(){
			bagView.QuitQueryResolveHUD ();
		}



		/// <summary>
		/// 分解物品并获得字母碎片
		/// </summary>
		/// <param name="item">Item.</param>
		private void ResolveAndGetCharacters(Item item){

			Item resolvedItem = Item.NewItemWith (item, 1);

			List<char> charactersReturn = ResolveItemAndGetCharacters (resolvedItem);

			List<CharacterFragment> resolveGainCharacterFragments = new List<CharacterFragment> ();

			// 返回的有字母，生成字母碎片表
			if (charactersReturn.Count > 0) {

				foreach (char c in charactersReturn) {
					resolveGainCharacterFragments.Add (new CharacterFragment (c));
				}

			}

			Item itemInBag = Player.mainPlayer.allItemsInBag.Find (delegate(Item obj) {
				return obj.itemId == resolvedItem.itemId;
			});

			if (itemInBag == null) {
				bagView.QuitItemDetailHUD ();
			}

			if (item is Equipment && (item as Equipment).equiped) {
				bagView.SetUpEquipedEquipmentsPlane ();
			}


			bagView.SetUpItemsDiaplayPlane (Player.mainPlayer.allItemsInBag);
				
			bagView.SetUpResolveGainHUD (resolveGainCharacterFragments);
		}

		/// <summary>
		/// 分解材料
		/// </summary>
		/// <returns>分解后获得的字母碎片</returns>
		public List<char> ResolveItemAndGetCharacters(Item item){

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
			for (int j = 0; j < item.itemCount; j++) {

				for (int i = 0; i < charactersReturnCount; i++) {

					char character = ReturnRandomCharacters (ref charList);

					int characterIndex = (int)character - CommonData.aInASCII;

					Player.mainPlayer.charactersCount [characterIndex]++;

					charactersReturn.Add (character);
				}
			}

			// 被分解的物品减去分解数量，如果数量<=0,从背包中删除物品
			Player.mainPlayer.RemoveItem(item);

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


			

		public void RemoveItem(Item item){
			bagView.RemoveItemInBag (item);
		}



		// 退出物品详细页HUD
		public void OnQuitItemDetailHUD(){
			bagView.QuitItemDetailHUD ();

		}


		// 退出背包界面
		public void OnQuitBagPlaneButtonClick(){

			bagView.QuitBagPlane ();

			Transform exploreCanvas = TransformManager.FindTransform ("ExploreCanvas");

			if (exploreCanvas == null) {

				GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.homeCanvasBundleName, "HomeCanvas", () => {

					TransformManager.FindTransform ("HomeCanvas").GetComponent<HomeViewController> ().SetUpHomeView ();

//					GameManager.Instance.gameDataCenter.ReleaseDataWithNames(new string[]{"AllItemSprites","AllMaterialSprites","AllMaterials","AllItemModels"});

//					TransformManager.DestroyTransfromWithName ("PoolContainerOfBagCanvas", TransformRoot.PoolContainer);
				});
			}

			Time.timeScale = 1;

		}



		// 完全清理背包界面内存
		public void DestroyInstances(){

			GameManager.Instance.UIManager.DestroryCanvasWith (CommonData.bagCanvasBundleName, "BagCanvas", "PoolContainerOfBagCanvas", "ModelContainerOfBagCanvas");

		}
	}
}
