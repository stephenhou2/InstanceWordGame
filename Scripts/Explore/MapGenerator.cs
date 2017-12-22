using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic; 		//Allows us to use Lists.
using Random = UnityEngine.Random; 		//Tells Random to use the Unity Engine random number generator.
using Transform = UnityEngine.Transform;
using DragonBones;

namespace WordJourney	
{
	public class MapGenerator:MonoBehaviour
	{
		// 地图的行数和列数
		[HideInInspector]public int columns; 										
		[HideInInspector]public int rows;								

		// 地图物品生成器
//		private MapItemGenerator mapItemGenerator;

		// 地图信息（用于绘制地图）
		private MapData mapInfo;
//		private TileInfo tileInfo;

		// 外墙模型
		public Transform wallModel;
		// 地板模型
		public Transform floorModel;
		// 地图上npc模型
		public Transform mapNpcModel;

		public Transform skillEffectModel;

		// 地图上掉落的物品模型
		public Transform rewardItemModel;

		// 地图上的工作台
		public Transform workBench;

		public Transform crystalModel;

		// 地图上所有地图物品列表
//		private List<MapItem> mapItems = new List<MapItem> ();

		// 地图上所有npc列表
//		private List<MapNPC> mapNpcs = new List<MapNPC>();

		// 地图上所有怪物列表
//		private List<Monster>monsters = new List<Monster>();

		// 所有地图元素在场景中的父容器
		public Transform floorsContainer;
		public Transform mapItemsContainer;
		public Transform npcsContainer;
		public Transform monstersContainer;
		public Transform skillEffectsContainer;
		public Transform rewardsContainer;
		public Transform crystalsContainer;
		public Transform otherAnimContainer;

		// 所有的缓存池
		private InstancePool outerWallPool;
		private InstancePool floorPool;
		private InstancePool npcPool;
		private InstancePool mapItemPool;
		private InstancePool monsterPool;
		private InstancePool skillEffectPool;
		private InstancePool rewardItemPool;
		private InstancePool crystalPool;
		private InstancePool otherAnimPool;

		public Transform destinationAnimation;

		public Transform otherAnimationModel;

		// 关卡数据
		private GameLevelData levelData;

		public UnityEngine.Material spriteMaterial; 

//		private List <Vector3> gridPositions = new List <Vector3> ();	//A list of possible locations to place tiles.

		public int[,] mapWalkableInfoArray;

		// 从地图数据中读取的原始可行走信息表
		private int[,] totalMapWalkableInfoArray;

		public float rewardFlyDuration;

		private BattlePlayerController bpCtr;

		// 获得地板层和附加信息层的数据
		private Layer floorLayer = null;
		private Layer attachedInfoLayer = null;
		private Layer attachedItemInfoLayer = null;


		// 障碍物模型数组
		public Obstacle treeModel;
		public Obstacle stoneModel;


		// 陷阱模型
		public Trap trapModel;

		// 开关模型
		public TrapSwitch trapSwitchModel;

		// 宝箱模型
		public TreasureBox lockedTreasureBoxModel;

		// 木桶模型
		public TreasureBox buckModel;

		// 瓦罐模型
		public TreasureBox potModel;

		// 传送阵模型
		public Transport transportModel;

		// 可移动地板模型
		public MovableFloor movableFloorModel;

		// 门模型
		public Door doorModel;

		private List<Trap> allTrapsInMap = new List<Trap> ();

		private List<Vector3> totalValablePosGridList = new List<Vector3> ();
		private List<Vector3> playerOriginalPosList = new List<Vector3> ();
//		private List<Vector3> treasureBox_npc_produceBuildingPosGridList;
//		private List<Vector3> learnCrystalPosGridList;
//		private List<Vector3> movableFloorPosGridList;
//		private Vector3 bossPos;
//		private List<Vector3> undestroyableObstacleGridList;

		private List<Transform> allUnusedMonsters = new List<Transform> ();
		private List<Transform> allUnusedOtherItems = new List<Transform> ();
		private List<Transform> allUnusedFloors = new List<Transform> ();

