using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploreMainViewController:MonoBehaviour {

	private ExploreMainView expChapterView;

//	public void Awake(){
//		SelectChapter (1);
//	}


	public void SelectChapter(int selectedChapterIndex){

//		ChapterDetailInfo[] chapterDetails = DataInitializer.LoadDataToModelWithPath <ChapterDetailInfo>("", "");

//		expChapterView.SetUpExploreMainView (chapterDetails[selectedChapterIndex]);

		expChapterView = GetComponent<ExploreMainView> ();

		expChapterView.SetUpExploreMainView (null);
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
