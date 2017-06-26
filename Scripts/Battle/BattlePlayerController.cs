using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePlayerController : MonoBehaviour {

	[HideInInspector]public Player player;

	private BattlePlayerView mBaPlayerView;

	// 角色UIView
	public new BattlePlayerView baView{

		get{
			if (mBaPlayerView == null) {
				mBaPlayerView = GetComponent<BattlePlayerView> ();
			}
			return mBaPlayerView;
		}

	}


}
