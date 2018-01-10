using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class UnlockScrollDetailHUD : MonoBehaviour {

		public Image unlockedItemIcon;
		public Text unlockedItemName;
		public Text statusText;
		public Text unlockedItemDescription;

		private bool quitWhenClickBackground = true;
		private CallBack quitCallBack;
		private CallBack unlockCallBack;
		private CallBack resolveCallBack;

		public Button unlockButton;
		public Button resolveButton;

		[HideInInspector]public UnlockScroll unlockScroll;

		/// <summary>
		/// quitWhenClickBackground 表示点击背景空白处是否可以退出物品详细页
		/// quitCallBack回调是在关闭物品详细页的逻辑中执行，所以回调中 不要 再次 关闭物品详细页
		/// </summary>
		public void InitUnlockScrollDetailHUD(bool quitWhenClickBackground,CallBack quitCallBack,CallBack unlockCallBack,CallBack resolveCallBack){
			this.quitWhenClickBackground = quitWhenClickBackground;
			this.quitCallBack = quitCallBack;
			this.unlockCallBack = unlockCallBack;
			this.resolveCallBack = resolveCallBack;
		}

		public void SetUpUnlockScrollDetailHUD(Item item){

			UnlockScroll unlockScrollInBag = item as UnlockScroll;

			this.unlockScroll = unlockScrollInBag;

			ItemModel itemModel = GameManager.Instance.gameDataCenter.allItemModels.Find (delegate(ItemModel obj) {
				return obj.itemId == unlockScrollInBag.unlockedItemId;
			});

			Sprite itemSprite = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
				return obj.name == itemModel.spriteName;
			});

			unlockedItemIcon.sprite = itemSprite;

			unlockedItemIcon.enabled = itemSprite != null;

			unlockedItemName.text = itemModel.itemName;

			bool hasScrollUnlocked = false;

			List<UnlockScroll> sameUnlockScrollsInBag = Player.mainPlayer.allUnlockScrollsInBag.FindAll (delegate(UnlockScroll obj) {
				return obj.itemId == unlockScroll.itemId;
			});

			for (int i = 0; i < sameUnlockScrollsInBag.Count; i++) {
				if (sameUnlockScrollsInBag [i].unlocked) {
					hasScrollUnlocked = true;
					break;
				}
			}
				
			statusText.text = hasScrollUnlocked ? "<color=green>已解锁</color>" : "<color=red>未解锁</color>";

			unlockedItemDescription.text = itemModel.itemDescription;

			unlockButton.gameObject.SetActive (!hasScrollUnlocked);
			resolveButton.gameObject.SetActive (hasScrollUnlocked);

			gameObject.SetActive (true);

		}

		public void OnBackgroundClicked(){
			if (quitWhenClickBackground) {
				QuitUnlockScrollDetailHUD ();
			}
		}

		public void OnUnlockButtonClick(){

			if (unlockCallBack != null) {
				unlockCallBack ();
			}

			QuitUnlockScrollDetailHUD ();
		}

		public void OnResolveButtonClick(){

			if (resolveCallBack != null) {
				resolveCallBack ();
			}

			QuitUnlockScrollDetailHUD ();
		}
			

		public void QuitUnlockScrollDetailHUD(){

			if (quitCallBack != null) {
				quitCallBack ();
			}

			unlockButton.gameObject.SetActive (false);
			resolveButton.gameObject.SetActive (false);

			gameObject.SetActive (false);

		}


	}
}
