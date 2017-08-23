using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace WordJourney
{
	public class BattlePlayerUIController : BattleAgentUIController {


		private Player player;


		public Button[] skillButtons;

		public Button[] itemButtons;

		private Transform mSkillAndItemDetailPlane;
		public Transform skillAndItemDetailPlane{
			get{
				if (mSkillAndItemDetailPlane == null) {
					mSkillAndItemDetailPlane = GameObject.Find (CommonData.instanceContainerName + "/SkillAndItemDetailPlane").transform;
				}
				return mSkillAndItemDetailPlane;
			}

		}
			
		private Sequence mSequence;

		private List<Sprite> skillSprites;

		private List<Sprite> itemSprites;


//		private void Awake(){
//
//			rewardButtonPool = InstancePool.GetOrCreateInstancePool ("RewardButtonPool");
//
//
//		}


		public void SetUpUI(Player player,List<Sprite> skillSprites,List<Sprite> itemSprites){

			this.player = player;
			this.skillSprites = skillSprites;
			this.itemSprites = itemSprites;

			SetUpPlayerStatusUI ();
			SetUpItemButtons ();

//			SetUpSkillButtons ();
//
//			SetUpItemButtons ();

		}


		private void SetUpPlayerStatusUI(){

			healthBar.maxValue = player.maxHealth;
			healthBar.value = player.health;
			manaBar.maxValue = player.maxMana;
			manaBar.value = player.mana;

			healthText.text = string.Format ("{0}/{1}", player.health, player.maxHealth);
			manaText.text = string.Format ("{0}/{1}", player.mana, player.maxMana);


		}

		private void SetUpSkillButtons(){

			for (int i = 0; i < player.skillsEquiped.Count; i++) {

				Button skillButton = skillButtons [i];
				Skill skill = player.skillsEquiped [i];

				Image skillIcon = skillButton.GetComponent<Image> ();

				Text strengthConsumeText = skillButton.transform.parent.FindChild ("StrengthConsumeText").GetComponent<Text>();

				Text skillNameText = skillButton.transform.FindChild ("SkillName").GetComponent<Text> ();

				skillIcon.sprite = skillSprites.Find (delegate(Sprite obj) {
					return obj.name == skill.skillIconName;
				});
				skillIcon.enabled = true;
				skillButton.interactable = true;
				strengthConsumeText.text = skill.manaConsume.ToString();
				skillNameText.text = skill.skillName;
				skillButton.transform.GetComponentInChildren<Text> ().text = "";
			}

			for (int i = player.skillsEquiped.Count; i < skillButtons.Length; i++) {
				skillButtons [i].interactable = false;
			}
		}

		public void SetUpItemButtons(){

			for (int i = 3; i < player.allEquipedItems.Count; i++) {

				Item consumable = player.allEquipedItems [i];

				Button itemButton = itemButtons [i - 3];

				Image itemIcon = itemButton.GetComponent<Image> ();

				Text itemCount = itemButton.transform.FindChild ("Text").GetComponent<Text> ();

				if (consumable == null) {

					itemButton.interactable = false;
					itemIcon.enabled = false;

					itemIcon.sprite = null;
					itemCount.text = string.Empty;

					continue;
				}

				itemIcon.sprite = itemSprites.Find (delegate(Sprite obj) {
					return obj.name == consumable.spriteName;
				});
				if (itemIcon.sprite != null) {
					itemIcon.enabled = true;
					itemButton.interactable = true;
					itemCount.text = consumable.itemCount.ToString ();
				}

				itemButton.interactable = (consumable.itemCount > 0);
			}

		}


		// 更新战斗中玩家UI的状态
		public void UpdateSkillButtonsStatus(Player player){

			for (int i = 0;i < player.skillsEquiped.Count;i++) {

				Skill s = player.skillsEquiped [i];
				// 如果是冷却中的技能
				if (s.isAvalible == false) {
					int actionBackCount = s.actionConsume - s.actionCount + 1;
					skillButtons [i].GetComponentInChildren<Text> ().text = actionBackCount.ToString ();
				} else {
					skillButtons [i].GetComponentInChildren<Text> ().text = "";
				}
				skillButtons [i].interactable = s.isAvalible && player.mana >= s.manaConsume; 
			}

			//			attackButton.interactable = player.isAttackEnable && player.strength >= player.attackSkill.manaConsume;
			//			defenceButton.interactable = player.isDefenceEnable && player.strength >= player.defenceSkill.manaConsume;


		}


		public void QuitDetailPlane(){

			skillAndItemDetailPlane.gameObject.SetActive (false);

		}


		public void ShowItemDetail(int index,Item item){

			skillAndItemDetailPlane.SetParent (itemButtons [index].transform,false);

			skillAndItemDetailPlane.FindChild ("Name").GetComponent<Text> ().text = item.itemName;

			skillAndItemDetailPlane.FindChild ("Description").GetComponent<Text> ().text = item.itemDescription;

			skillAndItemDetailPlane.FindChild ("Detail").GetComponent<Text> ().text = item.GetItemPropertiesString ();

			skillAndItemDetailPlane.gameObject.SetActive (true);
		}


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

		public void OnQuitBattle(){

			foreach (Button btn in skillButtons) {
				btn.interactable = false;
				btn.GetComponent<Image> ().enabled = false;
				foreach (Text t in btn.GetComponentsInChildren<Text>()) {
					t.text = string.Empty;
				}
			}

			foreach (Button btn in itemButtons) {
				btn.interactable = false;
				btn.GetComponent<Image> ().enabled = false;
				btn.GetComponentInChildren<Text> ().text = string.Empty;
			}
		}

		private BattlePlayerController mBaPlayerController;

		private List<Sprite> skillIcons = new List<Sprite>();

		//	private List<Item> consumables = new List<Item> ();

		// 角色UIView
		public BattlePlayerController baView{

			get{
				if (mBaPlayerController == null) {
					mBaPlayerController = GetComponent<BattlePlayerController> ();
				}
				return mBaPlayerController;
			}

		}


		public void SetUpBattlePlayerView(){

			//		for(int i = 3;i<player.allEquipedItems.Count;i++){
			//			Item consumable = player.allEquipedItems [i];
			//			consumables.Add (consumable);
			//		}

			List<Sprite> allItemSprites = GameManager.Instance.allItemSprites;

			if (skillIcons.Count != 0) {
				SetUpUI (player,skillIcons,allItemSprites);
				return;
			}

			ResourceManager.Instance.LoadSpritesAssetWithFileName("skills/skills", () => {

				foreach(Sprite s in ResourceManager.Instance.sprites){
					skillIcons.Add(s);
				}
				SetUpUI (player,skillIcons,allItemSprites);
			},true);

		}

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
				SetUpItemButtons ();
			}



			if (item.healthGain != 0 && item.manaGain != 0) {
				player.health += item.healthGain;
				player.mana += item.manaGain;
				UpdateHealthBarAnim (player);
				UpdateManaBarAnim (player);

				string hurtText = "<color=green>+" + item.healthGain.ToString() + "体力</color>" 
					+ "\t\t\t\t\t" 
					+ "<color=orange>+" + item.manaGain.ToString() + "魔法</color>";
				PlayHurtHUDAnim(hurtText);

			}else if (item.healthGain != 0) {
				player.health += item.healthGain;
				UpdateHealthBarAnim (player);
				string hurtText = "<color=green>+" + item.healthGain.ToString() + "体力</color>";
				PlayHurtHUDAnim(hurtText);

			}else if (item.manaGain != 0) {
				player.mana += item.manaGain;
				UpdateManaBarAnim (player);
				string hurtText = "<color=orange>+" + item.manaGain.ToString() + "魔法</color>";
				PlayHurtHUDAnim(hurtText);
			}

			SetUpItemButtons ();

		}


		public void OnSkillButtonUp(){

			QuitDetailPlane ();
		}

		public void OnItemLongPress(int index){
			Item i = player.allEquipedItems [3 + index];
			ShowItemDetail (index, i);
		}

		public void OnItemButtonUp(){

			QuitDetailPlane ();
		}

	}
}
