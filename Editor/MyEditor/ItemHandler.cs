using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;


namespace WordJourney
{
	public class ItemHandler {

		public List<MyItem> allItems = new List<MyItem> ();

		public List<MyMaterial> allMaterials = new List<MyMaterial>();

		public List<MyAttachedProperty> allAttachedProperties = new List<MyAttachedProperty>();

		public void LoadAllData(){

			LoadAllMaterial ();
			LoadAllItem ();
			LoadAllAttachedProperties ();

			List<int> valences = new List<int> ();

			for (int i = 0; i < allItems.Count; i++) {

				if (allItems [i].itemType == ItemType.Consumables) {
					continue;
				}

				valences.Clear ();

				MyItem item = allItems [i];

				for (int j = 0; j < item.materials.Count; j++) {
					valences.Add (item.materials [j].valence);
				}

				valences.Sort ();

				if (valences [0] * valences [valences.Count - 1] > 0) {

					if (valences [0] < 0) {
						Debug.Log (string.Format ("{0},minus", item.itemName));
					} else {
						Debug.Log (string.Format ("{0},plus", item.itemName));
					}
				}
			}
		}

		public void SaveData(){


			string itemsJson = JsonHelper.ToJson<MyItem>(allItems.ToArray());

			string materialsJson = JsonHelper.ToJson<MyMaterial> (allMaterials.ToArray ());

			string attachedPropertiesJson = JsonHelper.ToJson<MyAttachedProperty>(allAttachedProperties.ToArray());

			Debug.Log (itemsJson);
			Debug.Log (materialsJson);
			Debug.Log (attachedPropertiesJson);

			File.WriteAllText ("/Users/houlianghong/Desktop/Unityfolder/TestOnSkills/Assets/StreamingAssets/Data/Items.json", itemsJson);
			File.WriteAllText ("/Users/houlianghong/Desktop/Unityfolder/TestOnSkills/Assets/StreamingAssets/Data/Materials.json", materialsJson);
			File.WriteAllText ("/Users/houlianghong/Desktop/Unityfolder/TestOnSkills/Assets/StreamingAssets/Data/AttachedProperties.json", attachedPropertiesJson);


		}

		private void LoadAllMaterial(){

			string materialSource = File.ReadAllText ("/Users/houlianghong/Desktop/MyGameData/物品系统/材料和属性-表格 1.csv");

			string[] materialStrings = materialSource.Split (new string[]{ "\n" }, StringSplitOptions.RemoveEmptyEntries);

			for (int i = 1; i < materialStrings.Length; i++) {

				MyMaterial m = new MyMaterial (materialStrings[i]);

				allMaterials.Add (m);

			}



		}

		private void LoadAllItem(){

			string itemSource = File.ReadAllText ("/Users/houlianghong/Desktop/MyGameData/物品系统/物品和属性-表格 1.csv");

			string[] itemStrings = itemSource.Split (new String[]{ "\n" }, StringSplitOptions.RemoveEmptyEntries);

			for (int i = 1; i < itemStrings.Length; i++) {

				MyItem item = new MyItem (itemStrings[i]);

				if (item.itemType == ItemType.Equipment) {

					string[] materialStrings = item.materialString.Split (new char[]{ '+' });

					for (int j = 0; j < materialStrings.Length; j++) {

						MyMaterial m = allMaterials.Find (delegate(MyMaterial obj) {
							return obj.itemName == materialStrings [j];
						});

						if (m == null) {
							Debug.Log (string.Format ("null:{0}-{1}", item.itemName, materialStrings [j]));
						}

						item.materials.Add (m);
					}

					string[] failMaterialStrings = item.failMaterialString.Split (new char[]{ '+' });

					for (int k = 0; k < failMaterialStrings.Length; k++) {
						MyMaterial m = allMaterials.Find (delegate(MyMaterial obj) {
							return obj.itemName == failMaterialStrings [k];
						});
						if (m == null) {
							Debug.Log (string.Format ("fail-null:{0}-{1}", item.itemName, failMaterialStrings [k]));
						}
						item.failMaterials.Add (m);
					}
				}

				allItems.Add (item);

//				Debug.Log (item);

			}

		}

