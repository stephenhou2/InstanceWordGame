using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace WordJourney
{
	public class ExploreUICotroller : MonoBehaviour {


		// 图片遮罩
		public Image maskImage;

		public Transform tintHUD;


		private BattlePlayerUIController bpUICtr;
		private BattleMonsterUIController bmUICtr;


		/**********  dialogPlane UI *************/
		public Transform dialogPlane;
		public Transform choiceContainer;
		public Image npcIcon;
		public Text dialogText;
		public GameObject choiceButtonModel;
		public Button nextDialogButton;
		/**********  dialogPlane UI *************/


		/**********  RewardPlane UI *************/
		public Transform rewardPlane;
		public Transform rewardContainer;
		public GameObject rewardButtonModel;
		/**********  RewardPlane UI *************/

		private InstancePool choiceButtonPool;
		private InstancePool rewardButtonPool;

		private Dialog[] dialogs;
		private Choice[] choices;

		private List<Item> itemsToPickUp;

		void Awake(){

			bpUICtr = GetComponent<BattlePlayerUIController>();

			bmUICtr = GetComponent<BattleMonsterUIController> ();

			choiceButtonPool = InstancePool.GetOrCreateInstancePool ("ChoiceButtonPool");

			rewardButtonPool = InstancePool.GetOrCreateInstancePool ("RewardButtonPool");

		}


		public void SetUpExploreUI(){

			bpUICtr.SetUpUI (Player.mainPlayer, GameManager.Instance.allSkillSprites, GameManager.Instance.allItemSprites);



		}

		//Hides black image used between levels
		public void HideMaskImage()
		{
			maskImage.enabled = false;
		}



		public void SetUpTintHUD(string unlockItemName){
			
			tintHUD.gameObject.SetActive (true);
			Text tintText = tintHUD.GetComponent<Text> ();
			tintText.text = string.Format ("好像需要使用<color=orange>{0}</color>才能打开", unlockItemName);
			tintText.DOFade (0, 0.5f).OnComplete (() => {
				tintText.text = string.Empty;
				tintHUD.gameObject.SetActive (false);
			});

		}

		public void EnterNPC(NPC npc,int dialogGroupIndex){

			dialogPlane.gameObject.SetActive (true);

			dialogs = npc.dialogGroups [dialogGroupIndex].dialogs;

			choices = npc.choices;

			Dialog dialog = npc.dialogGroups [dialogGroupIndex].dialogs [0];

			Sprite npcSprite = GameManager.Instance.allMapSprites.Find (delegate(Sprite s) {
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

				Button choiceButton = choiceButtonPool.GetInstance<Button> (choiceButtonModel, choiceContainer);

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



		public void SetUpRewardItemsPlane(Item[] rewardItems){

			itemsToPickUp.Clear ();

			if (rewardItems == null || rewardItems.Length == 0) {
				return;
			}

			for (int i = 0; i < rewardItems.Length; i++) {

				Item rewardItem = rewardItems [i];
				
				Button rewardButton = rewardButtonPool.GetInstance<Button> (rewardButtonModel, rewardContainer);

				Sprite rewardSprite = GameManager.Instance.allItemSprites.Find (delegate(Sprite s) {
					return s.name == rewardItem.spriteName;
				});

				if (rewardSprite != null) {
					rewardButton.transform.FindChild("ItemIcon").GetComponent<Image> ().sprite = rewardSprite;
					rewardButton.GetComponentInChildren<Text> ().text = rewardItem.itemName;
					rewardButton.transform.FindChild ("SelectIcon").gameObject.SetActive (true);
					itemsToPickUp.Add (rewardItem);
					rewardButton.onClick.AddListener (delegate {
						ChangeRewardSelection(rewardButton,rewardItem);
					});
				} else {
					rewardButtonPool.AddInstanceToPool (rewardButton.gameObject);
				}

			}

			rewardPlane.gameObject.SetActive (true);

		}

		private void ChangeRewardSelection(Button rewardButton,Item rewardItem){

			Image selectionIcon = rewardButton.transform.FindChild ("SelectionIcon").GetComponent<Image>();

			if (selectionIcon.IsActive()) {
				selectionIcon.gameObject.SetActive (false);
				itemsToPickUp.Remove (rewardItem);
			} else {
				selectionIcon.gameObject.SetActive (true);
				itemsToPickUp.Add (rewardItem);
			}

		}

		public void DiscardAllItems(){

			QuitRewardPlane ();

		}

		public void PickUpSelected(){

			Player.mainPlayer.AddItems (itemsToPickUp);

			QuitRewardPlane ();

		}

		private void QuitRewardPlane(){

			rewardButtonPool.AddChildInstancesToPool (rewardContainer);

			rewardPlane.gameObject.SetActive (false);

		}
	}
}
