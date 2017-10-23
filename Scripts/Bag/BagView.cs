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
	

		private float itemDetailModelHeight;//单个cell的高度
		private float itemDetailViewPortHeight;//scrollView的可视窗口高度
		private float paddingY;//scrollView顶部间距&cell之间的间距（设定顶部间距和cell间距一致）
		private int maxCellsVisible;//最多同时显示的cell数量
		private int preloadCount;//预加载cell数量

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
		public void SetUpBagView(){

			//获取所有item的图片
			this.sprites = GameManager.Instance.dataCenter.allItemSprites;
			this.player = Player.mainPlayer;

			//创建缓存池
			itemDisplayButtonsPool = InstancePool.GetOrCreateInstancePool ("ItemDisplayButtonsPool");
			itemDetailsPool = InstancePool.GetOrCreateInstancePool ("ItemDetailsPool");

			// 获取模型
			itemDisplayButtonModel = TransformManager.FindTransform ("ItemDisplayButtonModel");
			itemDetailsModel = TransformManager.FindTransform ("ItemDetailsModel");

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
			preloadCount = maxCellsVisible + 1;


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

			Transform resolveCountContainer = resolveCountHUD.Find ("ResolveCountContainer");

			Button minusBtn = resolveCountContainer.Find("MinusButton").GetComponent<Button>();
			Button plusBtn = resolveCountContainer.Find("PlusButton").GetComponent<Button>();
			Slider resolveCountSlider = resolveCountContainer.Find("ResolveCountSliderContainer").GetComponentInChildren<Slider>();
			Text resolveCount = resolveCountContainer.Find("ResolveCount").GetComponent<Text>();


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

		public int GetResolveCountBySlider(){
			
			Transform resolveCountContainer = resolveCountHUD.Find ("ResolveCountContainer");

			Slider resolveCountSlider = resolveCountContainer.Find("ResolveCountSliderContainer").GetComponentInChildren<Slider>();

			return (int)resolveCountSlider.value;
		}

		public void UpdateResolveCountHUD(int count){

			Transform resolveCountContainer = resolveCountHUD.Find ("ResolveCountContainer");

			Slider resolveCountSlider = resolveCountContainer.Find("ResolveCountSliderContainer").GetComponentInChildren<Slider>();
			Text resolveCount = resolveCountContainer.Find("ResolveCount").GetComponent<Text>();

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

			//如果当前选中类的所有装备数量小于预加载数量，则只加载实际装备数量的cell
			int maxCount = preloadCount < allEquipmentsOfCurrentSelectTypeInBag.Count ? preloadCount : allEquipmentsOfCurrentSelectTypeInBag.Count;

			for(int i =0;i<preloadCount;i++){
				
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
		public void ResetReuseCount(){
			reuseCount = 0;
		}

		/// <summary>
		/// 拖拽过程中根据cell重用计数更改PointerEventData
		/// </summary>
		public void UpdatePointerEventData(){

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
