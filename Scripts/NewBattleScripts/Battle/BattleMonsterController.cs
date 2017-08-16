using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace WordJourney
{
	public class BattleMonsterController: BattleAgentController {

		public Text monsterNameText;

		public Monster monster;

		public AudioClip attackSound1;						//First of two audio clips to play when attacking the player.
		public AudioClip attackSound2;						//Second of two audio clips to play when attacking the player.


		private Animator animator;							//Variable of type Animator to store a reference to the enemy's Animator component.


		//Start overrides the virtual Start function of the base class.
		protected override void Awake ()
		{

			//Get and store a reference to the attached Animator component.
			animator = GetComponent<Animator> ();

			//Call the start function of our base class MovingObject.
			base.Awake ();
		}
			

		//OnCantMove is called if Enemy attempts to move into a space occupied by a Player, it overrides the OnCantMove function of MovingObject 
		//and takes a generic parameter T which we use to pass in the component we expect to encounter, in this case Player
		protected void OnCantMove <T> (T component)
		{
			//Set the attack trigger of animator to trigger Enemy attack animation.
			animator.SetTrigger ("enemyAttack");

			//Call the RandomizeSfx function of SoundManager passing in the two audio clips to choose randomly between.
			SoundManager.instance.RandomizeSfx (attackSound1, attackSound2);
		}

		public void SetUpMonsterView(Monster monster){
			// 加载怪物头像图片
			ResourceManager.Instance.LoadAssetWithFileName ("battle/monster_icons", () => {

			}, true, monster.agentIconName);

		}
	}
}
