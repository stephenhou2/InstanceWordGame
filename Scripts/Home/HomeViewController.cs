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
			StartCoroutine ("SetUpViewAfterDataReady");
		}

		private IEnumerator SetUpViewAfterDataReady(){

			bool dataReady = false;

			while (!dataReady) {
				dataReady = GameManager.Instance.gameDataCenter.CheckDatasReady (new GameDataCenter.GameDataType[] {
					GameDataCenter.GameDataType.UISprites
				});
				yield return null;
			}

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

			GameLevelData levelData = GameManager.Instance.gameDataCenter.gameLevelDatas [Player.mainPlayer.currentLevelIndex];

			QuitHomeView();

			GameManager.Instance.UIManager.UnloadAllCanvasInSceneExcept(new string[]{"BagCanvas"});

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.exploreSceneBundleName, "ExploreCanvas", () => {

				TransformManager.FindTransform("ExploreManager").GetComponent<ExploreManager> ().SetupExploreView(levelData);

			},true,false);

//			StartCoroutine ("LoadExploreData");

		}

//		private IEnumerator LoadExploreData(){
//
//			yield return null;
//
//			GameLevelData levelData = GameManager.Instance.gameDataCenter.gameLevelDatas [Player.mainPlayer.currentLevelIndex];
//
//			QuitHomeView();
//
//			GameManager.Instance.UIManager.UnloadAllCanvasInSceneExcept(new string[]{"BagCanvas"});
//
//			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.exploreSceneBundleName, "ExploreCanvas", () => {
//
//				TransformManager.FindTransform("ExploreManager").GetComponent<ExploreManager> ().SetupExploreView(levelData);
//
//			},true,false);
//
//		}

		public void OnRecordButtonClick(){

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.recordCanvasBundleName, "RecordCanvas", () => {
				TransformManager.FindTransform("RecordCanvas").GetComponent<RecordViewController> ().SetUpRecordView();
				homeView.OnQuitHomeView();
			},false,true);
		}

		public void OnLearnButtonClick(){

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.learnCanvasBundleName, "LearnCanvas", () => {
				TransformManager.FindTransform("LearnCanvas").GetComponent<LearnViewController>().SetUpLearnView(false);
				homeView.OnQuitHomeView();
			},false,true);

		}

		public void OnMaterialsButtonClick(){

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.materialDisplayCanvasBundleName, "MaterialDisplayCanvas", () => {
				TransformManager.FindTransform("MaterialDisplayCanvas").GetComponent<MaterialDisplayViewController>().SetUpMaterialView();
				homeView.OnQuitHomeView();
			},false,true);
		}

		public void OnProduceButtonClick(){

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.workBenchCanvasBundleName, "WorkbenchCanvas", () => {
				TransformManager.FindTransform("WorkbenchCanvas").GetComponent<WorkBenchViewController> ().SetUpWorkBenchView();
				homeView.OnQuitHomeView();
			},false,true);
		}


		public void OnSkillButtonClick(){

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.skillCanvasBundleName, "SkillsCanvas", () => {
				TransformManager.FindTransform("SkillsCanvas").GetComponent<SkillsViewController>().SetUpSkillsView();
				homeView.OnQuitHomeView();
			},false,true);
		}

		public void OnBagButtonClick(){

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.bagCanvasBundleName, "BagCanvas", () => {
				TransformManager.FindTransform("BagCanvas").GetComponent<BagViewController> ().SetUpBagView (true);
				homeView.OnQuitHomeView();
			},false,true);
		}

		public void OnSettingButtonClick(){

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.settingCanvasBundleName, "SettingCanvas", () => {
				TransformManager.FindTransform("SettingCanvas").GetComponent<SettingViewController> ().SetUpSettingView ();
				homeView.OnQuitHomeView();
			},false,true);
		}

		public void OnMaterialProduceButtonClick(){

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.spellCanvasBundleName, "SpellCanvas", () => {
				TransformManager.FindTransform("SpellCanvas").GetComponent<SpellViewController>().SetUpSpellViewForCreateMaterial(null);
				homeView.OnQuitHomeView();
			},false,true);
		}

		public void OnFuseStoneButtonClick(){

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.spellCanvasBundleName, "SpellCanvas", () => {
				TransformManager.FindTransform("SpellCanvas").GetComponent<SpellViewController>().SetUpSpellViewForCreateFuseStone();
				homeView.OnQuitHomeView();
			},false,true);
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
