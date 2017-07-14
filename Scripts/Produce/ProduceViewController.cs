using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProduceViewController : MonoBehaviour {

	public ProduceView produceView;

	private List<Item> allWeapons = new List<Item>() ;
	private List<Item> allAmours = new List<Item>() ;
	private List<Item> allShoes = new List<Item>() ;
	private List<Item> allConsumables = new List<Item>() ;
	private List<Item> allTaskItems = new List<Item>();

	private List<Item> currentItems;

	private Item itemToGenerate;

	private int[] charactersNeed = new int[26];

	private int aInAscii = (int)('a');

	public void SetUpProduceView(){

		LoadAllItems ();

		#warning 这里资源还没有做
		ResourceManager.Instance.LoadSpritesAssetWithFileName ("produce/icons", () => {

			// 获取所有游戏物品的图片
			for(int i = 0;i<ResourceManager.Instance.sprites.Count;i++){

				produceView.itemSprites.Add(ResourceManager.Instance.sprites[i]);

			}

			produceView.SetUpProduceView (allWeapons);
		});

	}

	public void OnItemTypeButtonClick(int index){

		switch (index) {
		case 0:
			produceView.SetUpItemIcons (allWeapons);
			currentItems = allWeapons;
			break;
		case 1:
			produceView.SetUpItemIcons (allAmours);
			currentItems = allAmours;
			break;
		case 2:
			produceView.SetUpItemIcons (allConsumables);
			currentItems = allConsumables;
			break;
		case 3:
			produceView.SetUpItemIcons (allTaskItems);
			currentItems = allTaskItems;
			break;
		default:
			break;
		}

	}

	public void OnItemButtonClick(int index){

		itemToGenerate = currentItems [index];

		produceView.SetUpItemDetailPlane (itemToGenerate);
	}

	public void OnGenerateItem(){

		string itemNameInEnglish = itemToGenerate.itemNameInEnglish;

		char[] charactersArray = itemNameInEnglish.ToCharArray ();

		foreach (char c in charactersArray) {
			int index = (int)c - aInAscii;
			charactersNeed [index]++;
		}

		// 判断玩家字母碎片是否足够
		for(int i = 0;i<charactersNeed.Length;i++){

			if (charactersNeed [i] > Player.mainPlayer.charactersCount[i]) {

				char c = (char)(i + aInAscii);

				Debug.Log(string.Format("字母{0}数量不足",c.ToString()));

				return;

			}

		}
		// 如果玩家字母碎片足够，则进入拼写界面
		GameObject spellCanvas = null;
		ResourceManager.Instance.LoadAssetWithFileName ("spell/canvas", () => {
			spellCanvas = ResourceManager.Instance.gos [0];
			spellCanvas.GetComponent<SpellViewController> ().SetUpSpellView ();
		});

//		for (int i = 0; i<charactersNeed.Length; i++) {
//
//			Player.mainPlayer.charactersCount [i] -= charactersNeed [i];
//
//		}

	}


	private void LoadAllItems(){

		Item[] allItems = DataInitializer.LoadDataToModelWithPath<Item> (CommonData.jsonFileDirectoryPath, CommonData.itemsDataFileName);

		for (int i = 0; i < allItems.Length; i++) {

			Item item = allItems [i];

			switch (item.itemType) {

			case ItemType.Weapon:
				allWeapons.Add (item);
				break;
			case ItemType.Amour:
				allAmours.Add (item);
				break;
			case ItemType.Shoes:
				allShoes.Add (item);
				break;
			case ItemType.Consumables:
				allConsumables.Add (item);
				break;
			case ItemType.Task:
				allTaskItems.Add (item);
				break;
			default:
				break;
			}

		}
	}

}
