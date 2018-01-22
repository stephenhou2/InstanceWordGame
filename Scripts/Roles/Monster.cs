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

//		public SkillWithProbability[] allEquipedActiveSkills;

//		public Skill[] allEquipedPassiveSkills;


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

	}
}
