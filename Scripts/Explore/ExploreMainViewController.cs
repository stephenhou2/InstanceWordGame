using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploreMainViewController:MonoBehaviour {

	private ExploreMainView expChapterView;

	private GameObject mDialogAndItemPlane;

	public GameObject dialogAndItemPlane{
		get{
			if (mDialogAndItemPlane == null) {
				ResourceManager.Instance.LoadAssetWithName ("explore/dialog_and_item_view", ()=>{
					mDialogAndItemPlane = ResourceManager.Instance.gos.Find(delegate(GameObject go) {
						return go.name == "DialogAndItemPlane";
					});
				}, true);
				mDialogAndItemPlane.transform.SetParent (GetComponent<Transform> (),false);
			}
			return mDialogAndItemPlane;
		}

	}

	public void SelectChapter(int selectedChapterIndex){

		ChapterDetailInfo[] chapterDetails = DataInitializer.LoadDataToModelWithPath <ChapterDetailInfo>(CommonData.JsonFileDirectoryPath,CommonData.chapterDataFileName);

		Debug.Log (chapterDetails[selectedChapterIndex]);
//		expChapterView.SetUpExploreMainView (chapterDetails[selectedChapterIndex]);

		expChapterView = GetComponent<ExploreMainView> ();

		expChapterView.SetUpExploreMainView (chapterDetails[selectedChapterIndex]);
	}


	public void OnSkillButtonSelected(){


	}

	public void OnBagButtonSelected(){

	}

	public void OnSettingButtonSelected(){

	}

//	public void OnEnterEvent(int eventType){
//
//	}

		
	public void OnEnterBattle(MonsterGroup monsterGroup){
		
		// 进入战斗场景前将探索场景隐藏
		GameObject battleCanvas = GameObject.Find ("BattleCanvas");
		GameObject exploreCanvas = GameObject.Find ("ExploreCanvas");
		if (battleCanvas != null) {
			exploreCanvas.GetComponent<Canvas> ().enabled = false;
			battleCanvas.GetComponent<Canvas> ().enabled = true;

		} else {
			ResourceManager.Instance.LoadAssetWithName ("battle/battle", () => {
				exploreCanvas.GetComponent<Canvas> ().enabled = false;
			},true);// 若当前场景中没有battleCanvas，从assetBundle中加载
		}

		//初始化战斗场景
		GameObject.Find ("BattleManager").GetComponent<BattleManager> ().OnEnterBattle (monsterGroup);
	}

	// 初始化与npc交谈界面
	public void OnEnterNPC(NPC npc){
		
		Debug.Log (dialogAndItemPlane);
		dialogAndItemPlane.gameObject.SetActive (true);

		dialogAndItemPlane.GetComponent<DialogAndItemView> ().SetUpDialogPlane(npc);

		Debug.Log (npc);
	}
	// 初始化物品展示界面
	public void OnEnterItem(Item item){
		
		dialogAndItemPlane.gameObject.SetActive (true);

		dialogAndItemPlane.GetComponent<DialogAndItemView> ().SetUpItemPlane(item);

		Debug.Log (item);
	}

	public void OnQuitExploreChapterView(){

	}



}
