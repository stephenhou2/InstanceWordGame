using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ProduceView : MonoBehaviour {

	public Button[] itemTypeButtons;

	public Button[] itembuttons;

	public Image itemDetailIcon;

	public Text itemName;

	public Text itemTypeText;

	public Transform[] charactersOwned;

	public Transform producePlane;

	private List<Sprite> itemSprites;
//	private List<Item> itemsOfCurrentType;


	public Sprite itemTypeButtonNormalIcon;
	public Sprite itemTypeButtonSelectedIcon;

	private int currentSelectItemIndex;
//	private 

	// 初始化制造界面
	public void SetUpProduceView(List<Sprite> itemSprites){

		this.itemSprites = itemSprites;

//		this.itemsOfCurrentType = items;

		SetUpCharactersPlane ();

	}

	// 初始化物品图鉴
	public void OnItemTypeButtonClick(List<Item> itemsOfCurrentType, int buttonIndex){

//		this.itemsOfCurrentType = itemsOfCurrentType;

		for (int i = 0; i < itemTypeButtons.Length; i++) {

			Button itemTypeBtn = itemTypeButtons [i];
			if (i == buttonIndex) {
				itemTypeBtn.GetComponent<Image> ().sprite = itemTypeButtonSelectedIcon;
			} else {
				itemTypeBtn.GetComponent<Image> ().sprite = itemTypeButtonNormalIcon;
			}

		}

		for(int i = 0;i<itemsOfCurrentType.Count;i++){

			Button itemButton = itembuttons [i];

//			itemButton.transform.FindChild ("SelectedBorder").GetComponent<Image> ().enabled = (i==0);

			Image itemIcon = itemButton.transform.FindChild ("ItemIcon").GetComponent<Image> ();

			itemIcon.sprite = itemSprites.Find (delegate (Sprite obj){
				return obj.name == itemsOfCurrentType[i].spriteName;
			});

			itemIcon.enabled = true;

		}

		for (int i = itemsOfCurrentType.Count; i < itembuttons.Length; i++) {
			Button itemButton = itembuttons [i];

			itemButton.transform.FindChild ("ItemIcon").GetComponent<Image> ().enabled = false;

		}

	}

	public void SetUpCharactersPlane(){

		Player player = Player.mainPlayer;

		for (int i = 0; i < charactersOwned.Length; i++) {

			Text characterCount = charactersOwned [i].FindChild("Count").GetComponent<Text>();

			characterCount.text = player.charactersCount [i].ToString ();

		}

	}

	public void OnItemButtonClick(int index,Item itemToGenerate){

		itembuttons [currentSelectItemIndex].transform.FindChild ("SelectedBorder").GetComponent<Image> ().enabled = false;

		currentSelectItemIndex = index;

		itembuttons [currentSelectItemIndex].transform.FindChild ("SelectedBorder").GetComponent<Image> ().enabled = true;

		itemDetailIcon.sprite = itemSprites.Find (delegate (Sprite obj){
			return obj.name == itemToGenerate.spriteName;
		});

		itemDetailIcon.enabled = true;

		itemName.text = itemToGenerate.itemName;

		switch (itemToGenerate.itemType) {
		case ItemType.Weapon:
			itemTypeText.text = "类型: 武器";
			break;
		case ItemType.Amour:
			itemTypeText.text = "类型: 防具";
			break;
		case ItemType.Shoes:
			itemTypeText.text = "类型: 鞋履";
			break;
		case ItemType.Consumables:
			itemTypeText.text = "类型: 消耗品";
			break;
		case ItemType.Task:
			itemTypeText.text = "类型: 任务物品";
			break;
		default:
			break;
		}
	}


	public void QuitProduceView(CallBack cb){

		producePlane.DOLocalMoveY (-Screen.height, 0.5f).OnComplete (() => {
			
			cb();

		});

	}
}
