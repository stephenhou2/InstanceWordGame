using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney{
	
	public delegate void CallBack ();
	public delegate void CallBack<T>(T[] parameters);

	public delegate void ExploreEventHandler (Transform colliderTrans);


	public struct CommonData{

		
		public static string jsonFileDirectoryPath = "Assets/Scripts/JsonData";
	//	public static string effectsFileName = "SkillEffectData.txt";
		public static string effectsDataFileName = "TestEffectString.txt";
		public static string chaptersDataFileName = "ChaptersJson.txt";
		public static string chapterDataFileName = "ChapterJson.txt";
		public static string itemsDataFileName = "ItemsJson.txt";

		public static string instanceContainerName = "InstanceContainer";
		public static string poolContainerName = "PoolContainer";


		public static string homeCanvas = "HomeCanvas";
		public static string exploreListCanvas = "ExploreListCanvas";
		public static string exploreMainCanvas = "ExploreMainCanvas";
		public static string dialogAndItemCanvas = "DialogAndItemCanvas";
		public static string battleCanvas = "BattleCanvas";
		public static string bagCanvas = "BagCanvas";
		public static string skillCanvas = "SkillCanvas";
		public static string settingCanvas = "SettingCanvas";
		public static string spellCanvas = "SpellCanvas";

		public static string dataBaseName = "MyGameDB.db";
		public static string itemsTable = "ItemsTable";

		public static string settingsFileName = "Settings.txt";
		public static string learningInfoFileName = "LearningInfo.txt";

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
		S
	}

	public enum ItemType{
		Weapon,
		Amour,
		Shoes,
		Consumables,
		Task,
		Inscription,
		Map
	}
		



	public enum PropertyType{
		Attack,
		Magic,
		Amour,
		MagicResist,
		Crit,
		Agility,
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