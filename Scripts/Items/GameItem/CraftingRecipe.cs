using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	[System.Serializable]
	public class CraftingRecipe : Item {

		public int craftItemId;


		public CraftingRecipe(int craftItemId){

			ItemModel itemModel = GameManager.Instance.gameDataCenter.allItemModels.Find (delegate (ItemModel obj) {
				return obj.itemId == craftItemId;
			});

			InitWithItemModel (itemModel);
		}


		public CraftingRecipe(ItemModel itemModel){
			InitWithItemModel (itemModel);
		}

		private void InitWithItemModel(ItemModel itemModel){

			this.itemType = ItemType.CraftingRecipes;
			this.craftItemId = itemModel.itemId;

			this.spriteName = "craftingRecipes";

			this.itemName = itemModel.itemName;
			this.itemNameInEnglish = itemModel.itemNameInEnglish;

			this.itemId = itemModel.itemId + 400;

			this.itemDescription = itemModel.itemDescription;

			this.itemCount = 1;

			this.price = itemModel.price;

		}

		public override string GetItemTypeString ()
		{
			return "合成配方";
		}


	}
}
