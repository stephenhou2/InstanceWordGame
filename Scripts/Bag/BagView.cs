using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BagView : MonoBehaviour {


	public Slider healthBar;
	public Slider strengthBar;

	public Text healthText;
	public Text strengthText;

	public Text playerLevelText;
	public Text progressText;

	public Text attackText;
	public Text magicText;
	public Text amourText;
	public Text magicResistText;
	public Text critText;
	public Text agilityText;

//	public Image weaponImage;
//	public Image amourImage;
//	public Image shoesImage;
	public Button[] allEquipedItemBtns;

	public Button[] bags;

	public Button[] allItemsBtns;


	private Player player;

	private List<Sprite> sprites = new List<Sprite> ();

	public Transform bagViewContainer;
	public GameObject bagPlane;


	public Transform itemDetailHUD;

	public Image itemIcon;
	public Text itemName;
	public Text itemTypeText;
	public Text itemQualityText;
	public Text itemStrengthenTimesText;
	public Text itemPropertiesText;

	public Transform choicePanelWithOneBtn;
	public Transform choicePanelWithTwoBtns;


	public Transform specificTypeItemHUD;
	public Transform itemDetailContainer;
	public GameObject itemDetailModel;

	private InstancePool itemDetailsPool;

	public Transform resolveCountHUD;
	public Button minusBtn;
	public Button plusBtn;
	public Slider resolveCountSlider;
	public Text resolveCount;

	private Sprite typeBtnNormalSprite;
	private Sprite typeBtnSelectedSprite;

	/// <summary>
	/// 初始化背包界面
	/// </summary>
	public void SetUpBagView(){

		this.sprites = GameManager.Instance.allItemSprites;
		this.player = Player.mainPlayer;

		typeBtnNormalSprite = GameManager.Instance.allUIIcons.Find (delegate(Sprite obj) {
			return obj.name == "typeButtonNormal";
		});

		typeBtnSelectedSprite = GameManager.Instance.allUIIcons.Find (delegate(Sprite obj) {
			return obj.name == "typeButtonSelected";
		});

		itemDetailsPool = InstancePool.GetOrCreateInstancePool ("ItemDetailsPool");

		SetUpPlayerStatusPlane ();

		SetUpEquipedItemPlane ();

		SetUpAllItemsPlane ();

		this.GetComponent<Canvas> ().enabled = true;

	}

	/// <summary>
	/// 初始化玩家属性界面
	/// </summary>
	private void SetUpPlayerStatusPlane(){

		healthBar.maxValue = player.maxHealth;
		healthBar.value = player.health;

		strengthBar.maxValue = player.maxStrength;
		strengthBar.value = player.strength;

		healthText.text = player.health.ToString () + "/" + player.maxHealth.ToString ();
		strengthText.text = player.strength.ToString () + "/" + player.maxStrength.ToString ();

		playerLevelText.text = "Lv." + player.agentLevel;

		#warning 进度文字未设置
		attackText.text = "攻击:" + player.attack.ToString ();
		magicText.text = "魔法:" + player.magic.ToString ();
		amourText.text = "护甲:" + player.amour.ToString();
		magicResistText.text = "抗性:" + player.magicResist.ToString();
		critText.text = "暴击:" + (player.crit / (1 + 0.01f * player.crit)).ToString("F0") + "%";
		agilityText.text = "闪避:" + (player.agility / (1 + 0.01f * player.agility)).ToString("F0") + "%";

	}

	// 初始化已装备物品界面
	private void SetUpEquipedItemPlane(){

		for(int i = 0;i<player.allEquipedItems.Count;i++){
			Item item = player.allEquipedItems[i];
			SetUpItemButton (item, allEquipedItemBtns [i]);
		}

	}
		
	/// <summary>
	/// 初始化背包物品界面
	/// </summary>
	#warning 现在用所有物品做测试，后面按照类型进行分
	public void SetUpAllItemsPlane(){
		
		for (int i = 0; i < allItemsBtns.Length; i++) {

			Button itemBtn = allItemsBtns [i];

			Text extraInfo = allItemsBtns [i].transform.FindChild ("ExtraInfo").GetComponent<Text> ();

			if (i < player.allItems.Count) {

				Item item = player.allItems [i];

				SetUpItemButton (player.allItems [i], itemBtn);

				if (item.equiped &&
				    (item.itemType == ItemType.Weapon || item.itemType == ItemType.Amour || item.itemType == ItemType.Shoes)) {
					extraInfo.text = "<color=green>已装备</color>";
				} else if (item.itemType == ItemType.Consumables) {
					extraInfo.text = item.itemCount.ToString ();
				} else {
					extraInfo.text = string.Empty;
				}
			} else {
				
				SetUpItemButton (null, itemBtn);

				Image selectedBorder = itemBtn.transform.FindChild ("SelectedBorder").GetComponent<Image> ();
				selectedBorder.enabled = false;

				extraInfo.text = string.Empty;

			}
		}

	}
		
	/// <summary>
	/// 初始化物品详细介绍页面
	/// </summary>
	/// <param name="item">Item.</param>
	private void SetUpItemDetailHUD(Item item){

		bool canStrengthen = item.CheckCanStrengthen ();

		itemDetailHUD.gameObject.SetActive (true);

		if (canStrengthen) {
			choicePanelWithTwoBtns.gameObject.SetActive (true);
		} else{
			choicePanelWithOneBtn.gameObject.SetActive (true);
		}

		itemIcon.sprite = sprites.Find (delegate(Sprite obj) {
			return obj.name == item.spriteName;
		});
		itemIcon.enabled = true;

		itemName.text = item.itemName;

		itemTypeText.text = item.GetItemTypeString ();

		if (item.itemType == ItemType.Weapon || item.itemType == ItemType.Amour || item.itemType == ItemType.Shoes) {

			itemQualityText.text = item.GetItemQualityString ();

			itemStrengthenTimesText.text = "已强化次数:" + item.strengthenTimes.ToString () + "次";
		}


		Item equipedItemOfCurrentType = null;

		string itemPropertiesString = string.Empty;


		if (item.itemType == ItemType.Consumables || item.itemType == ItemType.Inscription || item.itemType == ItemType.Task) {

			itemPropertiesText.text = item.GetItemPropertiesString ();

			return;

		}

		switch (item.itemType) {
		case ItemType.Weapon:
			equipedItemOfCurrentType = player.allEquipedItems [0];
			break;
		case ItemType.Amour:
			equipedItemOfCurrentType = player.allEquipedItems [1];
			break;
		case ItemType.Shoes:
			equipedItemOfCurrentType = player.allEquipedItems [2];
			break;
		}

		if (equipedItemOfCurrentType != null) {
			itemPropertiesString = item.GetComparePropertiesStringWithItem (equipedItemOfCurrentType);
		} else {
			itemPropertiesString = item.GetItemPropertiesString ();
		}

		itemPropertiesText.text = itemPropertiesString;
	

	}

	/// <summary>
	/// 初始化选择分解数量界面
	/// </summary>
	public void SetUpResolveCountHUD(int minValue,int maxValue){

		resolveCountHUD.gameObject.SetActive (true);

		if (minusBtn.GetComponent<Image> ().sprite == null 
			|| plusBtn.GetComponent<Image>().sprite == null) 
		{
			Sprite arrowSprite = GameManager.Instance.allUIIcons.Find (delegate(Sprite obj) {
				return obj.name == "arrowIcon";
			});

			minusBtn.GetComponent<Image> ().sprite = arrowSprite;
			plusBtn.GetComponent<Image> ().sprite = arrowSprite;
		}

		resolveCountSlider.minValue = minValue;
		resolveCountSlider.maxValue = maxValue;

		resolveCountSlider.value = minValue;

	}

	public void UpdateResolveCountHUD(int count){

		resolveCountSlider.value = count;

		resolveCount.text = "分解" + count.ToString() + "个";
	}

	/// <summary>
	/// 背包中单个物品按钮的初始化方法
	/// </summary>
	/// <param name="item">Item.</param>
	/// <param name="btn">Button.</param>
	private void SetUpItemButton(Item item,Button btn){

//		if (item == null || item.itemName == null) {
//			btn.interactable = (item != null);
//			Image itemIcon = btn.transform.FindChild ("ItemIcon").GetComponent<Image>();
//			itemIcon.enabled = false;
//			itemIcon.sprite = null;
//
//		}else if (item != null && item.itemName != null) {
//			btn.interactable = (item != null);
//			Image image = btn.transform.FindChild ("ItemIcon").GetComponent<Image>();
//			image.enabled = true;
//			image.sprite = sprites.Find (delegate(Sprite obj) {
//				return obj.name == item.spriteName;
//			});
//		}

		if (item == null) {
//			btn.interactable = (item != null);
			Image itemIcon = btn.transform.FindChild ("ItemIcon").GetComponent<Image>();
			itemIcon.enabled = false;
			itemIcon.sprite = null;

		}
		else if (item != null && item.itemName != null) {
//			btn.interactable = (item != null);
			Image image = btn.transform.FindChild ("ItemIcon").GetComponent<Image>();
			image.enabled = true;
			image.sprite = sprites.Find (delegate(Sprite obj) {
				return obj.name == item.spriteName;
			});
		}
	}


	/// <summary>
	/// 更换装备／物品的方法
	/// </summary>
	/// <param name="type">Type.</param>
	/// <param name="allItemsOfCurrentSelectType">All items of current select type.</param>
	public void OnEquipedItemButtonsClick(ItemType type,List<Item> allItemsOfCurrentSelectType){

		for(int i =0;i<allItemsOfCurrentSelectType.Count;i++){
			
			Item item = allItemsOfCurrentSelectType[i];

			Transform itemDetail = itemDetailsPool.GetInstance<Transform> (itemDetailModel,itemDetailContainer);

			itemDetail.GetComponent<ItemDetailView>().SetUpItemDetailView(item,GetComponent<BagViewController> ());
		}

		specificTypeItemHUD.gameObject.SetActive (true);

	}

	public void OnItemButtonOfSpecificItemPlaneClick(Item item,int currentSelectEquipIndex){

		SetUpItemDetailHUD (item);

	}


	public void OnEquipButtonOfDetailHUDClick(){

		OnQuitSpecificTypePlane ();

		SetUpPlayerStatusPlane ();

		SetUpEquipedItemPlane ();

		SetUpAllItemsPlane ();

	}


	public void OnResolveButtonOfDetailHUDClick(){

		OnQuitResolveCountHUD ();

		OnQuitItemDetailHUD ();

		SetUpPlayerStatusPlane ();

		SetUpEquipedItemPlane ();

		SetUpAllItemsPlane ();

	}

	public void OnQuitResolveCountHUD(){

		resolveCountHUD.gameObject.SetActive (false);

	}

	public void OnItemButtonClick(int index){

		if (index >= player.allItems.Count) {
			return;
		}

		Item item = player.allItems [index];

		for(int i = 0;i<allItemsBtns.Length;i++){
			Button btn = allItemsBtns [i];
			btn.transform.FindChild ("SelectedBorder").GetComponent<Image> ().enabled = i == index;
		}
			



		SetUpItemDetailHUD (item);

	}



	// 关闭物品详细说明HUD
	public void OnQuitItemDetailHUD(){
		
		itemDetailHUD.gameObject.SetActive (false);

		choicePanelWithOneBtn.gameObject.SetActive (false);
		choicePanelWithTwoBtns.gameObject.SetActive (false);

	}

	// 关闭更换物品的界面
	public void OnQuitSpecificTypePlane(){

		specificTypeItemHUD.gameObject.SetActive (false);

		for (int i = 0; i < itemDetailContainer.childCount; i++) {
			Transform trans = itemDetailContainer.GetChild (i);
			trans.GetComponent<ItemDetailView> ().ResetItemDetail ();
		}

		itemDetailsPool.AddChildInstancesToPool (itemDetailContainer);

	}

	// 关闭背包界面
	public void OnQuitBagPlane(CallBack cb){
		bagViewContainer.GetComponent<Image> ().color = new Color (0, 0, 0, 0);
		bagPlane.transform.DOLocalMoveY (-Screen.height, 0.5f).OnComplete (() => {
			cb();
		});

	}


}
