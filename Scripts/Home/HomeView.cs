using System.Collections;
using System.Collections.Generic;
using UnityEngine;




namespace WordJourney
{

	using UnityEngine.UI;

	public class HomeView : MonoBehaviour {

//		public Text playerLevelText;

//		public Slider playerHealthBar;

		public Image maskImage;

		public Transform chapterSelectPlane;
		public Transform chapterSelectHUD;
		public Transform chaptersContainer;

		private Transform chapterButtonModel;
		private InstancePool chapterButtonPool;

		public float chapterSelectPlaneZoomInDuration = 0.2f;

		public void SetUpHomeView(){

			Transform poolContainerOfHomeCanvas = TransformManager.FindOrCreateTransform (CommonData.poolContainerName + "/PoolContainerOfHomeCanvas");
			Transform modelContainerOfHomeCanvas = TransformManager.FindOrCreateTransform (CommonData.instanceContainerName + "/ModelContainerOfHomeCanvas");


			if (poolContainerOfHomeCanvas.childCount == 0) {
				chapterButtonPool = InstancePool.GetOrCreateInstancePool ("ChapterButtonPool", poolContainerOfHomeCanvas.name);
			}

			if (modelContainerOfHomeCanvas.childCount == 0) {
				chapterButtonModel = TransformManager.FindTransform ("ChapterButtonModel");
				chapterButtonModel.SetParent (modelContainerOfHomeCanvas);
			}

			GetComponent<Canvas> ().enabled = true;

			SetUpTopBar ();

		}
		public void ShowMaskImage (){
			maskImage.gameObject.SetActive (true);
		}

		private void HideMaskImage(){
			maskImage.gameObject.SetActive (false);
		}

//		private void VotexRotate(){
//			votexRotate = votexImage.DOLocalRotate (new Vector3 (0, 0, 360),10.0f, RotateMode.FastBeyond360);
//			votexRotate.SetLoops (-1);
//			votexRotate.SetEase (Ease.Linear);
//		}


		// 初始化顶部bar
		private void SetUpTopBar(){

			Player player = Player.mainPlayer;

//			playerLevelText.text = player.agentLevel.ToString();
//
//			playerHealthBar.maxValue = player.maxHealth;
//			playerHealthBar.value = player.health;
//			playerHealthBar.transform.Find ("HealthText").GetComponent<Text> ().text = player.health + "/" + Player.mainPlayer.maxHealth;

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

			Debug.Log (Time.time);

			while (chapterSelectHUDScale < 1) {
				float zoomInDelta = chapterSelectHUDZoomSpeed * Time.deltaTime;
				chapterSelectHUD.localScale += new Vector3 (zoomInDelta, zoomInDelta, 0);
				chapterSelectHUDScale += zoomInDelta;
				yield return null;
			}

			Debug.Log (Time.time);

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
