using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterBattleButton : MonoBehaviour {

	public void OnClick(){
		GameManager.gameManager.OnEnterBattle ();
	}
}