		private void LoadAllAttachedProperties(){

			string attachedPropertiesSource = File.ReadAllText ("/Users/houlianghong/Desktop/MyGameData/物品系统/附加属性-表格 1.csv");

			string[] attachedPropertyStrings = attachedPropertiesSource.Split (new string[]{ "\n" }, StringSplitOptions.RemoveEmptyEntries);

			for (int i = 1; i < attachedPropertyStrings.Length; i++) {

				MyAttachedProperty ap = new MyAttachedProperty (attachedPropertyStrings[i]);

				allAttachedProperties.Add (ap);

			}

		}

	}

	[System.Serializable]
	public class MyItem{

		//名称 物品描述 合成材料 攻击	攻速	护甲	抗性	闪避	暴击	生命	魔法 物理伤害加成 魔法伤害加成 效果持续时间 附加属性数量上限	附加属性名称	必出附加属性id	必出附加属性加成	装备等级需求
		public string itemName;
		public string itemDescription;
		public string materialString;
		public string failMaterialString;
		public float attackGain;
		public float attackSpeedGain;
		public float armorGain;
		public float manaResistGain;
		public float dodgeGain;
		public float critGain;
		public float healthGain;
		public float manaGain;
		public float physicalHurtScaler;
		public float magicHurtScaler;
		public int effectDuration;
		public int maxAttachedPropertyCount;
		public string attachedPropertyString;
		public int attachedPropertyId;
		public EquipmentType equipmentType;
		public ConsumablesType consumablesType;
		public string detailType;
		public int levelRequired;
		public string spriteName;
		public int itemId;
		public ItemType itemType;

		public List<MyMaterial> materials = new List<MyMaterial> ();

		public List<MyMaterial> failMaterials = new List<MyMaterial>();

		//		public List<Material> baseMaterials = new List<Material>();
		//		public List<Material> specialMaterials = new List<Material>();
		//		public List<Material> monsterMaterials = new List<Material>();
		//		public List<Material> bossMaterials = new List<Material>();

		public MyItem(string itemString){

			string[] itemStrings = itemString.Split (new char[]{ ',' });

			itemId = Convert.ToInt16 (itemStrings [0]);
			itemName = itemStrings[1];
			itemDescription = itemStrings[2];
			materialString = itemStrings[3];
			failMaterialString = itemStrings [4];
			attackGain = Convert.ToSingle(itemStrings[5]);
			attackSpeedGain = Convert.ToSingle(itemStrings[6]);
			armorGain = Convert.ToSingle(itemStrings[7]);
			manaResistGain = Convert.ToSingle(itemStrings[8]);
			dodgeGain = Convert.ToSingle(itemStrings[9]);
			critGain = Convert.ToSingle(itemStrings[10]);
			healthGain = Convert.ToSingle(itemStrings[11]);
			manaGain = Convert.ToSingle(itemStrings[12]);
			physicalHurtScaler = Convert.ToSingle (itemStrings [13]);
			magicHurtScaler = Convert.ToSingle (itemStrings [14]);
			effectDuration = Convert.ToInt16 (itemStrings [15]);
			maxAttachedPropertyCount = Convert.ToInt16(itemStrings[16]);
			attachedPropertyString = itemStrings[17];
			attachedPropertyId = Convert.ToInt16(itemStrings[18]);
			itemType = (ItemType)(Convert.ToInt16(itemStrings[19]));
			equipmentType = (EquipmentType)(Convert.ToInt16(itemStrings[20]));
			consumablesType = (ConsumablesType)(Convert.ToInt16 (itemStrings [21]));
			detailType = itemStrings [22];
			levelRequired = Convert.ToInt16 (itemStrings [23]);
			spriteName = itemStrings [24];

			itemDescription = itemDescription.Replace ("_", "\n"); 


			AdjustData (attackGain,out attackGain);
			AdjustData (attackSpeedGain,out attackSpeedGain);
			AdjustData (armorGain,out armorGain);
			AdjustData (manaResistGain,out manaResistGain);
			AdjustData (dodgeGain,out dodgeGain);
			AdjustData (critGain,out critGain);
			AdjustData (healthGain,out healthGain);
			AdjustData (manaGain,out manaGain);
			AdjustData (physicalHurtScaler, out physicalHurtScaler);
			AdjustData (magicHurtScaler, out magicHurtScaler);
			AdjustData (effectDuration, out effectDuration);



		}

		private void AdjustData(float v,out float value){
			value = v < 0 ? 0 : v;
		}

		private void AdjustData(int v,out int value){
			value = v < 0 ? 0 : v;
		}


