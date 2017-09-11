﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class HomeViewController : MonoBehaviour {

		public HomeView homeView;



		public void SetUpHomeView(){

			homeView.SetUpHomeView ();


		}

		public void OnExploreButtonClick(){

			int currentExploreLevel = GameManager.Instance.unlockedMaxChapterIndex;

			homeView.ShowMaskImage ();

			ResourceManager.Instance.LoadAssetWithFileName ("explore/scene", () => {

//				GetComponent<Canvas> ().enabled = false;

				GameObject.Find("ExploreManager").GetComponent<ExploreManager> ().SetupExploreView(currentExploreLevel);

				homeView.OnQuitHomeView();
	//			DestroyInstances();
				// 探索场景加载完成后后台加载战斗场景
	//			ResourceManager.Instance.LoadAssetWithFileName ("battle/canvas",()=>{
	//
	//				ResourceManager.Instance.LoadAssetWithFileName("dialog"
	//
	//			});
			});

		}

		public void OnRecordButtonClick(){

			ResourceManager.Instance.LoadAssetWithFileName ("record/canvas", () => {

				ResourceManager.Instance.gos[0].GetComponent<RecordViewController> ().SetUpRecordView();

				homeView.OnQuitHomeView();
			});


		}

		public void OnThinkingButtonClick(){

			Debug.Log ("单词界面");

		}

		public void OnProduceButtonClick(){

			ResourceManager.Instance.LoadAssetWithFileName ("produce/canvas", () => {

				GameObject.Find(CommonData.instanceContainerName + "/ProduceCanvas").GetComponent<ProduceViewController> ().SetUpProduceView();

				homeView.OnQuitHomeView();
			});

		}



		public void OnSkillButtonClick(){

			ResourceManager.Instance.LoadAssetWithFileName ("skills/canvas", () => {

				ResourceManager.Instance.gos[0].GetComponent<SkillsViewController>().SetUpSkillsView();

				homeView.OnQuitHomeView();
			});

		}

		public void OnBagButtonClick(){

			GameObject bagCanvas = GameObject.Find ("BagCanvas");

			if (bagCanvas != null) {
				
				bagCanvas.GetComponent<BagViewController> ().SetUpBagView ();

				homeView.OnQuitHomeView();
			}

			ResourceManager.Instance.LoadAssetWithFileName ("bag/canvas", () => {

				ResourceManager.Instance.gos [0].GetComponent<BagViewController> ().SetUpBagView ();

				homeView.OnQuitHomeView();

			});
		}

		public void OnSettingButtonClick(){

			ResourceManager.Instance.LoadAssetWithFileName ("setting/canvas", () => {

				ResourceManager.Instance.gos [0].GetComponent<SettingViewController> ().SetUpSettingView ();

				homeView.OnQuitHomeView();
			});
		}

		private void DestroyInstances(){

			TransformManager.DestroyTransfromWithName ("HomeCanvas", TransformRoot.InstanceContainer);

		}

	//	private void HideHomeView(){
	//
	//		GetComponent<Canvas> ().enabled = false;
	//
	//	}

	}
}
