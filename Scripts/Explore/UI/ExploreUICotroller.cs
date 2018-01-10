using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace WordJourney
{
	using UnityEngine.UI;
	using DG.Tweening;
	using System.Text;

	public class ExploreUICotroller : MonoBehaviour {

		private enum QueryType{
			Refresh,
			Quit
		}

//		public Transform tintTextContainer;
//		private InstancePool tintTextPool;
//		private Transform tintTextModel;

		public TintHUD tintHUD;
		public UnlockScrollDetailHUD unlockScrollDetail;
		public CraftingRecipesHUD craftingRecipesDetail;


		public Transform mask;

		/**********  battlePlane UI *************/
		public Transform battlePlane;

		/**********  battlePlane UI *************/

//		public NPCUIController npcUIController;

//		private InstancePool materialCardPool;
//		private Transform materialCardModel;
//		public Transform unlockScrollDetailPlane;
//		public Transform materialCardContainer;


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
//			materialCardPool = InstancePool.GetOrCreateInstancePool ("MaterialCardPool", poolContainerOfExploreCanvas.name);
			goodsPool = InstancePool.GetOrCreateInstancePool ("GoodsPool", poolContainerOfExploreCanvas.name);
			statusTintPool = InstancePool.GetOrCreateInstancePool ("StatusTintPool", poolContainerOfExploreCanvas.name);
//			tintTextPool = InstancePool.GetOrCreateInstancePool ("TintTextPool", poolContainerOfExploreCanvas.name);

			choiceButtonModel = TransformManager.FindTransform ("ChoiceButtonModel");
//			materialCardModel = TransformManager.FindTransform ("MaterialCardModel");
			goodsModel = TransformManager.FindTransform ("GoodsModel");
			statusTintModel = TransformManager.FindTransform ("StatusTintModel");
//			tintTextModel = TransformManager.FindTransform ("TintTextModel");

			choiceButtonModel.SetParent (modelContainerOfExploreScene);
//			materialCardModel.SetParent (modelContainerOfExploreScene);
			goodsModel.SetParent (modelContainerOfExploreScene);
			statusTintModel.SetParent (modelContainerOfExploreScene);
//			tintTextModel.SetParent (modelContainerOfExploreScene);



			unlockScrollDetail.InitUnlockScrollDetailHUD (true, null, UnlockItemCallBack, ResolveScrollCallBack);
			craftingRecipesDetail.InitCraftingRecipesHUD (true, UpdateBottomBar, CraftItemCallBack);


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


		public void SetUpTintHUD(string tint){
			tintHUD.SetUpTintHUD (tint);
		}

		/// <summary>
		/// 更新底部消耗品栏
		/// </summary>
		public void UpdateBottomBar(){
			GetComponent<BattlePlayerUIController> ().SetUpBottomConsumablesButtons ();
		}

//		public void SetUpTintHUD(string tint){
//			Transform tintText = tintTextPool.GetInstance<Transform> (tintTextModel.gameObject, tintTextContainer);
//			tintText.GetComponent<Text> ().text = tint;
//			tintText.localPosition = new Vector3 (0, 300, 0);
//			tintText.gameObject.SetActive (true);
//			tintText.DOLocalMoveY (500, 1f).OnComplete (() => {
//				tintText.gameObject.SetActive(false);
//				tintTextPool.AddInstanceToPool(tintText.gameObject);
//			});
//		}

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


		/// <summary>
		/// 显示解锁卷轴详细信息
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetUpUnlockScrollHUD(Item item){
			unlockScrollDetail.SetUpUnlockScrollDetailHUD (item);
		}

		/// <summary>
		/// 解锁卷轴展示界面点击事件原本就会退出展示页面，一般情况下不用主动调用这个方法
		/// </summary>
		public void QuitUnlockScrollHUD(){
			unlockScrollDetail.QuitUnlockScrollDetailHUD ();
		}

		/// <summary>
		/// 显示合成界面
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetUpCraftingRecipesHUD(Item item){
			craftingRecipesDetail.SetUpCraftingRecipesHUD (item);
		}

		/// <summary>
		/// 合成界面点击事件原本就会退出展示页面，一般情况下不用主动调用这个方法
		/// </summary>
		public void QuitCraftingRecipesHUD(){
			craftingRecipesDetail.QuitCraftingRecipesHUD ();
		}


//		public void SetUpRewardFormulaPlane(UnlockScroll unlockScroll){
//
//			ItemModel equipment = GameManager.Instance.gameDataCenter.allItemModels.Find (delegate(ItemModel obj) {
//				return obj.itemId == unlockScroll.unlockedItemId;
//			});
//
//			Transform formulaDetailContainer = unlockScrollDetailPlane.Find ("UnlockScrollDetailContainer");
//
//			Text equipmentName = formulaDetailContainer.Find ("EquipmentName").GetComponent<Text> ();
//			Image equipmentIcon = formulaDetailContainer.Find ("EquipmentIcon").GetComponent<Image> ();
//			Text equipmentDescription = formulaDetailContainer.Find ("EquipmentDescription").GetComponent<Text> ();
//
//			equipmentName.text = equipment.itemName;
//			equipmentDescription.text = equipment.itemDescription;
//
//			Sprite s = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
//				return obj.name == equipment.spriteName;
//			});
//
//			equipmentIcon.sprite = s;
//
//			materialCardPool.AddChildInstancesToPool (materialCardContainer);
//
////			for (int i = 0; i < equipment.itemIdsForProduce.Length; i++) {
////				
////				Transform materialCard = materialCardPool.GetInstance<Transform> (materialCardModel.gameObject, materialCardContainer);
////
////				int itemId = equipment.itemIdsForProduce [i];
////
////				Item item = Item.NewItemWith (itemId, 1);
////
////				Image materialIcon = materialCard.Find ("MaterialIcon").GetComponent<Image> ();
////				Text materialName = materialCard.Find ("MaterialName").GetComponent<Text> ();
////
////				materialName.text = item.itemName;
////
////				Sprite materialSprite = GameManager.Instance.gameDataCenter.allMaterialSprites.Find (delegate(Sprite obj) {
////					return obj.name == item.spriteName;
////				});
////
////				materialIcon.sprite = materialSprite;
////
////
////			}
//
//			Button confirmButton = formulaDetailContainer.Find ("ConfirmButton").GetComponent<Button> ();
//
//			confirmButton.onClick.RemoveAllListeners ();
//
//			confirmButton.onClick.AddListener (delegate() {
//				AddFormulaAndQuitFormulaDetailPlane (unlockScroll);
//			});
//
//			unlockScrollDetailPlane.gameObject.SetActive (true);
//		}

//		private void AddFormulaAndQuitFormulaDetailPlane(UnlockScroll unlockScroll){
//
//			Player.mainPlayer.AddItem (unlockScroll);
//
//			unlockScrollDetailPlane.gameObject.SetActive (false);
//
//		}

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

		public void SetUpSpellView(){

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.spellCanvasBundleName, "SpellCanvas", () => {
				ItemModel swordModel = GameManager.Instance.gameDataCenter.allItemModels.Find(delegate(ItemModel obj){
					return obj.itemId == 0;
				});
				TransformManager.FindTransform ("SpellCanvas").GetComponent<SpellViewController> ().SetUpSpellViewForCreate (swordModel,null);
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


		private void UnlockItemCallBack(){
			UnlockScroll currentSelectedUnlockScroll= unlockScrollDetail.unlockScroll;
			currentSelectedUnlockScroll.unlocked = true;
			Player.mainPlayer.RemoveItem (currentSelectedUnlockScroll,1);
			string tint = string.Format ("解锁拼写 <color=green>{0}</color>", currentSelectedUnlockScroll.itemName);
			SetUpTintHUD (tint);
		}

		private void ResolveScrollCallBack(){
			
			UnlockScroll currentSelectUnlockScroll = unlockScrollDetail.unlockScroll;

			List<char> charactersReturn = Player.mainPlayer.ResolveItemAndGetCharacters (currentSelectUnlockScroll,1);

			StringBuilder tint = new StringBuilder ();

			tint.Append ("分解获得字母碎片");

			for (int i = 0; i < charactersReturn.Count; i++) {
				tint.Append (charactersReturn [i]);
			}

			tintHUD.SetUpTintHUD (tint.ToString());
		}

		private void CraftItemCallBack(){

			CraftingRecipes craftingRecipes = craftingRecipesDetail.craftingRecipes;

			ItemModel craftItemModel = GameManager.Instance.gameDataCenter.allItemModels.Find (delegate(ItemModel obj) {
				return obj.itemId == craftingRecipes.craftItemId;
			});

			for (int i = 0; i < craftItemModel.itemInfosForProduce.Length; i++) {
				ItemModel.ItemInfoForProduce itemInfo = craftItemModel.itemInfosForProduce [i];
				for (int j = 0; j < itemInfo.itemCount; j++) {
					Item item = Player.mainPlayer.allItemsInBag.Find (delegate (Item obj) {
						return obj.itemId == itemInfo.itemId;
					});
					Player.mainPlayer.RemoveItem (item,1);
				}
			}

			Item craftedItem = Item.NewItemWith (craftItemModel,1);
			Player.mainPlayer.AddItem (craftedItem);

			string tint = string.Format ("获得 <color=green>{0}</color> x1", craftedItem.itemName);

			SetUpTintHUD (tint);

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
