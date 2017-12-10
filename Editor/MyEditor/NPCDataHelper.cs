using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	using UnityEditor;
	using System;
	using System.IO;

	public class NPCDataHelper  {

		[System.Serializable]
		private class MyNPCs
		{
			public NPC[] Items;
		}


		[MenuItem("EditHelper/NPCDataHelper")]
		public static void TransferNpcData(){

			string NPCsDataDirectoryPath = "/Users/houlianghong/Desktop/MyGameData/NPCs";

			DirectoryInfo di = new DirectoryInfo (NPCsDataDirectoryPath);

			FileInfo[] fiArray = di.GetFiles ();

			List<NPC> npcs = new List<NPC> ();

			for (int i = 0; i < fiArray.Length; i++) {
				
				string npcDataPath = fiArray [i].FullName;

				NPC npc = LoadNpcDataFromFile (npcDataPath);

				npcs.Add (npc);
			}

			string newNpcDataPath = string.Format("{0}/AllNpcsData.json",CommonData.originDataPath);

			MyNPCs myNpcs = new MyNPCs ();
			myNpcs.Items = npcs.ToArray ();

			DataHandler.SaveInstanceDataToFile<MyNPCs> (myNpcs, newNpcDataPath);

		}

		public static NPC LoadNpcDataFromFile(string npcDataPath){

			List<int> existGameLevels = new List<int> ();

			string npcDataString = DataHandler.LoadDataString (npcDataPath);

			string[] npcsDataStringArray = npcDataString.Split (new string[]{ "\n" }, System.StringSplitOptions.RemoveEmptyEntries);

			List<Dialog> dialogs = new List<Dialog> ();

			NPC npc = new NPC();

			for (int i = 1; i < npcsDataStringArray.Length; i++) {

				string[] npcDatas = npcsDataStringArray [i].Split (new char[]{ ',' }, System.StringSplitOptions.RemoveEmptyEntries);

				Dialog dialog = null;
				List<Choice> choices = new List<Choice> ();
				Task task = null;

				npc.npcId = Convert.ToInt16(npcDatas[0]);

				npc.npcName = npcDatas [1];
				npc.spriteName = npcDatas [2];
				npc.attachedFunction = (NPCAttachedFunctionType)(Convert.ToInt16 (npcDatas[3]));


				string dialogString = npcDatas [4].Replace('_',',');
				int dialogId = Convert.ToInt16 (npcDatas [5]);
				int accordGameLevel = Convert.ToInt16 (npcDatas [6]);
				int existGameLevel = existGameLevels.Find (delegate(int obj) {
					return obj == accordGameLevel;
				});

				if (existGameLevel == 0) {
					existGameLevels.Add (accordGameLevel);
				}

				int[] rewardItemIds = null;

				string rewardItemIdsString = npcDatas [20];
				if (rewardItemIdsString != "null") {
					string[] rewardItemIdsArray = rewardItemIdsString.Split (new char[]{ '_' }, StringSplitOptions.RemoveEmptyEntries);
					rewardItemIds = new int[rewardItemIdsArray.Length];
					for (int j = 0; j < rewardItemIdsArray.Length; j++) {
						rewardItemIds [j] = Convert.ToInt16 (rewardItemIdsArray [j]);
					}
				}

				int[] rewardItemCount = null;

				string rewardItemCountString = npcDatas [21].Replace ("\r", "");
				if (rewardItemCountString != "null") {
					string[] rewardCountArray = rewardItemCountString.Split (new char[]{ '_' }, StringSplitOptions.RemoveEmptyEntries);
					rewardItemCount = new int[rewardCountArray.Length];
					for (int j = 0; j < rewardCountArray.Length; j++) {
						rewardItemCount [j] = Convert.ToInt16 (rewardCountArray [j]);
					}
				}

				ChoiceTriggerType choiceTriggerType = (ChoiceTriggerType)(Convert.ToInt16 (npcDatas[14]));

				string choice1 = npcDatas [7].Replace('_',',');
				int dialogIdAccordToChoice1 = -1;
				bool choice1EndDialog = false;


				if (choice1 != "null") {
					dialogIdAccordToChoice1 = Convert.ToInt16 (npcDatas [8]);
					choice1EndDialog = Convert.ToBoolean (npcDatas [9]);
					Choice choice = new Choice (choice1, dialogIdAccordToChoice1, choice1EndDialog, choiceTriggerType);
					choices.Add (choice);

				}

				string choice2 = npcDatas [10].Replace('_',',');
				int dialogIdAccordToChoice2 = -1;
				bool choice2EndDialog = false;
				if (choice2 != "null") {
					dialogIdAccordToChoice2 = Convert.ToInt16 (npcDatas [11]);
					choice2EndDialog = Convert.ToBoolean (npcDatas [12]);
					Choice choice = new Choice (choice2, dialogIdAccordToChoice2, choice2EndDialog, choiceTriggerType);
					choices.Add (choice);
				}

				bool isEndingDialog = Convert.ToBoolean(npcDatas [13]);

				if (choiceTriggerType == ChoiceTriggerType.Task) {
					string taskDescription= npcDatas [15];
					TaskType taskType = (TaskType)(Convert.ToInt16 (npcDatas [16]));
					int accordId = Convert.ToInt16 (npcDatas [17]);
					int accordCount = Convert.ToInt16 (npcDatas [18]);
					int dialogIdWhenTaskAccomplished = Convert.ToInt16 (npcDatas [19]);
					task = new Task (taskDescription, taskType, accordId, accordCount, dialogIdWhenTaskAccomplished);

				}


				dialog = new Dialog (dialogString, dialogId, choices.ToArray (), isEndingDialog, task, accordGameLevel, rewardItemIds, rewardItemCount);

				dialogs.Add (dialog);

			}

			DialogGroup[] dialogGroups = new DialogGroup[existGameLevels.Count];

			for (int i = 0; i < existGameLevels.Count; i++) {

				int accordLevel = existGameLevels [i];

				Dialog[] dialogsInGroup = dialogs.FindAll (delegate(Dialog obj) {
					return obj.accordGameLevel == accordLevel;
				}).ToArray ();

				DialogGroup dg = new DialogGroup (accordLevel, dialogsInGroup);

				dialogGroups [i] = dg;

			}

			npc.dialogGroups = dialogGroups;

			return npc;
		}

	}
}
