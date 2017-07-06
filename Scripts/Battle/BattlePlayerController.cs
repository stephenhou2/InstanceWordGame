using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePlayerController : MonoBehaviour {

	[HideInInspector]public Player player;

	private BattlePlayerView mBaPlayerView;

	private List<Sprite> skillIcons = new List<Sprite>();

	// 角色UIView
	public BattlePlayerView baView{

		get{
			if (mBaPlayerView == null) {
				mBaPlayerView = GetComponent<BattlePlayerView> ();
			}
			return mBaPlayerView;
		}

	}


	public void SetUpBattlePlayerView(){
		
		if (skillIcons.Count != 0) {
			baView.SetUpUI (player,skillIcons);
			return;
		}

		ResourceManager.Instance.LoadSpritesAssetWithFileName("skills/skills", () => {
			skillIcons = ResourceManager.Instance.sprites;
			baView.SetUpUI (player,skillIcons);
		},true);

	}
}
