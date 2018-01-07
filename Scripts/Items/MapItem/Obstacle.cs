using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class Obstacle : MapItem {

		public string destroyToolName;


		/// <summary>
		/// 初始化障碍物
		/// </summary>
		public override void InitMapItem ()
		{
//			gameObject.SetActive (true);
//			bc2d.enabled = true;
//			mapItemAnimator.enabled = true;
			mapItemAnimator.SetBool ("Play",false);
			SetSortingOrder (-(int)transform.position.y);
		}
			
		public override void AddToPool(InstancePool pool){
			gameObject.SetActive (false);
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

			// 播放对应动画
			mapItemAnimator.SetBool ("Play",true);

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
