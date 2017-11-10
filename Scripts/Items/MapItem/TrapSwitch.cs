using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class TrapSwitch : MapItem {

		// 开关控制的陷阱
		public Trap trap;

		// 已经关闭陷阱
		public bool switchOff;

		// 开关的原始图片
		public Sprite originSprite;
		// 打开开关后的图片
		public Sprite unlockedOrDestroyedSprite;


		void Awake(){
			base.Awake ();
			this.mapItemType = MapItemType.TrapSwitch;
		}


		/// <summary>
		/// 初始化开关
		/// </summary>
		public override void InitMapItem ()
		{
			this.switchOff = false;

			transform.GetComponent<SpriteRenderer> ().sprite = originSprite;

		}

		/// <summary>
		/// 关闭陷阱
		/// </summary>
		public void SwitchOffTrap(){

			if (switchOff) {
				return;
			}

			transform.GetComponent<SpriteRenderer> ().sprite = unlockedOrDestroyedSprite;

			trap.OnSwitchOff ();

			this.switchOff = true;

		}

	}
}
