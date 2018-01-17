using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;
	using DG.Tweening;

	public class CharactersInBagHUD : MonoBehaviour {

		public Transform characterModel;

		private InstancePool characterPool;

		public Transform charactersDisplayContainer;

		public Transform charactersContainer;

		private float zoomInDuration = 0.2f;

		private IEnumerator zoomInCoroutine;

		public void InitCharactersInBagHUD(){
			characterPool = InstancePool.GetOrCreateInstancePool ("CharacterPool", CommonData.poolContainerName);
		}

		public void SetUpCharactersHUD(){

			SoundManager.Instance.PlayAudioClip ("UI/sfx_UI_Paper");

			characterPool.AddChildInstancesToPool (charactersContainer);
			
			for (int i = 0; i < Player.mainPlayer.charactersCount.Length; i++) {
				
				int count = Player.mainPlayer.charactersCount [i];

				if (count > 0) {
					AddCharacterToPaper ((char)(i + CommonData.aInASCII), count);
				}
					
			}

			charactersDisplayContainer.transform.localScale = new Vector3 (0.1f, 0.1f, 1);

			gameObject.SetActive (true);

			zoomInCoroutine = CharactersHUDZoomIn ();

			StartCoroutine (zoomInCoroutine);
		}

		private void AddCharacterToPaper(char character,int count){

			Transform characterCell = characterPool.GetInstance<Transform> (characterModel.gameObject, charactersContainer);

			characterCell.GetComponent<CharacterCell> ().SetUpCharacterCell (character, count);

		}

		public void QuitCharactersHUD(){
			if (zoomInCoroutine != null) {
				StopCoroutine (zoomInCoroutine);
			}
			gameObject.SetActive (false);
		}

		private IEnumerator CharactersHUDZoomIn(){

			float scale = charactersDisplayContainer.transform.localScale.x;

			float zoomInSpeed = (1 - scale) / zoomInDuration;

			float lastFrameRealTime = Time.realtimeSinceStartup;

			while (scale < 1) {

				yield return null;

				scale += zoomInSpeed * (Time.realtimeSinceStartup - lastFrameRealTime);

				lastFrameRealTime = Time.realtimeSinceStartup;

				charactersDisplayContainer.transform.localScale = new Vector3 (scale, scale, 1);

			}

			charactersDisplayContainer.transform.localScale = Vector3.one;

		}

	}
}
