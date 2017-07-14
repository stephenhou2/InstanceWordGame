using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagViewController : MonoBehaviour {

	public BagView bagView;

	private List<Sprite> mItemIcons = new List<Sprite>();

//	private List<Item> allGameItems = new List<Item>();




	private List<Item> allItemsOfCurrentSelcetType = new List<Item>();

	private int currentSelectEquipIndex;

	public void SetUpBagView(){

		if (Player.mainPlayer.allItems.Count == 0) {
			Player.mainPlayer.allItems.AddRange(DataInitializer.LoadDataToModelWithPath<Item> (CommonData.jsonFileDirectoryPath, CommonData.itemsDataFileName));
		}

//		Player.mainPlayer.consumablesEquiped = Player.mainPlayer.allItems;
		 

		// 异步加载物品图片,完成后回调初始化背包界面
		if (mItemIcons.Count == 0) {
			ResourceManager.Instance.LoadSpritesAssetWithFileName ("item/icons", () => {
				
				foreach (Sprite s in ResourceManager.Instance.sprites) {
					mItemIcons.Add (s);
				}

				bagView.SetUpBagView (mItemIcons);

			}, false);
		}


	}

	public void OnEquipedItemButtonsClick(int index){

		allItemsOfCurrentSelcetType.Clear ();

		ItemType type = ItemType.None;

		currentSelectEquipIndex = index;

		switch (index) {
		case 0:
			type = ItemType.Weapon;
			break;
		case 1:
			type = ItemType.Amour;
			break;
		case 2:
			type = ItemType.Shoes;
			break;
		case 3:
			type = ItemType.Consumables;
			break;
		case 4:
			type = ItemType.Consumables;
			break;
		case 5:
			type = ItemType.Consumables;
			break;
		default:
			break;
		}
			


		foreach (Item i in Player.mainPlayer.allItems) {
			if (i.itemType == type) {
				allItemsOfCurrentSelcetType.Add (i);
			}
		}

		bagView.OnEquipedItemButtonsClick (type,allItemsOfCurrentSelcetType);

	}

	public void OnItemButtonOfSpecificItemPlaneClick(int index){

		Item item = allItemsOfCurrentSelcetType [index];

		Player.mainPlayer.allEquipedItems [currentSelectEquipIndex] = item;

		Player.mainPlayer.ResetPropertiesByEquipment (item);

		bagView.OnItemButtonOfSpecificItemPlaneClick (item, currentSelectEquipIndex);




	}

	public void OnItemButtonClick(int index){

		bagView.OnItemButtonClick (index);
	}


	// 退出物品详细页HUD
	public void OnItemDetailHUDClick(){

		bagView.OnQuitItemDetailHUD ();

	}


	// 退出更换物品页面
	public void OnSpecificTypePlaneQuitButtonClick(){

		bagView.OnQuitSpecificTypePlane ();

	}

	// 退出背包界面
	public void OnQuitBagPlaneButtonClick(){

		bagView.OnQuitBagPlane (DestroyInstances);

	}

	// 退出背包界面时清理内存
	private void DestroyInstances(){

		TransformManager.DestroyTransform (gameObject.transform);
		TransformManager.DestroyTransfromWithName ("PropertyText", TransformRoot.InstanceContainer);
	}
}
