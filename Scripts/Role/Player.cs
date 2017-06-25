using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Player : BattleAgent {

	// 玩家角色单例
	public static Player mainPlayer;


	public Skill attackSkill;
	public Skill defenceSkill;

	public int playerLevel;

	// 角色管理的UI

	public Button attackButton;

	public Button defenceButton;

	public Button[] skillButtons;

	public Button[] itemButtons;

	private Sequence mSequence;

	public Sprite skillNormalBackImg; // 技能框默认背景图片
	public Sprite skillSelectedBackImg; // 选中技能的背景高亮图片
	public Sprite attackAndDefenceNormalBackImg;
	public Sprite attackAndDenfenceSelectedBackImg;


	private bool isAttackEnable = true;
	private bool isSkillEnable = true;
	private bool isItemEnable = true;
	private bool isDefenceEnable = true;

	Image selectedButtonBackImg;

	public override void Awake(){

		base.Awake ();

		if (mainPlayer == null) {
			mainPlayer = this;
		} else if (mainPlayer != this) {
			Destroy (gameObject);
		}

//		DontDestroyOnLoad (gameObject);

//		mainPlayer.originalMaxHealth = 100;
//		mainPlayer.originalMaxStrength = 10;
//		mainPlayer.originalHealth = 100;
//		mainPlayer.originalStrength = 10;
//		mainPlayer.originalAttack = 1;
//		mainPlayer.originalPower = 1;
//		mainPlayer.originalMagic = 1;
//		mainPlayer.originalCrit = 1;
//		mainPlayer.originalAmour = 1;
//		mainPlayer.originalMagicResist = 1;
//		mainPlayer.originalAgility = 1;

	}


	public void ValidActionForPlayer(){

		switch (validActionType) {

		case ValidActionType.All:
			break;
		case ValidActionType.PhysicalExcption:
			EnableOrDisableButtons(false, true, true, true);
			break;
		case ValidActionType.MagicException:
			EnableOrDisableButtons(true, false, true, true);
			break;
		case ValidActionType.None:
			EnableOrDisableButtons(false, false, false, false);
			break;
		case ValidActionType.PhysicalOnly:
			EnableOrDisableButtons(true, false, false, true);
			break;
		case ValidActionType.MagicOnly:
			EnableOrDisableButtons(false, true, false, true);
			break;
		default:
			break;
		}

		// 如果技能还在冷却中或者玩家气力值小于技能消耗的气力值，则相应按钮不可用
		for (int i = 0;i < skills.Count;i++) {

			Skill s = skills [i];
			// 如果是冷却中的技能
			if (s.isAvalible == false) {
				s.actionCount++;
				int actionBackCount = s.actionConsume - s.actionCount + 1;
				skillButtons [i].GetComponentInChildren<Text> ().text = actionBackCount == 0 ? "" : actionBackCount.ToString ();
				Debug.Log (s.skillName + "从使用开始经过了" + s.actionCount + "回合");
				if (s.actionCount > s.actionConsume) {
					s.isAvalible = true;
					s.actionCount = 0;
				} 
			}
			attackButton.interactable = isAttackEnable && strength >= attackSkill.strengthConsume;
			defenceButton.interactable = isDefenceEnable && strength >= defenceSkill.strengthConsume;;

			skillButtons [i].interactable = s.isAvalible && strength >= s.strengthConsume && isSkillEnable; 
		}
		foreach (Button btn in itemButtons) {
			btn.interactable = isItemEnable;
		}

	}

	//根据玩家状态判断按钮是否可以交互
	private void EnableOrDisableButtons(bool isAttackEnable,bool isSkillEnable,bool isItemEnable,bool isDefenceEnable){
		
		this.isAttackEnable = isAttackEnable;
		this.isSkillEnable = isSkillEnable;
		this.isItemEnable = isItemEnable;
		this.isDefenceEnable = isDefenceEnable;
	}

	public void SelectedSkillAnim(bool isAttack,bool isDefence,int skillId){

		if (selectedButtonBackImg != null) {
			KillSelectedSkillAnim ();
		}


		mSequence = null;

		if (isAttack) {
			selectedButtonBackImg = attackButton.transform.parent.GetComponent<Image> ();
		} else if (isDefence) {
			selectedButtonBackImg = defenceButton.transform.parent.GetComponent<Image> ();

		} else {
			Debug.Log (skillButtons [currentSkill.skillId]);
			selectedButtonBackImg = skillButtons [currentSkill.skillId].transform.parent.GetComponent<Image> ();
		}

		selectedButtonBackImg.sprite = (isAttack || isDefence) ? attackAndDenfenceSelectedBackImg : skillSelectedBackImg;

		mSequence = DOTween.Sequence ();

		mSequence.Append (selectedButtonBackImg.DOFade (0.5f, 3.0f));
		mSequence.Append(selectedButtonBackImg.DOFade(1.0f,3.0f));
		mSequence.SetLoops(int.MaxValue);
	}

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

}
