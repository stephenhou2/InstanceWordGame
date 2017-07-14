using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploreListViewController : MonoBehaviour {


	public ExploreListView exploreListView;

	private ChapterList[] chapterLists;

	public void SetUpExploreListView(){

		GameObject chapterButton = GameObject.Find ("ChapterButton");

		chapterButton.transform.SetParent (TransformManager.FindTransform (CommonData.instanceContainerName));

		chapterLists = DataInitializer.LoadDataToModelWithPath<ChapterList> (CommonData.jsonFileDirectoryPath, CommonData.chaptersDataFileName);

		int unlockedMaxChapterIndex = GameManager.Instance.unlockedMaxChapterIndex;

		exploreListView.SetUpExploreListView (chapterLists, unlockedMaxChapterIndex,chapterButton);

	}


	public void OnChapterListButtonClick(int index){

		ResourceManager.Instance.LoadAssetWithFileName ("explore/canvas", () => {

			ChapterDetailInfo chapterDetail = DataInitializer.LoadDataToModelWithPath <ChapterDetailInfo>(CommonData.jsonFileDirectoryPath,CommonData.chapterDataFileName)[index];

			ExploreMainViewController exploreMainViewCtr = GameObject.Find("ExploreMainCanvas").GetComponent<ExploreMainViewController>();

			exploreMainViewCtr.SetUpExploreMainView(chapterDetail);

			DestroyInstances();
		});

	}


	private void DestroyInstances(){


		TransformManager.DestroyTransfromWithName ("ExploreListCanvas", TransformRoot.InstanceContainer);

		TransformManager.DestroyTransfromWithName ("ChapterButton", TransformRoot.InstanceContainer);

//		Destroy(GameObject.Find("InstancePoll/ExploreListCanvas").gameObject);
//
//		Destroy (GameObject.Find ("InstancePoll/ChapterButton").gameObject);
	}

}
