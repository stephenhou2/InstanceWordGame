using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploreMainViewController:MonoBehaviour {

	private ExploreMainView expChapterView;

//	public void Awake(){
//		SelectChapter (1);
//	}


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
		
	public void OnEnterBattle(){

	}

	public void OnEnterDialog(){

	}



	public void OnQuitExploreChapterView(){

	}
}