		//SetupScene initializes our level and calls the previous functions to lay out the game board
		public void SetUpMap (GameLevelData levelData)
		{

			Transform poolContainerOfExploreScene = TransformManager.FindOrCreateTransform (CommonData.poolContainerName + "/PoolContainerOfExploreScene");
//			Transform modelContainerOfExploreScene = TransformManager.FindOrCreateTransform (CommonData.instanceContainerName + "/ModelContainerOfExploreScene");

			if (poolContainerOfExploreScene.childCount == 0) {
				outerWallPool = InstancePool.GetOrCreateInstancePool ("OuterWallPool",poolContainerOfExploreScene.name);
				floorPool = InstancePool.GetOrCreateInstancePool ("FloorPool",poolContainerOfExploreScene.name);
				npcPool = InstancePool.GetOrCreateInstancePool ("NPCPool",poolContainerOfExploreScene.name);
				mapItemPool = InstancePool.GetOrCreateInstancePool ("ItemPool",poolContainerOfExploreScene.name);
				monsterPool = InstancePool.GetOrCreateInstancePool ("MonsterPool",poolContainerOfExploreScene.name);
				skillEffectPool = InstancePool.GetOrCreateInstancePool ("SkillEffectPool",poolContainerOfExploreScene.name);
				rewardItemPool = InstancePool.GetOrCreateInstancePool ("RewardItemPool", poolContainerOfExploreScene.name);
				crystalPool = InstancePool.GetOrCreateInstancePool ("CrystalPool", poolContainerOfExploreScene.name);
				otherAnimPool = InstancePool.GetOrCreateInstancePool ("OtherAnimPool", poolContainerOfExploreScene.name);
			}

			MapInstancesToPool ();

			this.levelData = levelData;

			mapInfo = MapData.GetMapDataOfLevel(levelData.gameLevelIndex);

			// 获取地图建模的行数和列数
			rows = mapInfo.rowCount;
			columns = mapInfo.columnCount;

			// 初始化地图原始数据
			InitMapDatas();

			ResetMap ();

			SetupAllAnimatorControllers ();

			// 初始化地面和背景
			SetUpFloorAndBackground();

			SetUpMapWithAttachedInfo ();

			SetUpPlayer ();

			ClearPools ();



		}


		private void SetupAllAnimatorControllers(){

			treeModel.GetComponent<Animator> ().runtimeAnimatorController = GameManager.Instance.gameDataCenter.allAnimatorControllers.Find (delegate(RuntimeAnimatorController obj) {
				return obj.name == "TreeAnimatorController";
			});

			stoneModel.GetComponent<Animator> ().runtimeAnimatorController = GameManager.Instance.gameDataCenter.allAnimatorControllers.Find (delegate(RuntimeAnimatorController obj) {
				return obj.name == "StoneAnimatorController";
			});

			buckModel.GetComponent<Animator> ().runtimeAnimatorController = GameManager.Instance.gameDataCenter.allAnimatorControllers.Find (delegate(RuntimeAnimatorController obj) {
				return obj.name == "BuckAnimatorController";
			});
			potModel.GetComponent<Animator> ().runtimeAnimatorController = GameManager.Instance.gameDataCenter.allAnimatorControllers.Find (delegate(RuntimeAnimatorController obj) {
				return obj.name == "PotAnimatorController";
			});
			lockedTreasureBoxModel.GetComponent<Animator> ().runtimeAnimatorController = GameManager.Instance.gameDataCenter.allAnimatorControllers.Find (delegate(RuntimeAnimatorController obj) {
				return obj.name == "TreasureBoxAnimatorController";
			});
			transportModel.GetComponent<Animator> ().runtimeAnimatorController = GameManager.Instance.gameDataCenter.allAnimatorControllers.Find (delegate(RuntimeAnimatorController obj) {
				return obj.name == "OtherAnimatorController";
			});
			skillEffectModel.GetComponent<Animator> ().runtimeAnimatorController = GameManager.Instance.gameDataCenter.allAnimatorControllers.Find (delegate(RuntimeAnimatorController obj) {
				return obj.name == "SkillEffectAnimatorController";
			});
			destinationAnimation.GetComponent<Animator> ().runtimeAnimatorController = GameManager.Instance.gameDataCenter.allAnimatorControllers.Find (delegate(RuntimeAnimatorController obj) {
				return obj.name == "DestinationAnimatorController";
			});
			otherAnimationModel.GetComponent<Animator> ().runtimeAnimatorController = GameManager.Instance.gameDataCenter.allAnimatorControllers.Find (delegate(RuntimeAnimatorController obj) {
				return obj.name == "OtherAnimatorController";
			});
			crystalModel.GetComponent<Animator> ().runtimeAnimatorController = GameManager.Instance.gameDataCenter.allAnimatorControllers.Find (delegate(RuntimeAnimatorController obj) {
				return obj.name == "CrystalAnimatorController";
			});
		}

		private void ResetMap(){
			
			// 地图上的原始(地图完全解锁时的)可行走信息数组
			totalMapWalkableInfoArray = new int[columns, rows];
			// 地图当前的可行走信息数组
			mapWalkableInfoArray = new int[columns,rows];

			ResetMapWalkableInfoArray ();

			allTrapsInMap.Clear ();

			totalValablePosGridList.Clear ();

			playerOriginalPosList.Clear ();

			allUnusedFloors.Clear ();
			allUnusedMonsters.Clear ();
			allUnusedOtherItems.Clear ();
		}

		/// <summary>
		/// 将地图范围内的所有点都设置为不可行走点
		/// </summary>
		private void ResetMapWalkableInfoArray (){
			for (int i = 0; i < columns; i++) {
				for (int j = 0; j < rows ; j++) {
					mapWalkableInfoArray [i, j] = -1;
					totalMapWalkableInfoArray [i, j] = -1;
				}
			}
		}