		public override string ToString ()
		{
			return string.Format ("[Item]\nname:{0},itemDescription:{1},materialString:{2},failMaterialString:{3}maxAttachedProperties:{4},attachedPropertyString:{5},attachedPropertyId:{6}",
				itemName,itemDescription,materialString,failMaterialString,maxAttachedPropertyCount,attachedPropertyString,attachedPropertyId);
		}

	}

	[System.Serializable]
	public class MyMaterial{

		//材料id	材料名称	灵势	材料属性	攻击	攻速	护甲	抗性	闪避	暴击	生命	魔法	材料拼写
		public int itemId;
		public string itemName;
		public string itemDescription;
		public string spriteName;
		public string itemNameInEnglish;
		public ItemType itemType;

//		public int id;
//		public string materialName;
		public MaterialType materialType;
		public int valence;
//		public string propertyString;
		public int attackGain;
		public int attackSpeedGain;
		public int armorGain;
		public int manaResistGain;
		public int dodgeGain;
		public int critGain;
		public int healthGain;
		public int manaGain;
//		public string spell;
//		public string spriteName;
		public int unstableness;

		public MyMaterial(string materialString){

			string[] materialStrings = materialString.Split (new char[]{ ',' });

			itemId = Convert.ToInt16(materialStrings[0]);
			itemName = materialStrings[1];
			materialType = (MaterialType)(Convert.ToInt16(materialStrings[2]));
			valence = Convert.ToInt16(materialStrings[3]);
			itemDescription = materialStrings[4];
			attackGain = Convert.ToInt16(materialStrings[5]);
			attackSpeedGain = Convert.ToInt16(materialStrings[6]);
			armorGain = Convert.ToInt16(materialStrings[7]);
			manaResistGain = Convert.ToInt16(materialStrings[8]);
			dodgeGain = Convert.ToInt16(materialStrings[9]);
			critGain = Convert.ToInt16(materialStrings[10]);
			healthGain = Convert.ToInt16(materialStrings[11]);
			manaGain = Convert.ToInt16(materialStrings[12]);
			itemNameInEnglish = materialStrings[13];
			spriteName = materialStrings [14];
			unstableness = Math.Abs (valence);
			itemType = ItemType.Material;

			if (itemDescription == "-1") {
				itemDescription = string.Empty;
			}

		}


		public override string ToString ()
		{
			return string.Format ("[Material]\nid:{0},name:{1},materialType:{2},valence:{3},propertyString:{4},spell:{5}",
				itemId,itemName,materialType,valence,itemDescription,itemNameInEnglish);
		}

	}

	[System.Serializable]
	public class MyAttachedProperty{

		public int attachedPropertyId;
		public int attackGain;
		public int attackSpeedGain;
		public int armorGain;
		public int manaResistGain;
		public int dodgeGain;
		public int critGain;
		public int healthGain;
		public int manaGain;
		public int physicalHurtGain;
		public int magicHurtGain;
		public int healthAbsorbGain;
		public int hardBeatGain;
		public int brambleShiledGain;
		public int magicShieldGain;
		public int allPropertiesGain;

		public MyAttachedProperty(string attachedPropertyString){

			string[] attachedPropertyStrings = attachedPropertyString.Split (new char[]{ ',' });

			attachedPropertyId = Convert.ToInt16 (attachedPropertyStrings [0]);
			attackGain = Convert.ToInt16 (attachedPropertyStrings [2]);
			attackSpeedGain = Convert.ToInt16 (attachedPropertyStrings [3]);
			armorGain = Convert.ToInt16 (attachedPropertyStrings [4]);
			manaResistGain = Convert.ToInt16 (attachedPropertyStrings [5]);
			dodgeGain = Convert.ToInt16 (attachedPropertyStrings [6]);
			critGain = Convert.ToInt16 (attachedPropertyStrings [7]);
			healthGain = Convert.ToInt16 (attachedPropertyStrings [8]);
			manaGain = Convert.ToInt16 (attachedPropertyStrings [9]);
			physicalHurtGain = Convert.ToInt16 (attachedPropertyStrings [10]);
			magicHurtGain = Convert.ToInt16 (attachedPropertyStrings [11]);
			healthAbsorbGain = Convert.ToInt16 (attachedPropertyStrings [12]);
			hardBeatGain = Convert.ToInt16 (attachedPropertyStrings [13]);
			brambleShiledGain = Convert.ToInt16 (attachedPropertyStrings [14]);
			magicShieldGain = Convert.ToInt16 (attachedPropertyStrings [15]);
			allPropertiesGain = Convert.ToInt16 (attachedPropertyStrings [16]);


		}

	}
		
}
