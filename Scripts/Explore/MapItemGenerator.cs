using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	public class MapItemGenerator:MonoBehaviour {

		// 障碍物模型数组
		public Obstacle[] obstacleModels;

		// 陷阱模型
		public Trap trapModel;

		// 开关模型
		public TrapSwitch trapSwitchModel;

		// 宝箱模型
		public TreasureBox lockedTreasureBoxModel;

		// 正常箱子模型（不需要钥匙开启）
		public TreasureBox normalTreasureBoxModel;

		// 每个箱子能开出来的物品数量范围
//		private Count rewardsRange;
		// 每关的宝箱数量范围
//		private Count lockedTreasureBoxRange;
		// 每关的陷阱数量范围
		private Count trapRange;

		void Awake(){
//			rewardsRange = new Count (1, 3);
//			lockedTreasureBoxRange = new Count (0, 1);
			trapRange = new Count (0, 1);
		}

		/// <summary>
		/// 初始化关卡中的地图物品
		/// </summary>
		/// <returns>The map items.</returns>
		/// <param name="normalItems">放在正常箱子中的物品列表</param>
		/// <param name="lockedItems">必须放在宝箱中的物品列表</param>
		/// <param name="itemPool">缓存池</param>
		/// <param name="itemsContainer">地图物品在场景中的父容器</param>
		/// <param name="mapItemCount">地图物品数量</param>
		public List<MapItem> InitMapItems(GameLevelData levelData, InstancePool itemPool, Transform itemsContainer){


			List<Item> normalItems = levelData.normalItems;

			List<Item> lockedItems = levelData.lockedItems;

			int lockedTreasureBoxCount = levelData.lockedTreasureBoxCount;

			int potCount = Random.Range(levelData.potCount.minimum, levelData.potCount.maximum + 1);

			int obstacleCount = Random.Range(levelData.obstacleCount.minimum, levelData.potCount.maximum + 1);

			// 本关的所有地图物品列表
			List<MapItem> mapItems = new List<MapItem> ();

			// 创建宝箱
			for (int i = 0; i < lockedTreasureBoxCount; i++) {

//				int rewardItemCount = Random.Range (rewardsRange.minimum, rewardsRange.maximum);

				TreasureBox lockedTreasureBox = itemPool.GetInstanceWithName<TreasureBox> (lockedTreasureBoxModel.name, lockedTreasureBoxModel.gameObject, itemsContainer);

				lockedTreasureBox.mapItemName = lockedTreasureBoxModel.mapItemName;

				// 宝箱中装的item数组
//				Item[] rewardItems = new Item[rewardItemCount];
//
//				for (int j = 0; j < rewardItemCount; j++) {
//
//					Item item = RandomItem (lockedItems,rewardItems);
//
//					rewardItems [j] = item;
//				}

				int randomItemIndex = Random.Range (0, lockedItems.Count);

				Item rewardItem = lockedItems [randomItemIndex];
					
				// 初始化地图物品
				lockedTreasureBox.InitMapItem ();

				lockedTreasureBox.rewardItem = rewardItem;

				mapItems.Add (lockedTreasureBox);

			}

			// 创建0-1个陷阱
			int trapCount = Random.Range (trapRange.minimum, trapRange.maximum + 1);

			for (int i = 0; i < trapCount; i++) {

				Trap trap = itemPool.GetInstanceWithName<Trap> (trapModel.name, trapModel.gameObject, itemsContainer);

//				TrapSwitch trapSwitch = itemPool.GetInstanceWithName<TrapSwitch> (trapSwitchModel.name, trapSwitchModel.gameObject, itemsContainer);

//				trapSwitch.trap = trap;

				trap.mapItemName = trapModel.mapItemName;

//				trapSwitch.mapItemName = trapSwitchModel.mapItemName;

//				trapSwitch.InitMapItem ();

				trap.InitMapItem ();

				mapItems.Add (trap);

//				mapItems.Add (trapSwitch);

			}
				
			// 创建瓦罐
			for (int i = 0; i < potCount; i++) {

				TreasureBox normalTreasureBox = itemPool.GetInstanceWithName<TreasureBox> (normalTreasureBoxModel.name, normalTreasureBoxModel.gameObject, itemsContainer);

				normalTreasureBox.mapItemName = normalTreasureBoxModel.mapItemName;

				int randomItemIndex = Random.Range (0, normalItems.Count);

				Item rewardItem = normalItems [randomItemIndex];

				normalTreasureBox.InitMapItem ();

				normalTreasureBox.rewardItem = rewardItem;

				mapItems.Add (normalTreasureBox);

			}
				

			// 创建障碍物
			for (int i = 0; i < obstacleCount; i++) {

				int modelIndex = Random.Range (0, obstacleModels.Length);

				Obstacle randomModel = (obstacleModels [modelIndex]);

				Obstacle obstacle = itemPool.GetInstanceWithName<Obstacle> (randomModel.name, randomModel.gameObject, itemsContainer);

				obstacle.mapItemName = randomModel.mapItemName;

				obstacle.InitMapItem ();

				mapItems.Add (obstacle);

			}

			return mapItems;

		}



		/// <summary>
		/// 从列表中随机一种物品
		/// </summary>
		/// <returns>The item.</returns>
		/// <param name="eventsList">Events list.</param>
		private Item RandomItem(List<Item> itemList,Item[] rewardItems){

			int index = Random.Range (0, itemList.Count);

			int itemCount = 1;

			Item item = Item.NewItemWith(itemList[index].itemId,itemCount);

			for (int i = 0; i < rewardItems.Length; i++) {

				Item reward = rewardItems [i];

				if (reward != null && reward.itemId == item.itemId) {
					RandomItem (itemList, rewardItems);
				}

			}

			return item;

		}
			

 	}
}
