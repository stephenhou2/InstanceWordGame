using UnityEngine;
using UnityEngine.UI;
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

	Image selectedButtonBackImg;

	// 更新战斗中玩家UI的状态
	public void UpdateUIStatus(Player player){
		for (int i = 0;i < player.skills.Count;i++) {

			Skill s = player.skills [i];
			// 如果是冷却中的技能
			if (s.isAvalible == false) {
				int actionBackCount = s.actionConsume - s.actionCount + 1;
				skillButtons [i].GetComponentInChildren<Text> ().text = actionBackCount == 0 ? "" : actionBackCount.ToString ();

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
			selectedButtonBackImg = skillButtons [skillId].transform.parent.GetComponent<Image> ();
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

	public void ResetPlayerUI(){
		Player p = GetComponent<Player> ();
		for(int i = 0;i < p.skills.Count;i++){
			Button btn = skillButtons [i];
			btn.interactable = true;
			Debug.Log (btn.transform.parent.FindChild("StrengthConsumeText"));
			btn.transform.parent.FindChild("StrengthConsumeText").GetComponent<Text>().text 
			= p.skills [i].strengthConsume.ToString();
		}
	}

}
