using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploreMainViewController:MonoBehaviour {

	public ExploreMainView exploreMainView;

	private Transform mDialogAndItemPlane;

	private Transform currentSelectedEventView;

	private int stepsLeft;

	private ChapterDetailInfo detailInfo;

	public GameObject dialogAndItemPlane;


	private ChapterList[] chapterLists;

	private ChapterDetailInfo[] chapterDetails;


	public void SetUpExploreListPlane(){


		chapterLists = DataInitializer.LoadDataToModelWithPath<ChapterList> (CommonData.jsonFileDirectoryPath, CommonData.chaptersDataFileName);

		chapterDetails = DataInitializer.LoadDataToModelWithPath <ChapterDetailInfo>(CommonData.jsonFileDirectoryPath,CommonData.chapterDataFileName);

		int unlockedMaxChapterIndex = GameManager.Instance.unlockedMaxChapterIndex;

		exploreMainView.SetUpChapterListPlane (chapterLists,chapterDetails, unlockedMaxChapterIndex);

		GetComponent<Canvas>().enabled = true; 

	}


	public void OnChapterListClick(int index){


		ChapterDetailInfo chapterDetail = chapterDetails [index];

		SetUpExploreMainViewContainerPlane(chapterDetail);


	}


	public void SetUpExploreMainViewContainerPlane(ChapterDetailInfo chapterDetail){

		detailInfo = chapterDetail;

		stepsLeft = chapterDetail.stepsLeft;

		exploreMainView.SetUpExploreMainViewPlane(detailInfo,GameManager.Instance.allExploreIcons);
	}

	// 进入战斗场景
	public void OnEnterBattle(MonsterGroup monsterGroup,Transform chapterEventView){

		currentSelectedEventView = chapterEventView;

		// 进入战斗场景前将探索场景隐藏
		GameObject battleCanvas = GameObject.Find (CommonData.battleCanvas);
		GameObject exploreCanvas = GameObject.Find (CommonData.exploreMainCanvas);
		GameObject homeCanvas = GameObject.Find (CommonData.instanceContainerName + "/HomeCanvas");



		if (battleCanvas != null) {
			exploreCanvas.GetComponent<Canvas> ().enabled = false;
			homeCanvas.GetComponent<Canvas> ().enabled = false;
			battleCanvas.GetComponent<Canvas> ().enabled = true;


		} else {
			ResourceManager.Instance.LoadAssetWithFileName ("battle/canvas", () => {
				exploreCanvas.GetComponent<Canvas> ().enabled = false;
				homeCanvas.GetComponent<Canvas> ().enabled = false;
			},true);// 若当前场景中没有battleCanvas，从assetBundle中加载
		}

		//初始化战斗场景
		GameObject.Find (CommonData.battleCanvas).GetComponent<BattleViewController> ().SetUpBattleView (monsterGroup);
	}


	// 初始化与npc交谈界面
	public void OnEnterNPC(NPC npc,Transform chapterEventView,Sprite npcSprite){
		
		currentSelectedEventView = chapterEventView;

		dialogAndItemPlane.GetComponent<DialogAndItemViewController> ().SetUpDialogAndItemView (npc,npcSprite,null,null);

		Debug.Log (npc);
	}

	// 初始化物品展示界面
	public void OnEnterItem(Item item,Transform chapterEventView,Sprite itemSprite){
		
		currentSelectedEventView = chapterEventView;

		dialogAndItemPlane.GetComponent<DialogAndItemViewController> ().SetUpDialogAndItemView (null, null, item, itemSprite);

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

		exploreMainView.OnQuitExploreChapterView (DestroyInstances);

		GameObject homeCanvas = GameObject.Find (CommonData.instanceContainerName + "/HomeCanvas");

		if (homeCanvas != null) {
			homeCanvas.GetComponent<HomeViewController> ().SetUpHomeView ();
		}

	}

	private void DestroyInstances(){
		
		TransformManager.DestroyTransform (gameObject.transform);
		TransformManager.DestroyTransfromWithName ("ChapterItemModel", TransformRoot.InstanceContainer);
		TransformManager.DestroyTransfromWithName ("ChapterEventModel", TransformRoot.InstanceContainer);
		TransformManager.DestroyTransfromWithName ("ChapterEventsPool", TransformRoot.PoolContainer);
		TransformManager.DestroyTransfromWithName ("ChoiceButtonsPool", TransformRoot.PoolContainer);
		TransformManager.DestroyTransfromWithName ("ChoiceButtonModel", TransformRoot.InstanceContainer);

	}
		
}
