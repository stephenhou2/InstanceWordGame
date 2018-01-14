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

		private bool quitWhenClickBackground = true;
		private CallBack quitCallBack;

		private float zoomInDuration = 0.2f;
		private IEnumerator zoomInCoroutine;

		/// <summary>
		/// quitWhenClickBackground 表示点击背景空白处是否可以退出物品详细页
		/// quitCallBack回调是在关闭物品详细页的逻辑中执行，所以回调中 不要 再次 关闭物品详细页
		/// </summary>
		public void InitItemDetailHUD(bool quitWhenClickBackground,CallBack quitCallBack){
			this.quitWhenClickBackground = quitWhenClickBackground;
			this.quitCallBack = quitCallBack;
		}

		public void SetUpItemDetailHUD(Item item){

			SoundManager.Instance.PlayAudioClip ("UI/sfx_UI_Paper");

			Sprite itemSprite = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
				return obj.name == item.spriteName;
			});

			itemIcon.sprite = itemSprite;

			itemIcon.enabled = itemSprite != null;

			itemName.text = item.itemName;

			itemType.text = item.GetItemTypeString ();

			itemProperties.text = item.itemDescription;

			itemDescription.text = item.itemDescription;

			itemDetailsContainer.transform.localScale = new Vector3 (0.1f, 0.1f, 1);

			gameObject.SetActive (true);

			zoomInCoroutine = ItemDetailHUDZoomIn ();

			StartCoroutine (zoomInCoroutine);

		}


		private IEnumerator ItemDetailHUDZoomIn(){

			float scale = itemDetailsContainer.transform.localScale.x;

			float zoomInSpeed = (1 - scale) / zoomInDuration;

			float lastFrameRealTime = Time.realtimeSinceStartup;

			while (scale < 1) {

				yield return null;

				scale += zoomInSpeed * (Time.realtimeSinceStartup - lastFrameRealTime);

				lastFrameRealTime = Time.realtimeSinceStartup;

				itemDetailsContainer.transform.localScale = new Vector3 (scale, scale, 1);

			}

			itemDetailsContainer.transform.localScale = Vector3.one;

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

			if (zoomInCoroutine != null) {
				StopCoroutine (zoomInCoroutine);
			}

			gameObject.SetActive (false);
		}

	}
}
