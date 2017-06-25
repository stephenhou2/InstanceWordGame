using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ResourceManager:SingletonMono<ResourceManager> {

	public float loadProgress;

	private CallBack callBack;

	private bool isSprites = false;

	public List<Sprite> sprites = new List<Sprite>();



	public void LoadAssetWithName(string filePath,CallBack callBack,bool isSprites = false){

		this.isSprites = isSprites;

		this.callBack = callBack;

		StartCoroutine ("LoadFromFileAsync",filePath);



	}


	private IEnumerator LoadFromFileAsync(string filePath){

		string targetPath = Path.Combine (Application.streamingAssetsPath, filePath);

		var bundleLoadRequest = AssetBundle.LoadFromFileAsync(targetPath);

		loadProgress = bundleLoadRequest.progress;

		yield return bundleLoadRequest;

		var myLoadedAssetBundle = bundleLoadRequest.assetBundle;

		if (myLoadedAssetBundle == null)
		{
			Debug.Log("Failed to load AssetBundle!");
			yield break;
		}



		if (isSprites) {
			var assetLoadRequest = myLoadedAssetBundle.LoadAllAssetsAsync<Sprite> ();
			yield return assetLoadRequest;
			foreach (Object obj in assetLoadRequest.allAssets) {
				Debug.Log (obj);
				sprites.Add (obj as Sprite);
			}
		} else {
			var assetLoadRequest = myLoadedAssetBundle.LoadAllAssetsAsync ();
			yield return assetLoadRequest;
			foreach (Object obj in assetLoadRequest.allAssets) {
				Instantiate (obj as GameObject).name = obj.name;
			}
		}

		isSprites = false;
//		sprites.Clear ();
		myLoadedAssetBundle.Unload (false);

		if (callBack != null) {
			callBack ();
		}

	}
		
}
