using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

[System.Serializable]
public class Item {

	public string itemName;
	public string itemDescription;
	public string spriteName;

	public ItemType itemType;

	public string itemNameInEnglish;
	public int itemId;
	public int itemCount;

	public ItemQuality itemQuality;

	public int strengthenTimes;

	public int attackGain;//攻击力增益
	public int powerGain;//力量增益
	public int magicGain;//魔法增益
	public int critGain;//暴击增益
	public int amourGain;//护甲增益
	public int magicResistGain;//魔抗增益
	public int agilityGain;//闪避增益

//	public int strengthConsume;//气力消耗

	public int healthGain;//血量增益
	public int strengthGain;//气力增益

	public bool equiped;

	private int minGain = -3;
	private int maxGain = 8;

	private int[] propertiesArray;

	public Item(){

	}

	public Item(Item originalItem,bool completeCopy){

		itemId = originalItem.itemId;
		itemName = originalItem.itemName;
		itemDescription = originalItem.itemDescription;
		spriteName = originalItem.spriteName;
		itemType = originalItem.itemType;
		itemNameInEnglish = originalItem.itemNameInEnglish;
		attackGain = originalItem.attackGain;
		powerGain = originalItem.powerGain;
		magicGain = originalItem.magicGain;
		critGain = originalItem.critGain;
		amourGain = originalItem.amourGain;
		magicResistGain = originalItem.magicResistGain;
		agilityGain = originalItem.agilityGain;
		healthGain = originalItem.healthGain;
		strengthGain = originalItem.strengthGain;


		if (!completeCopy) {
			
			RandomQuility ();

			ResetBasePropertiesByQuality ();
		}

	}

