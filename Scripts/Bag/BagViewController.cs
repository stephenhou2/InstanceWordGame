using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class BagViewController : MonoBehaviour {

		public BagView bagView;


		private List<Item> allItemsOfCurrentSelcetType = new List<Item>();

		private int currentSelectEquipIndex;

		private int currentSelectItemIndex;

		private int resolveCount;

		private int minResolveCount;
		private int maxResolveCount;

		public void SetUpBagView(){

			bagView.SetUpBagView ();

			GetComponent<Canvas>().enabled = true; 

		}
			

		// 已装备界面上按钮点击响应
		public void OnEquipedItemButtonsClick(int index){

			allItemsOfCurrentSelcetType.Clear ();

			currentSelectEquipIndex = index;

			EquipmentType equipmentType = EquipmentType.Weapon;

			switch (index) {
			case 0:
				equipmentType = EquipmentType.Weapon;
				break;
			case 1:
				equipmentType = EquipmentType.Armour;
				break;
			case 2:
				equipmentType = EquipmentType.Helmet;
				break;
			case 3:
				equipmentType = EquipmentType.Shield;
				break;
			case 4:
				equipmentType = EquipmentType.Shoes;
				break;
			case 5:
				equipmentType = EquipmentType.Ring;
				break;
			}

			foreach (Item i in Player.mainPlayer.allItems) {
				if (i.itemType == ItemType.Equipment && (i as Equipment).equipmentType == equipmentType) {
					allItemsOfCurrentSelcetType.Add (i);
				}
			}

			bagView.OnEquipedItemButtonsClick (allItemsOfCurrentSelcetType);

		}

		public void OnItemButtonOfSpecificItemPlaneClick(int index){

			Item item = allItemsOfCurrentSelcetType [index];

			bagView.OnItemButtonOfSpecificItemPlaneClick (item, currentSelectEquipIndex);

		}

		public void OnItemButtonClick(int index){

			currentSelectItemIndex = index;

			bagView.OnItemButtonClick (index);

		}

		public void EquipItem(Equipment equipment){

			Player player = Player.mainPlayer;

			if (player.allEquipedEquipments [currentSelectEquipIndex] != null) {
				
				player.allEquipedEquipments [currentSelectEquipIndex].equiped = false;

			}

//			for (int i = 3; i < player.allEquipedEquipments.Count; i++) {
//
//				Item equipedConsumable = player.allEquipedEquipments [i];
//
//				if (equipedConsumable != null && equipedConsumable.itemId == item.itemId) {
//
//					equipedConsumable.equiped = false;
//
//					player.allEquipedEquipments [i] = null;
//
//				}
//
//			}

			equipment.equiped = true;

			player.allEquipedEquipments [currentSelectEquipIndex] = equipment;

			player.ResetBattleAgentProperties (false);

			bagView.OnEquipButtonOfDetailHUDClick ();

		}

		public void ResolveItem(){
			
			Player player = Player.mainPlayer;

			Item item = player.allItems [currentSelectItemIndex];

			maxResolveCount = item.itemCount;
			minResolveCount = 1;

			if (item.itemType == ItemType.Consumables && item.itemCount > 1) {

				bagView.SetUpResolveCountHUD (1, item.itemCount);

				return;
			}

			List<char> charactersReturn =  player.ResolveItem (item,1);

			// 返回的有字母，相应处理
			if (charactersReturn.Count > 0) {

				foreach (char c in charactersReturn) {
					Debug.Log (c.ToString ());
				}

			}

			bagView.OnResolveButtonOfDetailHUDClick ();

		}

		public void OnConfirmResolveCount(){

			Player player = Player.mainPlayer;

			Item item = player.allItems [currentSelectItemIndex];

			int resolveCount = (int)bagView.resolveCountSlider.value;

			List<char> charactersReturn =  player.ResolveItem (item,resolveCount);

			// 返回的有字母，相应处理
			if (charactersReturn.Count > 0) {

				foreach (char c in charactersReturn) {
					Debug.Log (c.ToString ());
				}

			}

			bagView.OnResolveButtonOfDetailHUDClick ();

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

			resolveCount = (int)bagView.resolveCountSlider.value;

			bagView.UpdateResolveCountHUD (resolveCount);
		}


		public void StrengthenItem(){

			Item item = Player.mainPlayer.allItems [currentSelectItemIndex];

			List<char> unsufficientCharacters = Player.mainPlayer.CheckUnsufficientCharacters (item.itemNameInEnglish);

			if (unsufficientCharacters.Count > 0) {

				foreach (char c in unsufficientCharacters) {
					Debug.Log (string.Format ("字母{0}数量不足", c.ToString ()));
				}
				return;

			} 
				

			// 玩家的字母碎片数量足够，进入强化界面
			ResourceManager.Instance.LoadAssetWithBundlePath ("spell/canvas", () => {
				
				GameObject spellCanvas = GameObject.Find(CommonData.instanceContainerName + "/SpellCanvas");

				spellCanvas.GetComponent<SpellViewController>().SetUpSpellViewForStrengthen(item);

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
