using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.EventSystems;


namespace WordJourney
{
	using System.Collections.Generic;		//Allows us to use Lists. 
	using UnityEngine.UI;					//Allows us to use UI.

	public class ExploreManager : MonoBehaviour
	{
		// 地图生成器
		private MapGenerator mapGenerator;						

		// 当前地图id
		private int currentMapIndex = 0;	
	
		// 当前关卡所有怪物
//		private List<BattleMonsterController> battleMonsters = new List<BattleMonsterController>();	

		// 当前碰到的怪物控制器
		private BattleMonsterController battleMonsterCtr;

		// 玩家控制器
		private BattlePlayerController battlePlayerCtr;

		private NavigationHelper navHelper;

		private List<Vector3> pathPosList;

		private ExploreUICotroller expUICtr;


		void Awake()
		{
//			TransitionDelay = 0.5f;
//			currentMapIndex = 0;


			mapGenerator = GetComponent<MapGenerator>();

			navHelper = GetComponent<NavigationHelper> ();

			battlePlayerCtr = GetComponentInChildren<BattlePlayerController> ();

			battlePlayerCtr.enterMonster = new ExploreEventHandler (EnterMonster);
			battlePlayerCtr.enterItem = new ExploreEventHandler (EnterItem);
			battlePlayerCtr.enterNpc = new ExploreEventHandler (EnterNPC);

			Transform exploreCanvas = TransformManager.FindTransform ("ExploreCanvas");

			expUICtr = exploreCanvas.GetComponent<ExploreUICotroller> ();

		}
			
		//Initializes the game for each level.
		public void SetupExploreView(int chapterIndex)
		{
			battlePlayerCtr.SetUpExplorePlayerUI ();

			ChapterDetailInfo chapterDetail = DataInitializer.LoadDataToModelWithPath<ChapterDetailInfo> (CommonData.jsonFileDirectoryPath, CommonData.chapterDataFileName)[chapterIndex];

			//Call the SetupScene function of the BoardManager script, pass it current level number.
			mapGenerator.SetUpMap(chapterDetail);

		}



		 

		private void Update(){


			Vector3 clickPos = Vector3.zero;

#if UNITY_STANDALONE || UNITY_EDITOR

			if(Input.GetMouseButtonDown(0)){

				if(EventSystem.current.IsPointerOverGameObject()){
					Debug.Log("点击在UI上");
					return;
				}

				clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			}

#elif UNITY_ANDROID || UNITY_IOS
			if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended){

				if(EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)){
						Debug.Log("点击在UI上");
						return;
					}

				clickPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
			}
#endif
			// 未检测到点击位置
			//（初始设置点击位置在世界坐标原点，如果检测到点击点并初始化点击点之后，z一定会和camera的z保持一致，为－10，如果为0表示没有重设点击点）
			if (clickPos.z == 0) {
				return;
			}

			// 点击位置在地图有效区之外，直接返回
			if(clickPos.x + 0.5f >= mapGenerator.rows 
				|| clickPos.y + 0.5f >= mapGenerator.columns 
				|| clickPos.x + 0.5f < 0 
				|| clickPos.y + 0.5f < 0){
				Debug.Log ("点击在地图有效区外部");
				return;
			}


			// 由于地图贴图 tile时是以中心点为参考，宽高为1，所以如果以实际拼出的地图左下角为坐标原点，则点击位置需要进行如下坐标转换
			int targetX = (int)(clickPos.x + 0.5f);

			int targetY = (int)(clickPos.y + 0.5f);

			// 以地图左下角为坐标原点时的点击位置
			Vector3 targetPos = new Vector3(targetX, targetY, 0);

//			Vector3 rayEndPos = targetPos + new Vector3(0,0,15);

			// 检测点击到的碰撞体
//			RaycastHit2D r2d = Physics2D.Linecast(targetPos,rayEndPos,battlePlayer.blockingLayer);


//			if(r2d.transform != null){

			if(battlePlayerCtr != null){

					// 计算自动寻路路径
				pathPosList = navHelper.FindPath(battlePlayerCtr.singleMoveEndPos,targetPos,mapGenerator.mapWalkableInfoArray);

				}
//			}else{
//				// 点击位置没有有效碰撞体（不在地图有效范围内），则清空寻路路径
//				pathPosList.Clear();
//			}

			// 地图上点击位置生成提示动画
			mapGenerator.PlayDestinationAnim(targetPos,pathPosList.Count > 0);

			// 游戏角色按照自动寻路路径移动到点击位置
			battlePlayerCtr.MoveToEndByPath (pathPosList, targetPos);

		}



		public void EnterMonster(Transform monsterTrans){

			CallBack<Transform> playerWinCallBack = BattlePlayerWin;

			CallBack playerLoseCallBack = BattlePlayerLose;

			battleMonsterCtr = monsterTrans.GetComponent<BattleMonsterController> ();

			battleMonsterCtr.InitMonster (monsterTrans);


			battleMonsterCtr.StartFight (battlePlayerCtr,playerWinCallBack);
			battlePlayerCtr.StartFight (battleMonsterCtr,playerLoseCallBack);


			expUICtr.ShowFightPlane ();

		}


	


		public void EnterItem(Transform mapItemTrans){
			
			Debug.Log ("碰到了item");

			MapItem mapItem = mapItemTrans.GetComponent<MapItem> ();

			// 如果mapitem已打开，则直接返回
			if (mapItem.unlocked) {
				return;
			}

			// 如果该地图物品需要使用特殊物品开启
			if (mapItem.unlockItemName != string.Empty) {

				Item unlockItem = Player.mainPlayer.allItems.Find(delegate(Item item) {
					return item.itemName == mapItem.unlockItemName;
				});

				if (unlockItem == null) {
					
					expUICtr.SetUpTintHUD (unlockItem.itemName);

				} else {

					unlockItem.itemCount--;

					mapItem.UnlockMapItem (expUICtr.SetUpRewardItemsPlane,mapItem.rewardItems);

				}
				return;
			}

			// 如果该地图物品不需要使用特殊物品开启
			mapItem.UnlockMapItem (expUICtr.SetUpRewardItemsPlane,mapItem.rewardItems);
		}

		public void EnterNPC(Transform mapNpcTrans){

			Debug.Log ("碰到了npc");

			expUICtr.EnterNPC (mapNpcTrans.GetComponent<MapNPC> ().npc, currentMapIndex);

		}

		public void BattlePlayerWin(Transform[] monsterTransArray){

			if (monsterTransArray.Length <= 0) {
				return;
			}

			Transform trans = monsterTransArray [0];

			Vector3 monsterPos = trans.position;

			int X = (int)monsterPos.x;
			int Y = (int)monsterPos.y;

			mapGenerator.mapWalkableInfoArray [X, Y] = 1;

			battlePlayerCtr.ContinueMove ();


		}

		private void BattlePlayerLose(){

			Debug.Log ("dead");

		}



	}
}

