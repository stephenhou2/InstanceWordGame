using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChapterDetailInfo {

	public int totalSteps;

	public string chapterLocation;

	public Monster[][] monstersGroup;

	public Item[] items;

	public NPC[] npcs;
}
