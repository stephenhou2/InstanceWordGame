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


		private int currentBagIndex;

		void Awake(){

//			Player.mainPlayer.AddItem (Item.NewItemWith(8,1));
//			Player.mainPlayer.AddItem (Item.NewItemWith (19, 1));
//			Player.mainPlayer.AddItem (Item.NewItemWith (56, 1));
//			Player.mainPlayer.AddItem(Item.NewItemWith(51,1));
//			Player.mainPlayer.AddItem(Item.NewItemWith(111,1));


//			Player.mainPlayer.AddItem (Item.NewItemWith (448, 1));
//			Player.mainPlayer.AddItem (Item.NewItemWith (13, 1));
//			Player.mainPlayer.AddItem (Item.NewItemWith (14, 2));
//			Player.mainPlayer.AddItem (Item.NewItemWith (17, 1));
//			Player.mainPlayer.AddItem (Item.NewItemWith (17, 1));
//			Player.mainPlayer.AddItem (Item.NewItemWith (17, 1));


			#warning 测试物品用
//			if (Player.mainPlayer.allEquipmentsInBag.Count == 0) {
//				for (int i = 0; i < 10; i++) {
//
////					Debug.Log (GameManager.Instance.gameDataCenter.allItemModels.Count);
//
//					ItemModel im = GameManager.Instance.gameDataCenter.allItemModels.Find (delegate (ItemModel obj) {
//						return obj.itemId == i;
//					});
//
//					Equipment e = new Equipment (im, 1);
//
//					Player.mainPlayer.AddItem (e);
//				}
//			}

				for (int i = 100; i < 120; i++) {
					ItemModel im = GameManager.Instance.gameDataCenter.allItemModels.Find (delegate(ItemModel obj) {
						return obj.itemId == i;
					});

					Consumables c = new Consumables (im,1);

					Player.mainPlayer.AddItem (c);

				}
	
//			#warning 测试解锁卷轴
//			if (Player.mainPlayer.allUnlockScrollsInBag.Count == 0) {
//				for(int i = 0;i<5;i++){
//					Item unlockScroll = Item.NewItemWith (200 + i,1);
//					Player.mainPlayer.AddItem (unlockScroll);
//				}
//			}
//
//			for (int i = 40; i < 45; i++) {
//				Item craftingRecipe = Item.NewItemWith (400 + i, 1);
//
//				Player.mainPlayer.AddItem (craftingRecipe);
//			}
				


		}





		public void SetUpBagView(bool setVisible){

			currentBagIndex = 0;

//			StartCoroutine ("SetUpViewAfterDataReady",setVisible);
//		}
//
//		private IEnumerator SetUpViewAfterDataReady(bool setVisible){
//
//			bool dataReady = false;
//
//			while (!dataReady) {
//				dataReady = GameManager.Instance.gameDataCenter.CheckDatasReady (new GameDataCenter.GameDataType[] {
//					GameDataCenter.GameDataType.UISprites,
//					GameDataCenter.GameDataType.ItemModels,
//					GameDataCenter.GameDataType.ItemSprites,
//					GameDataCenter.GameDataType.Skills
//				});
//				yield return null;
//			}

			bagView.SetUpBagView (setVisible);

		}

		public void OnItemInEquipmentPlaneClick(Item item,int equipmentIndexInPanel){

			if (!BuyRecord.Instance.equipmentSlotUnlockedArray [equipmentIndexInPanel]) {
				Debug.Log ("未解锁");
				return;
			}

			if (item.itemId < 0) {
				return;
			}

			OnItemInBagClick (item);

		}

		public void OnItemInBagClick(Item item){

			currentSelectItem = item;
			if (item is CraftingRecipe) {
				bagView.SetUpCraftRecipesDetailHUD (item);
			} else {
				bagView.SetUpItemDetailHUD (item);
			}
		}

		/// <summary>
		/// 在物品详细信息页点击了装备按钮（装备）
		/// </summary>
		public void OnEquipButtonClick(){

			bool[] equipmentSlotUnlockedArray = BuyRecord.Instance.equipmentSlotUnlockedArray;

			for (int i = 0; i < Player.mainPlayer.allEquipedEquipments.Length; i++) {

				Equipment equipment = Player.mainPlayer.allEquipedEquipments [i];

				if (equipment.itemId < 0 && equipmentSlotUnlockedArray[i]) {
					Agent.PropertyChange propertyChange = Player.mainPlayer.EquipEquipment (currentSelectItem as Equipment, i);
					bagView.SetUpEquipedEquipmentsPlane ();
					bagView.SetUpPlayerStatusPlane (propertyChange);
					bagView.RemoveItemInBag(currentSelectItem);
					bagView.QuitItemDetailHUD ();
					bagView.SetUpEquipedEquipmentsPlane ();
					return;
				}
			}

			bagView.SetUpTintHUD ("装备栏已满",null);

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
					bagView.SetUpBagItemsPlane (currentBagIndex);
					bagView.QuitItemDetailHUD ();
					return;
				}
			}

		}


		/// <summary>
		/// 在物品详细信息页点击了使用按钮（消耗品）
		/// </summary>
		public void OnUseButtonClick(){

			Debug.Log ("使用了" + currentSelectItem.itemName);

			bool consumblesUsedInExploreScene = false;

			Consumables consumables = currentSelectItem as Consumables;

			Agent.PropertyChange propertyChange = new Agent.PropertyChange();

			switch (consumables.itemName) {
			case "药剂":
			case "草药":
			case "蓝莓":
			case "菠菜":
			case "胡萝卜":
			case "樱桃":
			case "南瓜":
			case "蘑菇":
			case "辣椒":
				
				propertyChange = Player.mainPlayer.UseMedicines (consumables);

				bagView.SetUpPlayerStatusPlane (propertyChange);

				consumblesUsedInExploreScene = false;
				break;  
			case "卷轴":

				Player.mainPlayer.RemoveItem (consumables, 1);

				Player.mainPlayer.ResetBattleAgentProperties (true);

				TransformManager.FindTransform ("ExploreManager").GetComponent<ExploreManager> ().QuitExploreScene (true);

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
			case "土块":
			case "开关":
				consumblesUsedInExploreScene = true;
				break;
			}
				
			bagView.QuitItemDetailHUD ();

			if (consumblesUsedInExploreScene) {
				OnQuitBagPlaneButtonClick ();
				exploreManager.clickForConsumablesPos = true;
				exploreManager.ShowConsumablesValidPointTintAround (consumables);
			}

			bagView.SetUpBagItemsPlane (currentBagIndex);

		}


		public void ChangeBag(int bagIndex){
			if (bagIndex == currentBagIndex) {
				return;
			}
			currentBagIndex = bagIndex;
			bagView.SetUpBagItemsPlane (bagIndex);
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
			ResolveCurrentSelectItemAndGetCharacters ();
			bagView.QuitQueryResolveHUD ();
			bagView.QuitItemDetailHUD ();
		}

		/// <summary>
		/// 在分解确认页点击了取消按钮
		/// </summary>
		public void OnCancelResolveButtonClick(){
			bagView.QuitQueryResolveHUD ();
		}


		public void OnShowCharactersButtonClick(){
			bagView.SetUpCharactersInBagPlane ();
		}

		public void QuitCharactersPlane(){
			bagView.QuitCharactersInBagPlane ();
		}


		/// <summary>
		/// 分解物品并获得字母碎片
		/// </summary>
		/// <param name="item">Item.</param>
		public void ResolveCurrentSelectItemAndGetCharacters(){

			List<char> charactersReturn = Player.mainPlayer.ResolveItemAndGetCharacters (currentSelectItem,1);

//			List<CharacterFragment> resolveGainCharacterFragments = new List<CharacterFragment> ();

			// 返回的有字母，生成字母碎片表
//			if (charactersReturn.Count > 0) {
//
//				foreach (char c in charactersReturn) {
//					resolveGainCharacterFragments.Add (new CharacterFragment (c));
//				}
//
//			}

//			Item itemInBag = Player.mainPlayer.allItemsInBag.Find (delegate(Item obj) {
//				return obj == currentSelectItem;
//			});
//
//			if (itemInBag == null) {
//				bagView.QuitItemDetailHUD ();
//			}

			if (currentSelectItem is Equipment && (currentSelectItem as Equipment).equiped) {
				bagView.SetUpEquipedEquipmentsPlane ();
			}


			bagView.SetUpBagItemsPlane (currentBagIndex);
				
			bagView.SetUpResolveGainHUD (charactersReturn);
		}

		public void CraftCurrentSelectItem(){

			CraftingRecipe craftRecipe = currentSelectItem as CraftingRecipe;

			ItemModel craftItemModel = GameManager.Instance.gameDataCenter.allItemModels.Find (delegate(ItemModel obj) {
				return obj.itemId == craftRecipe.craftItemId;
			});

			for (int i = 0; i < craftItemModel.itemInfosForProduce.Length; i++) {
				ItemModel.ItemInfoForProduce itemInfo = craftItemModel.itemInfosForProduce [i];
				for (int j = 0; j < itemInfo.itemCount; j++) {
					Item item = Player.mainPlayer.allItemsInBag.Find (delegate (Item obj) {
						return obj.itemId == itemInfo.itemId;
					});
					Player.mainPlayer.RemoveItem (item,1);
				}
			}

			Item craftedItem = Item.NewItemWith (craftItemModel,1);
			Player.mainPlayer.AddItem (craftedItem);

			Player.mainPlayer.RemoveItem (currentSelectItem,1);

			string tint = string.Format ("获得 <color=orange>{0}</color> x1", craftedItem.itemName);

			Sprite itemSprite = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
				return obj.name == craftedItem.spriteName;
			});

			bagView.SetUpTintHUD (tint,itemSprite);
			bagView.RemoveItemInBag (currentSelectItem);
			bagView.AddBagItem (craftedItem);

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

			GameManager.Instance.UIManager.RemoveCanvasCache ("BagCanvas");

			Destroy (this.gameObject);

			MyResourceManager.Instance.UnloadAssetBundle (CommonData.bagCanvasBundleName,true);

		}
	}
}
