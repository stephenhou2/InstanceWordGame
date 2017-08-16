using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	[System.Serializable]
	public class NPC  {

		public int npcId;

		public string npcName;

		public string npcDescription;

		public string spriteName;

		public DialogGroup[] dialogGroups;

		public Choice[] choices;

		public int[] itemIds;

		public int[] skillIds;

		public override string ToString ()
		{
			return string.Format ("[NPC]:" + npcName + "[\nnpcDesc:]" + npcDescription);
		}


	}


	[System.Serializable]
	public class DialogGroup {
		
		public Dialog[] dialogs;

	}

	[System.Serializable]
	public class Dialog {

		// 每步的情节
		public string dialog;
//		// 是否会触发选择
//		public bool isTrigger = false;
		// 选择数组
		public int[] choiceIds;


		public override string ToString ()
		{
			return string.Format ("[Plot]" + dialog + "\n[choiceIds]" + choiceIds);
		}

	}




	[System.Serializable]
	public class Choice {

		public string choice;
		public int dialogId;
		public bool isEnd;
		public ChoiceTriggerType triggerType;


		public override string ToString ()
		{
			return string.Format ("[Choice]" + choice + "\n" + "[dialog]" + dialogId);
		}
	}
}