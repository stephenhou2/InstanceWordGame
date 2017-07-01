using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerManager:MonoBehaviour {

	private static Transform commonContainer;


	public static Transform FindContainer(string containerName){

		GameObject container = GameObject.Find (containerName);

		if (container == null) {
			container = new GameObject();
			container.name = containerName;
		}

		return container.transform;

	}
		

	public static Transform NewContainer(string containerName,Transform parentTrans = null){

		if (commonContainer == null) {
			commonContainer = (new GameObject ()).transform;
			commonContainer.name = "ContainerModel";
		}

		Transform mContainer = Instantiate (commonContainer);
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
