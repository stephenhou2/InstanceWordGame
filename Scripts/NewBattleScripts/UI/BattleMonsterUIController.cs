using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace WordJourney
{
	public class BattleMonsterUIController: BattleAgentUIController {

		public Text monsterNameText;

		public Monster monster;

		public AudioClip attackSound1;						//First of two audio clips to play when attacking the player.
		public AudioClip attackSound2;						//Second of two audio clips to play when attacking the player.


		private Animator animator;							//Variable of type Animator to store a reference to the enemy's Animator component.


		//Start overrides the virtual Start function of the base class.
		protected void Awake ()
		{

			//Get and store a reference to the attached Animator component.
			animator = GetComponent<Animator> ();

			//Call the start function of our base class MovingObject.
//			base.Awake ();
		}
			


		public void SetUpMonsterView(Monster monster){
			// 加载怪物头像图片
			ResourceManager.Instance.LoadAssetWithFileName ("battle/monster_icons", () => {

			}, true, monster.agentIconName);

		}



	}
}
