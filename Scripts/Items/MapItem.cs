using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	public class MapItem : MonoBehaviour {


		public int mapItemId;

		public string unlockItemName;

		// 默认一个宝箱只能开出一个物品
		private Item[] mRewardItems;
		[HideInInspector]public Item[] rewardItems{

			get{
				return mRewardItems;
			}
			set{
				mRewardItems = value;
				InitialiseSprites ();
			}


		}

		[HideInInspector]public Sprite originSprite;
		[HideInInspector]public Sprite destroyedSprite;

		private Animator mapItemDestroyAnimator;

		[HideInInspector] public CallBack<Item> animEndCallBack;

		private void InitialiseSprites(){

			string spriteName = string.Empty;

			do {
				int index = Random.Range (0, GameManager.Instance.allMapSprites.Count);

				Sprite s = GameManager.Instance.allMapSprites [index];

				spriteName = s.name;

			} while(!spriteName.Contains ("item"));

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

		public void PlayDestroyAnim(CallBack<Item> cb,Item[] rewardItems){

			animEndCallBack = cb;

			mapItemDestroyAnimator.SetBool ("Destroy", true);

		}


	}
}
