using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void CallBack ();

//public interface GameFlow{
//
//
//	void SetUpView();
//
//	void LoadAssets ();
//
// 	void DestroyInstances();
//
//
//
//}


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
	Inscription
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
	SpecificEnemy,
	AllFriends,
	AllEnemies,
	BothSides,
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



