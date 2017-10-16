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

//		private List<Sprite> skillSprites;
//
//		private List<Sprite> itemSprites;

		private List<Button> skillButtons = new List<Button>();

		private InstancePool skillButtonPool;

		private CallBack<int> skillSelectCallBack;


		protected override void Awake(){

			player = Player.mainPlayer;

			skillButtonPool = InstancePool.GetOrCreateInstancePool ("SkillButtonPool");

			base.Awake ();

		}


		public void SetUpExplorePlayerView(Player player,List<Sprite> skillSprites,List<Sprite> itemSprites,CallBack<int> skillSelectCallBack){

			this.player = player;
			this.skillSelectCallBack = skillSelectCallBack;

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

				Button skillButton = skillButtonPool.GetInstance<Button> (skillButtonModel, skillsContainer);

				Image skillIcon = skillButton.transform.Find ("SkillIcon").GetComponent<Image> ();
				Text skillName = skillButton.transform.Find ("SkillName").GetComponent<Text> ();
				Text manaConsume = skillButton.transform.Find ("ManaConsume").GetComponent<Text> ();


				Sprite sprite = GameManager.Instance.dataCenter.allSkillSprites.Find(delegate (Sprite s){
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
					SkillButtonCoolen(skillButton,skill);
				});

				skillButtons.Add (skillButton);

			}

			skillsContainer.gameObject.SetActive (true);

		}

		/// <summary>
		/// 技能按钮的冷却
		/// </summary>
		private void SkillButtonCoolen(Button skillButton,Skill skill){
			
			skillButton.interactable = false;

			Image coolenMask = skillButton.transform.Find ("CoolenMask").GetComponent<Image> ();

			coolenMask.enabled = true;

			coolenMask.fillAmount = 1;

			coolenMask.DOFillAmount (0, skill.coolenInterval).OnComplete(()=>{
				coolenMask.enabled = false;
				UpdateSkillButtonsStatus();
			});
				
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


		public void OnBagButtonClick(){

			BattlePlayerController bpCtr = GameObject.Find ("BattlePlayer").GetComponent<BattlePlayerController> ();

			bpCtr.OnPlayerClickBag ();

		}


		public void OnItemButtonClick(int buttonIndex){

			Item item = null;

			switch (buttonIndex) {
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

			BattlePlayerController bpCtr = GameObject.Find ("BattlePlayer").GetComponent<BattlePlayerController> ();

			bpCtr.OnPlayerUseItem (item);

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
//				itemIcon.sprite = GameManager.Instance.dataCenter.allItemSprites.Find (delegate(Sprite obj) {
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

//		private List<Sprite> skillIcons = new List<Sprite>();

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




//		public void OnSkillButtonUp(){
//
//			QuitDetailPlane ();
//		}
//
//		public void OnItemButtonUp(){
//
//			QuitDetailPlane ();
//		}


//		public void PlayPlayerDieAnim(BattleAgentController baCtr,CallBack cb){
//			
//			baCtr.GetComponent<SpriteRenderer> ().DOFade (0, 0.5f).OnComplete(()=>{
//				baCtr.gameObject.SetActive(false);
//
//				if(cb != null){
//					cb();
//				}
//
//			});
//		}


		public override void QuitFight(){

			skillButtonPool.AddChildInstancesToPool (skillsContainer);

			skillsContainer.gameObject.SetActive (false);

		}
	}
}
