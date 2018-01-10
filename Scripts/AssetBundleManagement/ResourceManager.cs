using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object= UnityEngine.Object;

namespace WordJourney
{

	public class MyLoadedAssetBundle{

		public AssetBundle assetBundle;

		public int assetBundleRefCount;

		public MyLoadedAssetBundle(AssetBundle assetBundle){
			this.assetBundle = assetBundle;
			this.assetBundleRefCount = 1;
		}
	}
		
	public class ResourceManager:SingletonMono<ResourceManager> {

		private Dictionary<string,WWW> m_DownloadingWWWs = new Dictionary<string, WWW> ();

		private Dictionary<string,MyLoadedAssetBundle> m_LoadedAssetBundles = new Dictionary<string, MyLoadedAssetBundle> ();

		private Dictionary<string, string> m_DownloadingErrors = new Dictionary<string, string> ();

		private Dictionary<string, string[]> m_Dependencies = new Dictionary<string, string[]> ();

//		private List<ResourceLoader> m_InProgressLoaders = new List<ResourceLoader> ();

		private AssetBundleManifest manifest;

		private bool manifestReady = false;

		private AssetBundleLoadType loadType;

		private enum AssetBundleLoadType
		{
			AssetBundleSync,
			AssetBundleAsync,
			WWW
		}

		void Awake(){
			Debug.Log ("resource manager awake");
		}

		public void LoadAssetsFromFileAsync(ResourceLoader loader,CallBack callBack){

			this.loadType = AssetBundleLoadType.AssetBundleAsync;

//			m_InProgressLoaders.Add (loader);

			loader.SetFinishAssetsLoadingCallBack (delegate {
				callBack ();
//				m_InProgressLoaders.Remove (loader);
				Destroy(loader.gameObject);
			});

			StartCoroutine ("WaitReferencedDataReady", loader);

		}

		public void LoadAssetsFromFileSync(ResourceLoader loader,CallBack callBack){

			this.loadType = AssetBundleLoadType.AssetBundleSync;

			loader.SetFinishAssetsLoadingCallBack (delegate {
				callBack ();
				Destroy(loader.gameObject);
			});

			LoadAllDependecyBundles (loader.assetBundleName);

			AssetBundle bundle = loader.LoadAssetsBundleFromFileSync ();

			loader.LoadAssetsSync (bundle);



		}


		public void LoadAssetsUsingWWW(ResourceLoader loader,CallBack callBack){

			this.loadType = AssetBundleLoadType.WWW;

//			m_InProgressLoaders.Add (loader);

			loader.SetFinishAssetsLoadingCallBack (delegate {
				callBack ();
//				m_InProgressLoaders.Remove (loader);
				Destroy(loader.gameObject);
			});

			StartCoroutine ("WaitReferencedDataReady", loader);

		}

		private IEnumerator WaitReferencedDataReady(ResourceLoader loader){

			yield return new WaitUntil (() => manifestReady);

			string[] dependencies = LoadAllDependecyBundles (loader.assetBundleName);

			for (int i = 0; i < dependencies.Length; i++) {
				string dependencyBundleName = dependencies [i];

					if (!m_LoadedAssetBundles.ContainsKey (dependencyBundleName)) {
						Debug.LogFormat ("load dependecy {0}", dependencyBundleName);
						yield return m_DownloadingWWWs[dependencyBundleName];
					}
			}

			AssetBundle bundle = GetAssetBundleCache (loader.assetBundleName);

			// 缓存中有已经有对应bundle
			if (bundle != null) {
				loader.LoadAssetsAsync (bundle);
			} else {
				loader.LoadAssetsBundleWithWWW ();
				// www 读取／下载完成后读取bundle数据
				IEnumerator coroutine = LoadAssetsAfterWWWFinish (loader);
				StartCoroutine (coroutine);
			}
		}

		/// <summary>
		/// 查询缓存中是否有对应AssetBundle
		/// </summary>
		/// <param name="bundleName">Bundle name.</param>
		public bool BundleCacheExist(string bundleName){
			return m_LoadedAssetBundles.ContainsKey (bundleName);
		}

		/// <summary>
		/// 查询是否有正在下载中的www
		/// </summary>
		/// <param name="bundleName">Bundle name.</param>
		public bool DownloadingWWWExist(string bundleName){
			return m_DownloadingWWWs.ContainsKey (bundleName);
		}


		/// <summary>
		/// 将assetbundle加入缓存中
		/// </summary>
		public void AddBundleIntoCache(string bundleName,AssetBundle bundle){

			Debug.LogFormat ("Add bundle {0} into cache", bundleName);

			if (m_LoadedAssetBundles.ContainsKey (bundleName)) {
				string error = string.Format ("there is already a loaded bundle named {0} in loading, please don't over load asset bundle", bundleName);
				Debug.LogError (error);
				return;
			}
			MyLoadedAssetBundle myLoadedBundle = new MyLoadedAssetBundle (bundle);
			m_LoadedAssetBundles.Add (bundleName, myLoadedBundle);
		}

		/// <summary>
		/// 将www加入到缓存中
		/// </summary>
		public void AddWWWIntoCache(string bundleName,WWW www){

			Debug.LogFormat ("Add www {0} into cache", bundleName);

			if (m_DownloadingWWWs.ContainsKey (bundleName)) {
				string error = string.Format ("there is already a in loading www named {0}, please don't over load asset bundle", bundleName);
				Debug.LogError (error);
				return;
			}
			m_DownloadingWWWs.Add (bundleName, www);
		}

