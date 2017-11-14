using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{

	public class GameDataCenter {

		private GameSettings mGameSettings;
		public GameSettings gameSettings{

			get{

				if (mGameSettings == null) {
					mGameSettings = GameManager.Instance.persistDataManager.LoadGameSettings ();
					if (mGameSettings == null) {
						mGameSettings = new GameSettings ();
					}
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
					mLearnInfo = GameManager.Instance.persistDataManager.LoadLearnInfo ();
					if (mLearnInfo == null) {
						mLearnInfo = new LearningInfo ();
					}
				}
				mLearnInfo.SetUpWords ();
				return mLearnInfo;
			}
//			set{
//				mLearnInfo = value;
//			}
		}

		private List<GameLevelData> mGameLevelDatas = new List<GameLevelData>();
		public List<GameLevelData> gameLevelDatas{
			get{
				if (mGameLevelDatas.Count == 0) {

					GameLevelData[] gameLevelDatasArray = DataHandler.LoadDataToModelWithPath<GameLevelData> (CommonData.gameLevelDataFilePath);

					for (int i = 0; i < gameLevelDatasArray.Length; i++) {
						gameLevelDatasArray[i].LoadAllData ();
						mGameLevelDatas.Add(gameLevelDatasArray[i]);
					}
				}

				return mGameLevelDatas;
			}

		}


		private List<Material> mAllMaterials = new List<Material> ();
		public List<Material> allMaterials{
			get{
				if (mAllMaterials.Count == 0) {
					Material[] materials = DataHandler.LoadDataToModelWithPath<Material> (CommonData.materialsDataFilePath);
					for (int i = 0; i < materials.Length; i++) {
						mAllMaterials.Add (materials [i]);
					}

				}
				return mAllMaterials;
			}
		}

		private List<Sprite> mAllMaterialSprites = new List<Sprite> ();
		public List<Sprite> allMaterialSprites{
			get{
				if (mAllMaterialSprites.Count == 0) {

					ResourceLoader materialSpritesLoader = ResourceLoader.CreateNewResourceLoader ();

					ResourceManager.Instance.LoadAssetsWithBundlePath<Sprite>(materialSpritesLoader,CommonData.allMaterialSpritesBundleName,()=>{
						for(int i = 0;i<materialSpritesLoader.sprites.Count;i++){
							mAllMaterialSprites.Add(materialSpritesLoader.sprites[i]);
						}
					},true);

				}
				return mAllMaterialSprites;
			}
		}

		private List<EquipmentAttachedProperty> mAllEquipmentAttachedProperties = new List<EquipmentAttachedProperty> ();
		public List<EquipmentAttachedProperty> allEquipmentAttachedProperties{
			get{
				if (mAllEquipmentAttachedProperties.Count == 0) {

					EquipmentAttachedProperty[] attachedPropertiesArray = DataHandler.LoadDataToModelWithPath<EquipmentAttachedProperty> (CommonData.persistDataPath + "/AttachedProperties.json");

					for (int i = 0; i < attachedPropertiesArray.Length; i++) {
						mAllEquipmentAttachedProperties.Add (attachedPropertiesArray [i]);
					}
				}
				return mAllEquipmentAttachedProperties;
			}
		}

		private List<ItemModel> mAllItemModels = new List<ItemModel> ();
		public List<ItemModel> allItemModels{
			get{
				if (mAllItemModels.Count == 0) {
					ItemModel[] itemModels = DataHandler.LoadDataToModelWithPath<ItemModel> (CommonData.itemsDataFilePath);
					for (int i = 0; i < itemModels.Length; i++) {
						mAllItemModels.Add (itemModels [i]);
					}
				}
				return mAllItemModels;
			}

		}

		private List<Sprite> mAllItemSprites = new List<Sprite>();
		public List<Sprite> allItemSprites{

			get{
				if (mAllItemSprites.Count == 0) {


					ResourceLoader itemSpritesLoader = ResourceLoader.CreateNewResourceLoader ();

					ResourceManager.Instance.LoadAssetsWithBundlePath<Sprite> (itemSpritesLoader, CommonData.allItemSpritesBundleName, () => {
						// 获取所有游戏物品的图片
						for(int i = 0;i<itemSpritesLoader.sprites.Count;i++){
							mAllItemSprites.Add(itemSpritesLoader.sprites[i]);
						}
					},true);

				}

				return mAllItemSprites;
			}

		}


		private List<Sprite> mAllMapSprites = new List<Sprite> ();
		public List<Sprite> allMapSprites{
			get{
				if (mAllMapSprites.Count == 0) {

					ResourceLoader mapSpritesLoader = ResourceLoader.CreateNewResourceLoader ();

					ResourceManager.Instance.LoadAssetsWithBundlePath<Sprite> (mapSpritesLoader, CommonData.allMapSpritesBundleName, () => {

						for(int i = 0;i<mapSpritesLoader.sprites.Count;i++){
							mAllMapSprites.Add (mapSpritesLoader.sprites[i]);
						}
					},true);

				}
				return mAllMapSprites;
			}

		}

		private List<Skill> mAllSkills = new List<Skill>();
		public List<Skill> allSkills{

			get{
				if(mAllSkills.Count == 0){

					Transform allSkillsContainer = TransformManager.FindOrCreateTransform (CommonData.instanceContainerName + "/AllSkills");

					ResourceLoader skillsLoader = ResourceLoader.CreateNewResourceLoader ();

					ResourceManager.Instance.LoadAssetsWithBundlePath (skillsLoader, CommonData.allSkillsBundleName, () => {
						for(int i = 0;i<skillsLoader.gos.Count;i++){
							Skill skill = skillsLoader.gos[i].GetComponent<Skill>();
							mAllSkills.Add(skill);
							skill.transform.SetParent(allSkillsContainer);
						}
					},true);

					SortSkillsById (mAllSkills);

				}

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

					ResourceLoader skillSpritesLoader = ResourceLoader.CreateNewResourceLoader ();

					ResourceManager.Instance.LoadAssetsWithBundlePath<Sprite> (skillSpritesLoader, CommonData.allSkillSpritesBundleName, () => {
						// 获取所有游戏物品的图片
						for(int i = 0;i<skillSpritesLoader.sprites.Count;i++){
							mAllSkillSprites.Add(skillSpritesLoader.sprites[i]);
						}
					},true);

				}

				return mAllSkillSprites;
			}
		}

		private List<Sprite> mAllUISprites = new List<Sprite> ();
		public List<Sprite> allUISprites{
			get{
				if (mAllUISprites.Count == 0) {

					ResourceLoader UISpritesLoader = ResourceLoader.CreateNewResourceLoader ();

					ResourceManager.Instance.LoadAssetsWithBundlePath<Sprite>(UISpritesLoader, CommonData.allUISpritesBundleName,()=>{
						for(int i = 0;i<UISpritesLoader.sprites.Count;i++){
							mAllUISprites.Add(UISpritesLoader.sprites[i]);
						}
					},true);

				}
				return mAllUISprites;
			}
		}

		private List<Transform> mAllMonsters = new List<Transform>();
		public List<Transform> allMonsters{
			get{
				if (mAllMonsters.Count == 0) {

					Transform monsterModelsContainer = TransformManager.FindOrCreateTransform(CommonData.instanceContainerName + "/MonsterModelsContainer");

					monsterModelsContainer.position = new Vector3 (0, 0, -100);

					ResourceLoader monstersLoader = ResourceLoader.CreateNewResourceLoader ();

					ResourceManager.Instance.LoadAssetsWithBundlePath (monstersLoader, CommonData.allMonstersBundleName, () => {
						for(int i = 0;i<monstersLoader.gos.Count;i++){
							Transform monster = monstersLoader.gos[i].transform;
							monster.SetParent(monsterModelsContainer,false);
							mAllMonsters.Add(monster);
						};
					}, true);

				}
				return mAllMonsters;
			}
		}
			
		private List<NPC> mAllNpcs = new List<NPC>();
		public List<NPC> allNpcs{
			get{
				if (mAllNpcs.Count == 0) {
					
					NPC[] npcsArray = DataHandler.LoadDataToModelWithPath<NPC> (CommonData.npcsDataFilePath);

					for (int i = 0; i < npcsArray.Length; i++) {
						mAllNpcs.Add (npcsArray [i]);
					}

				}
				return mAllNpcs;
			}

		}

		private List<AudioClip> mAllExploreAudioClips = new List<AudioClip>();
		public List<AudioClip> allExploreAudioClips{
			get{
				if (mAllExploreAudioClips.Count == 0) {
					ResourceLoader exploreAudioLoader = ResourceLoader.CreateNewResourceLoader ();
					ResourceManager.Instance.LoadAssetsWithBundlePath<AudioClip> (exploreAudioLoader, CommonData.allExploreAudioClipsBundleName, () => {
						CopyClips(exploreAudioLoader.audioClips,mAllExploreAudioClips,false);
					}, true);
				}
				return mAllExploreAudioClips;
			}
		}

		private List<AudioClip> mAllUIAudioClips = new List<AudioClip> ();
		public List<AudioClip> allUIClips{
			get{
				if (mAllUIAudioClips.Count == 0) {
					ResourceLoader UIAudioLoader = ResourceLoader.CreateNewResourceLoader ();
					ResourceManager.Instance.LoadAssetsWithBundlePath<AudioClip> (UIAudioLoader, CommonData.allUIAudioClipsBundleName, () => {
						CopyClips(UIAudioLoader.audioClips,mAllUIAudioClips,true);
					}, true);
				}
				return mAllUIAudioClips;
			}
		}

		private void CopyClips(List<AudioClip> originClips,List<AudioClip> targetClips,bool dontUnload){

			for(int i = 0;i<originClips.Count;i++){
				targetClips.Add(originClips[i]);
				if (dontUnload) {
					originClips [i].hideFlags = HideFlags.DontUnloadUnusedAsset;
				}
			}

		}
			

		public void ReleaseDataWithNames(string[] dataNames){

			for (int i = 0; i < dataNames.Length; i++) {
				ReleaseDataWithName (dataNames [i]);
			}

			Resources.UnloadUnusedAssets ();

			System.GC.Collect ();

		}

		private void ReleaseDataWithName(string dataName){

			switch (dataName) {
			case "GameSettings":
				mGameSettings = null;
				break;
			case "LearnInfo":
				mLearnInfo = null;
				break;
			case "AllGameLevelDatas":
				mGameLevelDatas.Clear ();
				break;
			case "AllMaterials":
				mAllMaterials.Clear ();
				break;
			case "AllMaterialSprites":
				mAllMaterialSprites.Clear ();
				ResourceManager.Instance.UnloadCaches (CommonData.allMaterialSpritesBundleName, true);
				break;
			case "AllItemModels":
				mAllItemModels.Clear ();
				break;
			case "AllItemSprites":
				mAllItemSprites.Clear ();
				ResourceManager.Instance.UnloadCaches (CommonData.allItemSpritesBundleName, true);
				break;
			case "AllAttachedProperties":
				mAllEquipmentAttachedProperties.Clear ();
				break;
			case "AllMapSprites":
				mAllMapSprites.Clear ();
				ResourceManager.Instance.UnloadCaches (CommonData.allMapSpritesBundleName, true);
				break;
			case "AllSkills":
				mAllSkills.Clear ();
				TransformManager.DestroyTransfromWithName("AllSkills",TransformRoot.InstanceContainer);
				ResourceManager.Instance.UnloadCaches (CommonData.allSkillsBundleName, true);
				break;
			case "AllSkillSprites":
				mAllSkillSprites.Clear ();
				ResourceManager.Instance.UnloadCaches (CommonData.allSkillSpritesBundleName, true);
				break;
			case "AllMonsters":
				mAllMonsters.Clear ();
				ResourceManager.Instance.UnloadCaches (CommonData.allMonstersBundleName, false);
				break;
			case "AllNpcs":
				mAllNpcs.Clear ();
				break;
			case "AllExploreAudioClips":
				mAllExploreAudioClips.Clear ();
				ResourceManager.Instance.UnloadCaches (CommonData.allExploreAudioClipsBundleName, true);
				break;
			case "AllUIAudioClips":
				mAllUIAudioClips.Clear ();
				ResourceManager.Instance.UnloadCaches (CommonData.allUIAudioClipsBundleName, true);
				break;
			default:
				Debug.LogErrorFormat ("{0} is not data managed by data center", dataName);
				break;
			}

		}


		/// <summary>
		/// 根据玩家已获得的配方解锁对应装备和技能
		/// </summary>
		public void InitItemsAndSkillDataByFormula(){
			
			for (int i = 0; i < Player.mainPlayer.allFormulasInBag.Count; i++) {
				Formula formula = Player.mainPlayer.allFormulasInBag [i];
				switch (formula.formulaType) {
				case FormulaType.Equipment:
					formula.GetItemModelUnlock ();
					break;
				case FormulaType.Skill:
					formula.GetSkillUnlock ();
					break;
				}
			}
		}

	}
}
