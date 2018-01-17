using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class SettingViewController : MonoBehaviour{

		public SettingView settingView;

		private bool isPointerUp;

		private bool settingChanged;

		private int currentSelectWordTypeIndex;


		public void SetUpSettingView(){
//			SoundManager.Instance.PlayAudioClip ("UI/sfx_UI_Click");
			StartCoroutine ("SetUpViewAfterDataReady");

		}

		private IEnumerator SetUpViewAfterDataReady(){
			
			bool dataReady = false;

			while (!dataReady) {
				dataReady = GameManager.Instance.gameDataCenter.CheckDatasReady (new GameDataCenter.GameDataType[] {
					GameDataCenter.GameDataType.UISprites,
					GameDataCenter.GameDataType.GameSettings
				});
				yield return null;
			}

			GameSettings settings = GameManager.Instance.gameDataCenter.gameSettings;

			settingView.SetUpSettingView (settings);

		}
			

		public void ChangeVolume(){

			GameManager.Instance.gameDataCenter.gameSettings.systemVolume = (int)settingView.volumeControl.value;

			settingChanged = true;

		}

		public void SetPronunciationEnable(bool enable){

			GameManager.Instance.gameDataCenter.gameSettings.isPronunciationEnable = enable;

			settingView.UpdatePronounceControl (enable);

			settingChanged = true;

		}
			

		public void ChangeWordType(int index){
			int wordTypeIndex = (int)GameManager.Instance.gameDataCenter.gameSettings.wordType;
			if (wordTypeIndex != index) {
				currentSelectWordTypeIndex = index;
				settingView.ShowAlertHUD ();

			}
		}

		public void OnConfirmButtonClick(){
			
//			int wordTypeIndex = settingView.GetCurrentWordType (index);
//
//			if (wordTypeIndex == -1) {
//				return;
//			}

			settingView.QuitAlertHUD ();

			GameManager.Instance.persistDataManager.ResetPlayerDataToOriginal ();

			switch (currentSelectWordTypeIndex) {
			case 0:
				GameManager.Instance.gameDataCenter.gameSettings.wordType = WordType.CET4;
				break;
			case 1:
				GameManager.Instance.gameDataCenter.gameSettings.wordType = WordType.CET6;
				break;
			case 2:
				GameManager.Instance.gameDataCenter.gameSettings.wordType = WordType.Daily;
				break;
			case 3:
				GameManager.Instance.gameDataCenter.gameSettings.wordType = WordType.Bussiness;
				break;

			}

			ExploreManager exploreManager = TransformManager.FindTransform ("ExploreManager").GetComponent<ExploreManager> ();
			if (exploreManager != null) {
				exploreManager.QuitExploreScene (false);
			}

			settingChanged = true;
			OnQuitSettingViewButtonClick ();

		}

		public void OnCancelButtonClick(){
			settingView.SetUpSettingView (GameManager.Instance.gameDataCenter.gameSettings);
			settingView.QuitAlertHUD ();
		}


		public void OnQuitSettingViewButtonClick(){



			if (settingChanged) {
				ChangeSettingsAndSave ();
			}

			settingChanged = false;

			settingView.QuitSettingView ();

			Transform exploreCanvas = TransformManager.FindTransform ("ExploreCanvas");
			if (exploreCanvas == null) {
				GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.homeCanvasBundleName, "HomeCanvas", () => {
					TransformManager.FindTransform ("HomeCanvas").GetComponent<HomeViewController> ().SetUpHomeView ();
				});
			} else {
				exploreCanvas.GetComponent<ExploreUICotroller> ().QuitPauseHUD ();
			}

//			GameManager.Instance.gameDataCenter.ReleaseDataWithDataTypes (new GameDataCenter.GameDataType[]{ 
//				GameDataCenter.GameDataType.GameSettings
//			});

		}

		/// <summary>
		/// Changes the settings and save.
		/// </summary>
		private void ChangeSettingsAndSave(){

//			GameManager.Instance.soundManager.effectAS.volume = GameManager.Instance.gameDataCenter.gameSettings.systemVolume;
//			GameManager.Instance.soundManager.bgmAS.volume = GameManager.Instance.gameDataCenter.gameSettings.systemVolume;
//
//			GameManager.Instance.soundManager.pronunciationAS.enabled = GameManager.Instance.gameDataCenter.gameSettings.isPronunciationEnable;

			GameManager.Instance.persistDataManager.SaveGameSettings ();

		}

		public void QuitAPP(){


			#warning 其他一些要保存的数据操作



		}


		public void Comment(){

		}

		public void DestroyInstances(){

			GameManager.Instance.UIManager.DestroryCanvasWith (CommonData.settingCanvasBundleName, "SettingCanvas", null, null);



		}

	}
}
