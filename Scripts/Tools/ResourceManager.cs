using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object=UnityEngine.Object;

namespace WordJourneyr
{
	public class ResourceManager : SingletonMono<ResourceManager> {

		public Dictionary<string,AssetBundle> cacheDic = new Dictionary<string, AssetBundle> ();

		public void AddCache(string bundleName,AssetBundle ab){
			cacheDic.Add (bundleName, ab);
		}


		public void UnloadCaches(string bundleName,bool completeUnload){

			if (cacheDic.ContainsKey(bundleName)) {
				cacheDic [bundleName].Unload (completeUnload);
				cacheDic.Remove (bundleName);
			}else{
				Debug.LogFormat ("缓存中未找到{0}", bundleName);
			}

		}

		public void LoadAssetsWithBundlePath (ResourceLoader resourceLoader, string bundleName, CallBack callBack, bool isSync = false, string fileName = null)
		{

			LoadDependencyAssets (bundleName);

			if (cacheDic.ContainsKey (bundleName)) {

				if (cacheDic [bundleName] == null) {

					throw new System.Exception (string.Format("{0} is null",bundleName));

				}


				AssetBundle myLoadedAssetBundle = cacheDic [bundleName];

				resourceLoader.LoadAssetWithAssetBundle (myLoadedAssetBundle, isSync, callBack, fileName);

				return;
			}

			resourceLoader.LoadAssetsWithBundleName (bundleName, callBack, isSync, fileName);
		}



		public void LoadAssetsWithBundlePath<T> (ResourceLoader resourceLoader, string bundleName, CallBack callBack, bool isSync = false, string fileName = null)
		{

			LoadDependencyAssets (bundleName);

			if (cacheDic.ContainsKey (bundleName)) {

				if (cacheDic [bundleName] == null) {

					throw new System.Exception (string.Format("{0} is null",bundleName));

				}


				AssetBundle myLoadedAssetBundle = cacheDic [bundleName];

				resourceLoader.LoadAssetWithAssetBundle<T> (myLoadedAssetBundle, isSync, callBack, fileName);

				return;
			}

			resourceLoader.LoadAssetsWithBundleName<T> (bundleName, callBack, isSync, fileName);
		}



		// 加载Manifest
		private void LoadDependencyAssets(string bundleName)
		{

			AssetBundle bundle = null;

			if (!cacheDic.ContainsKey ("ManifestBundle")) {

				string manifestPath = string.Format ("{0}/{1}", Application.streamingAssetsPath, "StreamingAssets");

				bundle = AssetBundle.LoadFromFile (manifestPath);

				cacheDic.Add ("ManifestBundle", bundle);
			} else {
				bundle = cacheDic ["ManifestBundle"];
			}

			AssetBundleManifest manifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

			string[] dependencies = manifest.GetAllDependencies (bundleName);

			for (int i = 0; i < dependencies.Length; i++) {
				string dependency = dependencies [i];
				if (!ResourceManager.Instance.cacheDic.ContainsKey (dependency)) {
					ResourceLoader dependencyLoader = ResourceLoader.CreateNewResourceLoader ();
					LoadAssetsWithBundlePath (dependencyLoader, dependency, null, true, null);
				}
			}

		}

	}
}