using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProduceView : MonoBehaviour {

	public Button[] itemTypeButtons;

	public Button[] itembuttons;

	public Image itemDetailIcon;

	public Text itemName;

	public Text itemTypeText;

	public Transform[] charactersOwned;

	[HideInInspector]public List<Sprite> itemSprites = new List<Sprite>();

	// 初始化制造界面
	public void SetUpProduceView(List<Item> items){

		SetUpItemIcons (items);

		SetUpCharactersPlane ();

	}

	// 初始化物品图鉴
	public void SetUpItemIcons(List<Item> items){

		for(int i = 0;i<items.Count;i++){

			Button itemButton = itembuttons [i];

			itemButton.transform.FindChild ("SelectedBorder").GetComponent<Image> ().enabled = true;

			Image itemIcon = itemButton.transform.FindChild ("ItemIcon").GetComponent<Image> ();

			itemIcon.sprite = itemSprites.Find (delegate (Sprite obj){
				return obj.name == items[i].spriteName;
			});

			itemIcon.enabled = true;

		}
	}

	private void SetUpCharactersPlane(){

		Player player = Player.mainPlayer;

		for (int i = 0; i < charactersOwned.Length; i++) {

			Text characterCount = charactersOwned [i].FindChild("Count").GetComponent<Text>();

			characterCount.text = player.charactersCount [i].ToString ();

		}

	}

	public void SetUpItemDetailPlane(Item item){

		itemDetailIcon.sprite = itemSprites.Find (delegate (Sprite obj){
			return obj.name == item.spriteName;
		});

		itemDetailIcon.enabled = true;

		itemName.text = item.itemName;

		switch (item.itemType) {
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

}
