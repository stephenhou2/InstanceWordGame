using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : SingletonMono<GameManager> {


	public int unlockedMaxChapterIndex = 1;

	public void SetUpHomeView(Player player){

		ResourceManager.Instance.LoadAssetWithFileName ("home/canvas", () => {

			ResourceManager.Instance.gos[0].GetComponent<HomeViewController> ().SetUpHomeView ();

		});
	}

}
