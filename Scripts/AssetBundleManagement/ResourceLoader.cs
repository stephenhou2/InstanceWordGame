using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using System.IO;

	public class ResourceLoader:MonoBehaviour{

		public string assetBundleName;

		public System.Type assetType;

		public AssetBundleCreateRequest bundleCreateRequest;

		public WWW downloadWWW;

		public AssetBundleRequest bundleRequest;

		protected CallBack assetsLoadingFinishCallBack;

		protected CallBack bundleLoadingFinishCallBack;

		public string assetName;

		public Object[] assets;

		public GameObject InstantiateAsset(Object asset){
			GameObject go = Instantiate (asset as GameObject, Vector3.zero, Quaternion.identity);
			go.name = asset.name;
			return go;
		}

		protected bool finishLoading{
			get{ return bundleRequest != null && bundleRequest.isDone; }
		}

		public void SetFinishAssetsLoadingCallBack(CallBack callBack){
			this.assetsLoadingFinishCallBack = callBack;
		}

		public void SetFinishBundleLoadingCallBack(CallBack callBack){
			this.bundleLoadingFinishCallBack = callBack;
		}

		public static ResourceLoader CreateNewResourceLoader<T>(string assetBundleName,string assetName = null){

			ResourceLoader loader = new GameObject ().AddComponent<ResourceLoader> ();

			loader.gameObject.name = assetBundleName;

			loader.assetBundleName = assetBundleName;

			loader.assetName = assetName;

			loader.assetType = typeof(T);

			return loader;

		}

		/// <summary>
		/// 根据不同平台获取AssetBundle的url
		/// </summary>
		public string GetBundleURL(){

			string url = "";

			#if UNITY_EDITOR || UNITY_STANDALONE
			url = "file://" + Application.streamingAssetsPath + "/" + assetBundleName;
			#elif UNITY_ANDROID
			url = "jar:file://" + Application.dataPath + "!/assets/" + assetBundleName;
			#elif UNITY_IOS
			url = "file://" + Application.streamingAssetsPath + "/" + assetBundleName;
			#endif

			return url;

		}

		/// <summary>
		/// 根据不同平台获取AssetBundle的本地绝对路径
		/// </summary>
		/// <returns>The bundle file path.</returns>
		public string GetBundleFilePath(){
			return Application.streamingAssetsPath + "/" + assetBundleName;
		}


		/// <summary>
		/// 如果缓存中没有对应的assetbundle，则异步加载本地assetbundle
		/// </summary>
		public void LoadAssetsBundleFromFileAsync(){
			AssetBundle bundle = ResourceManager.Instance.GetAssetBundleCache (assetBundleName);
			if (bundle == null) {
				StartCoroutine ("MyLoadAssetsBundleFromFileAsync");
			}
		}

		private IEnumerator MyLoadAssetsBundleFromFileAsync(){
			string bundlePath = GetBundleFilePath ();
			if (!File.Exists (bundlePath)) {
				string error = string.Format ("{0} not exist!", bundlePath);
				Debug.LogError (error);
			}

			bundleCreateRequest = AssetBundle.LoadFromFileAsync (bundlePath);
			yield return bundleCreateRequest;
			if (bundleCreateRequest.assetBundle == null) {
				string error = string.Format ("{0} is not valid asset bundle format!", bundlePath);
				Debug.LogError (error);
			}
			ResourceManager.Instance.AddBundleIntoCache (assetBundleName, bundleCreateRequest.assetBundle);
			if (bundleLoadingFinishCallBack != null) {
				bundleLoadingFinishCallBack ();
			}
		}

		/// <summary>
		/// 如果缓存中没有对应asetbundle，则同步加载本地AssetBundle
		/// </summary>
		public AssetBundle LoadAssetsBundleFromFileSync(){
			string bundlePath = GetBundleFilePath ();
			if (!File.Exists (bundlePath)) {
				string error = string.Format ("{0} not exist!", bundlePath);
				Debug.LogError (error);
			}
			AssetBundle bundle = ResourceManager.Instance.GetAssetBundleCache (assetBundleName);
			if (bundle == null) {
				bundle = AssetBundle.LoadFromFile (bundlePath);
				if (bundle == null) {
					string error = string.Format ("{0} is not valid asset bundle format!", bundlePath);
					Debug.LogError (error);
				}
				ResourceManager.Instance.AddBundleIntoCache (assetBundleName, bundle);
			}

			return bundle;
		}

			/// <summary>
			/// 如果缓存中没有对应assetbundle，则开启www加载assetbundle
			/// </summary>
		public void LoadAssetsBundleWithWWW(){

			AssetBundle bundle = ResourceManager.Instance.GetAssetBundleCache (assetBundleName);

			if (bundle == null) {
				downloadWWW = ResourceManager.Instance.GetDownloadingWWW (assetBundleName);
				if (downloadWWW == null) {
				StartCoroutine ("DownloadAssetBundleWithWWW");
				}
			}
		}


		/// <summary>
		/// 开启www加载AssetBundle
		/// </summary>
		private IEnumerator DownloadAssetBundleWithWWW(){

			string url = GetBundleURL ();

//			downloadWWW = WWW.LoadFromCacheOrDownload (url, 0);
			 
			downloadWWW = new WWW (url);

			ResourceManager.Instance.AddWWWIntoCache (assetBundleName, downloadWWW);

			yield return downloadWWW;

			if (downloadWWW.assetBundle == null) {
				string error = string.Format ("{0} is not the right url or the resource is not in valid asset bundle format", downloadWWW.url);
				Debug.LogError (error);
			}

			ResourceManager.Instance.AddBundleIntoCache (assetBundleName, downloadWWW.assetBundle);
			if (bundleLoadingFinishCallBack != null) {
				bundleLoadingFinishCallBack ();
			}
		}


		//从assetbundle中同步加载资源
		public void LoadAssetsSync(AssetBundle bundle){

			if (assetName == null) {
			assets = bundle.LoadAllAssets (assetType);
			} else {
			assets = new Object[]{bundle.LoadAsset (assetName, assetType)};
			}

			if (assetsLoadingFinishCallBack != null) {
				assetsLoadingFinishCallBack ();
			}

		}
		
		public void LoadAssetsAsync(AssetBundle bundle){
			StartCoroutine ("MyLoadAssetsAsync", bundle);
		}


		//从assetbundle中异步加载资源
		private IEnumerator MyLoadAssetsAsync (AssetBundle bundle){

			if (assetName == null) {
				bundleRequest = bundle.LoadAllAssetsAsync (assetType);
				yield return bundleRequest;
			} else {

				if (!bundle.Contains (assetName)) {
					string error = string.Format ("未找到名为{0}的文件数据", assetName);
					Debug.LogError (error);
				}

				bundleRequest = bundle.LoadAssetAsync (assetName, assetType);

				yield return bundleRequest;
			}

			assets = bundleRequest.allAssets;

			if (assetsLoadingFinishCallBack != null) {
				assetsLoadingFinishCallBack ();
			}

		}

	}

}
