using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogAndItemView : MonoBehaviour {

	public Transform dialogPlane;
	public Transform choicePlane;

	public Transform itemPlane;

	public Text dialogText;

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
			Debug.Log ("here");
			foreach (Transform trans in choicePlane.transform) {
				buttonPool.Add (trans.gameObject);
			}
			choicePlane.DetachChildren();
			gameObject.SetActive (false);
			GameObject.Find("ExploreCanvas").GetComponent<ExploreMainViewController> ().OnResetExploreChapterView ();
		});
		return btn;

	}

	public Image itemIcon;

	public Text itemDescription;

	private List<GameObject> buttonPool = new List<GameObject> ();



	public void SetUpDialogPlane(NPC npc){

		Debug.Log ("setup");

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

	public void SetUpItemPlane(Item item){
		itemPlane.gameObject.SetActive (true);
	}


	private void DialogChoiceButtonClicked(){

	}

//	public void MakeDialogChoice(){
//
//	}

}
