using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class TrapSwitch : MapItem {

		// 开关控制的陷阱
//		public Trap trap;

		public Sprite switchOffSprite;

		public Sprite switchOnSprite;

		private int switchStatusChangeCount;

		/// <summary>
		/// 初始化开关
		/// </summary>
		public override void InitMapItem ()
		{
			bc2d.enabled = true;
			mapItemRenderer.sprite = switchOffSprite;
			switchStatusChangeCount = 0;
			SetSortingOrder (-(int)transform.position.y);
		}

		public override void AddToPool (InstancePool pool)
		{
			bc2d.enabled = false;
			pool.AddInstanceToPool (this.gameObject);
		}

		/// <summary>
		/// 关闭陷阱
		/// </summary>
		public void ChangeSwitchStatus(){

			switchStatusChangeCount++;

			if (switchStatusChangeCount % 2 == 0) {
				mapItemRenderer.sprite = switchOffSprite;
			} else {
				mapItemRenderer.sprite = switchOnSprite;
			}
				
		}

	}
}
