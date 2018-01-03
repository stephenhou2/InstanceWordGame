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
		}

		public void CloseTheDoor(){
			mapItemRenderer.sprite = doorCloseSprite;
			isDoorOpen = false;
		}

		public override void InitMapItem ()
		{
			bc2d.enabled = true;
			isDoorOpen = false;
			mapItemRenderer.sprite = doorCloseSprite;
		}
			

	}
}
