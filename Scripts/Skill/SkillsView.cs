using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace WordJourney
{
	public class SkillsView : MonoBehaviour{

		public Text skillPointsLeft;

		public Button[] skillTreeButtons;

		public Image skillBigIcon;

		public Text skillLevelOnBigIcon;

		public Text skillName;

		public Text skillDesc;

		public Text skillConsume;

		public Text skillCoolen;

		public Text skillUnlock;

		public Button upgradeSkillButton;

		public Button quitSkillsPlaneButton;

		public Transform skillsViewContainer;
		public Transform skillPlane;

		private Button currentSelectSkillButton;

		/// <summary>
		/// 初始化技能页面
		/// </summary>
		/// <param name="skills">Skills.</param> 从assetBundle加载的所有技能
		/// <param name="sprites">Sprites.</param> 从assetBundle加载的所有技能图片
		public void SetUpSkillsView(List<Skill> skills,List<Sprite> sprites){

			skillPointsLeft.text = Player.mainPlayer.skillPointsLeft.ToString ();
			
			for (int i = 0; i < skillTreeButtons.Length; i++) {
				
				Skill skill = skills [i];
				Button skillButton = skillTreeButtons [i];
				Sprite skillSprite = sprites.Find (delegate(Sprite obj) {
					return obj.name == skill.skillIconName;
				});

				Skill learnedSkill = Player.mainPlayer.GetPlayerLearnedSkill (skill.skillId);

				if (learnedSkill == null) {
					learnedSkill = skill;
				}

				SetUpSkillButton (skillButton, skillSprite, learnedSkill);

				if (i == 0) {
					OnSkillTreeButtonClick (skillButton, skillSprite, learnedSkill);
				}
			}
//			GetComponent<Canvas>().enabled = true;

		}

		private void SetUpSkillButton(Button skillButton, Sprite skillSprite, Skill skill){

			skillButton.name = skill.skillName;

			Image skillIcon = skillButton.transform.Find ("SkillIcon").GetComponent<Image> ();
			Text skillLevelText = skillButton.transform.Find ("SkillLevel").GetComponent<Text> ();
			Image skillLockedMask = skillButton.transform.Find ("SkillLockedMask").GetComponent<Image> ();

			if (skillSprite != null) {
				skillIcon.sprite = skillSprite;
				skillIcon.enabled = true;
			}

			if (skill.skillLevel > 0) {
				skillLevelText.text = skill.skillLevel.ToString ();
			} else {
				skillLevelText.text = string.Empty;
			}
				
//			skillMask.enabled = Player.mainPlayer.agentLevel < skill.unlockAgentLevel;
			skillLockedMask.enabled = !skill.unlocked;

			skillButton.onClick.AddListener (delegate() {
				OnSkillTreeButtonClick(skillButton,skillSprite,skill);
			});
				
		}

		// 技能树上技能的点击响应
		private void OnSkillTreeButtonClick(Button skillButton, Sprite skillSprite, Skill skill){

			currentSelectSkillButton = skillButton;

			for (int i = 0; i < skillTreeButtons.Length; i++) {
				skillTreeButtons [i].transform.Find ("SelectedIcon").GetComponent<Image> ().enabled = false;
			}

			skillButton.transform.Find ("SelectedIcon").GetComponent<Image> ().enabled = true;


			if (skillSprite != null) {
				skillBigIcon.sprite = skillSprite;

				skillBigIcon.enabled = true;
			}

			skillLevelOnBigIcon.text = "Lv." + skill.skillLevel.ToString ();

			skillName.text = skill.skillName;

			skillDesc.text = skill.skillDescription;

			if (skill.skillType == SkillType.TalentPassive || skill.skillType == SkillType.TriggeredPassive) {
				
				skillConsume.text = "<color=orange>被动</color>";

				skillCoolen.text = string.Empty;

			} else {

				int manaConsume = (skill as ActiveSkill).manaConsume;

				skillConsume.text = "魔法消耗: " + manaConsume.ToString ();

//				skillCoolen.text = "冷却时间: " + skill.coolenInterval.ToString () + "s";

			}

//			bool unlock = Player.mainPlayer.agentLevel >= skill.unlockAgentLevel;
//
//			if (!unlock) {
//				skillUnlock.text = "<color=red>解锁：人物等级≥" + skill.unlockAgentLevel + "</color>";
//			} else {
//				skillUnlock.text = "<color=green>已解锁</color>";
//			}

			upgradeSkillButton.interactable = skill.unlocked && Player.mainPlayer.skillPointsLeft > 0;

			GetComponent<SkillsViewController> ().GetCurrentSelectInfo (skill);

		}

		// 升级按钮点击响应
		public void OnUpgradeSkillButtonClicked(Skill skill){

			skillPointsLeft.text = Player.mainPlayer.skillPointsLeft.ToString ();

			Text skillLevelText = currentSelectSkillButton.GetComponentInChildren<Text> ();

			if (skill.skillLevel > 0) {
				skillLevelText.text = skill.skillLevel.ToString ();
				skillLevelOnBigIcon.text = "Lv." + skill.skillLevel.ToString (); 
			} else {
				skillLevelText.text = string.Empty;
				skillLevelOnBigIcon.text = string.Empty;
			}



		}


		// 退出按钮点击响应
		public void QuitSkillsPlane(){

//			skillsViewContainer.GetComponent<Image> ().color = new Color (0, 0, 0, 0);

//			float offsetY = GetComponent<CanvasScaler> ().referenceResolution.y;
//
//			Vector3 originalPosition = skillPlane.localPosition;
//
//			skillPlane.transform.DOLocalMoveY (-offsetY, 0.5f).OnComplete (() => {
////				GetComponent<Canvas>().enabled = false;
//				skillPlane.localPosition = originalPosition;
//			});

		}
	}
}
