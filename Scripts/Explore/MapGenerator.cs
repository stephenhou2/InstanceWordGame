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
		public Transform outerWallsContainer;
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

		public Animator destinationAnimator;

		public Animator otherAnimModel;

		// 关卡数据
		private GameLevelData levelData;

		public UnityEngine.Material spriteMaterial; 

//		private List <Vector3> gridPositions = new List <Vector3> ();	//A list of possible locations to place tiles.

		public int[,] mapWalkableInfoArray;

		public float rewardFlyDuration;

		private BattlePlayerController bpCtr;

		// 获得地板层和附加信息层的数据
		private Layer floorLayer = null;
		private Layer attachedInfoLayer = null;


		// 障碍物模型数组
		public Obstacle[] treeModels;
		public Obstacle stoneModel;


		// 陷阱模型
		public Trap trapModel;

		// 开关模型
		public TrapSwitch trapSwitchModel;

		// 宝箱模型
		public TreasureBox lockedTreasureBoxModel;

		// 正常箱子模型
		public TreasureBox[] normalTreasureBoxModels;

		// 传送阵模型
		public Transport transportModel;

		// 可移动地板模型
		public MovableFloor movableFloorModel;

		// 门模型
		public Door doorModel;

		private List<Trap> allTrapsInMap;

		private List<Vector3> totalValablePosGridList;
//		private List<Vector3> treasureBox_npc_produceBuildingPosGridList;
//		private List<Vector3> learnCrystalPosGridList;
//		private List<Vector3> movableFloorPosGridList;
//		private Vector3 bossPos;
//		private List<Vector3> undestroyableObstacleGridList;

		void Awake(){
//			mapItemGenerator = GetComponent<MapItemGenerator> ();
			totalValablePosGridList = new List<Vector3> ();
//			treasureBox_npc_produceBuildingPosGridList = new List<Vector3> ();
//			learnCrystalPosGridList = new List<Vector3> ();
//			movableFloorPosGridList = new List<Vector3> ();
//			undestroyableObstacleGridList = new List<Vector3> ();

			allTrapsInMap = new List<Trap> ();
		}

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

			// 地图上的可行走信息数组
			mapWalkableInfoArray = new int[columns,rows];

			ResetMapWalkableInfoArray ();

			allTrapsInMap.Clear ();

			// 初始化地图原始数据
			InitMapDatas();

			// 初始化地面和背景
			SetUpFloorAndBackground();

			SetUpMapWithAttachedInfo ();

			// 初始化地图物品
//			SetUpItems ();

			// 初始化地图NPC
//			SetUpNPCs ();

			// 初始化地图怪物
//			SetUpMonsters ();

			// 初始化地图上的事件建筑