		/// <summary>
		/// 地图数据分离出地板层数据和附加信息层数据
		/// 根据地板层数据初始化基础的地图可行走信息
		/// </summary>
		private void InitMapDatas(){

			for (int i = 0; i < mapInfo.layers.Length; i++) {

				switch (mapInfo.layers [i].name) {
				case "FloorLayer":
					floorLayer = mapInfo.layers [i];
					break;
				case "AttachedInfoLayer":
					attachedInfoLayer = mapInfo.layers [i];
					break;
				case "AttachedItemLayer":
					attachedItemInfoLayer = mapInfo.layers [i];
					break;
				}
			
			}

			for (int j = 0; j < floorLayer.tileDatas.Length; j++) {

				Tile floorTile = floorLayer.tileDatas [j];

				if (floorTile.walkable) {
					totalValablePosGridList.Add (floorTile.position);
				}

			}


			if (floorLayer == null || attachedInfoLayer == null || attachedItemInfoLayer == null) {
				Debug.LogError ("地图数据不完整");
			}

		}



		private Item GetAttachedItem(Vector2 position){

			Item attachedItem = null;

			for (int i = 0; i < attachedItemInfoLayer.tileDatas.Length; i++) {

				Tile attachedItemTile = attachedItemInfoLayer.tileDatas [i];

				AttachedItemType type = AttachedItemType.Floor;

				if (attachedItemTile.position == position) {
					type = (AttachedItemType)(attachedItemTile.tileIndex);
				}

				switch (type) {
				case AttachedItemType.Floor:
					attachedItem = Item.NewItemWith (515, 1);
					break;
				case AttachedItemType.Key:
					attachedItem = Item.NewItemWith (513, 1);
					break;
				case AttachedItemType.Medicine:
					attachedItem = Item.NewItemWith (500, 1);
					break;
				case AttachedItemType.PickAxe:
					attachedItem = Item.NewItemWith (512, 1);
					break;
				case AttachedItemType.Saw:
					attachedItem = Item.NewItemWith (516, 1);
					break;
				case AttachedItemType.Sickle:
					attachedItem = Item.NewItemWith (517, 1);
					break;

				}

			}

			return attachedItem;

		}

		/// <summary>
		/// 根据附加信息层数据初始化关卡的其他信息
		/// </summary>
		private void SetUpMapWithAttachedInfo(){

			for (int i = 0; i < attachedInfoLayer.tileDatas.Length; i++) {
				Tile attachedInfoTile = attachedInfoLayer.tileDatas [i];
				Vector2 pos = attachedInfoTile.position;
				AttachedInfoType attachedInfoType = (AttachedInfoType)(attachedInfoTile.tileIndex);
				switch (attachedInfoType) {
				 
				// 人物初始点 水晶 商人 npc 传送阵 门 木桶 瓦罐 宝箱 石头 树木 陷阱开关 陷阱关 陷阱开 可移动石板 boss 怪物
				case AttachedInfoType.PlayerOriginPosition:
					playerOriginalPosList.Add (pos);
					break;
				case AttachedInfoType.Crystal:
					SetUpLearnCrystal (pos);
					break;
				case AttachedInfoType.Trader:
					NPC trader = GameManager.Instance.gameDataCenter.allNpcs.Find (delegate(NPC obj) {
						return obj.npcId == 0;
					});
					SetUpNPC (trader, pos);
					break;
				case AttachedInfoType.NPC:

					break;
				case AttachedInfoType.Transport:
					GenerateMapItem (MapItemType.Transport, pos, null);
					break;
				case AttachedInfoType.Door:
					GenerateMapItem (MapItemType.Door, pos, null);
					break;
				case AttachedInfoType.Buck:
					Item attachedItem = GetAttachedItem (pos);
					GenerateMapItem (MapItemType.Buck, pos, attachedItem);
					break;
				case AttachedInfoType.Pot:
					attachedItem = GetAttachedItem (pos);
					GenerateMapItem (MapItemType.Pot, pos, attachedItem);
					break;
				case AttachedInfoType.TreasureBox:
					attachedItem = GetAttachedItem (pos);
					GenerateMapItem (MapItemType.TreasureBox, pos, attachedItem);
					break;
				case AttachedInfoType.Stone:
					GenerateMapItem (MapItemType.Stone, pos, null);
					break;
				case AttachedInfoType.Tree:
					GenerateMapItem (MapItemType.Tree, pos, null);
					break;
				case AttachedInfoType.Switch:
					GenerateMapItem (MapItemType.Switch, pos, null);
					break;
				case AttachedInfoType.TrapOn:
					Trap trapOn = GenerateMapItem (MapItemType.TrapOn, pos, null) as Trap;
					allTrapsInMap.Add (trapOn);
					break;
				case AttachedInfoType.TrapOff:
					Trap trapOff = GenerateMapItem (MapItemType.TrapOff, pos, null) as Trap;
					allTrapsInMap.Add (trapOff);
					break;
				case AttachedInfoType.MovableFloor:
					GenerateMapItem (MapItemType.MovableFloor, pos, null);
					break;
				case AttachedInfoType.Monster:
					SetUpMonster (pos);
					break;
				case AttachedInfoType.Boss:
					SetUpBoss (pos);
					break;
				
				}

			}
		}

