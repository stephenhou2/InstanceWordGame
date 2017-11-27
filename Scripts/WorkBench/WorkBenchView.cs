using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;



namespace WordJourney
{
	public class WorkBenchView : MonoBehaviour {

		public Transform workBenchContainer;// 工作台界面容器

		public Transform spellViewContainer;// 材料制作拼写页


		public Transform itemDisplayViewContainer;// 物品图鉴展示页

		private InstancePool itemDetailsPool;//物品明细模型缓存池
		private Transform itemDetailsModel;//物品明细模型
		public Transform allItemsContainer;//物品明细模型容器（父级transform）


		private InstancePool itemDetailTypeBtnPool;//物品详细类型按钮缓存池
		private Transform itemDetailTypeBtnModel;//物品详细类型按钮模型
		public Transform itemDetailTypeBtnsContainer;//物品详细类型按钮容器（父级transform）


		private InstancePool materialPool;//材料图鉴模型缓存池
		private Transform materialModel;//材料图鉴模型

		public Transform[] charactersOwnedPlane;

		public Transform itemDisplayPlane;

		public Transform allItemsPlane;

		public Transform itemDetailsPlane;

		private List<Sprite> itemSprites;
		private List<Sprite> materialSprites;

		public Transform charactersContainer;

		private int currentSelectItemIndex;


		public void ChangeToSpellView(){
			GetComponent<SpellViewController> ().SetUpSpellViewForCreateMaterial (null);
			GetComponent<SpellView> ().ClearEnteredCharactersPlane ();
			spellViewContainer.gameObject.SetActive (true);
			itemDisplayViewContainer.gameObject.SetActive (false);
//			GetComponent<Canvas>().enabled = true;
		}

		public void ChangeToItemDisplayView(){
			itemDisplayViewContainer.gameObject.SetActive (true);
			spellViewContainer.gameObject.SetActive (false);
//			GetComponent<Canvas> ().enabled = true;
		}

		public void Initialize(){
			
			this.itemSprites = GameManager.Instance.gameDataCenter.allItemSprites;
			this.materialSprites = GameManager.Instance.gameDataCenter.allMaterialSprites;

			Transform poolContainerOfItemDisplayCanvas = TransformManager.FindOrCreateTransform (CommonData.poolContainerName + "/PoolContainerOfItemDisplayCanvas");
			Transform modelContainerOfItemDisplayCanvas = TransformManager.FindOrCreateTransform (CommonData.instanceContainerName + "/ModelContainerOfItemDisplayCanvas");

			if (poolContainerOfItemDisplayCanvas.childCount == 0) {
				// 创建缓存池
				itemDetailsPool = InstancePool.GetOrCreateInstancePool ("ItemDetailsPool",poolContainerOfItemDisplayCanvas.name);
				itemDetailTypeBtnPool = InstancePool.GetOrCreateInstancePool ("ItemDetailTypeBtnPool",poolContainerOfItemDisplayCanvas.name);
				materialPool = InstancePool.GetOrCreateInstancePool ("MaterialBtnPool",poolContainerOfItemDisplayCanvas.name);
			}

			if (modelContainerOfItemDisplayCanvas.childCount == 0) {

				// 获得模型
				itemDetailsModel = TransformManager.FindTransform ("ItemDetailsModelInItemDisplayCanvas");
				itemDetailTypeBtnModel = TransformManager.FindTransform ("ItemDetailTypeBtnModel");
				materialModel = TransformManager.FindTransform ("MaterialModel");

				itemDetailsModel.SetParent (modelContainerOfItemDisplayCanvas);
				itemDetailTypeBtnModel.SetParent (modelContainerOfItemDisplayCanvas);
				materialModel.SetParent (modelContainerOfItemDisplayCanvas);
			}
		}

		public void SetUpDetailTypeButtons(string[] detailTypes){

			Debug.Log (itemDetailTypeBtnPool);

			itemDetailTypeBtnPool.AddChildInstancesToPool (itemDetailTypeBtnsContainer);

			for (int i = 0; i < detailTypes.Length; i++) {

				Button itemDetailTypeBtn = itemDetailTypeBtnPool.GetInstance<Button> (itemDetailTypeBtnModel.gameObject, itemDetailTypeBtnsContainer);

				Text itemDetailType = itemDetailTypeBtn.transform.Find ("ItemDetailType").GetComponent<Text> ();

				itemDetailType.text = detailTypes [i];

				itemDetailTypeBtn.onClick.RemoveAllListeners ();

				int index = i;

				itemDetailTypeBtn.onClick.AddListener (delegate {
					GetComponent<WorkBenchViewController>().OnItemDetailTypeButtonClick(detailTypes[index]);
				});
					
			}


		}


