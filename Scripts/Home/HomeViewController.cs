using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;


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

//			SceneManager.LoadSceneAsync ("ExploreScene", LoadSceneMode.Single);

		}

		private IEnumerator LoadExploreData(){

			yield return null;

			int currentExploreLevel = GameManager.Instance.unlockedMaxChapterIndex;

			ResourceLoader exploreSceneLoader = ResourceLoader.CreateNewResourceLoader ();

			ResourceManager.Instance.LoadAssetsWithBundlePath (exploreSceneLoader, CommonData.exploreSceneBundleName, () => {

				TransformManager.FindTransform("ExploreManager").GetComponent<ExploreManager> ().SetupExploreView(currentExploreLevel);

				QuitHomeView();

			},true);

		}

		public void OnRecordButtonClick(){

			ResourceLoader recordCanvasLoader = ResourceLoader.CreateNewResourceLoader ();

			ResourceManager.Instance.LoadAssetsWithBundlePath (recordCanvasLoader, CommonData.recordCanvasBundleName, () => {

				TransformManager.FindTransform("RecordCanvas").GetComponent<RecordViewController> ().SetUpRecordView();

				homeView.OnQuitHomeView();
			});


		}

		public void OnThinkingButtonClick(){

			ResourceLoader materialDisplayCanvasLoader = ResourceLoader.CreateNewResourceLoader ();

			ResourceManager.Instance.LoadAssetsWithBundlePath (materialDisplayCanvasLoader,CommonData.materialDiaplayCanvasBundleName, () => {
				
				TransformManager.FindTransform("MaterialDisplayCanvas").GetComponent<MaterialDisplayViewController>().SetUpMaterialView();
			
				homeView.OnQuitHomeView();
			});

		}

		public void OnProduceButtonClick(){

			ResourceLoader itemDisplayCanvasLoader = ResourceLoader.CreateNewResourceLoader ();

			ResourceManager.Instance.LoadAssetsWithBundlePath (itemDisplayCanvasLoader, CommonData.itemDisplayCanvasBundleName, () => {

				TransformManager.FindTransform("ItemDisplayCanvas").GetComponent<ItemDisplayViewController> ().SetUpItemDisplayView();

				homeView.OnQuitHomeView();
			});

		}


		public void OnSkillButtonClick(){

			ResourceLoader skillCanvasLoader = ResourceLoader.CreateNewResourceLoader ();

			ResourceManager.Instance.LoadAssetsWithBundlePath (skillCanvasLoader, CommonData.skillCanvasBundleName, () => {

				TransformManager.FindTransform("SkillsCanvas").GetComponent<SkillsViewController>().SetUpSkillsView();

				homeView.OnQuitHomeView();
			});

		}

		public void OnBagButtonClick(){

			Transform bagCanvas = TransformManager.FindTransform ("BagCanvas");

			if (bagCanvas != null) {
				
				bagCanvas.GetComponent<BagViewController> ().SetUpBagView ();

				homeView.OnQuitHomeView();
			}

			ResourceLoader bagCanvasLoader = ResourceLoader.CreateNewResourceLoader ();

			ResourceManager.Instance.LoadAssetsWithBundlePath (bagCanvasLoader, CommonData.bagCanvasBundleName, () => {

				TransformManager.FindTransform("BagCanvas").GetComponent<BagViewController> ().SetUpBagView ();

				homeView.OnQuitHomeView();

			});
		}

		public void OnSettingButtonClick(){

			ResourceLoader settingCanvasLoader = ResourceLoader.CreateNewResourceLoader ();

			ResourceManager.Instance.LoadAssetsWithBundlePath (settingCanvasLoader, CommonData.settingCanvasBundleName, () => {

				TransformManager.FindTransform("SettingCanvas").GetComponent<SettingViewController> ().SetUpSettingView ();

				homeView.OnQuitHomeView();
			});
		}

		public void OnMaterialProduceButtonClick(){

			ResourceLoader spellCanvasLoader = ResourceLoader.CreateNewResourceLoader ();

			ResourceManager.Instance.LoadAssetsWithBundlePath (spellCanvasLoader, CommonData.spellCanvasBundleName, () => {

				TransformManager.FindTransform("SpellCanvas").GetComponent<SpellViewController>().SetUpSpellViewForCreateMaterial(null);

				homeView.OnQuitHomeView();

			});
		}

		public void OnFuseStoneButtonClick(){

			ResourceLoader spellCanvasLoader = ResourceLoader.CreateNewResourceLoader ();

			ResourceManager.Instance.LoadAssetsWithBundlePath (spellCanvasLoader, CommonData.spellCanvasBundleName, () => {

				TransformManager.FindTransform("SpellCanvas").GetComponent<SpellViewController>().SetUpSpellViewForCreateFuseStone();

				homeView.OnQuitHomeView();

			});
		}


		private void QuitHomeView(){
			homeView.OnQuitHomeView (DestroyInstances);
		}

		private void DestroyInstances(){

			ResourceManager.Instance.UnloadCaches (CommonData.homeCanvasBundleName, true);

			Destroy (this.gameObject);

			Resources.UnloadUnusedAssets ();

			System.GC.Collect ();

		}

	}
}
