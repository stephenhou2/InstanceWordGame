using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	public class MapItemGenerator:MonoBehaviour {

		public Obstacle[] obstacleModels;

		public Trap trapModel;

		public TrapSwitch trapSwitchModel;

		public TreasureBox[] treasureBoxModels;

		public Count rewardsCount;



		public List<MapItem> RandomMapItems(List<Item> currentChapterItems,int mapItemCount){

			List<MapItem> mapItems = new List<MapItem> ();

			for (int i = 0; i < mapItemCount; i++) {

				int rewardItemCount = Random.Range (rewardsCount.minimum, rewardsCount.maximum);

				MapItemType type = RandomMapItemType ();

				switch (type) {
				case MapItemType.None:
					break;
				case MapItemType.Obstacle:

					int modelIndex = Random.Range (0, obstacleModels.Length);

					Obstacle obstacle = Instantiate (obstacleModels [modelIndex]);

					mapItems.Add (obstacle);

					break;

				case MapItemType.Trap:

					Trap trap = Instantiate (trapModel);

					TrapSwitch trapSwitch = Instantiate (trapSwitchModel);

					trapSwitch.trap = trap;

					mapItems.Add (trap);

					mapItems.Add (trapSwitch);

					break;

				case MapItemType.TreasureBox:

					modelIndex = Random.Range (0, treasureBoxModels.Length);

					TreasureBox tb = Instantiate (treasureBoxModels [modelIndex]);

					tb.rewardItems = new Item[rewardItemCount];

					for (int j = 0; j < rewardItemCount; j++) {

						Item item = RandomItem (currentChapterItems);

						tb.rewardItems [j] = item;
					}
					
					mapItems.Add (tb);

					break;
				}
			}

			return mapItems;

		}

		private MapItemType RandomMapItemType(){

			int seed = Random.Range (0, 3);

			MapItemType mip = MapItemType.None;

			switch (seed) {
			case 0:
				mip = MapItemType.Obstacle;
				break;
			case 1:
				mip = MapItemType.Trap;
				break;
			case 2:
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
