using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Data;

namespace WordJourney{
	

	public enum ItemType{
		Equipment,
		Consumables,
		Task,
		FuseStone,
		Material,
		Formula,
		CharacterFragment

	}

	[System.Serializable]
	public abstract class Item {

		public string itemName;
		public string itemDescription;
		public string spriteName;
		public string itemNameInEnglish;
		public int itemId;
		public ItemType itemType;

//		public int levelRequired;

		public int itemCount;

		// 物品属性数组
//		protected double[] propertiesArray;


//		public int attackGain;//攻击力增益
//		public int attackSpeedGain;//攻速增益
//		public int critGain;//暴击增益
//		public int armorGain;//护甲增益
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
		/// 通过物品id和数量初始化物品
		/// </summary>
		public static Item NewItemWith(int itemId,int itemCount){
			
			ItemModel itemModel = null;
			Item newItem = null;

			if (itemId < 1000) {
				itemModel = GameManager.Instance.gameDataCenter.allItemModels.Find (delegate (ItemModel item) {
					return item.itemId == itemId;
				});
				switch (itemModel.itemType) {
				case ItemType.Equipment:
					newItem = new Equipment (itemModel, 0);
					break;
				case ItemType.Consumables:
					newItem = new Consumables (itemModel, 1);
					break;
				}
			}else if (itemId < 2000) {
				Material material = GameManager.Instance.gameDataCenter.allMaterials.Find (delegate(Material obj) {
					return obj.itemId == itemId;
				});

				newItem = new Material (material, itemCount);
			} else if (itemId < 3000) {
				itemModel = GameManager.Instance.gameDataCenter.allItemModels.Find (delegate (ItemModel item) {
					return item.itemId == itemId - 2000;
				});
				newItem = new Formula (FormulaType.Equipment, itemId - 2000);
			} else if (itemId < 3100) {
				newItem = new Formula (FormulaType.Skill, itemId - 3000);
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

			List<ItemModel> allItemModels = GameManager.Instance.gameDataCenter.allItemModels;

			List<Equipment> allEquipment = new List<Equipment> ();

			for (int i = 0; i < allItemModels.Count; i++) {

				ItemModel itemModel = allItemModels [i];

				if (itemModel.itemType == ItemType.Equipment) {
					#warning 这里耐久度都设为0
					Equipment equipment = new Equipment (itemModel,0);
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
		public abstract string GetItemBasePropertiesString ();


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
		public string itemNameInEnglish;
		public string itemDescription;
		public string spriteName;
		public ItemType itemType;
		public int itemId;

		public float attackGain;//攻击力增益
		public float attackSpeedGain;//攻速增益
		public float critGain;//暴击增益
		public float armorGain;//护甲增益
		public float manaResistGain;//魔抗增益
		public float dodgeGain;//闪避增益
		public float healthGain;//血量增益
		public float manaGain;//魔法增益
		public float physicalHurtScaler;//物理攻击增益
		public float magicHurtScaler;//魔法攻击增益
		public int effectDuration;//效果持续时间

		public int maxAttachedPropertyCount;//附加属性最大数量
		public int attachedPropertyId;//一定有的附加属性id
		public EquipmentType equipmentType;//装备类型
		public ConsumablesType consumbalesType;//消耗品类型
		public string detailType;//详细装备类型

		public List<Material> materials = new List<Material> ();
		public List<Material> failMaterials = new List<Material>();

		public bool formulaUnlocked;
	}


}
