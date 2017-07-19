using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePlayerController : MonoBehaviour {

	[HideInInspector]public Player player;

	private BattlePlayerView mBaPlayerView;

	private List<Sprite> skillIcons = new List<Sprite>();

//	private List<Item> consumables = new List<Item> ();

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

//		for(int i = 3;i<player.allEquipedItems.Count;i++){
//			Item consumable = player.allEquipedItems [i];
//			consumables.Add (consumable);
//		}

		List<Sprite> allItemSprites = GameManager.Instance.allItemSprites;

		if (skillIcons.Count != 0) {
			baView.SetUpUI (player,skillIcons,allItemSprites);
			return;
		}

		ResourceManager.Instance.LoadSpritesAssetWithFileName("skills/skills", () => {
			skillIcons = ResourceManager.Instance.sprites;
			baView.SetUpUI (player,skillIcons,allItemSprites);
//			baView.update
		},true);

	}


	public void OnPlayerUseItem(int itemIndex){



		Item item = player.allEquipedItems[itemIndex + 3];

		item.itemCount--;


		if (item.itemCount <= 0) {
			player.allEquipedItems [itemIndex + 3] = new Item ();
			player.allItems.Remove (item);
			baView.UpdateItemButtonsStatus (player);
		}



		if (item.healthGain != 0 && item.strengthGain != 0) {
			player.health += item.healthGain;
			player.strength += item.strengthGain;
			baView.UpdateHealthBarAnim (player);
			baView.UpdateStrengthBarAnim (player);

			string hurtText = "<color=green>+" + item.healthGain.ToString() + "体力</color>" 
				+ "\t\t\t\t\t" 
				+ "<color=orange>+" + item.strengthGain.ToString() + "气力</color>";
			baView.PlayHurtHUDAnim(hurtText);

		}else if (item.healthGain != 0) {
			player.health += item.healthGain;
			baView.UpdateHealthBarAnim (player);
			string hurtText = "<color=green>+" + item.healthGain.ToString() + "体力</color>";
			baView.PlayHurtHUDAnim(hurtText);

		}else if (item.strengthGain != 0) {
			player.strength += item.strengthGain;
			baView.UpdateStrengthBarAnim (player);
			string hurtText = "<color=orange>+" + item.strengthGain.ToString() + "气力</color>";
			baView.PlayHurtHUDAnim(hurtText);
		}

		baView.UpdateItemButtonsStatus (player);

	}

}
