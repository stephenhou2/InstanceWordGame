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

			ResourceLoader exploreSceneLoader = ResourceLoader.CreateNewResourceLoader ();

			ResourceManager.Instance.LoadAssetsWithBundlePath (exploreSceneLoader, "explore/scene", () => {

				TransformManager.FindTransform("ExploreManager").GetComponent<ExploreManager> ().SetupExploreView(currentExploreLevel);

				homeView.OnQuitHomeView();

			},true);

		}

		public void OnRecordButtonClick(){

			ResourceLoader recordCanvasLoader = ResourceLoader.CreateNewResourceLoader ();

			ResourceManager.Instance.LoadAssetsWithBundlePath (recordCanvasLoader, "record/canvas", () => {

				TransformManager.FindTransform("RecordCanvas").GetComponent<RecordViewController> ().SetUpRecordView();

				homeView.OnQuitHomeView();
			});


		}

		public void OnThinkingButtonClick(){

			ResourceLoader materialDisplayCanvasLoader = ResourceLoader.CreateNewResourceLoader ();

			ResourceManager.Instance.LoadAssetsWithBundlePath (materialDisplayCanvasLoader,"material/canvas", () => {
				
				TransformManager.FindTransform("MaterialDisplayCanvas").GetComponent<MaterialDisplayViewController>().SetUpMaterialView();
			
				homeView.OnQuitHomeView();
			});

		}

		public void OnProduceButtonClick(){

			ResourceLoader itemDisplayCanvasLoader = ResourceLoader.CreateNewResourceLoader ();

			ResourceManager.Instance.LoadAssetsWithBundlePath (itemDisplayCanvasLoader, "item/canvas", () => {

				TransformManager.FindTransform("ItemDisplayCanvas").GetComponent<ItemDisplayViewController> ().SetUpItemDisplayView();

				homeView.OnQuitHomeView();
			});

		}


		public void OnSkillButtonClick(){

			ResourceLoader skillCanvasLoader = ResourceLoader.CreateNewResourceLoader ();

			ResourceManager.Instance.LoadAssetsWithBundlePath (skillCanvasLoader, "skills/canvas", () => {

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

			ResourceManager.Instance.LoadAssetsWithBundlePath (bagCanvasLoader, "bag/canvas", () => {

				TransformManager.FindTransform("BagCanvas").GetComponent<BagViewController> ().SetUpBagView ();

				homeView.OnQuitHomeView();

			});
		}

		public void OnSettingButtonClick(){

			ResourceLoader settingCanvasLoader = ResourceLoader.CreateNewResourceLoader ();

			ResourceManager.Instance.LoadAssetsWithBundlePath (settingCanvasLoader, "setting/canvas", () => {

				TransformManager.FindTransform("SettingCanvas").GetComponent<SettingViewController> ().SetUpSettingView ();

				homeView.OnQuitHomeView();
			});
		}

		public void OnMaterialProduceButtonClick(){

			ResourceLoader spellCanvasLoader = ResourceLoader.CreateNewResourceLoader ();

			ResourceManager.Instance.LoadAssetsWithBundlePath (spellCanvasLoader, "spell/canvas", () => {

				TransformManager.FindTransform("SpellCanvas").GetComponent<SpellViewController>().SetUpSpellViewForCreateMaterial(null);

				homeView.OnQuitHomeView();

			});
		}

		public void OnFuseStoneButtonClick(){

			ResourceLoader spellCanvasLoader = ResourceLoader.CreateNewResourceLoader ();

			ResourceManager.Instance.LoadAssetsWithBundlePath (spellCanvasLoader, "spell/canvas", () => {

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
