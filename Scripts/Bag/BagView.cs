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

	public Transform itemPropertiesPlane;
	public Image itemIcon;
	public Text itemName;
	public GameObject propertyText;

	private Player player;

	private List<Sprite> sprites = new List<Sprite> ();

	public GameObject bagPlane;
	public GameObject specificTypeItemPlane;
	public GameObject itemDetailHUD;

	public Transform specificTypeItemsGridPlane;
	public GameObject itemButton;


//	public InstancePool instancePool;

	private InstancePool propertyTextPool;

	private InstancePool itemButtonPool;




	public void SetUpBagView(List<Sprite> sprites){

		this.sprites = sprites;
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

		attackText.text = "基础攻击:" + player.attack.ToString ();
		magicText.text = "基础魔法:" + player.magic.ToString ();
		amourText.text = "基础护甲:" + player.amour.ToString();
		magicResistText.text = "基础抗性:" + player.magicResist.ToString();
		critText.text = "基础暴击:" + (player.crit / (1 + 0.01f * player.crit)).ToString("F0") + "%";
		agilityText.text = "基础闪避:" + (player.agility / (1 + 0.01f * player.agility)).ToString("F0") + "%";

	}

	private void SetUpEquipedItemPlane(){

		for(int i = 0;i<player.allEquipedItems.Count;i++){
			Item item = player.allEquipedItems[i];
			SetUpItemButton (item, allEquipedItemBtns [i]);
		}

	}


	#warning 现在用所有物品做测试，后面按照类型进行分
	private void SetUpAllItemsPlane(){
		for (int i = 0; i < player.allItems.Count; i++) {
			Button itemBtn = allItemsBtns [i];
			SetUpItemButton (player.allItems [i], itemBtn);
			Text itemCount = allItemsBtns [i].transform.FindChild ("ItemCount").GetComponent<Text> ();
			itemCount.text = player.allItems [i].itemCount.ToString();
		}


	}

	private void SetUpItemButton(Item item,Button btn){
		
		if (item != null && item.itemName != null) {
			Image image = btn.transform.FindChild ("ItemIcon").GetComponent<Image>();
			image.enabled = true;
			image.sprite = sprites.Find (delegate(Sprite obj) {
				return obj.name == item.spriteName;
			});
			if (btn.transform.FindChild ("ItemCount") != null) {
				Text itemCountText = btn.transform.FindChild ("ItemCount").GetComponent<Text> ();
				itemCountText.text = item.itemCount.ToString ();
			}
		}
	}


	public void OnEquipedItemButtonsClick(ItemType type,List<Item> allItemsOfCurrentSelectType){

//		if (itemButtonPool == null) {
//			itemButtonPool = Instantiate (instancePool,TransformManager.FindTransform (CommonData.poolContainerName));
//			itemButtonPool.name = "ItemButtonPool";
//		}

		itemButtonPool = InstancePool.GetOrCreatInstancePool ("ItemButtonPool");

		List<Button> allCurrentTypeItemBtns = new List<Button> ();

		for(int i =0;i<allItemsOfCurrentSelectType.Count;i++){
			
			Item item = allItemsOfCurrentSelectType[i];

			Button itemBtn = itemButtonPool.GetInstance<Button> (itemButton, specificTypeItemsGridPlane);

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

		OnQuitSpecificTypePlane ();

		Button btn = allEquipedItemBtns [currentSelectEquipIndex];

		SetUpItemButton (item, btn);

	}

	public void OnItemButtonClick(int index){

		itemDetailHUD.SetActive (true);

		for(int i = 0;i<allItemsBtns.Length;i++){
			Button btn = allItemsBtns [i];
			btn.transform.FindChild ("SelectedBorder").GetComponent<Image> ().enabled = i == index;
		}



		Item item = player.allItems [index];

		itemIcon.sprite = sprites.Find (delegate(Sprite obj) {
			return obj.name == item.spriteName;
		});
		itemIcon.GetComponent<Image> ().enabled = true;

		itemName.text = item.itemName;

		if (item.attackGain != 0) {
			AddPropertyText (item.attackGain, PropertyType.Attack);
		} else if (item.magicGain != 0) {
			AddPropertyText (item.magicGain, PropertyType.Magic);
		} else if (item.amourGain != 0) {
			AddPropertyText (item.amourGain, PropertyType.Amour);
		} else if (item.magicResistGain != 0) {
			AddPropertyText (item.magicResistGain, PropertyType.MagicResist);
		} else if (item.critGain != 0) {
			AddPropertyText (item.critGain, PropertyType.Crit);
		} else if (item.agilityGain != 0) {
			AddPropertyText (item.agilityGain, PropertyType.Agility);
		} else if (item.healthGain != 0) {
			AddPropertyText (item.healthGain, PropertyType.Health);
		} else if (item.strengGain != 0) {
			AddPropertyText (item.strengGain, PropertyType.Strength);
		}

	}

	private void AddPropertyText(int gain,PropertyType type){

//		if (propertyTextPool == null) {
//			propertyTextPool = Instantiate (instancePool, TransformManager.FindTransform(CommonData.poolContainerName));
//			propertyTextPool.name = "PropertyTextPool";
//		}

		propertyTextPool = InstancePool.GetOrCreatInstancePool ("PropertyTextPool");

		Text newPropertyText = propertyTextPool.GetInstance<Text> (propertyText, itemPropertiesPlane);

		string preText = null;

		switch (type) {
		case PropertyType.Attack:
			preText = "攻击 + ";
			break;
		case PropertyType.Magic:
			preText = "魔法 + ";
			break;
		case PropertyType.Amour:
			preText = "护甲 + ";
			break;
		case PropertyType.MagicResist:
			preText = "抗性 + ";
			break;
		case PropertyType.Crit:
			preText = "暴击 + ";
			break;
		case PropertyType.Agility:
			preText = "闪避 + ";
			break;
		case PropertyType.Health:
			preText = "血量 + ";
			break;
		case PropertyType.Strength:
			preText = "气力 + ";
			break;
		default:
			break;
		}

		newPropertyText.text = preText + gain.ToString ();

	}




	// 关闭物品详细说明HUD
	public void OnQuitItemDetailHUD(){
		itemDetailHUD.SetActive (false);
		while (itemPropertiesPlane.transform.childCount > 0) {
			Transform trans = itemPropertiesPlane.transform.GetChild (0);
			propertyTextPool.AddInstanceToPool (trans.gameObject,"PropertyTextPool");
		}
	}

	// 关闭更换物品的界面
	public void OnQuitSpecificTypePlane(){
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
