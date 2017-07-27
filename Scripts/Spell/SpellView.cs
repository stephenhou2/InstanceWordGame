using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Text;


public class SpellView: MonoBehaviour {

	public GameObject spellPlane;

	public Text spellRequestText;

	public Text[] characterTexts;


	public GameObject createCountHUD;

	public Button minusBtn;

	public Button plusBtn;

	public Text createCount;

	public Slider countSlider;


	public GameObject createdItemsHUD;

	private InstancePool spellItemDetailPool;

	public GameObject spellItemDetailModel;

	public Transform itemDetailContainer;

//	public void SetUpSpellView(){
//
//	}
	public void SetUpSpellView(string itemName,string itemNameInEnglish){

		if (itemNameInEnglish != null) {
			spellRequestText.text = string.Format ("请正确拼写 <color=orange>{0}</color>", itemName);
		} else {
			spellRequestText.text = "请正确拼写任意物品";
		}

	}

	public void ClearEnteredCharactersPlane(){

		foreach (Text t in characterTexts) {
			t.text = string.Empty;
		}

	}

	public void OnEnterCharacter(StringBuilder enteredCharacters,string character){
		
		characterTexts [enteredCharacters.Length - 1].text = character;

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

	public void UpdateCreateCountHUD(int count){

		countSlider.value = count;

		createCount.text = "制作" + count.ToString() + "个";
	}

	public void SetUpCreateItemDetailHUD(List<Item> createItems){

		spellItemDetailPool =  InstancePool.GetOrCreateInstancePool ("SpellItemDetailPool");


		foreach (Item item in createItems) {
			
			Transform itemTrans = spellItemDetailPool.GetInstance<Transform> (spellItemDetailModel, itemDetailContainer);

			Image itemIcon = itemTrans.FindChild ("ItemIcon").GetComponent<Image> ();

			Text itemName = itemTrans.FindChild ("ItemName").GetComponent<Text> ();

			Text itemCount = itemTrans.FindChild ("ItemCount").GetComponent<Text> ();

			Text itemQuality = itemTrans.FindChild ("ItemQuality").GetComponent<Text> ();

			Text itemDesciption = itemTrans.FindChild ("ItemDescription").GetComponent<Text> ();

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

		QuitCreateCountHUD ();

		createdItemsHUD.SetActive (true);

		ClearEnteredCharactersPlane ();


	}

	public void QuitCreateCountHUD(){

		createCountHUD.SetActive (false);

	}



	public void OnQuitCreateDetailHUD(){

		createdItemsHUD.SetActive (false);

		spellItemDetailPool.AddChildInstancesToPool (itemDetailContainer);

	}


	public void OnQuitSpellPlane(){
		spellPlane.transform.DOLocalMoveY (-Screen.height, 0.5f).OnComplete (() => {
//			Destroy (GameObject.Find ("SpellCanvas"));
		});
	}


}
