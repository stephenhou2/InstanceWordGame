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
		public GameObject skillButtonModel;
	

		public Button healthBottleButton;
		public Button manaBottleButton;
		public Button antiDebuffButton;
		public Button bagButton;

//		private Transform mSkillAndItemDetailPlane;
//		public Transform skillAndItemDetailPlane{
//			get{
//				if (mSkillAndItemDetailPlane == null) {
//					mSkillAndItemDetailPlane = GameObject.Find (CommonData.instanceContainerName + "/SkillAndItemDetailPlane").transform;
//				}
//				return mSkillAndItemDetailPlane;
//			}
//
//		}
			
		private Sequence mSequence;

		private List<Sprite> skillSprites;

		private List<Sprite> itemSprites;

		private List<Button> skillButtons = new List<Button>();

		private InstancePool skillButtonPool;

		protected override void Awake(){

			player = Player.mainPlayer;

			skillButtonPool = InstancePool.GetOrCreateInstancePool ("SkillButtonPool");

			base.Awake ();

		}


		public void SetUpExplorePlayerView(Player player,List<Sprite> skillSprites,List<Sprite> itemSprites){

			this.player = player;
			this.skillSprites = skillSprites;
			this.itemSprites = itemSprites;

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

		public void SetUpBattlePlayerPlane(Player player){

			skillButtons.Clear ();

			for (int i = 0; i < player.equipedSkills.Count; i++) {

				Skill skill = player.equipedSkills [i];

				Button skillButton = skillButtonPool.GetInstance<Button> (skillButtonModel, skillsContainer);

				Image skillIcon = skillButton.transform.FindChild ("SkillIcon").GetComponent<Image> ();
				Text skillName = skillButton.transform.FindChild ("SkillName").GetComponent<Text> ();
				Text manaConsume = skillButton.transform.FindChild ("ManaConsume").GetComponent<Text> ();


				Sprite sprite = GameManager.Instance.allSkillSprites.Find(delegate (Sprite s){
					return s.name == skill.skillIconName;
				});

				if (sprite != null) {
					skillIcon.sprite = sprite;
					skillIcon.enabled = true;
				}

				skillName.text = skill.skillName;
				manaConsume.text = skill.manaConsume.ToString();

				skillButton.interactable = player.mana >= skill.manaConsume;

				if (skillButton.interactable) {
					skillIcon.color = Color.white;
				} else {
					skillIcon.color = new Color (100, 100, 100,255);
				}

				skillButtons.Add (skillButton);

			}

		}


		public void UpdateItemButtons(){

			Item healthBottle = player.allItems.Find (delegate(Item obj) {
				return obj.itemNameInEnglish == "health";
			});

			Item manaBottle = player.allItems.Find (delegate(Item obj) {
				return obj.itemNameInEnglish == "mana";
			});

			Item antiDebuffBottle = player.allItems.Find (delegate(Item obj) {
				return obj.itemNameInEnglish == "antiDebuff";
			});

			SetUpItemButton (healthBottle, healthBottleButton);
			SetUpItemButton (manaBottle, manaBottleButton);
			SetUpItemButton (antiDebuffBottle, antiDebuffButton);

//			for (int i = 3; i < player.allEquipedItems.Count; i++) {
//
//				Item consumable = player.allEquipedItems [i];
//
//				Button itemButton = itemButtons [i - 3];
//
//				Image itemIcon = itemButton.GetComponent<Image> ();
//
//				Text itemCount = itemButton.transform.FindChild ("Text").GetComponent<Text> ();
//
//				if (consumable == null) {
//
//					itemButton.interactable = false;
//					itemIcon.enabled = false;
//
//					itemIcon.sprite = null;
//					itemCount.text = string.Empty;
//
//					continue;
//				}
//
//				itemIcon.sprite = itemSprites.Find (delegate(Sprite obj) {
//					return obj.name == consumable.spriteName;
//				});
//				if (itemIcon.sprite != null) {
//					itemIcon.enabled = true;
//					itemButton.interactable = true;
//					itemCount.text = consumable.itemCount.ToString ();
//				}
//
//				itemButton.interactable = (consumable.itemCount > 0);
//			}

		}

		private void SetUpItemButton(Item item,Button itemButton){

			if (item == null) {
				itemButton.GetComponent<Image> ().color = new Color (100, 100, 100, 255);
				itemButton.interactable = false;
				itemButton.GetComponentInChildren<Text> ().text = "0";
			} else {
				itemButton.GetComponent<Image> ().color = Color.white;
				itemButton.interactable = true;
				itemButton.GetComponentInChildren<Text> ().text = item.itemCount.ToString ();
			}


		}


		// 更新战斗中玩家UI的状态
		public void UpdateSkillButtonsStatus(Player player){

			for (int i = 0; i < skillButtons.Count; i++) {

				Button skillButton = skillButtons [i];

//				Skill s = player.equipedSkills [i];
//				// 如果是冷却中的技能
//				if (s.isAvalible == false) {
//					int actionBackCount = s.actionConsume - s.actionCount + 1;
//					skillButtons [i].GetComponentInChildren<Text> ().text = actionBackCount.ToString ();
//				} else {
//					skillButtons [i].GetComponentInChildren<Text> ().text = "";
//				}
//				skillButtons [i].interactable = s.isAvalible && player.mana >= s.manaConsume; 
//			}

			}

		}

		public void OnItemButtonClick(int ButtonIndex){

			Item item = null;

			switch (ButtonIndex) {
			case 0:
				item = player.allItems.Find (delegate (Item obj) {
					return obj.itemNameInEnglish == "health";
				});
				break;
			case 1:
				item = player.allItems.Find (delegate (Item obj) {
					return obj.itemNameInEnglish == "mana";
				});
				break;
			case 2:
				item = player.allItems.Find (delegate (Item obj) {
					return obj.itemNameInEnglish == "antiDebuff";
				});
				break;
			default:
				break;

			}


			if (item == null) {
				return;
			}

			player.health += (item as Consumable).healthGain;
			player.mana += (item as Consumable).manaGain;

//			if (item.itemNameInEnglish == "antiDebuff") {
//				player.states.Clear ();
//			}

			item.itemCount--;

			if (item.itemCount <= 0) {
				player.allItems.Remove (item);
			}

			UpdateItemButtons ();
			UpdatePlayerStatusPlane ();

		}


		public void UpdateManaBarAnim(Agent ba){
			manaBar.maxValue = ba.maxMana;
			manaText.text = ba.mana + "/" + ba.maxMana;


			if (firstSetStrengthBar) {
				manaBar.value = ba.mana;
			} else {
				manaBar.DOValue (ba.mana, 0.2f);
			}

		}

		public void OnBagButtonClick(){

		}

		public void QuitDetailPlane(){

//			skillAndItemDetailPlane.gameObject.SetActive (false);

		}


//		public void ShowItemDetail(int index,Item item){
//
//			skillAndItemDetailPlane.SetParent (itemButtons [index].transform,false);
//
//			skillAndItemDetailPlane.FindChild ("Name").GetComponent<Text> ().text = item.itemName;
//
//			skillAndItemDetailPlane.FindChild ("Description").GetComponent<Text> ().text = item.itemDescription;
//
//			skillAndItemDetailPlane.FindChild ("Detail").GetComponent<Text> ().text = item.GetItemPropertiesString ();
//
//			skillAndItemDetailPlane.gameObject.SetActive (true);
//		}


//		public void SetUpBattleGainsHUD(List<Item> battleGains){
//
//			for (int i = 0; i < battleGains.Count; i++) {
//
//				Item item = battleGains [i];
//
//				Transform gainItem = battleGainsPool.GetInstance<Transform> (gainItemModel, battleGainsContainer);
//
//				Image itemIcon = gainItem.FindChild ("ItemIcon").GetComponent<Image> ();
//
//				Text itemCount = gainItem.FindChild ("ItemCount").GetComponent<Text> ();
//
//				itemIcon.sprite = GameManager.Instance.allItemSprites.Find (delegate(Sprite obj) {
//					return obj.name == item.spriteName;
//				});
//
//				if (itemIcon.sprite != null) {
//					itemIcon.enabled = true;
//				}
//
//				itemCount.text = item.itemCount.ToString ();
//			}

//		}

//		public void QuitBattleGainsHUD (){
//
//			foreach (Transform trans in battleGainsContainer) {
//
//				Image itemIcon = trans.FindChild ("ItemIcon").GetComponent<Image> ();
//
//				Text itemCount = trans.FindChild ("ItemCount").GetComponent<Text> ();
//
//				itemIcon.sprite = null;
//				itemIcon.enabled = false;
//				itemCount.text = string.Empty;
//
//			}
//
//			battleGainsPool.AddChildInstancesToPool (battleGainsContainer);
//
//			battleGainsHUD.gameObject.SetActive (false);
//
//		}

//		public void OnQuitBattle(){
//
//			foreach (Button btn in skillButtons) {
//				btn.interactable = false;
//				btn.GetComponent<Image> ().enabled = false;
//				foreach (Text t in btn.GetComponentsInChildren<Text>()) {
//					t.text = string.Empty;
//				}
//			}
//
//			foreach (Button btn in new Button[]{healthBottleButton,manaBottleButton,antiDebuffButton}) {
//				btn.interactable = false;
//				btn.GetComponent<Image> ().enabled = false;
//				btn.GetComponentInChildren<Text> ().text = string.Empty;
//			}
//		}

//		private BattlePlayerController mBaPlayerController;

		private List<Sprite> skillIcons = new List<Sprite>();

		//	private List<Item> consumables = new List<Item> ();

		// 角色UIView
//		public BattlePlayerController baView{
//
//			get{
//				if (mBaPlayerController == null) {
//					mBaPlayerController = GetComponent<BattlePlayerController> ();
//				}
//				return mBaPlayerController;
//			}
//
//		}




		public void OnPlayerSelectSkill(int skillIndex){


			//			baView.SelectedSkillAnim (player.currentSkill == player.attackSkill,
			//				player.currentSkill == player.defenceSkill,
			//				skillIndex);

		}

		public void OnPlayerUseItem(int itemIndex){


			Item item = player.allEquipedItems[itemIndex + 3];

			if (item == null) {
				return;
			}

			item.itemCount--;


			if (item.itemCount <= 0) {
				player.allEquipedItems [itemIndex + 3] = null;
				player.allItems.Remove (item);
				UpdateItemButtons ();
			}



//			if (item.healthGain != 0 && item.manaGain != 0) {
//				player.health += item.healthGain;
//				player.mana += item.manaGain;
//				UpdateHealthBarAnim (player);
//				UpdateManaBarAnim (player);
//
//				string tintText = "<color=green>+" + item.healthGain.ToString() + "体力</color>" 
//					+ "\t\t\t\t\t" 
//					+ "<color=orange>+" + item.manaGain.ToString() + "魔法</color>";
//				PlayTintHUDAnim(tintText);
//
//			}else if (item.healthGain != 0) {
//				player.health += item.healthGain;
//				UpdateHealthBarAnim (player);
//				string tintText = "<color=green>+" + item.healthGain.ToString() + "体力</color>";
//				PlayTintHUDAnim(tintText);
//
//			}else if (item.manaGain != 0) {
//				player.mana += item.manaGain;
//				UpdateManaBarAnim (player);
//				string tintText = "<color=blue>+" + item.manaGain.ToString() + "魔法</color>";
//				PlayTintHUDAnim(tintText);
//			}

			UpdateItemButtons ();

		}


		public void OnSkillButtonUp(){

			QuitDetailPlane ();
		}

//		public void OnItemLongPress(int index){
//			Item i = player.allEquipedItems [3 + index];
//			ShowItemDetail (index, i);
//		}

		public void OnItemButtonUp(){

			QuitDetailPlane ();
		}


		public void PlayPlayerDieAnim(BattleAgentController baCtr,CallBack cb){
			
			baCtr.GetComponent<SpriteRenderer> ().DOFade (0, 0.5f).OnComplete(()=>{
				baCtr.gameObject.SetActive(false);

				if(cb != null){
					cb();
				}

			});
		}


		public void OnQuitFight(){

			skillButtonPool.AddChildInstancesToPool (skillsContainer);

		}
	}
}
