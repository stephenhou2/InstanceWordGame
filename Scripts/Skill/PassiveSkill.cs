using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public abstract class PassiveSkill : Skill {

		// 技能等级
		[SerializeField]private int mySkillLevel;

		public new int skillLevel{
			get{
				return mySkillLevel;
			}
			set{
				if (value > 0) {
					mySkillLevel = value;
					levelChanged = true;
				}
			}
		}

		protected bool levelChanged;

		void Start(){
			if (skillType != SkillType.Passive) {
				Debug.LogError (string.Format ("{0}技能类型必须是被动类型", skillName));
			}
		}

		public override void AffectAgents (BattleAgentController self, BattleAgentController enemy)
		{
			ExcuteSkillLogic (self, enemy);
		}

	}
}
