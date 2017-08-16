using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace WordJourney
{
public class GameLoader : MonoBehaviour {


	void Awake(){


		DontDestroyOnLoad (Player.mainPlayer);

		DontDestroyOnLoad (GameManager.Instance);

	}


}
}
