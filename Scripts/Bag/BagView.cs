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
		public Text armorText;
		public Text manaResistText;
		public Text critText;
		public Text dodgeText;

		public Button[] bagTypeButtons;
		public Button[] allEquipedEquipmentsButtons;

		private InstancePool itemDisplayButtonsPool;
		public Transform itemDisplayButtonsContainer;
		private Transform itemDisplayButtonModel;



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


		void Awake(){
			itemDisplayButtonsPool = InstancePool.GetOrCreateInstancePool ("ItemDisplayButtonsPool");
			itemDisplayButtonModel = TransformManager.FindTransform ("ItemDisplayButtonModel");
		}


		/// <summary>
		/// 初始化背包界面
		/// </summary>
		public void SetUpBagView(){

			this.sprites = GameManager.Instance.dataCenter.allItemSprites;
			this.player = Player.mainPlayer;

//			typeBtnNormalSprite = GameManager.Instance.dataCenter.allUIIcons.Find (delegate(Sprite obj) {
//				return obj.name == "typeButtonNormal";
//			});
//
//			typeBtnSelectedSprite = GameManager.Instance.dataCenter.allUIIcons.Find (delegate(Sprite obj) {
//				return obj.name == "typeButtonSelected";
//			});

			itemDetailsPool = InstancePool.GetOrCreateInstancePool ("ItemDetailsPool");

			SetUpPlayerStatusPlane ();

			SetUpEquipedItemPlane ();

			SetUpItemsDiaplayPlane (player.allEquipmentsInBag);

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
			armorText.text = "护甲:" + player.armor.ToString();
			manaResistText.text = "抗性:" + player.manaResist.ToString();
			critText.text = "暴击:" + (player.crit / (1 + 0.01f * player.crit)).ToString("F0") + "%";
			dodgeText.text = "闪避:" + (player.dodge / (1 + 0.01f * player.dodge)).ToString("F0") + "%";

		}

		/// <summary>
		/// 初始化已装备物品界面
		/// </summary>
		private void SetUpEquipedItemPlane(){
			
			for(int i = 0;i<player.allEquipedEquipments.Count;i++){
				
				Equipment equipment = player.allEquipedEquipments[i];

				int buttonIndex = (int)(equipment.equipmentType);

				SetUpItemButton (equipment, allEquipedEquipmentsButtons[buttonIndex]);
			}

		}
			
		/// <summary>
		/// 初始化背包物品界面
		/// </summary>
		public void SetUpItemsDiaplayPlane<T>(List<T> items)
			where T:Item
		{

			itemDisplayButtonsPool.AddChildInstancesToPool (itemDisplayButtonsContainer);

			for (int i = 0; i < items.Count; i++) {

				Button itemDisplayButton = itemDisplayButtonsPool.GetInstance<Button> (itemDisplayButtonModel.gameObject, itemDisplayButtonsContainer);

				Item item = items [i] as Item;

				Text extraInfo = itemDisplayButton.transform.Find ("ExtraInfo").GetComponent<Text> ();

				SetUpItemButton (item, itemDisplayButton);

				if (item.itemType == ItemType.Equipment && (item as Equipment).equiped) {
					extraInfo.text = "<color=green>已装备</color>";
				} else if (item.itemType == ItemType.Consumables || item.itemType == ItemType.Material) {
					extraInfo.text = item.itemCount.ToString ();
				} else {
					extraInfo.text = string.Empty;
				}
			}

		}

		public void SetUpItemsDiaplayPlane(List<Material> materials){
			
			itemDisplayButtonsPool.AddChildInstancesToPool (itemDisplayButtonsContainer);

		}


		/// <summary>
		/// 初始化物品详细介绍页面
		/// </summary>
		/// <param name="item">Item.</param>
		private void SetUpItemDetailHUD(Item item){

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

				Equipment currentEquipment = null;

				int index = (int)(equipment.equipmentType);

				currentEquipment = player.allEquipedEquipments [index] as Equipment;

				if (currentEquipment != null) {
					itemPropertiesText.text = equipment.GetComparePropertiesStringWithItem (currentEquipment);
				} else {
					itemPropertiesText.text = equipment.GetItemBasePropertiesString ();
				}


			} 
			#warning 不是装备的情况后面再补充一下
			// 如果不是装备
			else{
				
				itemPropertiesText.text = item.GetItemBasePropertiesString ();
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
				Sprite arrowSprite = GameManager.Instance.dataCenter.allUIIcons.Find (delegate(Sprite obj) {
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

		}


		public void OnResolveButtonOfDetailHUDClick(){

//			OnQuitResolveCountHUD ();
//
//			OnQuitItemDetailHUD ();
//
//			SetUpPlayerStatusPlane ();
//
//			SetUpEquipedItemPlane ();
//
//			SetUpAllItemsPlane ();

		}

		public void OnQuitResolveCountHUD(){

			resolveCountHUD.gameObject.SetActive (false);

		}

		public void OnItemButtonClick(int index){

//			if (index >= player.allItems.Count) {
//				return;
//			}
//
//				Item item = player.allItems [index];
//
//			for(int i = 0;i<allItemsBtns.Length;i++){
//				Button btn = allItemsBtns [i];
//				btn.transform.Find ("SelectedBorder").GetComponent<Image> ().enabled = i == index;
//			}
//
//			SetUpItemDetailHUD (item);

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
