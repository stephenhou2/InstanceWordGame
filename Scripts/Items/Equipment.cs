using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace WordJourney
{
	/// <summary>
	/// 装备类型枚举
	/// </summary>
	public enum EquipmentType{
		Weapon,//武器
		Cloth,//衣服
		Pants,//裤子
		Helmet,//盔帽
		Shoes,//鞋子
		Jewelry,//项链和护符
		Ring,//戒指
	}

//	/// <summary>
//	/// 装备类型枚举
//	/// </summary>
//	public enum DetailEquipmentType{
//		Sword,//剑
//		Blade,//刀
//		Axe,//斧子
//		Harmer,//锤
//		Staff,//法杖
//		Knife,//匕首
//		Cloth,//袍
//		Armor,//盔甲
//		Pants,//裤子
//		Helmet,//盔帽
//		Shoes,//鞋子
//		Jewelry,//项链和护符
//		Ring,//戒指
//	}


	public class Equipment : Item {

		public double attackGain;//攻击力增益
		public double attackSpeedGain;//攻速增益
		public double critGain;//暴击增益
		public double armorGain;//护甲增益
		public double manaResistGain;//魔抗增益
		public double dodgeGain;//闪避增益
		public double healthGain;//生命增益
		public double manaGain;//魔法增益

		public int maxAttachedProperties;//附加属性最大数量
		public int attachedPropertyId;//附加属性id
		public EquipmentType equipmentType;//装备类型
		public string detailType;//详细装备类型
		public int levelNeed;//装备等级要求

		public List<Material> materials = new List<Material> ();//合成材料需求
		public List<Material> failMaterials = new List<Material>();//合成失败时可能掉落的特殊材料



		public double damagePercentage;//装备损坏程度
		private double fixPercentage = 0.05d;//每次修复百分比

		//装备是否已佩戴
		public bool equiped;


		/// <summary>
		/// 空构造函数，初始化一个0属性的装备
		/// </summary>
		public Equipment(){
			
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		public Equipment(ItemModel itemModel){

			this.itemType = ItemType.Equipment;

			// 初始化物品基本属性
			InitBaseProperties (itemModel);

			// 初始化装备属性
			attackGain = itemModel.attackGain;
			attackSpeedGain = itemModel.attackSpeedGain;
			critGain = itemModel.critGain;
			armorGain = itemModel.armorGain;
			manaResistGain = itemModel.manaResistGain;
			dodgeGain = itemModel.dodgeGain;
			healthGain = itemModel.healthGain;
			manaGain = itemModel.manaGain;

			damagePercentage = 0d;
			equipmentType = itemModel.equipmentType;


		}


		/// <summary>
		/// 获取物品类型字符串
		/// </summary>
		/// <returns>The item type string.</returns>
		public override string GetItemTypeString ()
		{
			string typeString = string.Empty;
			switch (equipmentType) 
			{
			case EquipmentType.Weapon:
				typeString = "类型: 武器";
				break;
			case EquipmentType.Cloth:
				typeString = "类型: 衣服";
				break;
			case EquipmentType.Pants:
				typeString = "类型: 裤子";
				break;
			case EquipmentType.Helmet:
				typeString = "类型: 盔帽";
				break;
			case EquipmentType.Shoes:
				typeString = "类型: 鞋子";
				break;
			case EquipmentType.Jewelry:
				typeString = "类型: 挂饰";
				break;
			case EquipmentType.Ring:
				typeString = "类型: 指环";
				break;
			}
			return typeString;
		}
			

		/// <summary>
		/// 生成单个属性的描述字符串，并加入到属性描述列表中
		/// </summary>
		/// <param name="property">Property.</param>
		/// <param name="gain">Gain.</param>
		/// <param name="propertiesList">Properties list.</param>
		private string GenerateSinglePropertyString(string property,double gain,List<string> propertiesList){

			string propertyStr = string.Empty;

			if (gain >= 1) {
				propertyStr = string.Format ("{0}+ {1}",gain);
			}else{
				propertyStr = string.Format ("{0}+ {1}%",(int)gain*100);
			}

			if (propertiesList != null) {
				propertiesList.Add (propertyStr);
			}

			return propertyStr;
		}

		/// <summary>
		/// 获取物品属性字符串
		/// </summary>
		/// <returns>The item properties string.</returns>
		public override string GetItemBasePropertiesString(){

			StringBuilder itemProperties = new StringBuilder ();

			List<string> propertiesList = new List<string> ();

			if (attackGain > 0) {
				GenerateSinglePropertyString ("攻击", attackGain, propertiesList);
			}
			if (attackSpeedGain > 0) {
				GenerateSinglePropertyString ("攻速", attackSpeedGain, propertiesList);
			}
			if (armorGain > 0) {
				GenerateSinglePropertyString ("护甲", armorGain, propertiesList);
			}
			if (manaResistGain > 0) {
				GenerateSinglePropertyString ("抗性", manaResistGain, propertiesList);
			}
			if (dodgeGain > 0) {
				GenerateSinglePropertyString ("闪避", dodgeGain, propertiesList);
			}
			if (critGain > 0) {
				GenerateSinglePropertyString ("暴击", critGain, propertiesList);
			}
			if (healthGain > 0) {
				GenerateSinglePropertyString ("生命", healthGain, propertiesList);
			}
			if (manaGain > 0) {
				GenerateSinglePropertyString ("魔法", manaGain, propertiesList);
			}


			if (propertiesList.Count > 0) {
				itemProperties.Append (propertiesList [0]);

				for (int i = 1; i < propertiesList.Count; i++) {

					itemProperties.AppendFormat ("\n{0}", propertiesList [i]);

				}

			}

			return itemProperties.ToString ();

		}
			




		/// <summary>
		/// 比较两个装备的给定属性，并返回比较后的字符串
		/// </summary>
		/// <param name="property">比较的属性名称.</param>
		/// <param name="compareValue">新装备属性值</param>
		/// <param name="comparedValue">原装备属性值</param>
		/// <param name="compareList">存储比较字符串的列表</param>
		private string CompareItemsProperty(string property, double compareValue,double comparedValue,List<string> compareList){

			// 获得单个属性描述
			string propertyString = GenerateSinglePropertyString (property, compareValue, null);

			// 该项属性数值对比 
			double compare = compareValue - comparedValue;

			// 比较后根据属性增减 决定连接符号用"-"还是"+"
			string linkSymbol = compare < 0 ? "-" : "+";

			// 比较后根据属性增加决定字体颜色
			string colorText = compare < 0 ? "red" : "green";

			// 比较后的描述字符串
			string compareString = string.Empty;

			if (compare >= 1) {
				compareString = string.Format ("{0}(<color={1}>{2}{3}</color>)",propertyString, colorText,linkSymbol,compare);
			} else if (compare > 0 && compare < 1) {
				compareString = string.Format ("{0}(<color={1}>{2}{3}%</color>)",propertyString, colorText,linkSymbol,(int)(compare * 100));
			} else if (compare < 0 && compare > -1) {
				compareString = string.Format ("{0}(<color={1}>{2}{3}%</color>)",propertyString, colorText,linkSymbol,(int)(-compare * 100));
			} else if (compare <= -1) {
				compareString = string.Format ("{0}(<color={1}>{2}{3}</color>)",propertyString, colorText,linkSymbol,-compare);
			}

			if (compareList != null) {
				compareList.Add (compareString);
			}

			return compareString;

		}
			

		/// <summary>
		/// 获取两件装备的对比字符串
		/// </summary>
		/// <returns>The compare properties string with item.</returns>
		/// <param name="compareEquipment">Compare equipment.</param>
		public string GetComparePropertiesStringWithItem(Equipment compareEquipment){

			StringBuilder itemProperties = new StringBuilder ();

			List<string> comparesList = new List<string> ();

			if (attackGain > 0) {
				CompareItemsProperty ("攻击", attackGain, compareEquipment.attackGain, comparesList);
			}
			if (attackSpeedGain > 0) {
				CompareItemsProperty ("攻速", attackSpeedGain, compareEquipment.attackSpeedGain, comparesList);
			}
			if (critGain > 0) {
				CompareItemsProperty ("暴击", critGain, compareEquipment.critGain, comparesList);
			}
			if (armorGain > 0) {
				CompareItemsProperty ("护甲", armorGain, compareEquipment.armorGain, comparesList);
			}
			if (manaResistGain > 0) {
				CompareItemsProperty ("抗性", manaResistGain, compareEquipment.manaResistGain, comparesList);
			}
			if (dodgeGain > 0) {
				CompareItemsProperty ("闪避", dodgeGain, compareEquipment.dodgeGain, comparesList);
			} 
			if (healthGain > 0) {
				CompareItemsProperty ("生命", healthGain, compareEquipment.healthGain, comparesList);
			}
			if (manaGain > 0) {
				CompareItemsProperty ("魔法", manaGain, compareEquipment.manaGain, comparesList);
			}

			if (comparesList.Count > 0) {

				itemProperties.Append (comparesList [0]);

				for (int i = 1; i < comparesList.Count; i++) {

					itemProperties.AppendFormat ("\n{0}", comparesList [i]);

				}

			}

			return itemProperties.ToString ();

		}

		/// <summary>
		/// Fixs the equipment.
		/// </summary>
		public void FixEquipment(){
			damagePercentage -= fixPercentage;
			if (damagePercentage < 0) {
				damagePercentage = 0;
			}
		}

	}
}
