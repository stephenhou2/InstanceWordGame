using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	using UnityEngine.UI;
	using DG.Tweening;
	using System.Text;

	public class BagView : MonoBehaviour {

		public Slider healthBar;
		public Text healthText;
		public Text coinCount;

		public Slider playerExperienceBar;
		public Text playerExperienceText;
		public Text playerLevelText;

		public Transform playerMaxHealth;
		public Transform playerAttack;
		public Transform playerCrit;
		public Transform playerHit;
		public Transform playerMana;
		public Transform playerMagicResist;
		public Transform playerArmor;
		public Transform playerDodge;

		public Transform[] allEquipedEquipmentButtons;

		public Transform bagItemsPlane;
		private InstancePool bagItemsPool;
		public Transform bagItemsContainer;
		private Transform bagItemModel;


		private int maxPreloadCountOfItem;

		private Player player;




		public Transform helpPlane;

		private Sequence[] changeTintFromEqSequences = new Sequence[8];
		private Sequence[] changeTintFromOtherSequences = new Sequence[8];

		public Transform queryResolveHUD;

		public TintHUD tintHUD;
		public CharactersInBagHUD charactersInBag;

		public ItemDetailHUD itemDetail;
		public Transform choiceHUDWithOneBtn;
		public Transform choiceHUDWithTwoBtns;

		/// <summary>
		/// 初始化背包界面
		/// </summary>
		public void SetUpBagView(bool setVisible){

			this.GetComponent<Canvas> ().enabled = setVisible;

			itemDetail.InitItemDetailHUD (true, HideOperationButtons);

			//获取所有item的图片
//			this.sprites = GameManager.Instance.gameDataCenter.allItemSprites;
			this.player = Player.mainPlayer;

			Transform poolContainerOfBagCanvas = TransformManager.FindOrCreateTransform (CommonData.poolContainerName + "/PoolContainerOfBagCanvas");
			Transform modelContainerOfBagCanvas = TransformManager.FindOrCreateTransform (CommonData.instanceContainerName + "/ModelContainerOfBagCanvas");

			if (poolContainerOfBagCanvas.childCount == 0) {
				//创建缓存池
				bagItemsPool = InstancePool.GetOrCreateInstancePool ("BagItemsPool",poolContainerOfBagCanvas.name);

			}

			if (modelContainerOfBagCanvas.childCount == 0) {
				// 获取模型
				bagItemModel = TransformManager.FindTransform ("BagItemModel");

				bagItemModel.SetParent (modelContainerOfBagCanvas);
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

			// 玩家生命条
			healthBar.maxValue = player.maxHealth;
			healthBar.value = player.health;
			healthText.text = player.health.ToString () + "/" + player.maxHealth.ToString ();

			// 玩家水晶数量
			coinCount.text = player.totalCoins.ToString ();

			// 玩家经验条
			playerLevelText.text = "Lv." + player.agentLevel.ToString();
			playerExperienceBar.maxValue = player.upgradeExprience;
			playerExperienceBar.value = player.experience;
			playerExperienceText.text = player.experience.ToString() + "/" + player.upgradeExprience.ToString ();


			// 玩家属性面板
			Text playerMaxHealthValue = playerMaxHealth.Find("PropertyValue").GetComponent<Text>();
			Text playerHitValue = playerHit.Find ("PropertyValue").GetComponent<Text> ();
			Text playerAttackValue = playerAttack.Find ("PropertyValue").GetComponent<Text> ();
			Text playerManaValue = playerMana.Find ("PropertyValue").GetComponent<Text> ();
			Text playerArmorValue = playerArmor.Find ("PropertyValue").GetComponent<Text> ();
			Text playerMagicResistValue = playerMagicResist.Find ("PropertyValue").GetComponent<Text> ();
			Text playerDodgeValue = playerDodge.Find ("PropertyValue").GetComponent<Text> ();
			Text playerCritValue = playerCrit.Find ("PropertyValue").GetComponent<Text> ();

			playerMaxHealthValue.text = player.maxHealth.ToString();
			playerHitValue.text = player.hit.ToString ();
			playerAttackValue.text = player.attack.ToString ();
			playerManaValue.text = player.mana.ToString ();
			playerArmorValue.text = player.armor.ToString ();
			playerMagicResistValue.text = player.magicResist.ToString ();
			playerDodgeValue.text = player.dodge.ToString ();
			playerCritValue.text = player.crit.ToString ();


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


			


		public void ShowQueryResolveHUD(){
			queryResolveHUD.gameObject.SetActive (true);
		}

		public void QuitQueryResolveHUD(){
			queryResolveHUD.gameObject.SetActive (false);
		}

		/// <summary>
		/// 初始化背包物品界面
		/// </summary>
		public void SetUpItemsDiaplayPlane(List<Item> items){

			bagItemsPlane.GetComponent<ScrollRect> ().velocity = Vector2.zero;

			bagItemsContainer.localPosition = Vector3.zero;

			bagItemsPool.AddChildInstancesToPool (bagItemsContainer);

			int loadCount = items.Count <= maxPreloadCountOfItem ? items.Count : maxPreloadCountOfItem;

			for (int i = 0; i < loadCount; i++) {
				AddBagItem (items[i]);
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
		public void SetUpItemDetailHUD(Item item){
			itemDetail.SetUpItemDetailHUD (item);
			SetUpOperationButtons (item);
		}


		private void SetUpOperationButtons(Item item){

			// 如果物品是装备
			switch (item.itemType) {

			case ItemType.Equipment:

				Equipment equipment = item as Equipment;

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

				Transform exploreManager = TransformManager.FindTransform ("ExploreManager");

				if (exploreManager != null) {
					choiceHUDWithTwoBtns.gameObject.SetActive (true);
					choiceHUDWithTwoBtns.Find ("UnloadButton").gameObject.SetActive (false);
					choiceHUDWithTwoBtns.Find ("EquipButton").gameObject.SetActive (false);
					choiceHUDWithTwoBtns.Find ("UseButton").gameObject.SetActive (true);
				} else {
					choiceHUDWithOneBtn.gameObject.SetActive (true);
				}
				break;
			}

		}




		/// <summary>
		/// 背包中单个物品按钮的初始化方法
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="btn">Button.</param>
		public void AddBagItem(Item item){

//			if (item is Equipment && (item as Equipment).equiped) {
//				return;
//			}

			Transform bagItem = bagItemsPool.GetInstance<Transform> (bagItemModel.gameObject, bagItemsContainer);

			ItemInBagDragControl dragItemScript = bagItem.GetComponent<ItemInBagDragControl> ();
			dragItemScript.item = item;

			Text itemName = bagItem.Find ("ItemName").GetComponent<Text> ();
			Text extraInfo = bagItem.Find ("ExtraInfo").GetComponent<Text> ();
			Image itemIcon = bagItem.Find ("ItemIcon").GetComponent<Image>();
			Image newItemTintIcon = bagItem.Find ("NewItemTintIcon").GetComponent<Image> ();

			itemIcon.enabled = false;
			newItemTintIcon.enabled = false;

			itemName.text = item.itemName;

			if (item.itemType == ItemType.Consumables) {
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

		public void SetUpResolveGainHUD(List<CharacterFragment> characters){

			StringBuilder tint = new StringBuilder();

			tint.Append ("分解获得字母碎片");

			for (int i = 0; i < characters.Count; i++) {

				tint.Append (characters [i].character);

			}

			tintHUD.SetUpTintHUD (tint.ToString());

//			for (int i = 0; i < resolveGains.Count; i++) {
//
//				Item resolveGainItem = resolveGains [i];
//
//				Transform resolveGain = resolveGainsPool.GetInstance<Transform> (resolveGainModel.gameObject, resolveGainsContainer);
//
//				Image resolveGainIcon = resolveGain.Find ("ResolveGainIcon").GetComponent<Image> ();
//
//				Text resolveGainName = resolveGain.Find ("ResolveGainName").GetComponent<Text> ();
//
//				Sprite s = GameManager.Instance.gameDataCenter.allMaterialSprites.Find (delegate(Sprite obj) {
//					return obj.name == resolveGainItem.spriteName;
//				});
//
//				if (s != null) {
//					resolveGainIcon.sprite = s;
//				}
//
//				resolveGainName.text = resolveGainItem.itemName;
//
//
//			}
//				
//			resolveGainsHUD.gameObject.SetActive (true);

		}



		// 关闭物品详细说明HUD
		public void QuitItemDetailHUD(){
			HideOperationButtons ();
			itemDetail.QuitItemDetailHUD ();
		}

		private void HideOperationButtons(){
			choiceHUDWithOneBtn.gameObject.SetActive (false);
			choiceHUDWithTwoBtns.gameObject.SetActive (false);
		}

		public void SetUpCharactersInBagPlane(){
			charactersInBag.SetUpCharactersHUD ();
		}

		public void QuitCharactersInBagPlane(){
			charactersInBag.QuitCharactersHUD ();
		}

		public void SetUpTintHUD(string tint){
			tintHUD.SetUpTintHUD (tint);
		}
			

		// 关闭背包界面
		public void QuitBagPlane(){

			tintHUD.QuitTintHUD ();

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
