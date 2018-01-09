using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class CharactersInBagHUD : MonoBehaviour {

		public Text[] charactersCountArray;

		public void SetUpCharactersHUD(){
			for (int i = 0; i < charactersCountArray.Length; i++) {
				Text characterCount = charactersCountArray [i];
				int characterNum = Player.mainPlayer.charactersCount [i];
				characterCount.text = characterNum > 0 ? characterNum.ToString () : "";
			}

			gameObject.SetActive (true);
		}

		public void QuitCharactersHUD(){
			gameObject.SetActive (false);
		}

	}
}
