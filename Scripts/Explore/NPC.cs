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
	// 是否会触发选择
	public bool isTrigger = false;
	// 选择数组
	public int[] choiceIds;


	public override string ToString ()
	{
		return string.Format ("[Plot]" + dialog + "\n"+ "[isTrigger]" + isTrigger + "\n" + "[choices]" + choiceIds);
	}

}


public enum ChoiceTriggerType{
	Plot,
	Fight,
	Magic
}
[System.Serializable]
public class Choice {

	public string text;
	public int plotId;
	public ChoiceTriggerType triggerType;

	public override string ToString ()
	{
		return string.Format ("[Choice]" + text + "\n" + "[plots]" + plotId + "\n" + "[isFightTrigger]" + triggerType);
	}
}