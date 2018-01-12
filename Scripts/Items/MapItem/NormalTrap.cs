using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class NormalTrap : Trap {

		public int lifeLose;

		// 陷阱打开状态的图片
		public Sprite trapOnSprite;
		// 陷阱关闭状态的图片
		public Sprite trapOffSprite;


		public override void InitMapItem ()
		{
			bc2d.enabled = true;
			SetSortingOrder (-(int)transform.position.y);
		}

		public override void AddToPool (InstancePool pool)
		{
			gameObject.SetActive (false);
			bc2d.enabled = false;
			pool.AddInstanceToPool (this.gameObject);
		}



		public override void SetTrapOn ()
		{
			mapItemRenderer.sprite = trapOnSprite;
			isTrapOn = true;
		}

		public override void SetTrapOff ()
		{
			mapItemRenderer.sprite = trapOffSprite;
			isTrapOn = false;
		}


		public override void OnTriggerEnter2D (Collider2D col)
		{

			if (!isTrapOn) {
				return;
			}

			GameManager.Instance.soundManager.PlayMapEffectClips(audioClipName);

			BattleAgentController ba = col.GetComponent<BattleAgentController> ();

			ba.propertyCalculator.InstantPropertyChange (ba, PropertyType.Health, -lifeLose, false);

		}

	}
}
