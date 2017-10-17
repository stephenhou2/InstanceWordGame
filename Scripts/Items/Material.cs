using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{

	public enum MaterialType{
		Base,//基础材料
		Rare,//特殊材料
		Monster,//怪物掉落材料
		Boss//boss掉落材料
	}

	[System.Serializable]
	public class Material:Item {
		
//		public int id;//材料id
//		public string materialName;//材料名称
		public MaterialType materialType;//材料类型
//		public string propertyString;//材料属性描述

		public int attackGain;//攻击增益
		public int attackSpeedGain;//攻速增益
		public int armorGain;//护甲增益
		public int manaResistGain;//抗性增益
		public int dodgeGain;//闪避增益
		public int critGain;//暴击增益
		public int healthGain;//生命增益
		public int manaGain;//魔法增益
		public int valence;//材料价位

//		public string spell;//拼写
//		public string spriteName;//材料对应的图片名称

		public int unstableness;//材料的不稳定性

//		public int materialCount;//材料数量


		public Material(Material materialModel,int count){
			this.itemId = materialModel.itemId;
			this.itemName = materialModel.itemName;
			this.materialType = materialModel.materialType;
			this.valence = materialModel.valence;
			this.itemDescription = materialModel.itemDescription;
			this.attackGain = materialModel.attackGain;
			this.attackSpeedGain = materialModel.attackSpeedGain;
			this.armorGain = materialModel.armorGain;
			this.manaResistGain = materialModel.manaResistGain;
			this.dodgeGain = materialModel.dodgeGain;
			this.critGain = materialModel.critGain;
			this.healthGain = materialModel.healthGain;
			this.manaGain = materialModel.manaGain;
			this.itemNameInEnglish = materialModel.itemNameInEnglish;
			this.spriteName = materialModel.spriteName;
			this.unstableness = materialModel.unstableness;
			this.itemCount = count;
		}


		public override string ToString ()
		{
			return string.Format ("[Material]\nid:{0},name:{1},materialType:{2},valence:{3},propertyString:{4},spell:{5}",
				itemId,itemName,materialType,valence,itemDescription,itemNameInEnglish);
		}

		public override string GetItemBasePropertiesString ()
		{
			return itemDescription;
		}

		public override string GetItemTypeString ()
		{
			return "材料";
		}

	}


}
