using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class Plant : MapItem {

		public Item attachedItem;

		public override void InitMapItem ()
		{
			bc2d.enabled = true;
			mapItemRenderer.sprite = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
				return obj.name == attachedItem.spriteName;
			});
		}

		public void AddPlantToPool(){

			attachedItem = null;

			gameObject.SetActive (false);

			TransformManager.FindTransform ("ExploreManager").GetComponent<MapGenerator> ().AddMapItemInPool (this.transform);


		}

	}
}
