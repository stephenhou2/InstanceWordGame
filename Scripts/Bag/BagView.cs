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
		private int maxPreloadCountOfItem;

		private Player player;

		private List<Sprite> sprites = new List<Sprite> ();

		public Transform itemDetailHUD;


		public Transform specificTypeItemHUD;
		public Transform specificTypeItemsScrollView;
		public Transform specificTypeItemDetailsContainer;

		private Transform itemDetailsModel;
		private InstancePool itemDetailsPool;

		public Transform resolveCountHUD;
	
		public Transform resolveGainsHUD;
		public Transform resolveGainsContainer;

		private InstancePool resolveGainsPool;
		private Transform resolveGainModel;


		/// <summary>
		/// 初始化背包界面
		/// </summary>
		public void SetUpBagView(Item currentSelectItem){

			this.GetComponent<Canvas> ().enabled = true;

			//获取所有item的图片
			this.sprites = GameManager.Instance.gameDataCenter.allItemSprites;
			this.player = Player.mainPlayer;

			Transform poolContainerOfBagCanvas = TransformManager.FindOrCreateTransform (CommonData.poolContainerName + "/PoolContainerOfBagCanvas");
			Transform modelContainerOfBagCanvas = TransformManager.FindOrCreateTransform (CommonData.instanceContainerName + "/ModelContainerOfBagCanvas");

			if (poolContainerOfBagCanvas.childCount == 0) {
				//创建缓存池
				itemDisplayButtonsPool = InstancePool.GetOrCreateInstancePool ("ItemDisplayButtonsPool",poolContainerOfBagCanvas.name);
				itemDetailsPool = InstancePool.GetOrCreateInstancePool ("ItemDetailsPool",poolContainerOfBagCanvas.name);
				resolveGainsPool = InstancePool.GetOrCreateInstancePool ("ResolveGainsPool",poolContainerOfBagCanvas.name);

			}

			if (modelContainerOfBagCanvas.childCount == 0) {
				// 获取模型
				itemDisplayButtonModel = TransformManager.FindTransform ("ItemDisplayButtonModel");
				itemDetailsModel = TransformManager.FindTransform ("ItemDetailsModelInBagCanvas");
				resolveGainModel = TransformManager.FindTransform ("ResolveGainModel");

				itemDisplayButtonModel.SetParent (modelContainerOfBagCanvas);
				itemDetailsModel.SetParent (modelContainerOfBagCanvas);
				resolveGainModel.SetParent (modelContainerOfBagCanvas);
			}

			// 背包中单类物品最大预加载数量
			maxPreloadCountOfItem = 30;

			SetUpPlayerStatusPlane ();

			SetUpEquipedEquipmentsPlane ();

			SetUpItemsDiaplayPlane (player.allEquipmentsInBag,currentSelectItem);

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

				Sprite s = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
					return obj.name == equipment.spriteName;
				});

				if (s != null) {
					itemIcon.sprite = s;
					itemIcon.enabled = true;
				}

			}

		}
			
		private struct MyParams{
			public Item targetItem;
			public Item currentSelectItem;
			public Button itemDisplayButton;

			public MyParams(Item targetItem, Button itemDisplayButton, Item currentSelectItem){
				this.targetItem = targetItem;
				this.itemDisplayButton = itemDisplayButton;
				this.currentSelectItem = currentSelectItem;
			}
		}

		/// <summary>
		/// 初始化背包物品界面
		/// </summary>
		public void SetUpItemsDiaplayPlane<T>(List<T> items,Item currentSelectItem)
			where T:Item
		{

			itemDisplayButtonsPool.AddChildInstancesToPool (itemDisplayButtonsContainer);

			int loadCount = items.Count <= maxPreloadCountOfItem ? items.Count : maxPreloadCountOfItem;

			for (int i = 0; i < loadCount; i++) {

				Button itemDisplayButton = itemDisplayButtonsPool.GetInstance<Button> (itemDisplayButtonModel.gameObject, itemDisplayButtonsContainer);

				Item item = items [i] as Item;

				SetUpItemButton (item, itemDisplayButton, currentSelectItem);

			}

			if (loadCount < items.Count) {

				for (int i = maxPreloadCountOfItem; i < items.Count; i++) {

					Button itemDisplayButton = itemDisplayButtonsPool.GetInstance<Button> (itemDisplayButtonModel.gameObject, itemDisplayButtonsContainer);

					Item item = items [i] as Item;

					MyParams myParams = new MyParams (item, itemDisplayButton, currentSelectItem);

					StartCoroutine ("LoadItemDisplayButtonAsync", myParams);

				}

			}
				
		}

		private IEnumerator LoadItemDisplayButtonAsync(MyParams myParams){
			yield return null;
			SetUpItemButton (myParams.targetItem, myParams.itemDisplayButton, myParams.currentSelectItem);
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

			choiceHUDWithOneBtn.gameObject.SetActive (false);
			choiceHUDWithTwoBtns.gameObject.SetActive (false);

			itemIcon.sprite = sprites.Find (delegate(Sprite obj) {
				return obj.name == item.spriteName;
			});
			itemIcon.enabled = true;

			itemName.text = item.itemName;

			itemType.text = item.GetItemTypeString ();

			// 如果物品是装备
			switch(item.itemType){

			case ItemType.Equipment:

				Equipment equipment = item as Equipment;

				Equipment currentEquipment = player.allEquipedEquipments.Find (delegate(Equipment obj) {
					return obj.equipmentType == equipment.equipmentType;
				});

				if (currentEquipment != null) {
					itemProperties.text = equipment.GetComparePropertiesStringWithItem (currentEquipment);
				} else {
					itemProperties.text = equipment.GetItemBasePropertiesString ();
				}

				itemName.text = equipment.itemName;

				string colorText = string.Empty;

				float damagePercentage = (float)equipment.durability / equipment.maxDurability;

				if (damagePercentage <= 0.5f) {
					colorText = "red";
				} else if (damagePercentage <= 0.8f) {
					colorText = "orange";
				} else {
					colorText = "green";
				}

				itemDamagePercentage.text = string.Format("耐久度：<color={0}>{1}/{2}</color>",colorText, equipment.durability,equipment.maxDurability);

				choiceHUDWithTwoBtns.gameObject.SetActive (true);

				break;
			case ItemType.Consumables:
				itemProperties.text = item.GetItemBasePropertiesString ();
				itemName.text = item.itemName;
				itemDamagePercentage.text = string.Empty;
//				choiceHUDWithOneBtn.gameObject.SetActive (true);
				break;
			case ItemType.Material:
				itemProperties.text = item.GetItemBasePropertiesString ();
				itemName.text = item.itemName;
				itemDamagePercentage.text = string.Empty;
				choiceHUDWithOneBtn.gameObject.SetActive (true);
				break;
			case ItemType.FuseStone:
				itemProperties.text = item.GetItemBasePropertiesString ();
				itemName.text = item.itemName;
				itemDamagePercentage.text = string.Empty;
				choiceHUDWithOneBtn.gameObject.SetActive (true);
				break;
			case ItemType.Task:
				itemProperties.text = item.GetItemBasePropertiesString ();
				itemName.text = item.itemName;
				itemDamagePercentage.text = string.Empty;
				break;
			} 
			itemDetailHUD.gameObject.SetActive (true);
		}

		public void UpdateItemDetailHUDAfterFix(Equipment equipment){

			Transform itemDetailsContainer = itemDetailHUD.Find ("ItemDetailsContainer");

			Text itemDamagePercentage = itemDetailsContainer.Find("ItemDamagePercentage").GetComponent<Text>();

			string colorText = string.Empty;

			float damagePercentage = (float)equipment.durability / equipment.maxDurability;

			if (damagePercentage <= 0.5f) {
				colorText = "red";
			} else if (damagePercentage <= 0.8f) {
				colorText = "orange";
			} else {
				colorText = "green";
			}

			itemDamagePercentage.text = string.Format("损坏率：<color={0}>{1}/{2}</color>",colorText, equipment.durability,equipment.maxDurability);

			GetComponent<Canvas> ().enabled = true;

		}

		/// <summary>
		/// 初始化选择分解数量界面
		/// </summary>
		public void SetUpResolveCountHUD(int minValue,int maxValue){

			Transform resolveCountContainer = resolveCountHUD.Find ("ResolveCountContainer");

			Button minusBtn = resolveCountContainer.Find("MinusButton").GetComponent<Button>();
			Button plusBtn = resolveCountContainer.Find("PlusButton").GetComponent<Button>();
			Slider resolveCountSlider = resolveCountContainer.Find("ResolveCountSlider").GetComponent<Slider>();
			Text resolveCount = resolveCountContainer.Find("ResolveCount").GetComponent<Text>();


			resolveCountHUD.gameObject.SetActive (true);

			if (minusBtn.GetComponent<Image> ().sprite == null 
				|| plusBtn.GetComponent<Image>().sprite == null) 
			{
				Sprite arrowSprite = GameManager.Instance.gameDataCenter.allUISprites.Find (delegate(Sprite obj) {
					return obj.name == "arrowIcon";
				});

				minusBtn.GetComponent<Image> ().sprite = arrowSprite;
				plusBtn.GetComponent<Image> ().sprite = arrowSprite;
			}

			resolveCountSlider.minValue = minValue;
			resolveCountSlider.maxValue = maxValue;

			resolveCountSlider.value = minValue;
			resolveCount.text = "分解1个";


		}

		public int GetResolveCountBySlider(){
			
			Transform resolveCountContainer = resolveCountHUD.Find ("ResolveCountContainer");

			Slider resolveCountSlider = resolveCountContainer.Find("ResolveCountSlider").GetComponent<Slider>();

			return (int)resolveCountSlider.value;
		}

		public void UpdateResolveCountHUD(int count){

			Transform resolveCountContainer = resolveCountHUD.Find ("ResolveCountContainer");

			Slider resolveCountSlider = resolveCountContainer.Find("ResolveCountSlider").GetComponent<Slider>();

			Text resolveCount = resolveCountContainer.Find("ResolveCount").GetComponent<Text>();

			resolveCountSlider.value = count;

			resolveCount.text = "分解" + count.ToString() + "个";
		}

		/// <summary>
		/// 背包中单个物品按钮的初始化方法
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="btn">Button.</param>
		private void SetUpItemButton(Item item,Button btn,Item currentSelectItem){

			Text itemName = btn.transform.Find ("ItemName").GetComponent<Text> ();
			Text extraInfo = btn.transform.Find ("ExtraInfo").GetComponent<Text> ();
			Image itemIcon = btn.transform.Find ("ItemIcon").GetComponent<Image>();
			Image newItemTintIcon = btn.transform.Find ("NewItemTintIcon").GetComponent<Image> ();

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

			// 如果是新物品，则显示新物品提示图片
			newItemTintIcon.enabled = item.isNewItem;

			btn.onClick.RemoveAllListeners ();

			btn.transform.Find ("SelectBorder").GetComponent<Image> ().enabled = item == currentSelectItem;

			btn.onClick.AddListener (delegate {

				for (int i = 0; i < itemDisplayButtonsContainer.childCount; i++) {

					Transform itemDisplayButtonTrans = itemDisplayButtonsContainer.GetChild (i);

					if (itemDisplayButtonTrans != btn.transform) {
						itemDisplayButtonTrans.Find ("SelectBorder").GetComponent<Image> ().enabled = false;
					} else {
						Image selectBorder = itemDisplayButtonTrans.Find ("SelectBorder").GetComponent<Image> ();
						selectBorder.enabled = !selectBorder.enabled;
					}
				}

				newItemTintIcon.enabled = false;

				GetComponent<BagViewController> ().OnSelectItemInBag (item);

				SetUpItemDetailHUD (item);

			});

		}

		public void SetUpResolveGainHUD(List<Item> resolveGains){

			for (int i = 0; i < resolveGains.Count; i++) {

				Item resolveGainItem = resolveGains [i];

				Transform resolveGain = resolveGainsPool.GetInstance<Transform> (resolveGainModel.gameObject, resolveGainsContainer);

				Image resolveGainIcon = resolveGain.Find ("ResolveGainIcon").GetComponent<Image> ();

				Text resolveGainName = resolveGain.Find ("ResolveGainName").GetComponent<Text> ();

				Sprite s = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
					return obj.name == resolveGainItem.spriteName;
				});

				if (s != null) {
					resolveGainIcon.sprite = s;
				}

				resolveGainName.text = resolveGainItem.itemName;


			}
				
			resolveGainsHUD.gameObject.SetActive (true);

		}

		public void QuitResolveGainHUD(){

			resolveGainsPool.AddChildInstancesToPool (resolveGainsContainer);

			resolveGainsHUD.gameObject.SetActive (false);

		}

		/// <summary>
		/// 更换装备／物品的方法
		/// </summary>
		/// <param name="equipedEquipmentToCompare">Equiped equipment to compare.</param>
		/// <param name="allEquipmentsOfCurrentSelectTypeInBag">All equipments of current select type in bag.</param>
		public void SetUpAllEquipmentsPlaneOfEquipmentType(Equipment equipedEquipmentToCompare,List<Equipment> allEquipmentsOfCurrentSelectTypeInBag){


			MyVerticalScrollView scrollView = specificTypeItemsScrollView.GetComponent<MyVerticalScrollView> ();
	
			List<object> dataList = new List<object> ();

			for (int i = 0; i < allEquipmentsOfCurrentSelectTypeInBag.Count; i++) {

				dataList.Add(new EquipmentAndCompareEquipment(allEquipmentsOfCurrentSelectTypeInBag[i],equipedEquipmentToCompare));

			}

			scrollView.InitVerticalScrollViewData (dataList, itemDetailsPool, itemDetailsModel);

			scrollView.SetUpScrollView ();

			specificTypeItemHUD.gameObject.SetActive (true);

		}
			


		public void OnResolveCountSliderDrag(){
			int resolveCount = GetResolveCountBySlider ();
			UpdateResolveCountHUD (resolveCount);
		}
	

		public void OnEquipButtonOfDetailHUDClick(List<Item> itemsOfCurrentSelectType,Item currentSelectItem){

			OnQuitSpecificTypePlane ();

			SetUpPlayerStatusPlane ();

			SetUpEquipedEquipmentsPlane ();

			SetUpItemsDiaplayPlane (itemsOfCurrentSelectType, currentSelectItem);

		}


		public void ResetBagView<T>(List<T> currentSelectedTypeItemsInBag,Item currentSelectItem)
			where T:Item
		{

			QuitResolveCountHUD ();

			QuitItemDetailHUD ();

			SetUpPlayerStatusPlane ();

			SetUpEquipedEquipmentsPlane ();

			SetUpItemsDiaplayPlane<T> (currentSelectedTypeItemsInBag,currentSelectItem);

		}

		public void QuitResolveCountHUD(){

			resolveCountHUD.gameObject.SetActive (false);

		}
					

		// 关闭物品详细说明HUD
		public void QuitItemDetailHUD(){
			
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

			Vector3 originalPosition = bagPlane.transform.localPosition;

			bagPlane.transform.DOLocalMoveY (-offsetY, 0.5f).OnComplete (() => {
				if(cb != null){
					cb();
					bagPlane.transform.localPosition = originalPosition;
					gameObject.SetActive(false);
				}
			});

		}

		}
}
