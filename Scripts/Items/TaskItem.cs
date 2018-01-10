using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	[System.Serializable]
	public class TaskItem : Item {

		public TaskItem(ItemModel itemModel){
//			this.itemType = ItemType.Task;

			InitBaseProperties (itemModel);

		}


		public override string GetItemTypeString ()
		{
			return string.Empty;
		}



	}
}
