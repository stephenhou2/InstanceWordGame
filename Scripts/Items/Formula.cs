using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public enum FormulaType{
		Equipment,
		Consumables
	}

	/*************attention****************

	itemId：原物品id基础上+200 200-400

	*************attention****************/
	[System.Serializable]
	public class Formula:Item {

		public int unlockedItemId;

		public bool unlocked;

		public FormulaType formulaType;

		public Formula(int unlockedItemId){

			this.unlockedItemId = unlockedItemId;

			if (unlockedItemId >= 0 && unlockedItemId < 100) {
				this.formulaType = FormulaType.Equipment;
			} else if (unlockedItemId >= 100 && unlockedItemId < 200) {
				this.formulaType = FormulaType.Consumables;
			}


			#warning 配方图片只有配方和卷轴两种，直接在构造函数里赋值
			this.spriteName = "";

			ItemModel itemModel = GameManager.Instance.gameDataCenter.allItemModels.Find (delegate (ItemModel obj) {
				return obj.itemId == unlockedItemId;
			});

			this.itemName = itemModel.itemName;
				
			this.itemId = itemModel.itemId + 200;

			this.itemDescription = itemModel.itemDescription;
		}



		/// <summary>
		/// 解锁该配方对应的物品，并返回该物品信息
		/// </summary>
		/// <returns>The item model unlock.</returns>
		public ItemModel GetUnlockedItemModel(){

			ItemModel itemModel = GameManager.Instance.gameDataCenter.allItemModels.Find (delegate (ItemModel obj) {
				return obj.itemId == unlockedItemId;
			});
			return itemModel;
		}
			

		public override string GetItemTypeString ()
		{
			return "解锁卷轴";
		}

		public override string ToString ()
		{
			return string.Format ("[Formula]");
		}
	}
}
