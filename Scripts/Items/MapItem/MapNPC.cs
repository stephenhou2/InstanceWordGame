using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney{
	
	public class MapNPC : MapItem {

		[HideInInspector]public NPC npc;

		public override void InitMapItem ()
		{
			bc2d.enabled = true;
			SetSortingOrder (-(int)transform.position.y);
		}

		public override void AddToPool (InstancePool pool)
		{
			bc2d.enabled = false;
			this.gameObject.SetActive (false);
			pool.AddInstanceToPool (this.gameObject);
		}


	}
}
