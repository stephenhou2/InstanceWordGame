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

	private List<Sprite> itemSprites;

	public void SetUpProduceView(){

		LoadAllItems ();

		ResourceManager.Instance.LoadSpritesAssetWithFileName ("item/icons", () => {

			// 获取所有游戏物品的图片
			itemSprites = GameManager.Instance.allItemSprites;

			produceView.SetUpProduceView (itemSprites);

			OnItemTypeButtonClick(0);

			GetComponent<Canvas>().enabled = true; 

		});

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

		produceView.SetUpItemDetailsPlane (itemsOfCurrentType,buttonIndex);	

	}

	public void OnCharactersButtonClick(){

		produceView.SetUpCharactersPlane ();
	}

	public void QuitCharactersPlane(){

		produceView.OnQuitCharactersPlane();
	}

	public void OnGenerateButtonClick(Item item){

		GameObject spellCanvas = null;

		if (item == null) {
			ResourceManager.Instance.LoadAssetWithFileName ("spell/canvas", () => {
				spellCanvas = GameObject.Find(CommonData.instanceContainerName + "/SpellCanvas");
				spellCanvas.GetComponent<SpellViewController>().SetUpSpellView(null,SpellPurpose.Create);
			});
			return;
		}


		List<char> unsufficientCharacters = Player.mainPlayer.CheckUnsufficientCharacters (item.itemNameInEnglish);

		if (unsufficientCharacters.Count > 0) {
			foreach (char c in unsufficientCharacters) {
				Debug.Log (string.Format ("字母{0}数量不足", c.ToString ()));
			}
			return;
		}
			
		// 如果玩家字母碎片足够，则进入拼写界面
		ResourceManager.Instance.LoadAssetWithFileName ("spell/canvas", () => {
			spellCanvas = GameObject.Find(CommonData.instanceContainerName + "/SpellCanvas");
			spellCanvas.GetComponent<SpellViewController>().SetUpSpellView(item,SpellPurpose.Create);
		});

	}

	public void GenerateAnyItem(){

		OnGenerateButtonClick (null);

	}

	public void OnQuitButtonClick(){

		produceView.QuitProduceView (DestroyInstances);

		GameObject homeCanvas = GameObject.Find (CommonData.instanceContainerName + "/HomeCanvas");

		if (homeCanvas != null) {
			homeCanvas.GetComponent<HomeViewController> ().SetUpHomeView ();
		}


	}


	private void DestroyInstances(){

		TransformManager.DestroyTransfromWithName ("ItemDetailsPool",TransformRoot.PoolContainer);
		TransformManager.DestroyTransfromWithName ("ItemDetailsModel", TransformRoot.InstanceContainer);

		TransformManager.DestroyTransform (gameObject.transform);

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
