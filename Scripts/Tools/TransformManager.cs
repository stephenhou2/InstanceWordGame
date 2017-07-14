using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformManager:MonoBehaviour {

	// 按照给定的transform名称（带层级）查找，如果没有找到指定transform，则按照参数中的层级关系创建该transform
	public static Transform FindTransform(string transformName){
		
		string[] strs = transformName.Split(new char[] {'/'});
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
		

	public static Transform NewTransform(string transformName,Transform parentTrans = null){

//		if (commonContainer == null) {
//			commonContainer = (new GameObject ()).transform;
//			commonContainer.name = "ContainerModel";
//		}

		Transform mContainer = (new GameObject ()).transform;
		if (parentTrans != null) {
			mContainer.SetParent (parentTrans);
		}
		mContainer.name = transformName;
		return mContainer;
	}

	public static void DestroyTransform(Transform trans){

		try{
			Destroy(trans.gameObject);
		}catch(System.Exception e){
			Debug.Log ("删除游戏物体失败" + e.ToString ());
		}

	}




	public static void DestroyTransfromWithName(string transformName,TransformRoot transRoot){

		string transInHierarchy = string.Empty;

		switch (transRoot) {
		case TransformRoot.InstanceContainer:
			transInHierarchy = CommonData.instanceContainerName + "/" + transformName;
			break;
		case TransformRoot.PoolContainer:
			transInHierarchy = CommonData.poolContainerName + "/" + transformName;
			break;
		case TransformRoot.Plain:
			transInHierarchy = transformName;
			break;
		}


		Transform trans = FindTransform (transInHierarchy);

		if (trans == null) {
			Debug.Log ("游戏物体不存在，无法删除");
		} else {

			try{
				Destroy(trans.gameObject);
			}catch(System.Exception e){
				Debug.Log ("删除游戏物体失败" + e.ToString ());
			}


		}


	}

}
