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
	
		private Animator animator;							//Variable of type Animator to store a reference to the enemy's Animator component.


		protected override void Awake ()
		{

			animator = GetComponent<Animator> ();

			base.Awake ();

		}
			


		public void SetUpMonsterStatusPlane(Monster monster){

			monsterNameText.text = monster.agentName;
			healthBar.maxValue = monster.maxHealth;
			healthBar.value = monster.maxHealth;
			healthText.text = string.Format ("{0}/{1}", monster.health, monster.maxHealth);
			attackText.text = string.Format ("攻击:{0}", monster.attack);
			attackSpeedText.text = string.Format ("攻速:{0}", monster.attackSpeed);;
			amourText.text = string.Format ("护甲:{0}", monster.amour);
			manaResistText.text = string.Format ("抗性:{0}", monster.manaResist);
			critText.text = string.Format ("暴击:{0}", monster.crit);
			agilityText.text = string.Format ("闪避:{0}", monster.agility);


		}

		public void UpdateMonsterStatusPlane(){

			UpdateHealthBarAnim (monster);

		}

		public void PlayMonsterDieAnim(BattleAgentController baCtr,CallBack<Transform> cb,Transform[] transArray){

			baCtr.GetComponent<SpriteRenderer> ().DOFade (0, 0.5f).OnComplete(()=>{
				baCtr.gameObject.SetActive(false);

				if(cb != null){
					cb(transArray);
				}

			});


		}

	}
}
