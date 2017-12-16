using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{

	[System.Serializable]
	public class Trader : NPC {

		// 商人售卖的商品列表
		public List<GoodsGroup> goodsGroupList = new List<GoodsGroup>();

	}

	[System.Serializable]
	public class GoodsGroup{

		public List<Goods> goodsList = new List<Goods> ();

		// 商品对应的关卡
		public int accordLevel;

	}


	[System.Serializable]
	public class Goods{

		// 商品对应物品的id
		public int goodsId;
		// 商品的价格
		public int goodsPrice;

		private Item myItemAsGoods;
		public Item itemAsGoods{
			get{
				if (myItemAsGoods == null) {
					myItemAsGoods = Item.NewItemWith (goodsId, 1);
				}
				return myItemAsGoods;
			}
		}

	}
}
