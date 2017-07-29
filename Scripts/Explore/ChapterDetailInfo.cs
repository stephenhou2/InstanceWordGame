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

	public int[] itemIds;

	public NPC[] npcs;


	public Item[] GetItems(){

		Item[] items = new Item[itemIds.Length];

		for (int i = 0; i < itemIds.Length; i++) {

			int orgItemIndex = itemIds [i];

			Item originalItem = GameManager.Instance.allItems [orgItemIndex];

			items [i] = new Item (originalItem,true);

		}

		return items;
	}



	public override string ToString ()
	{
		return string.Format ("[chapterIndex:]" + chapterIndex +
		"[\nTotalSteps]:" + totalSteps +
		"[\nchapterLocation:]" + chapterLocation);
	}
}
