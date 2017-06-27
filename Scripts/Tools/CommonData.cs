using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void CallBack ();


public struct CommonData{
	
	public static string JsonFileDirectoryPath = "Assets/Scripts/JsonData";
//	public static string effectsFileName = "SkillEffectData.txt";
	public static string effectsDataFileName = "TestEffectString.txt";
	public static string chaptersDataFileName = "ChaptersString.txt";
	public static string chapterDataFileName = "DataString.txt";
//	public static string battleDataCanvansName = ""
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
	BothSides
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


[System.Serializable]
public class EffectData{
	public string effectName;//魔法效果名称

	public string description;//魔法效果描述

	public int id;//魔法效果id

	public int strengthConsume;//消耗的气力数

	public int effectDuration;//效果持续回合数

	public int actionConsume;//效果消耗行动数

	public SkillEffectTarget effectTarget;//效果目标

	public float scaler;//影响系数

	public bool isStateEffect;//是否是状态类效果

	public bool canOverlay;//是否可以叠加

	public StartTurn startTurn;//效果开始轮

	public EffectType effectType;//物理攻击、魔法攻击、混乱攻击

	public override string ToString ()
	{
		return string.Format ("[EffectDataInitializer]" + "\n[effectName]:" + effectName + "\n[description]:" + description + "\n[id]:" + id);
	}

}

[System.Serializable]
public struct Word{

	public string word;
	public string description;

}


