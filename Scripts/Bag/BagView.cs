using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;



namespace WordJourney
{

	public class BagView : MonoBehaviour {


		public Slider healthBar;
		public Slider manaBar;

		public Text healthText;
		public Text manaText;

		public Text playerLevelText;
		public Text progressText;

		public Text attackText;
		public Text attackSpeedText;
		public Text armourText;
		public Text manaResistText;
		public Text critText;
		public Text dodgeText;

	//	public Image weaponImage;
	//	public Image armourImage;
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

//		private Sprite typeBtnNormalSprite;
//		private Sprite typeBtnSelectedSprite;

		/// <summary>
		/// 初始化背包界面
		/// </summary>
		public void SetUpBagView(){

			this.sprites = GameManager.Instance.allItemSprites;
			this.player = Player.mainPlayer;

//			typeBtnNormalSprite = GameManager.Instance.allUIIcons.Find (delegate(Sprite obj) {
//				return obj.name == "typeButtonNormal";
//			});
//
//			typeBtnSelectedSprite = GameManager.Instance.allUIIcons.Find (delegate(Sprite obj) {
//				return obj.name == "typeButtonSelected";
//			});

			itemDetailsPool = InstancePool.GetOrCreateInstancePool ("ItemDetailsPool");

			SetUpPlayerStatusPlane ();

			SetUpEquipedItemPlane ();

			SetUpAllItemsPlane ();

			bagViewContainer.GetComponent<Image> ().color = new Color (0, 0, 0, 200);

			bagPlane.transform.localPosition = Vector3.zero;

			this.GetComponent<Canvas> ().enabled = true;

		}

		/// <summary>
		/// 初始化玩家属性界面
		/// </summary>
		private void SetUpPlayerStatusPlane(){

			healthBar.maxValue = player.maxHealth;
			healthBar.value = player.health;

			manaBar.maxValue = player.maxMana;
			manaBar.value = player.mana;

			healthText.text = player.health.ToString () + "/" + player.maxHealth.ToString ();
			manaText.text = player.mana.ToString () + "/" + player.maxMana.ToString ();

			playerLevelText.text = "Lv." + player.agentLevel;

			attackText.text = "攻击:" + player.attack.ToString ();
			attackSpeedText.text = "攻速:" + player.attackSpeed.ToString ();
			armourText.text = "护甲:" + player.armour.ToString();
			manaResistText.text = "抗性:" + player.manaResist.ToString();
			critText.text = "暴击:" + (player.crit / (1 + 0.01f * player.crit)).ToString("F0") + "%";
			dodgeText.text = "闪避:" + (player.dodge / (1 + 0.01f * player.dodge)).ToString("F0") + "%";

		}

		// 初始化已装备物品界面
		private void SetUpEquipedItemPlane(){

//			for (int i = 0; i < player.allEquipedEquipments.Length; i++) {
//				Equipment equipment = player.allEquipedEquipments [i];
//				SetUpItemButton (equipment, allEquipedItemBtns [i]);
//			}
			
			for(int i = 0;i<player.allEquipedEquipments.Count;i++){
				Item item = player.allEquipedEquipments[i];
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

				Text extraInfo = allItemsBtns [i].transform.Find ("ExtraInfo").GetComponent<Text> ();

				if (i < player.allItems.Count) {

					Item item = player.allItems [i];

					SetUpItemButton (player.allItems [i], itemBtn);

					if (item.itemType == ItemType.Equipment && (item as Equipment).equiped) {
						extraInfo.text = "<color=green>已装备</color>";
					} else if (item.itemType == ItemType.Consumables) {
						extraInfo.text = item.itemCount.ToString ();
					} else {
						extraInfo.text = string.Empty;
					}
				} else {
					
					SetUpItemButton (null, itemBtn);

					Image selectedBorder = itemBtn.transform.Find ("SelectedBorder").GetComponent<Image> ();
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

//			bool canStrengthen = item.CheckCanStrengthen();

			itemDetailHUD.gameObject.SetActive (true);


			itemIcon.sprite = sprites.Find (delegate(Sprite obj) {
				return obj.name == item.spriteName;
			});
			itemIcon.enabled = true;

			itemName.text = item.itemName;

			itemTypeText.text = item.GetItemTypeString ();

			// 如果物品是装备
			if (item is Equipment) {

				Equipment equipment = item as Equipment;
				
				choicePanelWithTwoBtns.gameObject.SetActive (true);

				itemQualityText.text = equipment.GetItemQualityString ();

				itemStrengthenTimesText.text = "已强化次数:" + (item as Equipment).strengthenTimes.ToString () + "次";

				Equipment currentEquipment = null;

				switch (equipment.equipmentType) {
				case EquipmentType.Weapon:
					currentEquipment = player.allEquipedEquipments [0] as Equipment;
					break;
				case EquipmentType.Armour:
					currentEquipment = player.allEquipedEquipments [1] as Equipment;
					break;
				case EquipmentType.Shoes:
					currentEquipment = player.allEquipedEquipments [2] as Equipment;
					break;
				}

				if (currentEquipment != null) {
					itemPropertiesText.text = equipment.GetComparePropertiesStringWithItem (currentEquipment);
				} else {
					itemPropertiesText.text = equipment.GetItemPropertiesString ();
				}


			} 
			#warning 不是装备的情况后面再补充一下
			// 如果不是装备
			else{
				
				itemPropertiesText.text = item.GetItemPropertiesString ();
				choicePanelWithOneBtn.gameObject.SetActive (true);

			}
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
				Image itemIcon = btn.transform.Find ("ItemIcon").GetComponent<Image>();
				itemIcon.enabled = false;
				itemIcon.sprite = null;

			}
			else if (item != null && item.itemName != null) {
	//			btn.interactable = (item != null);
				Image image = btn.transform.Find ("ItemIcon").GetComponent<Image>();
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
		public void OnEquipedItemButtonsClick(List<Item> allItemsOfCurrentSelectType){

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
				btn.transform.Find ("SelectedBorder").GetComponent<Image> ().enabled = i == index;
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

			float offsetY = GetComponent<CanvasScaler> ().referenceResolution.y;

			bagPlane.transform.DOLocalMoveY (-offsetY, 0.5f).OnComplete (() => {
				if(cb != null){
					cb();
				}
			});

		}

		}
}
