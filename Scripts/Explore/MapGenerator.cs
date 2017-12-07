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
		private MapItemGenerator mapItemGenerator;

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
		private List<MapItem> mapItems = new List<MapItem> ();

		// 地图上所有npc列表
		private List<MapNPC> mapNpcs = new List<MapNPC>();

		// 地图上所有怪物列表
		private List<Monster>monsters = new List<Monster>();

		// 所有地图元素在场景中的父容器
		public Transform outerWallsContainer;
		public Transform floorsContainer;
		public Transform itemsContainer;
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
		private InstancePool itemPool;
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

		private List <Vector3> gridPositions = new List <Vector3> ();	//A list of possible locations to place tiles.

		public int[,] mapWalkableInfoArray;

		public float rewardFlyDuration;

		private BattlePlayerController bpCtr;

		// 获得地板层和附加信息层的数据
		private Layer floorLayer = null;
		private Layer attachedInfoLayer = null;

//		public SpriteRenderer background;

		private List<Vector3> totalValablePosGridList;
		private List<Vector3> treasureBox_npc_produceBuildingPosGridList;
		private List<Vector3> learnCrystalPosGridList;
		private List<Vector3> movableFloorPosGridList;
		private Vector3 bossPos;
		private List<Vector3> undestroyableObstacleGridList;

		void Awake(){
			mapItemGenerator = GetComponent<MapItemGenerator> ();
			totalValablePosGridList = new List<Vector3> ();
			treasureBox_npc_produceBuildingPosGridList = new List<Vector3> ();
			learnCrystalPosGridList = new List<Vector3> ();
			movableFloorPosGridList = new List<Vector3> ();
			undestroyableObstacleGridList = new List<Vector3> ();
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
				itemPool = InstancePool.GetOrCreateInstancePool ("ItemPool",poolContainerOfExploreScene.name);
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
			mapWalkableInfoArray = new int[rows,columns];

			ResetMapWalkableInfoArray ();

			// 初始化地图原始数据
			InitMapDatas();

			// 初始化地面和背景
			SetUpFloorAndBackground();

			// 初始化玩家
			SetUpPlayer ();

			// 初始化地图物品
			SetUpItems ();

			// 初始化地图NPC
			SetUpNPCs ();

			// 初始化地图怪物
			SetUpMonsters ();

			// 初始化地图上的事件建筑
			SetUpBuildings();

			ClearPools ();

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

		private BattlePlayerController GetBattlePlayer(){

			if (bpCtr == null) {
				Transform player = Player.mainPlayer.GetComponentInChildren<BattlePlayerController> ().transform;
				bpCtr = player.GetComponent<BattlePlayerController> ();
			}

			return bpCtr;

		}

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


			for (int j = 0; j < attachedInfoLayer.tileDatas.Length; j++) {
				
				Tile attachedInfoTile = attachedInfoLayer.tileDatas [j];

				if (attachedInfoTile.tileIndex >= 0) {
					switch ((AttachedInfoType)attachedInfoTile.tileIndex) {
					case AttachedInfoType.TreasureBoxOrNPCOrProduceBuilding:
						treasureBox_npc_produceBuildingPosGridList.Add (attachedInfoTile.position);
						break;
					case AttachedInfoType.LearnCrystal:
						learnCrystalPosGridList.Add (attachedInfoTile.position);
						break;
					case AttachedInfoType.MovableFloor:
						movableFloorPosGridList.Add (attachedInfoTile.position);
						break;
					case AttachedInfoType.Boss:
						bossPos = attachedInfoTile.position;
						break;
					case AttachedInfoType.UndestoryableObstacle:
						undestroyableObstacleGridList.Add (attachedInfoTile.position);
						break;

					}

					totalValablePosGridList.Remove (attachedInfoTile.position);
				}
			}


			if (floorLayer == null || attachedInfoLayer == null) {
				Debug.LogError ("地图数据不完整");
			}

		}

		private void SetUpPlayer(){

			Transform player = Player.mainPlayer.GetComponentInChildren<BattlePlayerController> ().transform;

			bpCtr = player.GetComponent<BattlePlayerController> ();

			// 随机玩家初始位置
			Vector3 playerOriginPos = RandomPosition(totalValablePosGridList);

			player.position = playerOriginPos;

			bpCtr.SetOrderInLayer (-(int)playerOriginPos.y);

			bpCtr.singleMoveEndPos = playerOriginPos;

			player.rotation = Quaternion.identity;

			// 视角聚焦到玩家身上
			Camera.main.transform.SetParent (player, false);

			Camera.main.transform.rotation = Quaternion.identity;

			Camera.main.transform.localPosition = new Vector3 (0, 0, -10);

//			Camera.main.transform.Find ("Cover").gameObject.SetActive (true);

			// 默认进入关卡后播放的角色动画
			bpCtr.PlayRoleAnim ("wait", 0, null);

		}

		private void SetUpItems(){

//			int mapItemCount = Random.Range (levelData.itemCount.minimum, levelData.itemCount.maximum + 1);

//			int mapItemCount = 50;

			List<MapItem> randomMapItems = mapItemGenerator.InitMapItems (levelData.normalItems,levelData.lockedItems, itemPool, itemsContainer);

			for (int i = 0; i < randomMapItems.Count; i++) {

				MapItem mapItem = randomMapItems [i];

				Vector3 pos = Vector3.zero;

				switch (mapItem.mapItemType) {
				case MapItemType.TreasureBox:
					pos = RandomPosition (treasureBox_npc_produceBuildingPosGridList);
					break;
				case MapItemType.Obstacle:
					pos = RandomPosition (totalValablePosGridList);
					break;
				case MapItemType.Trap:
					pos = RandomPosition (totalValablePosGridList);
					break;
				case MapItemType.TrapSwitch:
					pos = RandomPosition (treasureBox_npc_produceBuildingPosGridList);
					break;
				default:
					break;
				}

				mapItem.transform.position = pos;

				mapItem.GetComponent<SpriteRenderer> ().sortingOrder = -(int)pos.y;

				if (mapItem.mapItemType == MapItemType.Trap) {
					mapWalkableInfoArray [(int)pos.x, (int)pos.y] = 1;
				} else {
					mapWalkableInfoArray [(int)pos.x, (int)pos.y] = 0;
				}


				if (mapItem != null) {
					mapItems.Add (mapItem);
				}

			}

		}

		private void SetUpNPCs(){

			for (int i = 0; i < levelData.npcs.Count; i++) {

				NPC npc = levelData.npcs [i];

				Vector3 pos = RandomPosition (treasureBox_npc_produceBuildingPosGridList);

				mapWalkableInfoArray [(int)pos.x, (int)pos.y] = 0;

				MapNPC mapNpc = npcPool.GetInstance<MapNPC> (mapNpcModel.gameObject, npcsContainer);

				mapNpc.transform.position = pos;

				mapNpc.GetComponent<SpriteRenderer> ().sortingOrder = -(int)pos.y;

				mapNpc.npc = npc;

				mapNpc.name = npc.npcName;

				mapNpc.GetComponent<BoxCollider2D> ().enabled = true;

				mapNpcs.Add (mapNpc);

			}
		}

		private void SetUpMonsters(){

			int monsterCount = Random.Range (levelData.monsterCount.minimum, levelData.monsterCount.maximum + 1);

			for (int i = 0; i < monsterCount; i++) {
				
				int randomMonsterId = Random.Range (0, levelData.monsters.Count);

				Transform monsterModel = levelData.monsters [randomMonsterId];

				Transform monster = monsterPool.GetInstanceWithName<Transform> (monsterModel.gameObject.name, monsterModel.gameObject, monstersContainer);

				Vector3 pos = RandomPosition (totalValablePosGridList);

				monster.GetComponent<UnityArmatureComponent> ().sortingOrder = -(int)pos.y;

				mapWalkableInfoArray [(int)pos.x, (int)pos.y] = 0;

				monster.position = pos;

				monster.gameObject.SetActive (true);

				BattleMonsterController bmCtr = monster.GetComponent<BattleMonsterController> ();

				bmCtr.PlayRoleAnim ("wait",0,null);

				monsters.Add (monster.GetComponent<Monster>());


			}
		}


		private T RandomEvent<T>(List<T> eventsList){

			int index = Random.Range (0, eventsList.Count);

			return eventsList [index];

		}

		/// <summary>
		/// Sets up the outer walls and floor (background) of the game board.
		/// </summary>
		private void SetUpFloorAndBackground ()
		{

			// 获得地图图集
			string floorImageName = mapInfo.floorImageName;

//			Sprite floorImage = GameManager.Instance.gameDataCenter.allMapSprites.Find (delegate(Sprite obj) {
//				return obj.name == floorImageName;
//			});

			// 获得背景图片
			string backgroundImageName = mapInfo.backgroundImageName;

			Sprite backgroundSprite = GameManager.Instance.gameDataCenter.allMapSprites.Find (delegate(Sprite obj) {
				return obj.name == backgroundImageName;
			});

			Camera.main.transform.Find ("Background").GetComponent<SpriteRenderer> ().sprite = backgroundSprite;
//			background.sprite = backgroundSprite;

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

			// 创建地面和墙体
			// x代表列
			// y代表行
			// i代表地图层
//			for (int i = 0; i < mapInfo.layers.Length; i++) {
//
//				Layer layer = mapInfo.layers [i];
//
//				for(int y = 0; y < rows; y++)
//				{
//					for(int x = 0; x < columns; x++)
//					{
//
//						int tileIndexInMap = x + y * columns;
//
//						Vector3 tilePos = new Vector3 (x,rows - y - 1,0);
//
//						int tileIndex = layer.tileDatas [tileIndexInMap];
//
//						if (tileIndex == -1) {
//							continue;
//						}
//
//						int walkableInfo = mapInfo.tilesets[0].walkableInfoArray[tileIndex];
//
//						if (walkableInfo < mapWalkableInfoArray [x, rows - y - 1]) {
//							mapWalkableInfoArray [x, rows - y - 1] = walkableInfo;
//						}
//
//						string tileName = string.Format ("{0}_{1}", mapImageName,tileIndex);
//
//						Sprite tileSprite = GameManager.Instance.gameDataCenter.allMapSprites.Find (delegate(Sprite s) {
//							return s.name == tileName;
//						});
//
//						// 如果是遮罩层
//						if (layer.name == "Mask") {
//
//							Transform wall = outerWallPool.GetInstance<Transform> (wallModel.gameObject, outerWallsContainer);
//
//							wall.position = tilePos;
//
//							wall.name = tileName;
//
//							SpriteRenderer sr = wall.GetComponent<SpriteRenderer> ();
//
//							sr.sprite = tileSprite;
//
//							sr.material = spriteMaterial;
//
//							wall.SetParent (outerWallsContainer, true);
//
//							wall.GetComponent<SpriteRenderer> ().sortingOrder = -(int)(tilePos.y) + 1;
//
//							wall.GetComponent<BoxCollider2D> ().enabled = false;
//
//							continue;
//
//						}
//
//						// 非遮罩层的墙体
//						if (walkableInfo == -1) {
//							
//							Transform wall = outerWallPool.GetInstance<Transform> (wallModel.gameObject, outerWallsContainer);
//
//							wall.position = tilePos;
//
//							wall.name = tileName;
//
//							SpriteRenderer sr = wall.GetComponent<SpriteRenderer> ();
//
//							sr.sprite = tileSprite;
//
//							sr.material = spriteMaterial;
//
//							wall.SetParent (outerWallsContainer, true);
//
//							wall.GetComponent<SpriteRenderer> ().sortingOrder = -(int)(tilePos.y);
//
//							wall.GetComponent<BoxCollider2D> ().enabled = true;
//			
//						} 
//						// 其他
//						else {
//							
//							Transform floor = floorPool.GetInstance<Transform> (floorModel.gameObject, floorsContainer);
//
//							floor.position = tilePos;
//
//							floor.name = tileName;
//
//							SpriteRenderer sr = floor.GetComponent<SpriteRenderer> ();
//
//							sr.sprite = tileSprite;
//
//							sr.material = spriteMaterial;
//
//							floor.SetParent (floorsContainer, true);
//						}
//							
//					}
//
//				}
//
//			}

		}

		public void SetUpBuildings(){

			// 初始化地图上的工作台
			Vector3 workBenchPos = RandomPosition(treasureBox_npc_produceBuildingPosGridList);

			workBench.position = workBenchPos;

			workBench.GetComponent<SpriteRenderer> ().sortingOrder = -(int)workBenchPos.y;

			mapWalkableInfoArray[(int)workBenchPos.x,(int)workBenchPos.y] = 0;

			// 初始化单词水晶
			for (int i = 0; i < 4; i++) {
				
				Vector3 crystalPos = RandomPosition (learnCrystalPosGridList);

				Transform crystal = crystalPool.GetInstance<Transform> (crystalModel.gameObject, crystalsContainer);

				crystal.position = crystalPos;

				crystal.GetComponent<SpriteRenderer> ().sortingOrder = -(int)crystalPos.y;

				mapWalkableInfoArray[(int)crystalPos.x,(int)crystalPos.y] = 0;
			}
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

		//Clears our list gridPositions and prepares it to generate a new board.
		void ResetGridList ()
		{
			//Clear our list gridPositions.
			gridPositions.Clear ();

			//Loop through x axis (columns).
			for(int x = 1; x < columns-1; x++)
			{
				//Within each column, loop through y axis (rows).
				for(int y = 1; y < rows-1; y++)
				{
					//At each index add a new Vector3 to our list with the x and y coordinates of that position.
//					gridPositions.Add (new Vector3(x, y, y));
					gridPositions.Add (new Vector3(x, y, 0));
				}
			}
		}

		private void ResetMapWalkableInfoArray (){
			for (int i = 0; i < columns; i++) {
				for (int j = 0; j < rows; j++) {
					mapWalkableInfoArray [i, j] = -1;
				}
			}
		}
			

		//RandomPosition returns a random position from our list gridPositions.
		private Vector3 RandomPosition (List<Vector3> gridPositions)
		{

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

			mapItems = null;
			mapNpcs = null;
			monsters = null;

			Destroy (outerWallPool.gameObject);
			Destroy (floorPool.gameObject);
			Destroy (npcPool.gameObject);
			Destroy (itemPool.gameObject);
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
			itemPool.AddChildInstancesToPool (itemsContainer);
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
			itemPool.ClearInstancePool ();
			monsterPool.ClearInstancePool ();

		}


	}
		
}
