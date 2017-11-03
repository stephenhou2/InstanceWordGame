using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public enum FormulaType{
		Equipment,
		Skill
	}

	public class Formula:Item {

		public FormulaType formulaType;
		public string formulaName;


		public override string GetItemBasePropertiesString ()
		{
			throw new System.NotImplementedException ();
		}

		public override string GetItemTypeString ()
		{
			throw new System.NotImplementedException ();
		}

		public override string ToString ()
		{
			return string.Format ("[Formula]");
		}
	}
}
