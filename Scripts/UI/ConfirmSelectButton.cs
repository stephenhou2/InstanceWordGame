using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmSelectButton:MonoBehaviour {

	public void OnClick(){
		GameManager gm = GameManager.gameManager;
		gm.OnConfirm ();
	}
}
