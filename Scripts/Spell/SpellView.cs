using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Text;



namespace WordJourney
{
	public class SpellView: MonoBehaviour {

		public Transform spellViewContainer;
		public GameObject spellPlane;

		public Text spellRequestText;

		public Text[] characterTexts;


		public Button[] characterButtons;

		public Button onceButton;

		public Button multiTimesButton;


		public GameObject createCountHUD;

		public Button minusBtn;

		public Button plusBtn;

		public Text createCount;

		public Slider countSlider;


		public GameObject createdItemsHUD;

		private InstancePool createdItemDetailPool;

		public GameObject createdItemDetailModel;

		public Transform itemDetailContainer;


		public Transform strengthenItemDetailHUD;

		public Transform strengthenDetailContainer;

		public Text strengthenItemName;

		public Text strengthenItemType;

		public Text strengthenItemQuality;

		public Text strengthenItemProperties;

		public Text strengthenTimes;

		public Image strengthenItemIcon;

		private InstancePool strengthenGainTextPool;

		public GameObject strengthenGainTextModel;

	//	public void SetUpSpellView(){
	//
	//	}
		public void SetUpSpellView(Item item){

			if (item != null && item.itemNameInEnglish != null) {
				spellRequestText.text = string.Format ("请正确拼写 <color=orange>{0}</color>", item.itemName);
			} else {
				spellRequestText.text = "请正确拼写任意物品";
			}

		}


		public void SetUpSpellView(ItemModel itemModel){
			
			if (itemModel != null && itemModel.itemNameInEnglish != null) {
				spellRequestText.text = string.Format ("请正确拼写 <color=orange>{0}</color>", itemModel.itemName);
			} else {
				spellRequestText.text = "请正确拼写任意物品";
			}

			onceButton.gameObject.SetActive(true);
			multiTimesButton.gameObject.SetActive(true);
		}

		public void ClearEnteredCharactersPlane(){

			foreach (Text t in characterTexts) {
				t.text = string.Empty;
			}

		}

		public void OnEnterCharacter(StringBuilder enteredCharacters,string character){

			if (character == null) {
				return;
			}
			
			characterTexts [enteredCharacters.Length - 1].text = character;

		}

		public void ShowCharacterTintHUD(int buttonIndex){
			Button characterButton = characterButtons [buttonIndex];
			Transform characterTintHUD = characterButton.transform.Find ("TintHUD");
			characterTintHUD.gameObject.SetActive (true);
		}

		public void HideCharacterTintHUD(int buttonIndex){
			Button characterButton = characterButtons [buttonIndex];
			Transform characterTintHUD = characterButton.transform.Find ("TintHUD");
			characterTintHUD.gameObject.SetActive (false);
		}



		public void OnBackspace(int enteredStringLength){
			
			characterTexts [enteredStringLength].text = string.Empty;

		}
			


		public void SetUpCreateCountHUD(int minValue,int maxValue){

			createCountHUD.SetActive (true);

			if (minusBtn.GetComponent<Image> ().sprite == null 
				|| plusBtn.GetComponent<Image>().sprite == null) 
			{
				Sprite arrowSprite = GameManager.Instance.allUIIcons.Find (delegate(Sprite obj) {
					return obj.name == "arrowIcon";
				});

				minusBtn.GetComponent<Image> ().sprite = arrowSprite;
				plusBtn.GetComponent<Image> ().sprite = arrowSprite;
			}

			countSlider.minValue = minValue;
			countSlider.maxValue = maxValue;

			countSlider.value = minValue;

			createCount.text = "制作1个";



		}

		public void UpdateCreateCountHUD(int count,SpellPurpose spellPurpose){

			countSlider.value = count;

			createCount.text = "制作" + count.ToString() + "个";



		}

