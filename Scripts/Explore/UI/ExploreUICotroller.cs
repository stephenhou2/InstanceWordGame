using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace WordJourney
{
	public class ExploreUICotroller : MonoBehaviour {

		private enum QueryType{
			Refresh,
			Quit
		}

		public Transform tintTextContainer;
		private InstancePool tintTextPool;
		private Transform tintTextModel;


		public Transform mask;

		/**********  battlePlane UI *************/
		public Transform battlePlane;

		/**********  battlePlane UI *************/

//		public NPCUIController npcUIController;

		private InstancePool materialCardPool;
		private Transform materialCardModel;
		public Transform formulaDetailPlane;
		public Transform materialCardContainer;


		private InstancePool choiceButtonPool;
		private Transform choiceButtonModel;
		private Transform goodsModel;
		private InstancePool goodsPool;

		private Transform statusTintModel;
		private InstancePool statusTintPool;

		public Text gameLevelText;
		public Text gameLevelLocationText;

//		private Dialog[] dialogs;
//		private Choice[] choices;

//		private List<Item> itemsToPickUp = new List<Item>();
		private Item itemToPickUp;

		public Transform pauseHUD;
		public Transform queryHUD;


		public Transform billboardPlane;
		public Transform billboard;


		public Transform crystalQueryHUD;

		private QueryType queryType;



		public void SetUpExploreCanvas(int gameLevelIndex, string gameLevelLocation){

			Transform poolContainerOfExploreCanvas = TransformManager.FindOrCreateTransform (CommonData.poolContainerName + "/PoolContainerOfExploreCanvas");
			Transform modelContainerOfExploreScene = TransformManager.FindOrCreateTransform (CommonData.instanceContainerName + "/ModelContainerOfExploreScene");

			choiceButtonPool = InstancePool.GetOrCreateInstancePool ("ChoiceButtonPool",poolContainerOfExploreCanvas.name);
			materialCardPool = InstancePool.GetOrCreateInstancePool ("MaterialCardPool", poolContainerOfExploreCanvas.name);
			goodsPool = InstancePool.GetOrCreateInstancePool ("GoodsPool", poolContainerOfExploreCanvas.name);
			statusTintPool = InstancePool.GetOrCreateInstancePool ("StatusTintPool", poolContainerOfExploreCanvas.name);
			tintTextPool = InstancePool.GetOrCreateInstancePool ("TintTextPool", poolContainerOfExploreCanvas.name);

			choiceButtonModel = TransformManager.FindTransform ("ChoiceButtonModel");
			materialCardModel = TransformManager.FindTransform ("MaterialCardModel");
			goodsModel = TransformManager.FindTransform ("GoodsModel");
			statusTintModel = TransformManager.FindTransform ("StatusTintModel");
			tintTextModel = TransformManager.FindTransform ("TintTextModel");

			choiceButtonModel.SetParent (modelContainerOfExploreScene);
			materialCardModel.SetParent (modelContainerOfExploreScene);
			goodsModel.SetParent (modelContainerOfExploreScene);
			statusTintModel.SetParent (modelContainerOfExploreScene);
			tintTextModel.SetParent (modelContainerOfExploreScene);

			if (!GameManager.Instance.UIManager.UIDic.ContainsKey ("BagCanvas")) {

				GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.bagCanvasBundleName, "BagCanvas", () => {
					Transform bagCanvas = TransformManager.FindTransform ("BagCanvas");
					bagCanvas.GetComponent<BagViewController> ().SetUpBagView (false);
				}, true,true);
					
			}

			int chapterIndex = gameLevelIndex / 5 + 1;
			int levelIndex = gameLevelIndex % 5 + 1;

			gameLevelText.text = string.Format ("第 {0} 层    第 {1} 关", chapterIndex, levelIndex);
			gameLevelLocationText.text = gameLevelLocation;

			GetComponent<BattlePlayerUIController> ().InitExplorePlayerView (statusTintModel, statusTintPool);
			GetComponent<BattlePlayerUIController> ().SetUpExplorePlayerView (Player.mainPlayer);
			GetComponent<BattleMonsterUIController> ().InitExploreMonsterView (statusTintModel, statusTintPool);

			GetComponent<Canvas> ().enabled = true;

		}

		public void ShowMask(){
			mask.gameObject.SetActive (true);
		}

		public void HideMask(){
			mask.gameObject.SetActive (false);
		}

		public void ShowFightPlane(){
			battlePlane.gameObject.SetActive (true);
			GetComponent<BattlePlayerUIController> ().SetUpFightAttackCheck ();

		}

		public void HideFightPlane(){
			battlePlane.gameObject.SetActive (false);
		}


//		public void ResetAttackCheckPosition(){
//			attackCheckController.ResetRectAttackCheckPosition ();
//		}



		public void SetUpTintHUD(string tint){
			Transform tintText = tintTextPool.GetInstance<Transform> (tintTextModel.gameObject, tintTextContainer);
			tintText.GetComponent<Text> ().text = tint;
			tintText.localPosition = new Vector3 (0, 300, 0);
			tintText.gameObject.SetActive (true);
			tintText.DOLocalMoveY (500, 1f).OnComplete (() => {
				tintText.gameObject.SetActive(false);
				tintTextPool.AddInstanceToPool(tintText.gameObject);
			});
//			StartCoroutine ("PlayTintTextAnim",tintText);
		}

//		private IEnumerator PlayTintTextAnim(Transform tintText){
//
//			yield return new WaitForSeconds (1f);
//
//
//
//		}

		public void EnterNPC(NPC npc,int currentLevelIndex){

			GetComponent<NPCUIController>().SetupNpcPlane (npc, currentLevelIndex,choiceButtonPool,choiceButtonModel,goodsPool,goodsModel);

		}

		/// <summary>
		/// 初始化公告牌
		/// </summary>
		public void SetUpBillboard(Billboard bb){
			billboardPlane.gameObject.SetActive (true);
			Text billboardContent = billboard.Find ("Content").GetComponent<Text> ();
			billboardContent.text = bb.content;
		}

		/// <summary>
		/// 退出公告牌
		/// </summary>
		public void QuitBillboard(){
			billboardPlane.gameObject.SetActive (false);
		}


		public void SetUpRewardFormulaPlane(Formula formula){

			ItemModel equipment = GameManager.Instance.gameDataCenter.allItemModels.Find (delegate(ItemModel obj) {
				return obj.itemId == formula.unlockedItemId;
			});

			Transform formulaDetailContainer = formulaDetailPlane.Find ("FormulaDetailContainer");

			Text equipmentName = formulaDetailContainer.Find ("EquipmentName").GetComponent<Text> ();
			Image equipmentIcon = formulaDetailContainer.Find ("EquipmentIcon").GetComponent<Image> ();
			Text equipmentDescription = formulaDetailContainer.Find ("EquipmentDescription").GetComponent<Text> ();

			equipmentName.text = equipment.itemName;
			equipmentDescription.text = equipment.itemDescription;

			Sprite s = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
				return obj.name == equipment.spriteName;
			});

			equipmentIcon.sprite = s;

			materialCardPool.AddChildInstancesToPool (materialCardContainer);

			for (int i = 0; i < equipment.itemIdsForProduce.Length; i++) {
				
				Transform materialCard = materialCardPool.GetInstance<Transform> (materialCardModel.gameObject, materialCardContainer);

				int itemId = equipment.itemIdsForProduce [i];

				Item item = Item.NewItemWith (itemId, 1);

				Image materialIcon = materialCard.Find ("MaterialIcon").GetComponent<Image> ();
				Text materialName = materialCard.Find ("MaterialName").GetComponent<Text> ();

				materialName.text = item.itemName;

				Sprite materialSprite = GameManager.Instance.gameDataCenter.allMaterialSprites.Find (delegate(Sprite obj) {
					return obj.name == item.spriteName;
				});

				materialIcon.sprite = materialSprite;


			}

			Button confirmButton = formulaDetailContainer.Find ("ConfirmButton").GetComponent<Button> ();

			confirmButton.onClick.RemoveAllListeners ();

			confirmButton.onClick.AddListener (delegate() {
				AddFormulaAndQuitFormulaDetailPlane (formula);
			});

			formulaDetailPlane.gameObject.SetActive (true);
		}

		private void AddFormulaAndQuitFormulaDetailPlane(Formula formula){

			Player.mainPlayer.AddItem (formula);

			formulaDetailPlane.gameObject.SetActive (false);

		}

		public void SetUpCrystaleQueryHUD(){
			crystalQueryHUD.gameObject.SetActive (true);
		}

		public void QuitCrystalQuery(){
			crystalQueryHUD.gameObject.SetActive (false);
		}

		public void EnterCrystal(){
			QuitCrystalQuery ();
			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.learnCanvasBundleName, "LearnCanvas", () => {
				TransformManager.FindTransform("LearnCanvas").GetComponent<LearnViewController>().SetUpLearnView();
			}, false, true);
		}

		public void SetUpProducePlane(){

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.spellCanvasBundleName, "SpellCanvas", () => {
				ItemModel swordModel = GameManager.Instance.gameDataCenter.allItemModels.Find(delegate(ItemModel obj){
					return obj.itemId == 0;
				});
				TransformManager.FindTransform ("SpellCanvas").GetComponent<SpellViewController> ().SetUpSpellViewForCreate (swordModel);
			}, false, true);

		}

		public void OnPauseButtonClick(){
			ShowPauseHUD ();
		}

		public void ShowPauseHUD(){
			Time.timeScale = 0;
			pauseHUD.gameObject.SetActive (true);
		}

		public void QuitPauseHUD(){
			Time.timeScale = 1f;
			pauseHUD.gameObject.SetActive (false);
		}

		public void OnRefreshButtonClick(){
			queryType = QueryType.Refresh;
			ShowQueryHUD ();
		}

		public void OnHomeButtonClick(){
			queryType = QueryType.Quit;
			ShowQueryHUD ();
		}

		public void OnSettingsButtonClick(){
			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.settingCanvasBundleName, "SettingCanvas", () => {
				TransformManager.FindTransform("SettingCanvas").GetComponent<SettingViewController>().SetUpSettingView();
			},false,true);
		}

		public void ShowQueryHUD(){
			queryHUD.gameObject.SetActive (true);
		}
		public void QuitQueryHUD(){
			queryHUD.gameObject.SetActive (false);
		}

		public void OnConfirmButtonClick(){
			QuitQueryHUD ();
			QuitPauseHUD ();

			ExploreManager exploreManager = TransformManager.FindTransform ("ExploreManager").GetComponent<ExploreManager> ();

			switch (queryType) {
			case QueryType.Refresh:
				exploreManager.RefrestCurrentLevel ();
				break;
			case QueryType.Quit:
				exploreManager.QuitExploreScene (false);
				break;
			}
		}

		public void OnCancelButtonClick(){
			QuitQueryHUD ();
		}



		public void QuitFight(){
			GetComponent<BattlePlayerUIController> ().QuitFight ();
			GetComponent<BattleMonsterUIController>().QuitFight ();
			HideFightPlane ();
		}



		public void QuitExplore(){

			TransformManager.DestroyTransform(transform);

			DestroyInstances ();

			Resources.UnloadUnusedAssets ();

			System.GC.Collect ();

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.homeCanvasBundleName, "HomeCanvas", () => {

				TransformManager.FindTransform("HomeCanvas").GetComponent<HomeViewController> ().SetUpHomeView ();

			});

		}




		private void DestroyInstances(){
			GameManager.Instance.UIManager.DestroryCanvasWith (CommonData.exploreSceneBundleName, "ExploreCanvas", "PoolContainerOfExploreScene", "ModelContainerOfExploreScene");
		}


	}
}
