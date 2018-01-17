using System.Collections;
using System.Collections.Generic;
using UnityEngine;




namespace WordJourney
{

	using UnityEngine.UI;

	public class HomeView : MonoBehaviour {


		public Text wordTypeText;
		public Text coinCount;

		public Image maskImage;

		public Transform chapterSelectPlane;
		public Transform chaptersContainer;

		public Button[] chapterButtons;


		public float chapterSelectPlaneZoomInDuration = 0.2f;

		public void SetUpHomeView(){

			SetUpBasicInformation ();

			GetComponent<Canvas> ().enabled = true;

		}

		private void SetUpBasicInformation(){

			wordTypeText.text = GameManager.Instance.gameDataCenter.gameSettings.GetWordTypeString ();

			coinCount.text = Player.mainPlayer.totalCoins.ToString ();

		}


		public void ShowMaskImage (){
			maskImage.gameObject.SetActive (true);
		}



		private void HideMaskImage(){
			maskImage.gameObject.SetActive (false);
		}
			

		public void SetUpChapterSelectPlane(){

			int maxUnlockChapterIndex = Player.mainPlayer.maxUnlockLevelIndex / 5;

			for (int i = 0; i < chapterButtons.Length; i++) {

				Button chapterButton = chapterButtons [i];

				Text chapterNameText = chapterButton.GetComponentInChildren<Text> ();

				if (i <= maxUnlockChapterIndex) {
					
					chapterButton.interactable = true;

					string chapterName = GameManager.Instance.gameDataCenter.gameLevelDatas [5 * i].chapterName;

					string chapterIndexInChinese = MyTool.NumberToChinese (i + 1);

					string fullName = string.Format ("第{0}章  {1}", chapterIndexInChinese, chapterName);

					chapterNameText.text = fullName;

				} else {
					chapterButton.interactable = false;
					chapterNameText.text = "? ? ? ?";
				}

			}

			chaptersContainer.localScale = new Vector3 (0.1f, 0.1f, 1);

			chapterSelectPlane.gameObject.SetActive (true);

			IEnumerator chapterSelectZoomInCoroutine = ChapterSelectHUDZoomIn ();

			StartCoroutine (chapterSelectZoomInCoroutine);

		}

		private IEnumerator ChapterSelectHUDZoomIn(){
			

			float chapterSelectHUDScale = chaptersContainer.localScale.x;

			float chapterSelectHUDZoomSpeed = (1 - chapterSelectHUDScale) / chapterSelectPlaneZoomInDuration;

			while (chapterSelectHUDScale < 1) {
				float zoomInDelta = chapterSelectHUDZoomSpeed * Time.deltaTime;
				chaptersContainer.localScale += new Vector3 (zoomInDelta, zoomInDelta, 0);
				chapterSelectHUDScale += zoomInDelta;
				yield return null;
			}

			chaptersContainer.localScale = Vector3.one;

		}

		public void QuitChapterSelectPlane(){
			chapterSelectPlane.gameObject.SetActive (false);
		}

		public void OnQuitHomeView(){

			chapterSelectPlane.gameObject.SetActive (false);

			HideMaskImage ();

		}

	}
}
