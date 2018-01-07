using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	using UnityEngine.UI;
	using DG.Tweening;

	public class BagView : MonoBehaviour {

//		public Transform bagViewContainer;
//		public GameObject bagPlane;

		public Transform playerPropertiesPlane;
		public Transform playerStatusPlane;

		public Button[] bagTypeButtons;
		public Transform[] allEquipedEquipmentButtons;

		public Transform bagItemsPlane;
		private InstancePool bagItemsPool;
		public Transform bagItemsContainer;
		private Transform bagItemModel;


		private int maxPreloadCountOfItem;

		private Player player;

		public Transform itemDetailHUD;


//		public Transform specificTypeItemHUD;
//		public Transform specificTypeItemsScrollView;
//		public Transform specificTypeItemDetailsContainer;

		private Transform itemDetailsModel;
		private InstancePool itemDetailsPool;

		public Transform resolveCountHUD;
	
		public Transform resolveGainsHUD;
		public Transform resolveGainsContainer;

		private InstancePool resolveGainsPool;
		private Transform resolveGainModel;

		private Sequence[] changeTintFromEqSequences = new Sequence[8];
		private Sequence[] changeTintFromOtherSequences = new Sequence[8];

		/// <summary>
		/// 初始化背包界面
		/// </summary>
		public void SetUpBagView(bool setVisible){

			this.GetComponent<Canvas> ().enabled = setVisible;

			//获取所有item的图片
//			this.sprites = GameManager.Instance.gameDataCenter.allItemSprites;
			this.player = Player.mainPlayer;

			Transform poolContainerOfBagCanvas = TransformManager.FindOrCreateTransform (CommonData.poolContainerName + "/PoolContainerOfBagCanvas");
			Transform modelContainerOfBagCanvas = TransformManager.FindOrCreateTransform (CommonData.instanceContainerName + "/ModelContainerOfBagCanvas");

			if (poolContainerOfBagCanvas.childCount == 0) {
				//创建缓存池
				bagItemsPool = InstancePool.GetOrCreateInstancePool ("BagItemsPool",poolContainerOfBagCanvas.name);
				itemDetailsPool = InstancePool.GetOrCreateInstancePool ("ItemDetailsPool",poolContainerOfBagCanvas.name);
				resolveGainsPool = InstancePool.GetOrCreateInstancePool ("ResolveGainsPool",poolContainerOfBagCanvas.name);

			}

			if (modelContainerOfBagCanvas.childCount == 0) {
				// 获取模型
				bagItemModel = TransformManager.FindTransform ("BagItemModel");
				itemDetailsModel = TransformManager.FindTransform ("ItemDetailsModelInBagCanvas");
				resolveGainModel = TransformManager.FindTransform ("ResolveGainModel");

				bagItemModel.SetParent (modelContainerOfBagCanvas);
				itemDetailsModel.SetParent (modelContainerOfBagCanvas);
				resolveGainModel.SetParent (modelContainerOfBagCanvas);
			}

			// 背包中单类物品最大预加载数量
			maxPreloadCountOfItem = 24;

			Agent.PropertyChange propertyChange = player.ResetBattleAgentProperties (false);

			SetUpPlayerStatusPlane (propertyChange);

			SetUpEquipedEquipmentsPlane ();

			SetUpItemsDiaplayPlane (player.allItemsInBag);

		}


		/// <summary>
		/// 初始化玩家属性界面
		/// </summary>
		public void SetUpPlayerStatusPlane(Agent.PropertyChange propertyChange){

			Slider healthBar = playerStatusPlane.Find ("HealthBar").GetComponent<Slider> ();

			Image playerIcon = playerStatusPlane.Find ("PlayerIcon").GetComponent<Image> ();

			Text healthText = healthBar.transform.Find ("HealthText").GetComponent<Text> ();

//			Text playerLevel = playerPropertiesPlane.Find("PlayerLevel").GetComponent<Text>();

			Transform playerMaxHealth = playerPropertiesPlane.Find ("MaxHealth");
			Transform playerHit = playerPropertiesPlane.Find ("Hit");
			Transform playerAttack = playerPropertiesPlane.Find ("Attack");
			Transform playerMana = playerPropertiesPlane.Find ("Mana");
			Transform playerArmor = playerPropertiesPlane.Find ("Armor");
			Transform playerMagicResist = playerPropertiesPlane.Find ("MagicResist");
			Transform playerDodge = playerPropertiesPlane.Find ("Dodge");
			Transform playerCrit = playerPropertiesPlane.Find ("Crit");


			playerMaxHealth.Find ("PropertyValue").GetComponent<Text> ().text = player.maxHealth.ToString ();
			playerHit.Find ("PropertyValue").GetComponent<Text> ().text = player.hit.ToString ();
			playerAttack.Find ("PropertyValue").GetComponent<Text> ().text = player.attack.ToString ();
			playerMana.Find ("PropertyValue").GetComponent<Text> ().text = player.mana.ToString ();
			playerArmor.Find ("PropertyValue").GetComponent<Text> ().text = player.armor.ToString ();
			playerMagicResist.Find ("PropertyValue").GetComponent<Text> ().text = player.magicResist.ToString ();
			playerDodge.Find ("PropertyValue").GetComponent<Text> ().text = player.dodge.ToString ();
			playerCrit.Find ("PropertyValue").GetComponent<Text> ().text = player.crit.ToString ();

			healthBar.maxValue = player.maxHealth;
			healthBar.value = player.health;
			healthText.text = player.health.ToString () + "/" + player.maxHealth.ToString ();

//			playerLevel.text = "Lv." + player.agentLevel;

//			for (int i = 0; i < changeTintFromEqSequences.Length; i++) {
//				if (changeTintFromEqSequences [i] != null) {
//					changeTintFromEqSequences [i].Complete ();
//				}
//				if (changeTintFromOtherTweens [i] != null) {
//					changeTintFromOtherTweens [i].Kill(false);
//				}
//			}
		

			ShowEquipmentChangeTint (playerMaxHealth, propertyChange.maxHealthChangeFromEq,0);
			ShowEquipmentChangeTint (playerHit, propertyChange.hitChangeFromEq,1);
			ShowEquipmentChangeTint (playerAttack, propertyChange.attackChangeFromEq,2);
			ShowEquipmentChangeTint (playerMana, propertyChange.manaChangeFromEq,3);
			ShowEquipmentChangeTint (playerArmor, propertyChange.armorChangeFromEq,4);
			ShowEquipmentChangeTint (playerMagicResist, propertyChange.magicResistChangeFromEq,5);
			ShowEquipmentChangeTint (playerDodge, propertyChange.dodgeChangeFromEq,6);
			ShowEquipmentChangeTint (playerCrit, propertyChange.critChangeFromEq,7);

			ShowConsumablesEffectTint (playerMaxHealth, propertyChange.maxHealthChangeFromOther,0);
			ShowConsumablesEffectTint (playerHit, propertyChange.hitChangeFromOther,1);
			ShowConsumablesEffectTint (playerAttack, propertyChange.attackChangeFromOther,2);
			ShowConsumablesEffectTint (playerMana, propertyChange.manaChangeFromOther,3);
			ShowConsumablesEffectTint (playerArmor, propertyChange.armorChangeFromOther,4);
			ShowConsumablesEffectTint (playerMagicResist, propertyChange.magicResistChangeFromOther,5);
			ShowConsumablesEffectTint (playerDodge, propertyChange.dodgeChangeFromOther,6);
			ShowConsumablesEffectTint (playerCrit, propertyChange.critChangeFromOther,7);


		}

		private int CheckPropertyChange(int change){
			int changeResult = 0;
			if (change > 0) {
				changeResult = 1;
			} else if (change < 0) {
				changeResult = -1;
			}
			return changeResult;
		}

		private void ShowConsumablesEffectTint(Transform propertyTrans,int change,int indexInPanel){

			Text propertyValue = propertyTrans.Find ("PropertyValue").GetComponent<Text> ();

			if (change == 0) {
				propertyValue.color = Color.white;
				changeTintFromOtherSequences [indexInPanel].Complete ();
				return;
			}

			propertyValue.color = change > 0 ? Color.green : Color.red;

			if (changeTintFromOtherSequences [indexInPanel] == null) {

				Sequence propertyValueBlink = DOTween.Sequence ();
				propertyValueBlink
					.Append (propertyValue.DOFade (0.3f, 2))
					.Append (propertyValue.DOFade (1.0f, 2));

				propertyValueBlink.SetLoops (-1);

				changeTintFromOtherSequences [indexInPanel] = propertyValueBlink;

				propertyValueBlink.SetAutoKill (false);

				propertyValueBlink.SetUpdate (true);

				return;
			}

			changeTintFromOtherSequences [indexInPanel].Restart ();

		}



		private void ShowEquipmentChangeTint(Transform propertyTrans,int change,int indexInPanel){
			
			int changeResult = CheckPropertyChange (change);

			Image changeTint = propertyTrans.Find ("ChangeTint").GetComponent<Image>();

			changeTint.enabled = false;

			if (changeTintFromEqSequences [indexInPanel] != null && !changeTintFromEqSequences [indexInPanel].IsComplete()) {
				changeTintFromEqSequences [indexInPanel].Complete ();
			}

			if (changeResult == 0) {
				return;
			}

			changeTint.enabled = true;

			int rotationZ = changeResult > 0 ? 0 : 180;

			changeTint.transform.localRotation = Quaternion.Euler(new Vector3 (0, 0, rotationZ));

			changeTint.color = changeResult > 0 ? Color.green : Color.red;


			if (changeTintFromEqSequences[indexInPanel] == null) {
				Sequence changeTintSequence = DOTween.Sequence ();
				changeTintSequence
					.Append(changeTint.DOFade (0.2f, 1))
					.Append(changeTint.DOFade (1f, 1))
					.AppendCallback(()=>{
						changeTint.enabled = false;
					});
				changeTintSequence.SetUpdate (true);
				changeTintFromEqSequences[indexInPanel] = changeTintSequence;
				changeTintSequence.SetAutoKill (false);
				return;
			}

			changeTintFromEqSequences [indexInPanel].Restart ();

		}

		/// <summary>
		/// 初始化已装备物品界面
		/// </summary>
		public void SetUpEquipedEquipmentsPlane(){

			for(int i = 0;i<player.allEquipedEquipments.Length;i++){

				Transform equipedEquipmentButton = allEquipedEquipmentButtons[i];

				Image itemIcon = equipedEquipmentButton.transform.Find ("ItemIcon").GetComponent<Image> ();

				Equipment equipment = player.allEquipedEquipments [i];

				equipedEquipmentButton.GetComponent<ItemDragControl> ().item = equipment;

				if (equipment.itemId < 0) {
					itemIcon.enabled = false;
					continue;
				}

				Sprite s = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
					return obj.name == equipment.spriteName;
				});
				itemIcon.sprite = s;
				itemIcon.enabled = true;

			}

		}

		public void SetUpTintHUD(string tint){
			Debug.Log (tint);
		}
			
		/// <summary>
		/// 初始化背包物品界面
		/// </summary>
		public void SetUpItemsDiaplayPlane<T>(List<T> items)
			where T:Item
		{

//			int lastItemCount = itemDisplayButtonsContainer.childCount;
//
//			for (int i = 0; i < lastItemCount; i++) {
//
//				Button itemDisplayButton = itemDisplayButtonsContainer.GetChild (i).GetComponent<Button>();
//
//				Item item = items [i];
//
//				SetUpItemButton (item, itemDisplayButton, currentSelectItem);
//
//			}
//
//			for (int i = lastItemCount; i < items.Count; i++) {
//
//				Button itemDisplayButton = itemDisplayButtonsPool.GetInstance<Button> (itemDisplayButtonModel.gameObject, itemDisplayButtonsContainer);
//
//				Item item = items [i];
//
//				SetUpItemButton (item, itemDisplayButton, currentSelectItem);
//
//			}

			bagItemsPlane.GetComponent<ScrollRect> ().velocity = Vector2.zero;

			bagItemsContainer.localPosition = Vector3.zero;

			bagItemsPool.AddChildInstancesToPool (bagItemsContainer);

			int loadCount = items.Count <= maxPreloadCountOfItem ? items.Count : maxPreloadCountOfItem;

			for (int i = 0; i < loadCount; i++) {

				Item item = items [i] as Item;

//				itemDisplayButton.SetUpItemButtonView (item, itemDisplayButtonsContainer, currentSelectItem);

				AddBagItem (item);

			}
				
			if (loadCount < items.Count) {
				
				List<Item> myItems = new List<Item> ();

				for (int i = 0; i < items.Count; i++) {
					myItems.Add (items [i]);
				}

				IEnumerator coroutine = LoadItemDisplayButtonsAsync (myItems);

				StartCoroutine (coroutine);
			}	
		}
			
		private IEnumerator LoadItemDisplayButtonsAsync(List<Item> items){

			yield return null;

			for (int i = maxPreloadCountOfItem; i < items.Count; i++) {

				Item item = items [i] as Item;

				AddBagItem (item);
			}
		}

	

		/// <summary>
		/// 初始化物品详细介绍页面
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetUpItemDetailHUD(Item item,Sprite itemSprite){

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

			itemIcon.sprite = itemSprite;

			itemIcon.enabled = true;

			itemName.text = item.itemName;

			itemType.text = item.GetItemTypeString ();

			itemProperties.text = item.itemDescription;

			// 如果物品是装备
			switch(item.itemType){

			case ItemType.Equipment:

				Equipment equipment = item as Equipment;

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

				itemDamagePercentage.text = string.Format ("耐久度：<color={0}>{1}/{2}</color>", colorText, equipment.durability, equipment.maxDurability);

				choiceHUDWithTwoBtns.gameObject.SetActive (true);

				if (equipment.equiped) {
					choiceHUDWithTwoBtns.Find ("UnloadButton").gameObject.SetActive (true);
					choiceHUDWithTwoBtns.Find ("EquipButton").gameObject.SetActive (false);
					choiceHUDWithTwoBtns.Find ("UseButton").gameObject.SetActive (false);
				} else {
					choiceHUDWithTwoBtns.Find ("UnloadButton").gameObject.SetActive (false);
					choiceHUDWithTwoBtns.Find ("EquipButton").gameObject.SetActive (true);
					choiceHUDWithTwoBtns.Find ("UseButton").gameObject.SetActive (false);
				}

				break;
			case ItemType.Consumables:
				itemProperties.text = item.itemDescription;
				itemName.text = item.itemName;
				itemDamagePercentage.text = string.Empty;


				choiceHUDWithTwoBtns.gameObject.SetActive (true);

				choiceHUDWithTwoBtns.Find ("UnloadButton").gameObject.SetActive (false);
				choiceHUDWithTwoBtns.Find ("EquipButton").gameObject.SetActive (false);
				choiceHUDWithTwoBtns.Find ("UseButton").gameObject.SetActive (true);

				break;
			case ItemType.FuseStone:
				itemProperties.text = item.itemDescription;
				itemName.text = item.itemName;
				itemDamagePercentage.text = string.Empty;
				choiceHUDWithOneBtn.gameObject.SetActive (true);
				break;
			case ItemType.Task:
				itemProperties.text = item.itemDescription;
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
		public void AddBagItem(Item item){

			Transform bagItem = bagItemsPool.GetInstance<Transform> (bagItemModel.gameObject, bagItemsContainer);

			ItemInBagDragControl dragItemScript = bagItem.GetComponent<ItemInBagDragControl> ();
			dragItemScript.item = item;

			Text itemName = bagItem.Find ("ItemName").GetComponent<Text> ();
			Text extraInfo = bagItem.Find ("ExtraInfo").GetComponent<Text> ();
			Image itemIcon = bagItem.Find ("ItemIcon").GetComponent<Image>();
			Image newItemTintIcon = bagItem.Find ("NewItemTintIcon").GetComponent<Image> ();
			Image selectBorder = bagItem.Find ("SelectBorder").GetComponent<Image> ();

			itemIcon.enabled = false;
			newItemTintIcon.enabled = false;

			itemName.text = item.itemName;

			if (item.itemType == ItemType.Equipment && (item as Equipment).equiped) {
				extraInfo.text = "<color=green>已装备</color>";
			} else if (item.itemType == ItemType.Consumables) {
				extraInfo.text = item.itemCount.ToString ();
			} else {
				extraInfo.text = string.Empty;
			}


			Sprite itemSprite = null;

			itemSprite = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
				return obj.name == item.spriteName;
			});

			if(itemSprite != null){
				itemIcon.sprite = itemSprite;
			}

			itemIcon.enabled = itemIcon.sprite != null;

			// 如果是新物品，则显示新物品提示图片
			newItemTintIcon.enabled = item.isNewItem;

//			if (updateBagView) {
//				SetUpEquipedEquipmentsPlane ();
//				SetUpPlayerStatusPlane ();
//			}

		}


	

		public void RemoveItemInBag(Item item){

			for (int i = 0; i < bagItemsContainer.childCount; i++) {
				Transform bagItem = bagItemsContainer.GetChild (i);
				if (bagItem.GetComponent<ItemDragControl> ().item.itemId == item.itemId) {
					bagItemsPool.AddInstanceToPool (bagItem.gameObject);
					return;
				}
			}

		}

		public void SetUpResolveGainHUD(List<Item> resolveGains){

			for (int i = 0; i < resolveGains.Count; i++) {

				Item resolveGainItem = resolveGains [i];

				Transform resolveGain = resolveGainsPool.GetInstance<Transform> (resolveGainModel.gameObject, resolveGainsContainer);

				Image resolveGainIcon = resolveGain.Find ("ResolveGainIcon").GetComponent<Image> ();

				Text resolveGainName = resolveGain.Find ("ResolveGainName").GetComponent<Text> ();

				Sprite s = GameManager.Instance.gameDataCenter.allMaterialSprites.Find (delegate(Sprite obj) {
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


//			MyVerticalScrollView scrollView = specificTypeItemsScrollView.GetComponent<MyVerticalScrollView> ();
//	
//			List<object> dataList = new List<object> ();
//
//			for (int i = 0; i < allEquipmentsOfCurrentSelectTypeInBag.Count; i++) {
//
//				dataList.Add(new EquipmentAndCompareEquipment(allEquipmentsOfCurrentSelectTypeInBag[i],equipedEquipmentToCompare));
//
//			}
//
//			scrollView.InitVerticalScrollViewData (dataList, itemDetailsPool, itemDetailsModel);
//
//			scrollView.SetUpScrollView ();
//
//			specificTypeItemHUD.gameObject.SetActive (true);

		}
			


		public void OnResolveCountSliderDrag(){
			int resolveCount = GetResolveCountBySlider ();
			UpdateResolveCountHUD (resolveCount);
		}
	

//		public void OnEquipButtonOfDetailHUDClick(List<Item> itemsOfCurrentSelectType,Item currentSelectItem){
//
//			OnQuitSpecificTypePlane ();
//
//			SetUpPlayerStatusPlane ();
//
////			SetUpEquipedEquipmentsPlane ();
//
//			SetUpItemsDiaplayPlane (itemsOfCurrentSelectType);
//
//		}


//		public void ResetBagView<T>(List<T> currentSelectedTypeItemsInBag,Item currentSelectItem)
//			where T:Item
//		{
//
//			QuitResolveCountHUD ();
//
//			QuitItemDetailHUD ();
//
//			SetUpPlayerStatusPlane ();
//
////			SetUpEquipedEquipmentsPlane ();
//
//			SetUpItemsDiaplayPlane<T> (currentSelectedTypeItemsInBag);
//
//		}

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

//			specificTypeItemHUD.gameObject.SetActive (false);
//
//			for (int i = 0; i < specificTypeItemDetailsContainer.childCount; i++) {
//				Transform trans = specificTypeItemDetailsContainer.GetChild (i);
//				trans.GetComponent<ItemDetailView> ().ResetItemDetail ();
//			}

		}

		// 关闭背包界面
		public void QuitBagPlane(){

			for (int i = 0; i < changeTintFromEqSequences.Length; i++) {
				changeTintFromEqSequences [i].Kill (false);
				changeTintFromOtherSequences [i].Kill (false);
				changeTintFromEqSequences [i] = null;
				changeTintFromOtherSequences [i] = null;
			}

			GetComponent<Canvas> ().enabled = false;
		}
	}
}
