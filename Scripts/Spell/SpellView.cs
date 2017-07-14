using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Text;
using System.Data;

public class SpellView: MonoBehaviour {

	public GameObject spellPlane;

	public Text[] characterTexts;

	private StringBuilder enteredCharacters = new StringBuilder();

	public void SetUpSpellView(){

	}

	public void OnEnterCharacter(string character){
		
		if (enteredCharacters.Length < characterTexts.Length) {
			enteredCharacters.Append (character);
			characterTexts [enteredCharacters.Length - 1].text = character;
		}

	}

	public void OnBackspace(){
		
		if (enteredCharacters.Length >= 1) {
			enteredCharacters.Remove (enteredCharacters.Length - 1, 1);
		}
			
		characterTexts [enteredCharacters.Length].text = string.Empty;

	}

	public void OnGenerate(){

		MySQLiteHelper sql = MySQLiteHelper.Instance;

		sql.GetConnectionWith ("test2.db");

		sql.InsertValues ("test", new string[]{"10","'sword'"});

		string condition = "name='" + enteredCharacters.ToString() + "'";

		IDataReader reader = sql.ReadSpecificRowsAndColsOfTable ("test", null, new string[]{ condition },true);

		while (reader.Read ()) {
			if(reader.GetString(1) != null){
				Item newItem = new Item ();
				newItem.itemName = enteredCharacters.ToString ();
				newItem.attackGain = reader.GetInt32 (0);

				Debug.Log (newItem);
			}
		}


		sql.CloseConnection ("test2.db");
	}

	public void OnQuitSpellPlane(){
		spellPlane.transform.DOLocalMoveY (-Screen.height, 0.5f).OnComplete (() => {
			Destroy (GameObject.Find ("SpellCanvas"));
		});
	}


}
