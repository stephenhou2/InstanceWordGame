using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class ItemInBagCell : MonoBehaviour {

		public Image itemIcon;
		public Image newItemTintIcon;

		public Text itemName;
		public Text itemCount;


		public void SetUpIteminBagCell(Item item){

			itemIcon.enabled = false;
			newItemTintIcon.enabled = false;

			itemName.text = item.itemName;

			if (!item.isNewItem && item.itemType == ItemType.Consumables) {
				itemCount.text = item.itemCount.ToString ();
				itemCount.enabled = true;
			} else {
				itemCount.text = string.Empty;
				itemCount.enabled = false;
			}

			Sprite itemSprite = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
				return obj.name == item.spriteName;
			});

			itemIcon.sprite = itemSprite;

			itemIcon.enabled = itemSprite != null;

			// 如果是新物品，则显示新物品提示图片
			newItemTintIcon.enabled = item.isNewItem;


		}



	}
}
