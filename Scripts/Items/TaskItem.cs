using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class TaskItem : Item {

		public TaskItem(ItemModel itemModel){
			this.itemType = ItemType.Task;
		}

		public override string GetItemPropertiesString ()
		{
			return string.Empty;
		}

		public override string GetItemTypeString ()
		{
			return string.Empty;
		}

		public override string GetItemQualityString ()
		{
			return string.Empty;
		}
	}
}
