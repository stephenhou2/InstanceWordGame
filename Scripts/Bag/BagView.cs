using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;



namespace WordJourney
{

	public class BagView : MonoBehaviour {

		public Transform bagViewContainer;
		public GameObject bagPlane;

		public Transform playerPropertiesPlane;
		public Transform playerStatusPlane;

		public Button[] bagTypeButtons;
		public Button[] allEquipedEquipmentsButtons;

		private InstancePool itemDisplayButtonsPool;
		public Transform itemDisplayButtonsContainer;
		private Transform itemDisplayButtonModel;

		private Player player;

		private List<Sprite> sprites = new List<Sprite> ();

		public Transform itemDetailHUD;


		public Transform specificTypeItemHUD;
		public Transform specificTypeItemsScrollView;
		public Transform specificTypeItemDetailsContainer;

		private Transform itemDetailsModel;
		private InstancePool itemDetailsPool;

		public Transform resolveCountHUD;
		public Button minusBtn;
		public Button plusBtn;
		public Slider resolveCountSlider;
		public Text resolveCount;

		private float itemDetailModelHeight;
		private float paddingY;
		public int maxPreloadCount = 6;

		private int currentMinItemIndex;
		private int currentMaxItemIndex;

//		private float scrollViewLastPosY;
		private bool changeScrollViewPos;

		private List<Equipment> allEquipmentsOfCurrentSelectTypeInBag;
		private Equipment compareEquipment;


		/// <summary>
		/// 初始化背包界面
		/// </summary>
		public void SetUpBagView(){
			
			this.sprites = GameManager.Instance.dataCenter.allItemSprites;
			this.player = Player.mainPlayer;

			itemDisplayButtonsPool = InstancePool.GetOrCreateInstancePool ("ItemDisplayButtonsPool");
			itemDetailsPool = InstancePool.GetOrCreateInstancePool ("ItemDetailsPool");

			itemDisplayButtonModel = TransformManager.FindTransform ("ItemDisplayButtonModel");
			itemDetailsModel = TransformManager.FindTransform ("ItemDetailsModel");

			itemDetailModelHeight = (itemDetailsModel as RectTransform).rect.height;

			VerticalLayoutGroup vLayoutGroup = specificTypeItemDetailsContainer.GetComponent<VerticalLayoutGroup> ();

			paddingY = vLayoutGroup.padding.top;


			SetUpPlayerStatusPlane ();

			SetUpEquipedEquipmentsPlane ();

			SetUpItemsDiaplayPlane (player.allEquipmentsInBag);

			this.GetComponent<Canvas> ().enabled = true;

		}


		/// <summary>
		/// 初始化玩家属性界面
		/// </summary>
		private void SetUpPlayerStatusPlane(){

			Slider healthBar = playerStatusPlane.Find ("HealthBar").GetComponent<Slider> ();
			Slider manaBar = playerStatusPlane.Find ("ManaBar").GetComponent<Slider> ();

			Image playerIcon = playerStatusPlane.Find ("PlayerIcon").GetComponent<Image> ();

			Text healthText = healthBar.transform.Find ("HealthText").GetComponent<Text> ();
			Text manaText = manaBar.transform.Find ("ManaText").GetComponent<Text> ();

			Text playerLevel = playerPropertiesPlane.Find("PlayerLevel").GetComponent<Text>();
			Text playerAttack = playerPropertiesPlane.Find ("Attack").GetComponent<Text> ();
			Text playerAttackSpeed = playerPropertiesPlane.Find ("AttackSpeed").GetComponent<Text> ();
			Text playerArmor = playerPropertiesPlane.Find ("Armor").GetComponent<Text> ();
			Text playerManaResist = playerPropertiesPlane.Find ("ManaResist").GetComponent<Text> ();
			Text playerDodge = playerPropertiesPlane.Find ("Dodge").GetComponent<Text> ();
			Text playerCrit = playerPropertiesPlane.Find ("Crit").GetComponent<Text> ();

			healthBar.maxValue = player.maxHealth;
			healthBar.value = player.health;

			manaBar.maxValue = player.maxMana;
			manaBar.value = player.mana;

			healthText.text = player.health.ToString () + "/" + player.maxHealth.ToString ();
			manaText.text = player.mana.ToString () + "/" + player.maxMana.ToString ();

			playerLevel.text = "Lv." + player.agentLevel;

			playerAttack.text = "攻击:" + player.attack.ToString ();
			playerAttackSpeed.text = "攻速:" + player.attackSpeed.ToString ();
			playerArmor.text = "护甲:" + player.armor.ToString();
			playerManaResist.text = "抗性:" + player.manaResist.ToString();
			playerDodge.text = "闪避:" + player.dodge.ToString();
			playerCrit.text = "暴击:" + player.crit.ToString ();
		}

