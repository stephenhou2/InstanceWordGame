using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InstancePool: MonoBehaviour{


	private List<GameObject> mInstancePool = new List<GameObject>();

	public static InstancePool GetOrCreateInstancePool(string poolName){

		Transform trans = TransformManager.FindTransform (CommonData.poolContainerName + "/" + poolName);

		InstancePool instancePool = trans.GetComponent<InstancePool> ();

		if (instancePool == null) {
			instancePool = trans.gameObject.AddComponent<InstancePool> ();
		}
		return instancePool;
	}

	public T GetInstance<T>(GameObject instanceModel,Transform instanceParent)
		where T:Component
	{
		GameObject mInstance = null;

		if (mInstancePool.Count != 0) {
			mInstance = mInstancePool [0];
			mInstancePool.RemoveAt (0);
			mInstance.transform.SetParent (instanceParent,false);
		} else {
			mInstance = Instantiate (instanceModel,instanceParent);	
			mInstance.name = instanceModel.name;
		}

		return mInstance.GetComponent<T>();
	}

	public void AddChildInstancesToPool(Transform originalParent){

		while (originalParent.childCount > 0) {
			
			GameObject instance = originalParent.GetChild (0).gameObject;

			instance.transform.SetParent(GetComponent<Transform>());

			mInstancePool.Add (instance);
		}

	}

	public void AddInstanceToPool(GameObject instance){

		instance.transform.SetParent (GetComponent<Transform>());
		mInstancePool.Add (instance);

	}


}
