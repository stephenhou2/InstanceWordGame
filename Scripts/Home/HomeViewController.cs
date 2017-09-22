using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



namespace WordJourney
{
	public class HomeViewController : MonoBehaviour {

		public HomeView homeView;

		public void SetUpHomeView(){

			homeView.SetUpHomeView ();

		}

		public void OnExploreButtonClick(){
			
			homeView.ShowMaskImage ();

			StartCoroutine ("LoadExploreData");
		}

		private IEnumerator LoadExploreData(){

			yield return null;

			int currentExploreLevel = GameManager.Instance.unlockedMaxChapterIndex;
			
			ResourceManager.Instance.LoadAssetWithBundlePath ("explore/scene", () => {

				GameObject.Find("ExploreManager").GetComponent<ExploreManager> ().SetupExploreView(currentExploreLevel);

				QuitHomeView();

			},true);

		}

		public void OnRecordButtonClick(){

			ResourceManager.Instance.LoadAssetWithBundlePath ("record/canvas", () => {

				ResourceManager.Instance.gos[0].GetComponent<RecordViewController> ().SetUpRecordView();

				homeView.OnQuitHomeView();
			});


		}

		public void OnThinkingButtonClick(){

			Debug.Log ("单词界面");

		}

		public void OnProduceButtonClick(){

			ResourceManager.Instance.LoadAssetWithBundlePath ("produce/canvas", () => {

				GameObject.Find(CommonData.instanceContainerName + "/ProduceCanvas").GetComponent<ProduceViewController> ().SetUpProduceView();

				homeView.OnQuitHomeView();
			});

		}



		public void OnSkillButtonClick(){

			ResourceManager.Instance.LoadAssetWithBundlePath ("skills/canvas", () => {

				ResourceManager.Instance.gos[0].GetComponent<SkillsViewController>().SetUpSkillsView();

				homeView.OnQuitHomeView();
			});

		}

		public void OnBagButtonClick(){

			GameObject bagCanvas = GameObject.Find ("BagCanvas");

			if (bagCanvas != null) {
				
				bagCanvas.GetComponent<BagViewController> ().SetUpBagView ();

				homeView.OnQuitHomeView();
			}

			ResourceManager.Instance.LoadAssetWithBundlePath ("bag/canvas", () => {

				ResourceManager.Instance.gos [0].GetComponent<BagViewController> ().SetUpBagView ();

				homeView.OnQuitHomeView();

			});
		}

		public void OnSettingButtonClick(){

			ResourceManager.Instance.LoadAssetWithBundlePath ("setting/canvas", () => {

				ResourceManager.Instance.gos [0].GetComponent<SettingViewController> ().SetUpSettingView ();

				homeView.OnQuitHomeView();
			});
		}

		private void QuitHomeView(){
			homeView.OnQuitHomeView (DestroyInstances);
		}

		private void DestroyInstances(){

			TransformManager.DestroyTransfromWithName ("HomeCanvas", TransformRoot.InstanceContainer);

		}

	}
}
