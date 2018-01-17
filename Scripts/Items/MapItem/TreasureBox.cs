using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	public enum TreasureBoxType{
		NormalTreasureBox,
		LockedTreasureBox
	}

	public class TreasureBox: MapItem {

		// 地图物品状态变化之后是否可以行走
		public bool walkableAfterChangeStatus;

		// 开启所需物品的名称
		public string unlockItemName;

		// 是否有锁
		public bool locked;

		public TreasureBoxType tbType;

		// 奖励的物品
		public Item rewardItem;

		public Animator mapItemAnimator;

		/// <summary>
		/// 初始化箱子类道具
		/// </summary>
		public override void InitMapItem ()
		{
			bc2d.enabled = true;
			mapItemAnimator.gameObject.SetActive (false);
			mapItemRenderer.enabled = true;
			int sortingOrder = -(int)transform.position.y;
			SetSortingOrder (sortingOrder);
			SetAnimationSortingOrder (sortingOrder);
			if (tbType == TreasureBoxType.LockedTreasureBox) {
				locked = true; 
			}
		}

		private void SetAnimationSortingOrder(int order){
			mapItemAnimator.GetComponent<SpriteRenderer> ().sortingOrder = order;
		}

		public override void AddToPool (InstancePool pool)
		{
			bc2d.enabled = false;
			pool.AddInstanceToPool (this.gameObject);
		}


		/// <summary>
		/// 地图物品被破坏或开启
		/// </summary>
		/// <param name="cb">Cb.</param>
		public void UnlockTreasureBox(CallBack cb){

			animEndCallBack = cb;

			SoundManager.Instance.PlayAudioClip ("MapEffects/" + audioClipName);

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

			AnimatorStateInfo stateInfo = mapItemAnimator.GetCurrentAnimatorStateInfo (0);

			while (stateInfo.normalizedTime < 1) {

				yield return null;

				stateInfo = mapItemAnimator.GetCurrentAnimatorStateInfo (0);

			}

			// 如果开启或破坏后是可以行走的，动画结束后将包围盒设置为not enabled
			if (walkableAfterChangeStatus) {
				// 瓦罐和木桶解锁之后在图层内层级下调一级低层级（防止人物在上面走的时候遮挡住人物）
				int sortingOrder = mapItemRenderer.sortingOrder - 1;
				SetSortingOrder (sortingOrder);
				bc2d.enabled = false;
			}

//			Debug.Log ("anim end " + mapItemAnimator.GetBool ("Play"));

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
