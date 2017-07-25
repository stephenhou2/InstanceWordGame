using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAgentStatesManager:MonoBehaviour {



	// 状态管理方法，提供状态类效果的等级信息和作用的单位信息
	public static void AddStateCopyToBattleAgents (BattleAgent self,List<BattleAgent> friends,BattleAgent targetEnemy, List<BattleAgent> enemies, StateSkillEffect sse,int skillLevel)
	{
		sse.skillLevel = skillLevel;
		// 根据技能指向将状态加到指定的对象身上
		switch (sse.effectTarget) {
		case SkillEffectTarget.Self:
			AddStateTo (self,sse);
			break;
		case SkillEffectTarget.SpecificEnemy:
			AddStateTo (targetEnemy,sse);
			break;
		case SkillEffectTarget.AllEnemies:
			foreach (BattleAgent enemy in enemies) {
				AddStateTo (enemy, sse);
			}
			break;
		case SkillEffectTarget.AllFriends:
			foreach (BattleAgent friend in friends) {
				AddStateTo (friend, sse);
			}
			break;
		case SkillEffectTarget.BothSides:
			AddStateTo (self,sse);
			foreach (BattleAgent enemy in enemies) {
				AddStateTo (enemy, sse);
			}
			foreach (BattleAgent friend in friends) {
				AddStateTo (friend, sse);
			}
			break;
		default:
			break;
		}

	}

	// 将状态添加到对象身上
	public static void AddStateTo(BattleAgent ba,StateSkillEffect sse){

		StateSkillEffect state = null;
		for (int i = 0; i < ba.states.Count; i++) {
			state = ba.states [i];
			if (state.id == sse.id) {
				state.actionCount = 1;
				return;
			}
		}

		state = Instantiate (sse,ba.transform.FindChild ("States").transform);
		ba.AddState (state);
		Debug.Log (ba.agentName + " add state: " + state.effectName);

	}


	public static void CheckStates(List<BattleAgent> players,List<BattleAgent> monsters){

		CheckStatesOf (players);
		CheckStatesOf (monsters);


	}


	private static void CheckStatesOf(List<BattleAgent> bas){
		for (int i = 0; i < bas.Count; i++) {
			BattleAgent ba = bas [i];
			for(int j = 0;j<ba.states.Count;j++){
				StateSkillEffect sse = ba.states [j];
				sse.actionCount++;
				Debug.Log ("---------" + sse.effectName + sse.actionCount);
				if (sse.actionCount > sse.effectDuration) {
					ba.RemoveState (sse);
					Debug.Log (ba.agentName + "remove state:" + sse.effectName);
					Destroy (sse);
					j--;//删除sse后状态数组的长度-1，由于从前向后遍历，j--
				}

			}
		}

	}
}
