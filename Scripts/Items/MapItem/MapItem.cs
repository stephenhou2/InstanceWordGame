using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	// 地图物品类型枚举
	public enum MapItemType{
		None,
		Obstacle,
		TreasureBox,
		Trap,
		TrapSwitch
	}

	public abstract class MapItem : MonoBehaviour {

		// 地图物品名称
		public string mapItemName;

		// 地图物品开启或破坏之后是否可以行走
		public bool walkableAfterUnlockOrDestroy;

		protected Animator mapItemAnimator;

		protected CallBack animEndCallBack;

		public MapItemType mapItemType;

		protected BoxCollider2D bc2d;

		protected virtual void Awake(){

			mapItemAnimator = GetComponent<Animator> ();

			bc2d = GetComponent<BoxCollider2D> ();

		}

		public abstract void InitMapItem ();


		/// <summary>
		/// 地图物品被破坏或开启
		/// </summary>
		/// <param name="cb">Cb.</param>
		public void UnlockOrDestroyMapItem(CallBack cb){

			animEndCallBack = cb;

			// 播放对应动画
			mapItemAnimator.SetBool ("Play", true);

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
			if (walkableAfterUnlockOrDestroy) {
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
