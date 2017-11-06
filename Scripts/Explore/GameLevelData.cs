using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;



namespace WordJourney
{
	
	[System.Serializable]
	public class GameLevelData {

		// 关卡序号
		public int gameLevelIndex;

		// 所在章节名称(5关一个章节)
		public string chapterName;

		// 关卡中的所有怪物id
		public int[] monsterIds;

		// 关卡中出现的所有可以开出的物品id
		public int[] itemIds;

		// 每个itemId对应的物品是否只能由宝箱开出来
		public bool[] isItemLocked;

		// 关卡中所有npc的id
		public int[] npcIds;

		// 关卡中怪物相对与prefab的提升比例
		public float monsterScaler;

		// 关卡中一共出现的地图物品（瓦罐，箱子，宝箱）数量范围
		public Count itemCount;

		// 关卡中一共出现的怪物数量范围
		public Count monsterCount;

		// 关卡中出现的所有可以开出的物品id对应的物品
		private List<Item> items = new List<Item>();

		// 关卡中所有npc的id对应的npc
		private List<NPC> npcs = new List<NPC>();

		// 关卡中的所有怪物id对应的怪物
		private List<Transform> monsters = new List<Transform>();


		/// <summary>
		/// 加载所有本关卡物品数据
		/// </summary>
		private void LoadItemsData(){

			foreach (int itemId in itemIds) {

				ItemModel itemModel = GameManager.Instance.gameDataCenter.allItemModels.Find (delegate(ItemModel obj) {
					return obj.itemId == itemId;
				});

				Item item = null;

				switch (itemModel.itemType) {
				case ItemType.Equipment:
					item = new Equipment (itemModel,0);
					break;
				case ItemType.Consumables:
					item = new Consumables (itemModel,1);
					break;
				default:
					break;
				}

				if (item != null) {
					items.Add (item);
				} else {
					Debug.LogError ("item null when load level info");
				}
			}
		
		}

		/// <summary>
		/// 加载所有本关卡怪物
		/// </summary>
		private void LoadMonsters(){

			for (int i = 0; i < monsterIds.Length; i++) {

				Transform monster = GameManager.Instance.gameDataCenter.allMonsters.Find (delegate(Transform obj) {
					return obj.GetComponent<Monster>().monsterId == monsterIds [i];
				});

				if (monster != null) {
					monsters.Add (monster);
				} else {
					Debug.LogError ("monster null when load level info");
				}
			}
		}

		/// <summary>
		/// 加载所有本关卡npc
		/// </summary>
		private void LoadNPCsData(){

			for (int i = 0; i < npcIds.Length; i++) {
				
				NPC npc = GameManager.Instance.gameDataCenter.allNpcs.Find (delegate(NPC obj) {
					return obj.npcId == npcIds[i];
				});

				if (npc != null) {
					npcs.Add (npc);
				} else {
					Debug.LogError ("npc null when load level info");
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
			return string.Format ("[chapterIndex:]" + gameLevelIndex +
			"[\nchapterLocation:]" + chapterName);
		}
	}
}
