using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class ItemDisplayViewController : MonoBehaviour {

		private ItemDisplayView itemDisplayView;


//		private Dictionary<string,List<ItemModel>> modelsDic = new Dictionary<string,List<ItemModel>> ();
		private Dictionary<string,string[]> typeDic = new Dictionary<string,string[]>();

//		private List<ItemModel> allWeaponModels = new List<ItemModel> ();
//		private List<ItemModel> allClothModels = new List<ItemModel> ();
//		private List<ItemModel> allOrnamentModels = new List<ItemModel>();
//		private List<ItemModel> allConsumablesModels = new List<ItemModel>() ;
//		private List<ItemModel> allTaskItemModels = new List<ItemModel>();



		private List<ItemModel> allItemModels;

		private List<ItemModel> itemModelsOfCurrentType;

		private string weaponsString = "weapons";
		private string clothString = "cloth";
		private string ornamentString = "ornaments";
		private string consumablesString = "consumables";
		private string taskItemsString = "taskItems";



		void Awake(){

			typeDic.Add (weaponsString, new string[]{ "剑", "刀", "斧子", "锤", "法杖", "匕首" });
			typeDic.Add (clothString, new string[]{ "罩袍", "盔甲", "裤子", "盔帽", "鞋子" });
			typeDic.Add (ornamentString, new string[]{ "挂饰", "戒指" });
			typeDic.Add (consumablesString, new string[]{ "xxx" });
			typeDic.Add (taskItemsString, new string[]{ "xxx" });

			itemDisplayView = GetComponent<ItemDisplayView> ();

			TestMaterials ();

		}

		#warning for test use
		private void TestMaterials(){

			Material wood = GameManager.Instance.gameDataCenter.allMaterials.Find (delegate(Material obj) {
				return obj.itemName == "木";
			});
			Material iron = GameManager.Instance.gameDataCenter.allMaterials.Find (delegate(Material obj) {
				return obj.itemName == "铁锭";
			});
			Material bandage = GameManager.Instance.gameDataCenter.allMaterials.Find (delegate(Material obj) {
				return obj.itemName == "绷带";
			});

			wood.itemCount = 5;
			iron.itemCount = 5;
			bandage.itemCount = 5;

			Player.mainPlayer.AddItem (wood);
			Player.mainPlayer.AddItem (iron);
			Player.mainPlayer.AddItem (bandage);

			Formula f = new Formula (FormulaType.Equipment, 1);
			Player.mainPlayer.AddItem (f);
		}

		/// <summary>
		/// 初始化制造界面
		/// </summary:
		public void SetUpItemDisplayView(){

			allItemModels = GameManager.Instance.gameDataCenter.allItemModels;

//			for (int i = 0; i < allItemModels.Count; i++) {
//
//				ItemModel im = allItemModels [i];

//				int equipmentIndex = im.itemId;

//				if (equipmentIndex < 0) {
//					throw(new System.Exception("装备id数据错误"));
//				}

//				switch (im.detailType) {
//				case DetailEquipmentType.Sword:
//					modelsDic.Add("sword",
//				}

//				if (equipmentIndex < 10) {
//					allWeaponModels.Add (im);
//				} else if (equipmentIndex < 110) {
//					allClothModels.Add (im);
//				} else {
//					allOrnamentModels.Add (im);
//				}
//
//			}

			itemDisplayView.Initialize ();

			OnItemTypeButtonClick (0);

			GetComponent<Canvas>().enabled = true; 

		}

		public void OnItemTypeButtonClick(int buttonIndex){

			string[] typeStrings = new string[]{};

			switch (buttonIndex) {
			case 0:
//				itemModelsOfCurrentType = allWeaponModels;

				typeStrings = typeDic [weaponsString];
				break;
			case 1:
//				itemModelsOfCurrentType = allClothModels;
				typeStrings = typeDic [clothString];
				break;
			case 2:
//				itemModelsOfCurrentType = allOrnamentModels;
				typeStrings = typeDic [ornamentString];
				break;
			case 3:
//				itemModelsOfCurrentType = allConsumablesModels;
				typeStrings = typeDic [consumablesString];
				break;
			case 4:
//				itemModelsOfCurrentType = allTaskItemModels;
				typeStrings = typeDic [taskItemsString];
				break;
			}
				

			itemDisplayView.SetUpDetailTypeButtons (typeStrings);

			OnItemDetailTypeButtonClick (typeStrings [0]);

		}


		public void OnItemDetailTypeButtonClick(string detailTypeString){

			itemModelsOfCurrentType = allItemModels.FindAll (delegate(ItemModel obj) {
				return obj.detailType == detailTypeString;
			});

			itemDisplayView.SetUpItemDetailsPlane (itemModelsOfCurrentType);

		}
			

		public void OnCharactersButtonClick(){

			itemDisplayView.SetUpCharactersPlane ();
		}

		public void QuitCharactersPlane(){

			itemDisplayView.QuitCharactersPlane();
		}

		public void OnGenerateButtonClick(ItemModel itemModel){

			for (int i = 0; i < itemModel.materials.Count; i++) {

				Material m = itemModel.materials [i];

				Material materialInBag = Player.mainPlayer.GetMaterialInBagWithId (m.itemId);

				if (materialInBag == null) {
					Debug.Log(string.Format("缺少材料{0}",m.itemName));
					return;
				}


			}

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.produceCanvasBundleName, "ProduceCanvas", () => {

				TransformManager.FindTransform("ProduceCanvas").GetComponent<ProduceViewController>().SetUpProduceView(itemModel);

				GetComponent<Canvas>().enabled = false;

				gameObject.SetActive(false);

			});

		}

		public void GenerateAnyItem(){

			OnGenerateButtonClick (null);

		}

		public void OnItemDetailsPlaneClick(){
			itemDisplayView.QuitItemDetailsPlane ();
		}

		public void QuitItemDisplayView(){

			itemDisplayView.QuitItemDisplayView ();

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.homeCanvasBundleName, "HomeCanvas", () => {
				TransformManager.FindTransform("HomeCanvas").GetComponent<HomeViewController> ().SetUpHomeView ();
			});

			TransformManager.DestroyTransfromWithName ("PoolContainerOfItemDisplayCanvas",TransformRoot.PoolContainer);

			GameManager.Instance.gameDataCenter.ReleaseDataWithNames (new string[]{"AllItemModels","AllItemSprites","AllMaterials","AllMaterialSprites"});

		}


		public void DestroyInstances(){

			GameManager.Instance.UIManager.DestroryCanvasWith (CommonData.itemDisplayCanvasBundleName, "ItemDisplayCanvas", "PoolContainerOfItemDisplayCanvas", "ModelContainerOfItemDisplayCanvas");

		}

	}
}
