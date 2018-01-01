using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class Obstacle : MapItem {

		public int destroyToolId;


		/// <summary>
		/// 初始化障碍物
		/// </summary>
		public override void InitMapItem ()
		{
			bc2d.enabled = true;
			mapItemAnimator.SetBool ("Play",false);
		}
	}
}
