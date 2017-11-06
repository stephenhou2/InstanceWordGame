using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;


namespace WordJourney
{
	/// <summary>
	/// 消耗品类型
	/// </summary>
	public enum ConsumablesType{
		MedicineAndScroll,
		Pickaxe,
		Key,
		TP
	}

	public class ConsumablesEffectState{

		public Consumables consumables;
		public int effectTime;

		public ConsumablesEffectState(Consumables consumables){
			this.consumables = consumables;
			this.effectTime = 0;
		}

	}

	[System.Serializable]
	public class Consumables : Item {

		public float attackGain;//攻击力增益
		public float attackSpeedGain;//攻速增益
		public float critGain;//暴击增益
		public float armorGain;//护甲增益
		public float manaResistGain;//魔抗增益
		public float dodgeGain;//闪避增益
		public float healthGain;//生命增益
		public float manaGain;//魔法增益
		public float physicalHurtScaler;
		public float magicHurtScaler;
		public int effectDuration;

		public ConsumablesType consumablesType;

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="itemModel">Item model.</param>
		public Consumables(ItemModel itemModel,int itemCount){

			this.itemType = ItemType.Consumables;
			this.consumablesType = itemModel.consumbalesType;
			this.itemCount = itemCount;

			this.physicalHurtScaler = itemModel.physicalHurtScaler;
			this.magicHurtScaler = itemModel.magicHurtScaler;
			this.effectDuration = itemModel.effectDuration;

			// 初始化物品基础属性
			InitBaseProperties (itemModel);

			// 初始化消耗品属性
			healthGain = itemModel.healthGain;
			manaGain = itemModel.manaGain;

		}

		public Consumables (Consumables cons,int count){
			
			this.itemType = ItemType.Consumables;

			// 初始化物品基础属性
			this.itemId = cons.itemId;
			this.itemName = cons.itemName;
			this.itemDescription = cons.itemDescription;
			this.spriteName = cons.spriteName;
			this.itemType = cons.itemType;

			// 初始化消耗品属性
			this.healthGain = cons.healthGain;
			this.manaGain = cons.manaGain;

			this.itemCount = count;


		}

		/// <summary>
		/// 获取物品属性字符串
		/// </summary>
		/// <returns>The item properties string.</returns>
		public override string GetItemBasePropertiesString(){

			StringBuilder itemProperties = new StringBuilder ();

			List<string> propertiesList = new List<string> ();

			if (healthGain > 0) {
				string str = string.Format ("体力+{0}",healthGain);
				propertiesList.Add (str);
			}
			if (manaGain > 0) {
				string str = string.Format ("魔法+{0}",manaGain);
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
		/// 获取物品类型字符串
		/// </summary>
		/// <returns>The item type string.</returns>
		public override string GetItemTypeString ()
		{
			return "类型: 消耗品";
		}



	}
}