using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace WordJourney{
	public class SettingView : MonoBehaviour {

		public Slider volumeControl;

		public Image pronounceOnImage;

		public Image pronounceOffImage;


		public ToggleGroup tg;

		public Transform settingViewContainer;
		public Transform settingPlane;

		public Transform wordsPlane;
		public Toggle cet4;
		public Toggle cet6;
		public Toggle daily;
		public Toggle bussiness;

		public Transform alertHUD;

		public void SetUpSettingView(GameSettings settings){

			volumeControl.value = settings.systemVolume;

			pronounceOnImage.enabled = !settings.isPronunciationEnable;
			pronounceOffImage.enabled = settings.isPronunciationEnable;


			tg.SetAllTogglesOff ();

			switch (settings.wordType) {
			case WordType.CET4:
				cet4.isOn = true;
				break;
			case WordType.CET6:
				cet6.isOn = true;
				break;
			case WordType.Daily:
				daily.isOn = true;
				break;
			case WordType.Bussiness:
				bussiness.isOn = true;
				break;
			}

			GetComponent<Canvas> ().enabled = true;

		}

		public void UpdatePronounceControl(bool enable){

			pronounceOnImage.enabled = !enable;
			pronounceOffImage.enabled = enable;

		}



		public int GetCurrentWordType(int index){

			Transform trans = wordsPlane.GetChild (index);

			if (!trans.GetComponent<Toggle> ().isOn) {
				return -1;
			}

			return index;
		}

		public void ShowAlertHUD(){
			alertHUD.gameObject.SetActive (true);
		}

		public void QuitAlertHUD(){
			alertHUD.gameObject.SetActive (false);
		}


		public void QuitSettingView(){

			GetComponent<Canvas> ().enabled = false;

		}

	}
}
