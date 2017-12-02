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


		public void OnSaveButtonClick(){

			GameManager.Instance.persistDataManager.SavePersistDatas ();


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

			homeView.SetUpChapterSelectPlane ();

		}

		public void SelectChapter(int chapterIndex){
			
			Player.mainPlayer.currentLevelIndex = 5 * chapterIndex;

			homeView.ShowMaskImage ();

			StartCoroutine ("LoadExploreData");

		}

		private IEnumerator LoadExploreData(){

			yield return null;

			GameLevelData levelData = GameManager.Instance.gameDataCenter.gameLevelDatas [Player.mainPlayer.currentLevelIndex];

			QuitHomeView();

			GameManager.Instance.UIManager.UnloadAllCanvasInSceneExcept(new string[]{"BagCanvas"});

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.exploreSceneBundleName, "ExploreCanvas", () => {

				TransformManager.FindTransform("ExploreManager").GetComponent<ExploreManager> ().SetupExploreView(levelData);

			},true);

		}

		public void OnRecordButtonClick(){

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.recordCanvasBundleName, "RecordCanvas", () => {
				TransformManager.FindTransform("RecordCanvas").GetComponent<RecordViewController> ().SetUpRecordView();
				homeView.OnQuitHomeView();
			});
		}

		public void OnLearnButtonClick(){

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.learnCanvasBundleName, "LearnCanvas", () => {
				TransformManager.FindTransform("LearnCanvas").GetComponent<LearnViewController>().SetUpLearnView();
				homeView.OnQuitHomeView();
			});

		}

		public void OnMaterialsButtonClick(){

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.materialDisplayCanvasBundleName, "MaterialDisplayCanvas", () => {
				TransformManager.FindTransform("MaterialDisplayCanvas").GetComponent<MaterialDisplayViewController>().SetUpMaterialView();
				homeView.OnQuitHomeView();
			});
		}

		public void OnProduceButtonClick(){

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.workBenchCanvasBundleName, "WorkbenchCanvas", () => {
				TransformManager.FindTransform("WorkbenchCanvas").GetComponent<WorkBenchViewController> ().SetUpWorkBenchView();
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
				TransformManager.FindTransform("BagCanvas").GetComponent<BagViewController> ().SetUpBagView (true);
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

			GameManager.Instance.UIManager.DestroryCanvasWith (CommonData.homeCanvasBundleName, "HomeCanvas", "PoolContainerOfHomeCanvas","ModelContainerOfHomeCanvas");

		}

	}
}
