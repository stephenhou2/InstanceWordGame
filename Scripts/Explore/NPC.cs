using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class NPC  {

	public string npcName;

	public string npcDescription;

	public string spriteName;

	public Dialog[] dialogs;

	public Choice[] choices;

	public override string ToString ()
	{
		return string.Format ("[NPC]:" + npcName + "[\nnpcDesc:]" + npcDescription);
	}


}

[System.Serializable]
public class Dialog {

	// 每步的情节
	public string dialog;
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

	public override string ToString ()
	{
		return string.Format ("[Choice]" + choice + "\n" + "[dialog]" + dialogId);
	}
}