using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace WordJourney
{
	public class ExploreUICotroller : MonoBehaviour {


//		// 图片遮罩
//		public Image maskImage;

		public Transform tintHUD;

		public Transform mask;

		/**********  battlePlane UI *************/
		public Transform battlePlane;

		/**********  battlePlane UI *************/

		/**********  dialogPlane UI *************/
		public Transform dialogPlane;
		public Transform choiceContainer;
		public Image npcIcon;
		public Text dialogText;
		private Transform choiceButtonModel;
		public Button nextDialogButton;
		/**********  dialogPlane UI *************/


		/**********  RewardPlane UI *************/
//		public Transform rewardPlane;
//		public Transform rewardContainer;
//		private Transform rewardButtonModel;
		/**********  RewardPlane UI *************/


		public Transform formulaDetailPlane;

		public Transform materialCardContainer;
		private Transform materialCardModel;

		private InstancePool choiceButtonPool;
//		private InstancePool rewardButtonPool;
		private InstancePool materialCardPool;

//		private NPC currentEnteredNpc;
		private DialogGroup currentDialogGroup;

		private int currentDialogId;

//		private Dialog[] dialogs;
//		private Choice[] choices;

//		private List<Item> itemsToPickUp = new List<Item>();
		private Item itemToPickUp;

		private bool nextButtonEndDialog;

		public void SetUpExploreCanvas(){

			Initialize ();

		}

			
		private void Initialize(){

			Transform poolContainerOfExploreCanvas = TransformManager.FindOrCreateTransform (CommonData.poolContainerName + "/PoolContainerOfExploreCanvas");
			Transform modelContainerOfExploreScene = TransformManager.FindOrCreateTransform (CommonData.instanceContainerName + "/ModelContainerOfExploreScene");

			choiceButtonPool = InstancePool.GetOrCreateInstancePool ("ChoiceButtonPool",poolContainerOfExploreCanvas.name);
//			rewardButtonPool = InstancePool.GetOrCreateInstancePool ("RewardButtonPool",poolContainerOfExploreCanvas.name);
			materialCardPool = InstancePool.GetOrCreateInstancePool ("MaterialCardPool", poolContainerOfExploreCanvas.name);

			choiceButtonModel = TransformManager.FindTransform ("ChoiceButtonModel");
//			rewardButtonModel = TransformManager.FindTransform ("RewardButtonModel");
			materialCardModel = TransformManager.FindTransform ("MaterialCardModel");


			choiceButtonModel.SetParent (modelContainerOfExploreScene);
//			rewardButtonModel.SetParent (modelContainerOfExploreScene);
			materialCardModel.SetParent (modelContainerOfExploreScene);

			if (!GameManager.Instance.UIManager.UIDic.ContainsKey ("BagCanvas")) {

				GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.bagCanvasBundleName, "BagCanvas", () => {
					Transform bagCanvas = TransformManager.FindTransform ("BagCanvas");
					bagCanvas.GetComponent<BagViewController> ().SetUpBagView (false);
				}, true,true);
			}

		}


		public void ShowFightPlane(){
			battlePlane.gameObject.SetActive (true);
		}

		public void HideFightPlane(){
			battlePlane.gameObject.SetActive (false);
		}


		public void SetUpTintHUD(string tint){
//			HideMask ();
			tintHUD.gameObject.SetActive (true);
			Text tintText = tintHUD.GetComponentInChildren<Text> ();
			tintText.color = Color.black;
			tintText.text = tint;

			StartCoroutine ("PlayTintTextAnim", tintText);

		}

		private IEnumerator PlayTintTextAnim(Text tintText){

			yield return new WaitForSeconds (1f);

			tintText.text = string.Empty;
			tintHUD.gameObject.SetActive (false);

		}

		public void EnterNPC(NPC npc,int currentLevelIndex){

//			currentEnteredNpc = npc;

			currentDialogId = 0;

			dialogPlane.gameObject.SetActive (true);

			DialogGroup dg = null;

			for (int i = 0; i < npc.dialogGroups.Length; i++) {
				if (npc.dialogGroups [i].accordGameLevel == currentLevelIndex) {
					dg = npc.dialogGroups [i];
				}
					
			}

			if (dg == null) {
				Debug.LogError (string.Format ("第{0}关没有npc{1}", currentLevelIndex, npc.npcName));
			}

			currentDialogGroup = dg;


//			dialogs = dg.dialogs;
//
//			Dialog dialog = dialogs [0];

			Sprite npcSprite = GameManager.Instance.gameDataCenter.allMapSprites.Find (delegate(Sprite s) {
				return s.name == npc.spriteName;
			});

			if (npcSprite != null) {
				npcIcon.sprite = npcSprite;
				npcIcon.GetComponent<Image> ().enabled = true;
			}


			SetUpDialogPlane (currentDialogGroup.dialogs[0]);

		}

		private void SetUpDialogPlane(Dialog dialog){

			dialogText.text = dialog.dialog;

			if (dialog.choices.Length == 0) {
				nextDialogButton.gameObject.SetActive (true);
			} else {
				Choice[] choices = dialog.choices;
				for (int i = 0; i < choices.Length; i++) {
					
					Choice choice = choices [i];

					Button choiceButton = choiceButtonPool.GetInstance<Button> (choiceButtonModel.gameObject, choiceContainer);

					choiceButton.GetComponentInChildren<Text> ().text = choice.choice;

					choiceButton.onClick.RemoveAllListeners ();

					choiceButton.onClick.AddListener (delegate() {
						MakeChoice(choice);
					});
				}
			}

			if (dialog.rewardIds.Length != 0) {
				for (int i = 0; i < dialog.rewardIds.Length; i++) {
					Item rewardItem = Item.NewItemWith (dialog.rewardIds [i], dialog.rewardCounts [i]);
					Player.mainPlayer.AddItem (rewardItem);
					#warning 提示获得物品的界面逻辑没有做
				}
			}


		}

		public void OnNextDialogButtonClick (){

			if (currentDialogGroup.dialogs [currentDialogId].isEndingDialog) {
				QuitDialogPlane ();
				return;
			}

			currentDialogId++;

			SetUpDialogPlane(currentDialogGroup.dialogs[currentDialogId]);
		}



		private void MakeChoice(Choice choice){

			if (choice.isEnd) {
				QuitDialogPlane ();
				return;
			}

			int dialogId = choice.dialogId;

			currentDialogId = dialogId;

			Dialog dialog = currentDialogGroup.dialogs [dialogId];

			choiceButtonPool.AddChildInstancesToPool (choiceContainer);

			SetUpDialogPlane (dialog);

		}


		private void QuitDialogPlane(){

			choiceButtonPool.AddChildInstancesToPool (choiceContainer);

			npcIcon.GetComponent<Image> ().enabled = false;

			dialogText.text = string.Empty;

			dialogPlane.gameObject.SetActive (false);

		}

//		public void ShowMask(){
//			mask.gameObject.SetActive (true);
//		}
//
//		public void HideMask(){
//			mask.gameObject.SetActive (false);
//		}



//		public void SetUpRewardItemsPlane(Item rewardItem){
//
//			itemToPickUp = null;
//
//			if (rewardItem == null) {
//				HideMask ();
//				return;
//			}
////
////			for (int i = 0; i < rewardItems.Length; i++) {
////
////				Item rewardItem = rewardItems [i];
//				
//				Button rewardButton = rewardButtonPool.GetInstance<Button> (rewardButtonModel.gameObject, rewardContainer);
//
//				Image rewardItemIcon = rewardButton.transform.Find ("ItemIcon").GetComponent<Image> ();
//
//				Sprite rewardSprite = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite s) {
//					return s.name == rewardItem.spriteName;
//				});
//					
//
//				if (rewardSprite != null) {
//					rewardItemIcon.sprite = rewardSprite;
//				} 
//
//				rewardButton.GetComponentInChildren<Text> ().text = rewardItem.itemName;
//				rewardButton.transform.Find ("SelectIcon").gameObject.SetActive (true);
////				itemsToPickUp.Add (rewardItem);
//				rewardButton.onClick.AddListener (delegate {
//					ChangeRewardSelection(rewardButton,rewardItem);
//				});
//
////			}
//				
//			HideMask ();
//			rewardPlane.gameObject.SetActive (true);
//
//		}

//		private void ChangeRewardSelection(Button rewardButton,Item rewardItem){
//
//			Image selectionIcon = rewardButton.transform.Find ("SelectIcon").GetComponent<Image>();
//
//			if (selectionIcon.IsActive()) {
//				selectionIcon.gameObject.SetActive (false);
//				itemsToPickUp.Remove (rewardItem);
//			} else {
//				selectionIcon.gameObject.SetActive (true);
//				itemsToPickUp.Add (rewardItem);
//			}
//
//		}

//		public void DiscardAllItems(){
//
//			OnQuitRewardPlane ();
//
//		}

//		public void PickUpSelected(){
//
//			for (int i = 0; i < itemsToPickUp.Count; i++) {
//				Player.mainPlayer.AddItem (itemsToPickUp [i]);
//			}
//
//			OnQuitRewardPlane ();
//			GetComponent<BattlePlayerUIController> ().UpdateItemButtons ();
//
//		}

//		private void OnQuitRewardPlane(){
//
//			rewardButtonPool.AddChildInstancesToPool (rewardContainer);
//
//			rewardPlane.gameObject.SetActive (false);
//
//		}


		public void SetUpRewardFormulaPlane(Formula formula){

			ItemModel equipment = GameManager.Instance.gameDataCenter.allItemModels.Find (delegate(ItemModel obj) {
				return obj.itemId == formula.itemOrSkillId;
			});

			Transform formulaDetailContainer = formulaDetailPlane.Find ("FormulaDetailContainer");

			Text equipmentName = formulaDetailContainer.Find ("EquipmentName").GetComponent<Text> ();
			Image equipmentIcon = formulaDetailContainer.Find ("EquipmentIcon").GetComponent<Image> ();
			Text equipmentDescription = formulaDetailContainer.Find ("EquipmentDescription").GetComponent<Text> ();

			equipmentName.text = equipment.itemName;
			equipmentDescription.text = equipment.itemDescription;

			Sprite s = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
				return obj.name == equipment.spriteName;
			});

			equipmentIcon.sprite = s;

			materialCardPool.AddChildInstancesToPool (materialCardContainer);

			for (int i = 0; i < equipment.materials.Count; i++) {
				
				Transform materialCard = materialCardPool.GetInstance<Transform> (materialCardModel.gameObject, materialCardContainer);

				Material material = equipment.materials [i];

				Image materialIcon = materialCard.Find ("MaterialIcon").GetComponent<Image> ();
				Text materialName = materialCard.Find ("MaterialName").GetComponent<Text> ();

				materialName.text = material.itemName;

				Sprite materialSprite = GameManager.Instance.gameDataCenter.allMaterialSprites.Find (delegate(Sprite obj) {
					return obj.name == material.spriteName;
				});

				materialIcon.sprite = materialSprite;


			}

			Button confirmButton = formulaDetailContainer.Find ("ConfirmButton").GetComponent<Button> ();

			confirmButton.onClick.RemoveAllListeners ();

			confirmButton.onClick.AddListener (delegate() {
				AddFormulaAndQuitFormulaDetailPlane (formula);
			});

			formulaDetailPlane.gameObject.SetActive (true);
		}

		private void AddFormulaAndQuitFormulaDetailPlane(Formula formula){

			Player.mainPlayer.AddItem (formula);

			formulaDetailPlane.gameObject.SetActive (false);

		}


		public void SetUpWorkBenchPlane(){

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.workBenchCanvasBundleName, "WorkbenchCanvas", () => {
				TransformManager.FindTransform ("WorkbenchCanvas").GetComponent<WorkBenchViewController> ().SetUpWorkBenchView ();
			}, false, true);

		}

		public void SetUpLearnPlane(){
			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.learnCanvasBundleName, "LearnCanvas", () => {
				TransformManager.FindTransform("LearnCanvas").GetComponent<LearnViewController>().SetUpLearnView();

			}, false, true);
		}


