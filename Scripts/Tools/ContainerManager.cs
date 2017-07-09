using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerManager:MonoBehaviour {


	public static Transform FindContainer(string containerName){
		
		string[] strs = containerName.Split(new char[] {'/'});
		List<Transform> transList = new List<Transform> ();


		for (int i = 0; i < strs.Length; i++) {
			string hierarchy = null;
			for (int j = 0; j < i + 1; j++) {
				hierarchy += "/" + strs [j];
			}
			string mHierarchy = hierarchy.Substring (1);

			GameObject go = GameObject.Find (mHierarchy);

			if (go == null) {
				go = new GameObject ();
				go.name = strs [i];
			}
			transList.Add (go.transform);

			if (i != 0) {
				go.transform.SetParent (transList [i - 1]);
			}

		}

		return transList[transList.Count - 1];

	}
		

	public static Transform NewContainer(string containerName,Transform parentTrans = null){

//		if (commonContainer == null) {
//			commonContainer = (new GameObject ()).transform;
//			commonContainer.name = "ContainerModel";
//		}

		Transform mContainer = (new GameObject ()).transform;
		if (parentTrans != null) {
			mContainer.SetParent (parentTrans);
		}
		mContainer.name = containerName;
		return mContainer;
	}

	public static void DestroyContainer(Transform container){
		if (container.GetComponentInParent<Transform> () == null) {
			Destroy (container);
		} else {
			Debug.Log ("not top level gameObject, can not destroy" + container);
		}
	}

}
