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



		private void SetUpPlayerStatusPlane(){

			healthBar.maxValue = player.maxHealth;
			manaBar.maxValue = player.maxMana;

			healthText.text = string.Format ("{0}/{1}", player.health, player.maxHealth);
			manaText.text = string.Format ("{0}/{1}", player.mana, player.maxMana);

			healthBar.value = player.health;
			manaBar.value = player.mana;

		}

		public void UpdatePlayerStatusPlane(){
			UpdateHealthBarAnim(player);
			UpdateManaBarAnim (player);
		}

		public void SetUpPlayerSkillPlane(Player player){

			skillButtons.Clear ();

//			BattlePlayerController bpCtr = GameObject.Find (CommonData.instanceContainerName, "ExploreManager").GetComponent<BattlePlayerController> ();

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

		private void UpdateSkillButtonsStatus(){
			
			for (int i = 0; i < skillButtons.Count; i++) {
				
				Button skillButton = skillButtons [i];
				Skill skill = player.equipedSkills [i];

				Text manaConsume = skillButton.GetComponentInChildren<Text> ();
//				Image coolenMask = skillButton.transform.FindChild ("CoolenMask").GetComponent<Image>();

				skillButton.interactable = player.mana >= skill.manaConsume;

				if (!skillButton.interactable) {
					manaConsume.color = Color.red;
//					coolenMask.fillAmount = 1;
//					coolenMask.gameObject.SetActive (true);
				} else {
					manaConsume.color = Color.green;
//					coolenMask.gameObject.SetActive (false);
				}

			}
		}


		public void UpdateItemButtons(){

			Item healthBottle = player.allConsumablesInBag.Find (delegate(Consumables obj) {
				return obj.itemId == 500;
			});

			Item manaBottle = player.allConsumablesInBag.Find (delegate(Consumables obj) {
				return obj.itemId == 501;
			});

			Item TP = player.allConsumablesInBag.Find (delegate(Consumables obj) {
				return obj.itemId == 514;
			});

			SetUpItemButton (healthBottle, healthBottleButton);
			SetUpItemButton (manaBottle, manaBottleButton);
			SetUpItemButton (TP, antiDebuffButton);


		}

		private void SetUpItemButton(Item item,Button itemButton){

			if (item == null) {
//				itemButton.GetComponent<Image> ().color = new Color (100, 100, 100, 255);
				itemButton.interactable = false;
				itemButton.GetComponentInChildren<Text> ().text = "0";
			} else {
//				itemButton.GetComponent<Image> ().color = Color.white;
				itemButton.interactable = true;
				itemButton.GetComponentInChildren<Text> ().text = item.itemCount.ToString ();
			}


		}


		public void OnBagButtonClick(){

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.bagCanvasBundleName, "BagCanvas", () => {
				Transform bagCanvas = TransformManager.FindTransform("BagCanvas");
				bagCanvas.GetComponent<BagViewController>().SetUpBagView();
				bagCanvas.GetComponent<Canvas>().enabled =true;
				bagCanvas.gameObject.SetActive(true);
			}, false);

		}


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

			UpdateItemButtonsAndStatusPlane ();

		}

		public void OnAllConsumablesButtonClick(){

			if (allConsumablesButton.transform.localRotation != Quaternion.identity) {

				QuitAllConsumablesPlane ();

				return;

			}

			SetUpAllConsumablesPlane ();

			allConsumablesPlane.gameObject.SetActive (true);

		}

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

		public void QuitAllConsumablesPlane(){

			allConsumablesButton.transform.localRotation = Quaternion.identity;

			allConsumablesPlane.gameObject.SetActive (false);

		}


		public void UpdateItemButtonsAndStatusPlane(){

			UpdateItemButtons ();
			UpdatePlayerStatusPlane ();

		}


		private void UpdateManaBarAnim(Agent ba){
			
			manaBar.maxValue = ba.maxMana;
			manaText.text = ba.mana + "/" + ba.maxMana;


			if (firstSetManaBar) {
				manaBar.value = ba.mana;
			} else {
				manaBar.DOValue (ba.mana, 0.2f);
			}

		}

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

			equipedWeapon = player.allEquipedEquipments.Find (delegate(Equipment obj) {
				return obj.equipmentType == EquipmentType.Weapon;
			});

			if (consumablesAsTool == null && equipedWeapon == null) {
				string tint = "这里好像过不去";
				GetComponent<ExploreUICotroller>().SetUpTintHUD (tint);
				return;
			}

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

		public void QuitToolChoicePlane(){

			GetComponent<ExploreUICotroller>().HideMask();

			toolChoiceButtonPool.AddChildInstancesToPool (toolChoicesContaienr);

			toolChoicesPlane.gameObject.SetActive (false);

		}

		private void OnToolChoiceButtonClick(Consumables tool,MapItem mapItem){

			ExploreUICotroller expUICtr = GetComponent<ExploreUICotroller> ();
			string tint = string.Empty;
			Item[] rewards = null;

			QuitToolChoicePlane ();

			// 使用武器破坏
			if (tool == null) {
				
				Equipment equipment = player.allEquipedEquipments.Find (delegate(Equipment obj) {
					return obj.equipmentType == EquipmentType.Weapon;
				});
					
				GameManager.Instance.soundManager.PlayClips (
					GameManager.Instance.gameDataCenter.allExploreAudioClips, 
					SoundDetailTypeName.Map, 
					mapItem.mapItemName);

				mapItem.UnlockOrDestroyMapItem(()=>{

					if (mapItem.walkableAfterUnlockOrDestroy) {
						TransformManager.FindTransform("ExploreManager").GetComponent<MapGenerator>().mapWalkableInfoArray [(int)mapItem.transform.position.x, (int)mapItem.transform.position.y] = 1;
					}

					GetComponent<ExploreUICotroller>().HideMask();

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

						// 使用武器开箱子20%的几率可以成功，80%的概率拿不到东西
						int seed = Random.Range (0, 10);

						if (seed <= 1) {
							unlockSuccess = true;
						}

						if (unlockSuccess) {
							rewards = (mapItem as TreasureBox).rewardItems;
							GetComponent<ExploreUICotroller>().SetUpRewardItemsPlane(rewards);
						}
						break;
					}
						
				});

			} else {
				// 背包中的工具数量-1
				player.RemoveItem (new Consumables (tool, 1));

				// 播放音效
				GameManager.Instance.soundManager.PlayClips (
					GameManager.Instance.gameDataCenter.allExploreAudioClips, 
					SoundDetailTypeName.Map, 
					mapItem.mapItemName);

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
						rewards = new Item[]{new Material(rareMaterial,1)};
						GetComponent<ExploreUICotroller>().SetUpRewardItemsPlane(rewards);
						break;
						// 钥匙开宝箱一定成功
					case MapItemType.TreasureBox:
						rewards = (mapItem as TreasureBox).rewardItems;
						GetComponent<ExploreUICotroller>().SetUpRewardItemsPlane(rewards);
						break;
					}

				});
			}
		}


		public override void QuitFight(){

			skillButtonPool.AddChildInstancesToPool (skillsContainer);

			skillsContainer.gameObject.SetActive (false);

		}
	}
}
