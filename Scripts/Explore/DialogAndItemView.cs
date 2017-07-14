using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogAndItemView : MonoBehaviour {

	public Transform dialogPlane;
	public Transform choicePlane;

	public Transform itemPlane;

	public Text dialogText;

	public Image itemIcon;

	public Text itemDescription;

	public GameObject choiceButton;

	private GameObject GetChoiceButton(){

		GameObject btn = null;
		if (buttonPool.Count != 0) {
			btn = buttonPool [0];
			buttonPool.RemoveAt (0);
			return btn;
		} 
		btn = Instantiate (choiceButton);
		btn.GetComponent<Button> ().onClick.AddListener (delegate {
			foreach (Transform trans in choicePlane.transform) {
				buttonPool.Add (trans.gameObject);
			}


			while(choicePlane.transform.childCount > 0){
				Transform trans = choicePlane.transform.GetChild(0);
				trans.SetParent(TransformManager.FindTransform(CommonData.poolContainerName));

			}

			dialogPlane.gameObject.SetActive(false);
			gameObject.SetActive (false);
			GameObject.Find("ExploreMainCanvas").GetComponent<ExploreMainViewController> ().OnNextEvent ();
		});

		return btn;

	}


	private List<GameObject> buttonPool = new List<GameObject> ();



	public void SetUpDialogPlane(NPC npc){

		dialogPlane.gameObject.SetActive (true);

		Dialog firstDialog = npc.dialogs [0];
		 
		dialogText.text = firstDialog.dialog;

		int[] choicesIds = firstDialog.choiceIds; 

		for (int i = 0; i < choicesIds.Length; i++) {
	
			Choice choice = npc.choices [i];
			GameObject btn = GetChoiceButton();
			btn.transform.SetParent (choicePlane);
			Text choiceText = btn.transform.FindChild ("Text").GetComponent<Text> ();
			choiceText.text = choice.choice;
		}


	}

	public void SetUpItemPlane(Item item,Sprite itemSprite){
		
		itemPlane.gameObject.SetActive (true);

		itemIcon.sprite = itemSprite;

		itemDescription.text = item.itemDescription;


		#warning 选择按钮代码后面再写
//		for (int i = 0; i < 2; i++) {
//			
//			GameObject btn = GetChoiceButton();
//
//			Text choiceText = btn.transform.FindChild ("Text").GetComponent<Text> ();
//
//
//			btn.GetComponent<Button> ().onClick.AddListener (delegate {
//				foreach (Transform trans in choicePlane.transform) {
//					buttonPool.Add (trans.gameObject);
//				}
//				choicePlane.DetachChildren();
//				dialogPlane.gameObject.SetActive(false);
//				gameObject.SetActive (false);
//				GameObject.Find("ExploreCanvas").GetComponent<ExploreMainViewController> ().OnResetExploreChapterView ();
//			});
//
//		}

	}

}
