using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoader : MonoBehaviour {


	void Awake(){
		
		ResourceManager.Instance.MaxCachingSpace (200);

		GameManager.Instance.SetUpHomeView (Player.mainPlayer);

		DontDestroyOnLoad (Player.mainPlayer);

		DontDestroyOnLoad (GameManager.Instance);

	}


}
