using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SpellView: MonoBehaviour {

	public GameObject spellPlane;

	public Text[] characters;

	private List<string> enteredCharacters = new List<string>();

	public void SetUpSpellView(){

	}

	public void OnEnterCharacter(string character){

		enteredCharacters.Add (character);

		for (int i = 0; i < enteredCharacters.Count; i++) {
			characters [i].text = enteredCharacters [i];
		}

	}

	public void OnBackspace(){
		if (enteredCharacters.Count >= 1) {
			enteredCharacters.RemoveAt (enteredCharacters.Count - 1);
		}
		foreach (Text t in characters) {
			t.text = "";
		}
		for (int i = 0; i < enteredCharacters.Count; i++) {
			characters [i].text = enteredCharacters [i];
		}
	}

	public void OnGenerate(){
		
	}

	public void OnQuitSpellPlane(){
		spellPlane.transform.DOLocalMoveY (-Screen.height, 0.5f).OnComplete (() => {
			Destroy (GameObject.Find ("SpellCanvas"));
		});
	}


}
