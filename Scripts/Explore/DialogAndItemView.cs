using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogAndItemView : MonoBehaviour {

	public Transform dialogPlane;
	public Transform choicePlane;

	public Transform itemPlane;

	public Text dialogText;

	public Button choiceButton;

	public Image itemIcon;

	public Text itemDescription;



	public void SetUpDialogPlane(NPC npc){
		dialogPlane.gameObject.SetActive (true);
		Dialog firstDialog = npc.dialogs [0];
		 
		dialogText.text = firstDialog.dialog;

		int[] choicesIds = firstDialog.choiceIds; 
		for (int i = 0; i < choicesIds.Length; i++) {
			Choice choice = npc.choices [i];
			Button mChoiceButton = Instantiate (choiceButton,choicePlane.transform);
			Text choiceText = mChoiceButton.transform.FindChild ("Text").GetComponent<Text> ();
			choiceText.text = choice.choice;
		}


	}

	public void SetUpItemPlane(Item item){
		itemPlane.gameObject.SetActive (true);
	}

//	public void MakeDialogChoice(){
//
//	}

}
