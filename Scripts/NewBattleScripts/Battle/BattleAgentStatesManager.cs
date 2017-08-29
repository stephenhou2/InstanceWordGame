using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace WordJourney
{
	public class BattleAgentStatesManager:MonoBehaviour {

		// 状态管理方法，提供状态类效果的等级信息和作用的单位信息
		public static void AddStateCopyToBattleAgents (BattleAgentController self,BattleAgentController enemy, StateSkillEffect sse,int skillLevel,int effectDuration){

			sse.skillLevel = skillLevel;
			sse.effectDuration = effectDuration;

			// 根据技能指向将状态加到指定的对象身上
			switch (sse.effectTarget) {
			case SkillEffectTarget.Self:
				AddStateTo (self.agent,sse);
				break;
			case SkillEffectTarget.Enemy:
				AddStateTo (enemy.agent,sse);
				break;
			default:
				break;
			}

		}

		// 将状态添加到对象身上
		public static void AddStateTo(Agent ba,StateSkillEffect sse){

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


		public static void CheckStates(List<Agent> players,List<Agent> monsters){

			CheckStatesOf (players);
			CheckStatesOf (monsters);


		}


		private static void CheckStatesOf(List<Agent> bas){
			for (int i = 0; i < bas.Count; i++) {
				Agent ba = bas [i];
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
}