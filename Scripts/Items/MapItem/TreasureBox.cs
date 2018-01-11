using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	public class TreasureBox: MapItem {

		// 地图物品状态变化之后是否可以行走
		public bool walkableAfterChangeStatus;

		// 开启所需物品的名称
		public string unlockItemName;

		// 是否有锁
		public bool locked;

		// 奖励的物品数组
//		public Item[] rewardItems;

		// 奖励的物品
		public Item rewardItem;



		/// <summary>
		/// 初始化箱子类道具
		/// </summary>
		public override void InitMapItem ()
		{
//			gameObject.SetActive (true);
//			bc2d.enabled = true;
//			mapItemAnimator.enabled = true;
			bc2d.enabled = true;
			mapItemAnimator.SetBool ("Play",false);
			SetSortingOrder (-(int)transform.position.y);
		}

		public override void AddToPool (InstancePool pool)
		{
			gameObject.SetActive (false);
//			bc2d.enabled = false;
			pool.AddInstanceToPool (this.gameObject);
		}


		/// <summary>
		/// 地图物品被破坏或开启
		/// </summary>
		/// <param name="cb">Cb.</param>
		public void UnlockTreasureBox(CallBack cb){

			animEndCallBack = cb;

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

			AnimatorStateInfo stateInfo = mapItemAnimator.GetCurrentAnimatorStateInfo (0);

			while (stateInfo.normalizedTime < 1) {

				yield return null;

				stateInfo = mapItemAnimator.GetCurrentAnimatorStateInfo (0);

			}

			// 如果开启或破坏后是可以行走的，动画结束后将包围盒设置为not enabled
			if (walkableAfterChangeStatus) {
				bc2d.enabled = false;
			}

			AnimEnd ();

		}

		protected void AnimEnd (){
			locked = false;
			if (animEndCallBack != null) {
				animEndCallBack ();
			}
		}



	}
}
