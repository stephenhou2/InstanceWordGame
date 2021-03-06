﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class UnlockedItemsView : MonoBehaviour {
		
		public Button beginSpellButton;

		private InstancePool unlockedItemsPool;
		public Transform unlockedItemModel;
		public Transform unlockedItemsContainer;

		public ItemDetailHUD itemDetailHUD;

		public Button unlockEquipmentTitle;
		public Button unlockConsumablesTitle;

		public Sprite normalSprite;
		public Sprite selectedSprite;


		public void InitUnlockedItemView(){
			unlockedItemsPool = InstancePool.GetOrCreateInstancePool ("UnlockedItemPool", CommonData.poolContainerName);
		}


		public void SetUpUnlockedItemsView(UnlockScrollType unlockScrollType){

			switch (unlockScrollType) {
			case UnlockScrollType.Equipment:
				unlockEquipmentTitle.GetComponent<Image> ().sprite = selectedSprite;
				unlockEquipmentTitle.GetComponentInChildren<Text> ().color = new Color (
					CommonData.selectedColor.x, 
					CommonData.selectedColor.y, 
					CommonData.selectedColor.z);
				unlockConsumablesTitle.GetComponent<Image> ().sprite = normalSprite;
				unlockConsumablesTitle.GetComponentInChildren<Text>().color = new Color (
					CommonData.deselectedColor.x,
					CommonData. deselectedColor.y, 
					CommonData.deselectedColor.z);
				break;
			case UnlockScrollType.Consumables:
				unlockConsumablesTitle.GetComponent<Image> ().sprite = selectedSprite;
				unlockConsumablesTitle.GetComponentInChildren<Text> ().color = new Color (
					CommonData.selectedColor.x, 
					CommonData.selectedColor.y, 
					CommonData.selectedColor.z);
				unlockEquipmentTitle.GetComponent<Image> ().sprite = normalSprite;
				unlockEquipmentTitle.GetComponentInChildren<Text>().color = new Color (
					CommonData.deselectedColor.x, 
					CommonData.deselectedColor.y, 
					CommonData.deselectedColor.z);
				break;
			}

			unlockedItemsPool.AddChildInstancesToPool(unlockedItemsContainer);

			for (int i = 0; i < Player.mainPlayer.allUnlockScrollsInBag.Count; i++) {
				UnlockScroll unlockScroll = Player.mainPlayer.allUnlockScrollsInBag [i];
				if (unlockScroll.unlocked && unlockScroll.unlockScrollType == unlockScrollType) {
					Transform unlockedItem = unlockedItemsPool.GetInstance<Transform> (unlockedItemModel.gameObject, unlockedItemsContainer);
					SetUpUnlockedItem (unlockedItem, unlockScroll);
				}
			}

			GetComponent<Canvas> ().enabled = true;

		}


		private void SetUpUnlockedItem(Transform unlockedItem,UnlockScroll unlockScroll){

			ItemModel im = GameManager.Instance.gameDataCenter.allItemModels.Find (delegate(ItemModel obj) {
				return obj.itemId == unlockScroll.unlockedItemId;
			});

			Sprite itemSprite = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
				return obj.name == im.spriteName;
			});

			Image itemIcon = unlockedItem.Find ("ItemIcon").GetComponent<Image> ();
			Text itemName = unlockedItem.Find ("ItemName").GetComponent<Text> ();

			itemIcon.sprite = itemSprite;
			itemIcon.enabled = true;

			itemName.text = im.itemName;

			unlockedItem.GetComponent<Button> ().onClick.RemoveAllListeners ();
			unlockedItem.GetComponent<Button> ().onClick.AddListener (delegate {
				Item item = Item.NewItemWith(im,1);
				GetComponent<UnlockedItemsViewController>().itemToCreate = im;
				SetUpUnlockedItemDetailHUD(item);
			});
		}

		public void QuitUnlockedItemsPlane(){

			for (int i = 0; i < unlockedItemsContainer.childCount; i++) {
				Transform unlockedItem = unlockedItemsContainer.GetChild (i);
				unlockedItem.Find ("ItemIcon").GetComponent<Image> ().enabled = false;
				unlockedItemsPool.AddInstanceToPool (unlockedItem.gameObject);
			}

		}

		public void SetUpUnlockedItemDetailHUD(Item item){
			itemDetailHUD.SetUpItemDetailHUD (item);
		}

		public void QuitUnlockedItemDetailHUD(){
			itemDetailHUD.QuitItemDetailHUD ();
		}

		public void QuitUnlockedItemsView(){

			QuitUnlockedItemDetailHUD ();

			GetComponent<Canvas> ().enabled = false;

		}

	}
}
