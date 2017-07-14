using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeView : MonoBehaviour {


	public Text playerLevelText;
	public Slider playerHealthBar;

	public void SetUpHomeView(){

		SetUpTopBar ();

	}

	// 初始化顶部bar
	private void SetUpTopBar(){

		Player player = Player.mainPlayer;

		playerLevelText.text = player.agentLevel.ToString();

		playerHealthBar.maxValue = player.maxHealth;
		playerHealthBar.value = player.health;
		playerHealthBar.transform.FindChild ("HealthText").GetComponent<Text> ().text = player.health + "/" + Player.mainPlayer.maxHealth;

	}
}