		/// <summary>
		/// 从缓存中删除指定www
		/// </summary>
		/// <param name="bundleName">Bundle name.</param>
		public void RemoveWWWFromCache(string bundleName){

			Debug.LogFormat ("remove www {0} from cache", bundleName);

			if (!m_DownloadingWWWs.ContainsKey (bundleName)) {
				string error = string.Format ("there is not a loading www named {0}", bundleName);
				Debug.LogError (error);
				return;
			}
			m_DownloadingWWWs.Remove (bundleName);
		}

		/// <summary>
		/// 获取assetbundle的所有依赖文件名称
		/// </summary>
		public string[] GetDependencies(string bundleName){

			if (m_Dependencies.ContainsKey (bundleName)) {
				return m_Dependencies [bundleName];
			}

			string[] dependencies = manifest.GetAllDependencies (bundleName);

			m_Dependencies.Add (bundleName, dependencies);

			return dependencies;

		}

	
		/// <summary>
		/// 获取缓存中的AssetBundle
		/// </summary>
		public AssetBundle GetAssetBundleCache(string bundleName){
			if (!BundleCacheExist (bundleName)) {
				return null;
			}
			MyLoadedAssetBundle loadedBundle = m_LoadedAssetBundles [bundleName];

			loadedBundle.assetBundleRefCount++;

			return loadedBundle.assetBundle;
		}

		public void UnloadAssetBunlde(string assetBundleName){

			if (!m_LoadedAssetBundles.ContainsKey (assetBundleName)) {
				Debug.LogError (string.Format ("缓存中没有名称为{0}的包体数据", assetBundleName));
			}

			if (--m_LoadedAssetBundles [assetBundleName].assetBundleRefCount == 0) {
				m_LoadedAssetBundles [assetBundleName].assetBundle.Unload (false);
				m_LoadedAssetBundles.Remove (assetBundleName);
			}

		}

		public AssetBundle GetLoadedAssetBundle(string bundleName){
			if (!BundleCacheExist (bundleName)) {
				return null;
			}
			return m_LoadedAssetBundles [bundleName].assetBundle;
		}

		/// <summary>
		/// 获取正在下载中的www
		/// </summary>
		public WWW GetDownloadingWWW(string bundleName){
			if (!DownloadingWWWExist (bundleName)) {
				return null;
			}
			return m_DownloadingWWWs [bundleName];
		}

		public void SetUpManifest(){

			string bundleName = "StreamingAssets";

			ResourceLoader manifestLoader = ResourceLoader.CreateNewResourceLoader<AssetBundleManifest> (bundleName, "AssetBundleManifest");

			manifestLoader.SetFinishAssetsLoadingCallBack (delegate {
				manifest = manifestLoader.assets [0] as AssetBundleManifest;
				manifestReady = true;
				Destroy(manifestLoader.gameObject);
			});

			manifestLoader.LoadAssetsBundleWithWWW ();

			// www 读取／下载完成后读取bundle数据
			IEnumerator coroutine = LoadAssetsAfterWWWFinish (manifestLoader);
			StartCoroutine (coroutine);

		}



		private string[] LoadAllDependecyBundles(string bundleName){

			string[] dependencies = GetDependencies (bundleName);

			foreach (string dependency in dependencies) {
				if (GetLoadedAssetBundle (dependency) == null) {
					ResourceLoader dependencyBundleLoader = ResourceLoader.CreateNewResourceLoader<Object> (dependency);
					dependencyBundleLoader.SetFinishBundleLoadingCallBack (delegate {
						Destroy(dependencyBundleLoader.gameObject);
					});
					switch (loadType) {
					case AssetBundleLoadType.AssetBundleSync:
						dependencyBundleLoader.LoadAssetsBundleFromFileSync ();
						Destroy (dependencyBundleLoader.gameObject);
						break;
					case AssetBundleLoadType.AssetBundleAsync:
						dependencyBundleLoader.LoadAssetsBundleFromFileAsync ();
						break;
					case AssetBundleLoadType.WWW:
						dependencyBundleLoader.LoadAssetsBundleWithWWW ();
//						IEnumerator coroutine = LoadAssetsAfterWWWFinish (www, dependency, null);
//						StartCoroutine (coroutine);
						break;
					}
				}
			}
				
			return dependencies;

		}

		private IEnumerator LoadAssetsAfterAsyncLoadFinish(string assetBundleName, ResourceLoader loader){

			yield return new WaitUntil (() => loader.bundleCreateRequest.isDone);

			AssetBundle bundle = loader.bundleCreateRequest.assetBundle;

			if (bundle == null) {
				string error = string.Format ("{0}格式不正确，无法读取\nfilePath:{1}", assetBundleName,loader.GetBundleFilePath());
				Debug.LogError (error);
			} else {

				if (loader != null) {
					loader.LoadAssetsAsync (bundle);
				}

			}

		}


		private IEnumerator LoadAssetsAfterWWWFinish(ResourceLoader loader){

			yield return new WaitUntil (() => loader.downloadWWW.isDone);

			if (loader.downloadWWW.assetBundle == null) {
				string error = string.Format ("{0}格式不正确，无法读取\nurl:{1}", loader.assetBundleName,loader.downloadWWW.url);
				Debug.LogError (error);
			} else {

				if (loader != null) {
					loader.LoadAssetsAsync (loader.downloadWWW.assetBundle);
				}

				loader.downloadWWW.Dispose ();
				RemoveWWWFromCache (loader.assetBundleName);

			}

		}






	}
}
