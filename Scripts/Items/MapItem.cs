using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney{
	public class MapItem : MonoBehaviour {


		public int mapItemId;

		public int unlockItemId;

		// 默认一个宝箱只能开出一个物品
		private Item mRewardItem;
		[HideInInspector]public Item rewardItem{

			get{
				return mRewardItem;
			}
			set{
				mRewardItem = value;
				InitialiseSprites ();
			}


		}

		[HideInInspector]public Sprite originSprite;
		[HideInInspector]public Sprite destroyedSprite;

		private void InitialiseSprites(){

			string spriteName = string.Empty;

			do {
				int index = Random.Range (0, GameManager.Instance.allMapSprites.Count);

				Sprite s = GameManager.Instance.allMapSprites [index];

				spriteName = s.name;

			} while(spriteName == string.Empty || spriteName.Contains ("npc"));

			string itemName = spriteName.Split ('_')[1];

			string originSpriteName = "item_" + itemName + "_origin";

			originSprite = GameManager.Instance.allMapSprites.Find (delegate(Sprite s) {
				return s.name == originSpriteName;
			});

			string destroyedSpriteName = "item_" + itemName + "_destroyed";
			
			destroyedSprite = GameManager.Instance.allMapSprites.Find (delegate(Sprite s) {
				return s.name == destroyedSpriteName;
			});

			GetComponent<SpriteRenderer> ().sprite = originSprite;

		}




	}
}
