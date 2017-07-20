using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogAndItemViewController : MonoBehaviour {

	public DialogAndItemView dialogAndItemView;

	private NPC npc;

	private Item item;

	private Sprite npcOrItemSprite;



	public void SetUpDialogAndItemView(NPC npc,Sprite npcSprite,Item item,Sprite itemSprite){

		if (npc != null && item == null) {
			
			this.npc = npc;
			this.npcOrItemSprite = npcSprite;

			dialogAndItemView.SetUpDialogPlane (npc,npcSprite,0);

		} else if (item != null && npc == null) {

			this.item = item;
			this.npcOrItemSprite = itemSprite;
			
			dialogAndItemView.SetUpItemPlane (item, itemSprite);
		}

	}
		
	public void OnChoiceButtonOfDialogPlaneClick(Choice choice){

		dialogAndItemView.OnChoiceButtonClick ();


		if (choice.dialogId == -1) {
			QuitDialogAndItemPlane ();
			return;
		}
			
		int dialogId = choice.dialogId;

		dialogAndItemView.SetUpDialogPlane (npc, npcOrItemSprite, dialogId);

	}

	public void OnChoiceButtonOfItemPlaneClick(Choice choice){
		
		dialogAndItemView.OnChoiceButtonClick ();

		QuitDialogAndItemPlane ();

	}
	public void QuitDialogAndItemPlane(){

		npc = null;
		item = null;
		npcOrItemSprite = null;

		dialogAndItemView.OnQuitDialogAndItemPlane ();

		GetComponentInParent<ExploreMainViewController> ().OnNextEvent ();
	}


}
