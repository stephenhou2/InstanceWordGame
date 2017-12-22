using UnityEngine;
#if UNITY_EDITOR	
using UnityEditor;
#endif

namespace WordJourneyy
{
	public class AssetBundleUtility
	{
		public const string AssetBundlesOutputPath = "StreamingAssets/AssetBundles";
	
		public static string GetPlatformName()
		{
	#if UNITY_EDITOR
			return GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
	#else
			return GetPlatformForAssetBundles(Application.platform);
	#endif
		}
	
	#if UNITY_EDITOR
		private static string GetPlatformForAssetBundles(BuildTarget target)
		{
			switch(target)
			{
			case BuildTarget.Android:
				return "Android";
			case BuildTarget.iOS:
				return "iOS";
			case BuildTarget.StandaloneWindows:
			case BuildTarget.StandaloneWindows64:
				return "Windows";
			case BuildTarget.StandaloneOSXIntel:
			case BuildTarget.StandaloneOSXIntel64:
			case BuildTarget.StandaloneOSXUniversal:
				return "OSX";
			default:
				return null;
			}
		}
	#endif
	
		private static string GetPlatformForAssetBundles(RuntimePlatform platform)
		{
			switch(platform)
			{
			case RuntimePlatform.Android:
				return "Android";
			case RuntimePlatform.IPhonePlayer:
				return "iOS";
			case RuntimePlatform.WindowsPlayer:
				return "Windows";
			case RuntimePlatform.OSXPlayer:
				return "OSX";
			default:
				return null;
			}
		}
	}
}