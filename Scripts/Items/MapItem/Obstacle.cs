using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class Obstacle : MapItem {

		public string destroyToolName;

		public Animator mapItemAnimator;


		/// <summary>
		/// 初始化障碍物
		/// </summary>
		public override void InitMapItem ()
		{
			bc2d.enabled = true;
			mapItemAnimator.gameObject.SetActive (false);
			mapItemRenderer.enabled = true;
			int sortingOrder = -(int)transform.position.y;
			SetSortingOrder (sortingOrder);
			SetAnimationSortingOrder (sortingOrder);
		}

		private void SetAnimationSortingOrder(int order){
			mapItemAnimator.GetComponent<SpriteRenderer> ().sortingOrder = order;
		}
			
		public override void AddToPool(InstancePool pool){
			bc2d.enabled = false;
			pool.AddInstanceToPool (this.gameObject);
		}

		/// <summary>
		/// 地图物品被破坏或开启
		/// </summary>
		/// <param name="cb">Cb.</param>
		public void DestroyObstacle(CallBack cb){

			animEndCallBack = cb;

			// 如果开启或破坏后是可以行走的，动画结束后将包围盒设置为not enabled
			GetComponent<BoxCollider2D> ().enabled = false;

			mapItemRenderer.enabled = false;

			mapItemAnimator.gameObject.SetActive (true);
			// 播放对应动画
			mapItemAnimator.SetTrigger ("Play");

			StartCoroutine ("ResetMapItemOnAnimFinished");
		}

		/// <summary>
		/// 动画结束后重置地图物品
		/// </summary>
		/// <returns>The map item on animation finished.</returns>
		protected IEnumerator ResetMapItemOnAnimFinished(){

			yield return null;

			float animTime = mapItemAnimator.GetCurrentAnimatorStateInfo (0).normalizedTime;

			while (animTime < 1) {

				yield return null;

				animTime = mapItemAnimator.GetCurrentAnimatorStateInfo (0).normalizedTime;

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
