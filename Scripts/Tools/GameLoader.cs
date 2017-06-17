using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoader : MonoBehaviour {

	public GameManager gameManager;
	public Player player;
	void Awake(){

		if (Player.mainPlayer == null) {
			Instantiate (player);
			Player.mainPlayer.ResetBattleAgentProperties (true);
		}

		if (GameManager.gameManager == null) {
			Instantiate (gameManager);
		}
	}


}
