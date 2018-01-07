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
			LearnInfo,
			GameLevelDatas,
			Materials,
			MaterialSprites,
			ItemModels,
			ItemSprites,
			MapSprites,
			Skills,
			SkillSprites,
			UISprites,
			Monsters,
			NPCs,
//			AnimatorControllers
//			FootStepAudio,
//			MapEffectsAudio,
//			SkillEffectsAudio,
//			UIAudio
		}

		private GameSettings mGameSettings;
		private LearningInfo mLearnInfo;
		private List<GameLevelData> mGameLevelDatas = new List<GameLevelData>();
		private List<Material> mAllMaterials = new List<Material> ();
		private List<Sprite> mAllMaterialSprites = new List<Sprite> ();
		private List<ItemModel> mAllItemModels = new List<ItemModel> ();
		private List<Sprite> mAllItemSprites = new List<Sprite>();
		private List<Sprite> mAllMapSprites = new List<Sprite> ();
		private List<Skill> mAllSkills = new List<Skill>();
		private List<Sprite> mAllSkillSprites = new List<Sprite>();
		private List<Sprite> mAllUISprites = new List<Sprite> ();
		private List<Transform> mAllMonsters = new List<Transform>();
		private List<NPC> mAllNpcs = new List<NPC>();

//		private List<AudioClip> mAllFootStepAudioClips = new List<AudioClip>();
//		private List<AudioClip> mAllMapEffectAudioClips = new List<AudioClip> ();
//		private List<AudioClip> mAllSkillEffectAudioClips = new List<AudioClip> ();
//		private List<AudioClip> mAllUIAudioClips = new List<AudioClip> ();


		private Dictionary<GameDataType,bool> m_DataReadyDic = new Dictionary<GameDataType, bool> ();

		private Dictionary<GameDataType,bool> dataReadyDic{
			get{
				if (m_DataReadyDic.Count == 0) {
					for (int i = 0; i < 19; i++) {
						GameDataType type = (GameDataType)(i);
						m_DataReadyDic.Add (type, false);
					}
				}
				return m_DataReadyDic;
			}
		}
			

		private List<GameDataType> inLoadingDataTypes = new List<GameDataType> ();

		public bool CheckDatasReady(GameDataType[] types){

			bool ready = true;

			for (int i = 0; i < types.Length; i++) {
				GameDataType type = types [i];
				if (!dataReadyDic [type]) {
					ready = false;
					if (!inLoadingDataTypes.Contains (type)) {
						InitData (type);
						Debug.LogFormat ("load {0}", type);
					}

					break;
				}
			}

//			foreach (KeyValuePair<GameDataType,bool> kvp in dataReadyDic) {
//				Debug.Log(string.Format("{0}---{1}",(GameDataType)kvp.Key, (bool)kvp.Value));
//			}
//
//			Debug.Log (ready);

			return ready;

		}


		private void InitData(GameDataType type){
			switch (type) {
			case GameDataType.GameSettings:
				LoadGameSettings ();
				break;
			case GameDataType.LearnInfo:
				LoadLearnInfo ();
				break;
			case GameDataType.GameLevelDatas:
				LoadGameLevelDatas ();
				break;
			case GameDataType.Materials:
				LoadMaterials ();
				break;
			case GameDataType.MaterialSprites:
				LoadMaterialSprites ();
				break;
			case GameDataType.ItemModels:
				LoadItemModels ();
				break;
			case GameDataType.ItemSprites:
				LoadItemSprites ();
				break;
			case GameDataType.MapSprites:
				LoadMapSprites ();
				break;
			case GameDataType.Skills:
				LoadSkills ();
				break;
			case GameDataType.SkillSprites:
				LoadSkillSprites ();
				break;
			case GameDataType.UISprites:
				LoadUISprites ();
				break;
			case GameDataType.Monsters:
				LoadMonsters ();
				break;
			case GameDataType.NPCs:
				LoadNPCs ();
				break;
//			case GameDataType.AnimatorControllers:
//				LoadAllAnimatorControllers ();
//				break;
//			case GameDataType.UIAudio:
//				LoadUIAudioClips ();
//				break;
//			case GameDataType.FootStepAudio:
//				LoadFootStepAudioClips ();
//				break;
//			case GameDataType.MapEffectsAudio:
//				LoadMapEffectAudioClips();
//				break;
//			case GameDataType.SkillEffectsAudio:
//				LoadSkillEffectAudioClips();
//				break;
			}
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

			if(inLoadingDataTypes.Contains(GameDataType.GameSettings)){
				return;
			}

			inLoadingDataTypes.Add (GameDataType.GameSettings);
			mGameSettings = GameManager.Instance.persistDataManager.LoadGameSettings ();
			if (mGameSettings == null) {
				mGameSettings = new GameSettings ();
			}
			dataReadyDic [GameDataType.GameSettings] = true;
			inLoadingDataTypes.Remove (GameDataType.GameSettings);
		}




		public LearningInfo learnInfo{
			get{
				if (mLearnInfo == null) {
					LoadLearnInfo ();
				}
				return mLearnInfo;
			}
		}

		private void LoadLearnInfo(){
			if(inLoadingDataTypes.Contains(GameDataType.LearnInfo)){
				return;
			}
			inLoadingDataTypes.Add (GameDataType.LearnInfo);
			mLearnInfo = GameManager.Instance.persistDataManager.LoadLearnInfo ();
			if (mLearnInfo == null) {
				mLearnInfo = new LearningInfo ();
			}
			dataReadyDic [GameDataType.LearnInfo] = true;
			inLoadingDataTypes.Remove (GameDataType.LearnInfo);
		}


		public List<GameLevelData> gameLevelDatas{
			get{
				if (mGameLevelDatas.Count == 0) {
					LoadGameLevelDatas ();
				}
				return mGameLevelDatas;
			}

		}

		private void LoadGameLevelDatas(){
			if(inLoadingDataTypes.Contains(GameDataType.GameLevelDatas)){
				return;
			}
			inLoadingDataTypes.Add (GameDataType.GameLevelDatas);
			GameLevelData[] gameLevelDatasArray = DataHandler.LoadDataToModelsWithPath<GameLevelData> (CommonData.gameLevelDataFilePath);

			for (int i = 0; i < gameLevelDatasArray.Length; i++) {
				//						gameLevelDatasArray[i].LoadAllData ();
				mGameLevelDatas.Add(gameLevelDatasArray[i]);
			}
			dataReadyDic [GameDataType.GameLevelDatas] = true;
			inLoadingDataTypes.Remove (GameDataType.GameLevelDatas);
		}


		public List<Material> allMaterials{
			get{
				if (mAllMaterials.Count == 0) {
					LoadMaterials ();
				}
				return mAllMaterials;
			}
		}

		private void LoadMaterials(){
			if(inLoadingDataTypes.Contains(GameDataType.Materials)){
				return;
			}
			inLoadingDataTypes.Add (GameDataType.Materials);
			Material[] materials = DataHandler.LoadDataToModelsWithPath<Material> (CommonData.materialsDataFilePath);
			for (int i = 0; i < materials.Length; i++) {
				mAllMaterials.Add (materials [i]);
			}
			dataReadyDic [GameDataType.Materials] = true;
			inLoadingDataTypes.Remove (GameDataType.Materials);
		}


		public List<Sprite> allMaterialSprites{
			get{
				if (mAllMaterialSprites.Count == 0) {
					LoadMaterialSprites ();
				}
				return mAllMaterialSprites;
			}
		}

		private void LoadMaterialSprites(){
			if(inLoadingDataTypes.Contains(GameDataType.MaterialSprites)){
				return;
			}
			inLoadingDataTypes.Add (GameDataType.MaterialSprites);

			ResourceLoader materialSpritesLoader = ResourceLoader.CreateNewResourceLoader<Sprite> (CommonData.allMaterialSpritesBundleName);

			ResourceManager.Instance.LoadAssetsUsingWWW(materialSpritesLoader,()=>{

				for(int i = 0;i<materialSpritesLoader.assets.Length;i++){
					Sprite s = materialSpritesLoader.assets[i] as Sprite;
					mAllMaterialSprites.Add(s);
				}
				dataReadyDic [GameDataType.MaterialSprites] = true;
				inLoadingDataTypes.Remove(GameDataType.MaterialSprites);
			});

		}