//			SetUpBuildings();

			ClearPools ();

		}

		/// <summary>
		/// 将地图范围内的所有点都设置为不可行走点
		/// </summary>
		private void ResetMapWalkableInfoArray (){
			for (int i = 0; i < columns; i++) {
				for (int j = 0; j < rows ; j++) {
					mapWalkableInfoArray [i, j] = -1;
				}
			}
		}


		/// <summary>
		/// 地图数据分离出地板层数据和附加信息层数据
		/// 根据地板层数据初始化基础的地图可行走信息
		/// </summary>
		private void InitMapDatas(){

			for (int i = 0; i < mapInfo.layers.Length; i++) {
				if (mapInfo.layers [i].name == "FloorLayer") {
					floorLayer = mapInfo.layers [i];
				} else if (mapInfo.layers [i].name == "AttachedInfoLayer") {
					attachedInfoLayer = mapInfo.layers [i];
				}
			}

			for (int j = 0; j < floorLayer.tileDatas.Length; j++) {

				Tile floorTile = floorLayer.tileDatas [j];

				if (floorTile.walkable) {
					totalValablePosGridList.Add (floorTile.position);
				}

			}


			if (floorLayer == null || attachedInfoLayer == null) {
				Debug.LogError ("地图数据不完整");
			}

		}



		/// <summary>
		/// 根据附加信息层数据初始化关卡的其他信息
		/// </summary>
		private void SetUpMapWithAttachedInfo(){
			for (int i = 0; i < attachedInfoLayer.tileDatas.Length; i++) {
				Tile attachedInfoTile = attachedInfoLayer.tileDatas [i];
				Vector2 pos = attachedInfoTile.position;
				AttachedInfoType attachedInfo = (AttachedInfoType)(attachedInfoTile.tileIndex);
				switch (attachedInfo) {
				case AttachedInfoType.PlayerOriginPosition:
					SetUpPlayer (pos);
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
				case AttachedInfoType.Monster:
					SetUpMonster (pos);
					break;
				case AttachedInfoType.Boss:
					SetUpBoss (pos);
					break;
				case AttachedInfoType.TreasureBox:
					GenerateMapItem (MapItemType.LockedTreasureBox, pos, null);
					break;
				case AttachedInfoType.Key:
					Item key = Item.NewItemWith (513, 1);
					GenerateMapItem (MapItemType.NormalTreasureBox, pos, key);
					break;
				case AttachedInfoType.Tree:
					GenerateMapItem (MapItemType.Tree, pos, null);
					break;
				case AttachedInfoType.Stone:
					GenerateMapItem (MapItemType.Stone, pos, null);
					break;
				case AttachedInfoType.Pickaxe:
					Item pickAxe = Item.NewItemWith(512,1);
					GenerateMapItem (MapItemType.NormalTreasureBox, pos, pickAxe);
					break;
				case AttachedInfoType.Door:
					GenerateMapItem (MapItemType.Door, pos, null);
					break;
				case AttachedInfoType.TrapOff:
					Trap trapOff = GenerateMapItem (MapItemType.TrapOff, pos, null) as Trap;
					allTrapsInMap.Add (trapOff);
					break;
				case AttachedInfoType.TrapOn:
					Trap trapOn = GenerateMapItem (MapItemType.TrapOn, pos, null) as Trap;
					allTrapsInMap.Add (trapOn);
					break;
				case AttachedInfoType.Switch:
					GenerateMapItem (MapItemType.Switch, pos, null);
					break;
				case AttachedInfoType.Medicine:
					Item healthMedicine = Item.NewItemWith (500, 1);
					GenerateMapItem (MapItemType.NormalTreasureBox, pos, healthMedicine);
					break;
				case AttachedInfoType.Scroll:
					GenerateMapItem (MapItemType.LockedTreasureBox, pos, null);
					break;
				case AttachedInfoType.MovableFloor:
					GenerateMapItem (MapItemType.MovableFloor, pos, null);
					break;
				case AttachedInfoType.Transport:
					GenerateMapItem (MapItemType.Transport, pos, null);
					break;

				}

			}
		}

		public MapItem GenerateMapItem(MapItemType mapItemType, Vector2 position, Item rewardItem = null){

			MapItem mapItem = null;

			switch (mapItemType) {
			case MapItemType.Door:
				mapItem = mapItemPool.GetInstance<Door> (doorModel.gameObject, mapItemsContainer);
				mapWalkableInfoArray [(int)(position.x), (int)(position.y)] = 0;
				break;
			case MapItemType.LockedTreasureBox:
				mapItem = mapItemPool.GetInstance<TreasureBox> (lockedTreasureBoxModel.gameObject, mapItemsContainer);
				(mapItem as TreasureBox).rewardItem = rewardItem;
				mapWalkableInfoArray [(int)(position.x), (int)(position.y)] = 0;
				break;
			case MapItemType.NormalTreasureBox:
				int randomNormalTbIndex = Random.Range (0, normalTreasureBoxModels.Length);
				TreasureBox randomNormalTb = normalTreasureBoxModels [randomNormalTbIndex];
				mapItem = mapItemPool.GetInstance<TreasureBox> (randomNormalTb.gameObject, mapItemsContainer);
				(mapItem as TreasureBox).rewardItem = rewardItem;
				mapWalkableInfoArray [(int)(position.x), (int)(position.y)] = 0;
				break;
			case MapItemType.MovableFloor:
				mapItem = mapItemPool.GetInstance<MovableFloor> (movableFloorModel.gameObject, mapItemsContainer);
				mapWalkableInfoArray [(int)(position.x), (int)(position.y)] = 1;

				break;
			case MapItemType.Stone:
				mapItem = mapItemPool.GetInstance<Obstacle> (stoneModel.gameObject, mapItemsContainer);
				mapWalkableInfoArray [(int)(position.x), (int)(position.y)] = 0;
				break;
			case MapItemType.Switch:
				mapItem = mapItemPool.GetInstance<TrapSwitch> (trapSwitchModel.gameObject, mapItemsContainer);
				mapWalkableInfoArray [(int)(position.x), (int)(position.y)] = 0;
				break;
			case MapItemType.Transport:
				mapItem = mapItemPool.GetInstance<Transport> (transportModel.gameObject, mapItemsContainer);
				mapWalkableInfoArray [(int)(position.x), (int)(position.y)] = 0;
				break;
			case MapItemType.TrapOff:
				mapItem = mapItemPool.GetInstance<Trap> (trapModel.gameObject, mapItemsContainer);
				(mapItem as Trap).trapOn = false;
				mapWalkableInfoArray [(int)(position.x), (int)(position.y)] = 1;
				break;
			case MapItemType.TrapOn:
				mapItem = mapItemPool.GetInstance<Trap> (trapModel.gameObject, mapItemsContainer);
				(mapItem as Trap).trapOn = true;
				mapWalkableInfoArray [(int)(position.x), (int)(position.y)] = 1;
				break;
			case MapItemType.Tree:
				int randomTreeModelIndex = Random.Range (0, treeModels.Length);
				Obstacle randomTreeModel = treeModels [randomTreeModelIndex];
				mapItem = mapItemPool.GetInstance<Obstacle> (randomTreeModel.gameObject, mapItemsContainer);
				mapWalkableInfoArray [(int)(position.x), (int)(position.y)] = 0;
				break;
			}


			mapItem.transform.position = position;

			mapItem.SetSortingOrder (-(int)position.y);

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

				Transform floorTile = floorPool.GetInstance<Transform> (floorModel.gameObject, floorsContainer);
				floorTile.position = tile.position;
				if (tile.walkable) {
					mapWalkableInfoArray [(int)tile.position.x, (int)tile.position.y] = 1;
				}

				string tileSpriteName = string.Format ("{0}_{1}", floorImageName, tile.tileIndex);
				Sprite tileSprite = GameManager.Instance.gameDataCenter.allMapSprites.Find (delegate(Sprite obj) {
					return obj.name == tileSpriteName;
				});

				floorTile.GetComponent<SpriteRenderer> ().sprite = tileSprite;

			}
		}

		/// <summary>
		/// 初始化玩家
		/// </summary>
		/// <param name="position">Position.</param>
		private void SetUpPlayer(Vector2 position){

			Transform player = Player.mainPlayer.GetComponentInChildren<BattlePlayerController> ().transform;

			bpCtr = player.GetComponent<BattlePlayerController> ();

			player.position = position;

			bpCtr.SetOrderInLayer (-(int)position.y);

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

			learnCrystal.position = position;

			learnCrystal.GetComponent<SpriteRenderer> ().sortingOrder = -(int)(position.y);

			mapWalkableInfoArray [(int)(position.x), (int)(position.y)] = 0;

		}


		/// <summary>
		/// 初始化制造台
		/// </summary>
		/// <param name="position">Position.</param>
		private void SetUpWorkBench(Vector2 position){

			workBench.position = position;

			workBench.GetComponent<SpriteRenderer> ().sortingOrder = -(int)position.y;

			mapWalkableInfoArray[(int)position.x,(int)position.y] = 0;

		}

		/// <summary>
		/// 初始化地图上的npc
		/// </summary>
		/// <param name="npc">Npc.</param>
		/// <param name="position">Position.</param>
		private void SetUpNPC(NPC npc, Vector3 position){

			MapNPC mapNpc = npcPool.GetInstance<MapNPC> (mapNpcModel.gameObject, npcsContainer);

			mapNpc.transform.position = position;

			SpriteRenderer npcSpriteRenderer = mapNpc.GetComponent<SpriteRenderer> ();

			Sprite npcIcon = GameManager.Instance.gameDataCenter.allMapSprites.Find (delegate(Sprite obj) {
				return obj.name == npc.spriteName;
			});

			npcSpriteRenderer.sprite = npcIcon;

			npcSpriteRenderer.sortingOrder = -(int)position.y;

			mapWalkableInfoArray [(int)position.x, (int)position.y] = 0;

			mapNpc.npc = npc;

			mapNpc.name = npc.npcName;

			mapNpc.GetComponent<BoxCollider2D> ().enabled = true;

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

			monster.GetComponent<UnityArmatureComponent> ().sortingOrder = -(int)position.y;

			mapWalkableInfoArray [(int)position.x, (int)position.y] = 0;

			monster.position = position;

			monster.gameObject.SetActive (true);

			BattleMonsterController bmCtr = monster.GetComponent<BattleMonsterController> ();

			bmCtr.PlayRoleAnim ("wait", 0, null);

//			monsters.Add (monster.GetComponent<Monster> ());

		}

		private void SetUpBoss(Vector2 position){

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




//		private void SetUpItems(MapItemType mapItemType, Vector2 position, Item item = null){
//
//			List<MapItem> randomMapItems = mapItemGenerator.InitMapItems (levelData, itemPool, itemsContainer);
//
//			for (int i = 0; i < randomMapItems.Count; i++) {
//
//				MapItem mapItem = randomMapItems [i];
//
//				Vector3 pos = Vector3.zero;
//
//				switch (mapItem.mapItemType) {
//				case MapItemType.TreasureBox:
//					pos = RandomPosition (treasureBox_npc_produceBuildingPosGridList);
//					break;
//				case MapItemType.Obstacle:
//					pos = RandomPosition (totalValablePosGridList);
//					break;
//				case MapItemType.Trap:
//					pos = RandomPosition (totalValablePosGridList);
//					break;
//				case MapItemType.TrapSwitch:
//					pos = RandomPosition (treasureBox_npc_produceBuildingPosGridList);
//					break;
//				default:
//					break;
//				}
//
//				mapItem.transform.position = pos;
//
//				mapItem.GetComponent<SpriteRenderer> ().sortingOrder = -(int)pos.y;
//
//				if (mapItem.mapItemType == MapItemType.Trap) {
//					mapWalkableInfoArray [(int)pos.x, (int)pos.y] = 1;
//				} else {
//					mapWalkableInfoArray [(int)pos.x, (int)pos.y] = 0;
//				}
//
//
//				if (mapItem != null) {
//					mapItems.Add (mapItem);
//				}
//
//			}
//
//		}
//


		private T RandomEvent<T>(List<T> eventsList){

			int index = Random.Range (0, eventsList.Count);

			return eventsList [index];

		}


		public void PlayDestinationAnim(Vector3 targetPos,bool arrivable){

			destinationAnimator.transform.position = targetPos;

			destinationAnimator.ResetTrigger ("PlayArrivable");
			destinationAnimator.ResetTrigger ("PlayUnarrivable");

			if (arrivable) {
				destinationAnimator.SetTrigger ("PlayArrivable");
			} else {
				destinationAnimator.SetTrigger ("PlayUnarrivable");
			}


		}

		public void PlayMapOtherAnim(string triggerName,Vector3 targetPos){

			Animator otherAnim = otherAnimPool.GetInstance<Animator> (otherAnimModel.gameObject, otherAnimContainer);

			otherAnim.transform.position = targetPos;


			otherAnim.SetTrigger (triggerName);

			IEnumerator coroutine = CollectOtherAnimToPoolWhenAnimEnd (otherAnim, triggerName);

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
			


			

		//RandomPosition returns a random position from our list gridPositions.
		private Vector3 RandomPosition (List<Vector3> gridPositions)
		{
			if (gridPositions.Count <= 0) {
				Debug.LogError("位置数组元素数量小于地图元素设计数量");
			}
			//Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
			int randomIndex = Random.Range (0, gridPositions.Count);

			//Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
			Vector3 randomPosition = gridPositions[randomIndex];

//			Debug.Log (mapWalkableInfoArray.Length);
//			Debug.Log(string.Format("{0}/{1}",randomPosition.x,randomPosition.y));

			while (mapWalkableInfoArray [(int)randomPosition.x, (int)randomPosition.y] == -1) {
				randomIndex = Random.Range (0, gridPositions.Count);
				randomPosition = gridPositions [randomIndex];
			}

			//Remove the entry at randomIndex from the list so that it can't be re-used.
			gridPositions.RemoveAt (randomIndex);

			//Return the randomly selected Vector3 position.
			return randomPosition;
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
		/// 将场景中的墙体，地板，npc，地图物品，怪物加入缓存池中
		/// </summary>
		private void MapInstancesToPool(){

			outerWallPool.AddChildInstancesToPool (outerWallsContainer);
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
