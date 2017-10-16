using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	public class TreasureBox: MapItem {


		public string unlockItemName;
		public bool unlocked;


//		private Item[] mRewardItems;
//		public Item[] rewardItems{
//
//			get{
//				return mRewardItems;
//			}
//			set{
//				mRewardItems = value;
//			}
//		}

		public Item[] rewardItems;

		protected override void Awake ()
		{
			base.Awake ();
			this.mapItemType = MapItemType.TreasureBox;
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
//				int index = Random.Range (0, GameManager.Instance.dataCenter.allMapSprites.Count);
//
//				Sprite s = GameManager.Instance.dataCenter.allMapSprites [index];
//
//				spriteName = s.name;
//
//			} while(!spriteName.Contains ("item"));
//
//			string itemName = spriteName.Split ('_')[1];
//
//			string originSpriteName = "item_" + itemName + "_origin";
//
//			originSprite = GameManager.Instance.dataCenter.allMapSprites.Find (delegate(Sprite s) {
//				return s.name == originSpriteName;
//			});
//
//			string destroyedSpriteName = "item_" + itemName + "_unlocked";
//
//			unlockedSprite = GameManager.Instance.dataCenter.allMapSprites.Find (delegate(Sprite s) {
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
