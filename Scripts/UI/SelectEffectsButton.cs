using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectEffectsButton : MonoBehaviour {

	public void OnClick(){

		GameManager.gameManager.OnSelectEffects (0,1);
	}
}