//		public List<EquipmentAttachedProperty> allEquipmentAttachedProperties{
//			get{
//				if (mAllEquipmentAttachedProperties.Count == 0) {
//					LoadEquipmentAttachedProperties ();
//				}
//				return mAllEquipmentAttachedProperties;
//			}
//		}

//		private void LoadEquipmentAttachedProperties(){
//			if(inLoadingDataTypes.Contains(GameDataType.EquipmentAttachedProperties)){
//				return;
//			}
//			inLoadingDataTypes.Add (GameDataType.EquipmentAttachedProperties);
////			EquipmentAttachedProperty[] attachedPropertiesArray = DataHandler.LoadDataToModelsWithPath<EquipmentAttachedProperty> (CommonData.persistDataPath + "/AttachedProperties.json");
//
////			for (int i = 0; i < attachedPropertiesArray.Length; i++) {
////				mAllEquipmentAttachedProperties.Add (attachedPropertiesArray [i]);
////			}
//			dataReadyDic [GameDataType.EquipmentAttachedProperties] = true;
//			inLoadingDataTypes.Remove (GameDataType.EquipmentAttachedProperties);
//		}


		public List<ItemModel> allItemModels{
			get{
				if (mAllItemModels.Count == 0) {
					LoadItemModels ();
				}
				return mAllItemModels;
			}

		}

		private void LoadItemModels(){
			if(inLoadingDataTypes.Contains(GameDataType.ItemModels)){
				return;
			}
			inLoadingDataTypes.Add (GameDataType.ItemModels);
			ItemModel[] itemModels = DataHandler.LoadDataToModelsWithPath<ItemModel> (CommonData.itemsDataFilePath);
			for (int i = 0; i < itemModels.Length; i++) {
				mAllItemModels.Add (itemModels [i]);
			}
			dataReadyDic [GameDataType.ItemModels] = true;
			inLoadingDataTypes.Remove (GameDataType.ItemModels);
		}

	
		public List<Sprite> allItemSprites{

			get{
				if (mAllItemSprites.Count == 0) {
					LoadItemSprites ();
				}
				return mAllItemSprites;
			}

		}

		private void LoadItemSprites(){
			if(inLoadingDataTypes.Contains(GameDataType.ItemSprites)){
				return;
			}
			inLoadingDataTypes.Add (GameDataType.ItemSprites);
			ResourceLoader itemSpritesLoader = ResourceLoader.CreateNewResourceLoader <Sprite>( CommonData.allItemSpritesBundleName);

			ResourceManager.Instance.LoadAssetsUsingWWW (itemSpritesLoader, () => {
				// 获取所有游戏物品的图片
				for(int i = 0;i<itemSpritesLoader.assets.Length;i++){
					Sprite s = itemSpritesLoader.assets[i] as Sprite;
					mAllItemSprites.Add(s);
				}
				dataReadyDic [GameDataType.ItemSprites] = true;
				inLoadingDataTypes.Remove(GameDataType.ItemSprites);
			});

		}


		public List<Sprite> allMapSprites{
			get{
				if (mAllMapSprites.Count == 0) {
					LoadMapSprites ();
				}
				return mAllMapSprites;
			}
		}

		private void LoadMapSprites(){
			if(inLoadingDataTypes.Contains(GameDataType.MapSprites)){
				return;
			}
			inLoadingDataTypes.Add (GameDataType.MapSprites);
			ResourceLoader mapSpritesLoader = ResourceLoader.CreateNewResourceLoader<Sprite> ( CommonData.allMapSpritesBundleName);

			ResourceManager.Instance.LoadAssetsUsingWWW (mapSpritesLoader, () => {

				for(int i = 0;i<mapSpritesLoader.assets.Length;i++){
					Sprite s = mapSpritesLoader.assets[i] as Sprite;
					mAllMapSprites.Add (s);
				}

				dataReadyDic [GameDataType.MapSprites] = true;
				inLoadingDataTypes.Remove(GameDataType.MapSprites);
			});
		}


		public List<Skill> allSkills{
			get{
				if(mAllSkills.Count == 0){
					LoadSkills ();
				}
				return mAllSkills;
			}
		}

		private void LoadSkills(){
			if(inLoadingDataTypes.Contains(GameDataType.Skills)){
				return;
			}
			inLoadingDataTypes.Add (GameDataType.Skills);
			Transform allSkillsContainer = TransformManager.FindOrCreateTransform (CommonData.instanceContainerName + "/AllSkills");

			ResourceLoader skillsLoader = ResourceLoader.CreateNewResourceLoader <GameObject>(CommonData.allSkillsBundleName);

			ResourceManager.Instance.LoadAssetsUsingWWW (skillsLoader, () => {

				for(int i = 0;i<skillsLoader.assets.Length;i++){

					Object asset = skillsLoader.assets[i];

					GameObject skillGo = skillsLoader.InstantiateAsset(asset);

					Skill skill = skillGo.GetComponent<Skill>();
					mAllSkills.Add(skill);
					skill.transform.SetParent(allSkillsContainer);
				}

				SortSkillsById (mAllSkills);
				dataReadyDic [GameDataType.Skills] = true;
				inLoadingDataTypes.Remove(GameDataType.Skills);
			});
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
					LoadSkillSprites ();
				}
				return mAllSkillSprites;
			}
		}

		private void LoadSkillSprites(){
			if(inLoadingDataTypes.Contains(GameDataType.SkillSprites)){
				return;
			}
			inLoadingDataTypes.Add (GameDataType.SkillSprites);
			ResourceLoader skillSpritesLoader = ResourceLoader.CreateNewResourceLoader<Sprite> ( CommonData.allSkillSpritesBundleName);

			ResourceManager.Instance.LoadAssetsUsingWWW (skillSpritesLoader, () => {
				// 获取所有游戏物品的图片
				for(int i = 0;i<skillSpritesLoader.assets.Length;i++){
					Sprite s = skillSpritesLoader.assets[i] as Sprite;
					mAllSkillSprites.Add(s);
				}
				dataReadyDic [GameDataType.SkillSprites] = true;
				inLoadingDataTypes.Remove(GameDataType.SkillSprites);
			});
		}


		public List<Sprite> allUISprites{
			get{
				if (mAllUISprites.Count == 0) {
					LoadUISprites ();
				}
				return mAllUISprites;
			}
		}


		private void LoadUISprites(){
			if(inLoadingDataTypes.Contains(GameDataType.UISprites)){
				return;
			}
			inLoadingDataTypes.Add (GameDataType.UISprites);
			ResourceLoader UISpritesLoader = ResourceLoader.CreateNewResourceLoader<Sprite> ( CommonData.allUISpritesBundleName);

			ResourceManager.Instance.LoadAssetsUsingWWW(UISpritesLoader,()=>{
				for(int i = 0;i<UISpritesLoader.assets.Length;i++){
					Sprite s = UISpritesLoader.assets[i] as Sprite;
					mAllUISprites.Add(s);
				}
				dataReadyDic [GameDataType.UISprites] = true;
				inLoadingDataTypes.Remove(GameDataType.UISprites);
			});
		}


		public List<Transform> allMonsters{
			get{
				if (mAllMonsters.Count == 0) {
					LoadMonsters ();
				}
				return mAllMonsters;
			}
		}

		private void LoadMonsters(){
			if(inLoadingDataTypes.Contains(GameDataType.Monsters)){
				return;
			}
			inLoadingDataTypes.Add (GameDataType.Monsters);
			Transform monsterModelsContainer = TransformManager.FindOrCreateTransform(CommonData.instanceContainerName + "/MonsterModelsContainer");

			monsterModelsContainer.position = new Vector3 (0, 0, -100);

			ResourceLoader monstersLoader = ResourceLoader.CreateNewResourceLoader<GameObject> (CommonData.allMonstersBundleName);

			ResourceManager.Instance.LoadAssetsUsingWWW (monstersLoader, () => {

				for(int i = 0;i<monstersLoader.assets.Length;i++){

					Object asset = monstersLoader.assets[i];

					GameObject monster = monstersLoader.InstantiateAsset(asset);

					monster.transform.SetParent(monsterModelsContainer,false);

					mAllMonsters.Add(monster.transform);
				};
				dataReadyDic [GameDataType.Monsters] = true;
				inLoadingDataTypes.Remove(GameDataType.Monsters);
			});
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
			if(inLoadingDataTypes.Contains(GameDataType.NPCs)){
				return;
			}
			inLoadingDataTypes.Add (GameDataType.NPCs);
			string npcDataDirectory = string.Format ("{0}/NPCs", CommonData.persistDataPath);

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
			dataReadyDic [GameDataType.NPCs] = true;
			inLoadingDataTypes.Remove (GameDataType.NPCs);
		}

