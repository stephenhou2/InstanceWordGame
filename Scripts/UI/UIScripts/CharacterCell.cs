using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class CharacterCell : MonoBehaviour {

		public Image characterIcon;
		public Text characterCount;

		public void SetUpCharacterCell(char character,int count){

			Sprite characterSprite = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
				return obj.name == string.Format("character_{0}",character.ToString());
			});

			characterIcon.sprite = characterSprite;

			characterIcon.enabled = characterSprite != null;

			characterCount.text = count.ToString ();

		}

	}
}
