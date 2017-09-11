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


		// 物品属性数组
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

		/// <summary>
		/// 空构造函数
		/// </summary>
		public Item(){ 
		
		}


		/// <summary>
		/// 通过物品英文名称初始化物品
		/// </summary>
		/// <returns>The item with name.</returns>
		/// <param name="itemNameInEnglish">Item name in english.</param>
		public static Item NewItemWithName(string itemNameInEnglish){

			ItemModel itemModel = GameManager.Instance.allItemModels.Find(delegate (ItemModel item){
				return item.itemNameInEnglish == itemNameInEnglish;
			});

			Item newItem = null;

			switch (itemModel.itemType) {
			case ItemType.Equipment:
				newItem = new Equipment (itemModel,ItemQuality.Random);
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




		/// <summary>
		/// 初始化基础属性
		/// </summary>
		/// <param name="itemModel">Item model.</param>
		protected void InitBaseProperties(ItemModel itemModel){

			itemId = itemModel.itemId;
			itemName = itemModel.itemName;
			itemDescription = itemModel.itemDescription;
			spriteName = itemModel.spriteName;
			itemType = itemModel.itemType;
			itemNameInEnglish = itemModel.itemNameInEnglish;

		}

		/// <summary>
		/// 获取所有游戏物品中的武器类物品
		/// </summary>
		public static List<Equipment> GetAllEquipments(){

			List<ItemModel> allItemModels = GameManager.Instance.allItemModels;

			List<Equipment> allEquipment = new List<Equipment> ();

			for (int i = 0; i < allItemModels.Count; i++) {

				ItemModel itemModel = allItemModels [i];

				if (itemModel.itemType == ItemType.Equipment) {
					Equipment equipment = new Equipment (itemModel,ItemQuality.Random);
					allEquipment.Add (equipment);
				}
					
			}
			return allEquipment;
		}
			
		/// <summary>
		/// 物品是否可以进行强化
		/// </summary>
		/// <returns><c>true</c>, if can strengthen was checked, <c>false</c> otherwise.</returns>
		public bool CheckCanStrengthen(){

			return itemType == ItemType.Equipment;

		}


		/// <summary>
		/// 获取物品类型字符串
		/// </summary>
		/// <returns>The item type string.</returns>
		public abstract string GetItemTypeString ();

		/// <summary>
		/// 获取物品属性字符串
		/// </summary>
		/// <returns>The item properties string.</returns>
		public abstract string GetItemPropertiesString ();

		/// <summary>
		/// 获取物品品质字符串
		/// </summary>
		/// <returns>The item quality string.</returns>
		public abstract string GetItemQualityString ();


		public override string ToString ()
		{
			return string.Format ("[Item]:" + itemName + "[\nItemDesc]:" + itemDescription);
		}

	}


	/// <summary>
	/// 物品模型类
	/// </summary>
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