		public MapItem GenerateMapItem(MapItemType mapItemType, Vector2 position, Item attachedItem = null){

			MapItem mapItem = null;

			switch (mapItemType) {
			case MapItemType.Door:
				mapItem = mapItemPool.GetInstance<Door> (doorModel.gameObject, mapItemsContainer);
				totalMapWalkableInfoArray [(int)(position.x), (int)(position.y)] = 0;
				break;
			case MapItemType.TreasureBox:
				mapItem = mapItemPool.GetInstance<TreasureBox> (lockedTreasureBoxModel.gameObject, mapItemsContainer);
				(mapItem as TreasureBox).rewardItem = attachedItem;
				totalMapWalkableInfoArray [(int)(position.x), (int)(position.y)] = 0;
				break;
			case MapItemType.Buck:
				mapItem = mapItemPool.GetInstance<TreasureBox> (buckModel.gameObject, mapItemsContainer);
				(mapItem as TreasureBox).rewardItem = attachedItem;
				totalMapWalkableInfoArray [(int)(position.x), (int)(position.y)] = 0;
				break;
			case MapItemType.Pot:
				mapItem = mapItemPool.GetInstance<TreasureBox> (potModel.gameObject, mapItemsContainer);
				(mapItem as TreasureBox).rewardItem = attachedItem;
				totalMapWalkableInfoArray [(int)(position.x), (int)(position.y)] = 0;
				break;
			case MapItemType.MovableFloor:
				mapItem = mapItemPool.GetInstance<MovableFloor> (movableFloorModel.gameObject, mapItemsContainer);
				totalMapWalkableInfoArray [(int)(position.x), (int)(position.y)] = 1;
				break;
			case MapItemType.Stone:
				mapItem = mapItemPool.GetInstance<Obstacle> (stoneModel.gameObject, mapItemsContainer);
				totalMapWalkableInfoArray [(int)(position.x), (int)(position.y)] = 0;
				break;
			case MapItemType.Switch:
				mapItem = mapItemPool.GetInstance<TrapSwitch> (trapSwitchModel.gameObject, mapItemsContainer);
				totalMapWalkableInfoArray [(int)(position.x), (int)(position.y)] = 0;
				break;
			case MapItemType.Transport:
				mapItem = mapItemPool.GetInstance<Transport> (transportModel.gameObject, mapItemsContainer);
				totalMapWalkableInfoArray [(int)(position.x), (int)(position.y)] = 0;
				break;
			case MapItemType.TrapOff:
				mapItem = mapItemPool.GetInstance<Trap> (trapModel.gameObject, mapItemsContainer);
				(mapItem as Trap).trapOn = false;
				(mapItem as Trap).mapItemType = MapItemType.TrapOff;
				totalMapWalkableInfoArray [(int)(position.x), (int)(position.y)] = 1;
				break;
			case MapItemType.TrapOn:
				mapItem = mapItemPool.GetInstance<Trap> (trapModel.gameObject, mapItemsContainer);
				(mapItem as Trap).trapOn = true;
				(mapItem as Trap).mapItemType = MapItemType.TrapOn;
				totalMapWalkableInfoArray [(int)(position.x), (int)(position.y)] = 1;
				break;
			case MapItemType.Tree:
				mapItem = mapItemPool.GetInstance<Obstacle> (treeModel.gameObject, mapItemsContainer);
				totalMapWalkableInfoArray [(int)(position.x), (int)(position.y)] = 0;
				break;
			}

			mapItem.SetSortingOrder (-(int)position.y);

			mapItem.transform.position = new Vector3 (position.x, position.y, -100f);


			allUnusedOtherItems.Add (mapItem.transform);

			return mapItem;
		}


		public void ChangeAllTrapStatusInMap(){
			for (int i = 0; i < allTrapsInMap.Count; i++) {
				allTrapsInMap [i].ChangeTrapStatus ();
			}
		}


		private Tile GetTileAtPosition(Layer layer,Vector3 position){
			Tile tile = null;
			for (int i = 0; i < layer.tileDatas.Length; i++) {
				Tile t = layer.tileDatas [i];
				if ((int)(t.position.x) == (int)(position.x) && 
					(int)(t.position.y) == (int)(position.y)) {
					tile = t;
				}
			}
			return tile;
		}

		/// <summary>
		/// 初始化地面和背景图，初始化地图上的基础可行走信息
		/// </summary>
		private void SetUpFloorAndBackground ()
		{

			// 获得地图图集
			string floorImageName = mapInfo.floorImageName;

			// 获得背景图片
			string backgroundImageName = mapInfo.backgroundImageName;

			Sprite backgroundSprite = GameManager.Instance.gameDataCenter.allMapSprites.Find (delegate(Sprite obj) {
				return obj.name == backgroundImageName;
			});

			Camera.main.transform.Find ("Background").GetComponent<SpriteRenderer> ().sprite = backgroundSprite;;

			// 创建地板
			for (int i = 0; i < floorLayer.tileDatas.Length; i++) {
				Tile tile = floorLayer.tileDatas [i];

				Transform floor = floorPool.GetInstance<Transform> (floorModel.gameObject, floorsContainer);
				floor.position = new Vector3 (tile.position.x, tile.position.y, -100f);
				if (tile.walkable) {
					totalMapWalkableInfoArray [(int)tile.position.x, (int)tile.position.y] = 1;
				}

				string tileSpriteName = string.Format ("{0}_{1}", floorImageName, tile.tileIndex);
				Sprite tileSprite = GameManager.Instance.gameDataCenter.allMapSprites.Find (delegate(Sprite obj) {
					return obj.name == tileSpriteName;
				});

				SpriteRenderer floorTileRenderer = floor.GetComponent<SpriteRenderer> ();
				floorTileRenderer.sprite = tileSprite;
				floorTileRenderer.sortingOrder = -(int)tile.position.y;
				allUnusedFloors.Add(floor);
//				Debug.Log (floor.position);
			}
		}

