using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class CraftingRecipesHUD : MonoBehaviour {

		public Image craftingItemIcon;
		public Text craftingItemName;
		public Text craftingItemDescription;

		public Image horizontalLine;

		public Transform recipesItemModel;
		private InstancePool recipesItemPool;
		public Transform recipesItemsContainer;

		private bool quitWhenClickBackground = true;
		private CallBack quitCallBack;
		private CallBack craftCallBack;

		public Button craftButton;

		[HideInInspector]public CraftingRecipes craftingRecipes;


		public void InitCraftingRecipesHUD(bool quitWhenClickBackground,CallBack quitCallBack,CallBack craftCallBack){

			this.quitWhenClickBackground = quitWhenClickBackground;
			this.quitCallBack = quitCallBack;
			this.craftCallBack = craftCallBack;

			recipesItemPool = InstancePool.GetOrCreateInstancePool ("RecipesItemPool", CommonData.poolContainerName);

		}

		public void SetUpCraftingRecipesHUD(Item item){
			
			craftingRecipes = item as CraftingRecipes;

			ItemModel craftingItem = GameManager.Instance.gameDataCenter.allItemModels.Find (delegate(ItemModel obj) {
				return obj.itemId == craftingRecipes.craftItemId;
			});

			Sprite craftingItemSprite = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
				return obj.name == craftingItem.spriteName;
			});

			craftingItemIcon.sprite = craftingItemSprite;

			craftingItemIcon.enabled = craftingItemSprite != null;

			craftingItemName.text = craftingItem.itemName;

			craftingItemDescription.text = craftingItem.itemDescription;

			ItemModel.ItemInfoForProduce[] itemInfosForProduce = craftingItem.itemInfosForProduce;

			SetUpRecipesItems (itemInfosForProduce);

			float middleLineWidth = GetMiddleLineWidth (itemInfosForProduce.Length);

			horizontalLine.rectTransform.sizeDelta = new Vector2(middleLineWidth,3);

			gameObject.SetActive (true);



		}

		private float GetMiddleLineWidth(int itemCount){

			HorizontalLayoutGroup layout = recipesItemsContainer.GetComponent<HorizontalLayoutGroup> ();

			float recipesItemWidth = recipesItemModel.GetComponent<RectTransform> ().rect.width;

			return (itemCount - 1) * (layout.spacing + recipesItemWidth);

		}

		private void SetUpRecipesItems(ItemModel.ItemInfoForProduce[] itemInfosForProduce){

			recipesItemPool.AddChildInstancesToPool(recipesItemsContainer);

			for (int i = 0; i < itemInfosForProduce.Length; i++) {

				ItemModel item = GameManager.Instance.gameDataCenter.allItemModels.Find (delegate(ItemModel obj) {
					return obj.itemId == itemInfosForProduce [i].itemId;
				});

				int itemCountForProduce = itemInfosForProduce [i].itemCount;

				Transform recipesItem = recipesItemPool.GetInstance<Transform> (recipesItemModel.gameObject, recipesItemsContainer);

				Image recipesItemIcon = recipesItem.Find ("RecipesItemIcon").GetComponent<Image> ();
				Text recipesItemName = recipesItem.Find ("RecipesItemName").GetComponent<Text> ();
				Text recipesItemEnoughText = recipesItem.Find ("RecipesItemEnoughText").GetComponent<Text> ();

				Sprite itemSprite = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
					return obj.name == item.spriteName;
				});

				recipesItemIcon.sprite = itemSprite;

				recipesItemIcon.enabled = itemSprite != null;

				recipesItemName.text = item.itemName;

				List<Item> sameItemsInBag = Player.mainPlayer.allItemsInBag.FindAll (delegate(Item obj) {
					return obj.itemId == item.itemId;
				});

				int itemInBagCount = 0;

				for (int j = 0; j < sameItemsInBag.Count; j++) {

					itemInBagCount += sameItemsInBag [j].itemCount;

				}


				bool recipeItemEnough = itemInBagCount >= itemCountForProduce;

				string color = recipeItemEnough ? "green" : "red";

				recipesItemEnoughText.text = string.Format ("<color={0}>{1}/{2}</color>", color, itemInBagCount, itemCountForProduce);

				craftButton.interactable = recipeItemEnough;

			}

		}

		public void OnBackgroundClick(){
			if (quitWhenClickBackground) {
				QuitCraftingRecipesHUD ();
			}
		}


		public void QuitCraftingRecipesHUD(){

			if (quitCallBack != null) {
				quitCallBack ();
			}

			gameObject.SetActive (false);

		}


		public void OnCraftButtonClick(){

			if (craftCallBack != null) {
				craftCallBack ();
			}

			QuitCraftingRecipesHUD ();

		}

	}
}
