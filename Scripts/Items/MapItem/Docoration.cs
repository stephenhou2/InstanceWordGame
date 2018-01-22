using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class Docoration : MapItem {


//		private List<Sprite> mAllDocorationSprites = new List<Sprite>();
		public List<Sprite> allDocorationSprites;

		public override void InitMapItem ()
		{
			bc2d.enabled = true;
			int randomDocorationIndex = Random.Range (0, allDocorationSprites.Count);
			Sprite docorationSprite = allDocorationSprites [randomDocorationIndex];
			mapItemRenderer.sprite = docorationSprite;
			SetSortingOrder (-(int)transform.position.y);
		}

		public override void AddToPool (InstancePool pool)
		{
			bc2d.enabled = false;
			pool.AddInstanceToPool (this.gameObject);
		}

	}
}
