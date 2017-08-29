using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;



namespace WordJourney
{
	public class GameManager : SingletonMono<GameManager> {


		private GameSettings mGameSettings;

		public GameSettings gameSettings{

			get{
				
				if (mGameSettings == null) {
					mGameSettings = new GameSettings ();
				}
				return mGameSettings;

			}
			set{
				mGameSettings = value;
			}
		}

		private LearningInfo mLearnInfo;

		public LearningInfo learnInfo{
			get{
				if (mLearnInfo == null) {
					mLearnInfo = LearningInfo.Instance;
				}
				mLearnInfo.SetUpWords ();
				return mLearnInfo;
			}
			set{
				mLearnInfo = value;
			}
		}

		private AudioSource pronunciationAs;

		private AudioSource effectAs;

		private AudioSource bgmAs;

		public int unlockedMaxChapterIndex = 0;

		private List<Item> mAllItems = new List<Item> ();
		public List<Item> allItems{
			get{
				if (mAllItems.Count == 0) {
					
					Item[] ItemArray = DataInitializer.LoadDataToModelWithPath<Item> (CommonData.jsonFileDirectoryPath, "AllItemsJson.txt");

					foreach (Item Item in ItemArray) {
						mAllItems.Add (Item);
					}

				}
				return mAllItems;
			}

		}

		private List<Sprite> mAllMapSprites = new List<Sprite> ();
		public List<Sprite> allMapSprites{
			get{
				if (mAllMapSprites.Count == 0) {

					ResourceManager.Instance.LoadSpritesAssetWithFileName ("mapicons", () => {

						foreach (Sprite s in ResourceManager.Instance.sprites) {
							mAllMapSprites.Add (s);
						}
					},true);

				}
				return mAllMapSprites;
			}

		}
//
//		private List<MapNPC> mAllMapNpcs = new List<MapNPC> ();
//		public List<MapNPC> allMapNpcs{
//			get{
//				if (mAllMapNpcs.Count == 0) {
//					ResourceManager.Instance.LoadAssetWithFileName ("mapnpcs", () => {
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
//					ResourceManager.Instance.LoadAssetWithFileName ("monsters", () => {
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
//					ResourceManager.Instance.LoadAssetWithFileName ("npcs", () => {
//						for(int i = 0;i<ResourceManager.Instance.gos.Count;i++){
//							mAllNPCs.Add(ResourceManager.Instance.gos[i].GetComponent<NPC>());
//						}
//					},true);
//				}
//				return mAllNPCs;
//			}
//		}

		private List<Sprite> mAllItemsSprites = new List<Sprite>();
		public List<Sprite> allItemSprites{

			get{
				if (mAllItemsSprites.Count == 0) {
					ResourceManager.Instance.LoadAssetWithFileName ("item/icons", () => {
						// 获取所有游戏物品的图片
						for(int i = 0;i<ResourceManager.Instance.sprites.Count;i++){
							mAllItemsSprites.Add(ResourceManager.Instance.sprites[i]);
						}
					},true);
				}

				return mAllItemsSprites;
			}

	//		set{
	//			mAllItemsSprites = value;
	//		}

		}

		private List<Skill> mAllSkills = new List<Skill>();
		public List<Skill> allSkills{

			get{
				if(mAllSkills.Count == 0){
					Transform allSkillsContainer = TransformManager.NewTransform("AllSkills",GameObject.Find(CommonData.instanceContainerName).transform);
					ResourceManager.Instance.LoadAssetWithFileName ("skills/skills", () => {
						for(int i = 0;i<ResourceManager.Instance.gos.Count;i++){
							Skill skill = ResourceManager.Instance.gos[i].GetComponent<Skill>();
							mAllSkills.Add(skill);
							skill.transform.SetParent(allSkillsContainer);
						}
					});

				}

				return mAllSkills;
			}

		}

