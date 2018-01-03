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

		public int itemCount;

		public bool isNewItem = true;


		/// <summary>
		/// 空构造函数
		/// </summary>
		public Item(){ 
		
		}

		public static Item NewItemWith(Item item,int itemCount){
			ItemModel itemModel = null;
			Item newItem = null;
			switch (item.itemType) {
			case ItemType.Equipment:
				newItem = new Equipment (item as Equipment);
				break;
			case ItemType.Consumables:
				newItem = new Consumables (item as Consumables, itemCount);
				break;

			}
			return newItem;
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
					
				if (itemModel == null) {
					string error = string.Format ("未找到id为{0}的物品", itemId);
					Debug.LogError (error);
				}

				switch (itemModel.itemType) {
				case ItemType.Equipment:
					newItem = new Equipment (itemModel);
					break;
				case ItemType.Consumables:
					newItem = new Consumables (itemModel, 1);
					break;
				}
			} else if (itemId < 3000) {
				itemModel = GameManager.Instance.gameDataCenter.allItemModels.Find (delegate (ItemModel item) {
					return item.itemId == itemId - 2000;
				});
				newItem = new Formula (FormulaType.Equipment, itemId - 2000);
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
					Equipment equipment = new Equipment (itemModel);
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
//		public abstract string GetItemBasePropertiesString ();


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
		
		public int itemId;
		public string itemName;
		public string itemNameInEnglish;
		public string itemDescription;
		public string spriteName;
		public ItemType itemType;

		public float healthGain;//血量增益
		public float manaGain;//魔法增益
		public float attackGain;//攻击力增益
		public float hitGain;//命中增益
		public float armorGain;//护甲增益
		public float magicResistGain;//魔抗增益
		public float dodgeGain;//闪避增益
		public float critGain;//暴击增益
		public float wholePropertyGain;//全属性增益

		public float physicalHurtScalerGain;//物理伤害增益
		public float magicalHurtScalerGain;//魔法伤害增益
		public float critHurtScalerGain;//暴击倍率增益


		public SkillInfo[] attachedSkillInfos;

		public int[] itemIdsForProduce;


		public override string ToString ()
		{
			return string.Format ("[ItemModel]:\n itemId:{0},itemName:{1},itemNameInEnglish:{2},itemSpriteName:{3}" +
				"itemDescription:{4},healthGain:{5},manaGain:{6},attackGain:{7},hitGain:{8},armorGain:{9},magicResistGain:{10}" +
				"dodgeGain:{11},critGain:{12},wholePropertyGain:{13},physicalHurtScalerGain:{14},magicalHurtScalerGain:{15},critHurtScalerGain:{16}," +
				"attachedSkillInfosCount:{17},itemForProduceCount:{18}",
				itemId,itemName,itemNameInEnglish,spriteName,itemDescription,healthGain,manaGain,attackGain,hitGain,armorGain,magicResistGain,
				dodgeGain,critGain,wholePropertyGain,physicalHurtScalerGain,magicalHurtScalerGain,critHurtScalerGain,attachedSkillInfos.Length,itemIdsForProduce.Length);
		}

	}


}
