using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	public enum NPCAttachedFunctionType{
		None,
		SkillPromotion,
		PropertyPromotion,
		CharactersTrade
	}


	[System.Serializable]
	public class NPC  {

		public int npcId;

		public string npcName;

		public string spriteName;

		public NPCAttachedFunctionType attachedFunction;

		public DialogGroup[] dialogGroups;


		/// <summary>
		/// 获取从对话中得到的奖励物品
		/// </summary>
		/// <returns>The rewards from dialog.</returns>
//		public Item[] GetRewardsFromChoice(Choice choice){
//
//			Item[] rewards = new Item[choice.rewardIds.Length];
//
//			for (int i = 0; i < choice.rewardIds.Length; i++) {
//
//				Item item = Item.NewItemWith(choice.rewardIds [i], choice.rewardCounts [i]);
//
//				rewards [i] = item;
//
//			}
//
//			return rewards;
//		}

		/// <summary>
		/// 获得从任务中获得的奖励物品
		/// </summary>
		/// <returns>The rewards from task.</returns>
//		public Item[] GetRewardsFromTask(){
//
//			Item[] rewards = new Item[task.rewardIdsFromTask.Length];
//
//			for (int i = 0; i < task.rewardIdsFromTask.Length; i++) {
//
//				Item item = Item.NewItemWith(task.rewardIdsFromTask [i], task.rewardCountArray [i]);
//
//				rewards [i] = item;
//
//			}
//
//			return rewards;
//
//		}

		public override string ToString ()
		{
			return string.Format ("[NPC]:" + npcName);
		}


	}


	[System.Serializable]
	public class DialogGroup {

		public int accordGameLevel;
		public Dialog[] dialogs;

		public DialogGroup(int accordGameLevel,Dialog[] dialogs){
			this.accordGameLevel = accordGameLevel;
			this.dialogs = dialogs;
		}

	}

	[System.Serializable]
	public class Dialog {

		// 每步的情节
		public string dialog;

		public int dialogId;
		// 选择数组
		public Choice[] choices;

		public Task task;

		public int accordGameLevel;

		public int[] rewardIds;
		public int[] rewardCounts;

		public bool isEndingDialog;

		public Dialog(string dialog,int dialogId, Choice[] choices,bool isEndingDialog, Task task,int accordGameLevel,int[] rewardIds,int[] rewardCounts){
			this.dialog = dialog;
			this.dialogId = dialogId;
			this.choices = choices;
			this.isEndingDialog = isEndingDialog;
			this.task = task;
			this.accordGameLevel = accordGameLevel;
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

		public string choice;
		public int dialogId;
		public bool isEnd;
		public ChoiceTriggerType triggerType;


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
		KillMonster,
		HandInItem
	}

	[System.Serializable]
	public class Task{

		public string taskDescription;
		public TaskType taskType;
		public int accordMonsterOrItemId;
		public int accordMonsterOrItemCount;
		public int dialogIdWhenTaskAccomplished;


		public Task(string taskDescription,TaskType taskType,int accordMonsterOrItemId,int accordMonsterOrItemCount,int dialogIdWhenTaskAccomplished){
			this.taskDescription = taskDescription;
			this.taskType = taskType;
			this.accordMonsterOrItemId = accordMonsterOrItemId;
			this.accordMonsterOrItemCount = accordMonsterOrItemCount;
			this.dialogIdWhenTaskAccomplished = dialogIdWhenTaskAccomplished;
		}

	}
}