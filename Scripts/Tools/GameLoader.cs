using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoader : MonoBehaviour {


	void Awake(){
		


//		GameManager.Instance.SetUpHomeView (Player.mainPlayer);

		DontDestroyOnLoad (Player.mainPlayer);

		DontDestroyOnLoad (GameManager.Instance);

	}


}