	public string GetItemPotentialPropertiesString(){

		StringBuilder itemProperties = new StringBuilder ();

		List<string> propertiesList = new List<string> ();

		if (attackGain > 0) {
			string str = string.Format ("攻击: {0}~{1}", attackGain + minGain, attackGain + maxGain);
			propertiesList.Add (str);
		}
		if (magicGain > 0) {
			string str = string.Format ("魔法: {0}~{1}", magicGain + minGain, magicGain + maxGain);
			propertiesList.Add (str);
		}
		if (critGain > 0) {
			string str = string.Format ("暴击: {0}~{1}", critGain + minGain, critGain + maxGain);
			propertiesList.Add (str);
		}
		if (amourGain > 0) {
			string str = string.Format ("护甲: {0}~{1}", amourGain + minGain, amourGain + maxGain);
			propertiesList.Add (str);
		}
		if (magicResistGain > 0) {
			string str = string.Format ("抗性: {0}~{1}", magicResistGain + minGain, magicResistGain + maxGain);
			propertiesList.Add (str);
		}
		if (agilityGain > 0) {
			string str = string.Format ("闪避: {0}~{1}", agilityGain + minGain, agilityGain + maxGain);
			propertiesList.Add (str);
		} 
		if (healthGain > 0) {
			string str = string.Format ("体力+{0}",healthGain);
			propertiesList.Add (str);
		}
		if (strengthGain > 0) {
			string str = string.Format ("气力+{0}",strengthGain);
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

	public string GetItemPropertiesString(){

		StringBuilder itemProperties = new StringBuilder ();

		List<string> propertiesList = new List<string> ();

		if (attackGain > 0) {
			string str = string.Format ("攻击: {0}", attackGain);
			propertiesList.Add (str);
		}
		if (magicGain > 0) {
			string str = string.Format ("魔法: {0}", magicGain);
			propertiesList.Add (str);
		}
		if (critGain > 0) {
			string str = string.Format ("暴击: {0}", critGain);
			propertiesList.Add (str);
		}
		if (amourGain > 0) {
			string str = string.Format ("护甲: {0}", amourGain);
			propertiesList.Add (str);
		}
		if (magicResistGain > 0) {
			string str = string.Format ("抗性: {0}", magicResistGain);
			propertiesList.Add (str);
		}
		if (agilityGain > 0) {
			string str = string.Format ("闪避: {0}", agilityGain);
			propertiesList.Add (str);
		} 
		if (healthGain > 0) {
			string str = string.Format ("体力+{0}",healthGain);
			propertiesList.Add (str);
		}
		if (strengthGain > 0) {
			string str = string.Format ("气力+{0}",strengthGain);
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

	public string GetItemQualityString(){
		
		string itemQualityStr = string.Empty;

		if (itemType == ItemType.Consumables) {
			itemQualityStr = "品级: -";
			return itemQualityStr;
		}

		if (itemQuality == ItemQuality.S) {
			itemQualityStr = "<color=orange>品级: " + itemQuality.ToString () + "</color>";
		} else {
			itemQualityStr = "品级: " + itemQuality.ToString () + "级";
		}

		return itemQualityStr;

	}

	public string GetItemTypeString(){

		string itemTypeStr = string.Empty;

		switch (itemType) {
		case ItemType.Weapon:
			itemTypeStr = "类型: 武器";
			break;
		case ItemType.Amour:
			itemTypeStr = "类型: 防具";
			break;
		case ItemType.Shoes:
			itemTypeStr = "类型: 步履";
			break;
		case ItemType.Consumables:
			itemTypeStr = "类型: 消耗品";
			break;
		case ItemType.Task:
			itemTypeStr = "类型: 任务物品";
			break;
		case ItemType.FuseStone:
			itemTypeStr = "类型: 融合石";
			break;
		}

		return itemTypeStr;

	}

	public string GetComparePropertiesStringWithItem(Item compareItem){

		StringBuilder itemProperties = new StringBuilder ();

		List<string> propertiesList = new List<string> ();

		int compare = 0;
		string linkSymbol = string.Empty;
		string colorText = string.Empty;

		if (attackGain > 0) {

			CompareItemsProperty (attackGain, compareItem.attackGain,out compare,out linkSymbol,out colorText);

//			int compare = attackGain - item.attackGain;
//	
//			string linkSymbol = compare < 0 ? "-" : "+";
//	
//			string colorText = compare < 0 ? "<color=red>" : "<color=green>";

			string str = string.Format ("攻击: {0}({1}{2}{3}</color>)", attackGain,colorText,linkSymbol,Mathf.Abs(compare));

			propertiesList.Add (str);
		}
		if (magicGain > 0) {

			CompareItemsProperty (magicGain, compareItem.magicGain,out compare,out linkSymbol,out colorText);

//			int compare = magicGain - item.magicGain;
//
//			string linkSymbol = compare < 0 ? "-" : "+";
//
//			string colorText = compare < 0 ? "<color=red>" : "<color=green>";
//
			string str = string.Format ("魔法: {0}({1}{2}{3}</color>)", magicGain,colorText,linkSymbol,Mathf.Abs(compare));

			propertiesList.Add (str);
		}
		if (critGain > 0) {

			CompareItemsProperty (critGain, compareItem.critGain,out compare,out linkSymbol,out colorText);

//			int compare = critGain - item.critGain;
//
//			string preText = null;
//
//			string linkSymbol = compare < 0 ? "-" : "+";
//
//			string colorText = compare < 0 ? "<color=red>" : "<color=green>";

			string str = string.Format ("暴击: {0}({1}{2}{3}</color>)", critGain,colorText,linkSymbol,Mathf.Abs(compare));

			propertiesList.Add (str);
		}
		if (amourGain > 0) {

			CompareItemsProperty (amourGain, compareItem.amourGain,out compare,out linkSymbol,out colorText);

			string str = string.Format ("护甲: {0}({1}{2}{3}</color>)", amourGain,colorText,linkSymbol,Mathf.Abs(compare));

			propertiesList.Add (str);
		}
		if (magicResistGain > 0) {

			CompareItemsProperty (magicResistGain, compareItem.magicResistGain,out compare,out linkSymbol,out colorText);

			string str = string.Format ("抗性: {0}({1}{2}{3}</color>)", magicResistGain,colorText,linkSymbol,Mathf.Abs(compare));

			propertiesList.Add (str);
		}
		if (agilityGain > 0) {

			CompareItemsProperty (agilityGain, compareItem.agilityGain,out compare,out linkSymbol,out colorText);

			string str = string.Format ("闪避: {0}({1}{2}{3}</color>)", agilityGain,colorText,linkSymbol,Mathf.Abs(compare));

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

	private void CompareItemsProperty(int propertyValue0,int propertyValue1,out int compare,out string linkSymbol,out string colorText){
		
		compare = propertyValue0 - propertyValue1;

		linkSymbol = compare < 0 ? "-" : "+";

		colorText = compare < 0 ? "<color=red>" : "<color=green>";
	}

	public bool CheckCanStrengthen(){

		bool canStrengthen = false;

		switch (itemType) {
		case ItemType.Weapon:
			canStrengthen = true;
			break;
		case ItemType.Amour:
			canStrengthen = true;
			break;
		case ItemType.Shoes:
			canStrengthen = true;
			break;
		default:
			break;
		}

		return canStrengthen;

	}

	public string StrengthenItem(){

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
			
			propertiesArray = new int[]{ attackGain, magicGain, amourGain, magicResistGain, critGain, agilityGain };

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
			magicGain += propertyGain;
			strengthenGainStr = "魔法+" + propertyGain.ToString ();
			break;
		case 2:
			amourGain += propertyGain;
			strengthenGainStr = "护甲+" + propertyGain.ToString ();
			break;
		case 3:
			magicResistGain += propertyGain;
			strengthenGainStr = "抗性+" + propertyGain.ToString ();
			break;
		case 4:
			critGain += propertyGain;
			strengthenGainStr = "暴击+" + propertyGain.ToString ();
			break;
		case 5:
			agilityGain += propertyGain;
			strengthenGainStr = "闪避+" + propertyGain.ToString ();
			break;
		}

		strengthenTimes++;

		return strengthenGainStr;
	}

	private float chanceOfGain(float[] chanceArray,int gain){
		float totalChance = 0f;
		for (int i = 0; i < gain; i++) {
			totalChance += chanceArray [i];
		}
		return totalChance;
	}

	private void ResetBasePropertiesByQuality(){

		switch (itemQuality) {
		case ItemQuality.C:
			ResetProperties (-3, 1);
			break;
		case ItemQuality.B:
			ResetProperties (-2, 3);
			break;
		case ItemQuality.A:
			ResetProperties (-1, 5);
			break;
		case ItemQuality.S:
			ResetProperties (1, 8);
			break;

		}

	}

	private void ResetProperties(int minGain,int maxGain){

		if (attackGain > 0) {
			attackGain += Random.Range (minGain, maxGain);
		}
		if (powerGain > 0) {
			powerGain += Random.Range (minGain, maxGain);
		}
		if (magicGain > 0) {
			magicGain += Random.Range (minGain, maxGain);
		}
		if (critGain > 0) {
			critGain += Random.Range (minGain, maxGain);
		}
		if (amourGain > 0) {
			amourGain += Random.Range (minGain, maxGain);
		}
		if (magicResistGain > 0) {
			magicResistGain += Random.Range (minGain, maxGain);
		}
		if (agilityGain > 0) {
			agilityGain += Random.Range (minGain, maxGain);
		}


	}

	private void RandomQuility(){
		int seed = Random.Range (0, 100);
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

	public override string ToString ()
	{
		return string.Format ("[Item]:" + itemName + "[\nItemDesc]:" + itemDescription);
	}


}
