using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	public enum SkillType{
		None,
		Physical,
		Magical,
		Passive
	}

	public abstract class Skill:MonoBehaviour {

		public string skillName;// 技能名称

		public string sfxName;//音效名称

		public int skillId;

		public SkillType skillType;

		public string skillIconName;

		public string skillDescription;


		public static float dodgeSeed = 0.0035f; //计算闪避时的种子数

		public static float critSeed = 0.0035f; //计算暴击时的种子数

		public static float armorSeed = 0.01f; //计算护甲抵消伤害的种子数

		public static float magicResistSeed = 0.01f; //计算魔抗抵消伤害的种子数


		public int skillLevel;

		public bool isAvalible = true;

		public bool canOverlay;// 技能效果是否可以叠加


		public string selfAnimName;
		public string selfIntervalAnimName;
		public string enemyAnimName;

		public string selfEffectName;
		public string enemyEffectName;


		public bool unlocked;

		public virtual void AffectAgents (BattleAgentController self, BattleAgentController enemy){
		}

		protected abstract void ExcuteSkillLogic (BattleAgentController self, BattleAgentController enemy);


		// 判断概率性技能是否生效
		protected bool isEffective(float chance){
			float randomNum = Random.Range (0, 100)/100f;
			return randomNum <= chance;
		}

		public static Skill LoadSkillFromWithSkillInfo(SkillInfo skillInfo){

			List<Skill> allSkills = GameManager.Instance.gameDataCenter.allSkills;

			Skill skill = allSkills.Find (delegate(Skill obj) {
				return obj.skillId == skillInfo.skillId;
			});

			skill.skillLevel = skillInfo.skillLevel;

			return skill;

		}

		public override string ToString ()
		{
			return string.Format ("[Skill]" + "\n[SkillName]:" + skillName);
		}

	}

	[System.Serializable]
	public class SkillInfo{

		public int skillId;

		public int skillLevel;

		public SkillInfo(Skill skill){
			this.skillId = skill.skillId;
			this.skillLevel = skill.skillLevel;
		}

	}
}
	

