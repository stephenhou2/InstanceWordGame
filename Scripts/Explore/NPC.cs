using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	[System.Serializable]
	public class NPC  {

		public int npcId;

		public string npcName;

		public string spriteName;

		public DialogGroup[] dialogGroups;

		public Choice[] choices;

		public int[] rewardsIdsFromDialog;

		public int[] rewardCounts;

		public Task task;


		/// <summary>
		/// 获取从对话中得到的奖励物品
		/// </summary>
		/// <returns>The rewards from dialog.</returns>
		public Item[] GetRewardsFromDialog(){

			Item[] rewards = new Item[rewardsIdsFromDialog.Length];

			for (int i = 0; i < rewardsIdsFromDialog.Length; i++) {

				Item item = Item.NewItemWith(rewardsIdsFromDialog [i], rewardCounts [i]);

				rewards [i] = item;

			}

			return rewards;
		}

		/// <summary>
		/// 获得从任务中获得的奖励物品
		/// </summary>
		/// <returns>The rewards from task.</returns>
		public Item[] GetRewardsFromTask(){

			Item[] rewards = new Item[task.rewardsIdsFromTask.Length];

			for (int i = 0; i < task.rewardsIdsFromTask.Length; i++) {

				Item item = Item.NewItemWith(task.rewardsIdsFromTask [i], task.rewardCounts [i]);

				rewards [i] = item;

			}

			return rewards;

		}

		public override string ToString ()
		{
			return string.Format ("[NPC]:" + npcName);
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


	public enum ChoiceTriggerType{
		Plot,
		Fight,
		Magic
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