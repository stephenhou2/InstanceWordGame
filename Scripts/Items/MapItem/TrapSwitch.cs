using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class TrapSwitch : MapItem {

		public Trap trap;

		public bool switchOff;

		public Sprite originSprite;
		public Sprite unlockedOrDestroyedSprite;

		public override void InitMapItem ()
		{
			this.switchOff = false;

			transform.GetComponent<SpriteRenderer> ().sprite = originSprite;

		}

		public void SwitchOffTrap(){

			if (switchOff) {
				return;
			}

			transform.GetComponent<SpriteRenderer> ().sprite = unlockedOrDestroyedSprite;

			trap.OnSwitchOff ();

			this.switchOff = true;

		}

	}
}
