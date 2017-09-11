using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;



namespace WordJourney
{
	public class ProduceView : MonoBehaviour {

		public Button[] itemTypeButtons;

		public GameObject itemDetailsModel;

		public Transform allItemsContainer;

		public Transform produceViewContainer;

		private InstancePool itemDetailsPool;

		public Transform[] charactersOwned;

		public Transform producePlane;

		private List<Sprite> itemSprites;

		public Transform charactersContainer;

		private int currentSelectItemIndex;

		private Sprite typeBtnNormalSprite;
		private Sprite typeBtnSelectedSprite;

		// 初始化制造界面
		public void SetUpProduceView(List<Sprite> itemSprites){

			this.itemSprites = itemSprites;

			itemDetailsPool = InstancePool.GetOrCreateInstancePool ("ItemDetailsPool");

			typeBtnNormalSprite = GameManager.Instance.allUIIcons.Find (delegate(Sprite obj) {
				return obj.name == "typeButtonNormal";
			});

			typeBtnSelectedSprite = GameManager.Instance.allUIIcons.Find (delegate(Sprite obj) {
				return obj.name == "typeButtonSelected";
			});

		}

		// 初始化物品图鉴
		public void SetUpItemDetailsPlane(List<ItemModel> itemModelsOfCurrentType, int buttonIndex){

			itemDetailsPool.AddChildInstancesToPool (allItemsContainer);

			Sprite itemTypeButtonNormalIcon = typeBtnNormalSprite;

			Sprite itemTypeButtonSelectedIcon = typeBtnSelectedSprite;

			for (int i = 0; i < itemTypeButtons.Length; i++) {

				Image buttonImage = itemTypeButtons [i].GetComponent<Image> ();
				buttonImage.sprite = i == buttonIndex ? itemTypeButtonSelectedIcon : itemTypeButtonNormalIcon;

				buttonImage.SetNativeSize ();

			}


			for (int i = 0; i < itemModelsOfCurrentType.Count; i++) {

				ItemModel itemModel = itemModelsOfCurrentType [i];

				Transform itemDetails = itemDetailsPool.GetInstance<Transform> (itemDetailsModel, allItemsContainer);

				Image itemIcon = itemDetails.FindChild ("ItemIcon").GetComponent<Image>();

				Text itemName = itemDetails.FindChild ("ItemName").GetComponent<Text> ();

				Text itemDescText = itemDetails.FindChild ("ItemDescText").GetComponent<Text> ();

				Text itemPropertiesText = itemDetails.FindChild ("ItemPropertiesText").GetComponent<Text> ();

				Button produceButton = itemDetails.FindChild ("ProduceButton").GetComponent<Button> ();

				itemIcon.sprite = itemSprites.Find (delegate(Sprite obj) {
					return obj.name == itemModel.spriteName;
				});

				if (itemIcon.sprite != null) {
					itemIcon.enabled = true;
				}

				itemName.text = itemModel.itemName;

				itemDescText.text = itemModel.itemDescription;

				if (itemModel.itemType == ItemType.Equipment) {
					Equipment equipment = new Equipment (itemModel,ItemQuality.Random);
					itemPropertiesText.text = equipment.potentialPropertiesString;
				}

				produceButton.onClick.RemoveAllListeners ();

				produceButton.onClick.AddListener (delegate() {
					GetComponent<ProduceViewController>().OnGenerateButtonClick(itemModel);
				});

			}

		}



		public void SetUpCharactersPlane(){

			Player player = Player.mainPlayer;

			for (int i = 0; i < charactersOwned.Length; i++) {

				Text characterCount = charactersOwned [i].FindChild("Count").GetComponent<Text>();

				characterCount.text = player.charactersCount [i].ToString ();

			}

			charactersContainer.gameObject.SetActive (true);

		}

		public void OnQuitCharactersPlane(){

			for (int i = 0; i < charactersOwned.Length; i++) {

				Text characterCount = charactersOwned [i].FindChild("Count").GetComponent<Text>();

				characterCount.text = string.Empty;

			}
				
			charactersContainer.gameObject.SetActive (false);

		}

		public void QuitProduceView(CallBack cb){

			produceViewContainer.GetComponent<Image> ().color = new Color (0, 0, 0, 0);

			float offsetY = GetComponent<CanvasScaler> ().referenceResolution.y;

			producePlane.DOLocalMoveY (-offsetY, 0.5f).OnComplete (() => {
				
				cb();

			});

		}
	}
}
