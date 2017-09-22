using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class Obstacle : MapItem {

		protected override void Awake ()
		{
			base.Awake ();
			this.mapItemType = MapItemType.Obstacle;
		}

	}
}