//		public void HideFightPlane(){
//			GetComponent<BattlePlayerUIController> ().QuitFight ();
//			GetComponent<BattleMonsterUIController>().QuitFight ();
//		}

		public void QuitFight(){
			GetComponent<BattlePlayerUIController> ().QuitFight ();
			GetComponent<BattleMonsterUIController>().QuitFight ();
			HideFightPlane ();
		}


		public void EnterNextLevel(){
			TransformManager.FindTransform ("ExploreManager").GetComponent<ExploreManager> ().EnterNextLevel ();
		}

		public void QuitExplore(){

			TransformManager.DestroyTransform(transform);

			DestroyInstances ();

			Resources.UnloadUnusedAssets ();

			System.GC.Collect ();

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.homeCanvasBundleName, "HomeCanvas", () => {

				TransformManager.FindTransform("HomeCanvas").GetComponent<HomeViewController> ().SetUpHomeView ();

			});

		}


		#warning 测试用，退出按钮直接退出探索界面
		public void QuitExploreWithTP(){
			TransformManager.FindTransform ("ExploreManager").GetComponent<ExploreManager> ().QuitExploreScene ();
		}

		private void DestroyInstances(){
			GameManager.Instance.UIManager.DestroryCanvasWith (CommonData.exploreSceneBundleName, "ExploreCanvas", "PoolContainerOfExploreScene", "ModelContainerOfExploreScene");
		}


	}
}
