using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ResourceManager:SingletonMono<ResourceManager>
{

	public float loadProgress;

	private CallBack callBack;

	private bool spriteOnly;

	public List<Sprite> sprites = new List<Sprite> ();

	public List<GameObject> gos = new List<GameObject> ();

	private string fileName;

	//	private Dictionary<string,byte[]> dataCache = new Dictionary<string, byte[]> ();

	public void MaxCachingSpace (int maxCaching)
	{
		Caching.maximumAvailableDiskSpace = maxCaching * 1024 * 1024;
	}


	public void LoadAssetWithFileName (string bundlePath, CallBack callBack, bool isSync = false, string fileName = null)
	{

		this.fileName = fileName;

		this.callBack = callBack;

		this.spriteOnly = false;

		if (isSync) {
			LoadFromFileSync (bundlePath);
		} else {
			StartCoroutine ("LoadFromFileAsync", bundlePath);
		}

	}

	public void LoadSpritesAssetWithFileName(string bundlePath,CallBack callBack,bool isSync = false,string fileName = null){

		this.fileName = fileName;

		this.callBack = callBack;

		this.spriteOnly = true;

		if (isSync) {
			LoadFromFileSync (bundlePath);
		} else {
			StartCoroutine ("LoadFromFileAsync", bundlePath);
		}
	}


	private void LoadFromFileSync (string bundlePath)
	{
		
		string targetPath = Path.Combine (Application.streamingAssetsPath, bundlePath);

		var myLoadedAssetBundle = AssetBundle.LoadFromFile (targetPath);

		if (fileName != null) {
			
			var assetLoaded = myLoadedAssetBundle.LoadAsset (fileName);

			if (assetLoaded.GetType () == typeof(Sprite)) {
				Debug.Log ("加载图片" + assetLoaded.name);
				sprites.Add (assetLoaded as Sprite);
			} else if (assetLoaded.GetType () == typeof(Texture2D)) {
				Texture2D t2d = assetLoaded as Texture2D;
				Sprite s = Sprite.Create (t2d, new Rect (0.0f, 0.0f, t2d.width, t2d.height), new Vector2 (0.5f, 0.5f));
				Debug.Log ("加载图片" + assetLoaded.name);
				sprites.Add (s);
			} else if (!spriteOnly) {
				GameObject go = Instantiate (assetLoaded as GameObject);
				go.transform.SetParent (TransformManager.FindTransform (CommonData.instanceContainerName));
				go.name = assetLoaded.name;
				gos.Add (go);

			}


		} else {

			var assetsLoaded = myLoadedAssetBundle.LoadAllAssets ();

			foreach (Object obj in assetsLoaded) {
				if (obj.GetType () == typeof(Sprite)) {
					sprites.Add (obj as Sprite);
				} else if (obj.GetType () == typeof(Texture2D)) {
					continue;
				} else if (!spriteOnly) {
					GameObject go = Instantiate (obj as GameObject);
					go.transform.SetParent (TransformManager.FindTransform (CommonData.instanceContainerName));
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

	private IEnumerator LoadFromFileAsync (string bundlePath)
	{

		string targetPath = Path.Combine (Application.streamingAssetsPath, bundlePath);

		var bundleLoadRequest = AssetBundle.LoadFromFileAsync (targetPath);

		loadProgress = bundleLoadRequest.progress;

		yield return bundleLoadRequest;

		var myLoadedAssetBundle = bundleLoadRequest.assetBundle;




		if (myLoadedAssetBundle == null) {
			Debug.Log ("Failed to load AssetBundle!");
			yield break;
		}
			

		if (fileName != null) {

			var assetLoadRequest = myLoadedAssetBundle.LoadAssetAsync (fileName);

			yield return assetLoadRequest;

			var assetLoaded = assetLoadRequest.asset;

			if (assetLoaded.GetType () == typeof(Sprite)) {
				Debug.Log ("加载图片" + assetLoaded.name);
				sprites.Add (assetLoaded as Sprite);
			} else if (assetLoaded.GetType () == typeof(Texture2D)) {
				Texture2D t2d = assetLoaded as Texture2D;
				Sprite s = Sprite.Create (t2d, new Rect (0.0f, 0.0f, t2d.width, t2d.height), new Vector2 (0.5f, 0.5f));
				Debug.Log ("加载图片" + assetLoaded.name);
				sprites.Add (s);
			} else if (!spriteOnly) {
				GameObject go = Instantiate (assetLoaded as GameObject);
				go.transform.SetParent (TransformManager.FindTransform (CommonData.instanceContainerName));
				go.name = assetLoaded.name;
				gos.Add (go);

			}

		} else {
			
			var assetLoadRequest = myLoadedAssetBundle.LoadAllAssetsAsync ();

			yield return assetLoadRequest;

			var assetsLoaded = assetLoadRequest.allAssets;

			foreach (Object obj in assetsLoaded) {
				if (obj.GetType () == typeof(Sprite)) {
					sprites.Add (obj as Sprite);
				} else if (obj.GetType () == typeof(Texture2D)) {
					continue;
				} else if (!spriteOnly) {
					GameObject go = Instantiate (obj as GameObject);
					go.transform.SetParent (TransformManager.FindTransform (CommonData.instanceContainerName));
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



	public void WriteStringDataToFile(string stringData,string filePath){

		File.WriteAllText (filePath, stringData);

	}
		
}
