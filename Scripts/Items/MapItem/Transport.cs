using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class Transport : MapItem {


		public override void InitMapItem ()
		{
//			gameObject.SetActive (true);
//			bc2d.enabled = true;
			mapItemAnimator.enabled = true;
//			SetSortingOrder (-(int)transform.position.y);
		}

		public override void AddToPool(InstancePool pool){
//			bc2d.enabled = false;
//			mapItemAnimator.enabled = false;
			gameObject.SetActive(false);
			pool.AddInstanceToPool (this.gameObject);
		}


	}
}
