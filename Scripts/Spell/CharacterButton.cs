using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WordJourney
{
	public class CharacterButton : MonoBehaviour,IPointerDownHandler,IPointerUpHandler {


		public void OnPointerDown(PointerEventData data){

			transform.Find ("TintHUD").gameObject.SetActive(true);

		}

		public void OnPointerUp(PointerEventData data){

			transform.Find ("TintHUD").gameObject.SetActive(false);
		}

	}
}
