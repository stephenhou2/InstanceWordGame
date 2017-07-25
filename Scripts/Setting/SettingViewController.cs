using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingViewController : MonoBehaviour{

	public SettingView settingView;

	private bool isPointerUp;


	public void SetUpSettingView(){

		GameSettings settings = GameManager.Instance.gameSettings;

		settingView.SetUpSettingView (settings);

	}
		

	public void ChangeVolume(){

		GameManager.Instance.gameSettings.systemVolume = (int)settingView.volumeControl.value;

	}

	public void SetPronunciationEnable(bool enable){

		GameManager.Instance.gameSettings.isPronunciationEnable = enable;

		settingView.UpdatePronounceControl (enable);

	}

	public void SetDownloadEnable(bool enable){

		GameManager.Instance.gameSettings.isDownloadEnable = enable;

		settingView.UpdateDownloadControl (enable);

	}

	public void ChangeWordType(int index){


		int wordTypeIndex = settingView.GetCurrentWordType (index);

		if (wordTypeIndex == -1) {
			return;
		}

		switch (wordTypeIndex) {
		case 0:
			GameManager.Instance.gameSettings.wordType = WordType.CET4;
			break;
		case 1:
			GameManager.Instance.gameSettings.wordType = WordType.CET6;
			break;
		case 2:
			GameManager.Instance.gameSettings.wordType = WordType.Daily;
			break;
		case 3:
			GameManager.Instance.gameSettings.wordType = WordType.Bussiness;
			break;

		}

		GameManager.Instance.OnSettingsChanged ();

		Debug.Log (GameManager.Instance.gameSettings.wordType);
	}

	public void QuitSettingPlane(){

		GameManager.Instance.OnSettingsChanged ();

		settingView.QuitSettingView (DestroyInstances);

		GameObject homeCanvas = GameObject.Find (CommonData.instanceContainerName + "/HomeCanvas");

		if (homeCanvas != null) {
			homeCanvas.GetComponent<HomeViewController> ().SetUpHomeView ();
		}

	}

	public void QuitAPP(){


		#warning 其他一些要保存的数据操作

		GameManager.Instance.SaveGameSettings ();

	}

	public void Comment(){

	}

	private void DestroyInstances(){

		TransformManager.DestroyTransform (gameObject.transform);

	}

}
