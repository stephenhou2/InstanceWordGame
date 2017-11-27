using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class UIManager{


		public Dictionary<string,Transform> UIDic = new Dictionary<string, Transform> ();


		public void SetUpCanvasWith(string bundleName,string canvasName,CallBack cb,bool isSync = false,bool othersVisible = false){

			IDictionaryEnumerator dicEnumerator = UIDic.GetEnumerator ();

			if (!UIDic.ContainsKey (canvasName)) {

				ResourceLoader loader = ResourceLoader.CreateNewResourceLoader ();

				ResourceManager.Instance.LoadAssetsWithBundlePath (loader, bundleName, () => {

					if(!othersVisible){
						while (dicEnumerator.MoveNext ()) {
							Canvas canvas = (dicEnumerator.Value as Transform).GetComponent<Canvas> ();
							canvas.enabled = false;
//							canvas.targetDisplay = 1;
//							canvas.sortingOrder = -1;
						}
					}

					Canvas c = loader.gos.Find (delegate(GameObject obj) {
						return obj.name == canvasName;
					}).GetComponent<Canvas> ();


					c.enabled = true;

//					c.targetDisplay = 0;
//
//					c.sortingOrder = 0;

					if (cb != null) {
						cb ();
					}

					c.transform.SetAsLastSibling ();

					UIDic.Add (canvasName, c.transform);

				}, isSync);

			} else {
				while (dicEnumerator.MoveNext ()) {

					Canvas c = (dicEnumerator.Value as Transform).GetComponent<Canvas> ();

					if (dicEnumerator.Key as string == canvasName) {
//						c.targetDisplay = 0;
//						c.sortingOrder = 0;
						c.enabled = true;
						c.transform.SetAsLastSibling ();
					} else if(!othersVisible){
//						c.targetDisplay = 1;
//						c.sortingOrder = -1;
						c.enabled = false;
					}
				}

				if (cb != null) {
					cb ();
				}
			}

//			Transform canvas = UIDic.ContainsKey (canvasName) ? UIDic [canvasName] : null;
//
//			if (canvas != null) {
//
//				canvas.gameObject.SetActive (true);
//
////				canvas.GetComponent<Canvas> ().targetDisplay = 0;
////
////				canvas.GetComponent<Canvas> ().sortingOrder = 0;
//
//				if(cb != null){
//					cb ();
//				}
//
//				canvas.SetAsLastSibling ();
//
//			} else {
//				
//				ResourceLoader loader = ResourceLoader.CreateNewResourceLoader ();
//
//				ResourceManager.Instance.LoadAssetsWithBundlePath (loader, bundleName, () => {
//
//					canvas = loader.gos.Find(delegate(GameObject obj){
//						return obj.name == canvasName;
//					}).transform;
//
//
//					canvas.gameObject.SetActive (true);
//
////					canvas.GetComponent<Canvas> ().targetDisplay = 0;
////
////					canvas.GetComponent<Canvas> ().sortingOrder = 0;
//
//					if(cb != null){
//						cb ();
//					}
//
//					canvas.SetAsLastSibling ();
//
//					UIDic.Add(canvasName,canvas);
//
//				},isSync);
//			}
			

			Resources.UnloadUnusedAssets ();
			System.GC.Collect ();
		}

		public void HideCanvas(string canvasName){

			if(UIDic.ContainsKey(canvasName)){
				UIDic [canvasName].GetComponent<Canvas> ().enabled = false;
			}

		}

		public void DestroryCanvasWith(string bundleName,string canvasName,string poolContainerName,string modelContainerName){

			Transform canvas = UIDic [canvasName];

			GameObject.Destroy (canvas.gameObject);

			if (poolContainerName != null) {
				TransformManager.DestroyTransfromWithName (modelContainerName, TransformRoot.InstanceContainer);
			}

			if (modelContainerName != null) {
				TransformManager.DestroyTransfromWithName (poolContainerName, TransformRoot.PoolContainer);
			}

			ResourceManager.Instance.UnloadCaches (bundleName, true);

			UIDic.Remove (canvasName);
		}

		public void UnloadAllCanvasInSceneExcept(string[] exclusiveCanvasNames){

			string[] keys = new string[UIDic.Count];
			IDictionaryEnumerator enumerator = UIDic.GetEnumerator ();
			int i = 0;
			while (enumerator.MoveNext ()) {
				keys [i] = enumerator.Key as string;
				i++;
			}
			for(int j = 0;j<keys.Length;j++){
				
				string key = keys [j];

				bool keyExclusive = false;

				for (int k = 0; k < exclusiveCanvasNames.Length; k++) {
					if (key.Equals (exclusiveCanvasNames [k])) {
						keyExclusive = true;
					}
				}

				if (keyExclusive) {
					continue;
				}

				switch (key) {
				case "HomeCanvas":
					UIDic [key].GetComponent<HomeViewController> ().DestroyInstances ();
					break;
				case "BagCanvas":
					UIDic [key].GetComponent<BagViewController> ().DestroyInstances ();
					break;
				case "WorkbenchCanvas":
					UIDic [key].GetComponent<WorkBenchViewController> ().DestroyInstances ();
					break;
				case "MaterialDisplayCanvas":
					UIDic [key].GetComponent<MaterialDisplayViewController> ().DestroyInstances ();
					break;
				case "ProduceCanvas":
					UIDic [key].GetComponent<ProduceViewController> ().DestroyInstances ();
					break;
				case "RecordCanvas":
					UIDic [key].GetComponent<RecordViewController> ().DestroyInstances ();
					break;
				case "SettingCanvas":
					UIDic [key].GetComponent<SettingViewController> ().DestroyInstances ();
					break;
				case "SkillsCanvas":
					UIDic [key].GetComponent<SkillsViewController> ().DestroyInstances ();
					break;
				case "SpellCanvas":
					UIDic [key].GetComponent<SpellViewController> ().DestroyInstances ();
					break;
				}

				UIDic.Remove (key);

			}
		}
	}
}
