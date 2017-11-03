using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney{


	public delegate void CallBack ();
	public delegate void CallBack<T>(T[] parameters);

	public delegate void ExploreEventHandler (Transform colliderTrans);


	public struct CommonData{

		public static string originDataPath = Application.streamingAssetsPath + "/Data";
		public static string persistDataPath = Application.persistentDataPath + "/Data";

		public static string effectsDataFilePath = persistDataPath + "/TestEffectString.txt";
		public static string chaptersDataFilePath = persistDataPath + "/ChaptersJson.txt";
		public static string chapterDataFilePath = persistDataPath + "/ChapterJson.txt";
		public static string itemsDataFilePath = persistDataPath + "/Items.json";
		public static string materialsDataFilePath = persistDataPath + "/Materials.json";
		public static string npcsDataFilePath = persistDataPath + "/AllNpcsJson.txt";


		public static string mapDataFilePath = persistDataPath + "/NewMapJson.json";
//		public static string mapTilesDataFilePath = persistDataPath + "/NewTileJson.json";

//		public static string settingsFilePath = persistDataPath + "/Settings.json";
//		public static string learningInfoFilePath = persistDataPath + "/LearningInfo.json";

		public static string dataBaseName = "MyGameDB.db";

		public static string allWordTable = "AllWordsData";
		public static string CET4Table = "CET4";
		public static string CET6Table = "CET6";
		public static string BussinessEnglishTable = "Bussiness";
		public static string DailyEnglishTable = "Daily";

		public static string instanceContainerName = "InstanceContainer";
		public static string poolContainerName = "PoolsContainer";


//		public static string gameSettingsBundleName = "gameSettings";
//		public static string learnInfoBundleName = "learnInfo";
//		public static string allMaterialsBundleName = "allMaterials";
//		public static string allItemModelsBundleName = "AllItemModels";

		public static string mainStaticBundleName = "main/static";
		public static string homeCanvasBundleName = "home/canvas";
		public static string recordCanvasBundleName = "record/canvas";
		public static string materialDisplayCanvasBundleName = "material/canvas";
		public static string itemDisplayCanvasBundleName = "item/canvas";
		public static string skillCanvasBundleName = "skills/canvas";
		public static string bagCanvasBundleName = "bag/canvas";
		public static string settingCanvasBundleName = "setting/canvas";
		public static string spellCanvasBundleName = "spell/canvas";
		public static string exploreSceneBundleName = "explore/scene";
		public static string produceCanvasBundleName = "produce/canvas";


		public static string allMaterialSpritesBundleName = "material/icons";
		public static string allItemSpritesBundleName = "item/icons";
		public static string allMapSpritesBundleName = "explore/mapicons";
		public static string allSkillsBundleName = "skills/skills";
		public static string allSkillSpritesBundleName = "skills/icons";
		public static string allUISpritesBundleName = "ui/icons";
		public static string allMonstersBundleName = "explore/monsters";
		public static string allExploreAudioClipsBundleName = "audio/explore";
		public static string allUIAudioClipsBundleName = "audio/ui";


		public static int aInASCII = (int)('a');

		// 当前屏幕分辨率和预设屏幕分辨率之间的转换比例
		public static float scalerToPresetResulotion = 1920f / Camera.main.pixelHeight;

		public static int durabilityDecreaseWhenAttack = 1;
		public static int durabilityDecreaseWhenBeAttacked = 1;
		public static int durabilityDecreaseWhenBeMagicAttacked = 2;
		public static int durabilityDecreaseWhenAttackObstacle = 20;
		public static int fixDurability = 5;//每次修复百分比

	}




	public enum TransformRoot{
		InstanceContainer,
		PoolContainer,
		Plain
	}

	public enum ItemQuality{
		C,
		B,
		A,
		S,
		Random
	}


	public enum PropertyType{
		Attack,
		Magic,
		armor,
		MagicResist,
		Crit,
		dodge,
		Health,
		Strength
	}

	public enum EventType{
		Monster,
		NPC,
		Item
	}

	public enum Towards{
		Left,
		Right
	}

	public enum TintTextType{
		Crit,
		Miss,
		None
	}

	public enum ValidActionType{
		All,
		PhysicalExcption,
		MagicException,
		PhysicalOnly,
		MagicOnly,
		None

	}

	public enum SkillEffectTarget{
		Self,
		Enemy,
		None
	}

	public enum EffectType{
		PhysicalHurt,
		MagicHurt,
		DisorderHurt,
		Treat,
		Buff,
		DeBuff,
		Control
	}

	public enum StateType{
		Buff,
		Debuff,
		Control
	}

	public enum TriggerType{
		None,
		PhysicalHit,
		MagicalHit,
		DisorderHit,
		BePhysicalHit,
		BeMagicalHit,
		BeDisorderHit,
		Dodge,
		Debuff

	}

	public enum StartTurn{
		Current,
		Next
	}

	public enum ChoiceTriggerType{
		Plot,
		Fight,
		Magic
	}


	public enum WordType{
		CET4,
		CET6,
		Daily,
		Bussiness
	}

	public enum PressType
	{
		Click,
		LongPress,
		Cancel
	}


	// Using Serializable allows us to embed a class with sub properties in the inspector.
	[System.Serializable]
	public class Count
	{
		public int minimum; 			//Minimum value for our Count class.
		public int maximum; 			//Maximum value for our Count class.


		//Assignment constructor.
		public Count (int min, int max)
		{
			minimum = min;
			maximum = max;
		}
	}

	[System.Serializable]
	public struct MapInfo
	{
		public TileSet[] tilesets;
		public int width;
		public int height;
		public int tilewidth;
		public int tileheight;
		public Layer[] layers;


	}

	[System.Serializable]
	public struct Layer
	{
		public int[] data;
		public int height;
		public int width;
		public int x;
		public int y;
		public string name;

	}

	[System.Serializable]
	public struct TileSet{
		public string image;
		public int[] walkableInfoArray;
	}

//	[System.Serializable]
//	public struct TileInfo
//	{
//		
//		public int[] walkableInfoArray;
//
//	}



//	[System.Serializable]
//	public struct GameArchive{
//		
//		public Player player;
//		public LearningInfo learnInfo;
//		public GameSettings gameSettings;
//		public int unlockedMaxChapterIndex;
//
//		public void SaveGameArchive(){
//
//			this.player = Player.mainPlayer;
//			this.learnInfo = GameManager.Instance.gameDataCenter.learnInfo;
//			this.gameSettings = GameManager.Instance.gameDataCenter.gameSettings;
//			this.unlockedMaxChapterIndex = GameManager.Instance.unlockedMaxChapterIndex;
//
//
//			string gameArchiveString = JsonUtility.ToJson (this);
//
//			string gameArchivePath = Path.Combine (CommonData.persistDataPath, "GameArchive.json");
//
//
//			StreamWriter sw = new StreamWriter (gameArchivePath, false);
//
//			sw.Write (gameArchiveString);
//
//			sw.Dispose ();
//
//		}
//
//		public static GameArchive LoadGameArchive(){
//
//			GameArchive ga;
//
//			string gameArchivePath = Path.Combine (CommonData.persistDataPath, "GameArchive.json");
//
//			if (!File.Exists (gameArchivePath)) {
//
//				ga = new GameArchive ();
//
//				ga.player = Player.mainPlayer;
//
//				ga.gameSettings = new GameSettings ();
//
//				ga.learnInfo = new LearningInfo ();
//
//				ga.unlockedMaxChapterIndex = 0;
//
//			} else {
//
//				StreamReader sr = new StreamReader (gameArchivePath);
//
//				string gameArchiveString = sr.ReadToEnd ();
//
//				ga = JsonUtility.FromJson<GameArchive> (gameArchiveString);
//
//			}
//
//			return ga;
//
//		}
//
//	}
		
}