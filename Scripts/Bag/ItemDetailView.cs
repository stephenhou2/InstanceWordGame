using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



namespace WordJourney
{
	public class ItemDetailView : MonoBehaviour {

		public Text itemName;
		public Image itemIcon;

		public Text[] itemDetailPropTexts;
		public Text[] itemDetailPropDifTexts;

		public Button equipButton;

		public Transform propertiesPlane;
		public Transform detailDescText;

		public void SetUpItemDetailView(Equipment equipedEquipment,Equipment equipmentInBag){


//			if (item.itemType == ItemType.Consumables) {
//
//				Consumables consumables = item as Consumables;
//				
//				itemName.text = consumables.itemName;
//
//				itemIcon.sprite = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
//					return obj.name == consumables.spriteName;
//				});
//
//				if (itemIcon.sprite != null) {
//					itemIcon.enabled = true;
//				}
//
//				detailDescText.GetComponent<Text> ().text = consumables.GetItemBasePropertiesString ();
//
//				detailDescText.gameObject.SetActive (true);
//
//				return;
//			}


			itemName.text = equipmentInBag.itemName;

			itemIcon.sprite = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
				return obj.name == equipmentInBag.spriteName;
			});
			if (itemIcon.sprite != null) {
				itemIcon.enabled = true;
			}


			float[] itemProperties = null;
			float[] itemPropertiesDif = null;

			if (equipedEquipment == null) {
				equipedEquipment = new Equipment ();
			}

			itemProperties = new float[] {
				equipmentInBag.attackGain,
				equipmentInBag.attackSpeedGain,
				equipmentInBag.armorGain,
				equipmentInBag.manaResistGain,
				equipmentInBag.critGain,
				equipmentInBag.dodgeGain,
				equipmentInBag.healthGain,
				equipmentInBag.manaGain
			};
			
			itemPropertiesDif = new float[] {
				equipmentInBag.attackGain - equipedEquipment.attackGain,
				equipmentInBag.attackSpeedGain - equipedEquipment.attackSpeedGain,
				equipmentInBag.armorGain - equipedEquipment.armorGain,
				equipmentInBag.manaResistGain - equipedEquipment.manaResistGain,
				equipmentInBag.critGain - equipedEquipment.critGain,
				equipmentInBag.dodgeGain - equipedEquipment.dodgeGain,
				equipmentInBag.healthGain - equipedEquipment.healthGain,
				equipmentInBag.manaGain - equipedEquipment.manaGain
			};



			for (int i = 0; i < itemDetailPropTexts.Length; i++) {
			
				Text propText = itemDetailPropTexts [i];

				propText.text = itemProperties [i] > 0 ? itemProperties[i].ToString () : string.Empty;

				Text propDifText = itemDetailPropDifTexts [i];

				float compare = itemPropertiesDif [i];

				// 比较后根据属性增减 决定连接符号用"-"还是"+"
				string linkSymbol = compare < 0 ? "-" : "+";

				// 比较后根据属性增加决定字体颜色
				string colorText = compare < 0 ? "red" : "green";

				// 比较后的描述字符串
				string compareString = string.Empty;

				if (compare >= 1) {
					compareString = string.Format ("(<color={0}>{1}{2}</color>)",colorText,linkSymbol,compare);
				} else if (compare > 0 && compare < 1) {
					compareString = string.Format ("(<color={0}>{1}{2}%</color>)",colorText,linkSymbol,(int)(compare * 100));
				} else if (compare < 0 && compare > -1) {
					compareString = string.Format ("(<color={0}>{1}{2}%</color>)",colorText,linkSymbol,(int)(-compare * 100));
				} else if (compare <= -1) {
					compareString = string.Format ("(<color={0}>{1}{2}</color>)",colorText,linkSymbol,-compare);
				}

				propDifText.text = compareString;

			}
				
			equipButton.onClick.RemoveAllListeners ();



			equipButton.onClick.AddListener (delegate() {
				TransformManager.FindTransform("BagCanvas").GetComponent<BagViewController>().EquipEquipment (equipmentInBag);	
			});

			propertiesPlane.gameObject.SetActive (true);

		}

		public void ResetItemDetail(){

			itemName.text = string.Empty;
			itemIcon.sprite = null;
			itemIcon.enabled = false;

			detailDescText.GetComponent<Text> ().text = string.Empty;
			detailDescText.gameObject.SetActive (false);



			for (int i = 0; i < itemDetailPropTexts.Length; i++) {

				itemDetailPropTexts [i].text = string.Empty;

				itemDetailPropDifTexts [i].text = string.Empty;
			}
			propertiesPlane.gameObject.SetActive (false);

		}

	}
}
