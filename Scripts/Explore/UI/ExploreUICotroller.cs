using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace WordJourney
{
	using UnityEngine.UI;
	using DG.Tweening;
	using System.Text;

	public class ExploreUICotroller : MonoBehaviour {

//		private enum QueryType{
//			Refresh,
//			Quit
//		}
//
//		private QueryType queryType;


		public TintHUD tintHUD;
		public UnlockScrollDetailHUD unlockScrollDetail;
		public CraftingRecipesHUD craftingRecipesDetail;


		public Transform mask;

		/**********  battlePlane UI *************/
		public Transform battlePlane;

		/**********  battlePlane UI *************/

		public NPCUIController npcUIController;

		public Text gameLevelLocationText;

//		private Dialog[] dialogs;
//		private Choice[] choices;

//		private List<Item> itemsToPickUp = new List<Item>();
		private Item itemToPickUp;

//		public Transform pauseHUD;
//		public Transform queryHUD;
		public PauseHUD pauseHUD;


		public Transform billboardPlane;
		public Transform billboard;


		public Transform crystalQueryHUD;


		public void SetUpExploreCanvas(int gameLevelIndex, string gameLevelLocation){


			unlockScrollDetail.InitUnlockScrollDetailHUD (true, null, UnlockItemCallBack, ResolveScrollCallBack);
			craftingRecipesDetail.InitCraftingRecipesHUD (true, UpdateBottomBar, CraftItemCallBack);
			npcUIController.InitNPCHUD (gameLevelIndex);
			pauseHUD.InitPauseHUD (true, null, null, null, null);

			if (!GameManager.Instance.UIManager.UIDic.ContainsKey ("BagCanvas")) {

				GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.bagCanvasBundleName, "BagCanvas", () => {
					Transform bagCanvas = TransformManager.FindTransform ("BagCanvas");
					bagCanvas.GetComponent<BagViewController> ().SetUpBagView (false);
				}, true,true);
					
			}
				
			string gameLevelInCurrentLocation = MyTool.NumberToChinese(gameLevelIndex % 5 + 1);
			gameLevelLocationText.text = string.Format("{0}  第 {1} 关",gameLevelLocation,gameLevelInCurrentLocation);

			GetComponent<BattlePlayerUIController> ().InitExploreAgentView ();
			GetComponent<BattlePlayerUIController> ().SetUpExplorePlayerView (Player.mainPlayer);
			GetComponent<BattleMonsterUIController> ().InitExploreAgentView ();

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


		public void SetUpTintHUD(string tint,Sprite sprite){
			tintHUD.SetUpTintHUD (tint,sprite);
		}

		/// <summary>
		/// 更新底部消耗品栏
		/// </summary>
		public void UpdateBottomBar(){
			GetComponent<BattlePlayerUIController> ().SetUpBottomConsumablesButtons ();
		}

		public void UpdatePlayerStatusBar(){
			GetComponent<BattlePlayerUIController> ().UpdateAgentStatusPlane ();
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

			npcUIController.SetUpNpcPlane (npc);

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
//			equipmentDescription.text = equipment.itemGeneralDescription;
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

//		public void SetUpSpellView(){
//
//			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.spellCanvasBundleName, "SpellCanvas", () => {
//				ItemModel swordModel = GameManager.Instance.gameDataCenter.allItemModels.Find(delegate(ItemModel obj){
//					return obj.itemId == 0;
//				});
//				TransformManager.FindTransform ("SpellCanvas").GetComponent<SpellViewController> ().SetUpSpellViewForCreate (swordModel,null);
//			}, false, true);
//
//		}

		public void OnPauseButtonClick(){
			ShowPauseHUD ();
		}

		public void ShowPauseHUD(){
			pauseHUD.SetUpPauseHUD ();
		}

		public void QuitPauseHUD(){
			pauseHUD.QuitPauseHUD ();
		}

//		public void OnRefreshButtonClick(){
//			queryType = QueryType.Refresh;
//			ShowQueryHUD ();
//		}

//		public void OnHomeButtonClick(){
//			queryType = QueryType.Quit;
//			ShowQueryHUD ();
//		}

//		public void OnSettingsButtonClick(){
//			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.settingCanvasBundleName, "SettingCanvas", () => {
//				TransformManager.FindTransform("SettingCanvas").GetComponent<SettingViewController>().SetUpSettingView();
//			},false,true);
//			QuitPauseHUD ();
//		}

//		public void ShowQueryHUD(){
//			pauseHUD.SetUpPauseHUD ();
//		}
//		public void QuitQueryHUD(){
//			pauseHUD.QuitPauseHUD ();
//		}



		private void UnlockItemCallBack(){
			UnlockScroll currentSelectedUnlockScroll= unlockScrollDetail.unlockScroll;
			currentSelectedUnlockScroll.unlocked = true;
			Player.mainPlayer.RemoveItem (currentSelectedUnlockScroll,1);
			string tint = string.Format ("解锁拼写 <color=orange>{0}</color>", currentSelectedUnlockScroll.itemName);
			SetUpTintHUD (tint,null);
		}

		private void ResolveScrollCallBack(){
			
			UnlockScroll currentSelectUnlockScroll = unlockScrollDetail.unlockScroll;

			List<char> charactersReturn = Player.mainPlayer.ResolveItemAndGetCharacters (currentSelectUnlockScroll,1);

			StringBuilder tint = new StringBuilder ();

			tint.Append ("获得字母碎片");

			for (int i = 0; i < charactersReturn.Count; i++) {
				tint.Append (charactersReturn [i]);
			}

			tintHUD.SetUpTintHUD (tint.ToString(),null);
		}

		private void CraftItemCallBack(){

			CraftingRecipe craftingRecipe = craftingRecipesDetail.craftingRecipe;

			ItemModel craftItemModel = GameManager.Instance.gameDataCenter.allItemModels.Find (delegate(ItemModel obj) {
				return obj.itemId == craftingRecipe.craftItemId;
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

			string tint = string.Format ("获得 <color=orange>{0}</color> x1", craftedItem.itemName);

			Sprite itemSprite = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
				return obj.name == craftedItem.spriteName;
			});

			SetUpTintHUD (tint,itemSprite);

		}


		public void QuitFight(){
			GetComponent<BattlePlayerUIController> ().QuitFight ();
			GetComponent<BattleMonsterUIController>().QuitFight ();
			HideFightPlane ();
		}



		public void QuitExplore(){

//			TransformManager.DestroyTransform(transform);

			npcUIController.ClearNpcPlaneCache ();

			GetComponent<BattlePlayerUIController> ().ClearCache ();

			DestroyInstances ();

			Resources.UnloadUnusedAssets ();

			System.GC.Collect ();

//			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.homeCanvasBundleName, "HomeCanvas", () => {
//
//				TransformManager.FindTransform("HomeCanvas").GetComponent<HomeViewController> ().SetUpHomeView ();
//
//			});

		}




		public void DestroyInstances(){
			
			GameManager.Instance.UIManager.RemoveCanvasCache ("ExploreCanvas");

			Destroy (this.gameObject);

//			npcUIController.ClearNpcPlaneCache ();

			MyResourceManager.Instance.UnloadAssetBundle (CommonData.exploreSceneBundleName, true);
		}


	}
}
