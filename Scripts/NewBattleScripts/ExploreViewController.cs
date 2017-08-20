using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExploreViewController : MonoBehaviour {




	public Transform rewardItemsPlain;

	// 图片遮罩
	public Image maskImage;


	// 所有转场间隔时间(转场时背景全黑)
	public float TransitionDelay = 0.5f;

	public void SetUpExploreView(){

		Invoke("HideLevelImage", TransitionDelay);

	}

	//Hides black image used between levels
	private void HideLevelImage()
	{
		maskImage.enabled = false;
	}

}
