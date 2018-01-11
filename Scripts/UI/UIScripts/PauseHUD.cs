using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class PauseHUD : MonoBehaviour {

		private enum QueryType{
			Refresh,
			BackHome
		}


		public Transform queryContainer;

		private QueryType queryType;


		private bool quitWhenClickBackground = true;
		private CallBack quitCallBack;
		private CallBack refreshCallBack;
		private CallBack backHomeCallBack;
		private CallBack settingsCallBack;


		public void InitPauseHUD(bool quitWhenClickBackground,CallBack refreshCallBack,CallBack backHomeCallBack,CallBack quitCallBack,CallBack settingsCallBack){
			this.quitWhenClickBackground = quitWhenClickBackground;
			this.refreshCallBack = refreshCallBack;
			this.backHomeCallBack = backHomeCallBack;
			this.quitCallBack = quitCallBack;
			this.settingsCallBack = settingsCallBack;
		}


		public void SetUpPauseHUD(){

			Time.timeScale = 0f;

			gameObject.SetActive (true);

		}

		public void OnBackgroundClick(){
			if (quitWhenClickBackground) {
				QuitPauseHUD ();
			}
		}


		public void OnRefreshButtonClick(){

			queryType = QueryType.Refresh;

			if (refreshCallBack != null) {
				refreshCallBack ();
			}

			queryContainer.gameObject.SetActive (true);

//			QuitPauseHUD ();

		}


		public void OnBackHomeButtonClick(){

			queryType = QueryType.BackHome;

			if (backHomeCallBack != null) {
				backHomeCallBack ();
			}

			queryContainer.gameObject.SetActive (true);

//			QuitPauseHUD ();

		}

		public void OnSettingsButtonClick(){

			if (settingsCallBack != null) {
				settingsCallBack ();
			}

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.settingCanvasBundleName, "SettingCanvas", () => {
				TransformManager.FindTransform("SettingCanvas").GetComponent<SettingViewController>().SetUpSettingView();
			},false,true);

		}

		public void OnConfirmButtonClick(){
			
			queryContainer.gameObject.SetActive (false);

			QuitPauseHUD ();

			ExploreManager exploreManager = TransformManager.FindTransform ("ExploreManager").GetComponent<ExploreManager> ();

			switch (queryType) {
			case QueryType.Refresh:
				exploreManager.RefrestCurrentLevel ();
				break;
			case QueryType.BackHome:
				exploreManager.QuitExploreScene (false);
				break;
			}
		}

		public void OnCancelButtonClick(){

			queryContainer.gameObject.SetActive (false);

			QuitPauseHUD ();

		}


		public void QuitPauseHUD(){

			if (quitCallBack != null) {
				quitCallBack ();
			}

			Time.timeScale = 1f;

			gameObject.SetActive (false);
		}

	}
}
