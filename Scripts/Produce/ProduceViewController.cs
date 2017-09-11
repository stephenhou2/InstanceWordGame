using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class ProduceViewController : MonoBehaviour {

		public ProduceView produceView;

//		private List<Item> allWeapons = new List<Item>() ;
//		private List<Item> allarmours = new List<Item>() ;
//		private List<Item> allShoes = new List<Item>() ;

		private List<ItemModel> allEquipmentModels = new List<ItemModel> ();
		private List<ItemModel> allConsumablesModels = new List<ItemModel>() ;
		private List<ItemModel> allTaskItemModels = new List<ItemModel>();

		private List<ItemModel> itemModelsOfCurrentType;

		private List<Sprite> itemSprites;

		public void SetUpProduceView(){

				// 获取所有游戏物品的图片
			itemSprites = GameManager.Instance.allItemSprites;

			allEquipmentModels = GameManager.Instance.allItemModels.FindAll (delegate (ItemModel obj) {
				return obj.itemType == ItemType.Equipment;
			});

//			for (int i = 0; i < allEquipmentModel.Count; i++) {
//				Equipment equipment = new Equipment (allEquipmentModel [i]);
//				allEquipments.Add (equipment);
//			}

			produceView.SetUpProduceView (itemSprites);

			OnItemTypeButtonClick (0);

			GetComponent<Canvas>().enabled = true; 


		}

		public void OnItemTypeButtonClick(int buttonIndex){

			switch (buttonIndex) {
			case 0:
				itemModelsOfCurrentType = allEquipmentModels;
				break;
//			case 1:
//				itemsOfCurrentType = allarmours;;
//				break;
			case 2:
				itemModelsOfCurrentType = allConsumablesModels;
				break;
			case 3:
				itemModelsOfCurrentType = allTaskItemModels;
				break;
			default:
				break;
			}

			if (itemModelsOfCurrentType == null) {
				Debug.Log ("未找到制定类型的物品");
				return;
			}

			produceView.SetUpItemDetailsPlane (itemModelsOfCurrentType,buttonIndex);	

		}

		public void OnCharactersButtonClick(){

			produceView.SetUpCharactersPlane ();
		}

		public void QuitCharactersPlane(){

			produceView.OnQuitCharactersPlane();
		}

		public void OnGenerateButtonClick(ItemModel itemModel){

			GameObject spellCanvas = null;

			if (itemModel == null) {
				ResourceManager.Instance.LoadAssetWithFileName ("spell/canvas", () => {
					spellCanvas = GameObject.Find(CommonData.instanceContainerName + "/SpellCanvas");
					spellCanvas.GetComponent<SpellViewController>().SetUpSpellViewForCreate(null);
				});
				return;
			}


			List<char> unsufficientCharacters = Player.mainPlayer.CheckUnsufficientCharacters (itemModel.itemNameInEnglish);

			if (unsufficientCharacters.Count > 0) {
				foreach (char c in unsufficientCharacters) {
					Debug.Log (string.Format ("字母{0}数量不足", c.ToString ()));
				}
				return;
			}
				
			// 如果玩家字母碎片足够，则进入拼写界面
			ResourceManager.Instance.LoadAssetWithFileName ("spell/canvas", () => {
				spellCanvas = GameObject.Find(CommonData.instanceContainerName + "/SpellCanvas");
				spellCanvas.GetComponent<SpellViewController>().SetUpSpellViewForCreate(itemModel);
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


//		private void LoadAllItems(){
//
//	//		Item[] allItems = DataInitializer.LoadDataToModelWithPath<Item> (CommonData.persistDataPath, CommonData.itemsDataFileName);
//
//			List<Item> allItems = GameManager.Instance.allItems;
//
//
//			for (int i = 0; i < allItems.Count; i++) {
//
//				Item item = allItems [i];
//
//				switch (item.itemType) {
//
//				case ItemType.Weapon:
//					allWeapons.Add (item);
//					break;
//				case ItemType.armour:
//					allarmours.Add (item);
//					break;
//				case ItemType.Shoes:
//					allShoes.Add (item);
//					break;
//				case ItemType.Consumables:
//					allConsumables.Add (item);
//					break;
//				case ItemType.Task:
//					allTaskItems.Add (item);
//					break;
//				default:
//					break;
//				}
//
//			}
//		}
//
	}
}
