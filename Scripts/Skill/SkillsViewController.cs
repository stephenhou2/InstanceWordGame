using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillsViewController : MonoBehaviour {

	public SkillsView skillsView;

	private List<Skill> mSkills = new List<Skill>();
	private List<Sprite> mSprites = new List<Sprite>();


	private List<Skill> skillsOfCurrentType = new List<Skill> ();
	private List<Sprite> spritesOfCurrentType = new List<Sprite> ();

	private int currentSelectSkillIndex;
	private int currentSelectSkillTypeIndex;



	public void SetUpSkillsView(){

		if(mSkills.Count == 0 || mSprites.Count == 0)
		ResourceManager.Instance.LoadAssetWithFileName ("skills/skills", () => {

			Transform skillsTrans = TransformManager.NewTransform("Skills",GameObject.Find(CommonData.instanceContainerName).transform);
			foreach(GameObject go in ResourceManager.Instance.gos){
				mSkills.Add(go.GetComponent<Skill>());
				go.transform.SetParent(skillsTrans);
			}

			foreach (Sprite s in ResourceManager.Instance.sprites) {
				mSprites.Add (s);
			}

			skillsView.SetUpSkillsView (mSprites);

			OnSkillTypeButtonClick (0);
			OnSkillTreeButtonClick (0);

			GetComponent<Canvas>().enabled = true; 
		});

	}

	public void OnSkillTypeButtonClick(int typeIndex){

		// 根据传入的序号判断选择的技能树类型
		string skillType = string.Empty;

		switch (typeIndex) {
		case 0:
			skillType = "type0";
			break;
		case 1:
			skillType = "type1";
			break;
		case 2:
			skillType = "type2";
			break;
		case 3:
			skillType = "type3";
			break;
		default:
			break;

		}

		// 清空内存中的当前技能树和其对应的图片
		skillsOfCurrentType.Clear ();
		spritesOfCurrentType.Clear ();

		// 当前选中的技能树序号
		currentSelectSkillTypeIndex = typeIndex;

		// 从本地读取的所有技能中选出目标技能树下的所有技能
		for(int i = 0;i < mSkills.Count;i++){
			Skill s = mSkills [i];
			if(s.skillType == skillType){
				skillsOfCurrentType.Add(s);
			}
		}

		// 对技能树下的所有技能进行排序
		SortSkillsOfCurrentTypeById ();

		// 根据排序后的技能树，找到对应的技能图片
		foreach (Skill s in skillsOfCurrentType) {
			Sprite sprite = mSprites.Find(delegate (Sprite obj){
				return obj.name == s.skillIconName;
			});
			spritesOfCurrentType.Add(sprite);
		}

		skillsView.OnSkillTypeButtonClick (skillsOfCurrentType, spritesOfCurrentType, typeIndex);

	}

	// 技能树上技能的点击响应
	public void OnSkillTreeButtonClick(int buttonIndex){
		
		currentSelectSkillIndex = buttonIndex;

		Skill skill = skillsOfCurrentType [buttonIndex];

		Sprite sprite = spritesOfCurrentType [buttonIndex];

		skillsView.OnSkillTreeButtonClick (skill, sprite, buttonIndex);

	}

	public void OnUpgradeButtonClick(){

		// 如果玩家没有可用技能点
		if (Player.mainPlayer.skillPointsLeft <= 0) {
			Debug.Log ("剩余技能点不足，请先升级");
			return;
		}

		// 玩家有可用的技能点

		// 获取玩家想要升级的技能
		Skill skillToUpgradeInLearnedSkills = Player.mainPlayer.GetPlayerLearnedSkill (skillsOfCurrentType [currentSelectSkillIndex].skillName);
		Skill skillAssociatedInLearnedSkills = Player.mainPlayer.GetPlayerLearnedSkill(skillsOfCurrentType [currentSelectSkillIndex].associatedSkillName);

		// 想要升级的技能之前没有学过
		if (skillToUpgradeInLearnedSkills == null) {
			
			// 想要升级的技能达到了解锁要求（关联的解锁技能等级满足解锁要求）
			if (skillsOfCurrentType [currentSelectSkillIndex].unlocked || (skillAssociatedInLearnedSkills != null &&
			    skillAssociatedInLearnedSkills.skillLevel >= skillsOfCurrentType [currentSelectSkillIndex].associatedSkillUnlockLevel)) {

				// 生成技能
				skillToUpgradeInLearnedSkills = Instantiate (skillsOfCurrentType [currentSelectSkillIndex]);
				skillToUpgradeInLearnedSkills.name = skillsOfCurrentType [currentSelectSkillIndex].skillName;
				skillToUpgradeInLearnedSkills.transform.SetParent (Player.mainPlayer.transform.FindChild ("Skills").transform);

				// 技能标记为已解锁
				skillToUpgradeInLearnedSkills.unlocked = true;

				// 技能等级 + 1
				skillToUpgradeInLearnedSkills.skillLevel++;

				// 玩家可用技能点 - 1
				Player.mainPlayer.skillPointsLeft--;

				// 将该技能加入到玩家已学习过的技能列表中
				Player.mainPlayer.allLearnedSkills.Add (skillToUpgradeInLearnedSkills);

				// 更新技能界面
				OnSkillTypeButtonClick (currentSelectSkillTypeIndex);

			} else {
				Debug.Log ("关联技能等级不够");
			}
		} 
		// 想要升级的技能已经学习过（说明一定已经解锁了该技能）
		else {
			
			// 技能等级 + 1
			skillToUpgradeInLearnedSkills.skillLevel++;

			// 玩家可用技能点 - 1
			Player.mainPlayer.skillPointsLeft--;

			// 更新技能界面
			OnSkillTypeButtonClick (currentSelectSkillTypeIndex);

		}

//		skillsView.OnUpgradeSkillButtonClicked ( currentSelectSkillIndex,);

	}


	// 装备按钮点击响应
	public void OnEquipButtonClick(){

		Skill playerSkill = Player.mainPlayer.allLearnedSkills.Find (delegate(Skill obj) {
			return obj.skillId == skillsOfCurrentType [currentSelectSkillIndex].skillId;
		});

		skillsView.OnEquipButtonClick (playerSkill,spritesOfCurrentType,currentSelectSkillIndex);
	}


	public void OnSkillButtonOnEquipedSkillHUDClick(int index){

		Player.mainPlayer.skillsEquiped.RemoveAt (index);

		Skill playerSkill = Player.mainPlayer.allLearnedSkills.Find (delegate(Skill obj) {
			return obj.skillId == skillsOfCurrentType [currentSelectSkillIndex].skillId;
		});

		Player.mainPlayer.skillsEquiped.Insert (index, playerSkill);

		skillsView.OnSkillButtonOnEquipedSkillHUDClick (spritesOfCurrentType, currentSelectSkillIndex, index);

		skillsView.QuitEquipedSkillsHUD ();
	}

	public void OnEquipedSkillsHUDClick(){

		skillsView.QuitEquipedSkillsHUD ();

	}

	public void OnTintHUDClick(){

		skillsView.QuitTintHUD ();

	}

	public void OnQuitButtonClick(){

		skillsView.OnQuitSkillsPlane (DestroyInstances);

		GameObject homeCanvas = GameObject.Find (CommonData.instanceContainerName + "/HomeCanvas");

		if (homeCanvas != null) {
			homeCanvas.GetComponent<HomeViewController> ().SetUpHomeView ();
		}
	}

	private void DestroyInstances(){

		TransformManager.DestroyTransform (gameObject.transform);

		TransformManager.DestroyTransfromWithName ("Skills", TransformRoot.InstanceContainer);

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



}
