using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class Plant : MapItem {

		public Item attachedItem;

		public Animator attachedBlink;

		public override void InitMapItem ()
		{
//			gameObject.SetActive(true);
//			bc2d.enabled = true;
//			attachedBlink.enabled = true;
			mapItemRenderer.sprite = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
				return obj.name == attachedItem.spriteName;
			});
			SetSortingOrder (-(int)transform.position.y);
		}

		public override void AddToPool(InstancePool pool){

			attachedItem = null;

//			TransformManager.FindTransform ("ExploreManager").GetComponent<MapGenerator> ().AddMapItemInPool (this.transform);

			gameObject.SetActive (false);

			pool.AddInstanceToPool (this.gameObject);
		}

	}
}
