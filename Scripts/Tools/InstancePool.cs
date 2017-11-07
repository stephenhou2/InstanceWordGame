using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	public class InstancePool: MonoBehaviour{


		private List<GameObject> mInstancePool = new List<GameObject>();

		public static InstancePool GetOrCreateInstancePool(string poolName,string parentName){

			Transform trans = TransformManager.FindOrCreateTransform (parentName + "/" + poolName);

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
				ResetInstance (mInstance);
			} else {
				mInstance = Instantiate (instanceModel,instanceParent);
				ResetInstance (mInstance);
				mInstance.name = instanceModel.name;
			}

			return mInstance.GetComponent<T>();
		}

		public T GetInstanceWithName<T>(string goName, GameObject instanceModel,Transform instanceParent)
			where T:Component
		{
			GameObject mInstance = null;


			if (mInstancePool.Count != 0) {

				mInstance = mInstancePool.Find (delegate(GameObject obj) {
					return obj.name == goName;
				});

				if (mInstance != null) {
					mInstancePool.Remove (mInstance);
				} else {
					mInstance = Instantiate (instanceModel,instanceParent);
					ResetInstance (mInstance);
					mInstance.name = instanceModel.name;
				}

				mInstance.transform.SetParent (instanceParent,false);

				ResetInstance (mInstance);
			} else {
				mInstance = Instantiate (instanceModel,instanceParent);
				ResetInstance (mInstance);
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

		public void ClearInstancePool(){
			for(int i = 0;i<transform.childCount;i++){
				GameObject instance = transform.GetChild (i).gameObject;
				mInstancePool.Remove (instance);
				Destroy (instance);
			}
		}

		public void AddInstanceToPool(GameObject instance){

			instance.transform.SetParent (GetComponent<Transform>());
			mInstancePool.Add (instance);

		}

		private void ResetInstance(GameObject instance){

			instance.transform.localRotation = Quaternion.identity;
			instance.transform.localScale = Vector3.one;

		}

	}
}
