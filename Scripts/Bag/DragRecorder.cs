using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WordJourney
{
	public class DragRecorder : MonoBehaviour,IDragHandler {

		private PointerEventData mData;

		public void OnDrag(PointerEventData data){
			mData = data;
		}

		public PointerEventData GetPointerEventData(float offset){

			if (mData == null) {
				return null;
			}

			mData.position = new Vector2 (mData.position.x, mData.position.y - offset);

			return mData;

		}
	}
}
