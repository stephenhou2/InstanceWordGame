using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class TrapSwitch : MapItem {

		public Trap trap;

		public bool switchOff;

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
