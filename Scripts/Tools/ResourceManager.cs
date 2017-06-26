using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ResourceManager:SingletonMono<ResourceManager> {

	public float loadProgress;

	private CallBack callBack;

//	private bool stayInMemory;

	public List<Sprite> sprites = new List<Sprite>();

	public List<GameObject> gos = new List<GameObject> ();

//	private Dictionary<string,AssetBundle> dataCache = new Dictionary<string, AssetBundle> ();



	public void LoadAssetWithName(string filePath,CallBack callBack,bool isSync = false){


//		this.stayInMemory = stayInMemory;

		this.callBack = callBack;

		if (isSync) {
			LoadFromFileSync (filePath);
		} else {
			StartCoroutine ("LoadFromFileAsync",filePath);
		}

	}


	private void LoadFromFileSync(string filePath){
		string targetPath = Path.Combine (Application.streamingAssetsPath, filePath);
		var bundleLoadRequest = AssetBundle.LoadFromFile (targetPath);
		var assetsLoaded = bundleLoadRequest.LoadAllAssets ();

		foreach (Object obj in assetsLoaded) {
			GameObject go = Instantiate (obj as GameObject);
			go.name = obj.name;
			gos.Add(go);
		}

		if (callBack != null) {
			callBack ();
			gos.Clear ();
			sprites.Clear ();
		}
	}

	private IEnumerator LoadFromFileAsync(string filePath){

//		AssetBundle myLoadedAssetBundle = null;
//
//		foreach (string path in dataCache.Keys) {
//			if (filePath.Equals (path)) {
//				myLoadedAssetBundle = dataCache [path];
//				break;
//			}
//		}
//		if (myLoadedAssetBundle == null) {

			string targetPath = Path.Combine (Application.streamingAssetsPath, filePath);

			var bundleLoadRequest = AssetBundle.LoadFromFileAsync (targetPath);

			loadProgress = bundleLoadRequest.progress;

			yield return bundleLoadRequest;

			var myLoadedAssetBundle = bundleLoadRequest.assetBundle;


//		}

//		if (stayInMemory) {
//			dataCache [filePath] = myLoadedAssetBundle;
//		}

		if (myLoadedAssetBundle == null)
		{
			Debug.Log("Failed to load AssetBundle!");
			yield break;
		}

		var assetLoadRequest = myLoadedAssetBundle.LoadAllAssetsAsync ();
		yield return assetLoadRequest;
		foreach (Object obj in assetLoadRequest.allAssets) {
			if (obj.GetType() == typeof(Sprite)) {
				sprites.Add (obj as Sprite);
			}else if(obj.GetType() == typeof(Texture2D)){
				continue;
			}else {
				Instantiate (obj as GameObject).name = obj.name;
				Debug.Log (obj.name);
			}
		}

		myLoadedAssetBundle.Unload (false);

		if (callBack != null) {
			callBack ();
			gos.Clear ();
			sprites.Clear ();
		}

	}
		
}
