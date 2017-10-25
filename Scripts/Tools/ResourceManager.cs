using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object=UnityEngine.Object;

namespace WordJourney
{
	public class ResourceManager : SingletonMono<ResourceManager> {

		public Dictionary<string,AssetBundle> cacheDic = new Dictionary<string, AssetBundle> ();

		public void AddCache(string name,AssetBundle ab){
			cacheDic.Add (name, ab);
		}


		public void UnloadCaches(string cacheName){

			if (cacheDic [cacheName] != null) {
				cacheDic [cacheName].Unload (true);
				Resources.UnloadUnusedAssets ();
				System.GC.Collect ();
			}

		}

		public void LoadAssetsWithBundlePath (ResourceLoader resourceLoader, string bundleName, CallBack callBack, bool isSync = false, string fileName = null)
		{

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

			if (cacheDic.ContainsKey (bundleName)) {

				if (cacheDic [bundleName] == null) {

					throw new System.Exception (string.Format("{0} is null",bundleName));

				}


				AssetBundle myLoadedAssetBundle = cacheDic [bundleName];

				resourceLoader.LoadAssetWithAssetBundle (myLoadedAssetBundle, isSync, callBack, fileName);

				return;
			}

			resourceLoader.LoadAssetsWithBundleName<T> (bundleName, callBack, isSync, fileName);
		}

	}
}
