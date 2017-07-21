using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordViewController : MonoBehaviour {


	public RecordView recordView;

	private LearningInfo learnInfo;

	private int currentSelectTitleIndex = -1;

	public void SetUpRecordView(){

		learnInfo = GameManager.Instance.learnInfo;

		recordView.SetUpRecordView ();

		OnTitleButtonClick (0);

		currentSelectTitleIndex = 0;
	}

	public void OnTitleButtonClick(int index){

		if (currentSelectTitleIndex == index) {
			return;
		}

		currentSelectTitleIndex = index;

		recordView.OnSelectTitleButton (index);

		switch (index) {
		case 0:
			OnGeneralInfoButtonClick ();
			break;
		case 1:
			OnLearnedButtonClick ();
			break;
		case 2:
			OnUnlearnedButtonClick ();
			break;
		}

	}


	private void OnGeneralInfoButtonClick(){

		recordView.OnGeneralInfoButtonClick (learnInfo);
	}

	private void OnLearnedButtonClick(){

		recordView.OnQuitWordsRecordPlane ();

		recordView.OnLearnedButtonClick (learnInfo);
	}

	private void OnUnlearnedButtonClick(){

		recordView.OnQuitWordsRecordPlane ();
		
		recordView.OnUnlearnedButtonClick (learnInfo);
	}

	public void QuitRecordPlane(){

		recordView.OnQuitRecordPlane (DestroyInstances);

		GameObject homeCanvas = GameObject.Find (CommonData.instanceContainerName + "/HomeCanvas");

		if (homeCanvas != null) {
			homeCanvas.GetComponent<HomeViewController> ().SetUpHomeView ();
		}


	}

	private void DestroyInstances(){

		TransformManager.DestroyTransform (gameObject.transform);
		TransformManager.DestroyTransfromWithName ("WordItem", TransformRoot.InstanceContainer);
		TransformManager.DestroyTransfromWithName ("WordItemPool", TransformRoot.PoolContainer);



	}
}
