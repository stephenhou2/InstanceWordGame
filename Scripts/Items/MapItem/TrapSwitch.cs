using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class TrapSwitch : MapItem {

		// 开关控制的陷阱
//		public Trap trap;

		// 开关控制杆在左边的图片
		public Sprite switchLeftSprite;
		// 开关控制杆在右边的图片
		public Sprite switchRightSprite;

		private int switchStatusChangeCount;

		/// <summary>
		/// 初始化开关
		/// </summary>
		public override void InitMapItem ()
		{
			bc2d.enabled = true;
			mapItemRenderer.sprite = switchLeftSprite;
		}

		/// <summary>
		/// 关闭陷阱
		/// </summary>
		public void ChangeSwitchStatus(){

			switchStatusChangeCount++;

			if (switchStatusChangeCount % 2 == 0) {
				mapItemRenderer.sprite = switchLeftSprite;
			} else {
				mapItemRenderer.sprite = switchRightSprite;
			}
				
		}

	}
}
