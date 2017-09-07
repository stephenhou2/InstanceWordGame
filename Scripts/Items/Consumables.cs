using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;


namespace WordJourney
{

	public enum ConsumablesType{
		Medicine,
		Tool
	}

	public class Consumables : Item {

		public int healthGain;//血量增益
		public int manaGain;//魔法增益

		public Consumables(){
		}

		public Consumables(ItemModel itemModel){

			this.itemType = ItemType.Consumables;

			healthGain = itemModel.healthGain;
			manaGain = itemModel.manaGain;

		}

		public override string GetItemPropertiesString(){

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

		public override string GetItemTypeString ()
		{
			return "类型: 消耗品";
		}

		public override string GetItemQualityString ()
		{
			return string.Empty;
		}

	}
}