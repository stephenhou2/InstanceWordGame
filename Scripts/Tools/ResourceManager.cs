using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ResourceManager:SingletonMono<ResourceManager> {

	public float loadProgress;

	private CallBack callBack;

	public List<Sprite> sprites = new List<Sprite>();

	public List<GameObject> gos = new List<GameObject> ();

	private string fileName;

//	private Dictionary<string,byte[]> dataCache = new Dictionary<string, byte[]> ();

	public void MaxCachingSpace(int maxCaching){
		Caching.maximumAvailableDiskSpace = maxCaching * 1024 * 1024;
	}


	public void LoadAssetWithName(string bundlePath,CallBack callBack,bool isSync = false,string fileName = null){

		this.fileName = fileName;

		this.callBack = callBack;

		if (isSync) {
			LoadFromFileSync (bundlePath);
		} else {
			StartCoroutine ("LoadFromFileAsync",bundlePath);
		}

	}


	private void LoadFromFileSync(string bundlePath){
		
		string targetPath = Path.Combine (Application.streamingAssetsPath, bundlePath);

		var myLoadedAssetBundle = AssetBundle.LoadFromFile (targetPath);

		if (fileName != null) {
			
			var assetLoaded = myLoadedAssetBundle.LoadAsset (fileName);

			GameObject go = Instantiate (assetLoaded as GameObject);

			go.name = assetLoaded.name;

			gos.Add (go);


		} else {

			var assetsLoaded = myLoadedAssetBundle.LoadAllAssets ();

			foreach (Object obj in assetsLoaded) {
				if (obj.GetType () == typeof(Sprite)) {
					sprites.Add (obj as Sprite);
				} else if (obj.GetType () == typeof(Texture2D)) {
					continue;
				} else {
					GameObject go = Instantiate (obj as GameObject);
					go.name = obj.name;
					gos.Add (go);

				}
			}

		}
			
		myLoadedAssetBundle.Unload (false);

		if (callBack != null) {
			callBack ();
		}
		gos.Clear ();
		sprites.Clear ();
	}

	private IEnumerator LoadFromFileAsync(string bundlePath){

		string targetPath = Path.Combine (Application.streamingAssetsPath, bundlePath);

		var bundleLoadRequest = AssetBundle.LoadFromFileAsync (targetPath);

		loadProgress = bundleLoadRequest.progress;

		yield return bundleLoadRequest;

		var myLoadedAssetBundle = bundleLoadRequest.assetBundle;




		if (myLoadedAssetBundle == null)
		{
			Debug.Log("Failed to load AssetBundle!");
			yield break;
		}
			

		if (fileName != null) {

			var assetLoadRequest = myLoadedAssetBundle.LoadAssetAsync (fileName);

			yield return assetLoadRequest;

			var assetLoaded = assetLoadRequest.asset;

			GameObject go = Instantiate (assetLoaded as GameObject);

			go.name = assetLoaded.name;

			gos.Add (go);

		} else {
			
			var assetLoadRequest = myLoadedAssetBundle.LoadAllAssetsAsync ();

			yield return assetLoadRequest;

			var assetsLoaded = assetLoadRequest.allAssets;

			foreach (Object obj in assetsLoaded) {
				if (obj.GetType () == typeof(Sprite)) {
					sprites.Add (obj as Sprite);
				} else if (obj.GetType () == typeof(Texture2D)) {
					continue;
				} else {
					GameObject go = Instantiate (obj as GameObject);
					go.name = obj.name;
					gos.Add (go);
				}
			}
		}



		myLoadedAssetBundle.Unload (false);

		if (callBack != null) {
			callBack ();
		}
		gos.Clear ();
		sprites.Clear ();
	}
		
}
