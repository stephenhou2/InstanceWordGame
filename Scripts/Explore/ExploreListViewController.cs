using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploreListViewController : MonoBehaviour {





	private void DestroyInstances(){


		TransformManager.DestroyTransfromWithName ("ExploreListCanvas", TransformRoot.InstanceContainer);

		TransformManager.DestroyTransfromWithName ("ChapterButton", TransformRoot.InstanceContainer);

//		Destroy(GameObject.Find("InstancePoll/ExploreListCanvas").gameObject);
//
//		Destroy (GameObject.Find ("InstancePoll/ChapterButton").gameObject);
	}

}
