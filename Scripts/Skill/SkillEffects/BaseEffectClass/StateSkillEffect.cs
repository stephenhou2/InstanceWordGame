using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class StateSkillEffect : BaseSkillEffect {

	public bool hasBeenExcuted;

	public BattleAgent[] enemies;

//	// 状态管理方法，提供状态类效果的等级信息和作用的单位信息
//	public void AddStateCopyToEffectTarget (BattleAgent self, BattleAgent enemy, int skillLevel,TriggerType triggerType,int attachedInfo)
//	{
//		this.skillLevel = skillLevel;//效果等级等于技能等级
//
//		// 根据技能指向将状态加到指定的对象身上
//		switch (effectTarget) {
//		case SkillEffectTarget.Self:
//			AddStateTo (self);
//			break;
//		case SkillEffectTarget.Enemy:
//			AddStateTo (enemy);
//			break;
//		case SkillEffectTarget.Both:
//			AddStateTo (self);
//			AddStateTo (enemy);
//			break;
//		default:
//			break;
//		}
//
//	}

//	public bool ManagerStateOf(BattleAgent ba){
//		if (this.actionCount > effectDuration) {
//			ba.RemoveState (this);
//			Debug.Log (ba.agentName + "remove state:" + this.name);
//			Destroy (this);
//			return true;
//
//		}
//		return false;
//	}

//	// 将状态添加到对象身上
//	public virtual void AddStateTo(BattleAgent ba){
//		
//		StateSkillEffect sse = null;
//
//		for (int i = 0; i < ba.states.Count; i++) {
//			sse = ba.states [i];
//			if (this.id == sse.id) {
//				sse.actionCount = 1;
//				return;
//			}
//		}
//		sse = Instantiate (this);
//		ba.AddState (sse,skillLevel);
//		Debug.Log (ba.agentName + " add state: " + sse.name);
//
//	}

	public void ExcuteEffect(BattleAgent self,List<BattleAgent> friends, BattleAgent targetEnemy,
		List<BattleAgent> enemies, int skillLevel,TriggerType triggerType,int attachedInfo){
		if (hasBeenExcuted && !canOverlay) {
			return;
		}
		AffectAgents(self,friends,targetEnemy, enemies, skillLevel,triggerType,attachedInfo);
		hasBeenExcuted = true;
	}

}
