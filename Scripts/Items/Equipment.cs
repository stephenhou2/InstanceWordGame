using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	using System.Text;

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

	[System.Serializable]
	public class Equipment : Item {

		public float attackGain;//攻击力增益
		public float attackSpeedGain;//攻速增益
		public float critGain;//暴击增益
		public float armorGain;//护甲增益
		public float manaResistGain;//魔抗增益
		public float dodgeGain;//闪避增益
		public float healthGain;//生命增益
		public float manaGain;//魔法增益

		public int maxAttachedProperties;//附加属性最大数量
		public int attachedPropertyId;//一定有的附加属性id
		public EquipmentType equipmentType;//装备类型
//		public bool formulaUnlocked;//装备配方是否已经解锁

		public List<Material> materials = new List<Material> ();//合成材料需求
		public List<Material> failMaterials = new List<Material>();//合成失败时可能掉落的特殊材料

		public List<EquipmentAttachedProperty> attachedProperties = new List<EquipmentAttachedProperty> ();

		public int maxDurability;//装备最大耐久度
		public int durability;//装备实际耐久度


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
		public Equipment(ItemModel itemModel, int materialCount, FuseStone fuseStone = null){

			this.itemType = ItemType.Equipment;
			this.itemCount = 1;

			// 初始化物品基本属性
			InitBaseProperties (itemModel);

			// 初始化装备属性
			this.attackGain = itemModel.attackGain;
			this.attackSpeedGain = itemModel.attackSpeedGain;
			this.critGain = itemModel.critGain;
			this.armorGain = itemModel.armorGain;
			this.manaResistGain = itemModel.manaResistGain;
			this.dodgeGain = itemModel.dodgeGain;
			this.healthGain = itemModel.healthGain;
			this.manaGain = itemModel.manaGain;

			// 如果是玩家自己合成的装备
			if (materialCount != 0) {
				// 耐久度=材料使用数量*10
				this.maxDurability = 10 * materialCount;
				this.durability = this.maxDurability;
			} else {// 如果是探索中掉落的装备
				// 耐久度= 5*【材料种类，2倍材料种类】
				this.maxDurability = 5 * Random.Range (itemModel.materials.Count, 2 * itemModel.materials.Count + 1);
				this.durability = this.maxDurability;
			}

			this.equipmentType = itemModel.equipmentType;

			this.materials = itemModel.materials;

			this.failMaterials = itemModel.failMaterials;

			if (fuseStone != null) {
				this.itemName = string.Format ("{0}{1}", fuseStone.itemName.Replace ("之石", "的"), itemName);
			}

//			this.formulaUnlocked = itemModel.formulaUnlocked;

			InitAttachedProperties (itemModel.attachedPropertyId, itemModel.maxAttachedPropertyCount);

		}


		/// <summary>
		/// 初始化装备的附加属性
		/// </summary>
		/// <param name="predeterminedPropertyId">一定有的附加属性id.</param>
		/// <param name="maxAttachedPropertyCount">附加属性最大数量.</param>
		private void InitAttachedProperties(int predeterminedPropertyId,int maxAttachedPropertyCount){

			// 附加属性id列表
			List<int> attachedPropertiesIdList = new List<int> ();

			// 附加属性数量
			int attachedPropertiesCount = 0;

			// 初始化空的id表，用于从中抽取id后删除，保证每次拿到的附加属性id不同
			for (int i = 0; i < GameManager.Instance.gameDataCenter.allEquipmentAttachedProperties.Count; i++) {
				attachedPropertiesIdList.Add (i);
			}

			EquipmentAttachedProperty predeterminedProperty = null;

			// 如果有必出的附加属性
			if (predeterminedPropertyId >= 0) {
				// 取得必出的附加属性
				predeterminedProperty = GameManager.Instance.gameDataCenter.allEquipmentAttachedProperties.Find (delegate(EquipmentAttachedProperty obj) {
					return obj.attachedPropertyId == predeterminedPropertyId;
				});
				// 将必出的附加属性加入到附加属性列表中
				attachedProperties.Add (predeterminedProperty);
				// 获得附加属性实际数量
				attachedPropertiesCount = Random.Range (1, maxAttachedPropertyCount + 1);
			} else {// 如果没有必出的附加属性
				// 获得附加属性实际数量
				attachedPropertiesCount = Random.Range (0, maxAttachedPropertyCount + 1);
			}


			// 随机附加属性
			for (int i = attachedProperties.Count; i < attachedPropertiesCount; i++) {

				int randomId = Random.Range (0, attachedPropertiesIdList.Count);

				EquipmentAttachedProperty attachedProperty = new EquipmentAttachedProperty (randomId, true);

				attachedProperties.Add (attachedProperty);

			}

		}

		/// <summary>
		/// 分解装备，目前默认只从合成材料当中选择一种作为分解所得返还（但不包括boss材料）
		/// </summary>
		/// <returns>The equipment.</returns>
		public List<Item> ResolveEquipment(){

			// 返还的材料列表
			List<Material> returnedMaterials = new List<Material> ();

			// 去除制造材料中的boss材料（即boss材料在装备分解之后不会返还）
			for (int i = 0; i < materials.Count; i++) {

				Material m = materials [i];

				if (m.materialType == MaterialType.Boss) {
					continue;
				}else{
					returnedMaterials.Add (m);
				}
			}
				
			// 返还的材料序号
			int materialIndex = Random.Range (0, returnedMaterials.Count);

			Player.mainPlayer.RemoveItem (this);

			Material returnedMaterial = returnedMaterials [materialIndex];

			returnedMaterial.itemCount = 1;

			Player.mainPlayer.AddItem (returnedMaterial);

			return new List<Item>{ returnedMaterial };

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
				propertyStr = string.Format ("{0} + {1}",property,gain);
			}else{
				propertyStr = string.Format ("{0} + {1}%",property,(int)gain*100);
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
		private string CompareItemsProperty(string property, float compareValue,float comparedValue,List<string> compareList){

			// 获得单个属性描述
			string propertyString = GenerateSinglePropertyString (property, compareValue, null);

			// 该项属性数值对比 
			float compare = compareValue - comparedValue;

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
			durability += CommonData.fixDurability;
			if (durability > maxDurability) {
				durability = maxDurability;
			}
		}



		public bool EquipmentDamaged(EquipmentDamageSource source){

			bool completeDamaged = false;

			int equipmentTypeToInt = (int)this.equipmentType;

			switch (source) {
			case EquipmentDamageSource.PhysicalAttack:
				if (equipmentTypeToInt == 0) {
					this.durability -= CommonData.durabilityDecreaseWhenAttack;
				}
				break;
			case EquipmentDamageSource.DestroyObstacle:
				if (equipmentTypeToInt == 0) {
					this.durability -= CommonData.durabilityDecreaseWhenAttackObstacle;
				}
				break;
			case EquipmentDamageSource.BePhysicalAttacked:
				if(equipmentTypeToInt >= 1 && equipmentTypeToInt <= 4){
					this.durability -= CommonData.durabilityDecreaseWhenBeAttacked;
				}
				break;
			case EquipmentDamageSource.BeMagicAttacked:
				if (equipmentTypeToInt >= 5 && equipmentTypeToInt <= 6) {
					this.durability -= CommonData.durabilityDecreaseWhenBeMagicAttacked;
				}
				break;
			}


			completeDamaged = this.durability <= 0;

			if (completeDamaged) {
				Player.mainPlayer.RemoveItem (this);
			}

			return completeDamaged;

		}

	}

	public enum EquipmentDamageSource{
		PhysicalAttack,
		BePhysicalAttacked,
		BeMagicAttacked,
		DestroyObstacle
	}


}
