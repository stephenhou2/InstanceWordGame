using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace WordJourney
{
	public class BattlePlayerUIController : BattleAgentUIController {


		private Player player;

		//魔法槽
		public Slider manaBar;
		//魔法值
		public Text manaText;

		public Transform skillsContainer;
		private Transform skillButtonModel;
	
		/**********  ConsumablesPlane UI *************/
		public Transform allConsumablesPlane;
		public Transform allConsumablesContainer;
		private Transform consumablesButtonModel;
		private InstancePool consumablesButtonPool;
		/**********  ConsumablesPlane UI *************/

		public Button allConsumablesButton;
		public Button healthBottleButton;
		public Button manaBottleButton;
		public Button antiDebuffButton;

		public Transform toolChoicesPlane;
		public Transform toolChoicesContaienr;
		private InstancePool toolChoiceButtonPool;
		private Transform toolChoiceButtonModel;
			
		private Sequence mSequence;

		private List<Button> skillButtons = new List<Button>();

		private InstancePool skillButtonPool;

		private CallBack<int> skillSelectCallBack;




		/// <summary>
		/// 初始化探索界面中玩家UI
		/// 包括：人物状态栏 底部物品栏 战斗中的技能栏 所有消耗品显示栏
		/// </summary>
		/// <param name="player">Player.</param>
		/// <param name="skillSelectCallBack">Skill select call back.</param>
		public void SetUpExplorePlayerView(Player player,CallBack<int> skillSelectCallBack){

			this.player = player;
			this.skillSelectCallBack = skillSelectCallBack;

			Transform poolContainerOfExploreCanvas = TransformManager.FindOrCreateTransform (CommonData.poolContainerName + "/PoolContainerOfExploreCanvas");
			Transform modelContainerOfExploreScene = TransformManager.FindOrCreateTransform(CommonData.instanceContainerName + "/ModelContainerOfExploreScene");


			skillButtonPool = InstancePool.GetOrCreateInstancePool ("SkillButtonPool",poolContainerOfExploreCanvas.name);
			consumablesButtonPool = InstancePool.GetOrCreateInstancePool ("ConsumablesButtonPool", poolContainerOfExploreCanvas.name);
			toolChoiceButtonPool = InstancePool.GetOrCreateInstancePool ("ToolChoiceButtonPool", poolContainerOfExploreCanvas.name);


			skillButtonModel = TransformManager.FindTransform ("SkillButtonModel");
			consumablesButtonModel = TransformManager.FindTransform ("ConsumablesButtonModel");
			toolChoiceButtonModel = TransformManager.FindTransform ("ToolChoiceButtonModel");

			consumablesButtonModel.SetParent (modelContainerOfExploreScene);
			skillButtonModel.SetParent (modelContainerOfExploreScene);
			toolChoiceButtonModel.SetParent (modelContainerOfExploreScene);

			SetUpPlayerStatusPlane ();
			UpdateItemButtons ();

		}


		/// <summary>
		/// 初始化人物状态栏
		/// </summary>
		private void SetUpPlayerStatusPlane(){

			healthBar.maxValue = player.maxHealth;
			manaBar.maxValue = player.maxMana;

			healthText.text = string.Format ("{0}/{1}", player.health, player.maxHealth);
			manaText.text = string.Format ("{0}/{1}", player.mana, player.maxMana);

			healthBar.value = player.health;
			manaBar.value = player.mana;

		}

		/// <summary>
		/// 更新人物状态栏
		/// </summary>
		public void UpdatePlayerStatusPlane(){
			UpdateHealthBarAnim(player);
			UpdateManaBarAnim (player);
		}

		/// <summary>
		/// 初始化人物技能栏
		/// </summary>
		/// <param name="player">Player.</param>
		public void SetUpPlayerSkillPlane(Player player){

			skillButtons.Clear ();

			for (int i = 0; i < player.equipedSkills.Count; i++) {

				Skill skill = player.equipedSkills [i];

				Button skillButton = skillButtonPool.GetInstance<Button> (skillButtonModel.gameObject, skillsContainer);

				Image skillIcon = skillButton.transform.Find ("SkillIcon").GetComponent<Image> ();
				Text skillName = skillButton.transform.Find ("SkillName").GetComponent<Text> ();
				Text manaConsume = skillButton.transform.Find ("ManaConsume").GetComponent<Text> ();


				Sprite sprite = GameManager.Instance.gameDataCenter.allSkillSprites.Find(delegate (Sprite s){
					return s.name == skill.skillIconName;
				});

				if (sprite != null) {
					skillIcon.sprite = sprite;
					skillIcon.enabled = true;
				}

				skillName.text = skill.skillName;
				manaConsume.text = skill.manaConsume.ToString();

				UpdateSkillButtonsStatus ();

				int index = i;

				skillButton.onClick.RemoveAllListeners ();
				skillButton.onClick.AddListener (delegate() {
					skillSelectCallBack(new int[]{index});
					SkillButtonCoolen(skill);
				});

				skillButtons.Add (skillButton);

			}

			skillsContainer.gameObject.SetActive (true);

		}

		/// <summary>
		/// 技能按钮的冷却(所有技能共同进入冷却)
		/// </summary>
		private void SkillButtonCoolen(Skill skill){

			for (int i = 0; i < skillButtons.Count; i++) {
				
				Button skillButton = skillButtons [i];

				skillButton.interactable = false;

				Image coolenMask = skillButton.transform.Find ("CoolenMask").GetComponent<Image> ();

				coolenMask.enabled = true;

				coolenMask.fillAmount = 1;

				coolenMask.DOFillAmount (0, skill.coolenInterval).OnComplete (() => {
					coolenMask.enabled = false;
					UpdateSkillButtonsStatus ();
				});
			}
				
		}

		/// <summary>
		/// 更新技能按钮是否可交互，根据玩家魔法值更新技能所需魔法的显示颜色
		/// </summary>
		private void UpdateSkillButtonsStatus(){
			
			for (int i = 0; i < skillButtons.Count; i++) {
				
				Button skillButton = skillButtons [i];
				Skill skill = player.equipedSkills [i];
				Text manaConsume = skillButton.GetComponentInChildren<Text> ();

				skillButton.interactable = player.mana >= skill.manaConsume;

				if (!skillButton.interactable) {
					manaConsume.color = Color.red;
				} else {
					manaConsume.color = Color.green;
				}

			}
		}

		/// <summary>
		/// 更新底部物品栏状态
		/// </summary>
		public void UpdateItemButtons(){

			// 背包中的血瓶
			Item healthBottle = player.allConsumablesInBag.Find (delegate(Consumables obj) {
				return obj.itemId == 500;
			});

			// 背包中的蓝屏
			Item manaBottle = player.allConsumablesInBag.Find (delegate(Consumables obj) {
				return obj.itemId == 501;
			});

			// 背包中的回程卷轴
			Item TP = player.allConsumablesInBag.Find (delegate(Consumables obj) {
				return obj.itemId == 514;
			});

			SetUpItemButton (healthBottle, healthBottleButton);
			SetUpItemButton (manaBottle, manaBottleButton);
			SetUpItemButton (TP, antiDebuffButton);

		}

		/// <summary>
		/// 更新底部物品栏上对应消耗品的按钮状态
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="itemButton">Item button.</param>
		private void SetUpItemButton(Item item,Button itemButton){

			if (item == null) {
				itemButton.interactable = false;
				itemButton.GetComponentInChildren<Text> ().text = "0";
			} else {
				itemButton.interactable = true;
				itemButton.GetComponentInChildren<Text> ().text = item.itemCount.ToString ();
			}


		}


		/// <summary>
		/// 背包按钮点击响应
		/// </summary>
		public void OnBagButtonClick(){

			// 初始化背包界面并显示
			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.bagCanvasBundleName, "BagCanvas", () => {
				Transform bagCanvas = TransformManager.FindTransform("BagCanvas");
				bagCanvas.GetComponent<BagViewController>().SetUpBagView();
				bagCanvas.GetComponent<Canvas>().enabled =true;
				bagCanvas.gameObject.SetActive(true);
			}, false);

		}

		/// <summary>
		/// 底部物品栏上的消耗品按钮点击响应
		/// </summary>
		/// <param name="buttonIndex">Button index.</param>
		public void OnItemButtonClick(int buttonIndex){

			Consumables consumables = null;

			switch (buttonIndex) {
			case 0://生命药剂
				consumables = player.allConsumablesInBag.Find (delegate (Consumables obj) {
					return obj.itemId == 500;
				});
				break;
			case 1://魔法药剂
				consumables = player.allConsumablesInBag.Find (delegate (Consumables obj) {
					return obj.itemId == 501;
				});
				break;
			case 2://回程卷轴
				consumables = player.allConsumablesInBag.Find (delegate (Consumables obj) {
					return obj.itemId == 514;
				});
				break;
			}


			BattlePlayerController bpCtr = GameObject.Find ("BattlePlayer").GetComponent<BattlePlayerController> ();

			bpCtr.UseItem (consumables);

			// 更新底部物品栏和人物状态栏
			UpdateItemButtonsAndStatusPlane ();

		}

		/// <summary>
		/// 打开所有消耗品界面的 箭头按钮 的点击响应
		/// </summary>
		public void OnAllConsumablesButtonClick(){

			// 如果箭头朝下，则退出所有消耗品显示界面
			if (allConsumablesButton.transform.localRotation != Quaternion.identity) {

				QuitAllConsumablesPlane ();

				return;

			}

			// 箭头朝上，初始化所有消耗品显示界面
			SetUpAllConsumablesPlane ();

			allConsumablesPlane.gameObject.SetActive (true);

		}

		/// <summary>
		/// 初始化所有消耗品显示界面
		/// </summary>
		private void SetUpAllConsumablesPlane(){

			consumablesButtonPool.AddChildInstancesToPool (allConsumablesContainer);

			allConsumablesButton.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, 180));

			for (int i = 0; i < Player.mainPlayer.allConsumablesInBag.Count; i++) {

				Consumables consumables = Player.mainPlayer.allConsumablesInBag [i];

				Button consumablesButton = consumablesButtonPool.GetInstance<Button> (consumablesButtonModel.gameObject, allConsumablesContainer);

				Image consumablesIcon = consumablesButton.GetComponent<Image> ();
				Text consumablesCount = consumablesButton.GetComponentInChildren<Text> ();

				Sprite s = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
					return obj.name == consumables.spriteName;
				});

				consumablesIcon.sprite = s;

				consumablesCount.text = consumables.itemCount.ToString ();

				consumablesButton.onClick.RemoveAllListeners ();

				consumablesButton.onClick.AddListener (delegate {

					BattlePlayerController bpCtr = GameObject.Find ("BattlePlayer").GetComponent<BattlePlayerController> ();

					bpCtr.UseItem (consumables);

					SetUpAllConsumablesPlane();

				});

			}

		}

		/// <summary>
		/// 退出所有消耗品显示栏
		/// </summary>
		public void QuitAllConsumablesPlane(){

			allConsumablesButton.transform.localRotation = Quaternion.identity;

			allConsumablesPlane.gameObject.SetActive (false);

		}

		/// <summary>
		/// 更新底部物品栏和人物状态栏
		/// </summary>
		public void UpdateItemButtonsAndStatusPlane(){

			UpdateItemButtons ();
			UpdatePlayerStatusPlane ();

		}

		/// <summary>
		/// 更新人物魔法槽
		/// </summary>
		/// <param name="ba">Ba.</param>
		private void UpdateManaBarAnim(Agent ba){
			
			manaBar.maxValue = ba.maxMana;
			manaText.text = ba.mana + "/" + ba.maxMana;


			if (firstSetManaBar) {
				manaBar.value = ba.mana;
			} else {
				manaBar.DOValue (ba.mana, 0.2f);
			}

		}

		/// <summary>
		/// 初始化工具选择栏
		/// </summary>
		/// <param name="mapItem">Map item.</param>
		public void SetUpToolChoicePlane(MapItem mapItem){

			Consumables consumablesAsTool = null;
			Equipment equipedWeapon = null;

			switch (mapItem.mapItemType) {
			case MapItemType.Obstacle:

				// 查找背包中的十字镐
				consumablesAsTool = player.allConsumablesInBag.Find (delegate(Consumables obj) {
					return obj.itemId == 512;	
				});

				break;
			case MapItemType.TreasureBox:

				// 查找背包中的钥匙
				consumablesAsTool = player.allConsumablesInBag.Find (delegate(Consumables obj) {
					return obj.itemId == 513;	
				});
				break;
			}

			// 获得玩家已装备的武器
			equipedWeapon = player.allEquipedEquipments.Find (delegate(Equipment obj) {
				return obj.equipmentType == EquipmentType.Weapon;
			});

			// 如果玩家既没有装备武器，背包中又没有对应的工具，则显示提示栏
			if (consumablesAsTool == null && equipedWeapon == null) {
				string tint = "这里好像过不去";
				GetComponent<ExploreUICotroller>().SetUpTintHUD (tint);
				return;
			}

			// 如果背包中又对应的工具，则将该工具的按钮加入到工具选择栏中
			if (consumablesAsTool != null) {

				Transform toolChoiceButton = toolChoiceButtonPool.GetInstance<Transform> (toolChoiceButtonModel.gameObject, toolChoicesContaienr);

				Image toolIcon = toolChoiceButton.Find ("ToolIcon").GetComponent<Image> ();

				Text toolCount = toolChoiceButton.Find ("ToolCount").GetComponent<Text> ();

				Sprite s = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
					return obj.name == consumablesAsTool.spriteName;
				});

				if (s != null) {
					toolIcon.sprite = s;
				}

				toolCount.text = consumablesAsTool.itemCount.ToString ();

				toolChoiceButton.GetComponent<Button> ().onClick.RemoveAllListeners ();

				toolChoiceButton.GetComponent<Button> ().onClick.AddListener (delegate() {
					OnToolChoiceButtonClick(consumablesAsTool,mapItem);
				});

			}

			// 如果玩家装备的又武器，则将武器按钮加入到工具选择栏中
			if (equipedWeapon != null) {

				Transform weaponChoiceButton = toolChoiceButtonPool.GetInstance<Transform> (toolChoiceButtonModel.gameObject, toolChoicesContaienr);
				Image weaponIcon = weaponChoiceButton.Find ("ToolIcon").GetComponent<Image> ();

				Sprite weaponChoiceButtonSprite = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
					return obj.name == "";
				});

				weaponIcon.sprite = weaponChoiceButtonSprite;

				weaponChoiceButton.GetComponent<Button> ().onClick.RemoveAllListeners ();

				weaponChoiceButton.GetComponent<Button> ().onClick.AddListener (delegate() {
					OnToolChoiceButtonClick (null, mapItem);
				});
			}

			toolChoicesPlane.gameObject.SetActive (true);

		}

		/// <summary>
		/// 选择了一种工具后的响应方法
		/// </summary>
		/// <param name="tool">Tool.</param>
		/// <param name="mapItem">Map item.</param>
		private void OnToolChoiceButtonClick(Consumables tool,MapItem mapItem){

			ExploreUICotroller expUICtr = GetComponent<ExploreUICotroller> ();
			string tint = string.Empty;
			Item rewardItem = null;

			QuitToolChoicePlane ();

			// 使用武器破坏
			if (tool == null) {

				Equipment equipment = player.allEquipedEquipments.Find (delegate(Equipment obj) {
					return obj.equipmentType == EquipmentType.Weapon;
				});

				// 播放对应的音效
				GameManager.Instance.soundManager.PlayClips (
					GameManager.Instance.gameDataCenter.allExploreAudioClips, 
					SoundDetailTypeName.Map, 
					mapItem.mapItemName);

				// 播放地图物品对应的动画
				mapItem.UnlockOrDestroyMapItem(()=>{

					if (mapItem.walkableAfterUnlockOrDestroy) {
						TransformManager.FindTransform("ExploreManager").GetComponent<MapGenerator>().mapWalkableInfoArray [(int)mapItem.transform.position.x, (int)mapItem.transform.position.y] = 1;
					}

//					GetComponent<ExploreUICotroller>().HideMask();

					// 判断武器是否被完全损坏
					bool completeDamaged = equipment.EquipmentDamaged (EquipmentDamageSource.DestroyObstacle);

					if (completeDamaged) {
						tint = string.Format ("{0}完全损坏", equipment.itemName);
						expUICtr.SetUpTintHUD (tint);
					}


					switch (mapItem.mapItemType) {
					case MapItemType.Obstacle:
						break;
					case MapItemType.TreasureBox:

						bool unlockSuccess = false;

						// 使用武器开箱子20%的几率可以拿到箱子里的东西，80%的概率拿不到东西
						int seed = Random.Range (0, 10);

						if (seed <= 1) {
							unlockSuccess = true;
						}

						if (unlockSuccess) {
							rewardItem = (mapItem as TreasureBox).rewardItem;
							TransformManager.FindTransform("ExploreManager").GetComponent<MapGenerator>().SetUpRewardInMap(rewardItem,mapItem.transform.position);
//							GetComponent<ExploreUICotroller>().SetUpRewardItemsPlane(rewardItem);
						}
						break;
					}

				});

			} else {//如果使用工具开箱子或清理障碍物
				
				// 背包中的工具数量-1
				player.RemoveItem (new Consumables (tool, 1));

				// 播放对应的音效
				GameManager.Instance.soundManager.PlayClips (
					GameManager.Instance.gameDataCenter.allExploreAudioClips, 
					SoundDetailTypeName.Map, 
					mapItem.mapItemName);

				// 播放地图物品对应的动画
				mapItem.UnlockOrDestroyMapItem (()=>{

					if (mapItem.walkableAfterUnlockOrDestroy) {
						TransformManager.FindTransform("ExploreManager").GetComponent<MapGenerator>().mapWalkableInfoArray [(int)mapItem.transform.position.x, (int)mapItem.transform.position.y] = 1;
					}

					switch (mapItem.mapItemType) {
					// 十字镐清理障碍物有随机获得稀有材料（暂时设定为必出）
					case MapItemType.Obstacle:
						
						List<Material> allMaterials = GameManager.Instance.gameDataCenter.allMaterials;

						int rareMaterialIndex = Random.Range (0, allMaterials.Count + 1);

						Material rareMaterial = allMaterials [rareMaterialIndex];

						// 奖励一种随机稀有材料
						rewardItem = new Material(rareMaterial,1);

						TransformManager.FindTransform("ExploreManager").GetComponent<MapGenerator>().SetUpRewardInMap(rewardItem,mapItem.transform.position);

//						// 显示奖励栏
//						GetComponent<ExploreUICotroller>().SetUpRewardItemsPlane(rewardItem);

						break;
						// 钥匙开宝箱一定成功
					case MapItemType.TreasureBox:
						
						rewardItem = (mapItem as TreasureBox).rewardItem;

						TransformManager.FindTransform("ExploreManager").GetComponent<MapGenerator>().SetUpRewardInMap(rewardItem,mapItem.transform.position);

//						GetComponent<ExploreUICotroller>().SetUpRewardItemsPlane(rewardItem);

						break;
					}

				});
			}
		}


		/// <summary>
		/// 退出工具选择栏
		/// </summary>
		public void QuitToolChoicePlane(){

//			GetComponent<ExploreUICotroller>().HideMask();

			toolChoiceButtonPool.AddChildInstancesToPool (toolChoicesContaienr);

			toolChoicesPlane.gameObject.SetActive (false);

		}



		/// <summary>
		/// 退出战斗时的逻辑
		/// </summary>
		public override void QuitFight(){

			skillButtonPool.AddChildInstancesToPool (skillsContainer);

			skillsContainer.gameObject.SetActive (false);

		}
	}
}
