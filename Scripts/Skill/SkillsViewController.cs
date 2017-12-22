using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class SkillsViewController : MonoBehaviour {

		public SkillsView skillsView;

		private List<Skill> mSkills;
		private List<Sprite> mSkillSprites;

		private Skill currentSelectSkill;

		public void SetUpSkillsView(){

			StartCoroutine ("SetUpViewAfterDataReady");

		}

		private IEnumerator SetUpViewAfterDataReady(){

			bool dataReady = false;

			while(!dataReady){

				dataReady = GameManager.Instance.gameDataCenter.CheckDatasReady (new GameDataCenter.GameDataType[] {
					GameDataCenter.GameDataType.UISprites,
					GameDataCenter.GameDataType.Skills,
					GameDataCenter.GameDataType.SkillSprites
				});

				yield return null;

			}

			mSkills = GameManager.Instance.gameDataCenter.allSkills;

			mSkillSprites = GameManager.Instance.gameDataCenter.allSkillSprites;

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

				// 如果是天赋技能，则在升级的时候直接执行技能效果
				if (skillToUpgradeInLearnedSkills.skillType == SkillType.TalentPassive) {
					skillToUpgradeInLearnedSkills.AffectAgents (bpCtr, null);
				} 
				// 如果是主动技能，则将技能加入到主动技能表中
				else if(skillToUpgradeInLearnedSkills is ActiveSkill){
					Player.mainPlayer.equipedActiveSkills.Add (skillToUpgradeInLearnedSkills as ActiveSkill);
				}

				// 将技能加入到已学习技能表中
				Player.mainPlayer.allLearnedSkills.Add (skillToUpgradeInLearnedSkills);

			} 
			// 想要升级的技能已经学习过
			else {
				// 技能等级 + 1
				skillToUpgradeInLearnedSkills.skillLevel++;

				// 玩家可用技能点 - 1
				Player.mainPlayer.skillPointsLeft--;

				BattleAgentController bpCtr = Player.mainPlayer.GetComponentInChildren<BattlePlayerController> ();

				// 如果是天赋技能，则在升级的时候直接执行技能效果
				if (skillToUpgradeInLearnedSkills.skillType == SkillType.TalentPassive) {
					skillToUpgradeInLearnedSkills.AffectAgents (bpCtr, null);
				} 

				// 更新技能界面
				skillsView.OnUpgradeSkillButtonClicked (skillToUpgradeInLearnedSkills);

			}

		}

		public void OnQuitButtonClick(){
			
			Player.mainPlayer.transform.Find ("BattlePlayer").gameObject.SetActive (false);

			skillsView.QuitSkillsPlane ();

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.homeCanvasBundleName, "HomeCanvas", () => {
				TransformManager.FindTransform("HomeCanvas").GetComponent<HomeViewController>().SetUpHomeView();
			});

//			GameManager.Instance.gameDataCenter.ReleaseDataWithDataTypes (new GameDataCenter.GameDataType[]{ 
//				GameDataCenter.GameDataType.Skills, 
//				GameDataCenter.GameDataType.SkillSprites
//			});
				
		}

		public void DestroyInstances(){

			mSkills = null;

			mSkillSprites = null;

			GameManager.Instance.UIManager.DestroryCanvasWith (CommonData.skillCanvasBundleName, "SkillsCanvas", null, null);

		}
	}

}
