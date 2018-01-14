using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class Crystal : MapItem {

		public bool isExausted;

		public Animator mapItemAnimator;

		public override void InitMapItem ()
		{
			isExausted = false;
			bc2d.enabled = true;
			mapItemAnimator.SetBool ("Play",false);
			SetSortingOrder (-(int)transform.position.y);
		}

		public void CrystalExausted(){

			isExausted = true;

			bc2d.enabled = false;

			mapItemAnimator.SetBool ("Play",true);

		}

		public override void  AddToPool(InstancePool pool){
			bc2d.enabled = false;
			pool.AddInstanceToPool (this.gameObject);
		}

	}
}
