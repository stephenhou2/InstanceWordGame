using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class NPCUIController: MonoBehaviour {

		/**********  dialogPlane UI *************/
//		public Transform npcPlane;
		public Transform choiceContainer;
		public Image npcIcon;
		public Text dialogText;

		public Button nextDialogButton;
		/**********  dialogPlane UI *************/


		private DialogGroup currentDialogGroup;

		private int currentDialogId;

		private bool nextButtonEndDialog;


		public Transform tradePlane;
		public Transform goodsContainer;
//		public Transform itemDetailsContainer;


		private NPC currentEnteredNpc;
//		private Goods currentSelectGoods;

		private InstancePool choiceButtonPool;
		public Transform choiceButtonModel;

		private InstancePool goodsPool;
		public Transform goodsModel;



//		public ItemDetailHUD itemDetail;
		public TintHUD tintHUD;

		private int currentLevelIndex;
		private Item currentSelectedItem;


		public Transform itemDetailContainer;
		public Image itemIcon;
		public Text itemName;
		public Text itemType;
		public Text itemGeneralDescription;


		private BattlePlayerUIController mBpUICtr;
		private BattlePlayerUIController bpUICtr{
			get{
				if (mBpUICtr == null) {
					mBpUICtr = TransformManager.FindTransform ("ExploreCanvas").GetComponent<BattlePlayerUIController> ();
				}
				return mBpUICtr;
			}
		}

		public void InitNPCHUD(int currentLevelIndex){

			this.choiceButtonPool = InstancePool.GetOrCreateInstancePool ("NPCChoiceButtonPool", CommonData.poolContainerName);
			this.goodsPool = InstancePool.GetOrCreateInstancePool ("NPCGoodsPool", CommonData.poolContainerName);

			this.currentLevelIndex = currentLevelIndex;

		}


		public void SetUpNpcPlane(NPC npc){

			this.currentEnteredNpc = npc;

			dialogText.text = npc.greetingDialog;

			Sprite npcSprite = GameManager.Instance.gameDataCenter.allMapSprites.Find (delegate(Sprite s) {
				return s.name == currentEnteredNpc.spriteName;
			});

			if (npcSprite != null) {
				npcIcon.sprite = npcSprite;
				npcIcon.GetComponent<Image> ().enabled = true;
			}

			// 由于目前npc功能固定，暂时不调用添加npc功能的方法
//			AddFunctionChoices ();

			gameObject.SetActive (true);

		}

		private void AddFunctionChoices(){
			
			AddChatFunction ();

			for (int i = 0; i < currentEnteredNpc.attachedFunctions.Length; i++) {

				NPCAttachedFunctionType attachedFunction = currentEnteredNpc.attachedFunctions [i];

				switch (attachedFunction) {
				case NPCAttachedFunctionType.CharactersTrade:
					AddTradeFunction ();
					break;
				case NPCAttachedFunctionType.SkillPromotion:
//					attachedFunctionText.text = "技能提升";
					break;
				case NPCAttachedFunctionType.PropertyPromotion:
//					attachedFunctionText.text = "属性提升";
					break;
				case NPCAttachedFunctionType.Task:
//					attachedFunctionText.text = "任务";
					break;

				}
			}

			AddQuitFunction ();
		}


		/// <summary>
		/// 添加交谈功能（交谈功能目前是固有功能，暂时不调用）
		/// </summary>
		private void AddChatFunction(){

			Button dialogChoiceButton = choiceButtonPool.GetInstance<Button> (choiceButtonModel.gameObject, choiceContainer);
			Text dialogFunctionText = dialogChoiceButton.GetComponentInChildren<Text> ();

			dialogFunctionText.text = "交谈";

			dialogChoiceButton.onClick.RemoveAllListeners ();

			dialogChoiceButton.onClick.AddListener (delegate {
				SetUpChatPlane();
			});

		}

		public void SetUpChatPlane(){

			currentDialogId = 0;

			DialogGroup dg = null;

			for (int i = 0; i < currentEnteredNpc.chatDialogGroups.Count; i++) {
				if (currentEnteredNpc.chatDialogGroups [i].accordGameLevel == currentLevelIndex) {
					dg = currentEnteredNpc.chatDialogGroups [i];
				}

			}

			if (dg == null) {
				Debug.LogError (string.Format ("第{0}关没有npc{1}", currentLevelIndex, currentEnteredNpc.npcName));
			}

			currentDialogGroup = dg;

			SetUpDialogPlane (currentDialogGroup.dialogs [0]);

		}


		public void SetUpDialogPlane(Dialog dialog){

			dialogText.text = dialog.dialog;

			choiceButtonPool.AddChildInstancesToPool (choiceContainer);

			bool showNextButton = true;

			List<Choice> choices = dialog.choices;
			for (int i = 0; i < choices.Count; i++) {

				Choice choice = choices [i];

				if (choice.choice == string.Empty) {
					continue;
				}

				showNextButton = false;

				Button choiceButton = choiceButtonPool.GetInstance<Button> (choiceButtonModel.gameObject, choiceContainer);

				choiceButton.GetComponentInChildren<Text> ().text = choice.choice;

				choiceButton.onClick.RemoveAllListeners ();

				choiceButton.onClick.AddListener (delegate() {
					MakeChoice(choice);
				});

			}

			if (showNextButton) {
				nextDialogButton.gameObject.SetActive (true);
			}

			if (dialog.rewardIds.Length != 0) {
				for (int i = 0; i < dialog.rewardIds.Length; i++) {
					Item rewardItem = Item.NewItemWith (dialog.rewardIds [i], dialog.rewardCounts [i]);
					Player.mainPlayer.AddItem (rewardItem);
					dialog.finishRewarding = true;
					string tint = string.Format ("获得 {0} x{1}", rewardItem.itemName, rewardItem.itemCount);

					Sprite rewardSprite = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
						return obj.name == rewardItem.spriteName;
					});

					tintHUD.SetUpTintHUD (tint,rewardSprite);
				}
			}


		}

		public void OnNextDialogButtonClick (){

			if (currentDialogGroup.dialogs [currentDialogId].isEndingDialog) {
				QuitChatDialogs ();
				return;
			}

			currentDialogId++;

			SetUpDialogPlane(currentDialogGroup.dialogs[currentDialogId]);
		}
			
		private void MakeChoice(Choice choice){

			if (choice.isEnd) {
				QuitChatDialogs ();
				return;
			}

			int dialogId = choice.dialogId;

			currentDialogId = dialogId;

			Dialog dialog = currentDialogGroup.dialogs [dialogId];

			choiceButtonPool.AddChildInstancesToPool (choiceContainer);

			SetUpDialogPlane (dialog);

		}

		private void QuitChatDialogs(){
			
			dialogText.text = currentEnteredNpc.greetingDialog;
			nextDialogButton.gameObject.SetActive (false);
			choiceButtonPool.AddChildInstancesToPool (choiceContainer);
			AddFunctionChoices ();
		}



		/// <summary>
		/// 添加交易功能（交易功能目前是固有功能，暂时不调用）
		/// </summary>
		private void AddTradeFunction(){
			
			Button choiceButton = choiceButtonPool.GetInstance<Button> (choiceButtonModel.gameObject, choiceContainer);
			Text attachedFunctionText = choiceButton.GetComponentInChildren<Text> ();
			attachedFunctionText.text = "交易";
			choiceButton.onClick.RemoveAllListeners ();
			choiceButton.onClick.AddListener (SetUpTradePlane);
		}



		public void SetUpTradePlane(){

			SoundManager.Instance.PlayAudioClip ("UI/sfx_UI_Trader");

			ClearItemDetail ();

			Trader trader = currentEnteredNpc as Trader;

			currentEnteredNpc = trader;

			tradePlane.gameObject.SetActive (true);

			goodsPool.AddChildInstancesToPool (goodsContainer);

			List<Item> itemsAsGoods = trader.itemsAsGoodsOfCurrentLevel;

			for (int i = 0; i < itemsAsGoods.Count; i++) {

				Item itemAsGoods = itemsAsGoods [i];


				Transform goodsCell = goodsPool.GetInstance<Transform> (goodsModel.gameObject, goodsContainer);

				goodsCell.GetComponent<GoodsCell> ().SetUpGoodsCell (itemAsGoods);

				goodsCell.GetComponent<Button>().onClick.RemoveAllListeners ();

				int goodsIndex = i;

				goodsCell.GetComponent<Button>().onClick.AddListener (delegate {
					currentSelectedItem = itemAsGoods;
					SetUpItemDetailsInTrade(itemAsGoods);
					UpdateGoodsSelection(goodsIndex);
				});

			}

		}

		private void UpdateGoodsSelection(int selectedGoodsIndex){

			for (int i = 0; i < goodsContainer.childCount; i++) {

				Transform goodsCell = goodsContainer.GetChild (i);

				goodsCell.GetComponent<GoodsCell> ().SetSelection (i == selectedGoodsIndex);

			}

		}


		private void SetUpItemDetailsInTrade(Item item){

			Sprite itemSprite = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
				return obj.name == item.spriteName;
			});

			itemIcon.sprite = itemSprite;
			itemIcon.enabled = itemSprite != null;

			itemName.text = item.itemName;
			itemType.text = item.GetItemTypeString ();
			itemGeneralDescription.text = item.itemGeneralDescription;

			itemDetailContainer.gameObject.SetActive (true);
		}

		public void OnBuyButtonClick(){

			if (currentSelectedItem == null) {
				return;
			}

			bool buySuccess = PlayerBuyGoods (currentSelectedItem);

			string tint = "";

			if (!buySuccess) {
				tint = "水晶不足";
				tintHUD.SetUpTintHUD (tint,null);
				return;
			}

			switch (currentSelectedItem.itemType) {
			case ItemType.UnlockScroll:
				tint = string.Format ("获得 解锁卷轴{0}{1}{2}", CommonData.diamond, currentSelectedItem.itemName, CommonData.diamond);
				break;
			case ItemType.CraftingRecipes:
				tint = string.Format ("获得 合成卷轴{0}{1}{2}", CommonData.diamond, currentSelectedItem.itemName, CommonData.diamond);
				break;
			default:
				tint = string.Format ("获得 {0} x1", currentSelectedItem.itemName);
				break;
			}

			Sprite goodsSprite = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
				return obj.name == currentSelectedItem.spriteName;
			});

			tintHUD.SetUpTintHUD (tint,goodsSprite);

			bpUICtr.UpdateItemButtonsAndStatusPlane ();

			currentSelectedItem = null;

			ClearItemDetail ();

			if ((currentEnteredNpc as Trader).itemsAsGoodsOfCurrentLevel.Count == 0) {
				QuitTradePlane ();
				return;
			}

			SetUpTradePlane ();
		}

		private void ClearItemDetail(){
			itemDetailContainer.gameObject.SetActive (false);
			itemIcon.enabled = false;
			itemName.text = "";
			itemType.text = "";
			itemGeneralDescription.text = "";
		}

		private bool PlayerBuyGoods(Item itemAsGoods){

			Player player = Player.mainPlayer;

			if (player.totalCoins < itemAsGoods.price) {
				return false;
			}

			player.totalCoins -= itemAsGoods.price;

			player.AddItem (itemAsGoods);

			(currentEnteredNpc as Trader).itemsAsGoodsOfCurrentLevel.Remove (itemAsGoods);

			return true;
		}


		/// <summary>
		/// 添加退出功能（退出功能目前是固有功能，暂时不调用）
		/// </summary>
		private void AddQuitFunction(){

			Button choiceButton = choiceButtonPool.GetInstance<Button> (choiceButtonModel.gameObject, choiceContainer);
			Text attachedFunctionText = choiceButton.GetComponentInChildren<Text> ();
			attachedFunctionText.text = "离开";
			choiceButton.onClick.RemoveAllListeners ();
			choiceButton.onClick.AddListener (QuitNPCPlane);

		}

		public void QuitNPCPlane(){
			
//			choiceButtonPool.AddChildInstancesToPool (choiceContainer);

			QuitTradePlane ();

			npcIcon.GetComponent<Image> ().enabled = false;

			dialogText.text = string.Empty;

			gameObject.SetActive (false);

		}

		public void QuitTradePlane(){

//			itemDetail.QuitItemDetailHUD ();

			tradePlane.gameObject.SetActive (false);

		}
			
		
	}
}
