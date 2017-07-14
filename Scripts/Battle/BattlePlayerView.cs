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

	private Sequence mSequence;

	public Sprite skillNormalBackImg; // 技能框默认背景图片
	public Sprite skillSelectedBackImg; // 选中技能的背景高亮图片
	public Sprite attackAndDefenceNormalBackImg;
	public Sprite attackAndDenfenceSelectedBackImg;

	private Image selectedButtonBackImg;

	private List<Sprite> icons = new List<Sprite> ();


	public void SetUpUI(Player player,List<Sprite> sprites){
		
		if (icons.Count == 0) {
			foreach (Sprite s in sprites) {
				icons.Add (s);
			}
		}

		for (int i = 0; i < player.skillsEquiped.Count; i++) {

			Button skillButton = skillButtons [i];

			Image skillIcon = skillButton.GetComponent<Image> ();
			skillIcon.sprite = icons.Find (delegate(Sprite obj) {
				return obj.name == player.skillsEquiped[i].skillIconName;
			});
			skillIcon.enabled = true;
			skillButton.interactable = true;
			skillButton.transform.parent.FindChild("StrengthConsumeText").GetComponent<Text>().text = player.skillsEquiped [i].strengthConsume.ToString();
			skillButton.transform.GetComponentInChildren<Text> ().text = "";
		}

		List<Item> consumables = new List<Item> ();

		foreach (Item i in player.allEquipedItems) {
			if (i.itemType == ItemType.Consumables) {
				consumables.Add (i);
			}
		}

		for (int i = 0; i < consumables.Count; i++) {
			Debug.Log (i);
			Button itemButton = itemButtons [i];
			Item consumable = consumables [i];
			Image itemIcon = itemButton.GetComponent<Image> ();
			itemIcon.sprite = icons.Find (delegate(Sprite obj) {
				return obj.name == consumable.spriteName;
			});
			if (itemIcon.sprite != null) {
				itemIcon.enabled = true;
				itemButton.interactable = true;
				itemButton.transform.FindChild ("Text").GetComponent<Text> ().text = consumable.itemCount.ToString ();
			}
		}


	}

	// 更新战斗中玩家UI的状态
	public void UpdateUIStatus(Player player){
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

		foreach (Button btn in itemButtons) {
			btn.interactable = player.isItemEnable;
		}
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
