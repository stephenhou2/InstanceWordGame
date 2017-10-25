using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	/// <summary>
	/// streamingAssets内资源加载类
	/// </summary>
	public class ResourceLoader:MonoBehaviour{

		// 加载进度
		public float loadProgress;

		// 从本地加载出的资源包
		private AssetBundle myLoadedAssetBundle;

		private string bundleName;

		// 指定加载的文件名
		private string fileName;

		// 加载完成回调
		private CallBack callBack;

		// 想要加载的资源类型
		private System.Type type;

		// 加载asset请求
		private AssetBundleRequest assetLoadRequest;

		private Object[] assetsLoaded;

		// 从本地加载的图片
		public List<Sprite> sprites = new List<Sprite> ();

		// 从本地加载的游戏体
		public List<GameObject> gos = new List<GameObject> ();

		// 从本地加载的音频
		public List<AudioClip> audioClips = new List<AudioClip> ();

	
		//	private Dictionary<string,byte[]> dataCache = new Dictionary<string, byte[]> ();		
		public static ResourceLoader CreateNewResourceLoader(){

			ResourceLoader rm = new GameObject ().AddComponent<ResourceLoader> ();

			return rm;		
		}

		/// <summary>
		/// 加载指定名称的bundle中的资源
		/// </summary>
		/// <param name="bundlePath">Bundle path.</param>
		/// <param name="callBack">加载完成后回调</param>
		/// <param name="isSync">是否采用同步加载[default：false]</param>
		/// <param name="fileName">指定文件名（指定后只加载指定名称的文件镜像）.</param>
		public void LoadAssetsWithBundleName (string bundleName, CallBack callBack,  bool isSync = false, string fileName = null)
		{
			this.bundleName = bundleName;
			this.fileName = fileName;
			this.callBack = callBack;

			if (isSync) {
				LoadFromFileSync (bundleName);
			} else {
				StartCoroutine ("LoadFromFileAsync", bundleName);
			}
				
		}

		/// <summary>
		/// 加载指定名称的bundle中的资源
		/// </summary>
		/// <param name="bundlePath">Bundle path.</param>
		/// <param name="callBack">加载完成后回调</param>
		/// <param name="isSync">是否采用同步加载[default：false]</param>
		/// <param name="fileName">指定文件名（指定后只加载指定名称的文件镜像）.</param>
		/// <typeparam name="T">指定加载资源的类型</typeparam>
		public void LoadAssetsWithBundleName<T> (string bundleName, CallBack callBack, bool isSync = false, string fileName = null)
		{
			this.type = typeof(T);

			LoadAssetsWithBundleName (bundleName,callBack, isSync, fileName);

		}

			
		/// <summary>
		/// 同步加载资源
		/// </summary>
		/// <param name="bundlePath">Bundle path.</param>
		private void LoadFromFileSync (string bundleName)
		{
			string targetPath = string.Format("{0}/{1}",Application.streamingAssetsPath, bundleName);

			myLoadedAssetBundle = AssetBundle.LoadFromFile (targetPath);

			ResourceManager.Instance.AddCache (bundleName, myLoadedAssetBundle);

			LoadAssetWithAssetBundle (myLoadedAssetBundle,true,callBack,fileName);

		}

		/// <summary>
		/// 异步加载资源
		/// </summary>
		/// <returns>The from file async.</returns>
		/// <param name="bundlePath">Bundle path.</param>
		private IEnumerator LoadFromFileAsync (string bundleName)
		{

			string targetPath = string.Format("{0}/{1}",Application.streamingAssetsPath, bundleName);

			var bundleLoadRequest = AssetBundle.LoadFromFileAsync (targetPath);

			loadProgress = bundleLoadRequest.progress;

			yield return bundleLoadRequest;

			myLoadedAssetBundle = bundleLoadRequest.assetBundle;

			if (myLoadedAssetBundle == null) {
				Debug.Log ("Failed to load AssetBundle!");
				yield break;
			}

			ResourceManager.Instance.AddCache (bundleName, myLoadedAssetBundle);

			LoadAssetWithAssetBundle (myLoadedAssetBundle,false,callBack,fileName);

		}

		public void LoadAssetWithAssetBundle(AssetBundle myLoadedAssetBundle,bool isSync,CallBack callback,string fileName){

			if (callBack == null) {
				this.callBack = callback;
			}

			if (fileName != null) {
				this.fileName = fileName;
			}

			if (isSync) {
				LoadAssetsSync (myLoadedAssetBundle);
			} else {
				StartCoroutine ("LoadAssetsAsync", myLoadedAssetBundle);
			}

		}


		private void LoadAssetsSync(AssetBundle myLoadedAssetBundle){

			if (fileName != null) {

				if (type != null) {
					assetsLoaded = new Object[]{myLoadedAssetBundle.LoadAsset (fileName, type)};
				} else {
					assetsLoaded = new Object[]{myLoadedAssetBundle.LoadAsset (fileName)};
				}
			} else {

				if (type != null) {
					assetsLoaded = myLoadedAssetBundle.LoadAllAssets (type);
				} else {
					assetsLoaded = myLoadedAssetBundle.LoadAllAssets ();
				}
			}
			LoadResourceWithAssetObjects (assetsLoaded);

		}

		private IEnumerator LoadAssetsAsync(AssetBundle myLoadedAssetBundle){

			if (fileName != null) {

				if (type != null) {
					assetLoadRequest = myLoadedAssetBundle.LoadAssetAsync (fileName, type);
				} else {
					assetLoadRequest = myLoadedAssetBundle.LoadAssetAsync (fileName);
				}

				yield return assetLoadRequest;

			} else {

				if (type != null) {
					assetLoadRequest = myLoadedAssetBundle.LoadAllAssetsAsync (type);
				} else {
					assetLoadRequest = myLoadedAssetBundle.LoadAllAssetsAsync ();
				}

				yield return assetLoadRequest;

			}

			assetsLoaded = assetLoadRequest.allAssets;

			LoadResourceWithAssetObjects (assetsLoaded);

		}


		/// <summary>
		/// 处理加载出的资源
		/// 1.图片：放入缓存 
		/// 2.prefab：拷贝后放入缓存
		/// 清理资源
		/// </summary>
		/// <param name="assetsLoaded">Assets loaded.</param>
		private void LoadResourceWithAssetObjects(Object[] assetsLoaded){

			for (int i = 0; i < assetsLoaded.Length; i++) {

				Object obj = assetsLoaded [i];

				if (obj.GetType () == typeof(Sprite)) {
					sprites.Add (obj as Sprite);
				} else if(obj.GetType() == typeof(Texture2D)){
					continue;
				}else if(obj.GetType() == typeof(AudioClip)){
					audioClips.Add (obj as AudioClip);
				}else {
					GameObject go = Instantiate (obj as GameObject, Vector3.zero, Quaternion.identity);
					go.transform.SetParent (TransformManager.FindOrCreateTransform (CommonData.instanceContainerName));
					go.name = obj.name;
					gos.Add (go);
				}

				obj = null;
			}

			ExcuteCallBackAndClearMemory ();

		}

		/// <summary>
		/// Excutes the call back and clear memory.
		/// </summary>
		private void ExcuteCallBackAndClearMemory(){

			// 清除加载信息
			assetLoadRequest = null;

			assetsLoaded = null;


			// 执行资源加载完成后回调，取得资源必须在回调中完成，之后资源会从内存中删除
			if (callBack != null) {
				callBack ();
			}

			gos.Clear();

			sprites.Clear();

			audioClips.Clear();

			Destroy (this.gameObject);

		}



	}
}
