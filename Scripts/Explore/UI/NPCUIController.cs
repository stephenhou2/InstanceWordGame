using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class NPCUIController : MonoBehaviour {

		/**********  dialogPlane UI *************/
		public Transform npcPlane;
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
		public Transform itemDetailsContainer;


		private NPC currentEnteredNpc;
		private int currentLevel;
//		private Goods currentSelectGoods;

		private InstancePool choiceButtonPool;
		private Transform choiceButtonModel;
		private Transform goodsModel;
		private InstancePool goodsPool;

		public Transform itemDetailHUDInTrade;
		public Image itemIconBackgroundInDetail;
		public Image itemIconInDetail;
		public Text itemNameInDetail;
		public Text itemDescriptionInDetail;
		public Button buyButtonInDetail;


		public void SetupNpcPlane(NPC npc,int currentLevel,InstancePool choiceButtonPool,Transform choiceButtonModel,InstancePool goodsPool,Transform goodsModel){

			this.currentEnteredNpc = npc;
			this.currentLevel = currentLevel;
			this.choiceButtonPool = choiceButtonPool;
			this.choiceButtonModel = choiceButtonModel;
			this.goodsPool = goodsPool;
			this.goodsModel = goodsModel;


			dialogText.text = npc.greetingDialog;

			Sprite npcSprite = GameManager.Instance.gameDataCenter.allMapSprites.Find (delegate(Sprite s) {
				return s.name == currentEnteredNpc.spriteName;
			});

			if (npcSprite != null) {
				npcIcon.sprite = npcSprite;
				npcIcon.GetComponent<Image> ().enabled = true;
			}

			AddFunctionChoices ();

			npcPlane.gameObject.SetActive (true);

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


		private void AddChatFunction(){

			Button dialogChoiceButton = choiceButtonPool.GetInstance<Button> (choiceButtonModel.gameObject, choiceContainer);
			Text dialogFunctionText = dialogChoiceButton.GetComponentInChildren<Text> ();

			dialogFunctionText.text = "交谈";

			dialogChoiceButton.onClick.RemoveAllListeners ();

			dialogChoiceButton.onClick.AddListener (delegate {

				choiceButtonPool.AddChildInstancesToPool(choiceContainer);

				currentDialogId = 0;

				DialogGroup dg = null;

				for (int i = 0; i < currentEnteredNpc.chatDialogGroups.Count; i++) {
					if (currentEnteredNpc.chatDialogGroups [i].accordGameLevel == currentLevel) {
						dg = currentEnteredNpc.chatDialogGroups [i];
					}

				}

				if (dg == null) {
					Debug.LogError (string.Format ("第{0}关没有npc{1}", currentLevel, currentEnteredNpc.npcName));
				}

				currentDialogGroup = dg;

				SetUpDialogPlane (currentDialogGroup.dialogs [0]);
			});

		}


		private void SetUpDialogPlane(Dialog dialog){

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
					#warning 提示获得物品的界面逻辑没有做
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


		private void AddTradeFunction(){
			
			Button choiceButton = choiceButtonPool.GetInstance<Button> (choiceButtonModel.gameObject, choiceContainer);
			Text attachedFunctionText = choiceButton.GetComponentInChildren<Text> ();
			attachedFunctionText.text = "交易";
			choiceButton.onClick.RemoveAllListeners ();
			choiceButton.onClick.AddListener (SetUpTradePlane);
		}

		public void SetUpTradePlane(){


			Trader trader = currentEnteredNpc as Trader;

			currentEnteredNpc = trader;

			tradePlane.gameObject.SetActive (true);

			goodsPool.AddChildInstancesToPool (goodsContainer);

			GoodsGroup gg = trader.goodsGroupList.Find(delegate(GoodsGroup obj){
				return obj.accordLevel == currentLevel;
			});

			for (int i = 0; i < gg.goodsList.Count; i++) {

				Goods goods = gg.goodsList [i];

				Sprite goodsSprite = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
					return obj.name == goods.itemAsGoods.spriteName;
				});

				Transform goodsDisplay = goodsPool.GetInstance<Transform> (goodsModel.gameObject, goodsContainer);
				Image goodsIcon = goodsDisplay.Find ("GoodsIcon").GetComponent<Image> ();
				Text goodsPrice = goodsDisplay.Find ("GoodsPrice").GetComponent<Text> ();
				Button goodsSelection = goodsDisplay.GetComponent<Button> ();

				goodsIcon.sprite = goodsSprite;

				goodsPrice.text = goods.goodsPrice.ToString();

				goodsSelection.onClick.RemoveAllListeners ();

				goodsSelection.onClick.AddListener (delegate {
//					currentSelectGoods = goods;
					SetUpItemDetailsInTrade(goods,goodsSprite);
				});

			}


		}

		private void SetUpItemDetailsInTrade(Goods goods,Sprite goodsSprite){

			itemIconInDetail.sprite = goodsSprite;
			itemIconInDetail.enabled = true;
			itemIconBackgroundInDetail.enabled = true;
			itemNameInDetail.text = goods.itemAsGoods.itemName;
			itemDescriptionInDetail.text = goods.itemAsGoods.itemDescription;

			buyButtonInDetail.gameObject.SetActive (goods.itemAsGoods != null);

			if (goods.itemAsGoods != null) {
				buyButtonInDetail.onClick.RemoveAllListeners ();
				buyButtonInDetail.onClick.AddListener (delegate {
					bool buySuccess = PlayerBuyGoods(goods);
					if(!buySuccess){
						GetComponent<ExploreUICotroller>().SetUpTintHUD("金币不足");
					}else{
						GetComponent<BattlePlayerUIController>().UpdatePlayerStatusPlane();
						itemDetailHUDInTrade.gameObject.SetActive(false);
						SetUpTradePlane();
					}
				});
			}

			itemDetailHUDInTrade.gameObject.SetActive (true);

		}

		private bool PlayerBuyGoods(Goods goods){

			Player player = Player.mainPlayer;

			if (player.totalCoins < goods.goodsPrice) {
				return false;
			}

			player.totalCoins -= goods.goodsPrice;

			player.AddItem (goods.itemAsGoods);

			GoodsGroup gg = (currentEnteredNpc as Trader).goodsGroupList.Find (delegate(GoodsGroup obj) {
				return obj.accordLevel == currentLevel;
			});

			gg.goodsList.Remove (goods);

			return true;
		}



		private void AddQuitFunction(){

			Button choiceButton = choiceButtonPool.GetInstance<Button> (choiceButtonModel.gameObject, choiceContainer);
			Text attachedFunctionText = choiceButton.GetComponentInChildren<Text> ();
			attachedFunctionText.text = "离开";
			choiceButton.onClick.RemoveAllListeners ();
			choiceButton.onClick.AddListener (QuitNpcPlane);

		}

		private void QuitNpcPlane(){

			choiceButtonPool.AddChildInstancesToPool (choiceContainer);

			npcIcon.GetComponent<Image> ().enabled = false;

			dialogText.text = string.Empty;

			npcPlane.gameObject.SetActive (false);

		}

		public void QuitTradePlane(){

			itemIconInDetail.sprite = null;
			itemIconInDetail.enabled = false;
			itemIconBackgroundInDetail.enabled = false;
			itemNameInDetail.text = string.Empty;
			itemDescriptionInDetail.text = string.Empty;

			tradePlane.gameObject.SetActive (false);

		}
		
	}
}
