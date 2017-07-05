using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Skill:MonoBehaviour {

	public string skillName;// 技能名称

	public int skillId;

	public string skillIconName;

	public string skillDescription;

	public BaseSkillEffect[] skillEffects;//魔法效果数组

	public int strengthConsume;//技能的气力消耗

	public int actionConsume;//技能的行动数

	public int actionCount;//从释放技能开始已经走过的回合数

	public int skillLevel;// 技能等级

	public bool isAvalible = true;

	public bool isCopiedSkill = false;

	public int copiedSkillAvalibleTime = 2;

	public bool needSelectEnemy;

	public string skillType;

	public string associatedSkillName;

	public int associatedSkillUnlockLevel;

	public bool unlocked;

	public void AffectAgents(BattleAgent self, List<BattleAgent> friends,BattleAgent targetEnemy, List<BattleAgent> enemies,int skillLevel){
		foreach (BaseSkillEffect bse in skillEffects) {

			if (!bse.isStateEffect) {
				bse.AffectAgents (self,friends,targetEnemy,enemies, skillLevel, TriggerType.None, 0);
			} else {
				BattleAgentStatesManager.AddStateCopyToBattleAgents (self, friends, targetEnemy, enemies, bse as StateSkillEffect, skillLevel);
			}
		}
//		if (isCopiedSkill) {
//			copiedSkillAvalibleTime--;
//			if (copiedSkillAvalibleTime <= 0) {
//				self.skills.Remove (this);
//			}
//		}
	}

//	public int CompareTo(Skill other){
//		Debug.Log (this.skillId);
//		Debug.Log (other.skillId);
//		if (this.skillId < other.skillId) {
//			return 1;
//		} else if (this.skillId == other.skillId) {
////			RankException e = new RankException ();
//			Debug.Log ("same skill id");
////			throw e;
//		}
//		return 0;
//
//	}

	public override string ToString ()
	{
//		return string.Format ("[Skill]" + "\n[SkillName]:" + skillName + "\n[StrengthConsume]:" + strengthConsume + "\n[ActionConsume]:" + actionConsume + "\n[effect1]:" + skillEffects[0].effectName + "\n[effect2]:" + skillEffects[1].effectName);
		return string.Format ("[Skill]" + "\n[SkillName]:" + skillName + "\n[StrengthConsume]:" + strengthConsume + "\n[ActionConsume]:" + actionConsume);
	}

}

