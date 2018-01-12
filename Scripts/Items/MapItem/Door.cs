using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class Door : MapItem {

		public Sprite doorCloseSprite;
		public Sprite doorOpenSprite;

		public bool isDoorOpen;

		public void OpenTheDoor(){
			mapItemRenderer.sprite = doorOpenSprite;
			isDoorOpen = true;
			bc2d.enabled = false;
		}

		public void CloseTheDoor(){
			mapItemRenderer.sprite = doorCloseSprite;
			isDoorOpen = false;
			bc2d.enabled = true;
		}

		public override void InitMapItem ()
		{
			bc2d.enabled = true;
			isDoorOpen = false;
			mapItemRenderer.sprite = doorCloseSprite;
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
