using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;


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

	public int unlockedMaxChapterIndex = 1;

	private List<Item> mAllItems = new List<Item> ();
	public List<Item> allItems{
		get{
			if (mAllItems.Count == 0) {
				LoadAllItems ();
			}
			return mAllItems;
		}

//		set{
//			mAllItems = value;
//		}

	}

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

	private List<Sprite> mAllEffectsSprites = new List<Sprite>();
	public List<Sprite> allEffectsSprites{

		get{
			if (mAllEffectsSprites.Count == 0) {
				ResourceManager.Instance.LoadAssetWithFileName ("battle/effect_icons", () => {
					// 获取所有游戏物品的图片
					for(int i = 0;i<ResourceManager.Instance.sprites.Count;i++){
						mAllEffectsSprites.Add(ResourceManager.Instance.sprites[i]);
					}
				},true);
			}

			return mAllEffectsSprites;
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

	private void LoadAllItems(){

		MySQLiteHelper sql = MySQLiteHelper.Instance;

		sql.CreateDatabase (CommonData.dataBaseName);

		sql.GetConnectionWith (CommonData.dataBaseName);

		IDataReader reader = sql.ReadFullTable (CommonData.itemsTable);

		while(reader.Read()) {

			Item item = new Item ();

			item.itemId = reader.GetInt16 (0);
			item.itemName = reader.GetString (1);
			item.itemDescription = reader.GetString (2);
			item.spriteName = reader.GetString (3);
			item.itemType = (ItemType)reader.GetInt16 (4);
			item.itemNameInEnglish = reader.GetString (5);
			item.attackGain = reader.GetInt32 (6);
//			item.powerGain = reader.GetInt32 (7);
			item.magicGain = reader.GetInt32 (8);
			item.critGain = reader.GetInt32 (9);
			item.amourGain = reader.GetInt32 (10);
			item.magicResistGain = reader.GetInt32 (11);
			item.agilityGain = reader.GetInt32 (12);
			item.healthGain = reader.GetInt32 (13);
			item.strengthGain = reader.GetInt32 (14);

			mAllItems.Add (item);

		}

		for (int i = 0; i < mAllItems.Count; i++) {
			
			Item item = mAllItems [i];

			if (item.itemId != i) {
				throw new System.Exception ("物品id不对" + item.itemName);
			}

		}

		sql.CloseConnection (CommonData.dataBaseName);

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
