using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class Crystal : MapItem {

		public bool isExausted;

		public override void InitMapItem ()
		{
//			gameObject.SetActive (true);
			isExausted = false;
			mapItemAnimator.SetBool ("Play",false);
			SetSortingOrder (-(int)transform.position.y);
//			mapItemAnimator.enabled = true;
		}

		public void CrystalExausted(){

			isExausted = true;

			bc2d.enabled = false;

			mapItemAnimator.SetBool ("Play",true);

		}

		public override void  AddToPool(InstancePool pool){

			gameObject.SetActive (false);
//			bc2d.enabled = false;
			isExausted = false;
//			mapItemAnimator.enabled = false;
			pool.AddInstanceToPool (this.gameObject);

		}

	}
}
