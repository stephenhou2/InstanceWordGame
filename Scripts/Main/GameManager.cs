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


		public int unlockedMaxChapterIndex = 0;

		private List<ItemModel> mAllItemModels = new List<ItemModel> ();
		public List<ItemModel> allItemModels{
			get{
				if (mAllItemModels.Count == 0) {
					
					ItemModel[] ItemArray = DataHandler.LoadDataToModelWithPath<ItemModel> (CommonData.itemsDataFilePath);

					foreach (ItemModel itemModel in ItemArray) {
						Debug.Log (itemModel.itemType);
						mAllItemModels.Add (itemModel);
					}

				}
				return mAllItemModels;
			}

		}

		private List<Sprite> mAllMapSprites = new List<Sprite> ();
		public List<Sprite> allMapSprites{
			get{
				if (mAllMapSprites.Count == 0) {

					ResourceManager.Instance.LoadAssetWithBundlePath<Sprite> ("mapicons", () => {

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

		private List<Sprite> mAllItemsSprites = new List<Sprite>();
		public List<Sprite> allItemSprites{

			get{
				if (mAllItemsSprites.Count == 0) {
					ResourceManager.Instance.LoadAssetWithBundlePath<Sprite> ("item/icons", () => {
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

					ResourceManager.Instance.gos.Clear ();

					ResourceManager.Instance.LoadAssetWithBundlePath ("skills/skill", () => {
						for(int i = 0;i<ResourceManager.Instance.gos.Count;i++){
							Skill skill = ResourceManager.Instance.gos[i].GetComponent<Skill>();
							mAllSkills.Add(skill);
							skill.transform.SetParent(allSkillsContainer);
						}
					},true);

				}

				SortSkillsById (mAllSkills);

				return mAllSkills;
			}

		}

		// 技能按照id排序方法
		private void SortSkillsById(List<Skill> skills){
			Skill temp;
			for (int i = 0; i < skills.Count - 1; i++) {
				for (int j = 0; j < skills.Count - 1 - i; j++) {
					Skill sBefore = skills [j];
					Skill sAfter = skills [j + 1];
					if (sBefore.skillId > sAfter.skillId) {
						temp = sBefore;
						skills [j] = sAfter;
						skills [j + 1] = temp; 
					}
				}
			}
		}

		private List<Sprite> mAllSkillSprites = new List<Sprite>();
		public List<Sprite> allSkillSprites{

			get{
				if (mAllSkillSprites.Count == 0) {
					ResourceManager.Instance.LoadAssetWithBundlePath<Sprite> ("skills/icons", () => {
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
					ResourceManager.Instance.LoadAssetWithBundlePath<Sprite>("ui_icons",()=>{
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
					ResourceManager.Instance.LoadAssetWithBundlePath<Sprite>("explore/icons",()=>{
						for(int i = 0;i<ResourceManager.Instance.sprites.Count;i++){
							mAllExploreIcons.Add(ResourceManager.Instance.sprites[i]);
						}
					},true);
				}
				return mAllExploreIcons;
			}


		}

		private List<Transform> mAllMonsters = new List<Transform>();
		public List<Transform> allMonsters{
			get{
				if (mAllMonsters.Count == 0) {
					ResourceManager.Instance.LoadAssetWithBundlePath ("monsters", () => {
						for(int i = 0;i<ResourceManager.Instance.gos.Count;i++){
							Transform monster = ResourceManager.Instance.gos[i].transform;
							mAllMonsters.Add(monster);
						};
					}, true);
				}
				return mAllMonsters;
			}
		}

		void Awake(){
			
			#warning 加载本地游戏数据,后面需要写一下
			mGameSettings = DataHandler.LoadDataToSingleModelWithPath<GameSettings> (CommonData.settingsFilePath);

			mLearnInfo = DataHandler.LoadDataToSingleModelWithPath<LearningInfo> (CommonData.learningInfoFilePath);

		}

		void Start(){
			soundManager.InitAudioClips ();
		}

		// 系统设置更改后更新相关设置
		public void OnSettingsChanged(){

			soundManager.effectAS.volume = gameSettings.systemVolume;
			soundManager.bgmAS.volume = gameSettings.systemVolume;

			soundManager.pronunciationAS.enabled = gameSettings.isPronunciationEnable;


			#warning 离线下载和更改词库的代码后续补充
			// 保存游戏设置到本地文件
			DataHandler.WriteModelDataToFile <GameSettings>(gameSettings, CommonData.settingsFilePath);

		}




		public void SetUpHomeView(Player player){

			ResourceManager.Instance.LoadAssetWithBundlePath ("home/canvas", () => {

				ResourceManager.Instance.gos[0].GetComponent<HomeViewController> ().SetUpHomeView ();

			});
		}

	}
}
