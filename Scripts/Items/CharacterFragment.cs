using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	// 字母碎片的id=200
	public class CharacterFragment : Item {

		public char character;

		public CharacterFragment(char character,int count = 1){
			this.itemId = 200;
			this.character = character;
			itemName = string.Format ("字母碎片-{0}", character.ToString());
			itemNameInEnglish = character.ToString();
			spriteName = string.Format ("character_{0}", character);
			itemCount = count;
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
