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
		Armour,//护甲
		Helmet,//头盔
		Shield,//盾
		Shoes,//鞋
		Ring,//饰品
	}



	public class Equipment : Item {

		public int attackGain;//攻击力增益
		public int attackSpeedGain;//攻速增益
		public int critGain;//暴击增益
		public int armourGain;//护甲增益
		public int manaResistGain;//魔抗增益
		public int dodgeGain;//闪避增益

		//装备品质
		public ItemQuality itemQuality;

		//装备的潜在属性字符串（装备根据品质不同属性上有差异）
		public string potentialPropertiesString;

		//装备已强化次数
		public int strengthenTimes;

		//装备是否已佩戴
		public bool equiped;

		//装备类型
		public EquipmentType equipmentType;


		//装备根据品质不同属性变化范围
		private int minGain = -3;
		private int maxGain = 8;


		/// <summary>
		/// 空构造函数，初始化一个0属性的装备
		/// </summary>
		public Equipment(){
			
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="itemModel">Item model.</param>
		/// </param> 装备品质［if＝＝ItemQuality.Random，则装备属性随机］</param>
		public Equipment(ItemModel itemModel,ItemQuality iq){

			this.itemType = ItemType.Equipment;

			// 初始化物品基本属性
			InitBaseProperties (itemModel);

			// 初始化装备属性
			attackGain = itemModel.attackGain;
			attackSpeedGain = itemModel.attackSpeedGain;
			critGain = itemModel.critGain;
			armourGain = itemModel.armourGain;
			manaResistGain = itemModel.manaResistGain;
			dodgeGain = itemModel.dodgeGain;


			equipmentType = itemModel.equipmentType;

			// 获取装备潜在属性范围字符串
			this.potentialPropertiesString = GetItemPotentialPropertiesString ();

			// 确定装备品质
			if (iq == ItemQuality.Random) {
				RandomQuility ();
			} else {
				this.itemQuality = iq;
			}

			// 根据装备品质在基础属性上重新设定装备属性
			ResetBasePropertiesByQuality ();

		}


		/// <summary>
		/// 获取物品类型字符串
		/// </summary>
		/// <returns>The item type string.</returns>
		public override string GetItemTypeString ()
		{
			switch (equipmentType) {
				case EquipmentType.Weapon:
					return "类型: 武器";
				case EquipmentType.Armour:
					return "类型: 防具";
				case EquipmentType.Shoes:
					return "类型: 鞋子";
				default:
					return string.Empty;
			}
		}

		/// <summary>
		/// 随机装备品质
		/// </summary>
		protected void RandomQuility(){

			float seed = Random.Range (0f, 100f);
			if (seed >= 0 && seed < 50f) {
				itemQuality = ItemQuality.C;
			} else if (seed >= 50f && seed < 80f) {
				itemQuality = ItemQuality.B;
			} else if (seed >= 80f && seed < 95f) {
				itemQuality = ItemQuality.A;
			} else {
				itemQuality = ItemQuality.S;
			}

		}


		/// <summary>
		/// 根据装备品质重设装备初始属性
		/// </summary>
		protected void ResetBasePropertiesByQuality(){

			switch (itemQuality) {
			case ItemQuality.C:
				UpdateProperties (-3, 1);
				break;
			case ItemQuality.B:
				UpdateProperties (-2, 3);
				break;
			case ItemQuality.A:
				UpdateProperties (-1, 5);
				break;
			case ItemQuality.S:
				UpdateProperties (1, 8);
				break;

			}

		}

		/// <summary>
		/// 强化时更新装备属性
		/// </summary>
		/// <param name="minGain">Minimum gain.</param>
		/// <param name="maxGain">Max gain.</param>
		private void UpdateProperties(int minGain,int maxGain){

			if (attackGain > 0) {
				attackGain += Random.Range (minGain, maxGain);
			}
			if (attackSpeedGain > 0) {
				attackSpeedGain += Random.Range (minGain, maxGain);
			}
			if (critGain > 0) {
				critGain += Random.Range (minGain, maxGain);
			}
			if (armourGain > 0) {
				armourGain += Random.Range (minGain, maxGain);
			}
			if (manaResistGain > 0) {
				manaResistGain += Random.Range (minGain, maxGain);
			}
			if (dodgeGain > 0) {
				dodgeGain += Random.Range (minGain, maxGain);
			}
		}


		/// <summary>
		/// 获取装备潜在属性范围字符串
		/// </summary>
		private string GetItemPotentialPropertiesString(){

			StringBuilder itemProperties = new StringBuilder ();

			List<string> propertiesList = new List<string> ();

			if (attackGain > 0) {
				string str = string.Format ("攻击: {0}~{1}", attackGain + minGain, attackGain + maxGain);
				propertiesList.Add (str);
			}
			if (attackSpeedGain > 0) {
				string str = string.Format ("攻速: {0}~{1}", attackSpeedGain + minGain, attackSpeedGain + maxGain);
				propertiesList.Add (str);
			}
			if (critGain > 0) {
				string str = string.Format ("暴击: {0}~{1}", critGain + minGain, critGain + maxGain);
				propertiesList.Add (str);
			}
			if (armourGain > 0) {
				string str = string.Format ("护甲: {0}~{1}", armourGain + minGain, armourGain + maxGain);
				propertiesList.Add (str);
			}
			if (manaResistGain > 0) {
				string str = string.Format ("抗性: {0}~{1}", manaResistGain + minGain, manaResistGain + maxGain);
				propertiesList.Add (str);
			}
			if (dodgeGain > 0) {
				string str = string.Format ("闪避: {0}~{1}", dodgeGain + minGain, dodgeGain + maxGain);
				propertiesList.Add (str);
			} 

//			if (healthGain > 0) {
//				string str = string.Format ("体力+{0}",healthGain);
//				propertiesList.Add (str);
//			}
//			if (manaGain > 0) {
//				string str = string.Format ("魔法+{0}",manaGain);
//				propertiesList.Add (str);
//			}

			if (propertiesList.Count > 0) {
				itemProperties.Append (propertiesList [0]);

				for (int i = 1; i < propertiesList.Count; i++) {

					itemProperties.AppendFormat ("\n{0}", propertiesList [i]);

				}

			}

			return itemProperties.ToString ();

		}

		/// <summary>
		/// 获取物品属性字符串
		/// </summary>
		/// <returns>The item properties string.</returns>
		public override string GetItemPropertiesString(){

			StringBuilder itemProperties = new StringBuilder ();

			List<string> propertiesList = new List<string> ();

			if (attackGain > 0) {
				string str = string.Format ("攻击: {0}", attackGain);
				propertiesList.Add (str);
			}
			if (attackSpeedGain > 0) {
				string str = string.Format ("攻速: {0}", attackSpeedGain);
				propertiesList.Add (str);
			}
			if (critGain > 0) {
				string str = string.Format ("暴击: {0}", critGain);
				propertiesList.Add (str);
			}
			if (armourGain > 0) {
				string str = string.Format ("护甲: {0}", armourGain);
				propertiesList.Add (str);
			}
			if (manaResistGain > 0) {
				string str = string.Format ("抗性: {0}", manaResistGain);
				propertiesList.Add (str);
			}
			if (dodgeGain > 0) {
				string str = string.Format ("闪避: {0}", dodgeGain);
				propertiesList.Add (str);
			} 

//			if (healthGain > 0) {
//				string str = string.Format ("体力+{0}",healthGain);
//				propertiesList.Add (str);
//			}
//			if (manaGain > 0) {
//				string str = string.Format ("魔法+{0}",manaGain);
//				propertiesList.Add (str);
//			}

			if (propertiesList.Count > 0) {
				itemProperties.Append (propertiesList [0]);

				for (int i = 1; i < propertiesList.Count; i++) {

					itemProperties.AppendFormat ("\n{0}", propertiesList [i]);

				}

			}

			return itemProperties.ToString ();

		}

		/// <summary>
		/// 获取物品品质字符串
		/// </summary>
		/// <returns>The item quality string.</returns>
		public override string GetItemQualityString(){

			string itemQualityStr = string.Empty;

			if (itemQuality == ItemQuality.S) {
				itemQualityStr = "<color=orange>品级: " + itemQuality.ToString () + "</color>";
			} else {
				itemQualityStr = "品级: " + itemQuality.ToString () + "级";
			}

			return itemQualityStr;

		}


		/// <summary>
		/// 比较两个装备的给定属性，并返回比较后的字符串
		/// </summary>
		/// <param name="propertyValue0">Property value0.</param>
		/// <param name="propertyValue1">Property value1.</param>
		/// <param name="compare">Compare.</param>
		/// <param name="linkSymbol">Link symbol.</param>
		/// <param name="colorText">Color text.</param>
		private void CompareItemsProperty(int propertyValue0,int propertyValue1,out int compare,out string linkSymbol,out string colorText){

			compare = propertyValue0 - propertyValue1;

			linkSymbol = compare < 0 ? "-" : "+";

			colorText = compare < 0 ? "<color=red>" : "<color=green>";
		}

		/// <summary>
		/// 获取两件装备的对比字符串
		/// </summary>
		/// <returns>The compare properties string with item.</returns>
		/// <param name="compareEquipment">Compare equipment.</param>
		public string GetComparePropertiesStringWithItem(Equipment compareEquipment){

			StringBuilder itemProperties = new StringBuilder ();

			List<string> propertiesList = new List<string> ();

			int compare = 0;
			string linkSymbol = string.Empty;
			string colorText = string.Empty;

			if (attackGain > 0) {

				CompareItemsProperty (attackGain, compareEquipment.attackGain,out compare,out linkSymbol,out colorText);

				string str = string.Format ("攻击: {0}({1}{2}{3}</color>)", attackGain,colorText,linkSymbol,Mathf.Abs(compare));

				propertiesList.Add (str);
			}
			if (attackSpeedGain > 0) {

				CompareItemsProperty (attackSpeedGain, compareEquipment.attackSpeedGain,out compare,out linkSymbol,out colorText);

				string str = string.Format ("攻速: {0}({1}{2}{3}</color>)", attackSpeedGain,colorText,linkSymbol,Mathf.Abs(compare));

				propertiesList.Add (str);
			}
			if (critGain > 0) {

				CompareItemsProperty (critGain, compareEquipment.critGain,out compare,out linkSymbol,out colorText);

				string str = string.Format ("暴击: {0}({1}{2}{3}</color>)", critGain,colorText,linkSymbol,Mathf.Abs(compare));

				propertiesList.Add (str);
			}
			if (armourGain > 0) {

				CompareItemsProperty (armourGain, compareEquipment.armourGain,out compare,out linkSymbol,out colorText);

				string str = string.Format ("护甲: {0}({1}{2}{3}</color>)", armourGain,colorText,linkSymbol,Mathf.Abs(compare));

				propertiesList.Add (str);
			}
			if (manaResistGain > 0) {

				CompareItemsProperty (manaResistGain, compareEquipment.manaResistGain,out compare,out linkSymbol,out colorText);

				string str = string.Format ("抗性: {0}({1}{2}{3}</color>)", manaResistGain,colorText,linkSymbol,Mathf.Abs(compare));

				propertiesList.Add (str);
			}
			if (dodgeGain > 0) {

				CompareItemsProperty (dodgeGain, compareEquipment.dodgeGain,out compare,out linkSymbol,out colorText);

				string str = string.Format ("闪避: {0}({1}{2}{3}</color>)", dodgeGain,colorText,linkSymbol,Mathf.Abs(compare));

				propertiesList.Add (str);
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
		/// Chances the of gain.
		/// </summary>
		/// <returns>The of gain.</returns>
		/// <param name="chanceArray">Chance array.</param>
		/// <param name="gain">Gain.</param>
		protected float chanceOfGain(float[] chanceArray,int gain){
			float totalChance = 0f;
			for (int i = 0; i < gain; i++) {
				totalChance += chanceArray [i];
			}
			return totalChance;
		}



		/// <summary>
		/// 强化物品
		/// </summary>
		public string StrengthenItem(){

			// 增加不同属性点的概率数组
			float[] chanceArray = null;

			switch (itemQuality) {
			case ItemQuality.C:
				chanceArray = new float[]{ 80f, 15f, 4f, 1f };
				break;
			case ItemQuality.B:
				chanceArray = new float[]{ 60f, 25f, 12f, 3f };
				break;
			case ItemQuality.A:
				chanceArray = new float[]{ 50f, 30f, 15f, 5f };
				break;
			case ItemQuality.S:
				chanceArray = new float[]{ 30f, 30f, 30f, 10f };
				break;
			}

			return StrengthenPropertyByQuality (chanceArray);


		}


		/// <summary>
		/// 根据品质得到的属性增长概率数组，强化物品
		/// </summary>
		/// <returns>物品属性字符串</returns>
		/// <param name="chanceArray">属性［＋1的概率，＋2的概率，＋3的概率，＋4的概率］</param>
		private string StrengthenPropertyByQuality(float[] chanceArray){

			int propertyGain = 0;
			string strengthenGainStr = string.Empty;

			if (chanceOfGain(chanceArray,4) != 100f) {
				Debug.Log("概率和不等于1");
				propertyGain = 0;
			}

			int seed = Random.Range (0, 100);
			if (seed >= 0 && seed < chanceOfGain(chanceArray,1)) {
				propertyGain = 1;
			} else if (seed >= chanceOfGain(chanceArray,1) && seed < chanceOfGain(chanceArray,2)) {
				propertyGain = 2;
			} else if (seed >= chanceOfGain(chanceArray,2) && seed < chanceOfGain(chanceArray,3)) {
				propertyGain = 3;
			} else {
				propertyGain = 4;
			}

			if (propertiesArray == null) {

				propertiesArray = new int[]{ attackGain, attackSpeedGain, armourGain, manaResistGain, critGain, dodgeGain };

			}

			int propertyIndex = Random.Range(0,propertiesArray.Length);

			while (propertiesArray [propertyIndex] <= 0) {
				propertyIndex = Random.Range(0,propertiesArray.Length);
			}

			switch (propertyIndex) {
			case 0:
				attackGain += propertyGain;
				strengthenGainStr = "攻击+" + propertyGain.ToString ();
				break;
			case 1:
				attackSpeedGain += propertyGain;
				strengthenGainStr = "攻速+" + propertyGain.ToString ();
				break;
			case 2:
				armourGain += propertyGain;
				strengthenGainStr = "护甲+" + propertyGain.ToString ();
				break;
			case 3:
				manaResistGain += propertyGain;
				strengthenGainStr = "抗性+" + propertyGain.ToString ();
				break;
			case 4:
				critGain += propertyGain;
				strengthenGainStr = "暴击+" + propertyGain.ToString ();
				break;
			case 5:
				dodgeGain += propertyGain;
				strengthenGainStr = "闪避+" + propertyGain.ToString ();
				break;
			}

			strengthenTimes++;

			return strengthenGainStr;
		}
			
	}
}
