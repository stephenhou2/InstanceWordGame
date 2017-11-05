﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;



namespace WordJourney
{
	
	[System.Serializable]
	public class ChapterDetailInfo {

		public int chapterIndex;

		public string chapterLocation;

		public int[] monsterIds;

		public int[] itemIds;

		public int[] npcIds;

		public float monsterScaler;

		public Count itemCount;

		public Count monsterCount;

		private List<Item> items = new List<Item>();

		private List<NPC> npcs = new List<NPC>();

		private List<Transform> monsters = new List<Transform>();


		private void LoadItemsData(){

			foreach (int itemId in itemIds) {

				ItemModel itemModel = GameManager.Instance.gameDataCenter.allItemModels.Find (delegate(ItemModel obj) {
					return obj.itemId == itemId;
				});

				Item item = null;

				switch (itemModel.itemType) {
				case ItemType.Equipment:
					#warning 这里装备的耐久度都设为50
					item = new Equipment (itemModel,5);
					break;
				case ItemType.Consumables:
					item = new Consumables (itemModel,1);
					break;
				default:
					break;
				}

				if (item != null) {
					items.Add (item);
				}
			}
		
		}

		private void LoadMonsters(){

//			ResourceLoader.CreateNewResourceLoader().gos.Clear ();
//
//			for (int i = 0; i < monsterIds.Length; i++) {
//
//				string monsterName = string.Format ("Monster_{0}", monsterIds [i]);
//
//				ResourceLoader.CreateNewResourceLoader().LoadAssetWithBundlePath ("monsters", () => {
//
//					monsters.Add(ResourceLoader.CreateNewResourceLoader().gos[0].GetComponent<Monster>());
//
//				}, true, monsterName);
//					
//			}

			monsters = GameManager.Instance.gameDataCenter.allMonsters;

		}

		private void LoadNPCsData(){

			NPC[] npcsArray = DataHandler.LoadDataToModelWithPath<NPC> (CommonData.npcsDataFilePath);
			
			foreach (int npcId in npcIds) {
				foreach (NPC npc in npcsArray) {
					if (npc.npcId == npcId) {
						npcs.Add (npc);
					}
				}
			}
		}

		public List<Item> GetCurrentChapterItems(){

			if (items.Count == 0) {
				LoadItemsData ();
			}

			return items;

		}

		public List<NPC> GetCurrentChapterNpcs(){

			if (npcs.Count == 0) {
				LoadNPCsData ();
			}

			return npcs;

		}

		public List<Transform> GetCurrentChapterMonsters(){

			if (monsters.Count == 0) {
				LoadMonsters ();
			}

			return monsters;

		}


		public override string ToString ()
		{
			return string.Format ("[chapterIndex:]" + chapterIndex +
			"[\nchapterLocation:]" + chapterLocation);
		}
	}
}
