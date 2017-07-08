using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Player : BattleAgent {

	private static Player mPlayerSingleton;

	private static object objectLock = new object();

	// 玩家角色单例
	public static Player mainPlayer{
		get{
			if (mPlayerSingleton == null) {
				lock (objectLock) {
					ResourceManager.Instance.LoadAssetWithFileName("player",()=>{
						mPlayerSingleton = GameObject.Find ("Player").GetComponent<Player>();
						mPlayerSingleton.transform.SetParent(null);
						mPlayerSingleton.ResetBattleAgentProperties (true,false);
						DontDestroyOnLoad (mPlayerSingleton);
					},true);
				}
			}else{
				mPlayerSingleton.ResetBattleAgentProperties (false,false);
			}

			return mPlayerSingleton;
		}
//		set{
//			mPlayerSingleton = value;
//		}

	}


	public List<Skill> allLearnedSkills = new List<Skill>();

	public int skillPointsLeft;

//	public void Awake(){
//
//		if (mainPlayer == null) {
//			mainPlayer = this;
//		} else if (mainPlayer != this) {
//			Destroy (gameObject);
//		}
//
//		DontDestroyOnLoad (gameObject);
//	}




	public void UpdateValidActionType(){

		switch (validActionType) {

		case ValidActionType.All:
			break;
		case ValidActionType.PhysicalExcption:
			SetUpPlayerValidAction(false, true, true, true);
			break;
		case ValidActionType.MagicException:
			SetUpPlayerValidAction(true, false, true, true);
			break;
		case ValidActionType.None:
			SetUpPlayerValidAction(false, false, false, false);
			break;
		case ValidActionType.PhysicalOnly:
			SetUpPlayerValidAction(true, false, false, true);
			break;
		case ValidActionType.MagicOnly:
			SetUpPlayerValidAction(false, true, false, true);
			break;
		default:
			break;
		}
		// 如果技能还在冷却中或者玩家气力值小于技能消耗的气力值，则相应按钮不可用
		for (int i = 0;i < skillsEquiped.Count;i++) {

			Skill s = skillsEquiped [i];
			// 如果是冷却中的技能
			if (s.isAvalible == false) {
				s.actionCount++;
//				int actionBackCount = s.actionConsume - s.actionCount + 1;
				Debug.Log (s.skillName + "从使用开始经过了" + s.actionCount + "回合");
				if (s.actionCount > s.actionConsume) {
					s.isAvalible = true;
					s.actionCount = 0;
				} 
			}
		}
	}

	//根据玩家的可用行动状态
	private void SetUpPlayerValidAction(bool isAttackEnable,bool isSkillEnable,bool isItemEnable,bool isDefenceEnable){
		
		this.isAttackEnable = isAttackEnable;
		this.isSkillEnable = isSkillEnable;
		this.isItemEnable = isItemEnable;
		this.isDefenceEnable = isDefenceEnable;

	}




}
