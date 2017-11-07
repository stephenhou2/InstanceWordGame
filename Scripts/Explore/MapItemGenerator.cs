using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	public class MapItemGenerator:MonoBehaviour {

		public Obstacle[] obstacleModels;

		public Trap trapModel;

		public TrapSwitch trapSwitchModel;

		public TreasureBox lockedTreasureBoxModel;
		public TreasureBox normalTreasureBoxModel;

		private Count rewardsRange;
		private Count lockedTreasureBoxRange;
		private Count trapRange;

		void Awake(){
			rewardsRange = new Count (1, 3);
			lockedTreasureBoxRange = new Count (0, 2);
			trapRange = new Count (0, 2);
		}

		public List<MapItem> RandomMapItems(List<Item> normalItems, List<Item> lockedItems, InstancePool itemPool, Transform itemsContainer, int mapItemCount){

			List<MapItem> mapItems = new List<MapItem> ();

			int lockedTreasureBoxCount = Random.Range (lockedTreasureBoxRange.minimum, lockedTreasureBoxRange.maximum + 1);

			for (int i = 0; i < lockedTreasureBoxCount; i++) {

				int rewardItemCount = Random.Range (rewardsRange.minimum, rewardsRange.maximum);

				TreasureBox lockedTreasureBox = itemPool.GetInstanceWithName<TreasureBox> (lockedTreasureBoxModel.name, lockedTreasureBoxModel.gameObject, itemsContainer);

				lockedTreasureBox.mapItemName = lockedTreasureBoxModel.mapItemName;

				Item[] rewardItems = new Item[rewardItemCount];

				for (int j = 0; j < rewardItemCount; j++) {

					Item item = RandomItem (lockedItems);

					rewardItems [j] = item;
				}
					
				lockedTreasureBox.InitMapItem ();

				lockedTreasureBox.rewardItems = rewardItems;

				mapItems.Add (lockedTreasureBox);


			}

			int trapCount = Random.Range (trapRange.minimum, trapRange.maximum + 1);

			for (int i = 0; i < trapCount; i++) {

				Trap trap = itemPool.GetInstanceWithName<Trap> (trapModel.name, trapModel.gameObject, itemsContainer);

				TrapSwitch trapSwitch = itemPool.GetInstanceWithName<TrapSwitch> (trapSwitchModel.name, trapSwitchModel.gameObject, itemsContainer);

				trapSwitch.trap = trap;

				trap.mapItemName = trapModel.mapItemName;

				trapSwitch.mapItemName = trapSwitchModel.mapItemName;

				trapSwitch.InitMapItem ();

				trap.InitMapItem ();

				mapItems.Add (trap);

				mapItems.Add (trapSwitch);

			}

			for (int i = lockedTreasureBoxCount + trapCount; i < mapItemCount; i++) {

				int rewardItemCount = Random.Range (rewardsRange.minimum, rewardsRange.maximum);

				MapItemType type = RandomMapItemType ();

				switch (type) {
				case MapItemType.Obstacle:

					int modelIndex = Random.Range (0, obstacleModels.Length);

					Obstacle randomModel = (obstacleModels [modelIndex]);

					Obstacle obstacle = itemPool.GetInstanceWithName<Obstacle> (randomModel.name, randomModel.gameObject, itemsContainer);

					obstacle.mapItemName = randomModel.mapItemName;

					obstacle.InitMapItem ();

					mapItems.Add (obstacle);

					break;

				case MapItemType.TreasureBox:

					TreasureBox normalTreasureBox = itemPool.GetInstanceWithName<TreasureBox> (normalTreasureBoxModel.name, normalTreasureBoxModel.gameObject, itemsContainer);

					normalTreasureBox.mapItemName = normalTreasureBoxModel.mapItemName;

					Item[] rewardItems = new Item[rewardItemCount];

					for (int j = 0; j < rewardItemCount; j++) {

						Item item = RandomItem (normalItems);

						rewardItems [j] = item;
					}

					normalTreasureBox.InitMapItem ();

					normalTreasureBox.rewardItems = rewardItems;

					mapItems.Add (normalTreasureBox);

					break;
				}
			}

			return mapItems;

		}

		private MapItemType RandomMapItemType(){

			int seed = Random.Range (0, 2);

			MapItemType mip = MapItemType.None;

			switch (seed) {
			case 0:
				mip = MapItemType.Obstacle;
				break;
			case 1:
				mip = MapItemType.TreasureBox;
				break;
			}

			return mip;
		}



		private Item RandomItem(List<Item> eventsList){

			int index = Random.Range (0, eventsList.Count);

			return eventsList [index];

		}
			

 	}
}
