using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellController : MonoBehaviour {

	public void OnEnterSpellView(){

		SpellView sv = GetComponent<SpellView> ();

		sv.SetUpSpellView ();

	}


}
