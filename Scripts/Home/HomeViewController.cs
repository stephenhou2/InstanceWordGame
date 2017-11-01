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

//		private void SetUpCanvasWith(string bundleName,string canvasName,CallBack cb){
//			
//			Transform canvas = TransformManager.FindTransform (canvasName);
//
//			if (canvas != null) {
//
//				cb ();
//
//				homeView.OnQuitHomeView();
//
//				return;
//			}
//
//			ResourceLoader loader = ResourceLoader.CreateNewResourceLoader ();
//
//			ResourceManager.Instance.LoadAssetsWithBundlePath (loader, bundleName, () => {
//
//				cb();
//
//				homeView.OnQuitHomeView();
//			});
//
//		}

		public void OnExploreButtonClick(){
			
			homeView.ShowMaskImage ();

			StartCoroutine ("LoadExploreData");

//			SceneManager.LoadSceneAsync ("ExploreScene", LoadSceneMode.Single);

		}

		private IEnumerator LoadExploreData(){

			yield return null;

			int currentExploreLevel = GameManager.Instance.unlockedMaxChapterIndex;

//			ResourceLoader exploreSceneLoader = ResourceLoader.CreateNewResourceLoader ();

			QuitHomeView();

			GameManager.Instance.UIManager.UnloadAllCanvasInSceneExcept(new string[]{"BagCanvas"});

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.exploreSceneBundleName, "ExploreCanvas", () => {

				TransformManager.FindTransform("ExploreManager").GetComponent<ExploreManager> ().SetupExploreView(currentExploreLevel);

			},true);

		}

		public void OnRecordButtonClick(){

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.recordCanvasBundleName, "RecordCanvas", () => {
				TransformManager.FindTransform("RecordCanvas").GetComponent<RecordViewController> ().SetUpRecordView();
				homeView.OnQuitHomeView();
			});
		}

		public void OnThinkingButtonClick(){

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.materialDisplayCanvasBundleName, "MaterialDisplayCanvas", () => {
				TransformManager.FindTransform("MaterialDisplayCanvas").GetComponent<MaterialDisplayViewController>().SetUpMaterialView();
				homeView.OnQuitHomeView();
			});
		}

		public void OnProduceButtonClick(){

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.itemDisplayCanvasBundleName, "ItemDisplayCanvas", () => {
				TransformManager.FindTransform("ItemDisplayCanvas").GetComponent<ItemDisplayViewController> ().SetUpItemDisplayView();
				homeView.OnQuitHomeView();
			});
		}


		public void OnSkillButtonClick(){

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.skillCanvasBundleName, "SkillsCanvas", () => {
				TransformManager.FindTransform("SkillsCanvas").GetComponent<SkillsViewController>().SetUpSkillsView();
				homeView.OnQuitHomeView();
			});
		}

		public void OnBagButtonClick(){

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.bagCanvasBundleName, "BagCanvas", () => {
				TransformManager.FindTransform("BagCanvas").GetComponent<BagViewController> ().SetUpBagView ();
				homeView.OnQuitHomeView();
			});
		}

		public void OnSettingButtonClick(){

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.settingCanvasBundleName, "SettingCanvas", () => {
				TransformManager.FindTransform("SettingCanvas").GetComponent<SettingViewController> ().SetUpSettingView ();
				homeView.OnQuitHomeView();
			});
		}

		public void OnMaterialProduceButtonClick(){

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.spellCanvasBundleName, "SpellCanvas", () => {
				TransformManager.FindTransform("SpellCanvas").GetComponent<SpellViewController>().SetUpSpellViewForCreateMaterial(null);
				homeView.OnQuitHomeView();
			});
		}

		public void OnFuseStoneButtonClick(){

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.spellCanvasBundleName, "SpellCanvas", () => {
				TransformManager.FindTransform("SpellCanvas").GetComponent<SpellViewController>().SetUpSpellViewForCreateFuseStone();
				homeView.OnQuitHomeView();
			});
		}


		private void QuitHomeView(){
			homeView.OnQuitHomeView ();
			DestroyInstances ();
		}

		public void DestroyInstances(){

			GameManager.Instance.UIManager.DestroryCanvasWith (CommonData.homeCanvasBundleName, "HomeCanvas", null, null);

		}

	}
}
