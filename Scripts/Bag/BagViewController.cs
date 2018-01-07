using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class BagViewController : MonoBehaviour {

		public BagView bagView;

		private ItemType currentSelectItemType;

		public Item currentSelectItem;

//		public Transform currentSelectDragControl;

		private int minResolveCount;
		private int maxResolveCount;
		private int resolveCount;

		private ExploreManager mExploreManager;
		private ExploreManager exploreManager{
			get{
				if (mExploreManager == null) {
					mExploreManager = TransformManager.FindTransform ("ExploreManager").GetComponent<ExploreManager>();
				}
				return mExploreManager;
			}
		}

//		private Transform mExploreCanvas;
//		private Transform exploreCanvas{
//			get{
//				if (mExploreCanvas == null) {
//					mExploreCanvas = TransformManager.FindTransform ("ExploreCanvas");
//				}
//				return mExploreCanvas;
//			}
//		}


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

			resolveCount = 1;

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

			currentSelectItemType = ItemType.Equipment;

			InitItemsOfCurrentSelectType<Equipment> (Player.mainPlayer.allEquipmentsInBag);

			bagView.SetUpBagView (setVisible);

		}


		public void OnItemTypeButtonClick(int index){

			ItemType itemType = (ItemType)index;

			if (itemType == currentSelectItemType) {
				return;
			}

			currentSelectItemType = itemType;

			switch (itemType) {
			case ItemType.Equipment:
				InitItemsOfCurrentSelectType(Player.mainPlayer.allEquipmentsInBag);
				break;
			case ItemType.Consumables:
				InitItemsOfCurrentSelectType(Player.mainPlayer.allConsumablesInBag);
				break;
			case ItemType.Task:
				InitItemsOfCurrentSelectType(Player.mainPlayer.allTaskItemsInBag);
				break;
			case ItemType.FuseStone:
				InitItemsOfCurrentSelectType(Player.mainPlayer.allFuseStonesInBag);
				break;
//			case ItemType.Material:
//				InitItemsOfCurrentSelectType(Player.mainPlayer.allMaterialsInBag);
//				break;
			}

//			bagView.SetUpItemsDiaplayPlane (allItemsOfCurrentSelectType,currentSelectItem);

		}

		/// <summary>
		/// 将指定类型的物品加入到 <所有当前选择类型的物品> 列表中
		/// </summary>
		/// <param name="itemsInBag">Items in bag.</param>
		private void InitItemsOfCurrentSelectType<T>(List<T> itemsInBag)
			where T:Item
		{
			
//			allItemsOfCurrentSelectType.Clear ();

//			for (int i = 0; i < itemsInBag.Count; i++) {
//				allItemsOfCurrentSelectType.Add (itemsInBag [i]);
//			}
		}

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

		public void OnUseButtonClick(){

//			bool removeFromBag = false;
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
	
		public void OnResolveButtonClick(){

			switch (currentSelectItem.itemType) {
			case ItemType.Equipment:
				ResolveAndGetMaterials (currentSelectItem);
				break;
			case ItemType.Consumables:
				maxResolveCount = currentSelectItem.itemCount;
				minResolveCount = 1;
				bagView.SetUpResolveCountHUD (1, currentSelectItem.itemCount);
				break;
//			case ItemType.Material:
//				maxResolveCount = currentSelectItem.itemCount;
//				minResolveCount = 1;
//				bagView.SetUpResolveCountHUD (1, currentSelectItem.itemCount);
//				break;
			case ItemType.FuseStone:
				ResolveAndGetCharacters (currentSelectItem);
//				allItemsOfCurrentSelectType.Remove (currentSelectItem);
				break;
			}

		}

		public void OnConfirmResolveButtonClick(){

			resolveCount = bagView.GetResolveCountBySlider();

			switch (currentSelectItem.itemType) {
//			case ItemType.Material:
//				ResolveAndGetCharacters (new Material(currentSelectItem as Material,resolveCount));
//				break;
			case ItemType.Consumables:
				ResolveAndGetMaterials (new Consumables(currentSelectItem as Consumables,resolveCount));
				break;
			}


			bagView.QuitResolveCountHUD ();

		}

		/// <summary>
		/// 分解物品（材料和融合石）并获得字母碎片
		/// </summary>
		/// <param name="item">Item.</param>
		private void ResolveAndGetCharacters(Item item){

			List<char> charactersReturn = Player.mainPlayer.GetCharactersFromItem (item, resolveCount);

			List<Item> resolveGainCharacterFragments = new List<Item> ();

			// 返回的有字母，生成字母碎片表
			if (charactersReturn.Count > 0) {

				foreach (char c in charactersReturn) {
					resolveGainCharacterFragments.Add (new CharacterFragment (c));
				}

			}

			if (resolveCount >= currentSelectItem.itemCount) {
//				allItemsOfCurrentSelectType.Remove (currentSelectItem);
			}

			bagView.SetUpResolveGainHUD (resolveGainCharacterFragments);

			currentSelectItem = null;

			switch(item.itemType){
//			case ItemType.Material:
//				bagView.ResetBagView<Material> (Player.mainPlayer.allMaterialsInBag,currentSelectItem);
//				break;
			case ItemType.FuseStone:
				
				RemoveItem (item);
//				bagView.ResetBagView<FuseStone> (Player.mainPlayer.allFuseStonesInBag,currentSelectItem);
				break;
			}

		}
			

		public void RemoveItem(Item item){
			bagView.RemoveItemInBag (item);
		}

		/// <summary>
		/// 分解物品（装备和消耗品）并获得材料
		/// </summary>
		/// <param name="item">Item.</param>
		private void ResolveAndGetMaterials(Item item){

			List<Item> returnedMaterials = (item as Equipment).ResolveEquipment ();

			bagView.SetUpResolveGainHUD (returnedMaterials);

//			allItemsOfCurrentSelectType.Remove (currentSelectItem);

			currentSelectItem = null;

			bagView.RemoveItemInBag (item);

//			bagView.ResetBagView<Equipment> (Player.mainPlayer.allEquipmentsInBag,currentSelectItem);

		}



		// 数量加减按钮点击响应
		public void ResolveCountPlus(int plus){

			int targetCount = resolveCount + plus;

			// 最大或最小值直接返回
			if (targetCount > maxResolveCount || targetCount < minResolveCount) {
				return;
			}

			bagView.UpdateResolveCountHUD (targetCount);

			resolveCount = targetCount;

		}

		/// <summary>
		/// 选择数量的slider拖动时响应方法
		/// </summary>

//		public void OnFixButtonClick(){
//
//			Equipment equipment = currentSelectItem as Equipment;
//
//			GeneralWord word = GeneralWord.RandomGeneralWord();
//				
//			Transform spellCanvas = TransformManager.FindTransform ("SpellCanvas");
//
//			if (spellCanvas != null) {
//
//				spellCanvas.GetComponent<SpellViewController> ().SetUpSpellViewForFix (equipment, word);
//
//				return;
//			} else {// 如果当前没有拼写界面的缓存，则从本地加载拼写界面
//				ResourceLoader spellCanvasLoader = ResourceLoader.CreateNewResourceLoader<GameObject> (CommonData.spellCanvasBundleName);
//
//				ResourceManager.Instance.LoadAssetsUsingWWW (spellCanvasLoader, () => {
//
//					TransformManager.FindTransform ("SpellCanvas").GetComponent<SpellViewController> ().SetUpSpellViewForFix (equipment, word);
//
//				});
//			}
//
//		}



		public void OnQuitResolveCountHUD(){

			resolveCount = 1;

			bagView.QuitResolveCountHUD ();
		}


		// 退出物品详细页HUD
		public void OnQuitItemDetailHUD(){

			bagView.QuitItemDetailHUD ();

		}


		// 退出更换物品页面
		public void OnQuitSpecificTypeHUD(){

			bagView.OnQuitSpecificTypePlane ();

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
