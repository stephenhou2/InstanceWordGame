using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEditor;
	using System;

	public class NPCEditor : EditorWindow {

		private GUILayoutOption[] labelLayouts = new GUILayoutOption[]{ GUILayout.Height(20), GUILayout.Width(100)};

		private GUILayoutOption[] normalInputLayouts = new GUILayoutOption[]{GUILayout.Height(20), GUILayout.Width(60)};

		private GUILayoutOption[] textInputLayouts = new GUILayoutOption[]{ GUILayout.Height(40), GUILayout.Width(600)};

		private GUILayoutOption[] toggleLayouts =  new GUILayoutOption[]{GUILayout.Height(30),GUILayout.Width(200)};


		// npc的id
		public int npcId;
		// npc的名字
		public string npcName;
		// npc的图片名称
		public string spriteName;
		// npc的打招呼文字
		public string greetingDialog;
		// npc的附加功能数组
		public NPCAttachedFunctionType[] attachedFunctions;

		private bool skillPromotionFunction;
		private bool propertyPromotionFunction;
		private bool taskFunction;
		private bool characterTradeFunction;


		// npc能发布的所有任务数组
		private List<Task> tasks = new List<Task>();
		// npc的对话组
		private List<DialogGroup> dialogGroups = new List<DialogGroup>();

		private List<bool> dialogGroupFoldoutList = new List<bool> ();
		private List<List<bool>> dialogFoldoutList = new List<List<bool>>();
		private List<int> dialogIdList = new List<int> ();
		private List<List<int>> rewardTypeCountList = new List<List<int>>();
		 

//		private int dialogId = -1;

		private Vector2 scrollPos;


		[MenuItem("EditHelper/NPCEditor")]
		public static void ShowNPCEditor(){
//			Rect editorRect = new Rect (new Vector2 (500, 500), new Vector2 (500, 800));
//			NPCEditor npcEditor = (NPCEditor)EditorWindow.GetWindowWithRect (typeof(NPCEditor), editorRect, true, "NPC Editor");
			NPCEditor npcEditor = EditorWindow.GetWindow<NPCEditor>(true,"NPC Editor");
			npcEditor.Show ();
		}


		public void OnGUI(){


			NPCEditor npcEditor = EditorWindow.GetWindow<NPCEditor>(true,"NPC Editor");

			Rect windowRect = npcEditor.position;

			scrollPos = EditorGUILayout.BeginScrollView (scrollPos,new GUILayoutOption[]{
				GUILayout.Height(windowRect.height),
				GUILayout.Width(windowRect.width)
			});

			EditorGUILayout.LabelField ("please input information of npc");


			CreateNPCBaseInformationGUI();

			EditorGUILayout.BeginHorizontal ();
			bool createNewDialogGroup = GUILayout.Button ("Create New DialogGroup", new GUILayoutOption[]{
				GUILayout.Height(20),
				GUILayout.Width(200)
			});

			bool removeLastDialogGroup = GUILayout.Button("Remove Last DialogGroup", new GUILayoutOption[]{
				GUILayout.Height(20),
				GUILayout.Width(200)
			});

			EditorGUILayout.EndHorizontal();

			if (createNewDialogGroup) {
				DialogGroup dg = new DialogGroup ();
				dialogGroups.Add (dg);
				bool foldout = true;
				dialogGroupFoldoutList.Add (foldout);
				List<bool> foldoutList = new List<bool> ();
				dialogFoldoutList.Add (foldoutList);
				List<int> rewardIdsCount = new List<int> ();
				rewardTypeCountList.Add (rewardIdsCount);
			}

			if (removeLastDialogGroup && dialogGroups.Count > 0) {
				dialogGroups.RemoveAt (dialogGroups.Count - 1);
				dialogGroupFoldoutList.RemoveAt (dialogGroupFoldoutList.Count - 1);
				dialogFoldoutList.RemoveAt (dialogFoldoutList.Count - 1);
			}

			for (int i = 0; i < dialogGroups.Count; i++) {

				DialogGroup dg = dialogGroups [i];

				List<bool> currentDialogFoldoutList = dialogFoldoutList [i];

				List<int> currentDialogRewardTypeCountList = rewardTypeCountList [i];

				dialogGroupFoldoutList [i] = EditorGUILayout.Foldout (dialogGroupFoldoutList [i], "Dialog Group Content");

				if (dialogGroupFoldoutList [i]) {
					CreateDialogGroupGUI (dg, currentDialogFoldoutList,currentDialogRewardTypeCountList);
				}
			}

			EditorGUILayout.EndScrollView ();
		}


		private void CreateNPCBaseInformationGUI(){

			EditorGUILayout.BeginHorizontal ();

			EditorGUILayout.LabelField ("ID:",labelLayouts);

			npcId = EditorGUILayout.IntField (npcId,normalInputLayouts);

			GUILayout.EndHorizontal ();

			// name 编辑行
			EditorGUILayout.BeginHorizontal ();

			EditorGUILayout.LabelField("Name:",labelLayouts);

			npcName = EditorGUILayout.TextField (npcName, normalInputLayouts);

			EditorGUILayout.EndHorizontal ();

			// npc图片名称编辑行
			EditorGUILayout.BeginHorizontal ();

			EditorGUILayout.LabelField("Sprite Name:",labelLayouts);

			spriteName = EditorGUILayout.TextField (spriteName, normalInputLayouts);

			EditorGUILayout.EndHorizontal ();

			// npc打招呼文字编辑行
			EditorGUILayout.LabelField("Greeting Dialog:",labelLayouts);

			greetingDialog = EditorGUILayout.TextArea (greetingDialog , textInputLayouts);

			// npc附加功能编辑组
			EditorGUILayout.LabelField ("NPC Attached Func:", labelLayouts);

			skillPromotionFunction = EditorGUILayout.Toggle ("Skill Promotion", skillPromotionFunction, toggleLayouts);

			propertyPromotionFunction = EditorGUILayout.Toggle ("Property Promotion", propertyPromotionFunction, toggleLayouts);

			taskFunction = EditorGUILayout.Toggle ("Task", taskFunction, toggleLayouts);

			characterTradeFunction = EditorGUILayout.Toggle ("Character Trade", characterTradeFunction, toggleLayouts);
		}

		private void CreateDialogGroupGUI(DialogGroup dg,List<bool> foldoutList,List<int> rewardTypeCountList){

			// 对话组对应关卡id编辑行
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Accord Game Level:", labelLayouts);
			dg.accordGameLevel = EditorGUILayout.IntField (dg.accordGameLevel, normalInputLayouts);
			EditorGUILayout.EndHorizontal ();


			List<Dialog> dialogs = dg.dialogs;

			// 对话组内部对话编辑区
			EditorGUILayout.BeginHorizontal ();
			bool createNewDialog = GUILayout.Button ("Create New Dialog", new GUILayoutOption[]{
				GUILayout.Height(20),
				GUILayout.Width(200)
			});

			bool removeLastDialog = GUILayout.Button("Remove Last Dialog", new GUILayoutOption[]{
				GUILayout.Height(20),
				GUILayout.Width(200)
			});

			EditorGUILayout.EndHorizontal();

			if (createNewDialog) {
				Dialog dialog = new Dialog ();
				dialogs.Add (dialog);
				bool foldout = true;
				foldoutList.Add(foldout);
				int rewardIdCount = 0;
				rewardTypeCountList.Add (rewardIdCount);
				dialog.choices = new List<Choice> {new Choice (), new Choice ()};
			}

			if (removeLastDialog && dialogs.Count > 0) {
				dialogs.RemoveAt (dialogs.Count - 1);
				foldoutList.RemoveAt (foldoutList.Count - 1);
			}

			for (int i = 0; i < dialogs.Count; i++) {

				Dialog dialog = dialogs [i];

				int dialogGroupIndex = dialogGroups.FindIndex (delegate(DialogGroup obj) {
					return obj == dg;
				});

				int dialogIndex = 0;

				for (int j = 0; j < dialogGroupIndex; j++) {
					dialogIndex += dialogGroups [j].dialogs.Count;
				}

				dialogIndex += i;

				dialog.dialogId = dialogIndex;

				foldoutList [i] = EditorGUILayout.Foldout(foldoutList [i],"Dialog Content");

				if (foldoutList [i]) {
					// 对话id编辑行
					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField ("Dialog ID:", labelLayouts);
					EditorGUILayout.LabelField (dialog.dialogId.ToString (), normalInputLayouts);
//					dialog.dialogId = EditorGUILayout.IntField (dialog.dialogId, normalInputLayouts);
					EditorGUILayout.EndHorizontal ();

					// 对话文字编辑行
					EditorGUILayout.LabelField ("Dialog:", labelLayouts);
					dialog.dialog = EditorGUILayout.TextArea (dialog.dialog, textInputLayouts);


					CreateChoiceGUI (dialog);

					// 奖励物品信息编辑区
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField ("奖励物品的种类：", labelLayouts);

					rewardTypeCountList[i] = EditorGUILayout.IntField (rewardTypeCountList[i], normalInputLayouts);

					EditorGUILayout.EndHorizontal ();

					dialog.rewardIds = new int[rewardTypeCountList[i]];
					dialog.rewardCounts = new int[rewardTypeCountList [i]];

					for (int j = 0; j < rewardTypeCountList[i]; j++) {
						EditorGUILayout.BeginHorizontal ();
						int rewardId = -1;
						rewardId = EditorGUILayout.IntField ("奖励物品的id", rewardId, new GUILayoutOption[]{
							GUILayout.Height(20),
							GUILayout.Width(200)
						});
						dialog.rewardIds [j] = rewardId;
						int rewardCount = -1;
						rewardCount = EditorGUILayout.IntField ("奖励物品的数量",rewardCount, new GUILayoutOption[]{
							GUILayout.Height(20),
							GUILayout.Width(200)
						});
						dialog.rewardCounts [j] = rewardCount;
						EditorGUILayout.EndHorizontal ();
					}


					// 对话是否作为结束对话编辑行
					dialog.isEndingDialog = EditorGUILayout.Toggle ("Is Ending Dialog", dialog.isEndingDialog, toggleLayouts);
				}

			}

		}


		private void CreateChoiceGUI(Dialog dialog){

			List<Choice> choices = dialog.choices;

			EditorGUILayout.BeginHorizontal ();

			EditorGUILayout.BeginVertical ();

			EditorGUILayout.LabelField ("选择1:", labelLayouts);

			Choice choice1 = choices [0];

			choice1.choice = EditorGUILayout.TextField (choice1.choice, new GUILayoutOption[]{
				GUILayout.Height(20),
				GUILayout.Width(150)
			});

			if (choice1.choice != null && choice1.choice != string.Empty) {
				EditorGUILayout.LabelField ("选择1对应的对话id", labelLayouts);
				choice1.dialogId = EditorGUILayout.IntField (choice1.dialogId, normalInputLayouts);
				EditorGUILayout.LabelField ("选择1是否结束对话", labelLayouts);
				choice1.isEnd = EditorGUILayout.Toggle (choice1.isEnd, toggleLayouts);
				EditorGUILayout.LabelField ("选择1触发事件类型", labelLayouts);
				choice1.triggerType = (ChoiceTriggerType)EditorGUILayout.EnumPopup (choice1.triggerType, toggleLayouts);
			}

			EditorGUILayout.EndVertical ();

			EditorGUILayout.BeginVertical ();

			EditorGUILayout.LabelField ("选择2：", labelLayouts);

			Choice choice2 = choices [1];

			choice2.choice = EditorGUILayout.TextField (choice2.choice, new GUILayoutOption[]{
				GUILayout.Height(20),
				GUILayout.Width(150)
			});
			if (choice2.choice != null && choice1.choice != string.Empty) {
				EditorGUILayout.LabelField ("选择2对应的对话id", labelLayouts);
				choice2.dialogId = EditorGUILayout.IntField (choice2.dialogId, normalInputLayouts);
				EditorGUILayout.LabelField ("选择2是否结束对话", labelLayouts);
				choice2.isEnd = EditorGUILayout.Toggle (choice2.isEnd, toggleLayouts);
				EditorGUILayout.LabelField ("选择2触发事件类型", labelLayouts);
				choice2.triggerType = (ChoiceTriggerType)EditorGUILayout.EnumPopup (choice2.triggerType, toggleLayouts);
			} 

			EditorGUILayout.EndVertical ();

			EditorGUILayout.EndHorizontal ();



		}



	}
}
