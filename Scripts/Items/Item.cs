using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	public Item(){

	}

	public Item(Item originalItem){

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

		RandomQuility ();

		ResetPropertiesByQuality ();

	}

	public void ResetPropertiesByQuality(){

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
