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
		public Transform rewardPlane;
		public Transform rewardContainer;
		private Transform rewardButtonModel;
		/**********  RewardPlane UI *************/

		private InstancePool choiceButtonPool;
		private InstancePool rewardButtonPool;

		private Dialog[] dialogs;
		private Choice[] choices;

		private List<Item> itemsToPickUp = new List<Item>();


		public void SetUpExploreCanvas(){

			Initialize ();

		}

			
		private void Initialize(){

			Transform poolContainerOfExploreScene = TransformManager.FindOrCreateTransform (CommonData.poolContainerName + "/PoolContainerOfExploreScene");
			Transform modelContainerOfExploreScene = TransformManager.FindOrCreateTransform (CommonData.instanceContainerName + "/ModelContainerOfExploreScene");

			choiceButtonPool = InstancePool.GetOrCreateInstancePool ("ChoiceButtonPool");
			rewardButtonPool = InstancePool.GetOrCreateInstancePool ("RewardButtonPool");

			choiceButtonPool.transform.SetParent (poolContainerOfExploreScene);
			rewardButtonPool.transform.SetParent (poolContainerOfExploreScene);

			choiceButtonModel = TransformManager.FindTransform ("ChoiceButtonModel");
			rewardButtonModel = TransformManager.FindTransform ("RewardButtonModel");

			choiceButtonModel.SetParent (modelContainerOfExploreScene);
			rewardButtonModel.SetParent (modelContainerOfExploreScene);

			if (!GameManager.Instance.UIManager.UIDic.ContainsKey ("BagCanvas")) {

				GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.bagCanvasBundleName, "BagCanvas", () => {

					Transform bagCanvas = TransformManager.FindTransform ("BagCanvas");
					bagCanvas.GetComponent<BagViewController> ().SetUpBagView ();
					bagCanvas.GetComponent<Canvas>().enabled = false;
				}, true);
			}

		}


		public void ShowFightPlane(){

			battlePlane.gameObject.SetActive (true);

		}


		public void SetUpTintHUD(string unlockItemName){
			HideMask ();
			tintHUD.gameObject.SetActive (true);
			Text tintText = tintHUD.GetComponentInChildren<Text> ();
			tintText.color = Color.black;
			tintText.text = string.Format ("缺少 <color=blue>{0}x1</color>", unlockItemName);

			StartCoroutine ("PlayTintTextAnim", tintText);

		}

		private IEnumerator PlayTintTextAnim(Text tintText){

			yield return new WaitForSeconds (1f);

			tintText.text = string.Empty;
			tintHUD.gameObject.SetActive (false);

		}

		public void EnterNPC(NPC npc,int dialogGroupIndex){

			dialogPlane.gameObject.SetActive (true);

			dialogs = npc.dialogGroups [dialogGroupIndex].dialogs;

			choices = npc.choices;

			Dialog dialog = npc.dialogGroups [dialogGroupIndex].dialogs [0];

			Sprite npcSprite = GameManager.Instance.dataCenter.allMapSprites.Find (delegate(Sprite s) {
				return s.name == npc.spriteName;
			});

			if (npcSprite != null) {
				npcIcon.sprite = npcSprite;
				npcIcon.GetComponent<Image> ().enabled = true;
			}


			SetUpDialogPlane (dialog);

		}

		private void SetUpDialogPlane(Dialog dialog){

			dialogText.text = dialog.dialog;

			int[] choiceIds = dialog.choiceIds;

			for (int i = 0; i < choiceIds.Length; i++) {

				int choiceId = choiceIds [i];

				Choice choice = choices [choiceId];

				Button choiceButton = choiceButtonPool.GetInstance<Button> (choiceButtonModel.gameObject, choiceContainer);

				choiceButton.GetComponentInChildren<Text> ().text = choice.choice;

				choiceButton.onClick.RemoveAllListeners ();

				choiceButton.onClick.AddListener (delegate() {
					MakeChoice(choice);
				});


			}



		}



		private void MakeChoice(Choice choice){

			if (choice.dialogId == -1) {
				QuitDialogPlane ();
				return;
			}

			Dialog dialog = dialogs [choice.dialogId];

			choiceButtonPool.AddChildInstancesToPool (choiceContainer);

			SetUpDialogPlane (dialog);


		}


		private void QuitDialogPlane(){

			choiceButtonPool.AddChildInstancesToPool (choiceContainer);

			npcIcon.GetComponent<Image> ().enabled = false;

			dialogText.text = string.Empty;

			dialogPlane.gameObject.SetActive (false);

		}

		public void ShowMask(){
			mask.gameObject.SetActive (true);
		}

		public void HideMask(){
			mask.gameObject.SetActive (false);
		}


		public void SetUpRewardItemsPlane(Item[] rewardItems){

			itemsToPickUp.Clear ();

			if (rewardItems == null || rewardItems.Length == 0) {
				HideMask ();
				return;
			}

			for (int i = 0; i < rewardItems.Length; i++) {

				Item rewardItem = rewardItems [i];
				
				Button rewardButton = rewardButtonPool.GetInstance<Button> (rewardButtonModel.gameObject, rewardContainer);

				Sprite rewardSprite = GameManager.Instance.dataCenter.allItemSprites.Find (delegate(Sprite s) {
					return s.name == rewardItem.spriteName;
				});
					

				if (rewardSprite != null) {
					Image rewardItemIcon = rewardButton.transform.Find ("ItemIcon").GetComponent<Image> ();
					rewardItemIcon.sprite = rewardSprite;
					rewardItemIcon.enabled = true;
					rewardButton.GetComponentInChildren<Text> ().text = rewardItem.itemName;
					rewardButton.transform.Find ("SelectIcon").gameObject.SetActive (true);
					itemsToPickUp.Add (rewardItem);
					rewardButton.onClick.AddListener (delegate {
						ChangeRewardSelection(rewardButton,rewardItem);
					});
				} else {
					rewardButtonPool.AddInstanceToPool (rewardButton.gameObject);
				}

			}
				
			HideMask ();
			rewardPlane.gameObject.SetActive (true);

		}

		private void ChangeRewardSelection(Button rewardButton,Item rewardItem){

			Image selectionIcon = rewardButton.transform.Find ("SelectIcon").GetComponent<Image>();

			if (selectionIcon.IsActive()) {
				selectionIcon.gameObject.SetActive (false);
				itemsToPickUp.Remove (rewardItem);
			} else {
				selectionIcon.gameObject.SetActive (true);
				itemsToPickUp.Add (rewardItem);
			}

		}

		public void DiscardAllItems(){

			OnQuitRewardPlane ();

		}

		public void PickUpSelected(){

//			player.AddItems (itemsToPickUp);

			OnQuitRewardPlane ();

		}

		private void OnQuitRewardPlane(){

			rewardButtonPool.AddChildInstancesToPool (rewardContainer);

			rewardPlane.gameObject.SetActive (false);

		}


		public void HideFightPlane(){
			GetComponent<BattlePlayerUIController> ().QuitFight ();
			GetComponent<BattleMonsterUIController>().QuitFight ();
		}

		public void QuitFight(){
	
			battlePlane.gameObject.SetActive (false);
		}

		public void QuitExplore(){

			TransformManager.FindTransform ("ExploreManager").GetComponent<ExploreManager> ().OnQuitExplore();

			TransformManager.DestroyTransform(transform);

			DestroyInstances ();

			Resources.UnloadUnusedAssets ();

			System.GC.Collect ();

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.homeCanvasBundleName, "HomeCanvas", () => {

				TransformManager.FindTransform("HomeCanvas").GetComponent<HomeViewController> ().SetUpHomeView ();

			});

		}


		private void DestroyInstances(){
			GameManager.Instance.UIManager.DestroryCanvasWith (CommonData.exploreSceneBundleName, "ExploreCanvas", "PoolContainerOfExploreScene", "ModelContainerOfExploreScene");
		}


	}
}
