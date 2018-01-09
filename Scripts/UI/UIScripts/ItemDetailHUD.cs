using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class ItemDetailHUD : MonoBehaviour {


		public Transform itemDetailsContainer;
		public Text itemName;
		public Text itemType;
		public Text itemDescription;
		public Text itemProperties;
		public Image itemIcon;

		private bool quitWhenClickBackground;
		private CallBack quitCallBack;


		/// <summary>
		/// quitWhenClickBackground 表示点击背景空白处是否可以退出物品详细页
		/// quitCallBack回调是在关闭物品详细页的逻辑中执行，所以回调中 不要 再次 关闭物品详细页
		/// </summary>
		public void InitItemDetailHUD(bool quitWhenClickBackground,CallBack quitCallBack){
			this.quitWhenClickBackground = quitWhenClickBackground;
			this.quitCallBack = quitCallBack;
		}

		public void SetUpItemDetailHUD(Item item){

			Sprite itemSprite = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
				return obj.name == item.spriteName;
			});

			itemIcon.sprite = itemSprite;

			itemIcon.enabled = itemSprite != null;

			itemName.text = item.itemName;

			itemType.text = item.GetItemTypeString ();

			itemProperties.text = item.itemDescription;

			itemDescription.text = item.itemDescription;

			gameObject.SetActive (true);

		}

		public void OnBackgroundClicked(){
			if (quitWhenClickBackground) {
				QuitItemDetailHUD ();
			}
		}

		public void QuitItemDetailHUD(){
			if (quitCallBack != null) {
				quitCallBack ();
			}
			gameObject.SetActive (false);
		}

	}
}
