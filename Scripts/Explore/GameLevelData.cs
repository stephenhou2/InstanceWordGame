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

		// 关卡中所有可能出现的装备配方对应的装备／技能id
		public int[] formulaIds;

		// 每个itemId对应的物品是否只能由宝箱开出来
		public bool[] itemLockInfoArray;

		// 关卡中所有npc的id
		public int[] npcIds;

		// 关卡中所有宝箱的数量
		public int lockedTreasureBoxCount;


		// 关卡中怪物相对与prefab的提升比例
		public float monsterScaler;

		// 关卡中boss的id（-1代表本关不出现boss）
		public int bossId;

		// 关卡中出现的所有可以直接开出的物品id对应的物品
		public List<Item> normalItems = new List<Item>();

		// 关卡中出现的所有宝箱开出的物品id对应的物品
		public List<Item> lockedItems = new List<Item> ();

		// 关卡中所有npc的id对应的npc
		public List<NPC> npcs = new List<NPC>();

		// 关卡中的所有怪物id对应的怪物
		public List<Transform> monsters = new List<Transform>();


		public void LoadAllData(){
			LoadAllItemsData ();
			LoadNPCsData ();
			LoadMonsters ();
		}

		/// <summary>
		/// 加载所有本关卡物品数据
		/// </summary>
		private void LoadAllItemsData(){

			for(int i = 0;i<itemIds.Length;i++) {

				int itemId = itemIds [i];
				bool locked = itemLockInfoArray [i];

				Item item = null;

				//如果是材料（1000<=材料id<2000)
				if (itemId >= 1000 && itemId < 2000) {

//					Material material = GameManager.Instance.gameDataCenter.allMaterials.Find (delegate(Material obj) {
//						return obj.itemId == itemId;
//					});
//
//					item = new Material (material, 1);

				} else {

					ItemModel itemModel = GameManager.Instance.gameDataCenter.allItemModels.Find (delegate(ItemModel obj) {
						return obj.itemId == itemId;
					});

					switch (itemModel.itemType) {
					case ItemType.Equipment:
						item = new Equipment (itemModel);
						break;
					case ItemType.Consumables:
						item = new Consumables (itemModel, 1);
						break;
					}
						
				}

				if (item != null && !locked) {
					normalItems.Add (item);
				} else if (item != null && locked) {
					lockedItems.Add (item);
				} else {
					Debug.LogError ("item null when load level info");
				}
			}

			for (int i = 0; i < formulaIds.Length; i++) {

				Formula formula = new Formula (FormulaType.Equipment, formulaIds [i]);

				if (formula != null) {
					lockedItems.Add (formula);
				} else {
					Debug.LogError ("formula null when load level info");
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
			


		public override string ToString ()
		{
			return string.Format ("[chapterIndex:]" + gameLevelIndex +
			"[\nchapterLocation:]" + chapterName);
		}
	}
}
