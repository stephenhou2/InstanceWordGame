using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : BattleAgent {

	// 玩家角色单例
	public static Player mainPlayer;

	public override void Awake(){

		base.Awake ();

		if (mainPlayer == null) {
			mainPlayer = this;
		} else if (mainPlayer != this) {
			Destroy (gameObject);
		}

//		DontDestroyOnLoad (gameObject);

//		mainPlayer.originalMaxHealth = 100;
//		mainPlayer.originalMaxStrength = 10;
//		mainPlayer.originalHealth = 100;
//		mainPlayer.originalStrength = 10;
//		mainPlayer.originalAttack = 1;
//		mainPlayer.originalPower = 1;
//		mainPlayer.originalMagic = 1;
//		mainPlayer.originalCrit = 1;
//		mainPlayer.originalAmour = 1;
//		mainPlayer.originalMagicResist = 1;
//		mainPlayer.originalAgility = 1;

	}

//	public static Player GetPlayer(){
//		Player player = mainPlayer;
//		player.ResetBattleAgentProperties (false);
//		return player;
//	}

}
