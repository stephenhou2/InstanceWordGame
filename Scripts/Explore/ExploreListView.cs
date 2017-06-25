using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExploreListView : MonoBehaviour {

	public Transform explorePlane;

	public Transform chapter;

	private Transform mExplorePlane;

	public void SetUpExploreListView(ChapterList[] chapterLists,int unlockedMaxChapter){
		Transform canvas = GameObject.Find ("Canvas").transform;
		mExplorePlane = Instantiate (explorePlane,canvas);


		for (int i = 0; i < unlockedMaxChapter; i++) {
			Transform mChapter = Instantiate (chapter, mExplorePlane);

			mChapter.GetComponent<ChapterListView> ().chapterIndex = chapterLists [i].chapterIndex;

			Transform chapterTitle = mChapter.transform.FindChild ("ChapterTitle");
			Text description = mChapter.transform.FindChild ("Description").GetComponent<Text>();

			Text chapterName = chapterTitle.transform.FindChild ("ChapterName").GetComponent<Text>();
			Text requirement = chapterTitle.transform.FindChild ("Requirement").GetComponent<Text>();


			chapterName.text = "Chapter " + (chapterLists [i].chapterIndex + 1) + ":" + chapterLists [i].chapterName;
			requirement.text = "Level >= " + chapterLists [i].requiredLevel;
		}
	}

	public void KillExploreListView(){
		mExplorePlane.gameObject.SetActive (false);
		Destroy (mExplorePlane.gameObject);
	}


}
