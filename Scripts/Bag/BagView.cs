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


		private float itemDetailModelHeight;//单个cell的高度
		private float itemDetailViewPortHeight;//scrollView的可视窗口高度
		private float paddingY;//scrollView顶部间距&cell之间的间距（设定顶部间距和cell间距一致）
		private int maxCellsVisible;//最多同时显示的cell数量
		private int maxPreloadCountOfItemDetails;//预加载cell数量

		private int currentMinEquipmentIndex;//当前最上部（包括不可见）equipment在背包中当前选中类型equipments的序号
		private int currentMaxEquipmentIndex;//当前最底部（包括不可见）equipment在背包中当前选中类型equipments的序号

		private List<Equipment> allEquipmentsOfCurrentSelectTypeInBag;//背包中当前选中类型的所有equipments
		private Equipment compareEquipment;//用来比较的equipment

		// 拖拽过程中的cell重用计数（底部cell移至顶部count++，顶部cell移至底部count--）
		// scrollRect在拖拽过程中，content的localPosition由onDrag传入的PointerEventData计算而来，
		// 无法直接更改localPosition（更改后也会被从PointerEventData计算的position替换掉），因此需要自己传入PointerEventData
		// 记录拖拽过程的重用计数，获取原始PointerEventData后根据count更改其中的position后传入onDrag接口
		// 只有拖拽过程中会有content的localposition无法更改的问题 自由滑动过程中没有这个问题
		private int reuseCount;


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
				itemDisplayButtonsPool = InstancePool.GetOrCreateInstancePool ("ItemDisplayButtonsPool");
				itemDetailsPool = InstancePool.GetOrCreateInstancePool ("ItemDetailsPool");
				resolveGainsPool = InstancePool.GetOrCreateInstancePool ("ResolveGainsPool");

				itemDisplayButtonsPool.transform.SetParent (poolContainerOfBagCanvas);
				itemDetailsPool.transform.SetParent (poolContainerOfBagCanvas);
				resolveGainsPool.transform.SetParent (poolContainerOfBagCanvas);
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


			maxPreloadCountOfItem = 30;

			//获取装备更换页面cell的高度
			itemDetailModelHeight = (itemDetailsModel as RectTransform).rect.height;

			//获取装备更换页面可视区域高度
			itemDetailViewPortHeight = (specificTypeItemsScrollView as RectTransform).rect.height;

			//获取装备更换页面的顶部间距&cell间距
			VerticalLayoutGroup vLayoutGroup = specificTypeItemDetailsContainer.GetComponent<VerticalLayoutGroup> ();
			paddingY = vLayoutGroup.padding.top;

			//计算装备更换页面cell最大显示数量
			maxCellsVisible = (int)(itemDetailViewPortHeight / (itemDetailModelHeight + paddingY)) + 1;
			//计算装备更换页面cell预加载数量
			maxPreloadCountOfItemDetails = maxCellsVisible + 1;


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

				if (equipment.damagePercentage <= 15) {
					colorText = "green";
				} else if (equipment.damagePercentage <= 50) {
					colorText = "orange";
				} else {
					colorText = "red";
				}

				itemDamagePercentage.text = string.Format("损坏率：<color={0}>{1}%</color>",colorText, equipment.damagePercentage);

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

			if (equipment.damagePercentage <= 15) {
				colorText = "green";
			} else if (equipment.damagePercentage <= 50) {
				colorText = "orange";
			} else {
				colorText = "red";
			}

			itemDamagePercentage.text = string.Format("损坏率：<color={0}>{1}%</color>",colorText, equipment.damagePercentage);

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
		/// <param name="type">Type.</param>
		/// <param name="allItemsOfCurrentSelectType">All items of current select type.</param>
		public void SetUpAllEquipmentsPlaneOfEquipmentType(Equipment equipedEquipment,List<Equipment> allEquipmentsOfCurrentSelectTypeInBag){

			this.compareEquipment = equipedEquipment;

			this.allEquipmentsOfCurrentSelectTypeInBag = allEquipmentsOfCurrentSelectTypeInBag;

			itemDetailsPool.AddChildInstancesToPool (specificTypeItemDetailsContainer);

			specificTypeItemDetailsContainer.localPosition = new Vector3 (specificTypeItemDetailsContainer.localPosition.x, 0, 0);

			//如果当前选中类的所有装备数量小于预加载数量，则只加载实际装备数量的cell
			int maxCount = maxPreloadCountOfItemDetails < allEquipmentsOfCurrentSelectTypeInBag.Count ? maxPreloadCountOfItemDetails : allEquipmentsOfCurrentSelectTypeInBag.Count;

			for(int i =0;i<maxCount;i++){
				
				Equipment equipmentInBag = allEquipmentsOfCurrentSelectTypeInBag[i];

				Transform itemDetail = itemDetailsPool.GetInstance<Transform> (itemDetailsModel.gameObject,specificTypeItemDetailsContainer);

				itemDetail.GetComponent<ItemDetailView>().SetUpItemDetailView(equipedEquipment,equipmentInBag);

			}

			//初始化当前顶部和底部的equipment序号
			currentMinEquipmentIndex = 0;
			currentMaxEquipmentIndex = maxCount < 1 ? 0 : maxCount - 1;

			specificTypeItemHUD.gameObject.SetActive (true);

		}



		/// <summary>
		/// 装备更换页面开始进行拖拽时，重置拖拽过程中cell重用计数
		/// </summary>
		public void ResetScrollViewOnBeginDrag(){
			
			reuseCount = 0;


			if (currentMinEquipmentIndex == 0 || currentMaxEquipmentIndex == allEquipmentsOfCurrentSelectTypeInBag.Count - 1) {
				specificTypeItemsScrollView.GetComponent<ScrollRect> ().movementType = ScrollRect.MovementType.Clamped;
			} else {
				specificTypeItemsScrollView.GetComponent<ScrollRect> ().movementType = ScrollRect.MovementType.Elastic;
			}

		}


		public void ResetDataOnDrag(){

			if (currentMinEquipmentIndex == 0 || currentMaxEquipmentIndex == allEquipmentsOfCurrentSelectTypeInBag.Count - 1) {
				specificTypeItemsScrollView.GetComponent<ScrollRect> ().movementType = ScrollRect.MovementType.Clamped;
			} else {
				specificTypeItemsScrollView.GetComponent<ScrollRect> ().movementType = ScrollRect.MovementType.Elastic;
			}


			// content的位置移动（cell重用数量 * （cell高度+cell间距））
			// 为了方便这里计算，需要设定cell间距和scrollView顶部间距保持一致
			float offset = reuseCount * (itemDetailModelHeight + paddingY);

			UnityEngine.EventSystems.PointerEventData newData = specificTypeItemsScrollView.GetComponent<DragRecorder> ().GetPointerEventData (offset);

			// 传入更新后的PointerEventData
			if (newData != null) {
				specificTypeItemsScrollView.GetComponent<ScrollRect> ().OnDrag (newData);
			}

		}



		/// <summary>
		/// scrollView滑动过程中根据位置回收重用cell
		/// </summary>
		public void ReuseCellAndUpdateContentPosition(){

			// 获得content的localPosition
			float scrollRectPosY = specificTypeItemDetailsContainer.localPosition.y;
			// 获得content的滚动速度
			float velocityY = specificTypeItemsScrollView.GetComponent<ScrollRect> ().velocity.y;

			// 向下滚动
			if (velocityY > 0) {

				// 如果最底部equipment是当前选中类型equipments中的最后一个
				if (currentMaxEquipmentIndex >= allEquipmentsOfCurrentSelectTypeInBag.Count - 1) {
					Debug.Log ("所有物品加载完毕");
					return;
				}

				// 判断最顶部cell是否已经不可见
				bool firstCellInvisible = (int)(scrollRectPosY / (itemDetailModelHeight + paddingY))  >= 1;

				// 顶部cell移至底部并更新显示数据
				if (firstCellInvisible) {

					// 获得顶部cell
					Transform itemDetail = specificTypeItemDetailsContainer.GetChild (0);

					// 移至底部
					itemDetail.SetAsLastSibling ();

					// 获得将显示的euipment数据
					Equipment equipmentInBag = allEquipmentsOfCurrentSelectTypeInBag [currentMaxEquipmentIndex + 1];

					// 更新该cell的显示数据
					itemDetail.GetComponent<ItemDetailView> ().SetUpItemDetailView (compareEquipment, equipmentInBag);

					// 计算content新的位置信息
					float newPosY = specificTypeItemDetailsContainer.localPosition.y - itemDetailModelHeight - paddingY;

					// 移动content，确保屏幕显示和cell重用前一致
					//（cell重用时由于autoLayout，cell位置会产生变化，整体移动content的位置确保每个cell回到重用前的位置）
					specificTypeItemDetailsContainer.localPosition = new Vector3 (0, newPosY, 0);

					// 最顶部和最底部equipment的序号++
					currentMaxEquipmentIndex++;
					currentMinEquipmentIndex++;

					// 重用数量++
					reuseCount++;

				} 

			} else if (velocityY < 0) {//向下滚动

				// 如果最顶部equipment是当前选中类型equipments中的第一个
				if (currentMinEquipmentIndex <= 0) {
					Debug.Log ("所有物品加载完毕");
					return;
				}

				// 判断最底部cell是否可见
				bool lastCellInvisble = maxCellsVisible * (itemDetailModelHeight + paddingY) - scrollRectPosY >= itemDetailViewPortHeight;

				if (lastCellInvisble) {

					Transform itemDetail = specificTypeItemDetailsContainer.GetChild (specificTypeItemDetailsContainer.childCount - 1);

					itemDetail.SetAsFirstSibling ();

					Equipment equipmentInBag = allEquipmentsOfCurrentSelectTypeInBag [currentMinEquipmentIndex - 1];

					itemDetail.GetComponent<ItemDetailView> ().SetUpItemDetailView (compareEquipment, equipmentInBag);

					float newPosY = specificTypeItemDetailsContainer.localPosition.y + itemDetailModelHeight + paddingY;

					specificTypeItemDetailsContainer.localPosition = new Vector3 (0, newPosY, 0);

					currentMaxEquipmentIndex--;
					currentMinEquipmentIndex--;

					reuseCount--;

				}
			}
		}


		public void OnResolveCountSliderDrag(){
			int resolveCount = GetResolveCountBySlider ();
			UpdateResolveCountHUD (resolveCount);
		}
	

		public void OnEquipButtonOfDetailHUDClick(){

			OnQuitSpecificTypePlane ();

			SetUpPlayerStatusPlane ();

			SetUpEquipedEquipmentsPlane ();

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

			this.sprites = null;
			
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
