using System.Collections;
using System.Collections.Generic;
using UnityEngine;




namespace WordJourney
{

	using UnityEngine.UI;

	public class HomeView : MonoBehaviour {

		public Image maskImage;

		public Transform chapterSelectPlane;
		public Transform chapterSelectHUD;
		public Transform chaptersContainer;

		public Transform chapterButtonModel;
		private InstancePool chapterButtonPool;

		public float chapterSelectPlaneZoomInDuration = 0.2f;

		public void SetUpHomeView(){
			
			chapterButtonPool = InstancePool.GetOrCreateInstancePool ("ChapterButtonPool", CommonData.poolContainerName);

			GetComponent<Canvas> ().enabled = true;

		}
		public void ShowMaskImage (){
			maskImage.gameObject.SetActive (true);
		}

		private void HideMaskImage(){
			maskImage.gameObject.SetActive (false);
		}
			

		public void SetUpChapterSelectPlane(){

			int maxUnlockChapterIndex = Player.mainPlayer.maxUnlockLevelIndex / 5;

			for (int i = 0; i < maxUnlockChapterIndex + 1; i++) {

				int chapterLevelIndex = i;

				Button chapterButton = chapterButtonPool.GetInstance<Button> (chapterButtonModel.gameObject, chaptersContainer);

				Image lockImage = chapterButton.transform.Find ("LockImage").GetComponent<Image> ();

//				lockImage.enabled = 

				chapterButton.GetComponentInChildren<Text> ().text = GameManager.Instance.gameDataCenter.gameLevelDatas [5 * i].chapterName;

				chapterButton.onClick.RemoveAllListeners ();

				chapterButton.onClick.AddListener (delegate {
					GetComponent<HomeViewController>().SelectChapter(chapterLevelIndex);
				});

			}

			chapterSelectHUD.localScale = new Vector3 (0.1f, 0.1f, 1);

			chapterSelectPlane.gameObject.SetActive (true);

			IEnumerator chapterSelectZoomInCoroutine = ChapterSelectHUDZoomIn ();

			StartCoroutine (chapterSelectZoomInCoroutine);

		}

		private IEnumerator ChapterSelectHUDZoomIn(){
			

			float chapterSelectHUDScale = chapterSelectHUD.localScale.x;

			float chapterSelectHUDZoomSpeed = (1 - chapterSelectHUDScale) / chapterSelectPlaneZoomInDuration;

//			Debug.Log (Time.time);

			while (chapterSelectHUDScale < 1) {
				float zoomInDelta = chapterSelectHUDZoomSpeed * Time.deltaTime;
				chapterSelectHUD.localScale += new Vector3 (zoomInDelta, zoomInDelta, 0);
				chapterSelectHUDScale += zoomInDelta;
				yield return null;
			}

//			Debug.Log (Time.time);

			chapterSelectHUD.localScale = Vector3.one;

		}

		public void QuitChapterSelectPlane(){
			chapterButtonPool.AddChildInstancesToPool (chaptersContainer);
			chapterSelectPlane.gameObject.SetActive (false);
		}

		public void OnQuitHomeView(){

			chapterButtonPool.AddChildInstancesToPool (chaptersContainer);

			chapterSelectPlane.gameObject.SetActive (false);

			HideMaskImage ();

		}

	}
}
