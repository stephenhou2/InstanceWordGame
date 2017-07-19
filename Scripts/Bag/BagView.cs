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



	public GameObject propertyText;
	public GameObject itemButton;


	private Player player;

	private List<Sprite> sprites = new List<Sprite> ();

	public GameObject bagPlane;
	public GameObject specificTypeItemPlane;

	public GameObject itemDetailHUD;

	public Transform itemPropertiesPlane;
	public Image itemIcon;
	public Text itemName;
	public Button equipButton;
	public Button resolveButton;

	public Transform specificTypeItemsGridPlane;


//	public InstancePool instancePool;

	private InstancePool propertyTextPool;

	private InstancePool itemButtonPool;




	public void SetUpBagView(){

		this.sprites = GameManager.Instance.allItemSprites;
		this.player = Player.mainPlayer;

		SetUpPlayerStatusPlane ();

		SetUpEquipedItemPlane ();

		SetUpAllItemsPlane ();

		this.GetComponent<Canvas> ().enabled = true;

	}


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

	// 初始化背包物品界面
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

				extraInfo.text = string.Empty;

			}
		}


	}

	private void SetUpItemDetailHUD(Item item,bool isEquipButton,bool isResolveButton){

		itemDetailHUD.SetActive (true);

		if (isEquipButton) {
			equipButton.gameObject.SetActive (true);
		} else if(isResolveButton){
			resolveButton.gameObject.SetActive (true);
		}

		itemIcon.sprite = sprites.Find (delegate(Sprite obj) {
			return obj.name == item.spriteName;
		});
		itemIcon.GetComponent<Image> ().enabled = true;

		itemName.text = item.itemName;

		CompareWithCurrentEquipedItem (item);

	}

	private void SetUpItemButton(Item item,Button btn){

		if (item == null || item.itemName == null) {
			Image image = btn.transform.FindChild ("ItemIcon").GetComponent<Image>();
			image.enabled = false;
			image.sprite = null;
		}else if (item != null && item.itemName != null) {
			Image image = btn.transform.FindChild ("ItemIcon").GetComponent<Image>();
			image.enabled = true;
			image.sprite = sprites.Find (delegate(Sprite obj) {
				return obj.name == item.spriteName;
			});
		}
	}

	// 更换装备／物品的方法
	public void OnEquipedItemButtonsClick(ItemType type,List<Item> allItemsOfCurrentSelectType){

		itemButtonPool = InstancePool.GetOrCreateInstancePool ("ItemButtonPool");

		List<Button> allCurrentTypeItemBtns = new List<Button> ();

		for(int i =0;i<allItemsOfCurrentSelectType.Count;i++){
			
			Item item = allItemsOfCurrentSelectType[i];

			Button itemBtn = itemButtonPool.GetInstance<Button> (itemButton, specificTypeItemsGridPlane);

			itemBtn.transform.SetParent (specificTypeItemsGridPlane);

			allCurrentTypeItemBtns.Add (itemBtn);

			SetUpItemButton (item, itemBtn);
		}

		BagViewController bagViewCtr = GetComponent<BagViewController> ();

		for(int i = 0;i < allCurrentTypeItemBtns.Count;i++){
			int btnIndex = i;
			Button itemBtn = allCurrentTypeItemBtns [i];
			itemBtn.onClick.RemoveAllListeners ();
			itemBtn.onClick.AddListener (delegate {
				bagViewCtr.OnItemButtonOfSpecificItemPlaneClick(btnIndex);
			});
				

		}

		specificTypeItemPlane.SetActive (true);

	}

	public void OnItemButtonOfSpecificItemPlaneClick(Item item,int currentSelectEquipIndex){

		SetUpItemDetailHUD (item,true,false);

	}

	public void OnCreateButtonOfDetailHUDClick(){

		OnQuitSpecificTypePlane ();

		OnQuitItemDetailHUD ();

		SetUpPlayerStatusPlane ();

		SetUpEquipedItemPlane ();

		SetUpAllItemsPlane ();

	}

	public void OnResolveButtonOfDetailHUDClick(){

		OnQuitItemDetailHUD ();

		SetUpPlayerStatusPlane ();

		SetUpEquipedItemPlane ();

		SetUpAllItemsPlane ();

	}

	public void OnItemButtonClick(int index){


		for(int i = 0;i<allItemsBtns.Length;i++){
			Button btn = allItemsBtns [i];
			btn.transform.FindChild ("SelectedBorder").GetComponent<Image> ().enabled = i == index;
		}
			

		Item item = player.allItems [index];

		bool canResolve = item.itemType != ItemType.Consumables;

		SetUpItemDetailHUD (item,false,canResolve);

	}

	private void CompareWithCurrentEquipedItem(Item item){

		Item currentEquipedWAS = null;

		switch (item.itemType) {
		case ItemType.Weapon:
			currentEquipedWAS = player.allEquipedItems [0];
			break;
		case ItemType.Amour:
			currentEquipedWAS = player.allEquipedItems [1];
			break;
		case ItemType.Shoes:
			currentEquipedWAS = player.allEquipedItems [2];
			break;
		default:
			break;
		}
		if (currentEquipedWAS == null) {


			if (item.healthGain != 0) {
				AddPropertyText (item.healthGain, PropertyType.Health);
			}
			if (item.strengthGain != 0) {
				AddPropertyText (item.strengthGain, PropertyType.Strength);
			}

			return;

		}


		if (item.attackGain != 0 || currentEquipedWAS.attackGain != 0) {
			int compare = item.attackGain - currentEquipedWAS.attackGain;
			AddPropertyText (compare, PropertyType.Attack);
		}
		if (item.magicGain != 0 || currentEquipedWAS.magicGain != 0) {
			int compare = item.magicGain - currentEquipedWAS.magicGain;
			AddPropertyText (compare, PropertyType.Magic);
		}
		if (item.amourGain != 0 || currentEquipedWAS.amourGain != 0) {
			int compare = item.amourGain - currentEquipedWAS.amourGain;
			AddPropertyText (compare, PropertyType.Amour);
		}
		if (item.magicResistGain != 0 || currentEquipedWAS.magicResistGain != 0) {
			int compare = item.magicResistGain - currentEquipedWAS.magicResistGain;
			AddPropertyText (compare, PropertyType.MagicResist);
		}
		if (item.critGain != 0 || currentEquipedWAS.critGain != 0) {
			int compare = item.critGain - currentEquipedWAS.critGain;
			AddPropertyText (compare, PropertyType.Crit);
		}
		if (item.agilityGain != 0 || currentEquipedWAS.agilityGain != 0) {
			int compare = item.agilityGain - currentEquipedWAS.agilityGain;
			AddPropertyText (compare, PropertyType.Agility);
		}



	}
		

	private void AddPropertyText(int gain,PropertyType type){

		propertyTextPool = InstancePool.GetOrCreateInstancePool ("PropertyTextPool");

		Text newPropertyText = propertyTextPool.GetInstance<Text> (propertyText, itemPropertiesPlane);

		string preText = null;

		string linkSymbol = gain < 0 ? "-" : "+";

		string colorText = gain < 0 ? "<color=red>" : "<color=green>";

		switch (type) {
		case PropertyType.Attack:
			preText = "攻击";
			break;
		case PropertyType.Magic:
			preText = "魔法";
			break;
		case PropertyType.Amour:
			preText = "护甲";
			break;
		case PropertyType.MagicResist:
			preText = "抗性";
			break;
		case PropertyType.Crit:
			preText = "暴击";
			break;
		case PropertyType.Agility:
			preText = "闪避";
			break;
		case PropertyType.Health:
			preText = "血量";
			break;
		case PropertyType.Strength:
			preText = "气力";
			break;
		default:
			break;
		}



		newPropertyText.text = preText + colorText + linkSymbol + Mathf.Abs(gain).ToString() + "</color>";

	}




	// 关闭物品详细说明HUD
	public void OnQuitItemDetailHUD(){
		
		itemDetailHUD.SetActive (false);

		equipButton.gameObject.SetActive (false);
		resolveButton.gameObject.SetActive (false);


		while (itemPropertiesPlane.transform.childCount > 0) {
			Transform trans = itemPropertiesPlane.transform.GetChild (0);
			propertyTextPool.AddInstanceToPool (trans.gameObject,"PropertyTextPool");
		}
	}

	// 关闭更换物品的界面
	public void OnQuitSpecificTypePlane(){

		itemDetailHUD.SetActive (false);

		specificTypeItemPlane.SetActive (false);

		while (specificTypeItemsGridPlane.transform.childCount > 0) {
			Transform trans = specificTypeItemsGridPlane.transform.GetChild (0);
			itemButtonPool.AddInstanceToPool (trans.gameObject, "ItemButtonPool");
		}
	}

	// 关闭背包界面
	public void OnQuitBagPlane(CallBack cb){
		bagPlane.transform.DOLocalMoveY (-Screen.height, 0.5f).OnComplete (() => {
			cb();
		});

	}




}
