using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class Door : MapItem {

		public Sprite doorCloseSprite;
		public Sprite doorOpenSprite;

		public string question;
		public string answer;

		private bool myDoorOpen;
		public bool doorOpen{
			get{
				return myDoorOpen;
			}
			set{
				myDoorOpen = value;

				if (myDoorOpen) {
					mapItemRenderer.sprite = doorOpenSprite;
				} else {
					mapItemRenderer.sprite = doorCloseSprite;
				}
			}
		}

		public override void InitMapItem ()
		{
			bc2d.enabled = true;
			myDoorOpen = false;
			mapItemRenderer.sprite = doorCloseSprite;
		}

		public void OpenTheDoor(){
			doorOpen = true;
			mapItemRenderer.sprite = doorOpenSprite;
		}

	}
}
