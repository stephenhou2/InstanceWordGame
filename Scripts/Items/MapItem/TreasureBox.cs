using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	public class TreasureBox: MapItem {

		public string unlockItemName;
		public bool unlocked;

		public Item[] rewardItems;

		protected override void Awake ()
		{
			base.Awake ();
			this.mapItemType = MapItemType.TreasureBox;
		}

		public override void InitMapItem ()
		{
			unlocked = false;
			bc2d.enabled = true;
			mapItemAnimator.ResetTrigger ("Play");
		}

		protected override void AnimEnd ()
		{
			unlocked = true;
			base.AnimEnd ();
		}

//		private void InitialiseSprites(){
//
//			string spriteName = string.Empty;
//
//			do {
//				int index = Random.Range (0, GameManager.Instance.gameDataCenter.allMapSprites.Count);
//
//				Sprite s = GameManager.Instance.gameDataCenter.allMapSprites [index];
//
//				spriteName = s.name;
//
//			} while(!spriteName.Contains ("item"));
//
//			string itemName = spriteName.Split ('_')[1];
//
//			string originSpriteName = "item_" + itemName + "_origin";
//
//			originSprite = GameManager.Instance.gameDataCenter.allMapSprites.Find (delegate(Sprite s) {
//				return s.name == originSpriteName;
//			});
//
//			string destroyedSpriteName = "item_" + itemName + "_unlocked";
//
//			unlockedSprite = GameManager.Instance.gameDataCenter.allMapSprites.Find (delegate(Sprite s) {
//				return s.name == destroyedSpriteName;
//			});
//
//
//			if (originSprite != null) {
//				SpriteRenderer sr = transform.Find("MapItemIcon").GetComponent<SpriteRenderer> ();
//				sr.sprite = originSprite;
//				sr.enabled = true;
//			}
//		}




	}
}
