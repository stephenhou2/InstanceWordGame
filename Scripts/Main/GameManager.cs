using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;


namespace WordJourney
{

	using UnityEngine.SceneManagement;

	public class GameManager : MonoBehaviour {

		private static volatile GameManager instance;  
		private static object objectLock = new System.Object();  
		public static  GameManager Instance {  
			get {  
				if (instance == null) {  
					lock (objectLock) {  
						GameManager[] instances = FindObjectsOfType<GameManager> ();  
						if (instances != null) {  
							for (var i = 0; i < instances.Length; i++) {  
								Destroy (instances [i].gameObject);  
							}  
						} 

						ResourceLoader gameManagerLoader = ResourceLoader.CreateNewResourceLoader ();

						ResourceManager.Instance.LoadAssetsWithBundlePath (gameManagerLoader, CommonData.mainStaticBundleName, ()=>{
							instance = gameManagerLoader.gos[0].GetComponent<GameManager>();
							instance.transform.SetParent(null);
						}, true,"GameManager");
					}  
				}  
				return instance;  
			}  
		}

		public SoundManager soundManager;

		public DragonBones.UnityFactory bonesFactory;

		public DataCenter dataCenter;


		public int unlockedMaxChapterIndex = 0;




//
//		private List<MapNPC> mAllMapNpcs = new List<MapNPC> ();
//		public List<MapNPC> allMapNpcs{
//			get{
//				if (mAllMapNpcs.Count == 0) {
//					ResourceLoader.CreateNewResourceLoader().LoadAssetWithBundlePath ("mapnpcs", () => {
//
//						foreach (GameObject mapNpc in ResourceLoader.CreateNewResourceLoader().gos) {
//							mAllMapNpcs.Add (mapNpc.GetComponent<MapNPC>());
//						}
//
//					}, true);
//				}
//				return mAllMapNpcs;
//			}
//		}

//		private List<Monster> mAllMonsters = new List<Monster> ();
//		public List<Monster> allMonsters{
//			get{
//				if (mAllMonsters.Count == 0) {
//					ResourceLoader.CreateNewResourceLoader().LoadAssetWithBundlePath ("monsters", () => {
//						for(int i = 0;i<ResourceLoader.CreateNewResourceLoader().gos.Count;i++){
//						mAllMonsters.Add(ResourceLoader.CreateNewResourceLoader().gos[i].GetComponent<Monster>());
//						}
//					},true);
//				}
//				return mAllMonsters;
//			}
//		}

//		private List<NPC> mAllNPCs = new List<NPC> ();
//		public List<NPC> allNPCs{
//			get{
//				if (mAllNPCs.Count == 0) {
//					ResourceLoader.CreateNewResourceLoader().LoadAssetWithBundlePath ("npcs", () => {
//						for(int i = 0;i<ResourceLoader.CreateNewResourceLoader().gos.Count;i++){
//							mAllNPCs.Add(ResourceLoader.CreateNewResourceLoader().gos[i].GetComponent<NPC>());
//						}
//					},true);
//				}
//				return mAllNPCs;
//			}
//		}



		void Awake(){

			dataCenter = new DataCenter ();

			#warning 加载本地游戏数据,后面需要写一下
			dataCenter.gameSettings = DataHandler.LoadDataToSingleModelWithPath<GameSettings> (CommonData.settingsFilePath);

			dataCenter.learnInfo = DataHandler.LoadDataToSingleModelWithPath<LearningInfo> (CommonData.learningInfoFilePath);

			bonesFactory = new DragonBones.UnityFactory ();

		}

		void Start(){
//			soundManager.InitUIAudioClips ();
		}

		// 系统设置更改后更新相关设置
		public void OnSettingsChanged(){

			soundManager.effectAS.volume = dataCenter.gameSettings.systemVolume;
			soundManager.bgmAS.volume = dataCenter.gameSettings.systemVolume;

			soundManager.pronunciationAS.enabled = dataCenter.gameSettings.isPronunciationEnable;


			#warning 离线下载和更改词库的代码后续补充
			// 保存游戏设置到本地文件
			DataHandler.WriteModelDataToFile <GameSettings>(dataCenter.gameSettings, CommonData.settingsFilePath);

		}
			
		#warning 如果决定使用scene来进行场景转换打开下面的代码
//		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
//		static public void CallbackInitialization()
//		{
//			//register the callback to be called everytime the scene is loaded
//			SceneManager.sceneLoaded += OnSceneLoaded;
//		}
//
//		static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
//		{
//			switch (arg0.name) {
//			case "GameScene":
//				TransformManager.FindTransform ("GameLoader").GetComponent<GameLoader> ().SetUpHomeView ();
//				break;
//			case "ExploreScene":
//				int currentExploreLevel = GameManager.Instance.unlockedMaxChapterIndex;
//
//				ResourceLoader exploreSceneLoader = ResourceLoader.CreateNewResourceLoader ();
//
//				ResourceManager.Instance.LoadAssetsWithBundlePath (exploreSceneLoader, "explore/scene", () => {
//
//					TransformManager.FindTransform ("ExploreManager").GetComponent<ExploreManager> ().SetupExploreView (currentExploreLevel);
//
//				}, true);
//				break;
//
//			}
//
//		}


	}
}
