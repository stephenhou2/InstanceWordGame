using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



namespace WordJourney
{
	[System.Serializable]
	public struct SkillWithProbability{
		public Skill skill;
		public float probability;
	}

	public class Monster : Agent{

		public int monsterId;

		public int rewardExperience;//奖励的经验值

		public SkillWithProbability[] allEquipedActiveSkills;

		public Skill[] allEquipedPassiveSkills;


		private BattleMonsterController mBaMonsterController;

		// 角色UIView
		public BattleMonsterController baView{

			get{
				if (mBaMonsterController == null) {
					mBaMonsterController = GetComponent<BattleMonsterController> ();
				}
				return mBaMonsterController;
			}

		}

//		override void Awake(){
//			base.Awake ();
//			for (int i = 0; i < allEquipedSkills.Length; i++) {
//				Skill s = allEquipedSkills [i];
//				if (s.skillType == SkillType.Passive) {
//					
//				}
//			}
//		}


		public Skill InteligentSelectSkill(){

			int randomNum = Random.Range (0, 100);

			#if UNITY_STANDALONE || UNITY_EDITOR
			float totalProbability = 0f;
			for(int i = 0;i<allEquipedActiveSkills.Length;i++){
				totalProbability += allEquipedActiveSkills[i].probability;
			}

			if(totalProbability != 1f){
				Debug.LogError("total probability = " + totalProbability.ToString());
			}
			#endif

			Skill inteligentSelectedSkill = null;

			int chip = 0;

			for (int i = 0; i < allEquipedActiveSkills.Length; i++) {
				chip += (int)(allEquipedActiveSkills [i].probability * 100);
				if (randomNum < chip) {
					inteligentSelectedSkill = allEquipedActiveSkills [i].skill;
					break;
				}
			}

			return inteligentSelectedSkill;

		}





	}
}
