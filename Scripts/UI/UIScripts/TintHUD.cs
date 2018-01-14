using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	using UnityEngine.UI;

	public class TintHUD : MonoBehaviour {

		public Text tintText;
		public float tintHUDShowDuration = 1f;
		private IEnumerator tintHUDCoroutine;

		public void SetUpTintHUD(string tint){

			tintText.text = tint;

			gameObject.SetActive (true);

			if (tintHUDCoroutine != null) {
				StopCoroutine (tintHUDCoroutine);
			}

			tintHUDCoroutine = TintHUDLatelyDisappear ();

			StartCoroutine (tintHUDCoroutine);

		}

		private IEnumerator TintHUDLatelyDisappear(){

			yield return new WaitForSecondsRealtime (tintHUDShowDuration);

			gameObject.SetActive (false);
		}

		public void QuitTintHUD(){
			if (tintHUDCoroutine != null) {
				StopCoroutine (tintHUDCoroutine);
			}
			gameObject.SetActive (false);
		}
	}
}
