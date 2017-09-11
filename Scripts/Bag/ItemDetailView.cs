using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



namespace WordJourney
{
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

				Consumables consumables = item as Consumables;
				
				itemName.text = consumables.itemName;

				itemIcon.sprite = GameManager.Instance.allItemSprites.Find (delegate(Sprite obj) {
					return obj.name == consumables.spriteName;
				});

				if (itemIcon.sprite != null) {
					itemIcon.enabled = true;
				}

				detailDescText.GetComponent<Text> ().text = consumables.GetItemPropertiesString ();

				detailDescText.gameObject.SetActive (true);

				return;
			}

			if (item is Equipment) {

				Equipment equipment = item as Equipment;

				itemName.text = equipment.itemName;

				itemQuality.text = equipment.GetItemQualityString ();

				itemIcon.sprite = GameManager.Instance.allItemSprites.Find (delegate(Sprite obj) {
					return obj.name == equipment.spriteName;
				});
				if (itemIcon.sprite != null) {
					itemIcon.enabled = true;
				}

				Sprite arrowSprite = GameManager.Instance.allUIIcons.Find (delegate(Sprite obj) {
					return obj.name == "arrowIcon";
				});

				Equipment compareEquipment = null;
				int[] itemProperties = null;
				int[] itemPropertiesDif = null;


				switch (equipment.equipmentType) {
				case EquipmentType.Weapon:
					compareEquipment = Player.mainPlayer.allEquipedEquipments [0] as Equipment;
					break;
				case EquipmentType.Armour:
					compareEquipment = Player.mainPlayer.allEquipedEquipments [1] as Equipment;
					break;
				case EquipmentType.Shoes:
					compareEquipment = Player.mainPlayer.allEquipedEquipments [2] as Equipment;
					break;
				default:
					break;
				}

				if (compareEquipment == null) {
					compareEquipment = new Equipment ();
				}

				itemProperties = new int[] {
					equipment.attackGain,
					equipment.attackSpeedGain,
					equipment.armourGain,
					equipment.manaResistGain,
					equipment.critGain,
					equipment.dodgeGain
				};
				
				itemPropertiesDif = new int[] {
					equipment.attackGain - compareEquipment.attackGain,
					equipment.attackSpeedGain - compareEquipment.attackSpeedGain,
					equipment.armourGain - compareEquipment.armourGain,
					equipment.manaResistGain - compareEquipment.manaResistGain,
					equipment.critGain - compareEquipment.critGain,
					equipment.dodgeGain - compareEquipment.dodgeGain
				};



				for (int i = 0; i < itemDetailPropTexts.Length; i++) {
				
					Text propText = itemDetailPropTexts [i];
					propText.text = itemProperties [i].ToString ();


					Image arrowImage = arrowsImages [i];
					Text propDifText = itemDetailPropDifTexts [i];

					if (itemPropertiesDif [i] < 0) {
						arrowImage.sprite = arrowSprite;
						arrowImage.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, 180));
						arrowImage.color = Color.red;
						arrowImage.enabled = true;
						propDifText.text = "<color=red>" + (-itemPropertiesDif [i]).ToString () + "</color>";
					} else if (itemPropertiesDif [i] == 0 && itemProperties [i] > 0) {
						arrowImage.sprite = arrowSprite;
						arrowImage.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, -90));
						arrowImage.enabled = true;
						propDifText.text = string.Empty;
					} else if (itemPropertiesDif [i] > 0) {
						arrowImage.sprite = arrowSprite;
						arrowImage.color = Color.green;
						arrowImage.enabled = true;
						propDifText.text = "<color=green>" + itemPropertiesDif [i].ToString () + "</color>";
					}

				}
					
				equipButton.onClick.RemoveAllListeners ();

				equipButton.onClick.AddListener (delegate() {
					bagViewCtr.EquipItem (equipment);	
				});
			}

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
}