		public void SetUpCreateItemDetailHUD(List<Item> createItems){

			createdItemDetailPool =  InstancePool.GetOrCreateInstancePool ("CreatedItemDetailPool");


			foreach (Item item in createItems) {
			
				Transform itemTrans = createdItemDetailPool.GetInstance<Transform> (createdItemDetailModel, itemDetailContainer);

				Image itemIcon = itemTrans.Find ("ItemIcon").GetComponent<Image> ();

				Text itemName = itemTrans.Find ("ItemName").GetComponent<Text> ();

				Text itemCount = itemTrans.Find ("ItemCount").GetComponent<Text> ();

				Text itemQuality = itemTrans.Find ("ItemQuality").GetComponent<Text> ();

				Text itemDesciption = itemTrans.Find ("ItemDescription").GetComponent<Text> ();

				itemIcon.sprite = GameManager.Instance.allItemSprites.Find (delegate(Sprite obj) {
					return obj.name == item.spriteName;	
				});

				if (itemIcon.sprite != null) {
					itemIcon.enabled = true;
				}

				itemName.text = item.itemName;

				itemQuality.text = item.GetItemQualityString ();

				itemCount.text = item.itemCount.ToString ();

				itemDesciption.text = item.GetItemPropertiesString ();

			}

			QuitSpellCountHUD ();

			createdItemsHUD.SetActive (true);

			ClearEnteredCharactersPlane ();


		}

		public void SetUpStrengthenItemDetailHUD(Equipment equipment){

			strengthenItemName.text = equipment.itemName;
			strengthenItemType.text = equipment.GetItemTypeString ();
			strengthenItemQuality.text = equipment.GetItemQualityString ();
			strengthenTimes.text = "强化次数: " + equipment.strengthenTimes.ToString() + "次";
			strengthenItemProperties.text = equipment.GetItemPropertiesString ();

			strengthenItemIcon.sprite = GameManager.Instance.allItemSprites.Find (delegate (Sprite obj) {
				return obj.name == equipment.spriteName;
			});

			if (strengthenItemIcon.sprite != null) {
				strengthenItemIcon.enabled = true;
			}

			strengthenGainTextPool = InstancePool.GetOrCreateInstancePool ("StrengthenGainTextPool");

			strengthenItemDetailHUD.gameObject.SetActive (true);

		}

		public void UpdateStrengthenItemDetailHUD(Equipment equipment,string strengthenGainStr){

			strengthenTimes.text = "强化次数: " + equipment.strengthenTimes.ToString() + "次";
			strengthenItemProperties.text = equipment.GetItemPropertiesString ();

			Text strengthenGainText = strengthenGainTextPool.GetInstance<Text> (strengthenGainTextModel, strengthenDetailContainer);

			strengthenGainText.transform.localPosition = Vector3.zero;

			strengthenGainText.gameObject.SetActive(true);

			strengthenGainText.text = strengthenGainStr;

			strengthenGainText.transform.DOLocalMoveY (200f, 0.5f).OnComplete (() => {

				strengthenGainText.gameObject.SetActive(false);

				strengthenGainText.text = string.Empty;

				strengthenGainTextPool.AddInstanceToPool(strengthenGainText.gameObject);

			});
				
		}

		public void QuitStrengthenItemDetailHUD(){

			strengthenItemName.text = string.Empty;
			strengthenItemType.text = string.Empty;
			strengthenTimes.text = string.Empty;
			strengthenItemProperties.text = string.Empty;

			strengthenItemIcon.sprite = null;
			strengthenItemIcon.enabled = false;

			strengthenItemDetailHUD.gameObject.SetActive (false);

		}

		public void QuitSpellCountHUD(){

			createCountHUD.SetActive (false);

		}



		public void OnQuitCreateDetailHUD(){

			createdItemsHUD.SetActive (false);

			createdItemDetailPool.AddChildInstancesToPool (itemDetailContainer);

		}


		public void OnQuitSpellPlane(){

			spellViewContainer.GetComponent<Image> ().color = new Color (0, 0, 0, 0);

			float offsetY = GetComponent<CanvasScaler> ().referenceResolution.y;

			spellPlane.transform.DOLocalMoveY (-offsetY, 0.5f).OnComplete (() => {
	//			Destroy (GameObject.Find ("SpellCanvas"));
			});
		}


	}
}
