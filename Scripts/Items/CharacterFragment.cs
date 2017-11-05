using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	// 字母碎片的id=3102
	public class CharacterFragment : Item {

		public CharacterFragment(char character,int count = 1){
			this.itemId = 3102;
			itemName = string.Format ("字母碎片-{0}", character.ToString());
			itemNameInEnglish = character.ToString();
			spriteName = "character_fragment";
			itemCount = count;
		}


		public override string GetItemBasePropertiesString ()
		{
			return itemName;
		}

		public override string GetItemTypeString ()
		{
			return "字母碎片";
		}

		public override string ToString ()
		{
			return string.Format ("[CharacterFragment]");
		}
	}
}
