using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class WordItemView : CellDetailView {

		public Text spellText;
		public Text explainationText;

		public override void SetUpCellDetailView (object data)
		{
			Word word = data as Word;

			spellText.text = word.spell;

			explainationText.text = word.explaination;
		}

	}
}
