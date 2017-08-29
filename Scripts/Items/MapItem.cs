using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	public class MapItem : MonoBehaviour {


		public int mapItemId;

		public string unlockItemName;

		public bool unlocked;


		private Item[] mRewardItems;
		public Item[] rewardItems{

			get{
				return mRewardItems;
			}
			set{
				mRewardItems = value;
				InitialiseSprites ();
			}


		}

		[HideInInspector]public Sprite originSprite;
		[HideInInspector]public Sprite unlockedSprite;

		private Animator mapItemAnimator;

		[HideInInspector] public CallBack<Item> animEndCallBack;

		void Awake(){

			mapItemAnimator = GetComponent<Animator> ();

		}


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

			string destroyedSpriteName = "item_" + itemName + "_unlocked";
			
			unlockedSprite = GameManager.Instance.allMapSprites.Find (delegate(Sprite s) {
				return s.name == destroyedSpriteName;
			});


			if (originSprite != null) {
				SpriteRenderer sr = transform.FindChild("MapItemIcon").GetComponent<SpriteRenderer> ();
				sr.sprite = originSprite;
				sr.enabled = true;
			}

		}

		public void UnlockMapItem(CallBack<Item> cb,Item[] rewardItems){

			animEndCallBack = cb;

			mapItemAnimator.SetBool ("Unlock", true);

		}


	}
}
