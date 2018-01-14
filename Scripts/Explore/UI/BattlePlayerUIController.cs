using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace WordJourney
{
	public class BattlePlayerUIController : BattleAgentUIController {

		private Player player;

		public Text coinCount;

		public Button[] equipedConsumablesButtons;

//		public Transform skillsContainer;
//		private Transform skillButtonModel;
	
		/**********  ConsumablesPlane UI *************/
		public Transform consumablesInBagPlane;
		public Transform consumablesInBagContainer;
		public Transform consumablesButtonModel;
		private InstancePool consumablesButtonPool;
		/**********  ConsumablesPlane UI *************/

		public Button showConsumablesInBagButton;

		public Transform toolChoicesPlane;
		public Transform toolChoicesContaienr;
		private InstancePool toolChoiceButtonPool;
		public Transform toolChoiceButtonModel;


		private ExploreManager mExploreManager;
		private ExploreManager exploreManager{
			get{
				if (mExploreManager == null) {
					mExploreManager = TransformManager.FindTransform ("ExploreManager").GetComponent<ExploreManager>();
				}
				return mExploreManager;
			}
		}

		private BattlePlayerController mBpCtr;
		private BattlePlayerController bpCtr{
			get{
				if (mBpCtr == null) {
					mBpCtr = player.transform.Find("BattlePlayer").GetComponent<BattlePlayerController> ();
				}
				return mBpCtr;
			}
		}

		public AttackCheckController attackCheckController;


		private int consumablesCountInOnePage = 6;

		private int currentConsumablesPage;

		public Button nextPageButton;
		public Button lastPageButton;



		/// <summary>
		/// 初始化探索界面中玩家UI
		/// 包括：人物状态栏 底部物品栏 战斗中的技能栏 所有消耗品显示栏
		/// </summary>
		/// <param name="player">Player.</param>
		/// <param name="skillSelectCallBack">Skill select call back.</param>
		public void SetUpExplorePlayerView(Player player){
			
			currentConsumablesPage = 0;

			this.player = player;

			Transform poolContainerOfExploreCanvas = TransformManager.FindOrCreateTransform (CommonData.poolContainerName + "/PoolContainerOfExploreCanvas");
			Transform modelContainerOfExploreScene = TransformManager.FindOrCreateTransform(CommonData.instanceContainerName + "/ModelContainerOfExploreScene");

			consumablesButtonPool = InstancePool.GetOrCreateInstancePool ("ConsumablesButtonPool", poolContainerOfExploreCanvas.name);
			toolChoiceButtonPool = InstancePool.GetOrCreateInstancePool ("ToolChoiceButtonPool", poolContainerOfExploreCanvas.name);

//			toolChoiceButtonModel = TransformManager.FindTransform ("ToolChoiceButtonModel");

//			consumablesButtonModel.SetParent (modelContainerOfExploreScene);
//			toolChoiceButtonModel.SetParent (modelContainerOfExploreScene);

			SetUpPlayerStatusPlane ();
			SetUpBottomConsumablesButtons ();

		}


		/// <summary>
		/// 初始化人物状态栏
		/// </summary>
		private void SetUpPlayerStatusPlane(){

			healthBar.maxValue = player.maxHealth;
			coinCount.text = player.totalCoins.ToString ();

			healthText.text = string.Format ("{0}/{1}", player.health, player.maxHealth);

			healthBar.value = player.health;

		}

		/// <summary>
		/// 更新人物状态栏
		/// </summary>
		public override void UpdateAgentStatusPlane(){
			UpdateHealthBarAnim(player);
			UpdateSkillStatusPlane (player);
			coinCount.text = player.totalCoins.ToString ();
			if (bpCtr.isInFight) {
				attackCheckController.UpdateHealth ();
			}
		}
			


		/// <summary>
		/// 更新底部物品栏状态
		/// </summary>
		public void SetUpBottomConsumablesButtons(){

			int totalConsumablesCount = player.allConsumablesInBag.Count;

			for (int i = 0; i < equipedConsumablesButtons.Length; i++) {

				Button equipedConsumablesButton = equipedConsumablesButtons [i];

				if (i < totalConsumablesCount) {

					Consumables consumables = player.allConsumablesInBag [i];

					UpdateBottomConsumablesButton (consumables, equipedConsumablesButton);

				} else {
					UpdateBottomConsumablesButton (null, equipedConsumablesButton);
				}

			}
		}

		/// <summary>
		/// 更新底部物品栏上对应消耗品的按钮状态
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="itemButton">Item button.</param>
		private void UpdateBottomConsumablesButton(Consumables consumables,Button consumablesButton){

			Image consumablesIcon = consumablesButton.transform.Find ("ConsumablesIcon").GetComponent<Image> ();
			Text cosumbablesCount = consumablesButton.GetComponentInChildren<Text> ();

			if (consumables == null) {
				consumablesButton.interactable = false;
				consumablesIcon.enabled = false;
				cosumbablesCount.text = "";
				return;

			}

			consumablesButton.interactable = true;

			Sprite consumablesSprite = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
				return obj.name == consumables.spriteName;
			});
			consumablesIcon.sprite = consumablesSprite;
			consumablesIcon.enabled = true;
			cosumbablesCount.text = consumables.itemCount.ToString ();

		}


		/// <summary>
		/// 背包按钮点击响应
		/// </summary>
		public void OnBagButtonClick(){

			Time.timeScale = 0;

			// 初始化背包界面并显示
			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.bagCanvasBundleName, "BagCanvas", () => {
				Transform bagCanvas = TransformManager.FindTransform("BagCanvas");
				bagCanvas.GetComponent<BagViewController>().SetUpBagView(true);
			}, false,true);

		}
			

		/// <summary>
		/// 打开所有消耗品界面的 箭头按钮 的点击响应
		/// </summary>
		public void OnShowConsumablesInBagButtonClick(){

			currentConsumablesPage = 0;

			// 如果箭头朝下，则退出所有消耗品显示界面
			if (showConsumablesInBagButton.transform.localRotation != Quaternion.identity) {

				QuitConsumablesInBagPlane ();

				return;

			}

			showConsumablesInBagButton.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, 180));

			Time.timeScale = 0f;

			// 箭头朝上，初始化剩余的消耗品显示界面
			SetUpConsumablesInBagPlane ();

			consumablesInBagPlane.gameObject.SetActive (true);

		}

		private void UpdatePageButtonStatus(){

			bool nextButtonEnable = player.allConsumablesInBag.Count > equipedConsumablesButtons.Length + (currentConsumablesPage + 1) * consumablesCountInOnePage;
			bool lastButtonEnable = currentConsumablesPage >= 1;

			nextPageButton.gameObject.SetActive (nextButtonEnable);
			lastPageButton.gameObject.SetActive (lastButtonEnable);

		}

		/// <summary>
		/// 初始化所有消耗品显示界面
		/// </summary>
		public void SetUpConsumablesInBagPlane(){
			
			UpdatePageButtonStatus ();

			consumablesButtonPool.AddChildInstancesToPool (consumablesInBagContainer);

			if (player.allConsumablesInBag.Count <= equipedConsumablesButtons.Length) {
				return;
			}
				
			int firstIndexOfCurrentPage = equipedConsumablesButtons.Length + currentConsumablesPage * consumablesCountInOnePage; 

			int firstIndexOfNextPage = firstIndexOfCurrentPage + consumablesCountInOnePage;

			int endIndexOfConsumablesInCurrentPage = player.allConsumablesInBag.Count < firstIndexOfNextPage ? player.allConsumablesInBag.Count - 1 : firstIndexOfNextPage - 1;

			for (int i = firstIndexOfCurrentPage; i <= endIndexOfConsumablesInCurrentPage; i++) {

				Consumables consumables = Player.mainPlayer.allConsumablesInBag [i];

				AddConsumablesButton (consumables);

			}

		}

		public void OnNextPageButtonClick(){
			currentConsumablesPage++;
			SetUpConsumablesInBagPlane ();
		}

		public void OnLastPageButtonClick(){
			currentConsumablesPage--;
			SetUpConsumablesInBagPlane ();
		}

		public void AddConsumablesButton(Consumables consumables){

			Button consumablesButton = consumablesButtonPool.GetInstance<Button> (consumablesButtonModel.gameObject, consumablesInBagContainer);

			consumablesButton.GetComponent<ConsumablesInBagDragControl> ().item = consumables;

			Image consumablesIcon = consumablesButton.transform.Find ("ConsumablesIcon").GetComponent<Image> ();
			Text consumablesCount = consumablesButton.transform.Find ("ConsumablesCount").GetComponent<Text> ();
			Text consumablesName = consumablesButton.transform.Find ("ConsumablesName").GetComponent<Text> ();
			Image newItemTint = consumablesButton.transform.Find ("NewItemTint").GetComponent<Image> ();

			Sprite s = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
				return obj.name == consumables.spriteName;
			});

			consumablesIcon.sprite = s;

			consumablesIcon.enabled = true;

			consumablesCount.text = consumables.itemCount.ToString ();

			newItemTint.enabled = consumables.isNewItem;

			consumablesName.text = consumables.itemName;

		}

		public void OnEquipedConsumablesButtonClick(int indexInPanel){
			Consumables consumables = player.allConsumablesInBag [indexInPanel];
			OnConsumablesButtonClick (consumables);
		}


		public void OnConsumablesButtonClick(Consumables consumables){
			
			bool consumblesUsedInExploreScene = false;

			switch (consumables.itemName) {
			case "药剂":
			case "草药":
			case "蓝莓":
			case "菠菜":
			case "胡萝卜":
			case "樱桃":
			case "南瓜":
			case "蘑菇":
			case "辣椒":
				Player.mainPlayer.UseMedicines (consumables);
				consumblesUsedInExploreScene = false;
				break;  
			case "卷轴":
				exploreManager.GetComponent<ExploreManager> ().QuitExploreScene (true);
				consumblesUsedInExploreScene = false;
				break;
			case "锄头":
			case "锯子":
			case "镰刀":
			case "钥匙":
			case "树苗":
			case "火把":
			case "水":
			case "地板":
			case "开关":
			case "土块":
				consumblesUsedInExploreScene = true;
				break;
			}

			if (consumblesUsedInExploreScene) {
				QuitConsumablesInBagPlane ();
				exploreManager.clickForConsumablesPos = true;
				exploreManager.ShowConsumablesValidPointTintAround (consumables);

			} else {
				SetUpBottomConsumablesButtons ();
				SetUpConsumablesInBagPlane ();
			}


		}


		public void OnProduceButtonClick(){
			Time.timeScale = 0f;
			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.spellCanvasBundleName, "SpellCanvas", () => {
				TransformManager.FindTransform("SpellCanvas").GetComponent<SpellViewController>().SetUpSpellViewForCreate(null,null);
			}, false, true);
		}


		/// <summary>
		/// 退出所有消耗品显示栏
		/// </summary>
		public void QuitConsumablesInBagPlane(){

			Time.timeScale = 1f;

			showConsumablesInBagButton.transform.localRotation = Quaternion.identity;

			consumablesInBagPlane.gameObject.SetActive (false);

		}

		/// <summary>
		/// 更新底部物品栏和人物状态栏
		/// </summary>
		public void UpdateItemButtonsAndStatusPlane(){

			SetUpBottomConsumablesButtons ();
			UpdateAgentStatusPlane ();

		}

		public void SetUpFightAttackCheck(){
//			attackCheckController.StartRectAttackCheck ();
			attackCheckController.StartCircleAttackCheck();
		}
			
		/// <summary>
		/// 初始化工具选择栏
		/// </summary>
		/// <param name="mapItem">Map item.</param>
		public void SetUpToolChoicePlane(MapItem mapItem, Consumables tool){


			Transform toolChoiceButton = toolChoiceButtonPool.GetInstance<Transform> (toolChoiceButtonModel.gameObject, toolChoicesContaienr);

			Image toolIcon = toolChoiceButton.Find ("ToolIcon").GetComponent<Image> ();

			Text toolCount = toolChoiceButton.Find ("ToolCount").GetComponent<Text> ();

			Sprite s = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
				return obj.name == tool.spriteName;
			});

			if (s != null) {
				toolIcon.sprite = s;
			}

			toolCount.text = tool.itemCount.ToString ();

			toolChoiceButton.GetComponent<Button> ().onClick.RemoveAllListeners ();

			toolChoiceButton.GetComponent<Button> ().onClick.AddListener (delegate() {
				OnToolChoiceButtonClick(tool,mapItem);
			});

			toolChoicesPlane.gameObject.SetActive (true);

		}

		/// <summary>
		/// 选择了一种工具后的响应方法
		/// </summary>
		/// <param name="tool">Tool.</param>
		/// <param name="mapItem">Map item.</param>
		private void OnToolChoiceButtonClick(Consumables tool,MapItem mapItem){

			QuitToolChoicePlane ();
				
			// 背包中的工具数量-1
			player.RemoveItem (tool, 1);

			// 播放对应的音效
			SoundManager.Instance.PlayAudioClip("MapEffects/" + mapItem.audioClipName);

			Vector3 mapItemPos = mapItem.transform.position;
			MapGenerator mapGenerator = TransformManager.FindTransform ("ExploreManager").GetComponent<MapGenerator> ();
			int[,] mapWalkableInfoArray = mapGenerator.mapWalkableInfoArray;

			switch (mapItem.mapItemType) {
			case MapItemType.Stone:
			case MapItemType.Tree:
				(mapItem as Obstacle).DestroyObstacle (null);
				mapWalkableInfoArray [(int)mapItemPos.x, (int)mapItemPos.y] = 1;
				break;
			case MapItemType.TreasureBox:
				TreasureBox tb = mapItem as TreasureBox;
				tb.UnlockTreasureBox (null);
				mapWalkableInfoArray [(int)mapItemPos.x, (int)mapItemPos.y] = 1;
				mapGenerator.SetUpRewardInMap (tb.rewardItem, mapItemPos);
				break;
			case MapItemType.Plant:
				Plant plant = mapItem as Plant;
				mapGenerator.AddMapItemInPool (mapItem.transform);
				mapWalkableInfoArray [(int)mapItemPos.x, (int)mapItemPos.y] = 1;
				mapGenerator.SetUpRewardInMap (plant.attachedItem, mapItemPos);
				break;
			}

			SetUpBottomConsumablesButtons ();

		}


		/// <summary>
		/// 退出工具选择栏
		/// </summary>
		public void QuitToolChoicePlane(){

			toolChoiceButtonPool.AddChildInstancesToPool (toolChoicesContaienr);

			toolChoicesPlane.gameObject.SetActive (false);

		}



		/// <summary>
		/// 退出战斗时的逻辑
		/// </summary>
		public override void QuitFight(){
			attackCheckController.QuitAttackCheck ();
		}
	}
}