		/// <summary>
		/// 初始化已装备物品界面
		/// </summary>
		private void SetUpEquipedEquipmentsPlane(){
			
			for(int i = 0;i<player.allEquipedEquipments.Count;i++){
				
				Equipment equipment = player.allEquipedEquipments[i];

				int buttonIndex = (int)(equipment.equipmentType);

				Button equipedEquipmentButton = allEquipedEquipmentsButtons[buttonIndex];

				Image itemIcon = equipedEquipmentButton.transform.Find ("ItemIcon").GetComponent<Image> ();

				Sprite s = GameManager.Instance.dataCenter.allItemSprites.Find (delegate(Sprite obj) {
					return obj.name == equipment.spriteName;
				});

				if (s != null) {
					itemIcon.sprite = s;
					itemIcon.enabled = true;
				}

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

				SetUpItemButton (item, itemDisplayButton);


			}

		}


		/// <summary>
		/// 初始化物品详细介绍页面
		/// </summary>
		/// <param name="item">Item.</param>
		private void SetUpItemDetailHUD(Item item){



			Transform itemDetailsContainer = itemDetailHUD.Find ("ItemDetailsContainer");
			Text itemName = itemDetailsContainer.Find("ItemName").GetComponent<Text>();
			Text itemType = itemDetailsContainer.Find("ItemType").GetComponent<Text>();
			Text itemDamagePercentage = itemDetailsContainer.Find("ItemDamagePercentage").GetComponent<Text>();
			Text itemProperties = itemDetailsContainer.Find("ItemProperties").GetComponent<Text>();
			Image itemIcon = itemDetailsContainer.Find("ItemIcon").GetComponent<Image>();

			Transform choiceHUDWithOneBtn = itemDetailsContainer.Find("ChoiceHUDWithOneBtn");
			Transform choiceHUDWithTwoBtns = itemDetailsContainer.Find("ChoiceHUDWithTwoBtns");

			itemIcon.sprite = sprites.Find (delegate(Sprite obj) {
				return obj.name == item.spriteName;
			});
			itemIcon.enabled = true;

			itemName.text = item.itemName;

			itemType.text = item.GetItemTypeString ();

			// 如果物品是装备
			if (item is Equipment) {

				Equipment equipment = item as Equipment;
				
				choiceHUDWithTwoBtns.gameObject.SetActive (true);

//				Equipment currentEquipment = null;
//
//				int index = (int)(equipment.equipmentType);

				Equipment currentEquipment = player.allEquipedEquipments.Find (delegate(Equipment obj) {
					return obj.equipmentType == equipment.equipmentType;
				});

				if (currentEquipment != null) {
					itemProperties.text = equipment.GetComparePropertiesStringWithItem (currentEquipment);
				} else {
					itemProperties.text = equipment.GetItemBasePropertiesString ();
				}


			} 
			#warning 不是装备的情况后面再补充一下
			// 如果不是装备
			else{
				
				itemProperties.text = item.GetItemBasePropertiesString ();
				choiceHUDWithOneBtn.gameObject.SetActive (true);

			}

			itemDetailHUD.gameObject.SetActive (true);
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

			Text itemName = btn.transform.Find ("ItemName").GetComponent<Text> ();
			Text extraInfo = btn.transform.Find ("ExtraInfo").GetComponent<Text> ();
			Image itemIcon = btn.transform.Find ("ItemIcon").GetComponent<Image>();

			itemName.text = item.itemName;

			if (item.itemType == ItemType.Equipment && (item as Equipment).equiped) {
				extraInfo.text = "<color=green>已装备</color>";
			} else if (item.itemType == ItemType.Consumables || item.itemType == ItemType.Material) {
				extraInfo.text = item.itemCount.ToString ();
			} else {
				extraInfo.text = string.Empty;
			}

			itemIcon.sprite = sprites.Find (delegate(Sprite obj) {
				return obj.name == item.spriteName;
			});

			itemIcon.enabled = itemIcon.sprite != null;

			btn.onClick.RemoveAllListeners ();

			btn.onClick.AddListener (delegate {

				for (int i = 0; i < itemDisplayButtonsContainer.childCount; i++) {

					Transform itemDisplayButtonTrans = itemDisplayButtonsContainer.GetChild (i);

					if (itemDisplayButtonTrans != btn.transform) {
						itemDisplayButtonTrans.Find ("SelectBorder").GetComponent<Image> ().enabled = false;
					} else {
						Image selectBorder = itemDisplayButtonTrans.Find ("SelectBorder").GetComponent<Image> ();
						selectBorder.enabled = !selectBorder.enabled;
						GetComponent<BagViewController> ().SelectItem (item);
						SetUpItemDetailHUD (item);
					}

				}

			});

		}