		private bool PositionSame(Vector3 position1,Vector3 position2){

			if (position1.x > position2.x - 0.2f &&
			    position1.x < position2.x + 0.2f &&
			    position1.y > position2.y - 0.2f &&
			    position1.y < position2.y + 0.2f) {
				return true;
			}

			return false;

		}

		/// <summary>
		/// 初始化玩家
		/// </summary>
		/// <param name="position">Position.</param>
		private void SetUpPlayer(){

			int randomIndex = Random.Range (0, playerOriginalPosList.Count);

			Vector3 position = playerOriginalPosList [randomIndex];

			Transform player = Player.mainPlayer.GetComponentInChildren<BattlePlayerController> ().transform;

			player.position = position;

			bpCtr = player.GetComponent<BattlePlayerController> ();

			bpCtr.ActiveBattlePlayer (true, false, false);

			bpCtr.SetSortingOrder (-(int)position.y);

			Transform floor = allUnusedFloors.Find (delegate(Transform obj) {
//				Debug.LogFormat("{0}----{1}",obj.position,position);
				return PositionSame(obj.position,position);
			});

//			floor.GetComponent<SpriteRenderer>().sortingLayerName = "Ground";

			floor.transform.position = new Vector3 (position.x, position.y, 0);

			mapWalkableInfoArray [(int)position.x, (int)position.y] = 1;

			allUnusedFloors.Remove (floor);

			ItemsAroundAutoIntoLifeWithBasePoint (position);

			bpCtr.singleMoveEndPos = position;

			player.rotation = Quaternion.identity;

			// 视角聚焦到玩家身上
			Camera.main.transform.SetParent (player, false);

			Camera.main.transform.rotation = Quaternion.identity;

			Camera.main.transform.localPosition = new Vector3 (0, 0, -10);

//			Camera.main.transform.Find ("Cover").gameObject.SetActive (true);



			// 默认进入关卡后播放的角色动画
			bpCtr.PlayRoleAnim ("wait", 0, null);

		}

		/// <summary>
		/// 获取场景中的玩家人物模型
		/// </summary>
		/// <returns>The battle player.</returns>
		private BattlePlayerController GetBattlePlayer(){

			if (bpCtr == null) {
				Transform player = Player.mainPlayer.GetComponentInChildren<BattlePlayerController> ().transform;
				bpCtr = player.GetComponent<BattlePlayerController> ();
			}

			return bpCtr;

		}

		/// <summary>
		/// 初始化单词记忆水晶
		/// </summary>
		/// <param name="position">Position.</param>
		private void SetUpLearnCrystal(Vector2 position){

			Transform learnCrystal = crystalPool.GetInstance<Transform> (crystalModel.gameObject, crystalsContainer);

			learnCrystal.position = new Vector3 (position.x, position.y, -100f);

			learnCrystal.GetComponent<SpriteRenderer> ().sortingOrder = -(int)(position.y);

			totalMapWalkableInfoArray [(int)(position.x), (int)(position.y)] = 0;

			allUnusedOtherItems.Add (learnCrystal);

		}


		/// <summary>
		/// 初始化制造台
		/// </summary>
		/// <param name="position">Position.</param>
		private void SetUpWorkBench(Vector2 position){

			workBench.position = new Vector3 (position.x, position.y, -100f);

			workBench.GetComponent<SpriteRenderer> ().sortingOrder = -(int)position.y;

			totalMapWalkableInfoArray[(int)position.x,(int)position.y] = 0;

			allUnusedOtherItems.Add (workBench);

		}

		/// <summary>
		/// 初始化地图上的npc
		/// </summary>
		/// <param name="npc">Npc.</param>
		/// <param name="position">Position.</param>
		private void SetUpNPC(NPC npc, Vector3 position){

			MapNPC mapNpc = npcPool.GetInstance<MapNPC> (mapNpcModel.gameObject, npcsContainer);

			mapNpc.transform.position = new Vector3 (position.x, position.y, -100f);

			SpriteRenderer npcSpriteRenderer = mapNpc.GetComponent<SpriteRenderer> ();

			Sprite npcIcon = GameManager.Instance.gameDataCenter.allMapSprites.Find (delegate(Sprite obj) {
				return obj.name == npc.spriteName;
			});

			npcSpriteRenderer.sprite = npcIcon;

			npcSpriteRenderer.sortingOrder = -(int)position.y;

			totalMapWalkableInfoArray [(int)position.x, (int)position.y] = 0;

			mapNpc.npc = npc;

			mapNpc.name = npc.npcName;

			mapNpc.GetComponent<BoxCollider2D> ().enabled = true;

			allUnusedOtherItems.Add (mapNpc.transform);

//			mapNpcs.Add (mapNpc);

		}


