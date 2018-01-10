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

					Equipment e = new Equipment (im,1);

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

			#warning 测试解锁卷轴
			if (Player.mainPlayer.allUnlockScrollsInBag.Count == 0) {
				for(int i = 0;i<5;i++){
					Item unlockScroll = Item.NewItemWith (200 + i,1);
					Player.mainPlayer.AddItem (unlockScroll);
				}
			}

			Player.mainPlayer.AddItem (Item.NewItemWith (200, 1));

			Player.mainPlayer.AddItem (Item.NewItemWith (430, 1));
			Player.mainPlayer.AddItem (Item.NewItemWith (459, 1));

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

		public void OnItemInBagClick(Item item){
			currentSelectItem = item;
			if (item is CraftingRecipes) {
				bagView.SetUpCraftRecipesDetailHUD (item);
			} else {
				bagView.SetUpItemDetailHUD (item);
			}
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
					bagView.RemoveItemInBag(currentSelectItem);
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
					bagView.SetUpItemsDiaplayPlane ();
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
			ResolveCurrentSelectItemAndGetCharacters ();
			bagView.QuitQueryResolveHUD ();
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


			bagView.SetUpItemsDiaplayPlane ();
				
			bagView.SetUpResolveGainHUD (charactersReturn);
		}

		public void CraftCurrentSelectItem(){

			CraftingRecipes craftRecipes = currentSelectItem as CraftingRecipes;

			ItemModel craftItemModel = GameManager.Instance.gameDataCenter.allItemModels.Find (delegate(ItemModel obj) {
				return obj.itemId == craftRecipes.craftItemId;
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

			string tint = string.Format ("获得 <color=orange>{0}</color> x1", craftedItem.itemName);

			bagView.SetUpTintHUD (tint);

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
