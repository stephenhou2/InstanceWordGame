using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	public enum NPCAttachedFunctionType{
		Task,
		SkillPromotion,
		PropertyPromotion,
		CharactersTrade
	}


	[System.Serializable]
	public class NPC  {

		// npc的id
		public int npcId;
		// npc的名字
		public string npcName;
		// npc的图片名称
		public string spriteName;
		// npc的打招呼文字
		public string greetingDialog;
		// npc能否发布的所有任务数组
		public List<Task> tasks = new List<Task>();
		// npc的附加功能数组
		public NPCAttachedFunctionType[] attachedFunctions;
		// npc的对话组
		public List<DialogGroup> chatDialogGroups = new List<DialogGroup>();


		public override string ToString ()
		{
			return string.Format ("[NPC]:" + npcName);
		}


	}


	[System.Serializable]
	public class DialogGroup {

		// 对话组对应的关卡id
		public int accordGameLevel;
		// 对话组中的所有对话
		public List<Dialog> dialogs = new List<Dialog>();

		public DialogGroup(){

		}

		public DialogGroup(int accordGameLevel,List<Dialog> dialogs){
			this.accordGameLevel = accordGameLevel;
			this.dialogs = dialogs;
		}

	}

	[System.Serializable]
	public class Dialog {

		// 对话的文字
		public string dialog;
		// 对话的id
		public int dialogId;
		// 对话对应的选择组
		public List<Choice> choices = new List<Choice>();
		// 对话触发的奖励id
		public int[] rewardIds;
		// 对话触发的奖励的数量
		public int[] rewardCounts;
		// 该对话是否作为结束当前对话组的对话
		public bool isEndingDialog;
		// 该对话的奖励是否已经执行过
		public bool finishRewarding;


		public Dialog(){

		}

		public Dialog(string dialog,int dialogId, List<Choice> choices,bool isEndingDialog, int accordGameLevel,int[] rewardIds,int[] rewardCounts){
			this.dialog = dialog;
			this.dialogId = dialogId;
			this.choices = choices;
			this.isEndingDialog = isEndingDialog;
			this.rewardIds = rewardIds;
			this.rewardCounts = rewardCounts;
		}

//		public override string ToString ()
//		{
//			return string.Format ("[Plot]" + dialog + "\n[choiceIds]" + choiceIds);
//		}

	}


	public enum ChoiceTriggerType{
		Dialog,
		Reward,
		Task
	}

	[System.Serializable]
	public class Choice {

		// 对话对应的选择文字
		public string choice;
		// 该选择对应的接下来的对话id
		public int dialogId;
		// 点击该选择是否结束对话
		public bool isEnd;
		// 该选择对应的事件触发类型
		public ChoiceTriggerType triggerType;

		public Choice(){

		}

		public Choice(string choice,int dialogId,bool isEnd,ChoiceTriggerType choiceTriggerType){
			this.choice = choice;
			this.dialogId = dialogId;
			this.isEnd = isEnd;
			this.triggerType = choiceTriggerType;

		}

		public override string ToString ()
		{
			return string.Format ("[Choice]" + choice + "\n" + "[dialog]" + dialogId);
		}
	}


	public enum TaskType
	{
		KillMonster,// 击杀怪物
		HandInItem // 交付物品
	} 

	[System.Serializable]
	public class Task{

		// 发布任务前的对话组
		public DialogGroup taskDialogGroup;
		// 任务描述
		public string taskDescription;
		// 任务类型
		public TaskType taskType;
		// 任务对应怪物或者物品的id
		public int accordMonsterOrItemId;
		// 完成任务所需要达到的数量
		public int accordMonsterOrItemCount;
		// 完成任务后在npc出开启的对话id
		public int dialogIdWhenTaskAccomplished;
		// 任务出现的关卡id
		public int accordGameLevel;

		public Task(){
			
		}

		public Task(DialogGroup taskDialogGroup, string taskDescription,TaskType taskType,int accordMonsterOrItemId,int accordMonsterOrItemCount,int dialogIdWhenTaskAccomplished,int accordGameLevel){
			this.taskDialogGroup = taskDialogGroup;
			this.taskDescription = taskDescription;
			this.taskType = taskType;
			this.accordMonsterOrItemId = accordMonsterOrItemId;
			this.accordMonsterOrItemCount = accordMonsterOrItemCount;
			this.dialogIdWhenTaskAccomplished = dialogIdWhenTaskAccomplished;
			this.accordGameLevel = accordGameLevel;
		}

	}
}