		/// <summary>
		/// 初始化地图上的怪物
		/// </summary>
		/// <param name="position">Position.</param>
		private void SetUpMonster(Vector2 position){

			// 随机拿到一个本关中的怪物
			int monsterIndexInData = Random.Range (0, levelData.monsterIds.Length);

			// 拿到怪物模型
			Transform monsterModel = levelData.monsters [monsterIndexInData];

			// 使用上面拿到的怪物模型初始化一个新的怪物
			Transform monster = monsterPool.GetInstanceWithName<Transform> (monsterModel.gameObject.name, monsterModel.gameObject, monstersContainer);

			totalMapWalkableInfoArray [(int)position.x, (int)position.y] = 0;

			monster.position = position;

			monster.gameObject.SetActive (true);

			BattleMonsterController bmCtr = monster.GetComponent<BattleMonsterController> ();

			bmCtr.PlayRoleAnim ("wait", 0, null);

//			bmCtr.SetSortingLayerName ("Hide");

			bmCtr.transform.position = new Vector3 (position.x, position.y, -100f);

			bmCtr.SetSortingOrder (-(int)position.y);

			allUnusedMonsters.Add (monster);

//			monsters.Add (monster.GetComponent<Monster> ());

		}

		private void SetUpBoss(Vector2 position){

		}


		public void ItemsAroundAutoIntoLifeWithBasePoint(Vector3 basePosition){

			Vector3 upPoint = new Vector3 (basePosition.x, basePosition.y + 1);
			Vector3 downPoint = new Vector3 (basePosition.x, basePosition.y - 1);
			Vector3 leftPoint = new Vector3 (basePosition.x - 1, basePosition.y);
			Vector3 rightPoint = new Vector3 (basePosition.x + 1, basePosition.y);



			float delay = 0;

			delay = ValidTilesAutoIntoLifeAtPoint (upPoint,delay);
			delay = ValidTilesAutoIntoLifeAtPoint (downPoint,delay);
			delay = ValidTilesAutoIntoLifeAtPoint (leftPoint,delay);
			ValidTilesAutoIntoLifeAtPoint (rightPoint,delay);
		}


		private float ValidTilesAutoIntoLifeAtPoint(Vector3 position,float delay){

			GetValidTiles (position);

			if (validUnusedTiles.Count == 0) {
				return delay;
			}

			bool continueInitAroundItems = validUnusedTiles.Count == 1 && validUnusedTiles [0].type == UnusedTileType.Floor;
				


			Transform floor = null;
			Transform other = null;

			for (int i = 0; i < validUnusedTiles.Count; i++) {
				UnusedTile tile = validUnusedTiles [i];
				if (tile.type == UnusedTileType.Floor) {
					floor = tile.tileTransform;
				} else if (tile.type == UnusedTileType.Other) {
					other = tile.tileTransform;
				}
			}

			IEnumerator otherComeIntoLife = other != null ? OtherComeIntoLife (other) : null;

			IEnumerator floorComeIntoLife = FloorComeIntoLife (floor,otherComeIntoLife,continueInitAroundItems,delay);

			StartCoroutine (floorComeIntoLife);

			float delayBase = 0.3f;

			delay += delayBase;

			return delay;

		}

		public float floorComeIntoLifeInterval = 1f;

		public float floorOriginalScale = 0.2f;
		public float floorOriginalPositionOffsetX = 3f;
		public float floorOriginalPositionOffsetY = 3f;

		private float scalerIncreaseBase{
			get{ return (1 - floorOriginalScale) / floorComeIntoLifeInterval; }
		}

		private float positionYFixBase{
			get{ return -floorOriginalPositionOffsetY / floorComeIntoLifeInterval; }
		}
		private float positionXFixBase{
			get{ return -floorOriginalPositionOffsetX / floorComeIntoLifeInterval; }
		}

