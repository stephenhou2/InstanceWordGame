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

			Transform exploreCanvas = TransformManager.FindTransform ("ExploreCanvas");

			expUICtr = exploreCanvas.GetComponent<ExploreUICotroller> ();


		}
			
		//Initializes the game for each level.
		public void SetupExploreView(ChapterDetailInfo chapterDetail)
		{
			battlePlayerCtr.SetUpExplorePlayerUI ();

			//Call the SetupScene function of the BoardManager script, pass it current level number.
			mapGenerator.SetUpMap(chapterDetail);

			ExploreUICotroller expUICtr = TransformManager.FindTransform ("ExploreCanvas").GetComponent <ExploreUICotroller> ();

			expUICtr.SetUpExploreCanvas ();

			expUICtr.GetComponent<Canvas> ().enabled = true;

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

			expUICtr.ShowMask ();

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

			GameManager.Instance.soundManager.PlayClips (
				GameManager.Instance.gameDataCenter.allExploreAudioClips, 
				SoundDetailTypeName.Map, 
				mapItem.mapItemName);

			battlePlayerCtr.PlayRoleAnim ("fightWithAxe", 1, () => {

				obstacle.UnlockOrDestroyMapItem(()=>{

					mapGenerator.mapWalkableInfoArray [(int)obstacle.transform.position.x, (int)obstacle.transform.position.y] = 1;

					expUICtr.HideMask();

					Player player = Player.mainPlayer;

					for (int i = 0; i < player.allEquipedEquipments.Count; i++) {

						Equipment equipment = player.allEquipedEquipments [i];

						// 使用武器破坏障碍物时，武器的耐久度降低
						if (equipment.equipmentType == EquipmentType.Weapon) {

							equipment.durability -= CommonData.durabilityDecreaseWhenAttackObstacle;

							if (equipment.durability <= 0) {
								string tint = string.Format("{0}完全损坏",equipment.itemName);
								expUICtr.SetUpTintHUD (tint);
								player.allEquipmentsInBag.Remove (equipment);
								player.allEquipedEquipments.Remove (equipment);
								equipment = null;
							}
						}
					}

				});

			});


		}

		private void EnterSwitch(MapItem mapItem){

			TrapSwitch trapSwitch = mapItem as TrapSwitch;

			if (trapSwitch.switchOff) {
				
				expUICtr.HideMask ();

				return;
			}

			GameManager.Instance.soundManager.PlayClips (
				GameManager.Instance.gameDataCenter.allExploreAudioClips,
				SoundDetailTypeName.Map, 
				mapItem.mapItemName);

			trapSwitch.SwitchOffTrap ();

			expUICtr.HideMask ();

		}

		private void EnterTreasureBox(MapItem mapItem){

			TreasureBox tb = mapItem as TreasureBox;

			// 如果mapitem已打开，则直接返回
			if (tb.unlocked) {
				expUICtr.HideMask ();
				return;
			}

			// 如果该地图物品需要使用特殊物品开启
			if (tb.unlockItemName != string.Empty) {

				Consumables unlockItem = Player.mainPlayer.allConsumablesInBag.Find(delegate(Consumables item) {
					return item.itemName == tb.unlockItemName;
				});

				if (unlockItem == null) {
					string tint = string.Format ("缺少 <color=blue>{0}x1</color>", tb.unlockItemName);
					expUICtr.SetUpTintHUD (tint);

				} else {

					unlockItem.itemCount--;

					GameManager.Instance.soundManager.PlayClips (
						GameManager.Instance.gameDataCenter.allExploreAudioClips, 
						SoundDetailTypeName.Map, 
						mapItem.mapItemName);

					tb.UnlockOrDestroyMapItem (()=>{

						if (tb.walkableAfterUnlockOrDestroy) {
							mapGenerator.mapWalkableInfoArray [(int)tb.transform.position.x, (int)tb.transform.position.y] = 1;
						}
						expUICtr.SetUpRewardItemsPlane(tb.rewardItems);
					});



				}
				return;
			}

			GameManager.Instance.soundManager.PlayClips (
				GameManager.Instance.gameDataCenter.allExploreAudioClips,
				SoundDetailTypeName.Map, 
				mapItem.mapItemName);

			// 如果该地图物品不需要使用特殊物品开启
			tb.UnlockOrDestroyMapItem (()=>{

				if (tb.walkableAfterUnlockOrDestroy) {
					mapGenerator.mapWalkableInfoArray [(int)tb.transform.position.x, (int)tb.transform.position.y] = 1;
				}
				expUICtr.SetUpRewardItemsPlane(tb.rewardItems);
			});

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


			#warning 消灭怪物后需要走到怪物原位置的话开启下面这段代卖
//			battlePlayerCtr.ContinueMove ();


		}

		private void BattlePlayerLose(){

			Debug.Log ("dead");

			Player player = Player.mainPlayer;

			player.ResetBattleAgentProperties (true);

			#warning 玩家死亡后掉落的物品是否需要展示出来
			Item lostItem = player.LostItemWhenDie ();

			QuitExploreScene ();
		}


		public void EnterNextLevel(){

			Player player = Player.mainPlayer;

			#warning 关卡数据只做了一关，暂时使用第一关的数据，后面数据做好后打开下面注释的代码
//			player.currentChapterIndex++;

			if (player.currentChapterIndex > player.maxUnlockChapterIndex) {
				player.maxUnlockChapterIndex = player.currentChapterIndex;
			}

			ChapterDetailInfo chapterDetail = GameManager.Instance.gameDataCenter.chapterDetails [player.currentChapterIndex];

			SetupExploreView (chapterDetail);

		}


		public void QuitExploreScene(){

			Camera.main.transform.SetParent (null);

			battlePlayerCtr.gameObject.SetActive(false);
		
			mapGenerator.DestroyInstancePools ();

			battlePlayerCtr.ClearReference ();

			Destroy(this.gameObject);

			GameManager.Instance.gameDataCenter.ReleaseDataWithNames (new string[] {
				"AllMaterials", "AllMaterialSprites", "AllMapSprites", 
				"AllSkills", "AllSkillSprites", "AllMonsters","AllExploreAudioClips"
			});

			TransformManager.FindTransform ("ExploreCanvas").GetComponent<ExploreUICotroller> ().QuitExplore ();

		}


	}
}

