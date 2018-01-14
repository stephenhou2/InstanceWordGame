using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{

	[System.Serializable]
	public class Trader : NPC {
		// 商人售卖的商品列表
		public List<GoodsGroup> goodsGroupList = new List<GoodsGroup>();

		public List<Item> itemsAsGoodsOfCurrentLevel = new List<Item> ();

		public void InitGoodsGroupOfLevel(int level){

			itemsAsGoodsOfCurrentLevel.Clear ();

			GoodsGroup gg = goodsGroupList.Find (delegate(GoodsGroup obj) {
				return obj.accordLevel == level;
			});

			for (int i = 0; i < gg.goodsList.Count; i++) {

				Goods goods = gg.goodsList [i];

				Item itemAsGoods = goods.GetRandomPossibleGoods ();

				itemsAsGoodsOfCurrentLevel.Add (itemAsGoods);

			}

		}

		public void ClearTraderGoods(){
			itemsAsGoodsOfCurrentLevel.Clear ();
		}

	}

	[System.Serializable]
	public struct GoodsGroup{

		public List<Goods> goodsList;
		public int accordLevel;

		public GoodsGroup(List<Goods> goodsList,int accordLevel){
			this.goodsList = goodsList;
			this.accordLevel = accordLevel;
		}
	}

	[System.Serializable]
	public struct Goods{
		public int[] possibleItemIdsAsGoods;
		public Goods(int[] ids){
			this.possibleItemIdsAsGoods = ids;
		}

		public Item GetRandomPossibleGoods(){
			int randomIndex = Random.Range (0, possibleItemIdsAsGoods.Length);
			int goodsIdAsItem = possibleItemIdsAsGoods [randomIndex];
			if (goodsIdAsItem == -2) {
				goodsIdAsItem = GetRandomUnlockScrollId ();
			} else if (goodsIdAsItem == -3) {
				goodsIdAsItem = GetRandomCraftingRecipeId ();
			}
			return Item.NewItemWith (goodsIdAsItem, 1);
		}

		private int GetRandomUnlockScrollId(){

			int randomUnlockScrollId = 0;

			int type = Random.Range (0, 2);

			switch (type) {
			case 0:
				randomUnlockScrollId = 200 + Random.Range (Equipment.minProducableEquipmentId, Equipment.maxProducableEquipmentId + 1);
				break;
			case 1:
				randomUnlockScrollId = 200 + Random.Range (Consumables.minProducableConsumablesId, Consumables.maxProducableConsumablesId + 1);
				break;
			}

			return randomUnlockScrollId;
		}

		private int GetRandomCraftingRecipeId(){
			int randomCraftingRecipeId = 400 + Random.Range (Equipment.minCraftingEquipmentId, Equipment.maxCraftingEquipmentId + 1);
			return randomCraftingRecipeId;
		}

	}
		
}
