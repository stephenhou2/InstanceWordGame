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
					GameDataCenter.GameDataType.UISprites,
					GameDataCenter.GameDataType.Skills
				});
				yield return null;
			}

//			Player.mainPlayer.allEquipedEquipments [0] = null;

			homeView.SetUpHomeView ();

			Debug.Log (Player.mainPlayer.allEquipedEquipments [0]);

//			for (int i = 0; i < GameManager.Instance.gameDataCenter.allItemModels.Count; i++) {
//				ItemModel im = GameManager.Instance.gameDataCenter.allItemModels [i];
//				for (int j = 0; j < im.attachedSkillInfos.Length; j++) {
//					SkillGenerator.Instance.GeneratorSkill(null,im.attachedSkillInfos[j]);
//				}
//			}

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

		public void QuitChapterSelect(){
			homeView.QuitChapterSelectPlane ();
		}

		public void SelectChapter(int chapterIndex){
			
			Player.mainPlayer.currentLevelIndex = 5 * chapterIndex;

			homeView.ShowMaskImage ();

			GameLevelData levelData = GameManager.Instance.gameDataCenter.gameLevelDatas [Player.mainPlayer.currentLevelIndex];

			QuitHomeView();

			#warning 下面这个代码是使用场景管理器方式加载探索界面
//			SceneManager.LoadSceneAsync ("ExploreScene", LoadSceneMode.Single);

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

			homeView.StartLearnCountDown ();

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.learnCanvasBundleName, "LearnCanvas", () => {
				TransformManager.FindTransform("LearnCanvas").GetComponent<LearnViewController>().SetUpLearnView();
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

		public void OnSpellButtonClick(){

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.spellCanvasBundleName, "SpellCanvas", () => {
				ItemModel swordModel = GameManager.Instance.gameDataCenter.allItemModels[0];
				TransformManager.FindTransform("SpellCanvas").GetComponent<SpellViewController>().SetUpSpellViewForCreate(swordModel);
				homeView.OnQuitHomeView();
			},false,true);
		}


		public void OnUnlockedItemsButtonClick(){
			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.unlockedItemsCanvasBundleName, "UnlockedItemsCanvas", () => {
				TransformManager.FindTransform("UnlockedItemsCanvas").GetComponent<UnlockedItemsViewController>().SetUpUnlockedItemsView();
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