//		private List<RuntimeAnimatorController> m_AllAnimatorControllers = new List<RuntimeAnimatorController>();
//		public List<RuntimeAnimatorController> allAnimatorControllers{
//			get{
//				if (m_AllAnimatorControllers.Count == 0) {
//					LoadAllAnimatorControllers ();
//				}
//				return m_AllAnimatorControllers;
//			}
//		}

//		private void LoadAllAnimatorControllers(){
//			if(inLoadingDataTypes.Contains(GameDataType.AnimatorControllers)){
//				return;
//			}
//			inLoadingDataTypes.Add (GameDataType.AnimatorControllers);
//
//			ResourceLoader animatorControllerLoader = ResourceLoader.CreateNewResourceLoader<RuntimeAnimatorController> ("animator/runtimecontrollers");
//
//
//			#warning 现在只有animator controller使用assetbundle同步加载，后面再研究一下，animator controller到底怎么加载
//			ResourceManager.Instance.LoadAssetsFromFileSync (animatorControllerLoader, () => {
//				for(int i = 0;i<animatorControllerLoader.assets.Length;i++){
//
//					RuntimeAnimatorController animController = (RuntimeAnimatorController)animatorControllerLoader.assets[i];
//
//					m_AllAnimatorControllers.Add(animController);
//				};
//				dataReadyDic [GameDataType.AnimatorControllers] = true;
//				inLoadingDataTypes.Remove(GameDataType.AnimatorControllers);
//			});
//
////			ResourceManager.Instance.LoadAssetsUsingWWW (animatorControllerLoader, () => {
////
////				for(int i = 0;i<animatorControllerLoader.assets.Length;i++){
////
////					RuntimeAnimatorController animController = (RuntimeAnimatorController)animatorControllerLoader.assets[i];
////
////					m_AllAnimatorControllers.Add(animController);
////				};
////				dataReadyDic [GameDataType.AnimatorControllers] = true;
////				inLoadingDataTypes.Remove(GameDataType.AnimatorControllers);
////			});
//		}

