using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDetailView : MonoBehaviour {

	public Text itemName;
	public Text itemQuality;
	public Image itemIcon;

	public Image[] arrowsImages;
	public Text[] itemDetailPropTexts;
	public Text[] itemDetailPropDifTexts;

	public Button equipButton;

	public Transform propertiesPlane;
	public Transform detailDescText;

	public void SetUpItemDetailView(Item item,BagViewController bagViewCtr){

		if (item.itemType == ItemType.Consumables) {
			
			itemName.text = item.itemName;
			itemQuality.text = "品质: -";
			itemIcon.sprite = GameManager.Instance.allItemSprites.Find (delegate(Sprite obj) {
				return obj.name == item.spriteName;
			});
			if (itemIcon.sprite != null) {
				itemIcon.enabled = true;
			}
			detailDescText.GetComponent<Text> ().text = item.GetItemPropertiesString ();

			equipButton.onClick.RemoveAllListeners ();

			equipButton.onClick.AddListener (delegate() {
				bagViewCtr.EquipItem(item);	
			});

			detailDescText.gameObject.SetActive (true);

			return;
		}


		itemName.text = item.itemName;

		itemQuality.text = item.GetItemQualityString ();

		itemIcon.sprite = GameManager.Instance.allItemSprites.Find (delegate(Sprite obj) {
			return obj.name == item.spriteName;
		});
		if (itemIcon.sprite != null) {
			itemIcon.enabled = true;
		}

		Sprite arrowSprite = GameManager.Instance.allUIIcons.Find (delegate(Sprite obj) {
			return obj.name == "arrowIcon";
		});

		Item compareItem = null;
		int[] itemProperties = null;
		int[] itemPropertiesDif = null;


		switch (item.itemType) {
		case ItemType.Weapon:
			compareItem = Player.mainPlayer.allEquipedItems [0];
			break;
		case ItemType.Amour:
			compareItem = Player.mainPlayer.allEquipedItems [1];
			break;
		case ItemType.Shoes:
			compareItem = Player.mainPlayer.allEquipedItems [2];
			break;
		default:
			break;
		}

		if (compareItem == null) {
			compareItem = new Item ();
		}

		itemProperties = new int[] {
			item.attackGain,
			item.magicGain,
			item.amourGain,
			item.magicResistGain,
			item.critGain,
			item.agilityGain
		};
			
		itemPropertiesDif = new int[] {
			item.attackGain - compareItem.attackGain,
			item.magicGain - compareItem.magicGain,
			item.amourGain - compareItem.amourGain,
			item.magicResistGain - compareItem.magicResistGain,
			item.critGain - compareItem.critGain,
			item.agilityGain - compareItem.agilityGain
		};



		for (int i = 0; i < itemDetailPropTexts.Length; i++) {
			
			Text propText = itemDetailPropTexts [i];
			propText.text = itemProperties [i].ToString ();


			Image arrowImage = arrowsImages [i];
			Text propDifText = itemDetailPropDifTexts [i];

//			arrowImage.sprite = arrowSprite;

			if (itemPropertiesDif [i] < 0) {
				arrowImage.sprite = arrowSprite;
				arrowImage.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, 180));
				arrowImage.color = Color.red;
				arrowImage.enabled = true;
				propDifText.text = "<color=red>" + (-itemPropertiesDif [i]).ToString() + "</color>";
			} else if (itemPropertiesDif [i] == 0 && itemProperties[i] > 0) {
				arrowImage.sprite = arrowSprite;
				arrowImage.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, -90));
				arrowImage.enabled = true;
				propDifText.text = string.Empty;
			} else if(itemPropertiesDif [i] > 0){
				arrowImage.sprite = arrowSprite;
				arrowImage.color = Color.green;
				arrowImage.enabled = true;
				propDifText.text = "<color=green>" + itemPropertiesDif [i].ToString() + "</color>";
			}

		}


		equipButton.onClick.RemoveAllListeners ();

		equipButton.onClick.AddListener (delegate() {
			bagViewCtr.EquipItem(item);	
		});

		propertiesPlane.gameObject.SetActive (true);

	}

	public void ResetItemDetail(){

		itemName.text = string.Empty;
		itemQuality.text = string.Empty;
		itemIcon.sprite = null;
		itemIcon.enabled = false;

		detailDescText.GetComponent<Text> ().text = string.Empty;
		detailDescText.gameObject.SetActive (false);



		for (int i = 0; i < itemDetailPropTexts.Length; i++) {

			itemDetailPropTexts [i].text = string.Empty;

			arrowsImages [i].sprite = null;
			arrowsImages [i].transform.localRotation = Quaternion.Euler (Vector3.zero);
			arrowsImages [i].color = Color.white;
			arrowsImages [i].enabled = false;

			itemDetailPropDifTexts [i].text = string.Empty;
		}
		propertiesPlane.gameObject.SetActive (false);

	}

}