		/// <summary>
		/// 按照给定物品数据初始化制造界面
		/// </summary>
		/// <param name="itemModelsOfCurrentType">Item models of current type.</param>
		public void SetUpItemDetailsPlane(List<ItemModel> itemModelsOfCurrentType){

			itemDetailsPool.AddChildInstancesToPool (allItemsContainer);

			for (int i = 0; i < itemModelsOfCurrentType.Count; i++) {

				ItemModel itemModel = itemModelsOfCurrentType [i];

				if (!itemModel.formulaUnlocked) {
					continue;
				}

				Transform itemDetails = itemDetailsPool.GetInstance<Transform> (itemDetailsModel.gameObject, allItemsContainer);

				Image itemIcon = itemDetails.Find ("ItemIcon").GetComponent<Image>();

				Text itemName = itemDetails.Find ("ItemName").GetComponent<Text> ();

				Text itemDescText = itemDetails.Find ("ItemDescription").GetComponent<Text> ();

				Button itemDetailButton = itemDetails.Find ("ItemDetailButton").GetComponent<Button> ();

				Button produceButton = itemDetails.Find ("ProduceButton").GetComponent<Button> ();

				Transform newFormulaTintIcon = itemDetails.Find ("NewFormulaTintIcon");

				Transform materialsContainer = itemDetails.Find ("MaterialsContainer").transform;

				itemIcon.sprite = itemSprites.Find (delegate(Sprite obj) {
					return obj.name == itemModel.spriteName;
				});

				if (itemIcon.sprite != null) {
					itemIcon.enabled = true;
				}

				itemName.text = itemModel.itemName;

				Formula formula = Player.mainPlayer.allFormulasInBag.Find (delegate(Formula obj) {
					return obj.itemOrSkillId == itemModel.itemId;
				});

				if (formula == null) {
					newFormulaTintIcon.gameObject.SetActive(false);
				} else {
					newFormulaTintIcon.gameObject.SetActive(formula.isNewItem);
				}

				itemDescText.text = itemModel.itemDescription;


				itemDetailButton.onClick.RemoveAllListeners ();

				itemDetailButton.onClick.AddListener (delegate {
					SetUpItemDetailsHUD (itemModel);
				});


				produceButton.onClick.RemoveAllListeners ();

				produceButton.onClick.AddListener (delegate() {
					GetComponent<WorkBenchViewController>().OnGenerateButtonClick(itemModel,newFormulaTintIcon,formula);
				});


				List<Material> materialsNeed = itemModel.materials;

				materialPool.AddChildInstancesToPool (materialsContainer);

				for(int j = 0;j<materialsNeed.Count;j++){

					Material material = materialsNeed [j];

					Transform materialPlane = materialPool.GetInstance<Transform> (materialModel.gameObject, materialsContainer);

					SetUpMaterialPlane (materialPlane, material);

				}

				allItemsPlane.GetComponent<ScrollRect>().velocity = Vector2.zero;

				allItemsContainer.localPosition = new Vector3 (allItemsContainer.localPosition.x, 0, 0);

			}

		}

		private void SetUpMaterialPlane(Transform materialPlane,Material material){
			
			Image materialIcon = materialPlane.Find ("MaterialIcon").GetComponent<Image> ();

			Text materialName = materialPlane.Find ("MaterialName").GetComponent<Text> ();

			Text materialValence = materialPlane.Find ("MaterialValence").GetComponent<Text> ();

			Text materialProperty = materialPlane.Find ("MaterialProperty").GetComponent<Text> ();

			Text materialUnstableness = materialPlane.Find ("MaterialUnstableness").GetComponent<Text> ();

			Text materialPossessionCount = materialPlane.Find ("MaterialPossessionCount").GetComponent<Text> ();

			materialName.text = material.itemName;

			materialValence.text = material.valence.ToString();

			materialProperty.text = material.itemDescription;

			materialUnstableness.text = string.Format ("{0}%", material.unstableness);

			Material materialInBag = Player.mainPlayer.GetMaterialInBagWithId (material.itemId);

			if (materialInBag != null) {
				materialPossessionCount.text = materialInBag.itemCount.ToString ();
			} else {
				materialPossessionCount.text = "<color=red>0</color>";
			}


			Sprite s = materialSprites.Find (delegate(Sprite obj) {
				return obj.name == material.spriteName;
			});

			if (s != null) {
				materialIcon.sprite = s;
			}

		}

