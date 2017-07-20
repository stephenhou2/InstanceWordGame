using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Text;


public class SpellView: MonoBehaviour {

	public GameObject spellPlane;

	public Text[] characterTexts;

	public GameObject ItemDetailHUD;

	public GameObject createCountHUD;

	public Text createCount;

	public Slider createSlider;

	public Transform contentTrans;

	private InstancePool itemPool;

//	public void SetUpSpellView(){
//
//	}


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

		createSlider.minValue = minValue;
		createSlider.maxValue = maxValue;

		createSlider.value = minValue;
	
		createCount.text = "制作1个";

	}

	public void UpdateCreateCountHUD(int count){

		createSlider.value = count;

		createCount.text = "制作" + count.ToString() + "个";
	}

	public void SetUpCreateItemDetailHUD(List<Item> createItems){

		Debug.Log (createItems.Count);

		itemPool =  InstancePool.GetOrCreateInstancePool ("ItemPool");

		GameObject itemModel = GameObject.Find (CommonData.instanceContainerName + "/Item");

		foreach (Item item in createItems) {
			
			Transform itemTrans = itemPool.GetInstance<Transform> (itemModel, contentTrans);

			Image itemIcon = itemTrans.FindChild ("ItemIcon").GetComponent<Image> ();

			Text itemName = itemTrans.FindChild ("ItemName").GetComponent<Text> ();

			Text itemCount = itemTrans.FindChild ("ItemCount").GetComponent<Text> ();

			Text itemDesciption = itemTrans.FindChild ("ItemDescription").GetComponent<Text> ();

			itemIcon.sprite = GameManager.Instance.allItemSprites.Find (delegate(Sprite obj) {
				return obj.name == item.spriteName;	
			});

			itemIcon.enabled = true;

			itemName.text = item.itemName;

			itemCount.text = item.itemCount.ToString ();

			itemDesciption.text = item.itemDescription;

		}

		QuitCreateCountHUD ();

		ItemDetailHUD.SetActive (true);

		ClearEnteredCharactersPlane ();


	}

	public void QuitCreateCountHUD(){

		createCountHUD.SetActive (false);


	}



	public void OnQuitCreateDetailHUD(){

		ItemDetailHUD.SetActive (false);

		itemPool.AddChildInstancesToPool (contentTrans);

	}


	public void OnQuitSpellPlane(){
		spellPlane.transform.DOLocalMoveY (-Screen.height, 0.5f).OnComplete (() => {
//			Destroy (GameObject.Find ("SpellCanvas"));
		});
	}


}
