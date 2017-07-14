using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Monster : BattleAgent,IPointerClickHandler{

	public int monsterId;

	private BattleMonsterView mBaMonsterView;

	// 角色UIView
	public new BattleMonsterView baView{

		get{
			if (mBaMonsterView == null) {
				mBaMonsterView = GetComponent<BattleMonsterView> ();
			}
			return mBaMonsterView;
		}

	}

	public void SetupMonster(int gameProcess){
//		GameManager.gameManager.OnGenerateSkill ();
//		GameManager.gameManager.skillGenerator.GenerateSkillWithIds (2, 20,this);
	}
		
	//怪物的技能选择
//	public Skill SkillOfMonster(){
//		Skill monsterSkill = null;
//		switch (validActionType) {
//		case ValidActionType.All:
//			
//			break;
//		case ValidActionType.PhysicalExcption:
//			
//			break;
//		case ValidActionType.MagicException:
//			
//			break;
//		case ValidActionType.PhysicalOnly:
//			
//			break;
//		case ValidActionType.MagicOnly:
//			
//			break;
//		default:
//			break;
//		}
//
//		return monsterSkill;
//
//	}


	public void ManageSkillAvalibility(){
		// 如果技能还在冷却中或者怪物气力值小于技能消耗的气力值，则相应技能不可用
		for (int i = 0;i < skillsEquiped.Count;i++) {
			Skill s = skillsEquiped [i];

			if (s.isAvalible == false) {
				s.actionCount++;
				if (s.actionCount >= s.actionConsume && strength >= s.strengthConsume) {
					s.isAvalible = true;
					s.actionCount = 0;
				} 
			}
		}

	}



	public void OnPointerClick(PointerEventData data){
		GameObject.Find (CommonData.battleCanvas).GetComponent<BattleViewController> ().OnPlayerSelectMonster (monsterId);
	}

}
