using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	// 地图物品类型枚举
	public enum MapItemType{
		Buck,
		Pot,
		TreasureBox,
		Tree,
		Stone,
		NormalTrapOn,
		NormalTrapOff,
		Switch,
		Door,
		MovableFloor,
		Transport,
		Billboard,
		FireTrap,
		Hole,
		MovableBox,
		LauncherTowardsUp,
		LauncherTowardsDown,
		LauncherTowardsLeft,
		LauncherTowardsRight
	}

	public abstract class MapItem : MonoBehaviour {

		public string audioClipName;

		// 地图物品状态变化之后是否可以行走
		public bool walkableAfterChangeStatus;

		protected Animator mapItemAnimator;

		protected SpriteRenderer mapItemRenderer;

		protected CallBack animEndCallBack;

		public MapItemType mapItemType;

		protected BoxCollider2D bc2d;

		protected virtual void Awake(){

			mapItemAnimator = GetComponent<Animator> ();

			mapItemRenderer = GetComponent<SpriteRenderer> ();

			bc2d = GetComponent<BoxCollider2D> ();

//			InitMapItem ();

		}

		public abstract void InitMapItem ();

		public void SetSortingOrder(int order){
			mapItemRenderer.sortingOrder = order;
		}


//		public void SetSortingLayer(string layerName){
//			mapItemRenderer.sortingLayerName = layerName;
//		}

		/// <summary>
		/// 地图物品被破坏或开启
		/// </summary>
		/// <param name="cb">Cb.</param>
		public void UnlockOrDestroyMapItem(CallBack cb){

			animEndCallBack = cb;

			// 播放对应动画
			mapItemAnimator.SetTrigger ("Play");

			StartCoroutine ("ResetMapItemOnAnimFinished");
		}

		/// <summary>
		/// 动画结束后重置地图物品
		/// </summary>
		/// <returns>The map item on animation finished.</returns>
		protected IEnumerator ResetMapItemOnAnimFinished(){

			float animTime = mapItemAnimator.GetCurrentAnimatorStateInfo (0).normalizedTime;

			while (animTime < 1) {

				yield return null;

				animTime = mapItemAnimator.GetCurrentAnimatorStateInfo (0).normalizedTime;

			}

			// 如果开启或破坏后是可以行走的，动画结束后将包围盒设置为not enabled
			if (walkableAfterChangeStatus) {
				GetComponent<BoxCollider2D> ().enabled = false;
			}

			AnimEnd ();

		}

		protected virtual void AnimEnd (){
			if (animEndCallBack != null) {
				animEndCallBack ();
			}
		}
	}


}
