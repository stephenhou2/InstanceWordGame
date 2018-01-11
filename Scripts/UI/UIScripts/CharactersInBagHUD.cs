using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;
	using DG.Tweening;

	public class CharactersInBagHUD : MonoBehaviour {

		public Text[] charactersCountArray;

		public Transform charactersContainer;

		private float zoomInDuration = 0.2f;

		private IEnumerator zoomInCoroutine;

		public void SetUpCharactersHUD(){
			
			for (int i = 0; i < charactersCountArray.Length; i++) {
				Text characterCount = charactersCountArray [i];
				int characterNum = Player.mainPlayer.charactersCount [i];
				characterCount.text = characterNum > 0 ? characterNum.ToString () : "";
			}

			charactersContainer.transform.localScale = new Vector3 (0.1f, 0.1f, 1);

			gameObject.SetActive (true);

			zoomInCoroutine = CharactersHUDZoomIn ();

			StartCoroutine (zoomInCoroutine);
		}

		public void QuitCharactersHUD(){
			if (zoomInCoroutine != null) {
				StopCoroutine (zoomInCoroutine);
			}
			gameObject.SetActive (false);
		}

		private IEnumerator CharactersHUDZoomIn(){

			float scale = charactersContainer.transform.localScale.x;

			float zoomInSpeed = (1 - scale) / zoomInDuration;

			float lastFrameRealTime = Time.realtimeSinceStartup;

			while (scale < 1) {

				yield return null;

				scale += zoomInSpeed * (Time.realtimeSinceStartup - lastFrameRealTime);

				lastFrameRealTime = Time.realtimeSinceStartup;

				charactersContainer.transform.localScale = new Vector3 (scale, scale, 1);

			}

			charactersContainer.transform.localScale = Vector3.one;

		}

	}
}