		private IEnumerator FloorComeIntoLife(Transform floor,IEnumerator otherComeIntoLife,bool continueInitAroundItems,float delay){

			yield return new WaitForSeconds (delay);

			float timer = 0;

			Vector3 floorTargetPosition = new Vector3 (floor.position.x, floor.position.y, 0);

			float floorScale = floorOriginalScale;

			floor.localScale = new Vector3 (floorOriginalScale, floorOriginalScale, 1f);

			floor.position = new Vector3 (floorTargetPosition.x + floorOriginalPositionOffsetX, floorTargetPosition.y + floorOriginalPositionOffsetY, 0);

//			floor.GetComponent<SpriteRenderer>().sortingLayerName = "Ground";

			Vector3 positionFix = new Vector3 (positionXFixBase * Time.fixedDeltaTime, positionYFixBase * Time.fixedDeltaTime, 0);

			while (timer < floorComeIntoLifeInterval) {
				
				floorScale += scalerIncreaseBase * Time.fixedDeltaTime;

				floor.localScale = new Vector3 (floorScale, floorScale, 1);;

				floor.position = floor.position + positionFix;

				timer += Time.fixedDeltaTime;

				yield return null;
			}

			floor.localScale = Vector3.one;
			floor.position = floorTargetPosition;

			if (otherComeIntoLife != null) {
				StartCoroutine (otherComeIntoLife);
			} else {
				mapWalkableInfoArray [(int)(floor.position.x), (int)(floor.position.y)] = totalMapWalkableInfoArray [(int)(floor.position.x), (int)(floor.position.y)];
			}

			if (continueInitAroundItems) {
				ItemsAroundAutoIntoLifeWithBasePoint (floorTargetPosition);
			}
		}

		public float otherComeIntoLifeInterval = 0.3f;

		public float otherOriginalPositionOffsetY = 1f;

		public float otherPositonYFixBase{
			get{return -otherOriginalPositionOffsetY / otherComeIntoLifeInterval;}
		}

		private IEnumerator OtherComeIntoLife(Transform other){

			Vector3 otherTargetPosition = new Vector3 (other.position.x, other.position.y, 0);

			other.position = new Vector3 (otherTargetPosition.x, otherTargetPosition.y + otherOriginalPositionOffsetY, 0);

			float timer = 0;

			Vector3 positionFix = new Vector3 (0, otherPositonYFixBase * Time.fixedDeltaTime, 0);

			while (timer < otherComeIntoLifeInterval) {

				timer += Time.fixedDeltaTime;

				other.position = other.position + positionFix;

				yield return null;
			}

			other.position = otherTargetPosition;

			mapWalkableInfoArray [(int)other.position.x, (int)other.position.y] = 0;

		}


		private enum UnusedTileType{
			Floor,
			Other
		}

		private class UnusedTile
		{
			public Transform tileTransform;
			public UnusedTileType type;

			public UnusedTile(Transform tileTrans,UnusedTileType type){
				this.tileTransform = tileTrans;
				this.type = type;
			}
		}

		private List<UnusedTile> validUnusedTiles = new List<UnusedTile>();

		private void GetValidTiles(Vector3 position){

			validUnusedTiles.Clear ();

			for (int i = 0; i < allUnusedMonsters.Count; i++) {
				Transform monster = allUnusedMonsters [i];
				if(PositionSame(position,monster.position)){
					validUnusedTiles.Add(new UnusedTile(monster,UnusedTileType.Other));
					allUnusedMonsters.Remove (monster);
					break;
				}
			}

			for (int i = 0; i < allUnusedOtherItems.Count; i++) {
				Transform mapItem = allUnusedOtherItems [i];
				if (PositionSame(position,mapItem.position)) {
					validUnusedTiles.Add(new UnusedTile(mapItem,UnusedTileType.Other));
					allUnusedOtherItems.Remove (mapItem);
					break;
				}
			}

			for (int i = 0; i < allUnusedFloors.Count; i++) {
				Transform floor = allUnusedFloors [i];
				if (PositionSame(position,floor.position)) {
					validUnusedTiles.Add(new UnusedTile(floor,UnusedTileType.Floor));
					allUnusedFloors.Remove (floor);
					break;
				}
			}
		}

		public Transform GetSkillEffect(Transform agentTrans){
			Transform skillEffect = skillEffectPool.GetInstance<Transform> (skillEffectModel.gameObject, skillEffectsContainer);
			skillEffect.position = agentTrans.position;
			skillEffect.localScale = agentTrans.localScale;
			skillEffect.rotation = Quaternion.identity;
			return skillEffect;
		}

		public void AddSkillEffectToPool(Transform skillEffect){
			skillEffectPool.AddInstanceToPool (skillEffect.gameObject);
		}
			

		public void PlayDestinationAnim(Vector3 targetPos,bool arrivable){

			destinationAnimation.position = targetPos;

			Animator destinationAnimator = destinationAnimation.GetComponent<Animator> ();

			destinationAnimator.ResetTrigger ("PlayArrivable");
			destinationAnimator.ResetTrigger ("PlayUnarrivable");

			if (arrivable) {
				destinationAnimator.SetTrigger ("PlayArrivable");
			} else {
				destinationAnimator.SetTrigger ("PlayUnarrivable");
			}


		}

		public void PlayMapOtherAnim(string triggerName,Vector3 targetPos){

			Transform otherAnimation = otherAnimPool.GetInstance<Transform> (otherAnimationModel.gameObject, otherAnimContainer);

			otherAnimation.position = targetPos;

			Animator otherAnimator = otherAnimation.GetComponent<Animator> ();

			otherAnimator.SetTrigger (triggerName);

			IEnumerator coroutine = CollectOtherAnimToPoolWhenAnimEnd (otherAnimator, triggerName);

			StartCoroutine (coroutine);

		}

