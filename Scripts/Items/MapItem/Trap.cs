using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class Trap : MapItem {


		public int lifeLose;

		private bool triggered;

		// 陷阱已经关闭
		private bool myTrapOn = false;
		public bool trapOn{
			get{
				return myTrapOn;
			}

			set{

				myTrapOn = value;

				if (myTrapOn) {
					mapItemRenderer.sprite = trapOnSprite;
					bc2d.enabled = true;
				} else {
					mapItemRenderer.sprite = trapOffSprite;
					bc2d.enabled = false;
				}
			}

		}

		// 陷阱打开状态的图片
		public Sprite trapOnSprite;
		// 陷阱关闭状态的图片
		public Sprite trapOffSprite;



		/// <summary>
		/// 初始化陷阱
		/// </summary>
		public override void InitMapItem ()
		{
			bc2d.enabled = true;
			triggered = false;
		}

		public void ChangeTrapStatus(){

			trapOn = !trapOn;

		}


		/// <summary>
		/// 进入陷阱
		/// </summary>
		/// <param name="col">Col.</param>
		public void OnTriggerEnter2D(Collider2D col){

			triggered = !triggered;

			if (!trapOn || !triggered) {
				return;
			}

//			bc2d.enabled = false;

			GameManager.Instance.soundManager.PlayMapEffectClips(audioClipName);

			BattlePlayerController bpCtr = col.GetComponent<BattlePlayerController> ();

//			bpCtr.trapTriggered = this;

			bpCtr.LoseLifeInTrap (lifeLose);

			Debug.Log ("触发陷阱");

		}


	}
}
