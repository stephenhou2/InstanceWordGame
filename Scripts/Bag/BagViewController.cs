using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class BagViewController : MonoBehaviour {

		public BagView bagView;

		private List<Item> allItemsOfCurrentSelcetType = new List<Item>();

//		private List<Equipment> allEquipmentsOfSelectType = new List<Equipment>();

		private ItemType currentSelectItemType;

		private EquipmentType currentSelectEquipmentType;

//		private int currentSelectEquipIndex;

		private Item currentSelectItem;

		private int resolveCount;

		private int minResolveCount;
		private int maxResolveCount;


		#warning forTest init some equipments for test
		void Awake(){
			
			if (Player.mainPlayer.allEquipmentsInBag.Count == 0) {
				for (int i = 0; i < 128; i++) {

					ItemModel im = GameManager.Instance.dataCenter.allItemModels.Find (delegate (ItemModel obj) {
						return obj.itemId == i;
					});

					Equipment e = new Equipment (im);

					Player.mainPlayer.allEquipmentsInBag.Add (e);

				}
			}
		}

		public void SetUpBagView(){

			bagView.SetUpBagView ();

			GetComponent<Canvas>().enabled = true; 

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

			bagView.SetUpItemsDiaplayPlane (allItemsOfCurrentSelcetType);

		}

		/// <summary>
		/// 将指定类型的物品加入到 <所有当前选择类型的物品> 列表中
		/// </summary>
		/// <param name="itemsInBag">Items in bag.</param>
		private void InitItemsOfCurrentSelectType<T>(List<T> itemsInBag)
			where T:Item
		{
			
			allItemsOfCurrentSelcetType.Clear ();

			for (int i = 0; i < itemsInBag.Count; i++) {
				allItemsOfCurrentSelcetType.Add (itemsInBag [i]);
			}
		}

		public void OnItemButtonOfSpecificItemPlaneClick(int index){

			Item item = allItemsOfCurrentSelcetType [index];

			bagView.OnItemButtonOfSpecificItemPlaneClick (item);

		}

		public void SelectItem(Item item){

			if (currentSelectItem == item) {
				return;
			}

			currentSelectItem = item;

		}

		public void EquipEquipment(Equipment equipment){

			List<Equipment> allEquipedEquipments = Player.mainPlayer.allEquipedEquipments;

			Equipment equipedEquipmentOfSelectType = allEquipedEquipments.Find (delegate(Equipment obj) {
				return obj.equipmentType == equipment.equipmentType;
			});

			if (equipedEquipmentOfSelectType != null) {
				
				equipedEquipmentOfSelectType.equiped = false;

			}

			equipment.equiped = true;

			allEquipedEquipments.Add(equipment);

			Player.mainPlayer.ResetBattleAgentProperties (false);

			bagView.OnEquipButtonOfDetailHUDClick ();

		}

		public void ResolveItem(){
			
//			Player player = Player.mainPlayer;
//
//			Item item = player.allItems [currentSelectItemIndex];
//
//			maxResolveCount = item.itemCount;
//			minResolveCount = 1;
//
//			if (item.itemType == ItemType.Consumables && item.itemCount > 1) {
//
//				bagView.SetUpResolveCountHUD (1, item.itemCount);
//
//				return;
//			}
//
//			List<char> charactersReturn =  player.ResolveItem (item,1);
//
//			// 返回的有字母，相应处理
//			if (charactersReturn.Count > 0) {
//
//				foreach (char c in charactersReturn) {
//					Debug.Log (c.ToString ());
//				}
//
//			}
//
//			bagView.OnResolveButtonOfDetailHUDClick ();

		}

		public void OnConfirmResolveCount(){

//			Player player = Player.mainPlayer;
//
//			Item item = player.allItems [currentSelectItemIndex];
//
//			int resolveCount = (int)bagView.resolveCountSlider.value;
//
//			List<char> charactersReturn =  player.ResolveItem (item,resolveCount);
//
//			// 返回的有字母，相应处理
//			if (charactersReturn.Count > 0) {
//
//				foreach (char c in charactersReturn) {
//					Debug.Log (c.ToString ());
//				}
//
//			}
//
//			bagView.OnResolveButtonOfDetailHUDClick ();

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
		public void ResolveCountSliderDrag(){

			resolveCount = bagView.GetResolveCountBySlider ();

			bagView.UpdateResolveCountHUD (resolveCount);
		}


		public void FixEquipment(){

			Equipment equipment = currentSelectItem as Equipment;

			List<char> unsufficientCharacters = Player.mainPlayer.CheckUnsufficientCharacters (equipment.itemNameInEnglish);

			if (unsufficientCharacters.Count > 0) {

				foreach (char c in unsufficientCharacters) {
					Debug.Log (string.Format ("字母{0}数量不足", c.ToString ()));
				}
				return;

			} 
				

			// 玩家的字母碎片数量足够，进入修复界面

			ResourceLoader spellCanvasLoader = ResourceLoader.CreateNewResourceLoader ();

			ResourceManager.Instance.LoadAssetsWithBundlePath (spellCanvasLoader,"spell/canvas", () => {
	
				Word word = Word.RandomWord();

				TransformManager.FindTransform("SpellCanvas").GetComponent<SpellViewController>().SetUpSpellViewForFix(equipment,word);

				OnQuitItemDetailHUD ();

			});



		}



		public void OnQuitResolveCountHUD(){

			resolveCount = 1;

			bagView.OnQuitResolveCountHUD ();
		}


		// 退出物品详细页HUD
		public void OnQuitItemDetailHUD(){

			bagView.OnQuitItemDetailHUD ();

		}


		// 退出更换物品页面
		public void OnQuitSpecificTypeHUD(){

			bagView.OnQuitSpecificTypePlane ();

		}

		// 退出背包界面
		public void OnQuitBagPlaneButtonClick(){

			bagView.OnQuitBagPlane (() => {

				GameObject exploreCanvas = GameObject.Find (CommonData.instanceContainerName + "/ExploreCanvas");

				if (exploreCanvas != null) {

					GetComponent<Canvas>().enabled = false;

				} else {

					GameObject homeCanvas = GameObject.Find (CommonData.instanceContainerName + "/HomeCanvas");

					if (homeCanvas != null) {
						homeCanvas.GetComponent<HomeViewController> ().SetUpHomeView ();
					}

					DestroyInstances(); 
				}
			});

		}

		// 退出背包界面时清理内存
		private void DestroyInstances(){

			TransformManager.DestroyTransform (gameObject.transform);
			TransformManager.DestroyTransfromWithName ("ItemDetailModel", TransformRoot.InstanceContainer);
			TransformManager.DestroyTransfromWithName ("ItemDetailsPool", TransformRoot.PoolContainer);
		
			Resources.UnloadUnusedAssets ();

			System.GC.Collect ();
		
		}
	}
}
