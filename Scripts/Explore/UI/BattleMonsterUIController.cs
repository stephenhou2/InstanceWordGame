using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace WordJourney
{
	public class BattleMonsterUIController: BattleAgentUIController {


		public Text monsterNameText;

		public Monster monster;

		public Transform monsterStatusPlane;
	

		public void SetUpMonsterStatusPlane(Monster monster){
			
			monsterNameText.text = monster.agentName;
			healthBar.maxValue = monster.maxHealth;
			healthBar.value = monster.maxHealth;
			healthText.text = string.Format ("{0}/{1}", monster.health, monster.maxHealth);

			UpdateSkillStatusPlane (monster);

			monsterStatusPlane.gameObject.SetActive (true);
		}

		public override void UpdateAgentStatusPlane (){

			UpdateHealthBarAnim (monster);

			UpdateSkillStatusPlane (monster);

		}

		public override void QuitFight ()
		{
			monsterStatusPlane.gameObject.SetActive (false);
		}
//		public void PlayMonsterDieAnim(BattleAgentController baCtr,CallBack<Transform> cb,Transform[] transArray){
//
//			baCtr.GetComponent<SpriteRenderer> ().DOFade (0, 0.5f).OnComplete(()=>{
//				baCtr.gameObject.SetActive(false);
//
//				if(cb != null){
//					cb(transArray);
//				}
//			});
//		}

	}
}
