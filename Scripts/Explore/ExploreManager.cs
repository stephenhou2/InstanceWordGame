using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;



namespace WordJourney
{
	
//	using UnityEngine.SceneManagement;

	public class ExploreManager : MonoBehaviour
	{
		// 地图生成器
		private MapGenerator mapGenerator;						

		// 当前地图id
		private int currentLevelIndex;	
	
		// 当前关卡所有怪物
//		private List<BattleMonsterController> battleMonsters = new List<BattleMonsterController>();	

		// 当前碰到的怪物控制器
		private BattleMonsterController battleMonsterCtr;

		// 玩家控制器
		private BattlePlayerController battlePlayerCtr;


		private ExploreUICotroller expUICtr;

		private Transform currentEnteredTransform;

		[HideInInspector]public bool clickForConsumablesPos;

		void Awake()
		{

			mapGenerator = GetComponent<MapGenerator>();



			Transform battlePlayer = Player.mainPlayer.transform.Find ("BattlePlayer");

			battlePlayer.gameObject.SetActive (true);

			battlePlayerCtr = Player.mainPlayer.GetComponentInChildren<BattlePlayerController> ();

			battlePlayerCtr.ActiveBattlePlayer (false, false, false);

			battlePlayerCtr.enterMonster = new ExploreEventHandler (EnterMonster);
			battlePlayerCtr.enterNpc = new ExploreEventHandler (EnterNPC);
			battlePlayerCtr.enterWorkBench = new ExploreEventHandler (EnterWorkBench);
			battlePlayerCtr.enterCrystal = new ExploreEventHandler (EnterCrystal);

			battlePlayerCtr.enterTreasureBox = new ExploreEventHandler (EnterTreasureBox);
			battlePlayerCtr.enterObstacle = new ExploreEventHandler (EnterObstacle);
			battlePlayerCtr.enterTrapSwitch = new ExploreEventHandler (EnterSwitch);
			battlePlayerCtr.enterBillboard = new ExploreEventHandler (EnterBillboard);
			battlePlayerCtr.enterHole = new ExploreEventHandler (EnterHole);
			battlePlayerCtr.enterMovableBox = new ExploreEventHandler (EnterMovableBox);
			battlePlayerCtr.enterTransport = new ExploreEventHandler (EnterTransport);
			battlePlayerCtr.enterDoor = new ExploreEventHandler (EnterDoor);
			battlePlayerCtr.enterPlant = new ExploreEventHandler (EnterPlant);

			Transform exploreCanvas = TransformManager.FindTransform ("ExploreCanvas");

			expUICtr = exploreCanvas.GetComponent<ExploreUICotroller> ();


		}
			
		//Initializes the game for each level.
		public void SetupExploreView(GameLevelData levelData)
		{
			StartCoroutine ("SetUpExploreAfterDataReady",levelData);

		}

		private IEnumerator SetUpExploreAfterDataReady(GameLevelData levelData){

			bool dataReady = false;

			while (!dataReady) {

				dataReady = GameManager.Instance.gameDataCenter.CheckDatasReady (new GameDataCenter.GameDataType[] {
//					GameDataCenter.GameDataType.AnimatorControllers,
					GameDataCenter.GameDataType.UISprites,
					GameDataCenter.GameDataType.GameLevelDatas,
					GameDataCenter.GameDataType.Monsters,
					GameDataCenter.GameDataType.NPCs,
					GameDataCenter.GameDataType.ItemModels,
					GameDataCenter.GameDataType.ItemSprites,
					GameDataCenter.GameDataType.Materials,
					GameDataCenter.GameDataType.MaterialSprites,
					GameDataCenter.GameDataType.MapSprites,
					GameDataCenter.GameDataType.Skills,
//					GameDataCenter.GameDataType.EquipmentAttachedProperties,
					GameDataCenter.GameDataType.Skills,
					GameDataCenter.GameDataType.SkillSprites,


				});

				yield return null;
			}

			GameManager.Instance.soundManager.PlayExploreBackgroundMusic ();

			levelData.LoadAllData ();

			currentLevelIndex = levelData.gameLevelIndex;

			mapGenerator.SetUpMap(levelData);

			ExploreUICotroller expUICtr = TransformManager.FindTransform ("ExploreCanvas").GetComponent <ExploreUICotroller> ();

			expUICtr.SetUpExploreCanvas ();

			battlePlayerCtr.SetUpExplorePlayerUI ();

//			expUICtr.GetComponent<Canvas> ().enabled = true;

		}

