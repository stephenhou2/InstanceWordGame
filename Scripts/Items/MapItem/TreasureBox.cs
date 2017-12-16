using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	public class TreasureBox: MapItem {

		// 开启所需物品的名称
		public int unlockItemId;

		// 是否有锁
		public bool locked;

		// 奖励的物品数组
//		public Item[] rewardItems;

		// 奖励的物品
		public Item rewardItem;



		/// <summary>
		/// 初始化箱子类道具
		/// </summary>
		public override void InitMapItem ()
		{
			bc2d.enabled = true;
			mapItemAnimator.ResetTrigger ("Play");
		}

		protected override void AnimEnd ()
		{
			locked = false;
			base.AnimEnd ();
		}

	}
}
