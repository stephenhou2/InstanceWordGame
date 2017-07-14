using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeViewController : MonoBehaviour {

	public HomeView homeView;


	public void SetUpHomeView(){

		homeView.SetUpHomeView ();

	}

	public void OnExploreButtonClick(){

		ResourceManager.Instance.LoadAssetWithFileName ("explore/explore_list_canvas", () => {

			GameObject.Find("ExploreListCanvas").GetComponent<ExploreListViewController> ().SetUpExploreListView();

			DestroyInstances();
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

		});


	}

	public void OnThinkingButtonClick(){

		Debug.Log ("单词界面");

	}

	public void OnProduceButtonClick(){

		ResourceManager.Instance.LoadAssetWithFileName ("produce/canvas", () => {

			ResourceManager.Instance.gos[0].GetComponent<ProduceViewController> ().SetUpProduceView();

		});

	}



	public void OnSkillButtonClick(){

		ResourceManager.Instance.LoadAssetWithFileName ("skills/canvas", () => {

			ResourceManager.Instance.gos[0].GetComponent<SkillsViewController>().SetUpSkillsView();

		});

	}

	public void OnBagButtonClick(){

		ResourceManager.Instance.LoadAssetWithFileName ("bag/canvas", () => {

			ResourceManager.Instance.gos [0].GetComponent<BagViewController> ().SetUpBagView ();

		});
	}

	public void OnSettingButtonClick(){

		ResourceManager.Instance.LoadAssetWithFileName ("setting/canvas", () => {

			ResourceManager.Instance.gos [0].GetComponent<SettingViewController> ().SetUpSettingView ();

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
