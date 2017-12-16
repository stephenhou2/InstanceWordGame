using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	public class MapItemGenerator:MonoBehaviour {










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
