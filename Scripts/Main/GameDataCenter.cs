using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	using System.IO;

	public class GameDataCenter {

		public enum GameDataType
		{
			GameSettings,
//			LearnInfo,
			GameLevelDatas,
			ItemModels,
			ItemSprites,
			MapSprites,
			Skills,
			SkillSprites,
			UISprites,
			Monsters,
			NPCs,
		}

		private GameSettings mGameSettings;
		private List<GameLevelData> mGameLevelDatas = new List<GameLevelData>();
		private List<ItemModel> mAllItemModels = new List<ItemModel> ();
		private List<Sprite> mAllItemSprites = new List<Sprite>();
		private List<Sprite> mAllMapSprites = new List<Sprite> ();
		private List<Skill> mAllSkills = new List<Skill>();
		private List<Sprite> mAllSkillSprites = new List<Sprite>();
		private List<NPC> mAllNpcs = new List<NPC>();


		public void InitPersistentGameData(){
			LoadItemModels ();
			LoadAllItemSprites ();
			LoadAllSkills ();
		}


		public GameSettings gameSettings{

			get{
				if (mGameSettings == null) {
					LoadGameSettings ();
				}
				return mGameSettings;
			}
			set{
				mGameSettings = value;
			}
		}

		private void LoadGameSettings(){
			if (mGameSettings != null) {
				return;
			}
			mGameSettings = GameManager.Instance.persistDataManager.LoadGameSettings ();
			if (mGameSettings == null) {
				mGameSettings = new GameSettings ();
			}
		}




//		public LearningInfo learnInfo{
//			get{
//				if (mLearnInfo == null) {
//					LoadLearnInfo ();
//				}
//				return mLearnInfo;
//			}
//		}
//
//		private void LoadLearnInfo(){
//			if(inLoadingDataTypes.Contains(GameDataType.LearnInfo)){
//				return;
//			}
//			inLoadingDataTypes.Add (GameDataType.LearnInfo);
//			mLearnInfo = GameManager.Instance.persistDataManager.LoadLearnInfo ();
//			if (mLearnInfo == null) {
//				mLearnInfo = new LearningInfo ();
//			}
//			dataReadyDic [GameDataType.LearnInfo] = true;
//			inLoadingDataTypes.Remove (GameDataType.LearnInfo);
//		}


		public List<GameLevelData> gameLevelDatas{
			get{
				if (mGameLevelDatas.Count == 0) {
					LoadGameLevelDatas ();
				}
				return mGameLevelDatas;
			}

		}

		private void LoadGameLevelDatas(){
			GameLevelData[] gameLevelDatasArray = DataHandler.LoadDataToModelsWithPath<GameLevelData> (CommonData.gameLevelDataFilePath);
			for (int i = 0; i < gameLevelDatasArray.Length; i++) {
				mGameLevelDatas.Add(gameLevelDatasArray[i]);
			}
		}


		public List<ItemModel> allItemModels{
			get{
				if (mAllItemModels.Count == 0) {
					LoadItemModels ();
				}
				return mAllItemModels;
			}

		}

		private void LoadItemModels(){
			if (mAllItemModels.Count > 0) {
				return;
			}
			ItemModel[] itemModels = DataHandler.LoadDataToModelsWithPath<ItemModel> (CommonData.itemsDataFilePath);
			for (int i = 0; i < itemModels.Length; i++) {
				mAllItemModels.Add (itemModels [i]);
			}
		}
			

	
		public List<Sprite> allItemSprites{
			get{
				if (mAllItemSprites.Count == 0) {
					LoadAllItemSprites();
				}
				return mAllItemSprites;
			}

		}

		private void LoadAllItemSprites(){
			if (mAllItemSprites.Count > 0) {
				return;
			}
			Sprite[] spriteCache = MyResourceManager.Instance.LoadAssets<Sprite> (CommonData.allItemSpritesBundleName);
			for (int i = 0; i < spriteCache.Length; i++) {
				mAllItemSprites.Add (spriteCache[i]);
			}
		}


		public List<Sprite> allMapSprites{
			get{
				if (mAllMapSprites.Count == 0) {
					Sprite[] spriteCache = MyResourceManager.Instance.LoadAssets<Sprite> (CommonData.allMapSpritesBundleName);
					for (int i = 0; i < spriteCache.Length; i++) {
						mAllMapSprites.Add (spriteCache[i]);
					}
				}
				return mAllMapSprites;
			}
		}


		public List<Skill> allSkills{
			get{
				if(mAllSkills.Count == 0){
					LoadAllSkills ();
				}
				return mAllSkills;
			}
		}

		private void LoadAllSkills(){

			if (mAllSkills.Count > 0) {
				return;
			}

			GameObject[] skillCache = MyResourceManager.Instance.LoadAssets<GameObject> (CommonData.allSkillsBundleName);

			Transform skillsContainer = TransformManager.FindOrCreateTransform ("AllSkills");

			for (int i = 0; i < skillCache.Length; i++) {
				GameObject skill = GameObject.Instantiate (skillCache [i]);
				skill.name = skillCache [i].name;
				skill.transform.SetParent (skillsContainer);
				mAllSkills.Add(skill.GetComponent<Skill>());
			}

			SortSkillsById (mAllSkills);
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
			

		public List<Sprite> allSkillSprites{
			get{
				if (mAllSkillSprites.Count == 0) {
					Sprite[] spriteCache = MyResourceManager.Instance.LoadAssets<Sprite> (CommonData.allSkillSpritesBundleName);
					for (int i = 0; i < spriteCache.Length; i++) {
						mAllSkillSprites.Add (spriteCache[i]);
					}
				}
				return mAllSkillSprites;
			}
		}


	

		public List<NPC> allNpcs{
			get{
				if (mAllNpcs.Count == 0) {
					LoadNPCs ();
				}
				return mAllNpcs;
			}
		}


		private void LoadNPCs(){

			if (mAllNpcs.Count > 0) {
				return;
			}

			string npcDataDirectory = CommonData.npcsDataFilePath;

			DirectoryInfo npcDirectoryInfo = new DirectoryInfo (npcDataDirectory);

			FileInfo[] npcFiles = npcDirectoryInfo.GetFiles ();

			for (int i = 0; i <npcFiles.Length ; i++) {
				FileInfo npcData = npcFiles [i];
				if (npcData.Extension != ".json") {
					continue;
				}
				NPC npc = null;
				if (npcData.Name.Contains ("Normal")) {
					npc = DataHandler.LoadDataToSingleModelWithPath<NPC> (npcData.FullName);
				}else if(npcData.Name.Contains("Trader")){
					npc = DataHandler.LoadDataToSingleModelWithPath<Trader> (npcData.FullName);
				}
				mAllNpcs.Add (npc);
			}
		}


		public GameObject LoadMonster(string monsterName){

			GameObject[] assets = MyResourceManager.Instance.LoadAssets<GameObject> (CommonData.allMonstersBundleName, monsterName);

			Transform monstersContainer = TransformManager.FindOrCreateTransform ("MonstersContainer");

			GameObject monster = GameObject.Instantiate (assets [0]);

			monster.name = assets [0].name;

			monster.transform.SetParent (monstersContainer);

			return monster;

		}



		public void ReleaseDataWithDataTypes(GameDataType[] dataTypes){

//			for (int i = 0; i < dataTypes.Length; i++) {
//				ReleaseDataWithName (dataTypes [i]);
//			}

			Resources.UnloadUnusedAssets ();

			System.GC.Collect ();

		}

//		private void ReleaseDataWithName(GameDataType type){
//
//			switch (type) {
//			case GameDataType.GameSettings:
//				mGameSettings = null;
//				dataReadyDic [GameDataType.GameSettings] = false;
//				break;
////			case GameDataType.LearnInfo:
////				mLearnInfo = null;
////				dataReadyDic [GameDataType.LearnInfo] = false;
////				break;
//			case GameDataType.GameLevelDatas:
//				mGameLevelDatas.Clear ();
//				dataReadyDic [GameDataType.GameLevelDatas] = false;
//				break;
//			case GameDataType.ItemModels:
//				mAllItemModels.Clear ();
//				dataReadyDic [GameDataType.ItemModels] = false;
//				break;
//			case GameDataType.ItemSprites:
//				mAllItemSprites.Clear ();
//				dataReadyDic [GameDataType.ItemSprites] = false;
//				ResourceManager.Instance.UnloadAssetBunlde (CommonData.allItemSpritesBundleName);
//				break;
////			case GameDataType.EquipmentAttachedProperties:
////				mAllEquipmentAttachedProperties.Clear ();
////				dataReadyDic [GameDataType.EquipmentAttachedProperties] = false;
////				break;
//			case GameDataType.MapSprites:
//				mAllMapSprites.Clear ();
//				dataReadyDic [GameDataType.MapSprites] = false;
//				ResourceManager.Instance.UnloadAssetBunlde (CommonData.allMapSpritesBundleName);
//				break;
//			case GameDataType.Skills:
//				mAllSkills.Clear ();
//				dataReadyDic [GameDataType.Skills] = false;
//				TransformManager.DestroyTransfromWithName("AllSkills",TransformRoot.InstanceContainer);
//				ResourceManager.Instance.UnloadAssetBunlde (CommonData.allSkillsBundleName);
//				break;
//			case GameDataType.SkillSprites:
//				mAllSkillSprites.Clear ();
//				dataReadyDic [GameDataType.SkillSprites] = false;
//				ResourceManager.Instance.UnloadAssetBunlde (CommonData.allSkillSpritesBundleName);
//				break;
//			case GameDataType.Monsters:
//				mAllMonsters.Clear ();
//				dataReadyDic [GameDataType.Monsters] = false;
//				TransformManager.DestroyTransfromWithName("AllMonsters",TransformRoot.InstanceContainer);
//				ResourceManager.Instance.UnloadAssetBunlde (CommonData.allMonstersBundleName);
//				break;
//			case GameDataType.NPCs:
//				mAllNpcs.Clear ();
//				dataReadyDic [GameDataType.NPCs] = false;
//				break;
////			case GameDataType.AnimatorControllers:
////				allAnimatorControllers.Clear ();
////				dataReadyDic [GameDataType.AnimatorControllers] = false;
////				ResourceManager.Instance.UnloadAssetBunlde ("animator/runtimecontrollers");
////				break;
////			case GameDataType.UIAudio:
////				mAllUIAudioClips.Clear ();
////				ResourceManager.Instance.UnloadAssetBunlde (CommonData.allUIAudioClipsBundleName);
////				break;
////			case GameDataType.FootStepAudio:
////				mAllFootStepAudioClips.Clear ();
////				ResourceManager.Instance.UnloadAssetBunlde (CommonData.allFootStepAudioClipBundleName);
////				break;
////			case GameDataType.MapEffectsAudio:
////				mAllMapEffectAudioClips.Clear ();
////				ResourceManager.Instance.UnloadAssetBunlde (CommonData.allMapEffectAudoClipBundleName);
////				break;
////			case GameDataType.SkillEffectsAudio:
////				mAllSkillEffectAudioClips.Clear ();
////				ResourceManager.Instance.UnloadAssetBunlde (CommonData.allSkillEffectAudioClipBundleName);
////				break;
//			default:
//				Debug.LogErrorFormat ("{0} is not data managed by data center", type);
//				break;
//			}
//
//		}


	}
}
