using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagViewController : MonoBehaviour {

	public BagView bagView;

//	private List<Sprite> mItemIcons = new List<Sprite>();

//	private List<Item> allGameItems = new List<Item>();




	private List<Item> allItemsOfCurrentSelcetType = new List<Item>();

	private int currentSelectEquipIndex;

	private int currentSelectItemIndex;

	private int currentSelectItemIndexOfSpecificPlane;

	public void SetUpBagView(){

//		if (Player.mainPlayer.allItems.Count == 0) {
//			Player.mainPlayer.allItems.AddRange(DataInitializer.LoadDataToModelWithPath<Item> (CommonData.jsonFileDirectoryPath, CommonData.itemsDataFileName));
//		}

	
		bagView.SetUpBagView ();

//		Player.mainPlayer.consumablesEquiped = Player.mainPlayer.allItems;
		 

//		// 异步加载物品图片,完成后回调初始化背包界面
//		if (mItemIcons.Count == 0) {
//			ResourceManager.Instance.LoadSpritesAssetWithFileName ("item/icons", () => {
//				
//				foreach (Sprite s in ResourceManager.Instance.sprites) {
//					mItemIcons.Add (s);
//				}
//
//				bagView.SetUpBagView (mItemIcons);
//
//			}, false);
//		}


	}
		

	// 已装备界面上按钮点击响应
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

	#warning 明天这里继续，点击物品后弹出物品比较栏
	public void OnItemButtonOfSpecificItemPlaneClick(int index){

		currentSelectItemIndexOfSpecificPlane = index;

		Item item = allItemsOfCurrentSelcetType [index];

		bagView.OnItemButtonOfSpecificItemPlaneClick (item, currentSelectEquipIndex);

	}

	public void OnItemButtonClick(int index){

		currentSelectItemIndex = index;

		bagView.OnItemButtonClick (index);

	}

	public void EquipItem(){

		Player player = Player.mainPlayer;

		player.allEquipedItems [currentSelectEquipIndex].equiped = false;

		Item item = allItemsOfCurrentSelcetType [currentSelectItemIndexOfSpecificPlane];

		item.equiped = true;

		player.allEquipedItems [currentSelectEquipIndex] = item;

		player.ResetBattleAgentProperties (false,false);

		bagView.OnCreateButtonOfDetailHUDClick ();

	}

	public void ResolveItem(){
		
		Player player = Player.mainPlayer;

		Item item = player.allItems [currentSelectItemIndex];

		List<char> charactersReturn =  player.ResolveItem (item);

		// 返回的有字母，相应处理
		if (charactersReturn.Count > 0) {

			foreach (char c in charactersReturn) {
				Debug.Log (c.ToString ());
			}

		}

		bagView.OnResolveButtonOfDetailHUDClick ();

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

		GameObject homeCanvas = GameObject.Find (CommonData.instanceContainerName + "/HomeCanvas");

		if (homeCanvas != null) {
			homeCanvas.GetComponent<HomeViewController> ().SetUpHomeView ();
		}

	}

	// 退出背包界面时清理内存
	private void DestroyInstances(){

		TransformManager.DestroyTransform (gameObject.transform);
		TransformManager.DestroyTransfromWithName ("PropertyText", TransformRoot.InstanceContainer);
		TransformManager.DestroyTransfromWithName ("ItemButton", TransformRoot.InstanceContainer);
		TransformManager.DestroyTransfromWithName ("ItemButtonPool", TransformRoot.PoolContainer);
		TransformManager.DestroyTransfromWithName ("PropertyTextPool", TransformRoot.PoolContainer);
	}
}
