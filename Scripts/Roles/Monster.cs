using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



namespace WordJourney
{
	[System.Serializable]
	public struct MonsterSkill{
		public Skill skill;
		public float probability;
	}

	public class Monster : Agent{

		public int monsterId;

		public int rewardExperience;//奖励的经验值

		public MonsterSkill[] allEquipedSkills;


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

			return null;
		}





	}
}
