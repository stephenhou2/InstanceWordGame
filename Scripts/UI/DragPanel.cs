using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	using UnityEngine.UI;
	using UnityEngine.EventSystems;

	public class DragPanel : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler {

		public Transform dragedPanel;



		public void OnBeginDrag(PointerEventData data){
			dragedPanel.localScale = new Vector3 (1.2f, 1.2f, 1);
		}

		public void OnDrag(PointerEventData data){
			
		}

		public void OnEndDrag(PointerEventData data){
			dragedPanel.localScale = Vector3.one;
		}

	}
}
