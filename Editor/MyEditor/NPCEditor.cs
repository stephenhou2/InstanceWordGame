using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEditor;
	using System;
	using System.IO;

	public class MyNpc:NPC{
		public List<GoodsGroup> goodsGroupList = new List<GoodsGroup>();
	}

	public class NPCEditor : EditorWindow {


		private GUILayoutOption[] labelLayouts = new GUILayoutOption[]{ GUILayout.Height(20), GUILayout.Width(100)};

		private GUILayoutOption[] normalInputLayouts = new GUILayoutOption[]{GUILayout.Height(20), GUILayout.Width(60)};

		private GUILayoutOption[] textInputLayouts = new GUILayoutOption[]{ GUILayout.Height(40), GUILayout.Width(600)};

		private GUILayoutOption[] toggleLayouts =  new GUILayoutOption[]{GUILayout.Height(30),GUILayout.Width(200)};

		private MyNpc npc;

//		private Trader trader;

		private NPCType npcType;

		// npc的id
//		public int npcId;
//		// npc的名字
//		public string npcName;
//		// npc的图片名称
//		public string spriteName;
//		// npc的打招呼文字
//		public string greetingDialog;
//		// npc的附加功能数组
//		public NPCAttachedFunctionType[] attachedFunctions;

		private bool skillPromotionFunction;
		private bool propertyPromotionFunction;
		private bool taskFunction;
		private bool characterTradeFunction;


		// npc能发布的所有任务数组
//		private List<Task> tasks = new List<Task>();
		// npc的对话组
		private List<DialogGroup> dialogGroups = new List<DialogGroup>();

//		private List<DialogGroup> dialogGroupsInTask = new List<DialogGroup> ();

		private List<bool> dialogGroupFoldoutList = new List<bool> ();
		private List<List<bool>> dialogFoldoutList = new List<List<bool>> ();
		private List<List<int>> rewardTypeCountList = new List<List<int>>();
		private List<bool> taskFoldoutList = new List<bool> ();
		private List<bool> goodsGroupFoldoutList = new List<bool> ();
	

		private Vector2 scrollPos;

//		private bool loadNpcData;
//		private bool saveNpcData;

		private string npcDataPath;
		private Rect rect;



		private static NPCEditor mInstance;

		public static NPCEditor npcEditor{

			get{
				if (mInstance == null) {
					mInstance = ScriptableObject.CreateInstance<NPCEditor> ();
					mInstance.ShowUtility ();
				}

				return mInstance;
			}


		}



		[MenuItem("EditHelper/NPCEditor")]
		public static void ShowNPCEditor(){
//			Rect editorRect = new Rect (new Vector2 (500, 500), new Vector2 (500, 800));
//			NPCEditor npcEditor = (NPCEditor)EditorWindow.GetWindowWithRect (typeof(NPCEditor), editorRect, true, "NPC Editor");
//			NPCEditor npcEditor = ScriptableObject.CreateInstance<NPCEditor> ();
//			npcEditor.ShowUtility ();

			NPCEditor editor = NPCEditor.npcEditor;
			editor.npc = new MyNpc ();

		}


		private List<DialogGroup> chatDialogGroups = new List<DialogGroup>();
		private List<DialogGroup> taskDialogGroups =new List<DialogGroup>();



		public void OnGUI(){

			Rect windowRect = npcEditor.position;

			scrollPos = EditorGUILayout.BeginScrollView (scrollPos,new GUILayoutOption[]{
				GUILayout.Height(windowRect.height),
				GUILayout.Width(windowRect.width)
			});

			EditorGUILayout.LabelField ("please input information of npc");

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("npc数据源",labelLayouts);

			rect = EditorGUILayout.GetControlRect(GUILayout.Width(300));
			npcDataPath = EditorGUI.TextField(rect, npcDataPath);
			//如果鼠标正在拖拽中或拖拽结束时，并且鼠标所在位置在文本输入框内
			if ((UnityEngine.Event.current.type == UnityEngine.EventType.dragUpdated
				|| UnityEngine.Event.current.type == UnityEngine.EventType.DragExited)
				&& rect.Contains (UnityEngine.Event.current.mousePosition)) {
				//改变鼠标的外表
				DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
				if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0) {
					npcDataPath = DragAndDrop.paths [0];
				}
			}

		
			EditorGUILayout.EndHorizontal ();

			bool loadNpcData = GUILayout.Button ("加载npc数据", new GUILayoutOption[] {
				GUILayout.Height (20),
				GUILayout.Width (150)
			});

			if (loadNpcData) {

				dialogGroups.Clear ();
				chatDialogGroups.Clear ();
				taskDialogGroups.Clear ();

				dialogGroupFoldoutList.Clear ();
				dialogFoldoutList.Clear ();
				rewardTypeCountList.Clear ();
				taskFoldoutList.Clear ();

				string npcDataFileName = Path.GetFileName (npcDataPath);

				npcDataPath = string.Format ("{0}/NPCs/{1}",CommonData.originDataPath, npcDataFileName);

				string npcTypeStr = npcDataFileName.Split (new char[]{ '_' }, StringSplitOptions.RemoveEmptyEntries)[0];


				npc = DataHandler.LoadDataToSingleModelWithPath<MyNpc> (npcDataPath);

				switch (npcTypeStr) {
				case "Normal":
					npcType = NPCType.Normal;
						break;
				case "Trader":
					npcType = NPCType.Trader;
					break;
				}

				Debug.Log (npcDataPath);



				for (int i = 0; i < npc.chatDialogGroups.Count; i++) {
					dialogGroups.Add (npc.chatDialogGroups [i]);
					chatDialogGroups.Add (npc.chatDialogGroups [i]);

					bool foldout = false;
					dialogGroupFoldoutList.Add (foldout);

					List<bool> foldoutList = new List<bool> ();
					dialogFoldoutList.Add (foldoutList);

					List<int> rewardTypeCounts = new List<int> ();
					rewardTypeCountList.Add (rewardTypeCounts);

					for (int j = 0; j < npc.chatDialogGroups [i].dialogs.Count; j++) {
						Dialog d = npc.chatDialogGroups [i].dialogs [j];
						bool dialogFoldout = false;
						foldoutList.Add (dialogFoldout);

						rewardTypeCounts.Add (d.rewardIds.Length);

					}
				}

				for (int i = 0; i < npc.tasks.Count; i++) {
					dialogGroups.Add (npc.tasks [i].taskDialogGroup);
					bool foldout = false;
					taskFoldoutList.Add (foldout);
					dialogGroupFoldoutList.Add (dialogGroupFoldout);
					List<bool> foldoutList = new List<bool> ();
					dialogFoldoutList.Add (foldoutList);
					List<int> rewardTypeCounts = new List<int> ();
					rewardTypeCountList.Add (rewardTypeCounts);

					for (int j = 0; j < npc.tasks [i].taskDialogGroup.dialogs.Count; j++) {

						Dialog d = npc.tasks [i].taskDialogGroup.dialogs [j];
						bool dialogFoldout = false;
						foldoutList.Add (dialogFoldout);

						rewardTypeCounts.Add (d.rewardIds.Length);
					}
				}

				for (int i = 0; i < npc.attachedFunctions.Length; i++) {
					if (npc.attachedFunctions [i] == NPCAttachedFunctionType.SkillPromotion) {
						skillPromotionFunction = true;
					}
					if (npc.attachedFunctions [i] == NPCAttachedFunctionType.PropertyPromotion) {
						propertyPromotionFunction = true;
					}
					if (npc.attachedFunctions [i] == NPCAttachedFunctionType.Task) {
						taskFunction = true;
					}
					if (npc.attachedFunctions [i] == NPCAttachedFunctionType.CharactersTrade) {
						characterTradeFunction = true;
					}
				}
			}

			CreateNPCBaseInformationGUI();



			EditorGUILayout.BeginHorizontal ();
			bool createNewDialogGroup = GUILayout.Button ("新建对话组", new GUILayoutOption[]{
				GUILayout.Height(20),
				GUILayout.Width(200)
			});

			bool removeLastDialogGroup = GUILayout.Button("删除尾部对话组", new GUILayoutOption[]{
				GUILayout.Height(20),
				GUILayout.Width(200)
			});

			EditorGUILayout.EndHorizontal();

			if (createNewDialogGroup) {
				DialogGroup chatDialogGroup = new DialogGroup ();
				dialogGroups.Add (chatDialogGroup);
				bool foldout = true;
				dialogGroupFoldoutList.Add (foldout);
				List<bool> foldoutList = new List<bool> ();
				dialogFoldoutList.Add (foldoutList);
				List<int> rewardTypeCounts = new List<int> ();
				rewardTypeCountList.Add (rewardTypeCounts);
				ReCalculateDialogGroups ();
			}

			if (removeLastDialogGroup && chatDialogGroups.Count > 0) {
				DialogGroup dg = chatDialogGroups [chatDialogGroups.Count - 1];

				int index = dialogGroups.FindIndex (delegate(DialogGroup obj) {
					return obj == dg;
				});
				dialogGroupFoldoutList.RemoveAt (index);
				dialogFoldoutList.RemoveAt (index);
				rewardTypeCountList.RemoveAt (index);

				chatDialogGroups.Remove (dg);
				dialogGroups.Remove (dg);

				ReCalculateDialogGroups ();
			}




			for (int i = 0; i < chatDialogGroups.Count; i++) {

				DialogGroup dg = chatDialogGroups [i];

				int index = dialogGroups.FindIndex (delegate(DialogGroup obj) {
					return obj == dg;
				});

				List<bool> currentDialogFoldoutList = dialogFoldoutList [index];

				List<int> currentDialogRewardTypeCountList = rewardTypeCountList [index];

				dialogGroupFoldoutList [index] = EditorGUILayout.Foldout (dialogGroupFoldoutList [index], "对话组编辑区");

				if (dialogGroupFoldoutList [index]) {
					CreateDialogGroupGUI (dg, currentDialogFoldoutList,currentDialogRewardTypeCountList);
				}
			}


			EditorGUILayout.BeginHorizontal ();
			bool createNewTask = GUILayout.Button ("新建任务", new GUILayoutOption[]{
				GUILayout.Height(20),
				GUILayout.Width(200)
			});

			bool removeLastTask = GUILayout.Button("删除尾部任务", new GUILayoutOption[]{
				GUILayout.Height(20),
				GUILayout.Width(200)
			});

			EditorGUILayout.EndHorizontal();


			if (createNewTask) {
				Task task = new Task ();
				npc.tasks.Add (task);
				DialogGroup taskDialogGroup = new DialogGroup ();
				dialogGroups.Add (taskDialogGroup);
				task.taskDialogGroup = taskDialogGroup;
				bool foldout = true;
				taskFoldoutList.Add (foldout);
				dialogGroupFoldoutList.Add (dialogGroupFoldout);
				List<bool> foldoutList = new List<bool> ();
				dialogFoldoutList.Add (foldoutList);
				List<int> rewardIdsCount = new List<int> ();
				rewardTypeCountList.Add (rewardIdsCount);
				ReCalculateDialogGroups ();
			}

			if (removeLastTask && npc.tasks.Count > 0) {
				
				Task task = npc.tasks [npc.tasks.Count - 1];

				npc.tasks.RemoveAt (npc.tasks.Count - 1);

				taskFoldoutList.RemoveAt (taskFoldoutList.Count - 1);



				int index = dialogGroups.FindIndex (delegate(DialogGroup obj) {
					return obj == task.taskDialogGroup;
				});

				dialogGroupFoldoutList.RemoveAt (index);
				dialogFoldoutList.RemoveAt (index);
				rewardTypeCountList.RemoveAt (index);

				taskDialogGroups.Remove (task.taskDialogGroup);
				dialogGroups.Remove (task.taskDialogGroup);

				ReCalculateDialogGroups ();
			}

			for (int i = 0; i < npc.tasks.Count; i++) {
				Task task = npc.tasks [i];
				taskFoldoutList [i] = EditorGUILayout.Foldout (taskFoldoutList [i], "任务编辑区");
				if (taskFoldoutList [i]) {
					CreateTaskGUI (task);
				}
			}

//			EditorGUILayout.Separator ();
//			EditorGUILayout.LabelField ("************************结束*************************", new GUILayoutOption[] {
//				GUILayout.Height (20),
//				GUILayout.Width (600)
//			});
			bool saveNpcData =  GUILayout.Button ("保存NPC数据", new GUILayoutOption[] {
				GUILayout.Width (500),
				GUILayout.Height (20)
			});

			if (saveNpcData) {
				SaveNpcData ();
			}



			EditorGUILayout.EndScrollView ();
		}

		public enum NPCType
		{
			Normal,
			Trader
		}


		private void CreateNPCBaseInformationGUI(){


			EditorGUILayout.BeginHorizontal ();

			EditorGUILayout.LabelField ("npc的id:",labelLayouts);

			npc.npcId = EditorGUILayout.IntField (npc.npcId,normalInputLayouts);

			GUILayout.EndHorizontal ();

			// name 编辑行
			EditorGUILayout.BeginHorizontal ();

			EditorGUILayout.LabelField("npc名称:",labelLayouts);

			npc.npcName = EditorGUILayout.TextField (npc.npcName, normalInputLayouts);

			EditorGUILayout.EndHorizontal ();

			// npc图片名称编辑行
			EditorGUILayout.BeginHorizontal ();

			EditorGUILayout.LabelField("npc图片名称:",labelLayouts);

			npc.spriteName = EditorGUILayout.TextField (npc.spriteName, normalInputLayouts);

			EditorGUILayout.EndHorizontal ();

			// npc类型编辑行
			EditorGUILayout.BeginHorizontal ();

			EditorGUILayout.LabelField ("npc的类型：", labelLayouts);

			npcType = (NPCType)EditorGUILayout.EnumPopup (npcType, toggleLayouts);



			EditorGUILayout.EndHorizontal ();

			switch (npcType) {
			case NPCType.Normal:
				break;
			case NPCType.Trader:

				EditorGUILayout.BeginHorizontal ();

				bool addNewGoodsGroup = GUILayout.Button ("新增商品组：", new GUILayoutOption[] {
					GUILayout.Height (20),
					GUILayout.Width (100)
				});

				bool removeLastGoodsGroup = GUILayout.Button ("删除尾部商品组", new GUILayoutOption[] {
					GUILayout.Height (20),
					GUILayout.Width (100)
				});

				EditorGUILayout.EndHorizontal ();

				if (addNewGoodsGroup) {

					GoodsGroup goodsGroup = new GoodsGroup ();

					npc.goodsGroupList.Add (goodsGroup);

					bool goodsGroupFoldout = false;

					goodsGroupFoldoutList.Add (goodsGroupFoldout);

				}

				if (removeLastGoodsGroup && npc.goodsGroupList.Count > 0) {
					npc.goodsGroupList.RemoveAt (npc.goodsGroupList.Count - 1);
					goodsGroupFoldoutList.RemoveAt (goodsGroupFoldoutList.Count - 1);
				}

				for (int i = 0; i < npc.goodsGroupList.Count; i++) {

					GoodsGroup goodsGroup = npc.goodsGroupList [i];
						

					EditorGUILayout.BeginHorizontal ();

					EditorGUILayout.LabelField (string.Format ("商品组{0}对应的关卡", i), labelLayouts);

					goodsGroup.accordLevel = EditorGUILayout.IntField (goodsGroup.accordLevel, normalInputLayouts);

					EditorGUILayout.EndHorizontal ();

					EditorGUILayout.BeginHorizontal ();

					bool addNewGoods = GUILayout.Button ("新增商品", new GUILayoutOption[] {
						GUILayout.Height (20),
						GUILayout.Width (100)
					});

					bool removeLastGoods = GUILayout.Button ("删除尾部商品", new GUILayoutOption[] {
						GUILayout.Height (20),
						GUILayout.Width (100)
					});



					if (addNewGoods) {
						Goods goods = new Goods ();
						goodsGroup.goodsList.Add (goods);
					}

					if (removeLastGoods && goodsGroup.goodsList.Count > 0) {
						goodsGroup.goodsList.RemoveAt (goodsGroup.goodsList.Count - 1);
					}

					for (int j = 0; j < goodsGroup.goodsList.Count; j++) {

						Goods goods = goodsGroup.goodsList [j];

						EditorGUILayout.BeginVertical ();

						EditorGUILayout.BeginHorizontal ();

						EditorGUILayout.LabelField ("物品id：", new GUILayoutOption[] {
							GUILayout.Height(20),
							GUILayout.Width(50)
						});

						goods.goodsId = EditorGUILayout.IntField (goods.goodsId, normalInputLayouts);

						EditorGUILayout.EndHorizontal ();

						EditorGUILayout.BeginHorizontal ();
						EditorGUILayout.LabelField("物品价格：",new GUILayoutOption[] {
							GUILayout.Height(20),
							GUILayout.Width(50)
						});

						goods.goodsPrice = EditorGUILayout.IntField (goods.goodsPrice, normalInputLayouts);

						EditorGUILayout.EndHorizontal ();

						EditorGUILayout.EndVertical ();
					}

					EditorGUILayout.EndHorizontal ();
		
				}

				break;
			}

			// npc打招呼文字编辑行
			EditorGUILayout.LabelField("npc打招呼文字:",labelLayouts);

			npc.greetingDialog = EditorGUILayout.TextArea (npc.greetingDialog , textInputLayouts);

			// npc附加功能编辑组

			EditorGUILayout.LabelField ("npc附加功能编辑区:", labelLayouts);


			skillPromotionFunction = EditorGUILayout.Toggle ("技能提升", skillPromotionFunction, toggleLayouts);

			propertyPromotionFunction = EditorGUILayout.Toggle ("属性提升", propertyPromotionFunction, toggleLayouts);

			taskFunction = EditorGUILayout.Toggle ("任务", taskFunction, toggleLayouts);

			characterTradeFunction = EditorGUILayout.Toggle ("交易", characterTradeFunction, toggleLayouts);

		}

		private void CreateDialogGroupGUI(DialogGroup dg,List<bool> foldoutList,List<int> rewardTypeCountList){

			EditorGUILayout.Separator ();
			EditorGUILayout.LabelField ("************************对话组编辑区*************************", new GUILayoutOption[] {
				GUILayout.Height (20),
				GUILayout.Width (600)
			});

			// 对话组对应关卡id编辑行
			EditorGUILayout.BeginHorizontal ();

			EditorGUILayout.LabelField ("对话组对应关卡id:", labelLayouts);
			dg.accordGameLevel = EditorGUILayout.IntField (dg.accordGameLevel, normalInputLayouts);

			EditorGUILayout.EndHorizontal ();


			List<Dialog> dialogs = dg.dialogs;


			// 对话组内部对话编辑区
			EditorGUILayout.BeginHorizontal ();

			EditorGUILayout.PrefixLabel ("____");

			EditorGUILayout.BeginVertical ();

			EditorGUILayout.BeginHorizontal ();

			bool createNewDialog = GUILayout.Button ("新建对话", new GUILayoutOption[]{
				GUILayout.Height(20),
				GUILayout.Width(150)
			});

			bool removeLastDialog = GUILayout.Button("删除尾部对话", new GUILayoutOption[]{
				GUILayout.Height(20),
				GUILayout.Width(150)
			});

			EditorGUILayout.EndHorizontal ();

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

				foldoutList[i] = EditorGUILayout.Foldout(foldoutList[i],"对话编辑区");

				if (foldoutList [i]) {

					CreateDialogGUI (dialog, rewardTypeCountList, i);
				}

			}

			EditorGUILayout.EndVertical ();

			EditorGUILayout.EndHorizontal();

		
			EditorGUILayout.LabelField ("************************对话组编辑区*************************", new GUILayoutOption[] {
				GUILayout.Height (20),
				GUILayout.Width (600)
			});
			EditorGUILayout.Separator ();

		}

		private void CreateDialogGUI(Dialog dialog, List<int> rewardTypeCountList,int index){

			EditorGUILayout.Separator ();
			EditorGUILayout.LabelField ("++++++++++++++++++++++++对话编辑区++++++++++++++++++++++++++", new GUILayoutOption[] {
				GUILayout.Height (20),
				GUILayout.Width (600)
			});


			// 对话id编辑行
			EditorGUILayout.LabelField ("对话id:", labelLayouts);
			EditorGUILayout.LabelField (dialog.dialogId.ToString (), normalInputLayouts);

			// 对话文字编辑行
			EditorGUILayout.LabelField ("对话文字:", labelLayouts);
			dialog.dialog = EditorGUILayout.TextArea (dialog.dialog, textInputLayouts);


			CreateChoiceGUI (dialog);

			// 奖励物品信息编辑区
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField ("奖励物品的种类：", labelLayouts);

			rewardTypeCountList[index] = EditorGUILayout.IntField (rewardTypeCountList[index], normalInputLayouts);

			EditorGUILayout.EndHorizontal ();

			dialog.rewardIds = new int[rewardTypeCountList[index]];
			dialog.rewardCounts = new int[rewardTypeCountList[index]];

			for (int j = 0; j < rewardTypeCountList[index]; j++) {
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
			dialog.isEndingDialog = EditorGUILayout.Toggle ("是否是对话组最后一句对话", dialog.isEndingDialog, new GUILayoutOption[]{
				GUILayout.Height(20),
				GUILayout.Width(200)
			});


			EditorGUILayout.LabelField ("++++++++++++++++++++++++对话编辑区++++++++++++++++++++++++++", new GUILayoutOption[] {
				GUILayout.Height (20),
				GUILayout.Width (600)
			});
			EditorGUILayout.Separator ();


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

		private bool dialogGroupFoldout = true;

		private void CreateTaskGUI(Task task){

			EditorGUILayout.Separator ();
			EditorGUILayout.LabelField ("^^^^^^^^^^^^^^^^^^^^任务编辑区^^^^^^^^^^^^^^^^^^^^^^", new GUILayoutOption[] {
				GUILayout.Height (20),
				GUILayout.Width (600)
			});

			EditorGUILayout.LabelField ("任务描述文字：", labelLayouts);
			task.taskDescription = EditorGUILayout.TextArea (task.taskDescription, textInputLayouts);

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("任务类型：", labelLayouts);
			task.taskType = (TaskType)(EditorGUILayout.EnumPopup(task.taskType,toggleLayouts));
			EditorGUILayout.EndHorizontal ();


			int index = dialogGroups.FindIndex (delegate(DialogGroup obj) {
				return obj == task.taskDialogGroup;
			});

			List<bool> currentDialogFoldoutList = dialogFoldoutList [index];

			List<int> currentDialogRewardTypeCountList = rewardTypeCountList [index];

			dialogGroupFoldoutList [index] = EditorGUILayout.Foldout (dialogGroupFoldoutList [index], "对话组编辑区");

			if (dialogGroupFoldoutList [index]) {
				CreateDialogGroupGUI (task.taskDialogGroup, currentDialogFoldoutList,currentDialogRewardTypeCountList);
			}

			EditorGUILayout.BeginHorizontal ();
			task.accordGameLevel = task.taskDialogGroup.accordGameLevel;
			EditorGUILayout.LabelField ("任务对应的关卡id：", labelLayouts);
			EditorGUILayout.LabelField (task.accordGameLevel.ToString(), labelLayouts);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("任务对应的怪物或物品id：", new GUILayoutOption[]{
				GUILayout.Height(20),
				GUILayout.Width(200)
			});
			task.accordMonsterOrItemId = EditorGUILayout.IntField (task.accordMonsterOrItemId, normalInputLayouts);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("任务对应的怪物或物品数量：", new GUILayoutOption[]{
				GUILayout.Height(20),
				GUILayout.Width(200)
			});
			task.accordMonsterOrItemCount = EditorGUILayout.IntField (task.accordMonsterOrItemCount, normalInputLayouts);
			EditorGUILayout.EndHorizontal ();


			EditorGUILayout.LabelField ("^^^^^^^^^^^^^^^^^^^^任务编辑区^^^^^^^^^^^^^^^^^^^^^^", new GUILayoutOption[] {
				GUILayout.Height (20),
				GUILayout.Width (600)
			});
			EditorGUILayout.Separator ();
		}


		private void ReCalculateDialogGroups(){

			chatDialogGroups.Clear ();
			taskDialogGroups.Clear ();

			for (int i = 0; i < dialogGroups.Count; i++) {

				DialogGroup dg = dialogGroups [i];

				bool normalDg = true;

				for (int j = 0; j < npc.tasks.Count; j++) {
					if (!normalDg) {
						break;
					}
					normalDg = normalDg && dg != npc.tasks [j].taskDialogGroup;
				}

				if (!normalDg) {
					taskDialogGroups.Add (dg);
				} else {
					chatDialogGroups.Add (dg);
				}

			}
		}


		private void SaveNpcData(){
			
			NPC npcData = null;

			string prefix = string.Empty;

			switch (npcType) {
			case NPCType.Normal:
				prefix = "Normal";
				npcData = new NPC ();

				break;
			case NPCType.Trader:
				prefix = "Trader";
				npcData = new Trader ();
				(npcData as Trader).goodsGroupList = npc.goodsGroupList;
				break;
			}

			npcData.npcId = npc.npcId;
			npcData.npcName = npc.npcName;
			npcData.spriteName = npc.spriteName;
			npcData.greetingDialog = npc.greetingDialog;
			npcData.attachedFunctions = npc.attachedFunctions;
			npcData.tasks = npc.tasks;

			npcData.chatDialogGroups = chatDialogGroups;

			int attachedFunctionCount = (skillPromotionFunction ? 1 : 0) + 
				(propertyPromotionFunction ? 1 : 0) + 
				(taskFunction ? 1 : 0) + 
				(characterTradeFunction ? 1 : 0);

			npcData.attachedFunctions = new NPCAttachedFunctionType[attachedFunctionCount];

			int index = 0;

			if (skillPromotionFunction) {
				npcData.attachedFunctions [index] = NPCAttachedFunctionType.SkillPromotion;
				index++;
			}

			if (propertyPromotionFunction) {
				npcData.attachedFunctions [index] = NPCAttachedFunctionType.PropertyPromotion;
				index++;
			}

			if (taskFunction) {
				npcData.attachedFunctions [index] = NPCAttachedFunctionType.Task;
				index++;
			}

			if (characterTradeFunction) {
				npcData.attachedFunctions [index] = NPCAttachedFunctionType.CharactersTrade;
			}


			string npcJson = string.Empty;


			npcJson = EditorJsonUtility.ToJson (npcData);

			string npcFilePath = string.Format ("{0}/NPCs/{1}_{2}.json", CommonData.originDataPath, prefix, npc.npcName);


			File.WriteAllText (npcFilePath, npcJson);



		}

	}
}
