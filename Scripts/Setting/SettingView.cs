﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace WordJourney{
	public class SettingView : MonoBehaviour {

		public Slider volumeControl;

		public Image pronounceOnImage;

		public Image pronounceOffImage;

		public Image downloadOnImage;

		public Image downloadOffIamge;

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

			downloadOnImage.enabled = !settings.isDownloadEnable;
			downloadOffIamge.enabled = settings.isDownloadEnable;

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

		public void UpdateDownloadControl(bool enable){

			downloadOnImage.enabled = !enable;
			downloadOffIamge.enabled = enable;

		}

		public int GetCurrentWordType(int index){

			Transform trans = wordsPlane.GetChild (index);

			if (!trans.GetComponent<Toggle> ().isOn) {
				return -1;
			}

			return index;
		}

		public void QuitSettingView(){

			float offsetY = GetComponent<CanvasScaler> ().referenceResolution.y;

			Vector3 originalPosition = settingPlane.localPosition;

		}

	}
}