//		public List<AudioClip> allFootStepAudioClips{
//			get{
//				if (mAllFootStepAudioClips.Count == 0) {
//					LoadFootStepAudioClips ();
//				}
//				return mAllFootStepAudioClips;
//			}
//		}
//
//		private void LoadFootStepAudioClips(){
//
//			if (inLoadingDataTypes.Contains (GameDataType.FootStepAudio)) {
//				return;
//			}
//
//			inLoadingDataTypes.Add (GameDataType.FootStepAudio);
//
//			ResourceLoader footStepAudioLoader = ResourceLoader.CreateNewResourceLoader<AudioClip> (CommonData.allFootStepAudioClipBundleName);
//
//			ResourceManager.Instance.LoadAssetsWithLoader (footStepAudioLoader, () => {
//				for(int i = 0;i<footStepAudioLoader.assets.Length;i++){
//
//					AudioClip clip = footStepAudioLoader.assets[i] as AudioClip;
//
//					CopyClips (clip, mAllSkillEffectAudioClips, false);
//
//				}
//
//				dataReadyDic[GameDataType.FootStepAudio] = true;
//				inLoadingDataTypes.Remove(GameDataType.FootStepAudio);
//			});
//		}
//
//
//		public List<AudioClip> allMapEffectAudioClips{
//			get{
//				if (mAllMapEffectAudioClips.Count == 0) {
//					LoadMapEffectAudioClips ();
//				}
//				return mAllMapEffectAudioClips;
//			}
//		}
//
//		private void LoadMapEffectAudioClips(){
//			if (inLoadingDataTypes.Contains (GameDataType.MapEffectsAudio)) {
//				return;
//			}
//			inLoadingDataTypes.Add (GameDataType.MapEffectsAudio);
//
//			ResourceLoader mapEffectAudioLoader = ResourceLoader.CreateNewResourceLoader<AudioClip> (CommonData.allMapEffectAudoClipBundleName);
//
//			ResourceManager.Instance.LoadAssetsWithLoader (mapEffectAudioLoader, () => {
//
//				for(int i = 0;i<mapEffectAudioLoader.assets.Length;i++){
//
//					AudioClip clip = mapEffectAudioLoader.assets[i] as AudioClip;
//
//					CopyClips (clip, mAllSkillEffectAudioClips, false);
//
//				}
//
//				dataReadyDic[GameDataType.MapEffectsAudio] = true;
//				inLoadingDataTypes.Remove(GameDataType.MapEffectsAudio);
//			});
//		}
//
//
//		public List<AudioClip> allSkillEffectAudioClips{
//			get{
//				if (mAllSkillEffectAudioClips.Count == 0) {
//					LoadSkillEffectAudioClips ();
//				}
//				return mAllSkillEffectAudioClips;
//			}
//		}
//
//		private void LoadSkillEffectAudioClips(){
//			if(inLoadingDataTypes.Contains(GameDataType.SkillEffectsAudio)){
//				return;
//			}
//			inLoadingDataTypes.Add (GameDataType.SkillEffectsAudio);
//			ResourceLoader skillEffectAudioLoader = ResourceLoader.CreateNewResourceLoader<AudioClip> (CommonData.allSkillEffectAudioClipBundleName);
//
//			ResourceManager.Instance.LoadAssetsWithLoader (skillEffectAudioLoader, () => {
//
//				for(int i = 0;i<skillEffectAudioLoader.assets.Length;i++){
//
//					AudioClip clip = skillEffectAudioLoader.assets[i] as AudioClip;
//
//					CopyClips (clip, mAllSkillEffectAudioClips, false);
//
//				}
//				dataReadyDic[GameDataType.SkillEffectsAudio] = true;
//				inLoadingDataTypes.Remove(GameDataType.SkillEffectsAudio);
//			});
//		}
//
//		public List<AudioClip> allUIClips{
//			get{
//				if (mAllUIAudioClips.Count == 0) {
//					
//				}
//				return mAllUIAudioClips;
//			}
//		}
//
//
//		private void LoadUIAudioClips(){
//			if (inLoadingDataTypes.Contains (GameDataType.UIAudio)) {
//				return;
//			}
//			inLoadingDataTypes.Add (GameDataType.UIAudio);
//			ResourceLoader UIAudioLoader = ResourceLoader.CreateNewResourceLoader<AudioClip> (CommonData.allUIAudioClipsBundleName);
//			ResourceManager.Instance.LoadAssetsWithLoader (UIAudioLoader, () => {
//
//				for(int i = 0;i<UIAudioLoader.assets.Length;i++){
//
//					AudioClip clip = UIAudioLoader.assets[i] as AudioClip;
//
//					CopyClips(clip,mAllUIAudioClips,true);
//
//				}
//
//				dataReadyDic[GameDataType.UIAudio] = true;
//
//				inLoadingDataTypes.Remove(GameDataType.UIAudio);
//
//			});
//		}
//
//		private void CopyClips(AudioClip originClip,List<AudioClip> targetClips,bool dontUnload){
//			targetClips.Add(originClip);
//			if (dontUnload) {
//				originClip.hideFlags = HideFlags.DontUnloadUnusedAsset;
//			}
//
//		}

			

		public void ReleaseDataWithDataTypes(GameDataType[] dataTypes){

			for (int i = 0; i < dataTypes.Length; i++) {
				ReleaseDataWithName (dataTypes [i]);
			}

			Resources.UnloadUnusedAssets ();

			System.GC.Collect ();

		}

		private void ReleaseDataWithName(GameDataType type){

			switch (type) {
			case GameDataType.GameSettings:
				mGameSettings = null;
				dataReadyDic [GameDataType.GameSettings] = false;
				break;
			case GameDataType.LearnInfo:
				mLearnInfo = null;
				dataReadyDic [GameDataType.LearnInfo] = false;
				break;
			case GameDataType.GameLevelDatas:
				mGameLevelDatas.Clear ();
				dataReadyDic [GameDataType.GameLevelDatas] = false;
				break;
			case GameDataType.Materials:
				mAllMaterials.Clear ();
				dataReadyDic [GameDataType.Materials] = false;
				break;
			case GameDataType.MaterialSprites:
				mAllMaterialSprites.Clear ();
				dataReadyDic [GameDataType.MaterialSprites] = false;
				ResourceManager.Instance.UnloadAssetBunlde (CommonData.allMaterialSpritesBundleName);
				break;
			case GameDataType.ItemModels:
				mAllItemModels.Clear ();
				dataReadyDic [GameDataType.ItemModels] = false;
				break;
			case GameDataType.ItemSprites:
				mAllItemSprites.Clear ();
				dataReadyDic [GameDataType.ItemSprites] = false;
				ResourceManager.Instance.UnloadAssetBunlde (CommonData.allItemSpritesBundleName);
				break;
//			case GameDataType.EquipmentAttachedProperties:
//				mAllEquipmentAttachedProperties.Clear ();
//				dataReadyDic [GameDataType.EquipmentAttachedProperties] = false;
//				break;
			case GameDataType.MapSprites:
				mAllMapSprites.Clear ();
				dataReadyDic [GameDataType.MapSprites] = false;
				ResourceManager.Instance.UnloadAssetBunlde (CommonData.allMapSpritesBundleName);
				break;
			case GameDataType.Skills:
				mAllSkills.Clear ();
				dataReadyDic [GameDataType.Skills] = false;
				TransformManager.DestroyTransfromWithName("AllSkills",TransformRoot.InstanceContainer);
				ResourceManager.Instance.UnloadAssetBunlde (CommonData.allSkillsBundleName);
				break;
			case GameDataType.SkillSprites:
				mAllSkillSprites.Clear ();
				dataReadyDic [GameDataType.SkillSprites] = false;
				ResourceManager.Instance.UnloadAssetBunlde (CommonData.allSkillSpritesBundleName);
				break;
			case GameDataType.Monsters:
				mAllMonsters.Clear ();
				dataReadyDic [GameDataType.Monsters] = false;
				ResourceManager.Instance.UnloadAssetBunlde (CommonData.allMonstersBundleName);
				break;
			case GameDataType.NPCs:
				mAllNpcs.Clear ();
				dataReadyDic [GameDataType.NPCs] = false;
				break;
//			case GameDataType.AnimatorControllers:
//				allAnimatorControllers.Clear ();
//				dataReadyDic [GameDataType.AnimatorControllers] = false;
//				ResourceManager.Instance.UnloadAssetBunlde ("animator/runtimecontrollers");
//				break;
//			case GameDataType.UIAudio:
//				mAllUIAudioClips.Clear ();
//				ResourceManager.Instance.UnloadAssetBunlde (CommonData.allUIAudioClipsBundleName);
//				break;
//			case GameDataType.FootStepAudio:
//				mAllFootStepAudioClips.Clear ();
//				ResourceManager.Instance.UnloadAssetBunlde (CommonData.allFootStepAudioClipBundleName);
//				break;
//			case GameDataType.MapEffectsAudio:
//				mAllMapEffectAudioClips.Clear ();
//				ResourceManager.Instance.UnloadAssetBunlde (CommonData.allMapEffectAudoClipBundleName);
//				break;
//			case GameDataType.SkillEffectsAudio:
//				mAllSkillEffectAudioClips.Clear ();
//				ResourceManager.Instance.UnloadAssetBunlde (CommonData.allSkillEffectAudioClipBundleName);
//				break;
			default:
				Debug.LogErrorFormat ("{0} is not data managed by data center", type);
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
