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
		public Transform spellPlane;

		public Text spellRequestText;

//		public Text[] characterTexts;


		public Button[] characterButtons;

		public Button onceButton;

		public Button multiTimesButton;

		public Button confirmButton;

		public Text charactersEntered;


		public GameObject createCountHUD;

		public Button minusBtn;

		public Button plusBtn;

		public Text createCount;

		public Slider countSlider;


		public Transform createdMaterialHUD;

//		public Transform materialDetailsContainer;

		public Image materialIcon;
		public Text materialName;
		public Text materialCount;
		public Text materialProperty;
		public Text materialValence;


	//	public void SetUpSpellView(){
	//
	//	}
		public void SetUpSpellViewWith(string wordInChinese,SpellPurpose spellPurpose){

			if (wordInChinese != null) {
				spellRequestText.text = string.Format ("拼写 <color=orange>{0}</color>", wordInChinese);

			} else {
				spellRequestText.text = "拼写任意单词";
			}

			switch (spellPurpose) {
			case SpellPurpose.CreateMaterial:
				onceButton.gameObject.SetActive (true);
				multiTimesButton.gameObject.SetActive (true);
				confirmButton.gameObject.SetActive (false);
				break;
			case SpellPurpose.CreateFuseStone:
				onceButton.gameObject.SetActive (false);
				multiTimesButton.gameObject.SetActive (false);
				confirmButton.GetComponentInChildren<Text> ().text = "生成";
				confirmButton.gameObject.SetActive (true);
				break;
			case SpellPurpose.Fix:
				onceButton.gameObject.SetActive (false);
				multiTimesButton.gameObject.SetActive (false);
				confirmButton.GetComponentInChildren<Text> ().text = "修复";
				confirmButton.gameObject.SetActive (true);
				break; 
			}
				
//			GetComponent<Canvas>().enabled = true;

		}


//		public void SetUpSpellView(ItemModel itemModel){
//			
//			if (itemModel != null && itemModel.itemNameInEnglish != null) {
//				spellRequestText.text = string.Format ("请正确拼写 <color=orange>{0}</color>", itemModel.itemName);
//			} else {
//				spellRequestText.text = "请正确拼写任意物品";
//			}
//
//			onceButton.gameObject.SetActive(true);
//			multiTimesButton.gameObject.SetActive(true);
//		}

		public void ClearEnteredCharactersPlane(){

//			foreach (Text t in characterTexts) {
//				t.text = string.Empty;
//			}

			charactersEntered.text = string.Empty;

		}


		public void UpdateCharactersEntered(string characters,int[] charactersInsufficientArray){

			charactersEntered.text = characters;

			for (int i = 0; i < characterButtons.Length; i++) {
				characterButtons [i].interactable = charactersInsufficientArray [i] == 0 
					&& Player.mainPlayer.charactersCount[i] > 0;
			}

		}

		public void ShowCharacterTintHUD(int index){
			Button characterButton = characterButtons [index];
			if (characterButton.interactable) {
				characterButton.transform.Find ("TintHUD").gameObject.SetActive (true);
			}
		}

		public void HideCharacterTintHUD(int index){
			Button characterButton = characterButtons [index];
			if (characterButton.interactable) {
				characterButton.transform.Find ("TintHUD").gameObject.SetActive (false);
			}
		}

		public void UpdateCharactersPlane(){
			for (int i = 0; i < characterButtons.Length; i++) {
				characterButtons [i].interactable = Player.mainPlayer.charactersCount[i] > 0;
			}
		}

		public void SetUpCreateCountHUD(int minValue,int maxValue){

			createCountHUD.SetActive (true);

			if (minusBtn.GetComponent<Image> ().sprite == null 
				|| plusBtn.GetComponent<Image>().sprite == null) 
			{
				Sprite arrowSprite = GameManager.Instance.gameDataCenter.allUISprites.Find (delegate(Sprite obj) {
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

		public void SetUpCreateMaterialDetailHUD(Item item){

			QuitSpellCountHUD ();

			materialName.text = item.itemName;
			materialProperty.text = item.itemDescription;

			switch (item.itemType) {

			case ItemType.Material:

				materialCount.text = string.Format ("数量:{0}", item.itemCount);

				materialValence.text = (item as Material).valence.ToString ();

				Sprite s = GameManager.Instance.gameDataCenter.allMaterialSprites.Find (delegate(Sprite obj) {
					return obj.name == item.spriteName;
				});

				if (s != null) {
					materialIcon.sprite = s;
				}
				break;
			case ItemType.FuseStone:
				materialCount.text = string.Empty;
				materialValence.text = string.Empty;

				s = GameManager.Instance.gameDataCenter.allItemSprites.Find(delegate(Sprite obj){
					return obj.name == item.spriteName;
				});

				if (s != null) {
					materialIcon.sprite = s;
				}
				break;
			default:
				break;
			}


			createdMaterialHUD.gameObject.SetActive (true);

			ClearEnteredCharactersPlane ();

		}

//		public void SetUpFixedItemDetailHUD(Equipment equipment){
//
//			fixedItemName.text = equipment.itemName;
//			fixedItemType.text = equipment.GetItemTypeString ();
//			fixedItemProperties.text = equipment.GetItemBasePropertiesString ();
//			fixedItemDamagePercentage.text = string.Format ("损坏度:{0}%", (int)(equipment.damagePercentage * 100));
//
//
//			fixedItemIcon.sprite = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate (Sprite obj) {
//				return obj.name == equipment.spriteName;
//			});
//
//			if (fixedItemIcon.sprite != null) {
//				fixedItemIcon.enabled = true;
//			}
//
//			fixGainTextPool = InstancePool.GetOrCreateInstancePool ("FixGainTextPool");
//
//			fixedItemDetailHUD.gameObject.SetActive (true);
//
//		}

//		public void UpdateFixedItemDetailHUD(Equipment equipment){
//
//			fixedItemProperties.text = equipment.GetItemBasePropertiesString ();
//
//			Text strengthenGainText = fixGainTextPool.GetInstance<Text> (fixGainTextModel, fixedItemDetailContainer);
//
//			strengthenGainText.transform.localPosition = Vector3.zero;
//
//			strengthenGainText.gameObject.SetActive(true);
//
//			strengthenGainText.transform.DOLocalMoveY (200f, 0.5f).OnComplete (() => {
//
//				strengthenGainText.gameObject.SetActive(false);
//
//				strengthenGainText.text = string.Empty;
//
//				fixGainTextPool.AddInstanceToPool(strengthenGainText.gameObject);
//
//			});
//				
//		}
//
//		public void QuitFixedItemDetailHUD(){
//
//			fixedItemName.text = string.Empty;
//			fixedItemType.text = string.Empty;
//			fixedItemProperties.text = string.Empty;
//
//			fixedItemIcon.sprite = null;
//			fixedItemIcon.enabled = false;
//
//			fixedItemDetailHUD.gameObject.SetActive (false);
//
//		}

		public void QuitSpellCountHUD(){

			createCountHUD.SetActive (false);

		}



		public void OnQuitCreateDetailHUD(){

			createdMaterialHUD.gameObject.SetActive (false);

		}


		public void OnQuitSpellPlane(){

			spellViewContainer.GetComponent<Image> ().color = new Color (0, 0, 0, 0);

			float offsetY = GetComponent<CanvasScaler> ().referenceResolution.y;

			Vector3 originalPosition = spellPlane.localPosition;

			spellPlane.DOLocalMoveY (-offsetY, 0.5f).OnComplete (() => {
//				GetComponent<Canvas>().enabled = false;
				spellPlane.localPosition = originalPosition;
			});
		}


	}
}