		public void ItemsAroundAutoIntoLifeWithBasePoint(Vector3 basePostion,CallBack cb = null){

			mapGenerator.ItemsAroundAutoIntoLifeWithBasePoint (basePostion,cb);

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

			int targetX = 0;
			int targetY = 0;
			Vector3 targetPos = Vector3.zero;

			if (clickForConsumablesPos) {
				targetX = (int)(clickPos.x + 0.5f);
				targetY = (int)(clickPos.y + 0.5f);
				// 以地图左下角为坐标原点时的点击位置
				targetPos = new Vector3(targetX, targetY, 0);
				mapGenerator.ClickConsumablesPosAt (targetPos);
				clickForConsumablesPos = false;
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
			targetX = (int)(clickPos.x + 0.5f);

			targetY = (int)(clickPos.y + 0.5f);

			// 以地图左下角为坐标原点时的点击位置
			targetPos = new Vector3(targetX, targetY, 0);


			// 如果点在镂空区域，则直接返回
			if (mapGenerator.mapWalkableInfoArray [(int)targetPos.x, (int)targetPos.y] == -1) {
				return;
			}

			// 游戏角色按照自动寻路路径移动到点击位置
			bool arrivable = battlePlayerCtr.MoveToPosition (targetPos,mapGenerator.mapWalkableInfoArray);

			// 地图上点击位置生成提示动画
			mapGenerator.PlayDestinationAnim(targetPos,arrivable);



		}

//		public List<Vector3> FindPath(Vector3 startPos,Vector3 endPos){
//			navHelper.FindPath(startPos,endPos,
//		}

		public void DisableInteractivity(){
			expUICtr.ShowMask ();
		}

		public void EnableInteractivity(){
			expUICtr.HideMask ();
		}

		public void ObtainReward(Item reward){

			expUICtr.GetComponent<BattlePlayerUIController> ().UpdateItemButtons ();

		}


		/// <summary>
		/// 遭遇怪物时的响应方法
		/// </summary>
		/// <param name="monsterTrans">Monster trans.</param>
		public void EnterMonster(Transform monsterTrans){

			battlePlayerCtr.isInFight = true;

			battleMonsterCtr = monsterTrans.GetComponent<BattleMonsterController> ();

			battleMonsterCtr.InitMonster (monsterTrans);

			SetUpForFight ();

		}

		/// <summary>
		/// 调整人物角色和怪物角色的位置到战斗位置
		/// </summary>
		/// <param name="playerTrans">Player trans.</param>
		/// <param name="monsterTrans">Monster trans.</param>
		private void SetUpForFight(){

			battlePlayerCtr.boxCollider.enabled = false;
			battleMonsterCtr.boxCollider.enabled = false;

			battlePlayerCtr.enemy = battleMonsterCtr;

			battleMonsterCtr.enemy = battlePlayerCtr;

			battlePlayerCtr.InitFightTextDirectionTowards (battleMonsterCtr.transform.position);
			battleMonsterCtr.InitFightTextDirectionTowards (battlePlayerCtr.transform.position);

			battlePlayerCtr.SetUpPropertyCalculator ();
			battleMonsterCtr.SetUpPropertyCalculator ();

			// 初始化人物被动技能
			for (int i = 0; i < (battlePlayerCtr.agent as Player).attachedTriggeredSkills.Count; i++) {
				Skill skill = (battlePlayerCtr.agent as Player).attachedTriggeredSkills [i];
				skill.AffectAgents (battlePlayerCtr, battleMonsterCtr);
			}

			// 初始化怪物被动技能
			for (int i = 0; i < (battleMonsterCtr.agent as Monster).attachedTriggeredSkills.Count; i++) {
				Skill skill = (battleMonsterCtr.agent as Monster).attachedTriggeredSkills [i];
				skill.AffectAgents (battleMonsterCtr, battlePlayerCtr);
			}

			StartCoroutine ("AdjustCameraAndStartFight");

		}

		private IEnumerator AdjustCameraAndStartFight(){

			Vector3 playerOriPos = battlePlayerCtr.transform.position;
			Vector3 monsterOriPos = battleMonsterCtr.transform.position;

			if (Mathf.RoundToInt(playerOriPos.y) == Mathf.RoundToInt(monsterOriPos.y)) {

				battlePlayerCtr.transform.position = new Vector3 (0.2f * (monsterOriPos.x - playerOriPos.x ) + playerOriPos.x,
					playerOriPos.y,0);

			} else if(Mathf.RoundToInt(playerOriPos.x) == Mathf.RoundToInt(monsterOriPos.x)){

				float newPlayerPosX = playerOriPos.x;
				float newPlayerPosY = playerOriPos.y;

				float newMonsterPosX = monsterOriPos.x;
				float newMonsterPosY = monsterOriPos.y;

				if (playerOriPos.y > monsterOriPos.y) {
					newPlayerPosX = playerOriPos.x - 0.2f;
					newPlayerPosY = playerOriPos.y - 1f;
					battlePlayerCtr.transform.position = new Vector3 (newPlayerPosX, newPlayerPosY, 0);
					newMonsterPosX = monsterOriPos.x + 0.2f;
					newMonsterPosY = monsterOriPos.y - 0.3f;
					battleMonsterCtr.transform.position = new Vector3 (newMonsterPosX, newMonsterPosY, 0);
					battlePlayerCtr.TowardsRight ();
					battleMonsterCtr.TowardsLeft ();
				} else {
					newPlayerPosX = playerOriPos.x + 0.2f;
					newPlayerPosY = playerOriPos.y + 1f;
					battlePlayerCtr.transform.position = new Vector3 (newPlayerPosX, newPlayerPosY,0);
					newMonsterPosX = monsterOriPos.x - 0.2f;
					newMonsterPosY = monsterOriPos.y + 0.3f;
					battleMonsterCtr.transform.position = new Vector3 (newMonsterPosX, newMonsterPosY, 0);
					battlePlayerCtr.TowardsLeft ();
					battleMonsterCtr.TowardsRight ();
				}
			}


			DisableInteractivity ();

			Camera c = Camera.main;

			float cameraSizeFixSpeed = 2f;

			float fixDuration = 0.5f;

			float timer = 0;

			while (timer < fixDuration) {

				c.orthographicSize -= cameraSizeFixSpeed * Time.deltaTime;

//				c.transform.localPosition -= new Vector3 (0, cameraFixSpeedY * Time.deltaTime, 0);

				timer += Time.deltaTime;

				yield return null;

			}

			// 执行玩家角色战斗前技能回调
			battlePlayerCtr.ExcuteBeforeFightSkillCallBacks(battleMonsterCtr);

			// 执行怪物角色战斗前技能回调
			battleMonsterCtr.ExcuteBeforeFightSkillCallBacks(battlePlayerCtr);

			battleMonsterCtr.StartFight (battlePlayerCtr,BattlePlayerWin);
			battlePlayerCtr.StartFight (battleMonsterCtr,BattlePlayerLose);

			expUICtr.ShowFightPlane ();
			EnableInteractivity ();
		}



		private void EnterObstacle(Transform obstacleTrans){

			Obstacle obstacle = obstacleTrans.GetComponent<Obstacle>();

			Consumables tool = Player.mainPlayer.allConsumablesInBag.Find (delegate(Consumables obj) {
				return obj.itemName == obstacle.destroyToolName;
			});

			if (tool != null) {
				expUICtr.GetComponent<BattlePlayerUIController> ().SetUpToolChoicePlane (obstacle,tool);
			} else {
				expUICtr.SetUpTintHUD ("缺少可以清除当前路障的工具");
			}

		}



		private void EnterSwitch(Transform switchTrans){

			TrapSwitch trapSwitch = switchTrans.GetComponent<TrapSwitch>();

			GameManager.Instance.soundManager.PlayMapEffectClips (trapSwitch.audioClipName);

			trapSwitch.ChangeSwitchStatus ();

			mapGenerator.ChangeAllTrapStatusInMap ();

		}

		private void EnterTreasureBox(Transform treasureBoxTrans){

			TreasureBox tb = treasureBoxTrans.GetComponent<TreasureBox>();

			// 如果mapitem已打开，则直接返回
			if (tb.unlockItemId != -1 && !tb.locked) {
				return;
			}

			// 如果该宝箱不需要使用钥匙开启
			if (tb.unlockItemId == -1) {

				GameManager.Instance.soundManager.PlayMapEffectClips (tb.audioClipName);

				// 如果该地图物品不需要使用特殊物品开启
				tb.UnlockTreasureBox (()=>{

					if (tb.walkableAfterChangeStatus) {
						mapGenerator.mapWalkableInfoArray [(int)tb.transform.position.x, (int)tb.transform.position.y] = 1;
					}

					mapGenerator.SetUpRewardInMap(tb.rewardItem,tb.transform.position);
//					expUICtr.SetUpRewardItemsPlane(tb.rewardItem);
				});

				return;

			}


			// 宝箱需要使用钥匙开启
			// 查找背包中是否有钥匙
			Consumables key = Player.mainPlayer.allConsumablesInBag.Find (delegate(Consumables obj) {
				return obj.itemId == tb.unlockItemId;
			});

			// 如果背包中有钥匙，则进入工具选择栏
			if (key != null) {
				expUICtr.GetComponent<BattlePlayerUIController> ().SetUpToolChoicePlane (tb, key);
			} else {
				expUICtr.SetUpTintHUD ("缺少钥匙");
			}

		}

		private void EnterDoor(Transform doorTrans){
			Debug.Log ("door");
		}

		private void EnterNPC(Transform mapNpcTrans){

			Debug.Log ("碰到了npc");

			expUICtr.EnterNPC (mapNpcTrans.GetComponent<MapNPC> ().npc, currentLevelIndex);

		}

		private void EnterWorkBench(Transform workBench){

			Debug.Log ("进入工作台");

			expUICtr.SetUpWorkBenchPlane ();


		}

		private void EnterCrystal(Transform crystal){
			Debug.Log ("进入水晶");
			currentEnteredTransform = crystal;
			expUICtr.SetUpCrystaleQueryHUD();
		}

		private void EnterBillboard(Transform billboard){

			Debug.Log ("进入公告牌");

			Billboard bb = billboard.GetComponent<Billboard> ();

			expUICtr.SetUpBillboard (bb);

		}

		private void EnterHole(Transform hole){
			Debug.Log ("进入坑洞");

			hole.GetComponent<Hole> ().EnterHole (mapGenerator,battlePlayerCtr);


		}

		private void EnterMovableBox(Transform movableBox){

			Debug.Log ("推箱子");

			movableBox.GetComponent<MovableBox> ().OnAgentPushBox (battlePlayerCtr,mapGenerator);

		}

		private void EnterTransport(Transform transportTrans){

			Debug.Log ("进入传送阵");


		}

		private void EnterPlant(Transform plantTransform){
			Debug.Log ("碰到了植物");
		}

		public void ShowConsumablesValidPointTintAround(Consumables consumables){

			mapGenerator.ShowConsumablesValidPointsTint (consumables);

		}


		#warning 退出学习貌似不应该放在这里，后面在看一看
		public void FinishLearning(){
			GameManager.Instance.UIManager.HideCanvas ("LearnCanvas");
			currentEnteredTransform.GetComponent<Animator> ().SetTrigger ("ChangeStatus");
		}


		public void BattlePlayerWin(Transform[] monsterTransArray){

			battlePlayerCtr.enemy = null;

			battleMonsterCtr.enemy = null;

			battlePlayerCtr.RemoveTriggeredSkillEffectFromAgent ();
			battleMonsterCtr.RemoveTriggeredSkillEffectFromAgent ();

			battlePlayerCtr.agent.ResetBattleAgentProperties (false);
			battleMonsterCtr.agent.ResetBattleAgentProperties (true);

			FightEndCallBacks ();

			if (monsterTransArray.Length <= 0) {
				return;
			}

			Transform trans = monsterTransArray [0];

			Vector3 monsterPos = trans.position;

			// 0.1为位置偏差修正【如果怪物在（10，10）点，可能实际的位置数据为（10.023456，9.023455），故加上0.1作为偏差修正】
			int X = Mathf.RoundToInt(monsterPos.x);
			int Y = Mathf.RoundToInt(monsterPos.y);

			mapGenerator.mapWalkableInfoArray [X, Y] = 1;

			Player player = battlePlayerCtr.agent as Player;

			player.experience += trans.GetComponent<Monster> ().rewardExperience;//更新玩家经验值

			player.LevelUpIfExperienceEnough ();//判断是否升级

			int characterIndex = Random.Range (0, 26);

			char character = (char)(characterIndex + CommonData.aInASCII);

			CharacterFragment characterFragment = new CharacterFragment (character, 1);

			mapGenerator.SetUpRewardInMap (characterFragment, monsterPos);

			ResetCamareAndContinueMove ();

		}

		private void BattlePlayerLose(){

			battlePlayerCtr.enemy = null;

			battleMonsterCtr.enemy = null;

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


		private void ResetCamareAndContinueMove(){

			StartCoroutine ("ResetCamera");

			battlePlayerCtr.ContinueMove ();

		}


		private IEnumerator ResetCamera(){

			DisableInteractivity ();

			Camera c = Camera.main;

			float cameraSizeFixSpeed = 2f;

			float fixDuration = 0.5f;

			float timer = 0;

			while (timer < fixDuration) {

				c.orthographicSize += cameraSizeFixSpeed * Time.deltaTime;

				timer += Time.deltaTime;

				yield return null;

			}

			EnableInteractivity ();

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

			GameManager.Instance.gameDataCenter.ReleaseDataWithDataTypes (new GameDataCenter.GameDataType[] {
				GameDataCenter.GameDataType.Materials,
				GameDataCenter.GameDataType.MaterialSprites,
				GameDataCenter.GameDataType.MapSprites,
				GameDataCenter.GameDataType.Skills, 
				GameDataCenter.GameDataType.SkillSprites,
				GameDataCenter.GameDataType.Monsters,
				GameDataCenter.GameDataType.NPCs,
			});

			TransformManager.FindTransform ("ExploreCanvas").GetComponent<ExploreUICotroller> ().QuitExplore ();

		}


	}
}

