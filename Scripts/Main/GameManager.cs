using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;


namespace WordJourney
{
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
						ResourceManager.Instance.LoadAssetWithBundlePath ("main", ()=>{
							instance = ResourceManager.Instance.gos[0].GetComponent<GameManager>();
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
//					ResourceManager.Instance.LoadAssetWithBundlePath ("mapnpcs", () => {
//
//						foreach (GameObject mapNpc in ResourceManager.Instance.gos) {
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
//					ResourceManager.Instance.LoadAssetWithBundlePath ("monsters", () => {
//						for(int i = 0;i<ResourceManager.Instance.gos.Count;i++){
//						mAllMonsters.Add(ResourceManager.Instance.gos[i].GetComponent<Monster>());
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
//					ResourceManager.Instance.LoadAssetWithBundlePath ("npcs", () => {
//						for(int i = 0;i<ResourceManager.Instance.gos.Count;i++){
//							mAllNPCs.Add(ResourceManager.Instance.gos[i].GetComponent<NPC>());
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
			



	}
}
