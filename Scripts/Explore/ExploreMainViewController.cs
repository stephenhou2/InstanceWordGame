using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploreMainViewController:MonoBehaviour {

	public ExploreMainView exploreMainView;

	private GameObject mDialogAndItemPlane;

	private GameObject currentSelectedEventView;

	private int stepsLeft;

	private ChapterDetailInfo detailInfo;



	public GameObject dialogAndItemPlane{
		get{
			if (mDialogAndItemPlane == null) {
				ResourceManager.Instance.LoadAssetWithFileName ("explore/dialog_and_item_view", ()=>{
					mDialogAndItemPlane = ResourceManager.Instance.gos.Find(delegate(GameObject go) {
						return go.name == "DialogAndItemPlane";
					});
				}, true);
				mDialogAndItemPlane.transform.SetParent (GetComponent<Transform> (),false);
			}
			return mDialogAndItemPlane;
		}

	}


	public void SetUpExploreMainView(ChapterDetailInfo chapterDetail){

		detailInfo = chapterDetail;

		stepsLeft = chapterDetail.stepsLeft;

		Debug.Log (stepsLeft);

		ResourceManager.Instance.LoadAssetWithFileName ("explore/icons",()=>{
			exploreMainView.SetUpExploreMainView(detailInfo);
		});

	}

	// 进入战斗场景
	public void OnEnterBattle(MonsterGroup monsterGroup,GameObject chapterEventView){

		currentSelectedEventView = chapterEventView;

		// 进入战斗场景前将探索场景隐藏
		GameObject battleCanvas = GameObject.Find (CommonData.battleCanvas);
		GameObject exploreCanvas = GameObject.Find (CommonData.exploreMainCanvas);

		if (battleCanvas != null) {
			exploreCanvas.GetComponent<Canvas> ().enabled = false;
			battleCanvas.GetComponent<Canvas> ().enabled = true;

		} else {
			ResourceManager.Instance.LoadAssetWithFileName ("battle/canvas", () => {
				exploreCanvas.GetComponent<Canvas> ().enabled = false;
			},true);// 若当前场景中没有battleCanvas，从assetBundle中加载
		}

		//初始化战斗场景
		GameObject.Find (CommonData.battleCanvas).GetComponent<BattleViewController> ().SetUpBattleView (monsterGroup);
	}


	// 初始化与npc交谈界面
	public void OnEnterNPC(NPC npc,GameObject chapterEventView){
		
		currentSelectedEventView = chapterEventView;

		dialogAndItemPlane.gameObject.SetActive (true);

		dialogAndItemPlane.GetComponent<DialogAndItemView> ().SetUpDialogPlane(npc);

		Debug.Log (npc);
	}

	// 初始化物品展示界面
	public void OnEnterItem(Item item,GameObject chapterEventView,Sprite itemSprite){
		
		currentSelectedEventView = chapterEventView;

		dialogAndItemPlane.gameObject.SetActive (true);

		dialogAndItemPlane.GetComponent<DialogAndItemView> ().SetUpItemPlane(item,itemSprite);

		Debug.Log (item);
	}

	public void OnNextEvent(){

		stepsLeft--;

		Debug.Log (stepsLeft);

		if (stepsLeft <= 0) {
			Debug.Log ("本章节结束");
			return;
		}

		exploreMainView.SetUpNextStepChapterEventPlane (currentSelectedEventView, stepsLeft);
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



	public void OnQuitExploreChapterView(){

		Destroy(GameObject.Find (CommonData.exploreMainCanvas).gameObject);

	}



}
