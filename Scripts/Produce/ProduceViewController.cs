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

	private List<Item> itemsOfCurrentType;

	private List<Sprite> itemSprites = new List<Sprite>();

	private Item itemToGenerate;

	private int[] charactersNeed = new int[26];

	private int aInAscii = (int)('a');

	public void SetUpProduceView(){

		LoadAllItems ();

		ResourceManager.Instance.LoadSpritesAssetWithFileName ("item/icons", () => {

			// 获取所有游戏物品的图片
			for(int i = 0;i<ResourceManager.Instance.sprites.Count;i++){

				itemSprites.Add(ResourceManager.Instance.sprites[i]);

			}

			produceView.SetUpProduceView (itemSprites);

			OnItemTypeButtonClick(0);

			GetComponent<Canvas>().enabled = true; 

		});

	}

	public void UpdateProduceView(){
		produceView.SetUpCharactersPlane ();
	}

	public void OnItemTypeButtonClick(int buttonIndex){

		switch (buttonIndex) {
		case 0:
			itemsOfCurrentType = allWeapons;
			break;
		case 1:
			itemsOfCurrentType = allAmours;;
			break;
		case 2:
			itemsOfCurrentType = allConsumables;
			break;
		case 3:
			itemsOfCurrentType = allTaskItems;
			break;
		default:
			break;
		}

		if (itemsOfCurrentType == null) {
			Debug.Log ("未找到制定类型的物品");
			return;
		}

		produceView.OnItemTypeButtonClick (itemsOfCurrentType,buttonIndex);	

		OnItemButtonClick (0);
	}

	public void OnItemButtonClick(int index){

		itemToGenerate = itemsOfCurrentType[index];

		produceView.OnItemButtonClick (index,itemToGenerate);
	}

	public void OnGenerateItemButtonClick(){

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

		for (int i = 0; i < charactersNeed.Length; i++) {
			charactersNeed [i] = 0;
		}

		// 如果玩家字母碎片足够，则进入拼写界面
		GameObject spellCanvas = null;

		ResourceManager.Instance.LoadAssetWithFileName ("spell/canvas", () => {
			spellCanvas = GameObject.Find(CommonData.instanceContainerName + "/SpellCanvas");
			spellCanvas.GetComponent<SpellViewController>().SetUpSpellView(itemNameInEnglish);
			GetComponent<Canvas>().enabled = false;
		});


//		for (int i = 0; i<charactersNeed.Length; i++) {
//
//			Player.mainPlayer.charactersCount [i] -= charactersNeed [i];
//
//		}

	}

	public void OnQuitButtonClick(){

		produceView.QuitProduceView ();

		GameObject homeCanvas = GameObject.Find (CommonData.instanceContainerName + "/HomeCanvas");

		if (homeCanvas != null) {
			homeCanvas.GetComponent<HomeViewController> ().SetUpHomeView ();
		}


	}


	private void LoadAllItems(){

//		Item[] allItems = DataInitializer.LoadDataToModelWithPath<Item> (CommonData.jsonFileDirectoryPath, CommonData.itemsDataFileName);

		List<Item> allItems = GameManager.Instance.allItems;


		for (int i = 0; i < allItems.Count; i++) {

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
