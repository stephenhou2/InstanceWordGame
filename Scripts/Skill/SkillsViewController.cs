using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class SkillsViewController : MonoBehaviour {

		public SkillsView skillsView;

		private List<Skill> mSkills = new List<Skill>();
		private List<Sprite> mSkillSprites = new List<Sprite>();

		private Skill currentSelectSkill;

		public void SetUpSkillsView(){

			mSkills = GameManager.Instance.dataCenter.allSkills;

			mSkillSprites = GameManager.Instance.dataCenter.allSkillSprites;

			skillsView.SetUpSkillsView (mSkills,mSkillSprites);

			GetComponent<Canvas>().enabled = true; 

			Player.mainPlayer.transform.Find ("BattlePlayer").gameObject.SetActive(true);



		}

		public void GetCurrentSelectInfo(Skill skill){
			this.currentSelectSkill = skill;
		}


		public void OnUpgradeButtonClick(){

			// 获取玩家想要升级的技能
			Skill skillToUpgradeInLearnedSkills = Player.mainPlayer.GetPlayerLearnedSkill (currentSelectSkill.skillId);

			// 想要升级的技能之前没有学过
			if (skillToUpgradeInLearnedSkills == null) {

				// 生成技能
				skillToUpgradeInLearnedSkills = Instantiate (currentSelectSkill);
				skillToUpgradeInLearnedSkills.name = currentSelectSkill.skillName;
				skillToUpgradeInLearnedSkills.transform.SetParent (Player.mainPlayer.transform.Find ("Skills").transform);

				// 技能等级 + 1
				skillToUpgradeInLearnedSkills.skillLevel++;

				// 玩家可用技能点 - 1
				Player.mainPlayer.skillPointsLeft--;

				// 将该技能加入到玩家已学习过的技能列表中
				Player.mainPlayer.allLearnedSkills.Add (skillToUpgradeInLearnedSkills);

				skillsView.OnUpgradeSkillButtonClicked (skillToUpgradeInLearnedSkills);

				BattleAgentController bpCtr = Player.mainPlayer.GetComponentInChildren<BattlePlayerController> ();

				if (skillToUpgradeInLearnedSkills.isPassive) {
					skillToUpgradeInLearnedSkills.AffectAgents (bpCtr, null);
				} else {
					Player.mainPlayer.equipedSkills.Add (skillToUpgradeInLearnedSkills);
				}
			} 
			// 想要升级的技能已经学习过
			else {
				// 技能等级 + 1
				skillToUpgradeInLearnedSkills.skillLevel++;

				// 玩家可用技能点 - 1
				Player.mainPlayer.skillPointsLeft--;

				// 更新技能界面
				skillsView.OnUpgradeSkillButtonClicked (skillToUpgradeInLearnedSkills);

			}

		}

		public void OnQuitButtonClick(){
			
			Player.mainPlayer.transform.Find ("BattlePlayer").gameObject.SetActive (false);

			skillsView.OnQuitSkillsPlane (DestroyInstances);

			GameObject homeCanvas = GameObject.Find (CommonData.instanceContainerName + "/HomeCanvas");

			if (homeCanvas != null) {
				homeCanvas.GetComponent<HomeViewController> ().SetUpHomeView ();
			} 
		}

		private void DestroyInstances(){

			mSkills = null;

			mSkillSprites = null;

			TransformManager.DestroyTransform (gameObject.transform);

			TransformManager.DestroyTransfromWithName ("Skills", TransformRoot.InstanceContainer);

			Resources.UnloadUnusedAssets ();

			System.GC.Collect ();

		}
	}

}
