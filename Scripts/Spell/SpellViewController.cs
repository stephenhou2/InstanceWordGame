using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellViewController : MonoBehaviour {

	public void SetUpSpellView(){

		SpellView sv = GetComponent<SpellView> ();

		sv.SetUpSpellView ();

	}


}
