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
			bc2d.enabled = true;
			isDroppable = false;
			SetSortingOrder (-(int)transform.position.y);
		}

		public override void AddToPool(InstancePool pool){

//			attachedItem = null;

			bc2d.enabled = false;

			gameObject.SetActive (false);

			pool.AddInstanceToPool (this.gameObject);
		}

		public static Item GenerateRandomAward(){
			int awardCount = Random.Range (1, 4);
			int awardItemId = Random.Range (113, 120);
			return Item.NewItemWith (awardItemId, awardCount);
		}

	}
}
