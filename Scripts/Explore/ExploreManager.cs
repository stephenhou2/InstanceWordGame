using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploreManager : MonoBehaviour {

//	private ExploreMainViewController expMainViewController;
//
//	private ChapterList[] chapterLists;

//	public void Awake(){
//		
//
//		chapterLists = DataInitializer.LoadDataToModelWithPath<ChapterList> (CommonData.jsonFileDirectoryPath, CommonData.chaptersDataFileName);
////		expListView.SetUpExploreListView (chapterLists,ExploreManager.unlockedMaxChapterIndex);
//
//		ResourceManager.Instance.LoadAssetWithFileName ("explore/explore_list_canvas",null,true);
//
//		expMainViewController = GameObject.Find("ExploreCanvas").GetComponent<ExploreMainViewController> ();
//
//		expMainViewController.SelectChapter(0);
//	}
//
//	public void OnSelectChapter(int chapterIndex){
//
//		ChapterList cl = chapterLists [chapterIndex];
//
//		expMainViewController.SelectChapter (chapterIndex);
//
//		Debug.Log ("Enter chapter" + chapterIndex);
//	}

	public void OnQuitExploreView(){

	}

}
