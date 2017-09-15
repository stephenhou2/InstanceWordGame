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
		public static string itemsDataFilePath = persistDataPath + "/AllItemsJson.txt";
		public static string npcsDataFilePath = persistDataPath + "/AllNpcsJson.txt";


		public static string mapDataFilePath = persistDataPath + "/MapJson.json";
		public static string mapTilesDataFilePath = persistDataPath + "/TileJson.json";

		public static string settingsFilePath = persistDataPath + "/Settings.txt";
		public static string learningInfoFilePath = persistDataPath + "/LearningInfo.txt";

		public static string dataBaseName = "MyGameDB.db";

		public static string allWordTable = "AllWordsData";
		public static string CET4Table = "CET4";
		public static string CET6Table = "CET6";
		public static string BussinessEnglishTable = "Bussiness";
		public static string DailyEnglishTable = "Daily";




		public static string instanceContainerName = "InstanceContainer";
		public static string poolContainerName = "PoolContainer";


		public static string homeCanvas = "HomeCanvas";
		public static string bagCanvas = "BagCanvas";
		public static string skillCanvas = "SkillCanvas";
		public static string settingCanvas = "SettingCanvas";
		public static string spellCanvas = "SpellCanvas";



		public static int aInASCII = (int)('a');

	}

	public enum SpellPurpose{
		Create,
		Strengthen,
		Task
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

	public enum ItemType{
		Equipment,
		Consumables,
		Task,
		Inscription,
		Map
	}
		



	public enum PropertyType{
		Attack,
		Magic,
		armour,
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
	public class MapInfo
	{
		public int width;
		public int height;
		public int tilewidth;
		public int tileheight;
		public Layer[] layers;



	}

	[System.Serializable]
	public class Layer
	{
		public int[] data;
		public int height;
		public int width;
		public int x;
		public int y;

	}

	[System.Serializable]
	public class TileInfo
	{
		public int[] walkableInfoArray;

	}
		
}