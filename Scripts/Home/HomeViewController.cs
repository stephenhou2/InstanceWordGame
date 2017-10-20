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

				homeView.OnQuitHomeView();

			},true);

		}

		public void OnRecordButtonClick(){

			ResourceManager.Instance.LoadAssetWithBundlePath ("record/canvas", () => {

				ResourceManager.Instance.gos[0].GetComponent<RecordViewController> ().SetUpRecordView();

				homeView.OnQuitHomeView();
			});


		}

		public void OnThinkingButtonClick(){

			ResourceManager.Instance.LoadAssetWithBundlePath ("material/canvas", () => {
				
				GameObject.Find(CommonData.instanceContainerName + "/MaterialDisplayCanvas").GetComponent<MaterialDisplayViewController>().SetUpMaterialView();
			
				homeView.OnQuitHomeView();
			});

		}

		public void OnProduceButtonClick(){

			ResourceManager.Instance.LoadAssetWithBundlePath ("item/canvas", () => {

				GameObject.Find(CommonData.instanceContainerName + "/ItemDisplayCanvas").GetComponent<ItemDisplayViewController> ().SetUpItemDisplayView();

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

		public void OnMaterialProduceButtonClick(){
			
			ResourceManager.Instance.LoadAssetWithBundlePath ("spell/canvas", () => {

				TransformManager.FindTransform("SpellCanvas").GetComponent<SpellViewController>().SetUpSpellViewForCreateMaterial(null);

				homeView.OnQuitHomeView();

			});
		}

		public void OnFuseStoneButtonClick(){
			
			ResourceManager.Instance.LoadAssetWithBundlePath ("spell/canvas", () => {

				TransformManager.FindTransform("SpellCanvas").GetComponent<SpellViewController>().SetUpSpellViewForCreateFuseStone();

				homeView.OnQuitHomeView();

			});
		}


		private void QuitHomeView(){
			homeView.OnQuitHomeView (DestroyInstances);
		}

		private void DestroyInstances(){

			TransformManager.DestroyTransfromWithName ("HomeCanvas", TransformRoot.InstanceContainer);

			Resources.UnloadUnusedAssets ();

			System.GC.Collect ();

		}

	}
}
