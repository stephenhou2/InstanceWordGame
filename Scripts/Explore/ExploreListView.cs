using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExploreListView : MonoBehaviour {

	public Transform exploreLists;

	public void SetUpExploreListView(ChapterList[] chapterLists,int unlockedMaxChapter,GameObject chapterButton){

		for (int i = 0; i < unlockedMaxChapter; i++) {

			GameObject newChapterButton = Instantiate (chapterButton, TransformManager.FindTransform (CommonData.poolContainerName));

			newChapterButton.name = "ChapterButton";

			ChapterList chapterList = chapterLists [i];

			#warning章节列表目前只显示章节名称，没有加入地点，进入条件等，后续细化
			newChapterButton.GetComponentInChildren<Text> ().text = chapterList.chapterName;

			int chapterIndex = i;

			newChapterButton.GetComponent<Button> ().onClick.AddListener (delegate {
				GetComponent<ExploreListViewController>().OnChapterListButtonClick(chapterIndex);
			});

			newChapterButton.transform.SetParent (exploreLists);
		}
	}




//	public void KillExploreListView(){
//		mExplorePlane.gameObject.SetActive (false);
//		Destroy (mExplorePlane.gameObject);
//	}


}
