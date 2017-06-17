using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WordGenerator : MonoBehaviour {


	public Text wordText;


	private Word[] words;

	void Awake(){
		words = DataInitializer.LoadDataWithPath<Word> ("Assets", "WordsTestJson.txt");
	}


	public void UpdateWordText(string charactor){

		wordText.text += charactor;

	}

	public void OnConfirm(){

		string word = wordText.text.ToLower();

		foreach (Word w in words) {
			if (w.word == word) {

				Debug.Log ("succeed make " + word + w.description);
				return;
			}

		}

		Debug.Log ("there is not this word");

	}

}

