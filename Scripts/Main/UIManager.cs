using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class UIManager{

		private Transform mCanvasContainer;
		private Transform canvasContainer{
			get{
				if (mCanvasContainer == null) {
					mCanvasContainer = TransformManager.FindTransform ("CanvasContainer");
				}
				return mCanvasContainer;
			}
		}


		public Dictionary<string,Transform> UIDic = new Dictionary<string, Transform> ();


		public void SetUpCanvasWith(string bundleName,string canvasName,CallBack cb,bool isSync = false,bool keepBackCanvas = true){

			IDictionaryEnumerator dicEnumerator = UIDic.GetEnumerator ();

			if (!UIDic.ContainsKey (canvasName)) {

				ResourceLoader loader = ResourceLoader.CreateNewResourceLoader<GameObject> (bundleName);

				if (isSync) {
					ResourceManager.Instance.LoadAssetsFromFileSync (loader, () => {
						if (!keepBackCanvas) {
							while (dicEnumerator.MoveNext ()) {
								Canvas canvas = (dicEnumerator.Value as Transform).GetComponent<Canvas> ();
								canvas.enabled = false;
							}
						}

						Canvas c = null;

						foreach (Object asset in loader.assets) {
							GameObject obj = loader.InstantiateAsset (asset);
							if (obj.name == canvasName) {
								c = obj.GetComponent<Canvas> ();
								if (cb == null) {
									c.enabled = true;
								}
							}
						}

						if (cb != null) {
							cb ();
						}

						c.transform.SetParent(canvasContainer);
						c.transform.SetAsLastSibling ();

						ResetCanvasesSortintOrder ();

						UIDic.Add (canvasName, c.transform);

					});
				} else {

					ResourceManager.Instance.LoadAssetsUsingWWW (loader, () => {

						if (!keepBackCanvas) {
							while (dicEnumerator.MoveNext ()) {
								Canvas canvas = (dicEnumerator.Value as Transform).GetComponent<Canvas> ();
								canvas.enabled = false;
							}
						}

						Canvas c = null;

						foreach (Object asset in loader.assets) {
							GameObject obj = loader.InstantiateAsset (asset);
							if (obj.name == canvasName) {
								c = obj.GetComponent<Canvas> ();
								if (cb == null) {
									c.enabled = true;
								}
							}
						}

						if (cb != null) {
							cb ();
						}

						c.transform.SetParent(canvasContainer);
						c.transform.SetAsLastSibling ();

						ResetCanvasesSortintOrder ();

						UIDic.Add (canvasName, c.transform);

					});
				}

			} else {
				
				while (dicEnumerator.MoveNext ()) {

					Canvas c = (dicEnumerator.Value as Transform).GetComponent<Canvas> ();

					if (dicEnumerator.Key as string == canvasName) {
						if (cb == null) {
							c.enabled = true;
						}
						c.transform.SetAsLastSibling ();
					} else {
						c.enabled = c.enabled && keepBackCanvas;
					}
				}

				if (cb != null) {
					cb ();
				}

			}

			ResetCanvasesSortintOrder ();

			Resources.UnloadUnusedAssets ();
			System.GC.Collect ();
		}
			
		private void ResetCanvasesSortintOrder(){
			for (int i = 0; i < canvasContainer.childCount; i++) {
				Canvas canvas = canvasContainer.GetChild (i).GetComponent<Canvas>();
				canvas.sortingOrder = i;
			}
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
				TransformManager.DestroyTransfromWithName (poolContainerName, TransformRoot.InstanceContainer);
			}

			if (modelContainerName != null) {
				TransformManager.DestroyTransfromWithName (modelContainerName, TransformRoot.PoolContainer);
			}

			ResourceManager.Instance.UnloadAssetBunlde (bundleName);

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
//				case "WorkbenchCanvas":
//					UIDic [key].GetComponent<WorkBenchViewController> ().DestroyInstances ();
//					break;
				case "UnlockedItemsCanvas":
					UIDic [key].GetComponent<UnlockedItemsViewController> ().DestroyInstances ();
					break;
//				case "ProduceCanvas":
//					UIDic [key].GetComponent<ProduceViewController> ().DestroyInstances ();
//					break;
				case "RecordCanvas":
					UIDic [key].GetComponent<RecordViewController> ().DestroyInstances ();
					break;
				case "SettingCanvas":
					UIDic [key].GetComponent<SettingViewController> ().DestroyInstances ();
					break;
//				case "SkillsCanvas":
//					UIDic [key].GetComponent<SkillsViewController> ().DestroyInstances ();
//					break;
				case "SpellCanvas":
					UIDic [key].GetComponent<SpellViewController> ().DestroyInstances ();
					break;
				case "LearnCanvas":
					UIDic [key].GetComponent<LearnViewController> ().DestroyInstances ();
					break;
				}

				UIDic.Remove (key);

			}
		}
	}
}
