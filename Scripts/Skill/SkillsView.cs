using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SkillsView : MonoBehaviour{

	public Text skillPointsTotal;

	public Text skillPointsLeft;

	public Button[] equipedSkillButtons;

	public Button[] skillTypeButtons;

	public Button[] skillTreeButtons;

	public Image skillBigIcon;

	public Text skillLevelOnBigIcon;

	public Text skillName;

	public Text skillDesc;

	public Text skillCosume;

	public Text skillCoolen;

	public Text skillUnlock;

	public Button upgradeSkillButton;
	public Button equipSkillButton;

	public Button quitSkillsPlaneButton;

	private Sprite typeBtnNormalSprite;
	private Sprite typeBtnSelectedSprite;

	public Transform skillsViewContainer;
	public Transform skillPlane;

	/// <summary>
	/// 提示弹窗
	/// </summary>
	public GameObject tintHUD;

	/// <summary>
	/// 所有已装备技能弹窗
	/// </summary>
	public GameObject equipedSkillsHUD;
	public Button[] equipedSkillButtonsOfHUD;

	// 技能详细信息弹窗和上面的组件
	public GameObject skillDetailHUD;
	public Text skillNameOnHUD;
	public Text skillDescOnHUD;
	public Text skillCosumeOnHUD;
	public Text skillCoolenOnHUD;


	/// <summary>
	/// 初始化技能页面
	/// </summary>
	/// <param name="skills">Skills.</param> 从assetBundle加载的所有技能
	/// <param name="sprites">Sprites.</param> 从assetBundle加载的所有技能图片
	public void SetUpSkillsView(List<Sprite> sprites){
		
		Player player = Player.mainPlayer;

		skillPointsTotal.text = player.agentLevel.ToString();

		skillPointsLeft.text = player.skillPointsLeft.ToString();

		typeBtnNormalSprite = GameManager.Instance.allUIIcons.Find (delegate(Sprite obj) {
			return obj.name == "typeButtonNormal";
		});

		typeBtnSelectedSprite = GameManager.Instance.allUIIcons.Find (delegate(Sprite obj) {
			return obj.name == "typeButtonSelected";
		});

		for (int i = 0; i < player.skillsEquiped.Count; i++) {
			
			Image skillIcon = equipedSkillButtons [i].transform.FindChild ("SkillIcon").GetComponent<Image> ();

			skillIcon.sprite = sprites.Find (delegate (Sprite obj) {
				return obj.name == player.skillsEquiped [i].skillIconName;
			});
			skillIcon.enabled = true;
		}

	}

	// 技能类型按钮点击响应
	public void OnSkillTypeButtonClick(List<Skill> skillsOfCurrentType,List<Sprite> spritesOfCurrentType,int typeIndex){

		skillPointsTotal.text = Player.mainPlayer.agentLevel.ToString ();
		skillPointsLeft.text = Player.mainPlayer.skillPointsLeft.ToString ();

		for (int i = 0; i < skillTypeButtons.Length; i++) {
			Button skillTypeBtn = skillTypeButtons [i];
			if (i == typeIndex) {
				skillTypeBtn.GetComponent<Image> ().sprite = typeBtnSelectedSprite;
			} else {
				skillTypeBtn.GetComponent<Image> ().sprite = typeBtnNormalSprite;
			}
		}


		for(int i = 0;i < skillsOfCurrentType.Count;i++){
			
			Skill s = skillsOfCurrentType[i];

			Button skillButton = skillTreeButtons[i];

			Image skillIcon = skillButton.transform.FindChild("SkillIcon").GetComponent<Image>();

			skillIcon.sprite = spritesOfCurrentType[i];

			skillIcon.enabled = true;

			Text skillLevel = skillButton.transform.FindChild("SkillLevel").GetComponent<Text>();

			Skill associatedUnlockSkill = Player.mainPlayer.GetPlayerLearnedSkill (s.associatedSkillName);

			Skill playerSkill = Player.mainPlayer.GetPlayerLearnedSkill (s.skillName);


//			if (playerSkill == null && associatedUnlockSkill == null) {
//
//			}
//
//			if (associatedUnlockSkill != null && associatedUnlockSkill.skillLevel >= playerSkill.associatedSkillUnlockLevel) {
//
//			}
//
//			if (playerSkill != null) {
//				skillLevel.text = playerSkill.skillLevel.ToString ();
//				Image mask = skillButton.transform.FindChild("SkillMask").GetComponent<Image>();
//				if(associatedUnlockSkill == null){
//					playerSkill.unlocked = false;
//					mask.enabled = true;
//				}else{
//					mask.enabled = associatedUnlockSkill.skillLevel < s.associatedSkillUnlockLevel;
//					playerSkill.unlocked = !mask.enabled;
//				}
//			}
			Image mask = skillButton.transform.FindChild("SkillMask").GetComponent<Image>();

				
			if (playerSkill != null) {
				mask.enabled = false;
				skillLevel.text = playerSkill.skillLevel.ToString ();
			} else {
				if(associatedUnlockSkill == null){
					mask.enabled = true;
				}else{
					mask.enabled = associatedUnlockSkill.skillLevel < s.associatedSkillUnlockLevel;
				}
			}
			if (i == 0) {
				mask.enabled = false;
			}
		}

	}

	// 技能树上技能的点击响应
	public void OnSkillTreeButtonClick(Skill skill,Sprite sprite,int buttonIndex){
		

		for (int i = 0; i < skillTreeButtons.Length; i++) {
			skillTreeButtons [i].transform.FindChild ("SelectedIcon").GetComponent<Image> ().enabled = 
				i == buttonIndex;
		}
		skillBigIcon.sprite = sprite;
		skillBigIcon.enabled = true;
		skillLevelOnBigIcon.text = "Lv." + skill.skillLevel.ToString ();
		skillName.text = skill.skillName;
		skillDesc.text = skill.skillDescription;
		skillCosume.text = "气力消耗： " + skill.strengthConsume.ToString () + "点";
		skillCoolen.text = "冷却回合： " + skill.actionConsume.ToString () + "回合";

		Image mask = skillTreeButtons [buttonIndex].transform.FindChild ("SkillMask").GetComponent<Image> ();

		if (mask.enabled == true) {
			skillUnlock.text = "<color=red>解锁：" + skill.associatedSkillName + "等级>=" + skill.associatedSkillUnlockLevel + "</color>";
//			upgradeSkillButton.interactable = false;
		} else {
			skillUnlock.text = "已解锁";
//			upgradeSkillButton.interactable = true;
//			skillUnlock.text = "<color=white>解锁：" + s.associatedSkillName + "等级>=" + s.associatedSkillUnlockLevel + "</color>";
		}

//		Skill playerSkill = Player.mainPlayer.allLearnedSkills.Find (delegate(Skill obj) {
//			return obj.skillId == s.skillId;
//		});
//
//		equipSkillButton.interactable = playerSkill != null;

	}

	// 升级按钮点击响应
	public void OnUpgradeSkillButtonClicked(int currentSelectSkillIndex,int selectSkillLevel){

		skillPointsLeft.text = Player.mainPlayer.skillPointsLeft.ToString ();

		skillTreeButtons [currentSelectSkillIndex].GetComponentInChildren<Text> ().text = selectSkillLevel.ToString ();

		skillLevelOnBigIcon.GetComponentInChildren<Text> ().text = "Lv." + selectSkillLevel.ToString ();

//
//		if (Player.mainPlayer.skillPointsLeft <= 0) {
//			Debug.Log ("剩余技能点不足，请先升级");
//			return;
//		}
//
//		Skill skillToUpgrade = Player.mainPlayer.GetPlayerLearnedSkill (skillsOfCurrentType [currentSelectSkillIndex].skillName);
//		Skill skillAssociated = Player.mainPlayer.GetPlayerLearnedSkill(skillsOfCurrentType [currentSelectSkillIndex].associatedSkillName);
//
//		// 技能没有学过
//		if (skillToUpgrade == null) {
//			if (skillAssociated == null ||
//			    skillAssociated.skillLevel >= skillsOfCurrentType [currentSelectSkillIndex].associatedSkillUnlockLevel) {
//				skillToUpgrade = Instantiate (skillsOfCurrentType [currentSelectSkillIndex]);
//				skillToUpgrade.name = skillsOfCurrentType [currentSelectSkillIndex].skillName;
//				skillToUpgrade.transform.SetParent (Player.mainPlayer.transform.FindChild ("Skills").transform);
//				skillToUpgrade.unlocked = true;
//				skillToUpgrade.skillLevel++;
//				Player.mainPlayer.skillPointsLeft--;
//				if (skillToUpgrade.skillLevel == 1) {
//					Player.mainPlayer.allLearnedSkills.Add (skillToUpgrade);
//				}
//				OnSkillTypeButtonClick (currentSelectSkillTypeIndex);
//				skillPointsLeft.text = Player.mainPlayer.skillPointsLeft.ToString ();
//				skillTreeButtons [currentSelectSkillIndex].GetComponentInChildren<Text> ().text = skillToUpgrade.skillLevel.ToString ();
//				skillLevelOnBigIcon.GetComponentInChildren<Text> ().text = "Lv." + skillToUpgrade.skillLevel.ToString ();
//			} 
//			// 技能学过
//			else{
//				tintHUD.SetActive (true);
//				tintHUD.GetComponentInChildren<Text>().text = skillsOfCurrentType [currentSelectSkillIndex].skillName + 
//					"到达" + 
//					skillsOfCurrentType [currentSelectSkillIndex].associatedSkillUnlockLevel.ToString() + 
//					"级后解锁";
//			}
//		} else {
//			if (skillAssociated != null &&
//				skillAssociated.skillLevel >= skillsOfCurrentType [currentSelectSkillIndex].associatedSkillUnlockLevel) {
//				skillToUpgrade.unlocked = true;
//				skillToUpgrade.skillLevel++;
//				Player.mainPlayer.skillPointsLeft--;
//
//				skillPointsLeft.text = Player.mainPlayer.skillPointsLeft.ToString ();
//				skillTreeButtons [currentSelectSkillIndex].GetComponentInChildren<Text> ().text = skillToUpgrade.skillLevel.ToString ();
//				skillLevelOnBigIcon.GetComponentInChildren<Text> ().text = "Lv." + skillToUpgrade.skillLevel.ToString ();
//				OnSkillTypeButtonClick (currentSelectSkillTypeIndex);
//			} else{
//				tintHUD.SetActive (true);
//				tintHUD.GetComponentInChildren<Text>().text = skillsOfCurrentType [currentSelectSkillIndex].skillName + "到达" + skillToUpgrade.associatedSkillUnlockLevel.ToString() + "级后解锁";
//			}
//
//		}
	}

	// 装备按钮点击响应
	public void OnEquipButtonClick(Skill playerSkill,List<Sprite> spritesOfCurrentType,int currentSelectSkillIndex){

		if (playerSkill != null) {

			for (int i = 0; i < equipedSkillButtons.Length; i++) {
				
				Button skillButton = equipedSkillButtons [i];

				Image skillIcon = skillButton.transform.FindChild ("SkillIcon").GetComponent<Image> ();

				if (skillIcon.enabled == true && skillIcon.sprite.name == spritesOfCurrentType [currentSelectSkillIndex].name) {
					return;
				}

				if (skillIcon.enabled == false) {
					skillIcon.sprite = spritesOfCurrentType [currentSelectSkillIndex];
					skillIcon.enabled = true;
					Player.mainPlayer.skillsEquiped.Insert (i, playerSkill);
					return;
				}
			} 
			equipedSkillsHUD.SetActive (true);

			for (int i = 0; i < equipedSkillButtonsOfHUD.Length; i++) {
				
				Image equipedSkillIconOfHUD = equipedSkillButtonsOfHUD [i].transform.FindChild("SkillIcon").GetComponent<Image> ();
				Image equipedSkillIcon = equipedSkillButtons [i].transform.FindChild("SkillIcon").GetComponent<Image> ();

				equipedSkillIconOfHUD.sprite = equipedSkillIcon.sprite;

				if (equipedSkillIconOfHUD.sprite != null) {
					equipedSkillIconOfHUD.enabled = true;
				}

				Text equipedSkillNameOfHUD = equipedSkillButtonsOfHUD [i].GetComponentInChildren<Text> ();
				equipedSkillNameOfHUD.text = Player.mainPlayer.skillsEquiped [i].skillName;

			}

		} else {
			tintHUD.SetActive (true);
			tintHUD.GetComponentInChildren<Text>().text = "不能装备未掌握的技能";
		}

	}
	// 已装备技能栏上的技能点击响应

	public void OnEquipedSkillButtonClick(int index){
		skillDetailHUD.SetActive (true);
		Skill s = Player.mainPlayer.skillsEquiped [index];
		skillNameOnHUD.text = s.skillName;
		skillDescOnHUD.text = s.skillDescription;
		skillCosumeOnHUD.text = "气力消耗： " + s.strengthConsume.ToString () + "点";
		skillCoolenOnHUD.text = "冷却回合：" + s.actionConsume.ToString () + "回合"; 
	}


	// 点击已装备技能详细介绍弹窗
	public void OnSkillButtonOnEquipedSkillHUDClick(List<Sprite> spritesOfCurrentType, int currentSelectSkillIndex,int btnIndex){

		Image SkillIcon = equipedSkillButtons [btnIndex].transform.FindChild ("SkillIcon").GetComponent<Image> ();
		SkillIcon.sprite = spritesOfCurrentType [currentSelectSkillIndex];

		QuitTintHUD ();

	}

	// 退出按钮点击响应
	public void OnQuitSkillsPlane(CallBack cb){

		skillsViewContainer.GetComponent<Image> ().color = new Color (0, 0, 0, 0);

		skillPlane.transform.DOLocalMoveY (-Screen.height, 0.5f).OnComplete (() => {
			cb();
		});

	}
	// 提示弹窗点击响应
	public void QuitTintHUD(){
		tintHUD.SetActive (false);
	}


	// 已装备技能弹窗点击响应
	public void QuitEquipedSkillsHUD(){


		for (int i = 0; i < equipedSkillButtonsOfHUD.Length; i++) {

			Image equipedSkillIconOfHUD = equipedSkillButtonsOfHUD [i].transform.FindChild("SkillIcon").GetComponent<Image> ();
//			Image equipedSkillIcon = equipedSkillButtons [i].transform.FindChild("SkillIcon").GetComponent<Image> ();
			Text equipedSkillNameOfHUD = equipedSkillButtonsOfHUD [i].GetComponentInChildren<Text> ();

			equipedSkillIconOfHUD.sprite = null;
			equipedSkillIconOfHUD.enabled = false;

			equipedSkillNameOfHUD.text = string.Empty;

		}

		equipedSkillsHUD.SetActive (false);
	}
	// 已装备技能详细信息弹窗点击响应
	public void OnEquipedSkillDetailHUDClick(){
		skillDetailHUD.SetActive (false);
	}

}
