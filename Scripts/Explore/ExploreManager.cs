using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploreManager : MonoBehaviour {

	private ExploreMainViewController expMainViewController;

	private ChapterList[] chapterLists;

	public static int unlockedMaxChapterIndex = 1;

	public void Awake(){
		/********for test**********/
		ExploreManager.unlockedMaxChapterIndex = 2;
		/********for test**********/
		chapterLists = DataInitializer.LoadDataToModelWithPath<ChapterList> (CommonData.JsonFileDirectoryPath, CommonData.chaptersDataFileName);
//		expListView.SetUpExploreListView (chapterLists,ExploreManager.unlockedMaxChapterIndex);

		expMainViewController = GetComponent<ExploreMainViewController> ();

		expMainViewController.SelectChapter(0);
	}

	public void OnSelectChapter(int chapterIndex){
		
//		expListView.KillExploreListView ();

		ChapterList cl = chapterLists [chapterIndex];

		expMainViewController.SelectChapter (chapterIndex);
		Debug.Log ("Enter chapter" + chapterIndex);
	}

	public void OnQuitExploreView(){

	}

}
