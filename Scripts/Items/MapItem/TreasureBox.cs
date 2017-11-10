using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	public class TreasureBox: MapItem {

		// 开启所需物品的名称
		public string unlockItemName;

		// 是否已经打开
		public bool unlocked;

		// 奖励的物品数组
		public Item[] rewardItems;

		protected override void Awake ()
		{
			base.Awake ();
			this.mapItemType = MapItemType.TreasureBox;
		}

		/// <summary>
		/// 初始化箱子类道具
		/// </summary>
		public override void InitMapItem ()
		{
			unlocked = false;
			bc2d.enabled = true;
			mapItemAnimator.ResetTrigger ("Play");
		}

		protected override void AnimEnd ()
		{
			unlocked = true;
			base.AnimEnd ();
		}

	}
}
