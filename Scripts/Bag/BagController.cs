using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagController : MonoBehaviour {

	public BagView bagView;

	private List<Sprite> mItemIcons = new List<Sprite>();

//	private List<Item> allGameItems = new List<Item>();



	public void OnEnterBagView(){

		#warning ----明天从这里开始----这里测试用，玩家的游戏物品暂时定为从本地读取的所有游戏物品
		if (Player.mainPlayer.allItems.Count == 0) {
			Player.mainPlayer.allItems.AddRange(DataInitializer.LoadDataToModelWithPath<Item> (CommonData.JsonFileDirectoryPath, CommonData.itemsDataFileName));
		}

//		Player.mainPlayer.consumablesEquiped = Player.mainPlayer.allItems;
		 

		// 异步加载物品图片,完成后回调初始化背包界面
		if (mItemIcons.Count == 0) {
			ResourceManager.Instance.LoadSpritesAssetWithFileName ("item/icons", () => {
				
				foreach (Sprite s in ResourceManager.Instance.sprites) {
					mItemIcons.Add (s);
				}

				bagView.SetUpBagView (mItemIcons);

			}, false);
		}


	}
}
