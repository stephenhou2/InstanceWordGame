using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public abstract class Trap : MapItem {

		// 陷阱已经关闭
		public bool trapOn;

		/// <summary>
		/// 初始化陷阱
		/// </summary>
		public override void InitMapItem ()
		{
			bc2d.enabled = true;
		}

		public void ChangeTrapStatus(){
			if (trapOn) {
				SetTrapOff ();
				trapOn = false;
			} else {
				SetTrapOn ();
				trapOn = true;
			}
		}

		public abstract void SetTrapOn ();
		public abstract void SetTrapOff ();


		/// <summary>
		/// 进入陷阱
		/// </summary>
		/// <param name="col">Col.</param>
		public abstract void OnTriggerEnter2D(Collider2D col);


	}
}
