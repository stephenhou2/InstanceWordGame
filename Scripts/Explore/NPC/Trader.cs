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
			return Item.NewItemWith (goodsIdAsItem, 1);
		}

	}
		
}
