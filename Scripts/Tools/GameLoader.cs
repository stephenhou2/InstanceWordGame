using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoader : MonoBehaviour {

	public GameManager gameManager;
//	private Player mPlayer;
	public Player player;
//	{
//
//		get{ return mPlayer; }
//		set{ mPlayer = value;
//			value.GetComponent<Transform> ().SetParent (this.transform);}
//
//	}
	void Awake(){
		
		ResourceManager.Instance.MaxCachingSpace (200);

		player = Player.mainPlayer;
//
//		if (GameManager.gameManager == null) {
//			Instantiate (gameManager);
//		}

//		DontDestroyOnLoad
	}


}