		private List<Sprite> mAllSkillSprites = new List<Sprite>();
		public List<Sprite> allSkillSprites{

			get{
				if (mAllSkillSprites.Count == 0) {
					ResourceManager.Instance.LoadAssetWithFileName ("skill/icons", () => {
						// 获取所有游戏物品的图片
						for(int i = 0;i<ResourceManager.Instance.sprites.Count;i++){
							mAllSkillSprites.Add(ResourceManager.Instance.sprites[i]);
						}
					},true);
				}

				return mAllSkillSprites;
			}

	//		set{
	//			mAllEffectsSprites = value;
	//		}

		}

		private List<Sprite> mAllUIIcons = new List<Sprite> ();
		public List<Sprite> allUIIcons{

			get{
				if (mAllUIIcons.Count == 0) {
					ResourceManager.Instance.LoadAssetWithFileName("ui_icons",()=>{
						for(int i = 0;i<ResourceManager.Instance.sprites.Count;i++){
							mAllUIIcons.Add(ResourceManager.Instance.sprites[i]);
						}
					},true);
				}
				return mAllUIIcons;
			}


		}


		private List<Sprite> mAllExploreIcons = new List<Sprite> ();
		public List<Sprite> allExploreIcons{

			get{
				if (mAllExploreIcons.Count == 0) {
					ResourceManager.Instance.LoadAssetWithFileName("explore/icons",()=>{
						for(int i = 0;i<ResourceManager.Instance.sprites.Count;i++){
							mAllExploreIcons.Add(ResourceManager.Instance.sprites[i]);
						}
					},true);
				}
				return mAllExploreIcons;
			}


		}

		private List<Monster> mAllMonsters = new List<Monster>();
		public List<Monster> allMonsters{
			get{
				if (mAllMonsters.Count == 0) {
					ResourceManager.Instance.LoadAssetWithFileName ("monsters", () => {
						for(int i = 0;i<ResourceManager.Instance.gos.Count;i++){
							Monster m = ResourceManager.Instance.gos[i].GetComponent<Monster>();
							mAllMonsters.Add(m);
						};
					}, true);
				}
				return mAllMonsters;
			}
		}

		void Awake(){

			#warning 加载本地游戏数据,后面需要写一下
			mGameSettings = DataInitializer.LoadDataToSingleModelWithPath<GameSettings> (Application.persistentDataPath, CommonData.settingsFileName);

			mLearnInfo = DataInitializer.LoadDataToSingleModelWithPath<LearningInfo> (Application.persistentDataPath, CommonData.learningInfoFileName);

			ResourceManager.Instance.MaxCachingSpace (200);

			SetUpHomeView (Player.mainPlayer);

		}

		void Start(){
			SetUpAudioSources ();
		}

		private void SetUpAudioSources(){

			pronunciationAs = Instance.gameObject.AddComponent<AudioSource> ();
			effectAs = Instance.gameObject.AddComponent<AudioSource> ();
			bgmAs = Instance.gameObject.AddComponent<AudioSource> ();

			pronunciationAs.playOnAwake = false;
			effectAs.playOnAwake = false;

		}

		// 系统设置更改后更新相关设置
		public void OnSettingsChanged(){

			effectAs.volume = gameSettings.systemVolume;
			bgmAs.volume = gameSettings.systemVolume;

			pronunciationAs.enabled = gameSettings.isPronunciationEnable;


			#warning 离线下载和更改词库的代码后续补充

			SaveGameSettings ();

		}




		private void SetUpHomeView(Player player){

			ResourceManager.Instance.LoadAssetWithFileName ("home/canvas", () => {

				ResourceManager.Instance.gos[0].GetComponent<HomeViewController> ().SetUpHomeView ();

			});
		}
			
		public void SaveGameSettings(){

			string settingsString = JsonUtility.ToJson (gameSettings);

			ResourceManager.Instance.WriteStringDataToFile (settingsString, Application.persistentDataPath + "/" + CommonData.settingsFileName);

			Debug.Log (Application.persistentDataPath + "/" + CommonData.settingsFileName);

		}

		public void SaveLearnInfo(){

			string learnInfoStr = JsonUtility.ToJson (learnInfo);

			ResourceManager.Instance.WriteStringDataToFile (learnInfoStr, Application.persistentDataPath + "/" + CommonData.learningInfoFileName);

			Debug.Log (Application.persistentDataPath + "/" + CommonData.learningInfoFileName);

		}

	}
}
