using UnityEngine;
using UnityEditor;

public class AssetBundlesBuilder {

	[MenuItem("Assets/Build AssetBundles")]
	public static void BuildAssetBundles(){

		//打包资源的路径
		string targetPath = Application.dataPath + "/StreamingAssets";

		BuildPipeline.BuildAssetBundles (targetPath, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.iOS);


	}
}
