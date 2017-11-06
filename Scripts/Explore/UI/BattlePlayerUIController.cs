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





		public void SetUpExplorePlayerView(Player player,CallBack<int> skillSelectCallBack){

			this.player = player;
			this.skillSelectCallBack = skillSelectCallBack;

			Transform poolContainerOfExploreCanvas = TransformManager.FindOrCreateTransform (CommonData.poolContainerName + "/PoolContainerOfExploreCanvas");
			Transform modelContainerOfExploreScene = TransformManager.FindOrCreateTransform(CommonData.instanceContainerName + "/ModelContainerOfExploreScene");


			skillButtonPool = InstancePool.GetOrCreateInstancePool ("SkillButtonPool",poolContainerOfExploreCanvas.name);
			skillButtonModel = TransformManager.FindTransform ("SkillButtonModel");

			consumablesButtonPool = InstancePool.GetOrCreateInstancePool ("ConsumablesButtonPool", poolContainerOfExploreCanvas.name);
			consumablesButtonModel = TransformManager.FindTransform ("ConsumablesButtonModel");

			consumablesButtonModel.SetParent (modelContainerOfExploreScene);
			skillButtonModel.SetParent (modelContainerOfExploreScene);

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

			Item healthBottle = player.allConsumablesInBag.Find (delegate(Consumables obj) {
				return obj.itemNameInEnglish == "health";
			});

			Item manaBottle = player.allConsumablesInBag.Find (delegate(Consumables obj) {
				return obj.itemNameInEnglish == "mana";
			});

			Item antiDebuffBottle = player.allConsumablesInBag.Find (delegate(Consumables obj) {
				return obj.itemNameInEnglish == "antiDebuff";
			});

			SetUpItemButton (healthBottle, healthBottleButton);
			SetUpItemButton (manaBottle, manaBottleButton);
			SetUpItemButton (antiDebuffBottle, antiDebuffButton);


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
				bagCanvas.GetComponent<Canvas>().enabled =true;
				bagCanvas.gameObject.SetActive(true);
			}, false);

		}


		public void OnItemButtonClick(int buttonIndex){

			Consumables consumables = null;

			switch (buttonIndex) {
			case 0://生命药剂
				consumables = player.allConsumablesInBag.Find (delegate (Consumables obj) {
					return obj.itemId == 501;
				});
				break;
			case 1://魔法药剂
				consumables = player.allConsumablesInBag.Find (delegate (Consumables obj) {
					return obj.itemId == 502;
				});
				break;
			case 2://回程卷轴
				consumables = player.allConsumablesInBag.Find (delegate (Consumables obj) {
					return obj.itemId == 515;
				});
				break;
			}
				
			if (consumables == null) {
				return;
			}

			BattlePlayerController bpCtr = GameObject.Find ("BattlePlayer").GetComponent<BattlePlayerController> ();

			bpCtr.UseItem (consumables);

		}

		public void OnAllConsumablesButtonClick(){

			if (allConsumablesButton.transform.localRotation != Quaternion.identity) {

				allConsumablesButton.transform.localRotation = Quaternion.identity;

				QuitAllConsumablesPlane ();

				return;

			}

			allConsumablesButton.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, 180));

			for (int i = 0; i < Player.mainPlayer.allConsumablesInBag.Count; i++) {

				Consumables consumables = Player.mainPlayer.allConsumablesInBag [i];

				Button consumablesButton = consumablesButtonPool.GetInstance<Button> (consumablesButtonModel.gameObject, allConsumablesContainer);

				Image consumablesIcon = consumablesButton.GetComponent<Image> ();
				Text consumablesCount = consumablesButton.GetComponentInChildren<Text> ();

				Sprite s = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
					return obj.name == consumables.spriteName;
				});

				if (s != null) {
					consumablesIcon.sprite = s;
				}

				consumablesCount.text = consumables.itemCount.ToString ();

				consumablesButton.onClick.RemoveAllListeners ();

				consumablesButton.onClick.AddListener (delegate {

					BattlePlayerController bpCtr = GameObject.Find ("BattlePlayer").GetComponent<BattlePlayerController> ();

					bpCtr.UseItem (consumables);

				});

			}

			allConsumablesPlane.gameObject.SetActive (true);

		}

		public void QuitAllConsumablesPlane(){

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
//				itemIcon.sprite = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
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
