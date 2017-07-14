using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChapterDetailInfo {

	public int chapterIndex;

	public int totalSteps;

	public int stepsLeft;

	public string chapterLocation;

	public MonsterGroup[] monsterGroups;

	public Item[] items;

	public NPC[] npcs;

	public override string ToString ()
	{
		return string.Format ("[chapterIndex:]" + chapterIndex + 
			"[\nTotalSteps]:" + totalSteps + 
			"[\nchapterLocation:]" + chapterLocation + 
			"[\nmonsterGroupsCount:]" + monsterGroups.Length + 
			"[\nitems:]" + items.Length + 
			"[\nnpcs:]" + npcs.Length);
	}
}
