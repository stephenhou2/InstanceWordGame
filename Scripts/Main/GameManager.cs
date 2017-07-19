using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;


public class GameManager : SingletonMono<GameManager> {


	public int unlockedMaxChapterIndex = 1;

	private List<Item> mAllItems = new List<Item> ();
	public List<Item> allItems{
		get{
			if (mAllItems.Count == 0) {
				LoadAllItems ();
			}
			return mAllItems;
		}

		set{
			mAllItems = value;
		}

	}

	private List<Sprite> mAllItemsSprites = new List<Sprite>();
	public List<Sprite> allItemSprites{

		get{
			if (mAllItemsSprites.Count == 0) {
				ResourceManager.Instance.LoadAssetWithFileName ("item/icons", () => {
					// 获取所有游戏物品的图片
					for(int i = 0;i<ResourceManager.Instance.sprites.Count;i++){
						mAllItemsSprites.Add(ResourceManager.Instance.sprites[i]);
					}
				},true);
			}

			return mAllItemsSprites;
		}

		set{
			mAllItemsSprites = value;
		}

	}


	public void SetUpHomeView(Player player){

		ResourceManager.Instance.LoadAssetWithFileName ("home/canvas", () => {

			ResourceManager.Instance.gos[0].GetComponent<HomeViewController> ().SetUpHomeView ();

		});
	}

	private void LoadAllItems(){

		MySQLiteHelper sql = MySQLiteHelper.Instance;

		sql.CreatDatabase (CommonData.dataBaseName);

		sql.GetConnectionWith (CommonData.dataBaseName);

		IDataReader reader = sql.ReadFullTable (CommonData.itemsTable);

		while(reader.Read()) {

			int fieldCount = reader.FieldCount;

			Item item = new Item ();

			item.itemId = reader.GetInt16 (0);
			item.itemName = reader.GetString (1);
			item.itemDescription = reader.GetString (2);
			item.spriteName = reader.GetString (3);
			item.itemType = (ItemType)reader.GetInt16 (4);
			item.itemNameInEnglish = reader.GetString (5);
			item.attackGain = reader.GetInt32 (6);
			item.powerGain = reader.GetInt32 (7);
			item.magicGain = reader.GetInt32 (8);
			item.critGain = reader.GetInt32 (9);
			item.amourGain = reader.GetInt32 (10);
			item.magicResistGain = reader.GetInt32 (11);
			item.agilityGain = reader.GetInt32 (12);
			item.healthGain = reader.GetInt32 (13);
			item.strengthGain = reader.GetInt32 (14);

			mAllItems.Add (item);

		}

		sql.CloseConnection (CommonData.dataBaseName);

	}

}
