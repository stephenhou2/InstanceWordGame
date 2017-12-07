using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class Trap : MapItem {

		// 陷阱已经关闭
		public bool trapOff;

		// 陷阱的原始图片
		public Sprite originSprite;
		// 陷阱关闭后的图片
		public Sprite unlockedOrDestroyedSprite;

		protected override void Awake ()
		{
			base.Awake ();
			this.mapItemType = MapItemType.Trap;
		}

		/// <summary>
		/// 初始化陷阱
		/// </summary>
		public override void InitMapItem ()
		{
			bc2d.enabled = true;
			GetComponent<SpriteRenderer> ().sprite = originSprite;
			trapOff = false;
		}

		/// <summary>
		/// 陷阱被触发的响应方法
		/// </summary>
		/// <param name="col">Col.</param>
		public void OnTriggerEnter2D(Collider2D col){

			if (trapOff) {
				return;
			}

			GetComponent<BoxCollider2D> ().enabled = false;

			GameManager.Instance.soundManager.PlayMapEffectClips(mapItemName);

//			GameManager.Instance.soundManager.PlayClips (
//				GameManager.Instance.gameDataCenter.allExploreAudioClips, 
//				SoundDetailTypeName.Map, 
//				mapItemName);

			col.GetComponent<BattlePlayerController> ().trapTriggered = this;

			Debug.Log ("触发陷阱");

		}

		/// <summary>
		/// 关闭陷阱
		/// </summary>
		public void OnSwitchOff(){

			this.trapOff = true;

			transform.GetComponent<SpriteRenderer> ().sprite = unlockedOrDestroyedSprite;

		}

	}
}