		private IEnumerator CollectOtherAnimToPoolWhenAnimEnd(Animator anim,string trigger){

			yield return null;

			AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo (0);

			while (info.normalizedTime < 1.0f) {
				yield return null;
				info = anim.GetCurrentAnimatorStateInfo (0);
			}

			otherAnimPool.AddInstanceToPool (anim.gameObject);

			anim.ResetTrigger (trigger);

		}
			

		private class RewardInMap
		{
			public Transform rewardTrans;
			public Item reward;

			public RewardInMap(Transform rewardTrans,Item reward){
				this.rewardTrans = rewardTrans;
				this.reward = reward;
			}


		}

		public void SetUpRewardInMap(Item reward, Vector3 rewardPosition){

			Debug.Log (reward.itemName);

			Transform rewardTrans = rewardItemPool.GetInstance<Transform> (rewardItemModel.gameObject, rewardsContainer);

			SpriteRenderer sr = rewardTrans.GetComponent<SpriteRenderer> ();

			Sprite s = null;

			if (reward.itemType == ItemType.Material) {
				s = GameManager.Instance.gameDataCenter.allMaterialSprites.Find (delegate(Sprite obj) {
					return obj.name == reward.spriteName;
				});
			} else {
				s = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
					return obj.name == reward.spriteName;
				});
			}

			sr.sprite = s;

			rewardTrans.position = rewardPosition;

			sr.sortingOrder = -(int)rewardPosition.y;

			RewardInMap rewardInMap = new RewardInMap (rewardTrans, reward);

			StartCoroutine ("RewardFlyToPlayer", rewardInMap);

		}

		private IEnumerator RewardFlyToPlayer(RewardInMap rewardInMap){

			Transform rewardTrans = rewardInMap.rewardTrans;

			yield return new WaitForSeconds (2.0f);

			float passedTime = 0;

			float leftTime = rewardFlyDuration - passedTime;

			BattlePlayerController bpCtr = GetBattlePlayer ();

//			Vector3 playerPos = bpCtr.transform.position;
//
//			Vector3 rewardPos = rewardInMap.position;

//			float distance = (bpCtr.transform.position - rewardInMap.position).sqrMagnitude;

			float distance = Mathf.Sqrt (Mathf.Pow ((bpCtr.transform.position.x - rewardTrans.position.x), 2.0f) 
				+ Mathf.Pow ((bpCtr.transform.position.y - rewardTrans.position.y), 2.0f));

			while (distance > 0.5f) {

				Vector3 rewardVelocity = new Vector3 ((bpCtr.transform.position.x - rewardTrans.position.x) / leftTime, 
					(bpCtr.transform.position.y - rewardTrans.position.y) / leftTime, 0);

				Vector3 newRewardPos = new Vector3 (rewardTrans.position.x + rewardVelocity.x * Time.deltaTime, 
					rewardTrans.position.y + rewardVelocity.y * Time.deltaTime);

				rewardTrans.position = newRewardPos;

//				rewardInMap.position = Vector3.MoveTowards(rewardInMap.position,bpCtr.transform.position,

				passedTime += Time.deltaTime;

				leftTime = rewardFlyDuration - passedTime;

				distance = Mathf.Sqrt (Mathf.Pow ((bpCtr.transform.position.x - rewardTrans.position.x), 2.0f) 
					+ Mathf.Pow ((bpCtr.transform.position.y - rewardTrans.position.y), 2.0f));

				yield return null;

			}

			Item reward = rewardInMap.reward;

			Debug.LogFormat ("获得{0}", reward.itemName);

			Player.mainPlayer.AddItem (reward);

			GetComponent<ExploreManager> ().ObtainReward (reward);

			rewardTrans.position = new Vector3 (0, 0, -100);

			rewardItemPool.AddInstanceToPool (rewardTrans.gameObject);

		}


		public void DestroyInstancePools(){

//			mapItems = null;
//			mapNpcs = null;
//			monsters = null;

			Destroy (outerWallPool.gameObject);
			Destroy (floorPool.gameObject);
			Destroy (npcPool.gameObject);
			Destroy (mapItemPool.gameObject);
			Destroy (monsterPool.gameObject);
			Destroy (crystalPool.gameObject);

		}


		/// <summary>
		/// 将场景中的地板，npc，地图物品，怪物加入缓存池中
		/// </summary>
		private void MapInstancesToPool(){
			floorPool.AddChildInstancesToPool (floorsContainer);
			npcPool.AddChildInstancesToPool (npcsContainer);
			mapItemPool.AddChildInstancesToPool (mapItemsContainer);
			monsterPool.AddChildInstancesToPool (monstersContainer);
			crystalPool.AddChildInstancesToPool (crystalsContainer);

		}

		/// <summary>
		/// 每关初始化完毕后清除缓存池中没有复用到的游戏体
		/// </summary>
		private void ClearPools(){
			outerWallPool.ClearInstancePool ();
			floorPool.ClearInstancePool ();
			npcPool.ClearInstancePool ();
			mapItemPool.ClearInstancePool ();
			monsterPool.ClearInstancePool ();

		}


	}
		
}
