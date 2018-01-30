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
//			StartCoroutine ("SetUpViewAfterDataReady");
//		}
//
//		private IEnumerator SetUpViewAfterDataReady(){
//
//			bool dataReady = false;
//
//			while (!dataReady) {
//				dataReady = GameManager.Instance.gameDataCenter.CheckDatasReady (new GameDataCenter.GameDataType[] {
//					GameDataCenter.GameDataType.UISprites,
//					GameDataCenter.GameDataType.Skills
//				});
//				yield return null;
//			}

//			Player.mainPlayer.allEquipedEquipments [0] = null;

			homeView.SetUpHomeView ();

			Time.timeScale = 1f;


			if (!GameManager.Instance.soundManager.bgmAS.isPlaying 
				|| GameManager.Instance.soundManager.bgmAS.clip.name != CommonData.homeBgmName) {
				GameManager.Instance.soundManager.PlayBgmAudioClip (CommonData.homeBgmName, true);
			}else {
	

			}

//			Debug.Log (Player.mainPlayer.allEquipedEquipments [0]);

//			for (int i = 0; i < GameManager.Instance.gameDataCenter.allItemModels.Count; i++) {
//				ItemModel im = GameManager.Instance.gameDataCenter.allItemModels [i];
//				for (int j = 0; j < im.attachedSkillInfos.Length; j++) {
//					SkillGenerator.Instance.GeneratorSkill(null,im.attachedSkillInfos[j]);
//				}
//			}

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

			SoundManager.Instance.PlayAudioClip ("UI/sfx_UI_Paper");

			homeView.SetUpChapterSelectPlane ();

		}

		public void QuitChapterSelect(){
			homeView.QuitChapterSelectPlane ();
		}

		public void SelectChapter(int chapterIndex){

			SoundManager.Instance.PlayAudioClip ("UI/sfx_UI_Click");
			
			Player.mainPlayer.currentLevelIndex = 5 * chapterIndex;
//			Player.mainPlayer.currentLevelIndex = 4;

			homeView.ShowMaskImage ();

//			GameLevelData levelData = GameManager.Instance.gameDataCenter.gameLevelDatas [Player.mainPlayer.currentLevelIndex];
//
//			QuitHomeView();

//			GameManager.Instance.UIManager.UnloadAllCanvasInSceneExcept(new string[]{"BagCanvas"});
//
//			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.exploreSceneBundleName, "ExploreCanvas", () => {
//
//				TransformManager.FindTransform("ExploreManager").GetComponent<ExploreManager> ().SetUpExploreView(levelData);
//
//			},true,false);

			StartCoroutine ("LoadExploreData");


			#warning 下面这个代码是使用场景管理器方式加载探索界面
//			SceneManager.LoadSceneAsync ("ExploreScene", LoadSceneMode.Single);

		}

		private IEnumerator LoadExploreData(){

			yield return null;

			GameLevelData levelData = GameManager.Instance.gameDataCenter.gameLevelDatas [Player.mainPlayer.currentLevelIndex];

			QuitHomeView();

			GameManager.Instance.UIManager.UnloadAllCanvasInSceneExcept(new string[]{"BagCanvas"});

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.exploreSceneBundleName, "ExploreCanvas", () => {

				TransformManager.FindTransform("ExploreManager").GetComponent<ExploreManager> ().SetUpExploreView(levelData);

			},true,false);

		}

		public void OnRecordButtonClick(){
			SoundManager.Instance.PlayAudioClip ("UI/sfx_UI_Click");
			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.recordCanvasBundleName, "RecordCanvas", () => {
				TransformManager.FindTransform("RecordCanvas").GetComponent<RecordViewController> ().SetUpRecordView();
				homeView.OnQuitHomeView();
			},false,true);
		}

		public void OnLearnButtonClick(){
			SoundManager.Instance.PlayAudioClip ("UI/sfx_UI_Click");
			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.learnCanvasBundleName, "LearnCanvas", () => {
				TransformManager.FindTransform("LearnCanvas").GetComponent<LearnViewController>().SetUpLearnView();
				homeView.OnQuitHomeView();
			},false,true);

		}
			

		public void OnBagButtonClick(){
			SoundManager.Instance.PlayAudioClip ("UI/sfx_UI_Click");
			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.bagCanvasBundleName, "BagCanvas", () => {
				TransformManager.FindTransform("BagCanvas").GetComponent<BagViewController> ().SetUpBagView (true);
				homeView.OnQuitHomeView();
			},false,true);
		}

		public void OnSettingButtonClick(){
			SoundManager.Instance.PlayAudioClip ("UI/sfx_UI_Click");
			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.settingCanvasBundleName, "SettingCanvas", () => {
				TransformManager.FindTransform("SettingCanvas").GetComponent<SettingViewController> ().SetUpSettingView ();
				homeView.OnQuitHomeView();
			},false,true);
		}

		public void OnSpellButtonClick(){
			SoundManager.Instance.PlayAudioClip ("UI/sfx_UI_Click");
			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.spellCanvasBundleName, "SpellCanvas", () => {
//				ItemModel swordModel = GameManager.Instance.gameDataCenter.allItemModels[0];
				TransformManager.FindTransform("SpellCanvas").GetComponent<SpellViewController>().SetUpSpellViewForCreate(null,null);
				homeView.OnQuitHomeView();
			},false,true);
		}



		private void QuitHomeView(){
			homeView.OnQuitHomeView ();
		}

		public void DestroyInstances(){

			GameManager.Instance.UIManager.RemoveCanvasCache ("HomeCanvas");

			Destroy (this.gameObject);

			MyResourceManager.Instance.UnloadAssetBundle (CommonData.homeCanvasBundleName, true);

		}

	}
}
