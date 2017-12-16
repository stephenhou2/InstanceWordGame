using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class BagViewController : MonoBehaviour {

		public BagView bagView;

		private List<Item> allItemsOfCurrentSelectType = new List<Item>();

//		private List<Equipment> allEquipmentsOfSelectType = new List<Equipment>();

		private ItemType currentSelectItemType;

		private EquipmentType currentSelectEquipmentType;

//		private int currentSelectEquipIndex;

		private Item currentSelectItem;

		private int minResolveCount;
		private int maxResolveCount;
		private int resolveCount;


		void Awake(){

			#warning forTest init some equipments for test
//			if (Player.mainPlayer.allEquipmentsInBag.Count == 0) {
//				for (int i = 0; i < 128; i++) {
//
//					ItemModel im = GameManager.Instance.gameDataCenter.allItemModels.Find (delegate (ItemModel obj) {
//						return obj.itemId == i;
//					});
//
//					Equipment e = new Equipment (im,5);
//
//					Player.mainPlayer.allEquipmentsInBag.Add (e);
//
//				}
//			}

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

			currentSelectItemType = ItemType.Equipment;

			InitItemsOfCurrentSelectType<Equipment> (Player.mainPlayer.allEquipmentsInBag);

			bagView.SetUpBagView (currentSelectItem,setVisible);

		}
			

		// 已装备界面上按钮点击响应
		public void OnEquipedEquipmentButtonClick(int index){

			currentSelectEquipmentType = (EquipmentType)index;

			Equipment equipedEquipment = Player.mainPlayer.allEquipedEquipments.Find (delegate(Equipment obj) {
				return obj.equipmentType == currentSelectEquipmentType;
			});

			List<Equipment> allEquipmentsOfSelectType = Player.mainPlayer.allEquipmentsInBag.FindAll (delegate(Equipment obj) {
				return obj.equipmentType == currentSelectEquipmentType;
			});

			bagView.SetUpAllEquipmentsPlaneOfEquipmentType (equipedEquipment,allEquipmentsOfSelectType);

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
			case ItemType.Material:
				InitItemsOfCurrentSelectType(Player.mainPlayer.allMaterialsInBag);
				break;
			}

			bagView.SetUpItemsDiaplayPlane (allItemsOfCurrentSelectType,currentSelectItem);

		}

		/// <summary>
		/// 将指定类型的物品加入到 <所有当前选择类型的物品> 列表中
		/// </summary>
		/// <param name="itemsInBag">Items in bag.</param>
		private void InitItemsOfCurrentSelectType<T>(List<T> itemsInBag)
			where T:Item
		{
			
			allItemsOfCurrentSelectType.Clear ();

			for (int i = 0; i < itemsInBag.Count; i++) {
				allItemsOfCurrentSelectType.Add (itemsInBag [i]);
			}
		}
			

		public void OnSelectItemInBag(Item item){

			if (currentSelectItem == item) {
				return;
			}

			item.isNewItem = false;

			currentSelectItem = item;

		}

		public void OnEquipButtonClick(Equipment equipment){

			if (equipment.equiped) {
				bagView.OnQuitSpecificTypePlane ();
				return;
			}

			if (equipment.isNewItem) {
				equipment.isNewItem = false;
			}

			Player.mainPlayer.EquipEquipment (equipment);

			bagView.OnEquipButtonOfDetailHUDClick (allItemsOfCurrentSelectType, currentSelectItem);

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
			case ItemType.Material:
				maxResolveCount = currentSelectItem.itemCount;
				minResolveCount = 1;
				bagView.SetUpResolveCountHUD (1, currentSelectItem.itemCount);
				break;
			case ItemType.FuseStone:
				ResolveAndGetCharacters (currentSelectItem);
				allItemsOfCurrentSelectType.Remove (currentSelectItem);
				break;
			}

		}

		public void OnConfirmResolveButtonClick(){

			resolveCount = bagView.GetResolveCountBySlider();

			switch (currentSelectItem.itemType) {
			case ItemType.Material:
				ResolveAndGetCharacters (new Material(currentSelectItem as Material,resolveCount));
				break;
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
				allItemsOfCurrentSelectType.Remove (currentSelectItem);
			}

			bagView.SetUpResolveGainHUD (resolveGainCharacterFragments);

			currentSelectItem = null;

			switch(item.itemType){
			case ItemType.Material:
				bagView.ResetBagView<Material> (Player.mainPlayer.allMaterialsInBag,currentSelectItem);
				break;
			case ItemType.FuseStone:
				bagView.ResetBagView<FuseStone> (Player.mainPlayer.allFuseStonesInBag,currentSelectItem);
				break;
			}

		}

		/// <summary>
		/// 分解物品（装备和消耗品）并获得材料
		/// </summary>
		/// <param name="item">Item.</param>
		private void ResolveAndGetMaterials(Item item){

			List<Item> returnedMaterials = (item as Equipment).ResolveEquipment ();

			bagView.SetUpResolveGainHUD (returnedMaterials);

			allItemsOfCurrentSelectType.Remove (currentSelectItem);

			currentSelectItem = null;

			bagView.ResetBagView<Equipment> (Player.mainPlayer.allEquipmentsInBag,currentSelectItem);

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

		public void OnFixButtonClick(){

			Equipment equipment = currentSelectItem as Equipment;

			GeneralWord word = GeneralWord.RandomGeneralWord();
				
			Transform spellCanvas = TransformManager.FindTransform ("SpellCanvas");

			if (spellCanvas != null) {

				spellCanvas.GetComponent<SpellViewController> ().SetUpSpellViewForFix (equipment, word);

				return;
			} else {// 如果当前没有拼写界面的缓存，则从本地加载拼写界面
				ResourceLoader spellCanvasLoader = ResourceLoader.CreateNewResourceLoader ();

				ResourceManager.Instance.LoadAssetsWithBundlePath (spellCanvasLoader, CommonData.spellCanvasBundleName, () => {

					TransformManager.FindTransform ("SpellCanvas").GetComponent<SpellViewController> ().SetUpSpellViewForFix (equipment, word);

				});
			}

		}



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

			bagView.OnQuitBagPlane (() => {

				GameObject exploreCanvas = GameObject.Find (CommonData.instanceContainerName + "/ExploreCanvas");

				if (exploreCanvas == null) {

						GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.homeCanvasBundleName,"HomeCanvas",()=>{

						GameObject.Find (CommonData.instanceContainerName + "/HomeCanvas").GetComponent<HomeViewController> ().SetUpHomeView ();

//						GameManager.Instance.gameDataCenter.ReleaseDataWithNames(new string[]{"AllItemSprites","AllMaterialSprites","AllMaterials","AllItemModels"});

//						TransformManager.DestroyTransfromWithName ("PoolContainerOfBagCanvas", TransformRoot.PoolContainer);
					});
				}
			});

		}



		// 完全清理背包界面内存
		public void DestroyInstances(){

			GameManager.Instance.UIManager.DestroryCanvasWith (CommonData.bagCanvasBundleName, "BagCanvas", "PoolContainerOfBagCanvas", "ModelContainerOfBagCanvas");

		}
	}
}
