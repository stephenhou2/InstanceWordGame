using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class BattlePlayerView : BattleAgentView {

	// 战斗中的玩家UI
	public Button attackButton;

	public Button defenceButton;

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

	public Sprite skillNormalBackImg; // 技能框默认背景图片
	public Sprite skillSelectedBackImg; // 选中技能的背景高亮图片
	public Sprite attackAndDefenceNormalBackImg;
	public Sprite attackAndDenfenceSelectedBackImg;

	private Image selectedButtonBackImg;

	private List<Sprite> skillSprites;

	private List<Sprite> itemSprites;


	public Transform battleGainsHUD;

	public Transform battleGainsContainer;

	public GameObject gainItemModel;

	private InstancePool battleGainsPool;


	public void SetUpUI(Player player,List<Sprite> skillSprites,List<Sprite> itemSprites){

		battleGainsPool = InstancePool.GetOrCreateInstancePool ("BattleGainsPool");
		
		this.skillSprites = skillSprites;
		this.itemSprites = itemSprites;


		SetUpSkillButtonsStatus (player);
		SetUpItemButtonsStatus (player);

	}

	private void SetUpSkillButtonsStatus(Player player){
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
			strengthConsumeText.text = skill.strengthConsume.ToString();
			skillNameText.text = skill.skillName;
			skillButton.transform.GetComponentInChildren<Text> ().text = "";
		}

		for (int i = player.skillsEquiped.Count; i < skillButtons.Length; i++) {
			skillButtons [i].interactable = false;
		}
	}

	public void SetUpItemButtonsStatus(Player player){

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

			itemButton.interactable = (consumable.itemCount > 0) && player.isItemEnable;
		}

//		for (int i = player.allEquipedItems.Count; i < itemButtons.Length; i++) {
//			itemButtons [i].interactable = false;
//		}

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
			skillButtons [i].interactable = s.isAvalible && player.strength >= s.strengthConsume && player.isSkillEnable; 
		}

		attackButton.interactable = player.isAttackEnable && player.strength >= player.attackSkill.strengthConsume;
		defenceButton.interactable = player.isDefenceEnable && player.strength >= player.defenceSkill.strengthConsume;


	}

	// 选择技能后的动画
	public void SelectedSkillAnim(bool isAttack,bool isDefence,int buttonIndex){

		if (selectedButtonBackImg != null) {
			KillSelectedSkillAnim ();
		}


		mSequence = null;

		if (isAttack) {
			selectedButtonBackImg = attackButton.transform.parent.GetComponent<Image> ();
		} else if (isDefence) {
			selectedButtonBackImg = defenceButton.transform.parent.GetComponent<Image> ();

		} else {
			selectedButtonBackImg = skillButtons [buttonIndex].transform.parent.GetComponent<Image> ();
		}

		selectedButtonBackImg.sprite = (isAttack || isDefence) ? attackAndDenfenceSelectedBackImg : skillSelectedBackImg;

		mSequence = DOTween.Sequence ();

		mSequence.Append (selectedButtonBackImg.DOFade (0.5f, 3.0f));
		mSequence.Append(selectedButtonBackImg.DOFade(1.0f,3.0f));
		mSequence.SetLoops(int.MaxValue);
	}

	// 结束技能选框的动画
	public void KillSelectedSkillAnim(){

		if (selectedButtonBackImg.sprite == attackAndDenfenceSelectedBackImg) {
			selectedButtonBackImg.color = Color.white;
			selectedButtonBackImg.sprite = attackAndDefenceNormalBackImg;
		} else {
			selectedButtonBackImg.color = Color.white;
			selectedButtonBackImg.sprite = skillNormalBackImg;
		}

		mSequence.Kill (false);
		selectedButtonBackImg = null;

	}

	public void ShowSkillDetail(int index,Skill skill){

		skillAndItemDetailPlane.SetParent (skillButtons [index].transform,false);

		skillAndItemDetailPlane.FindChild ("Name").GetComponent<Text> ().text = skill.skillName;

		skillAndItemDetailPlane.FindChild ("Description").GetComponent<Text> ().text = skill.skillDescription;

		skillAndItemDetailPlane.FindChild("Detail").GetComponent<Text> ().text = string.Format ("气力消耗:{0}\n冷却回合{1}", skill.strengthConsume, skill.actionConsume);

		skillAndItemDetailPlane.gameObject.SetActive (true);
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
		

	public void SetUpBattleGainsHUD(List<Item> battleGains){

		for (int i = 0; i < battleGains.Count; i++) {

			Item item = battleGains [i];

			Transform gainItem = battleGainsPool.GetInstance<Transform> (gainItemModel, battleGainsContainer);

			Image itemIcon = gainItem.FindChild ("ItemIcon").GetComponent<Image> ();

			Text itemCount = gainItem.FindChild ("ItemCount").GetComponent<Text> ();

			itemIcon.sprite = GameManager.Instance.allItemSprites.Find (delegate(Sprite obj) {
				return obj.name == item.spriteName;
			});

			if (itemIcon.sprite != null) {
				itemIcon.enabled = true;
			}

			itemCount.text = item.itemCount.ToString ();
		}

	}

	public void QuitBattleGainsHUD (){

		foreach (Transform trans in battleGainsContainer) {

			Image itemIcon = trans.FindChild ("ItemIcon").GetComponent<Image> ();

			Text itemCount = trans.FindChild ("ItemCount").GetComponent<Text> ();

			itemIcon.sprite = null;
			itemIcon.enabled = false;
			itemCount.text = string.Empty;

		}

		battleGainsPool.AddChildInstancesToPool (battleGainsContainer);

		battleGainsHUD.gameObject.SetActive (false);

	}

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

}
