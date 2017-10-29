using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WordJourney
{
	public class DragRecorder : MonoBehaviour,IDragHandler,IBeginDragHandler {

		private PointerEventData mData;

		/// <summary>
		/// 拖拽时记录屏幕触摸记录
		/// </summary>
		/// <param name="data">Data.</param>
		public void OnDrag(PointerEventData data){
			mData = data;
		}

		/// <summary>
		/// 开始拖拽时记录屏幕拖拽记录
		/// </summary>
		/// <param name="data">Data.</param>
		public void OnBeginDrag(PointerEventData data){
			mData = data;
		}


		/// <summary>
		/// 根据原始拖拽记录转换为新的拖拽记录（消除拖拽过程中cell重用产生的拖拽位置偏差）
		/// </summary>
		/// <returns>The pointer event data.</returns>
		/// <param name="offset">Offset.</param>
		public PointerEventData GetPointerEventData(float offset){

			if (mData == null) {
				return null;
			}
				
			mData.position = new Vector2 (mData.position.x, 
				mData.position.y - offset/CommonData.scalerToPresetResulotion);

			return mData;

		}
	}
}
