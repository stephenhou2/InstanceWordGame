using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;


namespace WordJourney
{



	[System.Serializable]
	public class Consumables : Item {

//		public float healthGain;//生命增益
//		public float manaGain;//魔法增益
//		public float attackGain;//攻击力增益
//		public float attackSpeedGain;//攻速增益
//		public float hitGain;//命中增益
//		public float critGain;//暴击增益
//		public float armorGain;//护甲增益
//		public float manaResistGain;//魔抗增益
//		public float dodgeGain;//闪避增益

//		public float physicalHurtScalerGain;
//		public float magicHurtScalerGain;
//		public float critHurtScalerGain;


		public SkillInfo[] attachedSkillInfos;

		public ItemModel.ItemInfoForProduce[] itemInfosForProduce;



		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="itemModel">Item model.</param>
		public Consumables(ItemModel itemModel,int itemCount){

			// 初始化物品基础属性
			InitBaseProperties (itemModel);

			this.itemType = ItemType.Consumables;
			this.itemCount = itemCount;

//			this.physicalHurtScalerGain = itemModel.physicalHurtScalerGain;
//			this.magicHurtScalerGain = itemModel.magicalHurtScalerGain;
//			this.critHurtScalerGain = itemModel.critHurtScalerGain;

			this.attachedSkillInfos = itemModel.attachedSkillInfos;

			this.itemInfosForProduce = itemModel.itemInfosForProduce;

			this.price = itemModel.price;
		}

		public Consumables (Consumables cons,int count){
	
			// 初始化物品基础属性
			this.itemId = cons.itemId;
			this.itemName = cons.itemName;
			this.itemDescription = cons.itemDescription;
			this.spriteName = cons.spriteName;
			this.itemType = cons.itemType;
			this.itemNameInEnglish = cons.itemNameInEnglish;

			this.itemType = ItemType.Consumables;
			this.itemCount = count;

			this.attachedSkillInfos = cons.attachedSkillInfos;
			this.itemInfosForProduce = cons.itemInfosForProduce;

			this.price = cons.price;


		}




		/// <summary>
		/// 获取物品类型字符串
		/// </summary>
		/// <returns>The item type string.</returns>
		public override string GetItemTypeString ()
		{
			return "消耗品";
		}



	}
}