		/// <summary>
		/// 更换装备／物品的方法
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="allItemsOfCurrentSelectType">All items of current select type.</param>
		public void SetUpAllEquipmentsPlaneOfEquipmentType(Equipment equipedEquipment,List<Equipment> allEquipmentsOfCurrentSelectTypeInBag){

			this.compareEquipment = equipedEquipment;

			this.allEquipmentsOfCurrentSelectTypeInBag = allEquipmentsOfCurrentSelectTypeInBag;

			itemDetailsPool.AddChildInstancesToPool (specificTypeItemDetailsContainer);

			specificTypeItemDetailsContainer.localPosition = new Vector3 (specificTypeItemDetailsContainer.localPosition.x, 0, 0);

			int maxCount = maxPreloadCount < allEquipmentsOfCurrentSelectTypeInBag.Count ? maxPreloadCount : allEquipmentsOfCurrentSelectTypeInBag.Count;

			for(int i =0;i<maxPreloadCount;i++){
				
				Equipment equipmentInBag = allEquipmentsOfCurrentSelectTypeInBag[i];

				Transform itemDetail = itemDetailsPool.GetInstance<Transform> (itemDetailsModel.gameObject,specificTypeItemDetailsContainer);

				itemDetail.GetComponent<ItemDetailView>().SetUpItemDetailView(equipedEquipment,equipmentInBag,GetComponent<BagViewController> ());
			}

			currentMinItemIndex = 0;
			currentMaxItemIndex = maxCount < 1 ? 0 : maxCount - 1;

			specificTypeItemHUD.gameObject.SetActive (true);

		}

		private int count;
		private bool isDragging;

		private List<Transform> itemDetailsInvisible = new List<Transform>();

		public void OnBeginDrag(){
			count = 0;
			isDragging = true;
		}

		public void OnEndDrag(){

			int cellCount = itemDetailsInvisible.Count;

			for (int i = 0; i < itemDetailsInvisible.Count; i++) {
				itemDetailsPool.AddInstanceToPool (itemDetailsInvisible [i].gameObject);
			}

			float newPosY = specificTypeItemDetailsContainer.localPosition.y - cellCount * (itemDetailModelHeight + paddingY);

			Debug.Log (newPosY);

			specificTypeItemDetailsContainer.localPosition = new Vector3 (0, newPosY, 0);


			itemDetailsInvisible.Clear ();

			isDragging = false;
		}


		public void OnValueChanged(){

			float scrollRectPosY = specificTypeItemDetailsContainer.localPosition.y;
			float velocityY = specificTypeItemsScrollView.GetComponent<ScrollRect> ().velocity.y;

			if (isDragging) {

				int delta = (int)((scrollRectPosY - paddingY) / (itemDetailModelHeight + paddingY));

				if (delta > count) {
					Transform itemDetail = itemDetailsPool.GetInstance<Transform> (itemDetailsModel.gameObject, specificTypeItemDetailsContainer);
					Equipment equipmentInBag = allEquipmentsOfCurrentSelectTypeInBag [currentMaxItemIndex + 1];
					itemDetail.GetComponent<ItemDetailView> ().SetUpItemDetailView (compareEquipment, equipmentInBag, GetComponent<BagViewController> ());
					itemDetailsInvisible.Add (specificTypeItemDetailsContainer.GetChild (count));
					count++;
				}

				return;

			}

			// 向下滚动
			if (velocityY > 0) {

				if (currentMaxItemIndex >= allEquipmentsOfCurrentSelectTypeInBag.Count - 1) {
					Debug.Log ("所有物品加载完毕");
					return;
				}

				int minVisibleCellIndex = (int)(scrollRectPosY / (itemDetailModelHeight + paddingY));

				if (minVisibleCellIndex >= 1) {

				Transform itemDetail = specificTypeItemDetailsContainer.GetChild (0);

				itemDetail.SetAsLastSibling ();

				Equipment equipmentInBag = allEquipmentsOfCurrentSelectTypeInBag [currentMaxItemIndex + 1];

				itemDetail.GetComponent<ItemDetailView> ().SetUpItemDetailView (compareEquipment, equipmentInBag, GetComponent<BagViewController> ());

				float newPosY = specificTypeItemDetailsContainer.localPosition.y - itemDetailModelHeight - paddingY;

				specificTypeItemDetailsContainer.localPosition = new Vector3 (0, newPosY, 0);

				currentMaxItemIndex++;
				currentMinItemIndex++;

				} 

			}

		}
			

		public void OnScrollViewEndDrag(){

			if (currentMinItemIndex > 0) {

			}



		}


		public void OnItemButtonOfSpecificItemPlaneClick(Item item){

			SetUpItemDetailHUD (item);

		}

	

		public void OnEquipButtonOfDetailHUDClick(){

			OnQuitSpecificTypePlane ();

			SetUpPlayerStatusPlane ();

			SetUpEquipedEquipmentsPlane ();

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
					

		// 关闭物品详细说明HUD
		public void OnQuitItemDetailHUD(){
			
			itemDetailHUD.gameObject.SetActive (false);

			itemDetailHUD.Find ("ItemDetailsContainer/ChoiceHUDWithOneBtn").gameObject.SetActive (false);
			itemDetailHUD.Find ("ItemDetailsContainer/ChoiceHUDWithTwoBtns").gameObject.SetActive (false);

		}

		// 关闭更换物品的界面
		public void OnQuitSpecificTypePlane(){

			specificTypeItemHUD.gameObject.SetActive (false);

			for (int i = 0; i < specificTypeItemDetailsContainer.childCount; i++) {
				Transform trans = specificTypeItemDetailsContainer.GetChild (i);
				trans.GetComponent<ItemDetailView> ().ResetItemDetail ();
			}

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
