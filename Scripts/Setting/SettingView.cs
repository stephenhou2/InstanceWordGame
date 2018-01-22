using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace WordJourney{
	public class SettingView : MonoBehaviour {

		public Slider volumeControl;


		public ToggleGroup tg;

		public Transform settingPlane;

		public Toggle cet4;
		public Toggle cet6;
		public Toggle daily;
		public Toggle bussiness;

		public Transform queryChangeWordHUD;

		public Image pronounceOnImage;
		public Image pronounceOffImage;

		public void SetUpSettingView(GameSettings settings){

			volumeControl.value = (int)(settings.systemVolume * 100);

			UpdatePronounceControl (settings.isAutoPronounce);

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

			if (!enable) {
				pronounceOnImage.gameObject.SetActive (false);
				pronounceOffImage.gameObject.SetActive (true);
			} else {
				pronounceOnImage.gameObject.SetActive (true);
				pronounceOffImage.gameObject.SetActive (false);
			}

		}



		public void ShowAlertHUD(){
			queryChangeWordHUD.gameObject.SetActive (true);
		}

		public void QuitAlertHUD(){
			queryChangeWordHUD.gameObject.SetActive (false);
		}


		public void QuitSettingView(){

			GetComponent<Canvas> ().enabled = false;

		}

	}
}
