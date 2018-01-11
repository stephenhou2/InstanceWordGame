using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;



namespace WordJourney
{
	[System.Serializable]
	public struct MonsterInfo{
		public int monsterId;
		public int monsterCount;

		public MonsterInfo(int monsterId,int monsterCount){
			this.monsterId = monsterId;
			this.monsterCount = monsterCount;
		}
	}

	
	[System.Serializable]
	public class GameLevelData {

		// 关卡序号
		public int gameLevelIndex;

		// 所在章节名称(5关一个章节)
		public string chapterName;

		// 关卡中的所有怪物信息
		public MonsterInfo[] monsterInfos;

		// 瓦罐中一定能开出来的物品id数组
		public int[] mustAppearItemIdsInUnlockedBox;

		// 瓦罐中可能会开出来的物品id数组
		public int[] possiblyAppearItemIdsInUnlockedBox;

		// 宝箱中可能会开出来的物品id数组
		public int[] possiblyAppearItemIdsInLockedBox;

		// 商人处卖的商品信息组
//		public List<GoodsGroup> goodsGroups;

		// 关卡中怪物相对与prefab的提升比例
		public float monsterScaler;

		// 关卡中boss的id（-1代表本关不出现boss）
		public int bossId;



		// 关卡的瓦罐一定会出现的物品组
		public List<Item> mustAppearItemsInUnlockedBox = new List<Item>();

		// 关卡的瓦罐中可能会出现的物品组
		public List<Item> possiblyAppearItemsInUnlockedBox = new List<Item> ();

		// 关卡的宝箱中可能会出现的物品组
		public List<Item> possiblyAppearItemsInLockedBox = new List<Item> ();

//		// 关卡中所有npc的id对应的npc
//		public List<NPC> npcs = new List<NPC>();

		// 关卡中的所有怪物
		public List<Transform> monsters = new List<Transform>();


		public void LoadAllData(){
			LoadAllItemsData ();
			LoadNPCData ();
			LoadMonsters ();
		}

		/// <summary>
		/// 加载所有本关卡物品数据
		/// </summary>
		private void LoadAllItemsData(){

			for (int i = 0; i < mustAppearItemIdsInUnlockedBox.Length; i++) {
				Item item = Item.NewItemWith(mustAppearItemIdsInUnlockedBox [i],1);
				mustAppearItemsInUnlockedBox.Add (item);
			}
			for (int i = 0; i < possiblyAppearItemIdsInUnlockedBox.Length; i++) {
				Item item = Item.NewItemWith (possiblyAppearItemIdsInUnlockedBox [i], 1);
				possiblyAppearItemsInUnlockedBox.Add (item);
			}
			for (int i = 0; i < possiblyAppearItemIdsInLockedBox.Length; i++) {
				Item item = Item.NewItemWith (possiblyAppearItemIdsInLockedBox [i], 1);
				possiblyAppearItemsInLockedBox.Add (item);
			}
		
		}

		/// <summary>
		/// 加载所有本关卡怪物
		/// </summary>
		private void LoadMonsters(){

			for (int i = 0; i < monsterInfos.Length; i++) {

				MonsterInfo info = monsterInfos [i];

				Transform monster = GameManager.Instance.gameDataCenter.allMonsters.Find (delegate(Transform obj) {
					return obj.GetComponent<Monster>().monsterId == info.monsterId;
				});

				if (monster == null) {
					Debug.LogError ("monster null when load level info");
				}

				for (int j = 0; j < info.monsterCount; j++) {
					monsters.Add (monster);
				}
			}
		}

		/// <summary>
		/// 加载所有本关卡npc
		/// </summary>
		private void LoadNPCData(){

//			for (int i = 0; i < npcIds.Length; i++) {
//				
//				NPC npc = GameManager.Instance.gameDataCenter.allNpcs.Find (delegate(NPC obj) {
//					return obj.npcId == npcIds[i];
//				});
//
//				if (npc != null) {
//					npcs.Add (npc);
//				} else {
//					Debug.LogError ("npc null when load level info");
//				}
//			}
		}
			


		public override string ToString ()
		{
			return string.Format ("[chapterIndex:]" + gameLevelIndex +
			"[\nchapterLocation:]" + chapterName);
		}
	}
}
