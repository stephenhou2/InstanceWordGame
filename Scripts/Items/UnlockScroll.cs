using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public enum UnlockScrollType{
		Equipment,
		Consumables
	}

	/*************attention****************

	itemId：原物品id基础上+200 200-400

	*************attention****************/
	[System.Serializable]
	public class UnlockScroll:Item {

		public int unlockedItemId;

		public bool unlocked;

		public UnlockScrollType unlockScrollType;

		public UnlockScroll(int unlockedItemId){

			ItemModel unlockedItemModel = GameManager.Instance.gameDataCenter.allItemModels.Find (delegate (ItemModel obj) {
				return obj.itemId == unlockedItemId;
			});

			InitWithItemModel (unlockedItemModel);
		}

		public UnlockScroll(ItemModel unlockedItemModel){

			InitWithItemModel (unlockedItemModel);
		}

		private void InitWithItemModel(ItemModel itemModel){

			this.itemType = ItemType.UnlockScroll;

			this.unlockedItemId = itemModel.itemId;

			if (unlockedItemId >= 0 && unlockedItemId < 100) {
				this.unlockScrollType = UnlockScrollType.Equipment;
			} else if (unlockedItemId >= 100 && unlockedItemId < 200) {
				this.unlockScrollType = UnlockScrollType.Consumables;
			}
			#warning 配方图片只有配方和卷轴两种，直接在构造函数里赋值
			this.spriteName = "";

			this.itemName = itemModel.itemName;
			this.itemNameInEnglish = itemModel.itemNameInEnglish;

			this.itemId = itemModel.itemId + 200;

			this.itemDescription = itemModel.itemDescription;

			this.itemCount = 1;

			this.unlocked = false;
		}
			

		public override string GetItemTypeString ()
		{
			return "解锁卷轴";
		}

		public override string ToString ()
		{
			return string.Format ("[解锁卷轴]--{0}",itemName);
		}
	}
}
