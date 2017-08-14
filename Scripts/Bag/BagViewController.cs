using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

		ItemType type = ItemType.Task;

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

		bagView.OnItemButtonOfSpecificItemPlaneClick (item, currentSelectEquipIndex);

	}

	public void OnItemButtonClick(int index){

		currentSelectItemIndex = index;

		bagView.OnItemButtonClick (index);

	}

	public void EquipItem(Item item){

		Player player = Player.mainPlayer;

		if (player.allEquipedItems [currentSelectEquipIndex] != null) {
			
			player.allEquipedItems [currentSelectEquipIndex].equiped = false;

		}

		for (int i = 3; i < player.allEquipedItems.Count; i++) {

			Item equipedConsumable = player.allEquipedItems [i];

			if (equipedConsumable != null && equipedConsumable.itemId == item.itemId) {

				equipedConsumable.equiped = false;

				player.allEquipedItems [i] = null;

			}

		}

		item.equiped = true;

		player.allEquipedItems [currentSelectEquipIndex] = item;

		player.ResetBattleAgentProperties (false,false);

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
		ResourceManager.Instance.LoadAssetWithFileName ("spell/canvas", () => {
			
			GameObject spellCanvas = GameObject.Find(CommonData.instanceContainerName + "/SpellCanvas");
			spellCanvas.GetComponent<SpellViewController>().SetUpSpellView(item,SpellPurpose.Strengthen);

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

		bagView.OnQuitBagPlane (DestroyInstances);

		GameObject homeCanvas = GameObject.Find (CommonData.instanceContainerName + "/HomeCanvas");

		if (homeCanvas != null) {
			homeCanvas.GetComponent<HomeViewController> ().SetUpHomeView ();
		}

	}

	// 退出背包界面时清理内存
	private void DestroyInstances(){

		TransformManager.DestroyTransform (gameObject.transform);
		TransformManager.DestroyTransfromWithName ("ItemDetailModel", TransformRoot.InstanceContainer);
		TransformManager.DestroyTransfromWithName ("ItemDetailsPool", TransformRoot.PoolContainer);
	}
}
