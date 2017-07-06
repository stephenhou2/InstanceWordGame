using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SkillsView : MonoBehaviour{

	public GameObject skillPlane;

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

	private int currentSelectSkillIndex;
	private string currentSkillType;

	private List<Skill> skills = new List<Skill> ();
	private List<Sprite> sprites = new List<Sprite> ();


	private List<Skill> skillsOfCurrentType = new List<Skill> ();
	private List<Sprite> spritesOfCurrentType = new List<Sprite> ();

	public Sprite skillTypeButtonNormalIcon;
	public Sprite skillTypeButtonSelectedIcon;


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

//	public SkillsViewController ctrl;// 控制器


	/// <summary>
	/// 初始化技能页面
	/// </summary>
	/// <param name="skills">Skills.</param> 从assetBundle加载的所有技能
	/// <param name="sprites">Sprites.</param> 从assetBundle加载的所有技能图片
	public void SetUpSkillsView(List<Skill> skills,List<Sprite>sprites){
		
		this.skills = skills;
		foreach (Sprite s in sprites) {
			this.sprites.Add (s);
		}

		Player player = Player.mainPlayer;

		skillPointsTotal.text = player.agentLevel.ToString();

		skillPointsLeft.text = player.skillPointsLeft.ToString();

		for (int i = 0; i < player.skillsEquipped.Count; i++) {
			
			Image skillIcon = equipedSkillButtons [i].transform.FindChild ("SkillIcon").GetComponent<Image> ();

			skillIcon.sprite = sprites.Find (delegate (Sprite obj) {
				return obj.name == player.skillsEquipped [i].skillIconName;
			});
			skillIcon.enabled = true;
		}

		OnSkillTypeButtonClick ("type1_0");
		OnSkillTreeButtonClick (0);
		skillPlane.SetActive (true);

	}

	// 技能类型按钮点击响应
	public void OnSkillTypeButtonClick(string skillTypeInfo){
		Debug.Log (skillTypeInfo);
		string[] strs = skillTypeInfo.Split (new char[] {'_'});
		string skillType = strs [0];
//		Debug.Log (strs [0]);
//		Debug.Log (strs [1]);
		Debug.Log(strs.Length);
		int buttonIndex = Convert.ToInt32 (strs [1]);

		for (int i = 0; i < skillTypeButtons.Length; i++) {
			Button skillTypeBtn = skillTypeButtons [i];
			if (i == buttonIndex) {
				skillTypeBtn.GetComponent<Image> ().sprite = skillTypeButtonSelectedIcon;
			} else {
				skillTypeBtn.GetComponent<Image> ().sprite = skillTypeButtonNormalIcon;
			}
		}

		skillsOfCurrentType.Clear ();
		spritesOfCurrentType.Clear ();
		currentSkillType = skillTypeInfo;

		for(int i = 0;i<skills.Count;i++){
			Skill s = skills [i];
			if(s.skillType == skillType){
				skillsOfCurrentType.Add(s);
			}
		}

		SortSkillsOfCurrentTypeById ();

		foreach (Skill s in skillsOfCurrentType) {
			Sprite sprite = sprites.Find(delegate (Sprite obj){
				return obj.name == s.skillIconName;
			});
			spritesOfCurrentType.Add(sprite);
		}

		for(int i = 0;i<skillsOfCurrentType.Count;i++){
			
			Skill s = skillsOfCurrentType[i];
			Button skillButton = skillTreeButtons[i];
			Image skillIcon = skillButton.transform.FindChild("SkillIcon").GetComponent<Image>();
			skillIcon.sprite = spritesOfCurrentType[i];
			skillIcon.enabled = true;

			Text skillLevel = skillButton.transform.FindChild("SkillLevel").GetComponent<Text>();

			Skill associatedUnlockSkill = GetPlayerLearnedSkill (s.associatedSkillName);
			Skill playerSkill = GetPlayerLearnedSkill (s.skillName);

//			Debug.Log ("player skill" + playerSkill);
//			Debug.Log ("associatedSkill" + associatedUnlockSkill);

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
	public void OnSkillTreeButtonClick(int buttonIndex){
		currentSelectSkillIndex = buttonIndex;
		Skill s = skillsOfCurrentType [buttonIndex];

		for (int i = 0; i < skillTreeButtons.Length; i++) {
			skillTreeButtons [i].transform.FindChild ("SelectedIcon").GetComponent<Image> ().enabled = 
				i == buttonIndex;
		}
		skillBigIcon.sprite = spritesOfCurrentType [buttonIndex];
		skillBigIcon.enabled = true;
		skillLevelOnBigIcon.text = "Lv." + s.skillLevel.ToString ();
		skillName.text = s.skillName;
		skillDesc.text = s.skillDescription;
		skillCosume.text = "气力消耗： " + s.strengthConsume.ToString ();
		skillCoolen.text = "冷却：" + s.actionConsume.ToString () + "回合";



		Image mask = skillTreeButtons [buttonIndex].transform.FindChild ("SkillMask").GetComponent<Image> ();

		if (mask.enabled == true) {
			skillUnlock.text = "<color=red>解锁：" + s.associatedSkillName + "等级>=" + s.associatedSkillUnlockLevel + "</color>";
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
	public void OnUpgradeSkillButtonClicked(){

		Skill skillToUpgrade = GetPlayerLearnedSkill (skillsOfCurrentType [currentSelectSkillIndex].skillName);
		Skill skillAssociated = GetPlayerLearnedSkill(skillsOfCurrentType [currentSelectSkillIndex].associatedSkillName);

		if (Player.mainPlayer.skillPointsLeft <= 0) {
			Debug.Log ("剩余技能点不足，请先升级");
			return;
		}

		// 技能没有学过
		if (skillToUpgrade == null) {
			if (skillAssociated == null ||
			    skillAssociated.skillLevel >= skillsOfCurrentType [currentSelectSkillIndex].associatedSkillUnlockLevel) {
				skillToUpgrade = Instantiate (skillsOfCurrentType [currentSelectSkillIndex]);
				skillToUpgrade.name = skillsOfCurrentType [currentSelectSkillIndex].skillName;
				skillToUpgrade.transform.SetParent (Player.mainPlayer.transform.FindChild ("Skills").transform);
				skillToUpgrade.unlocked = true;
				skillToUpgrade.skillLevel++;
				Player.mainPlayer.skillPointsLeft--;
				if (skillToUpgrade.skillLevel == 1) {
					Player.mainPlayer.allLearnedSkills.Add (skillToUpgrade);
				}
				OnSkillTypeButtonClick (currentSkillType);
				skillPointsLeft.text = Player.mainPlayer.skillPointsLeft.ToString ();
				skillTreeButtons [currentSelectSkillIndex].GetComponentInChildren<Text> ().text = skillToUpgrade.skillLevel.ToString ();
				skillLevelOnBigIcon.GetComponentInChildren<Text> ().text = "Lv." + skillToUpgrade.skillLevel.ToString ();
			} 
			// 技能学过
			else{
				tintHUD.SetActive (true);
				tintHUD.GetComponentInChildren<Text>().text = skillsOfCurrentType [currentSelectSkillIndex].skillName + 
					"到达" + 
					skillsOfCurrentType [currentSelectSkillIndex].associatedSkillUnlockLevel.ToString() + 
					"级后解锁";
			}
		} else {
			if (skillAssociated != null &&
				skillAssociated.skillLevel >= skillsOfCurrentType [currentSelectSkillIndex].associatedSkillUnlockLevel) {
				skillToUpgrade.unlocked = true;
				skillToUpgrade.skillLevel++;
				Player.mainPlayer.skillPointsLeft--;

				skillPointsLeft.text = Player.mainPlayer.skillPointsLeft.ToString ();
				skillTreeButtons [currentSelectSkillIndex].GetComponentInChildren<Text> ().text = skillToUpgrade.skillLevel.ToString ();
				skillLevelOnBigIcon.GetComponentInChildren<Text> ().text = "Lv." + skillToUpgrade.skillLevel.ToString ();
				OnSkillTypeButtonClick (currentSkillType);
			} else{
				tintHUD.SetActive (true);
				tintHUD.GetComponentInChildren<Text>().text = skillsOfCurrentType [currentSelectSkillIndex].skillName + "到达" + skillToUpgrade.associatedSkillUnlockLevel.ToString() + "级后解锁";
			}

		}
	}

	// 装备按钮点击响应
	public void OnEquipButtonClick(){

		Skill playerSkill = Player.mainPlayer.allLearnedSkills.Find (delegate(Skill obj) {
			return obj.skillId == skillsOfCurrentType [currentSelectSkillIndex].skillId;
		});

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
					Player.mainPlayer.skillsEquipped.Insert (i, playerSkill);
					return;
				}
			} 
			equipedSkillsHUD.SetActive (true);
			for (int i = 0; i < equipedSkillButtonsOfHUD.Length; i++) {
				Image equipedSkillIconOfHUD = equipedSkillButtonsOfHUD [i].transform.FindChild("SkillIcon").GetComponent<Image> ();
				Image equipedSkillIcon = equipedSkillButtons [i].transform.FindChild("SkillIcon").GetComponent<Image> ();
				equipedSkillIconOfHUD.sprite = equipedSkillIcon.sprite;

				Text equipedSkillNameOfHUD = equipedSkillButtonsOfHUD [i].GetComponentInChildren<Text> ();
				equipedSkillNameOfHUD.text = Player.mainPlayer.skillsEquipped [i].skillName;

			}

		} else {
			tintHUD.SetActive (true);
			tintHUD.GetComponentInChildren<Text>().text = "不能装备未掌握的技能";
		}

	}
	// 已装备技能栏上的技能点击响应
	public void OnEquipedSkillButtonClick(int index){
		skillDetailHUD.SetActive (true);
		Skill s = Player.mainPlayer.skillsEquipped [index];
		skillNameOnHUD.text = s.skillName;
		skillDescOnHUD.text = s.skillDescription;
		skillCosumeOnHUD.text = "气力消耗： " + s.strengthConsume.ToString ();
		skillCoolenOnHUD.text = "冷却：" + s.actionConsume.ToString () + "回合"; 
	}


	// 已装备技能弹窗上按技能的点击响应
	public void OnSkillButtonOnEquipedSkillHUDClick(int index){

		Player.mainPlayer.skillsEquipped.RemoveAt (index);

		Skill playerSkill = Player.mainPlayer.allLearnedSkills.Find (delegate(Skill obj) {
			return obj.skillId == skillsOfCurrentType [currentSelectSkillIndex].skillId;
		});
		Player.mainPlayer.skillsEquipped.Insert (index, playerSkill);

		Image SkillIcon = equipedSkillButtons [index].transform.FindChild ("SkillIcon").GetComponent<Image> ();
		SkillIcon.sprite = spritesOfCurrentType [currentSelectSkillIndex];
		OnEquipedSkillHUDClick ();

	}

	// 退出按钮点击响应
	public void OnQuitSkillsPlane(){

//		GameObject skillCanvas = GameObject.Find ("SkillsCanvas");
		skillPlane.transform.DOLocalMoveY (-Screen.height, 0.5f).OnComplete (() => {
			Destroy (GameObject.Find ("SkillsCanvas"));
			Destroy (GameObject.Find (CommonData.instanceContainerName + "/Skills").gameObject);
		});

	}
	// 提示弹窗点击响应
	public void OnTintHUDClick(){
		tintHUD.SetActive (false);
	}
	// 已装备技能弹窗点击响应
	public void OnEquipedSkillHUDClick(){
		equipedSkillsHUD.SetActive (false);
	}
	// 已装备技能详细信息弹窗点击响应
	public void OnEquipedSkillDetailHUDClick(){
		skillDetailHUD.SetActive (false);
	}

	// 技能按照id排序方法
	private void SortSkillsOfCurrentTypeById(){
		Skill temp;
		for(int i = 0;i<skillsOfCurrentType.Count - 1;i++) {
			for(int j = 0;j<skillsOfCurrentType.Count - 1 - i;j++){
				Skill sBefore = skillsOfCurrentType [j];
				Skill sAfter = skillsOfCurrentType [j + 1];
				if (sBefore.skillId > sAfter.skillId) {
					temp = sBefore;
					skillsOfCurrentType [j] = sAfter;
					skillsOfCurrentType [j + 1] = temp; 
				}

			}
		}
	}

	// 获取玩家已学习的技能
	private Skill GetPlayerLearnedSkill(string skillName){
		Skill s = null;
		s = Player.mainPlayer.allLearnedSkills.Find (delegate(Skill obj) {
			return obj.skillName == skillName;	
		});
		return s;
	}

}
