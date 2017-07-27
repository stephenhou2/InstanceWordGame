using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogAndItemView : MonoBehaviour {

	public Transform dialogPlane;
	public Transform itemPlane;

	public Transform choicePlane;

	public Text dialogText;

	public Image itemIcon;

	public Text itemDescription;

	public GameObject choiceButtonModel;

	private InstancePool choiceButtonPool;


	public void SetUpDialogPlane(NPC npc,Sprite npcSprite,int dialogId){

		gameObject.SetActive (true);

		dialogPlane.gameObject.SetActive (true);

		choiceButtonPool = InstancePool.GetOrCreateInstancePool ("ChoiceButtonPool");

		Dialog dialog = npc.dialogs [dialogId];
		 
		dialogText.text = dialog.dialog;

		int[] choicesIds = dialog.choiceIds; 

		for (int i = 0; i < choicesIds.Length; i++) {

			int choiceId = choicesIds [i];
	
			Choice choice = npc.choices [choiceId];

			Button choiceButton = choiceButtonPool.GetInstance<Button> (choiceButtonModel, choicePlane);

			Text choiceText = choiceButton.transform.FindChild ("Text").GetComponent<Text> ();

			choiceButton.onClick.RemoveAllListeners ();

			choiceButton.onClick.AddListener (delegate {
				GetComponent<DialogAndItemViewController>().OnChoiceButtonOfDialogPlaneClick(choice);
			});

			choiceText.text = choice.choice;
		}


	}


	public void SetUpItemPlane(Item item,Sprite itemSprite){

		gameObject.SetActive (true);

		itemPlane.gameObject.SetActive (true);

		choiceButtonPool = InstancePool.GetOrCreateInstancePool ("ChoiceButtonPool");

		itemIcon.sprite = itemSprite;

		if (itemSprite != null) {
			itemIcon.enabled = true;
		}

		itemDescription.text = item.itemDescription;


		#warning 选择按钮代码后面再写
		for (int i = 0; i < 2; i++) {
			
			Button choiceButton = choiceButtonPool.GetInstance<Button> (choiceButtonModel, choicePlane);

			Text choiceText = choiceButton.transform.FindChild ("Text").GetComponent<Text> ();

			choiceButton.onClick.RemoveAllListeners ();

			choiceButton.onClick.AddListener (delegate {
				GetComponent<DialogAndItemViewController>().OnChoiceButtonOfItemPlaneClick(null);
			});

			choiceText.text = "hello";

		}

	}


	public void OnChoiceButtonClick(){

		ResetChoiceButton ();

		choiceButtonPool.AddChildInstancesToPool(choicePlane);

	}

	private void ResetChoiceButton(){

		for (int i = 0; i < choicePlane.childCount; i++) {

			Transform trans = choicePlane.GetChild (i);

			trans.GetComponentInChildren<Text> ().text = string.Empty;
		}

	}


	public void OnQuitDialogAndItemPlane(){

		ResetDialogAndItemPlane ();

		gameObject.SetActive (false);
		dialogPlane.gameObject.SetActive (false);
		itemPlane.gameObject.SetActive (false);


	}

	private void ResetDialogAndItemPlane(){

		dialogText.text = string.Empty;

		itemIcon.sprite = null;

		itemIcon.enabled = false;

		itemDescription.text = string.Empty;

	}


}
