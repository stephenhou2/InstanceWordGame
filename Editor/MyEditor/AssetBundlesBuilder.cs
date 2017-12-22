using UnityEngine;
using UnityEditor;

public class AssetBundlesBuilder {

	[MenuItem("Assets/Build AssetBundles")]
	public static void BuildAssetBundles(){

		//打包资源的路径
		string targetPath = Application.dataPath + "/StreamingAssets";

		BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;

		switch (buildTarget) {
		case BuildTarget.Android:
			BuildPipeline.BuildAssetBundles (targetPath, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);
			break;
		case BuildTarget.iOS:
			BuildPipeline.BuildAssetBundles (targetPath, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.iOS);
			break;
		}

	}
}