		public void SetUpCharactersPlane(){

			Player player = Player.mainPlayer;

			for (int i = 0; i < charactersOwnedPlane.Length; i++) {

				Text characterCount = charactersOwnedPlane [i].Find("Count").GetComponent<Text>();

				characterCount.text = player.charactersCount [i].ToString ();

//				char character = (char)(i + CommonData.aInASCII);
//
//				CharacterFragment cf = player.allCharacterFragmentsInBag.Find (delegate(CharacterFragment obj) {
//					return obj.itemName == character.ToString();
//				});
//
//				if (cf == null) {
//					characterCount.text = "0";
//				} else {
//					characterCount.text = cf.itemCount.ToString ();
//				}

			}

			charactersContainer.gameObject.SetActive (true);

		}


		public void SetUpItemDetailsHUD(ItemModel itemModel){

			itemDetailsPlane.gameObject.SetActive (true);

			Transform itemDetailsHUD = itemDetailsPlane.Find ("ItemDetailsHUD");

			Image itemIcon = itemDetailsHUD.Find ("ItemIcon").GetComponent<Image> ();

			Text itemName = itemDetailsHUD.Find ("ItemName").GetComponent<Text> ();

			Text itemDescText = itemDetailsHUD.Find ("ItemDescription").GetComponent<Text> ();

			Text itemBaseProperties = itemDetailsHUD.Find ("ItemBaseProperties").GetComponent<Text> ();

			Text itemAttachedProperties = itemDetailsHUD.Find ("ItemAttachedProperties").GetComponent<Text> ();

			Button produceButton = itemDetailsHUD.Find ("ProduceButton").GetComponent<Button> ();

			Transform materialsContainer = itemDetailsHUD.Find ("MaterialsContainer").transform;


			itemIcon.sprite = itemSprites.Find (delegate(Sprite obj) {
				return obj.name == itemModel.spriteName;
			});

			if (itemIcon.sprite != null) {
				itemIcon.enabled = true;
			}

			itemName.text = itemModel.itemName;

			itemDescText.text = itemModel.itemDescription;

			itemBaseProperties.text = itemModel.itemDescription;

			produceButton.onClick.RemoveAllListeners ();

			produceButton.onClick.AddListener (delegate() {

				Formula formula = Player.mainPlayer.allFormulasInBag.Find (delegate(Formula obj) {
					return obj.itemOrSkillId == itemModel.itemId;
				});
				
				GetComponent<WorkBenchViewController>().OnGenerateButtonClick(itemModel,null,formula);
			});

			List<Material> materialsNeed = itemModel.materials;

			materialPool.AddChildInstancesToPool (materialsContainer);

			for(int j = 0;j<materialsNeed.Count;j++){

				Material material = materialsNeed [j];

				Transform materialPlane = materialPool.GetInstance<Transform> (materialModel.gameObject, materialsContainer);

				SetUpMaterialPlane (materialPlane, material);

			}

		}

		public void QuitItemDetailsPlane(){
			itemDetailsPlane.gameObject.SetActive (false);
		}


		public void QuitCharactersPlane(){

			for (int i = 0; i < charactersOwnedPlane.Length; i++) {

				Text characterCount = charactersOwnedPlane [i].Find("Count").GetComponent<Text>();

				characterCount.text = string.Empty;

			}
				
			charactersContainer.gameObject.SetActive (false);

		}

		public void QuitItemDisplayView(){

			this.itemSprites = null;
			this.materialSprites = null;

//			itemDisplayViewContainer.GetComponent<Image> ().color = new Color (0, 0, 0, 0);

			float offsetY = GetComponent<CanvasScaler> ().referenceResolution.y;

			Vector3 originalPosition = workBenchContainer.localPosition;

			workBenchContainer.DOLocalMoveY (-offsetY, 0.5f).OnComplete (() => {
				GameManager.Instance.UIManager.HideCanvas(gameObject.name);
				workBenchContainer.localPosition = originalPosition;

			});

		}
	}
}
