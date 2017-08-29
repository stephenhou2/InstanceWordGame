using System.Collections;
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

		private List<Monster> monsters = new List<Monster>();


		private void LoadItemsData(){

			foreach (int itemId in itemIds) {

				Item item = GameManager.Instance.allItems.Find (delegate(Item i) {
					return i.itemId == itemId;
				});

				items.Add (item);
			}
		
		}

		private void LoadMonsters(){

//			ResourceManager.Instance.gos.Clear ();
//
//			for (int i = 0; i < monsterIds.Length; i++) {
//
//				string monsterName = string.Format ("Monster_{0}", monsterIds [i]);
//
//				ResourceManager.Instance.LoadAssetWithFileName ("monsters", () => {
//
//					monsters.Add(ResourceManager.Instance.gos[0].GetComponent<Monster>());
//
//				}, true, monsterName);
//					
//			}

			monsters = GameManager.Instance.allMonsters;

		}

		private void LoadNPCsData(){

			NPC[] npcsArray = DataInitializer.LoadDataToModelWithPath<NPC> (CommonData.jsonFileDirectoryPath, "AllNpcsJson.txt");
			
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

		public List<Monster> GetCurrentChapterMonsters(){

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
