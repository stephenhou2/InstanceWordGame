using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{

	public enum MapItemType{
		None,
		Obstacle,
		TreasureBox,
		Trap,
		TrapSwitch
	}

	public abstract class MapItem : MonoBehaviour {

		public string mapItemName;

		public bool walkableAfterUnlockOrDestroy;

		protected Animator mapItemAnimator;

		protected CallBack animEndCallBack;

		public MapItemType mapItemType;

		protected BoxCollider2D bc2d;

		protected virtual void Awake(){

			mapItemAnimator = GetComponent<Animator> ();

			bc2d = GetComponent<BoxCollider2D> ();

//			SetUpItemIcon ();

		}

		public abstract void InitMapItem ();

//		private void SetUpItemIcon(){
//			
//			if (originSprite != null) {
//
//				SpriteRenderer sr = transform.Find ("MapItemIcon").GetComponent<SpriteRenderer> ();
//
//				sr.sprite = originSprite;
//
//				sr.enabled = true;
//
//			}
//
//		}

		public void UnlockOrDestroyMapItem(CallBack cb){

			animEndCallBack = cb;

			mapItemAnimator.SetBool ("Play", true);

			StartCoroutine ("ResetMapItemOnAnimFinished");
		}

		protected IEnumerator ResetMapItemOnAnimFinished(){

			float animTime = mapItemAnimator.GetCurrentAnimatorStateInfo (0).normalizedTime;

			while (animTime < 1) {

				yield return null;

				animTime = mapItemAnimator.GetCurrentAnimatorStateInfo (0).normalizedTime;

			}

			if (walkableAfterUnlockOrDestroy) {
				GetComponent<BoxCollider2D> ().enabled = false;
			}

//			SpriteRenderer sr = transform.GetComponent<SpriteRenderer> ();
//
//			sr.sprite = unlockedOrDestroyedSprite;

			AnimEnd ();

		}

		protected virtual void AnimEnd (){
			if (animEndCallBack != null) {
				animEndCallBack ();
			}
		}
	}


}
