using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Data;

namespace WordJourney{
	

	public abstract class Item {

		public string itemName;
		public string itemDescription;
		public string spriteName;

		public string itemNameInEnglish;
		public int itemId;
		public int itemCount;

		public ItemType itemType;

		public ItemQuality itemQuality;

		public int strengthenTimes;


	

		protected int[] propertiesArray;


//		public int attackGain;//攻击力增益
//		public int attackSpeedGain;//攻速增益
//		public int critGain;//暴击增益
//		public int armourGain;//护甲增益
//		public int manaResistGain;//魔抗增益
//		public int dodgeGain;//闪避增益
//
//
//		public int healthGain;//血量增益
//		public int manaGain;//魔法增益


		public static List<Equipment> GetAllEquipments(){

			List<ItemModel> allItemModels = GameManager.Instance.allItemModels;

			List<Equipment> allEquipment = new List<Equipment> ();

			for (int i = 0; i < allItemModels.Count; i++) {

				ItemModel itemModel = allItemModels [i];

				if (itemModel.itemType == ItemType.Equipment) {
					Equipment equipment = new Equipment (itemModel);
					allEquipment.Add (equipment);
				}
					
			}
			return allEquipment;
		}

//		public static Item CreateInscription(string englishName){
//
//			MySQLiteHelper sql = MySQLiteHelper.Instance;
//
//			sql.GetConnectionWith (CommonData.dataBaseName);
//
//			IDataReader reader =  sql.ReadSpecificRowsAndColsOfTable ("AllWordsData", "*",
//				new string[]{ string.Format ("Spell='{0}'", englishName)},
//				true);
//
//			if (!reader.Read ()) {
//				Debug.Log ("不存在");
//				return null;
//			}
//
//			bool valid = reader.GetBoolean (4);
//
//			if (!valid) {
//				Debug.Log ("已使用");
//				return null;
//			}
//
//			int id = reader.GetInt32 (0);
//
//			string explaination = reader.GetString (2);
//
//
//			Item inscription = new Item ();
//
//			inscription.itemType = ItemType.Inscription;
//
//			string inscriptionName = explaination.Split (new string[]{ ".", "，" }, System.StringSplitOptions.RemoveEmptyEntries)[1];
//
//			inscription.itemName = string.Format ("{0}之石", inscriptionName);
//
//			inscription.RandomProperties ();
//
//			sql.UpdateSpecificColsWithValues ("AllWordsData",
//				new string[]{ "Valid" },
//				new string[]{ "0" },
//				new string[]{ string.Format("Id={0}",id) },
//				true);
//
//			sql.CloseConnection (CommonData.dataBaseName);
//
//			return inscription;
//		}

		public Item(){
		}

		public static Item NewItemWithName(string itemNameInEnglish){

			ItemModel itemModel = GameManager.Instance.allItemModels.Find(delegate (ItemModel item){
				return item.itemNameInEnglish == itemNameInEnglish;
			});

			Item newItem = null;

			switch (itemModel.itemType) {
			case ItemType.Equipment:
				newItem = new Equipment (itemModel);
				break;
			case ItemType.Consumables:
				newItem = new Consumables (itemModel);
				break;
			case ItemType.Inscription:
				newItem = new Inscription (itemModel);
				break;
			case ItemType.Task:
				newItem = new TaskItem(itemModel);
				break;
			default:
				break;
			}

			return newItem;
		}
			


		public abstract string GetItemTypeString ();

		public abstract string GetItemPropertiesString ();

		public abstract string GetItemQualityString ();


		public bool CheckCanStrengthen(){

			return itemType == ItemType.Equipment;

		}


		public override string ToString ()
		{
			return string.Format ("[Item]:" + itemName + "[\nItemDesc]:" + itemDescription);
		}


	}

	[System.Serializable]
	public class ItemModel{

		public string itemName;
		public string itemDescription;
		public string spriteName;

		public ItemType itemType;


		public string itemNameInEnglish;
		public int itemId;


		public int attackGain;//攻击力增益
		public int attackSpeedGain;//攻速增益
		public int critGain;//暴击增益
		public int armourGain;//护甲增益
		public int manaResistGain;//魔抗增益
		public int dodgeGain;//闪避增益


		public int healthGain;//血量增益
		public int manaGain;//魔法增益

		public EquipmentType equipmentType; //装备类型

	}


}
