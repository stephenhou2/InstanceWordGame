using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class Billboard : MapItem {

		public string content;

		public override void InitMapItem ()
		{
//			gameObject.SetActive (true);
			SetSortingOrder (-(int)transform.position.y);
		}


		public override void AddToPool (InstancePool pool)
		{
			gameObject.SetActive (false);
			bc2d.enabled = false;
			pool.AddInstanceToPool (this.gameObject);
		}


	}
}
