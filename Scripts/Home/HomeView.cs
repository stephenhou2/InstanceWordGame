using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace WordJourney
{
	public class HomeView : MonoBehaviour {


		public Text playerLevelText;

		public Slider playerHealthBar;

		public Transform votexImage;

		private Tweener votexRotate;

		public Image maskImage;

		public Transform chapterSelectPlane;
		public Transform chaptersContainer;

		private Transform chapterButtonModel;
		private InstancePool chapterButtonPool;

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

			VotexRotate ();

		}
		public void ShowMaskImage (){
			maskImage.gameObject.SetActive (true);
		}

		private void HideMaskImage(){
			maskImage.gameObject.SetActive (false);
		}

		private void VotexRotate(){
			votexRotate = votexImage.DOLocalRotate (new Vector3 (0, 0, 360),10.0f, RotateMode.FastBeyond360);
			votexRotate.SetLoops (-1);
			votexRotate.SetEase (Ease.Linear);
		}


		// 初始化顶部bar
		private void SetUpTopBar(){

			Player player = Player.mainPlayer;

			playerLevelText.text = player.agentLevel.ToString();

			playerHealthBar.maxValue = player.maxHealth;
			playerHealthBar.value = player.health;
			playerHealthBar.transform.Find ("HealthText").GetComponent<Text> ().text = player.health + "/" + Player.mainPlayer.maxHealth;

		}

		public void SetUpChapterSelectPlane(){

			int maxUnlockLevelIndex = Player.mainPlayer.maxUnlockChapterIndex / 5;

			for (int i = 0; i < maxUnlockLevelIndex + 1; i++) {

				int chapterLevelIndex = i;

				Button chapterButton = chapterButtonPool.GetInstance<Button> (chapterButtonModel.gameObject, chaptersContainer);

				chapterButton.GetComponentInChildren<Text> ().text = GameManager.Instance.gameDataCenter.chapterDetails [5 * i].chapterLocation;

				chapterButton.onClick.RemoveAllListeners ();

				chapterButton.onClick.AddListener (delegate {
					GetComponent<HomeViewController>().SelectChapter(5 * chapterLevelIndex);
				});

			}

			chapterSelectPlane.gameObject.SetActive (true);

		}

		public void OnQuitHomeView(){

			chapterButtonPool.AddChildInstancesToPool (chaptersContainer);

			chapterSelectPlane.gameObject.SetActive (false);

			gameObject.SetActive(false);

			votexImage.localRotation = Quaternion.identity;

			votexRotate.Kill (false);

			HideMaskImage ();

		}

	}
}
