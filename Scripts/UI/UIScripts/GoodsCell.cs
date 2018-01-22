using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class GoodsCell : MonoBehaviour {

		public Image goodsIcon;
		public Image selectedIcon;
		public Text goodsPrice;


		public void SetUpGoodsCell(Item goods){

			goodsPrice.text = goods.price.ToString ();

			Sprite itemSprite = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
				return obj.name == goods.spriteName;
			});

			goodsIcon.sprite = itemSprite;

			goodsIcon.enabled = itemSprite != null;

			selectedIcon.enabled = false;

		}


		public void SetSelection(bool selected){
			selectedIcon.enabled = selected;
		}


	}
}
