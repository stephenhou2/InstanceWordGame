using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;



namespace WordJourney
{
	
//	using UnityEngine.SceneManagement;

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

			mapGenerator = GetComponent<MapGenerator>();

			navHelper = GetComponent<NavigationHelper> ();

			Transform battlePlayer = Player.mainPlayer.transform.Find ("BattlePlayer");

			battlePlayer.gameObject.SetActive (true);

			battlePlayerCtr = Player.mainPlayer.GetComponentInChildren<BattlePlayerController> ();

			battlePlayerCtr.ActiveBattlePlayer (true, false, false);

			battlePlayerCtr.enterMonster = new ExploreEventHandler (EnterMonster);
			battlePlayerCtr.enterItem = new ExploreEventHandler (EnterItem);
			battlePlayerCtr.enterNpc = new ExploreEventHandler (EnterNPC);
			battlePlayerCtr.enterWorkBench = new ExploreEventHandler (EnterWorkBench);
			battlePlayerCtr.enterCrystal = new ExploreEventHandler (EnterCrystal);

			Transform exploreCanvas = TransformManager.FindTransform ("ExploreCanvas");

			expUICtr = exploreCanvas.GetComponent<ExploreUICotroller> ();


		}
			
		//Initializes the game for each level.
		public void SetupExploreView(GameLevelData levelData)
		{
			battlePlayerCtr.SetUpExplorePlayerUI ();

			//Call the SetupScene function of the BoardManager script, pass it current level number.
			mapGenerator.SetUpMap(levelData);

			ExploreUICotroller expUICtr = TransformManager.FindTransform ("ExploreCanvas").GetComponent <ExploreUICotroller> ();

			expUICtr.SetUpExploreCanvas ();

			expUICtr.GetComponent<Canvas> ().enabled = true;

		}

		 

		private void Update(){


			Vector3 clickPos = Vector3.zero;

#if UNITY_STANDALONE || UNITY_EDITOR

			if(Input.GetMouseButtonDown(0)){

				if(EventSystem.current.IsPointerOverGameObject()){
//					Debug.Log("点击在UI上");
					return;
				}

				clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			}

#elif UNITY_ANDROID || UNITY_IOS

			if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began){

				if(EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)){
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
			if(clickPos.x + 0.5f >= mapGenerator.columns 
				|| clickPos.y + 0.5f >= mapGenerator.rows
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

		public void ObtainReward(Item reward){

			expUICtr.GetComponent<BattlePlayerUIController> ().UpdateItemButtons ();

		}


		/// <summary>
		/// 遭遇怪物时的响应方法
		/// </summary>
		/// <param name="monsterTrans">Monster trans.</param>
		public void EnterMonster(Transform monsterTrans){

			CallBack<Transform> playerWinCallBack = BattlePlayerWin;

			CallBack playerLoseCallBack = BattlePlayerLose;

			battleMonsterCtr = monsterTrans.GetComponent<BattleMonsterController> ();

			battleMonsterCtr.InitMonster (monsterTrans);

			AdjustAgentsPosotion (battlePlayerCtr.transform,battleMonsterCtr.transform);

			// 初始化人物被动技能
			for (int i = 0; i < (battlePlayerCtr.agent as Player).allLearnedSkills.Count; i++) {
				Skill skill = (battlePlayerCtr.agent as Player).allLearnedSkills [i];
				if (skill.skillType == SkillType.TriggeredPassive) {
					skill.AffectAgents (battlePlayerCtr, battleMonsterCtr);
				}
			}

			// 初始化怪物被动技能
			for (int i = 0; i < (battleMonsterCtr.agent as Monster).allEquipedPassiveSkills.Length; i++) {
				Skill skill = (battleMonsterCtr.agent as Monster).allEquipedPassiveSkills [i];
				if (skill.skillType == SkillType.TriggeredPassive) {
					skill.AffectAgents (battleMonsterCtr, battlePlayerCtr);
				}

			}

			// 执行玩家角色战斗前技能回调
			battlePlayerCtr.ExcuteBeforeFightSkillCallBacks(battleMonsterCtr);

			// 执行怪物角色战斗前技能回调
			battleMonsterCtr.ExcuteBeforeFightSkillCallBacks(battlePlayerCtr);

			battleMonsterCtr.StartFight (battlePlayerCtr,playerWinCallBack);
			battlePlayerCtr.StartFight (battleMonsterCtr,playerLoseCallBack);

			expUICtr.ShowFightPlane ();

		}

		/// <summary>
		/// 调整人物角色和怪物角色的位置到战斗位置
		/// </summary>
		/// <param name="playerTrans">Player trans.</param>
		/// <param name="monsterTrans">Monster trans.</param>
		private void AdjustAgentsPosotion(Transform playerTrans,Transform monsterTrans){

			Vector3 playerOriPos = playerTrans.position;
			Vector3 monsterOriPos = monsterTrans.position;

			if ((int)playerTrans.position.y == (int)monsterTrans.position.y) {

				playerTrans.position = new Vector3 (0.2f * (monsterTrans.position.x - playerTrans.position.x ) + playerTrans.position.x,
					playerTrans.position.y,playerOriPos.y);

			} else {
				float newPosX = playerTrans.position.x - 0.2f;
				float newPosY = playerTrans.position.y + 0.5f * (monsterTrans.position.y-  playerTrans.position.y);

				playerTrans.position = new Vector3 (newPosX, newPosY,playerOriPos.z);

			}

		}

	


		public void EnterItem(Transform mapItemTrans){
			
			Debug.Log ("碰到了item");

//			expUICtr.ShowMask ();

			MapItem mapItem = mapItemTrans.GetComponent<MapItem> ();

			switch (mapItem.mapItemType) {

			case MapItemType.Obstacle:
				EnterObstacle (mapItem);
				break;
			case MapItemType.TrapSwitch:
				EnterSwitch (mapItem);
				break;
			case MapItemType.TreasureBox:
				EnterTreasureBox (mapItem);
				break;
			default:
				break;
			}

		}

		private void EnterObstacle(MapItem mapItem){

			Obstacle obstacle = mapItem as Obstacle;

			expUICtr.GetComponent<BattlePlayerUIController> ().SetUpToolChoicePlane (obstacle);

		}

		private void EnterSwitch(MapItem mapItem){

			TrapSwitch trapSwitch = mapItem as TrapSwitch;

			if (trapSwitch.switchOff) {

				return;
			}

			GameManager.Instance.soundManager.PlayClips (
				GameManager.Instance.gameDataCenter.allExploreAudioClips,
				SoundDetailTypeName.Map, 
				mapItem.mapItemName);

			trapSwitch.SwitchOffTrap ();

			Transform trapTrans = trapSwitch.trap.transform;

			mapGenerator.mapWalkableInfoArray[(int)trapTrans.position.x,(int)trapTrans.position.y] = 1;


		}

		private void EnterTreasureBox(MapItem mapItem){

			TreasureBox tb = mapItem as TreasureBox;

			// 如果mapitem已打开，则直接返回
			if (tb.unlocked) {
				return;
			}

			// 如果该地图物品不需要使用特殊物品开启
			if (tb.unlockItemName == string.Empty) {

				GameManager.Instance.soundManager.PlayClips (
					GameManager.Instance.gameDataCenter.allExploreAudioClips,
					SoundDetailTypeName.Map, 
					mapItem.mapItemName);

				// 如果该地图物品不需要使用特殊物品开启
				tb.UnlockOrDestroyMapItem (()=>{

					if (tb.walkableAfterUnlockOrDestroy) {
						mapGenerator.mapWalkableInfoArray [(int)tb.transform.position.x, (int)tb.transform.position.y] = 1;
					}

					mapGenerator.SetUpRewardInMap(tb.rewardItem,tb.transform.position);
//					expUICtr.SetUpRewardItemsPlane(tb.rewardItem);
				});

				return;

			}

			// 如果该地图物品需要使用特殊物品开启,则进入工具选择界面
			expUICtr.GetComponent<BattlePlayerUIController> ().SetUpToolChoicePlane (mapItem);

		}

		public void EnterNPC(Transform mapNpcTrans){

			Debug.Log ("碰到了npc");

			expUICtr.EnterNPC (mapNpcTrans.GetComponent<MapNPC> ().npc, currentMapIndex);

		}

		public void EnterWorkBench(Transform workBench){

			Debug.Log ("进入工作台");

			expUICtr.SetUpWorkBenchPlane ();


		}

		public void EnterCrystal(Transform crystal){
			Debug.Log ("进入水晶");
		}
			

		public void BattlePlayerWin(Transform[] monsterTransArray){

			FightEndCallBacks ();

			if (monsterTransArray.Length <= 0) {
				return;
			}

			Transform trans = monsterTransArray [0];

			Vector3 monsterPos = trans.position;

			int X = (int)monsterPos.x;
			int Y = (int)monsterPos.y;

			mapGenerator.mapWalkableInfoArray [X, Y] = 1;

			Player player = battlePlayerCtr.agent as Player;

			player.experience += trans.GetComponent<Monster> ().rewardExperience;//更新玩家经验值

			player.LevelUpIfExperienceEnough ();//判断是否升级

			#warning 消灭怪物后需要走到怪物原位置的话开启下面这段代卖
			battlePlayerCtr.ContinueMove ();


		}

		private void BattlePlayerLose(){

			FightEndCallBacks ();

			Player player = Player.mainPlayer;

			player.ResetBattleAgentProperties (true);

			#warning 玩家死亡后掉落的物品是否需要展示出来
			Item lostItem = player.LostItemWhenDie ();

			QuitExploreScene ();
		}

		private void FightEndCallBacks(){

			// 执行玩家角色战斗结束技能回调
			battlePlayerCtr.ExcuteFightEndCallBacks(battleMonsterCtr);

			// 执行怪物角色战斗结束技能回调
			battleMonsterCtr.ExcuteFightEndCallBacks(battlePlayerCtr);

			// 清理所有状态和技能回调
			battlePlayerCtr.ClearAllEffectStatesAndSkillCallBacks ();
			battleMonsterCtr.ClearAllEffectStatesAndSkillCallBacks ();

		}


		public void EnterNextLevel(){

			Player player = Player.mainPlayer;

			player.currentLevelIndex++;

			if (player.currentLevelIndex >= 5) {
				Debug.Log ("通关");
				return;
			}

			if (player.currentLevelIndex > player.maxUnlockLevelIndex) {
				player.maxUnlockLevelIndex = player.currentLevelIndex;
			}
				

			GameLevelData levelData = GameManager.Instance.gameDataCenter.gameLevelDatas [player.currentLevelIndex];

			SetupExploreView (levelData);

		}


		public void QuitExploreScene(){

			Camera.main.transform.SetParent (null);

			battlePlayerCtr.QuitExplore ();
		
			mapGenerator.DestroyInstancePools ();

			Destroy(this.gameObject);

			GameManager.Instance.gameDataCenter.ReleaseDataWithNames (new string[] {
				"AllMaterials", "AllMaterialSprites", "AllMapSprites", 
				"AllSkills", "AllSkillSprites", "AllMonsters","AllNpcs","AllExploreAudioClips"
			});

			TransformManager.FindTransform ("ExploreCanvas").GetComponent<ExploreUICotroller> ().QuitExplore ();

		}


	}
}

