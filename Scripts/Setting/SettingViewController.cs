using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class SettingViewController : MonoBehaviour{

		public SettingView settingView;

		private bool isPointerUp;

		private bool settingChanged;


		public void SetUpSettingView(){

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

		public void SetDownloadEnable(bool enable){

			GameManager.Instance.gameDataCenter.gameSettings.isDownloadEnable = enable;

			settingView.UpdateDownloadControl (enable);

			settingChanged = true;


		}

		public void ChangeWordType(int index){

			int wordTypeIndex = settingView.GetCurrentWordType (index);

			if (wordTypeIndex == -1) {
				return;
			}

			switch (wordTypeIndex) {
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

			settingChanged = true;

		}

		public void QuitSettingPlane(){

			if (settingChanged) {
				ChangeSettingsAndSave ();
			}

			settingChanged = false;

			settingView.QuitSettingView ();

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.homeCanvasBundleName, "HomeCanvas", () => {
				TransformManager.FindTransform("HomeCanvas").GetComponent<HomeViewController>().SetUpHomeView();
			});

			GameManager.Instance.gameDataCenter.ReleaseDataWithDataTypes (new GameDataCenter.GameDataType[]{ 
				GameDataCenter.GameDataType.GameSettings
			});

		}

		/// <summary>
		/// Changes the settings and save.
		/// </summary>
		private void ChangeSettingsAndSave(){

			GameManager.Instance.soundManager.effectAS.volume = GameManager.Instance.gameDataCenter.gameSettings.systemVolume;
			GameManager.Instance.soundManager.bgmAS.volume = GameManager.Instance.gameDataCenter.gameSettings.systemVolume;

			GameManager.Instance.soundManager.pronunciationAS.enabled = GameManager.Instance.gameDataCenter.gameSettings.isPronunciationEnable;

			GameManager.Instance.persistDataManager.SaveGameSettings ();

		}

		public void QuitAPP(){


			#warning 其他一些要保存的数据操作



		}

		public void QuitSettingView(){



		}

		public void Comment(){

		}

		public void DestroyInstances(){

			GameManager.Instance.UIManager.DestroryCanvasWith (CommonData.settingCanvasBundleName, "SettingCanvas", null, null);

		}

	}
}
