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
		private MapInfo mapInfo;
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
		public Transform rewardsContainer;

		// 所有的缓存池
		private InstancePool outerWallPool;
		private InstancePool floorPool;
		private InstancePool npcPool;
		private InstancePool itemPool;
		private InstancePool monsterPool;
		private InstancePool skillEffectPool;
		private InstancePool rewardItemPool;

		public Animator destinationAnimator;

		// 关卡数据
		private GameLevelData levelData;

		public UnityEngine.Material spriteMaterial; 

		private List <Vector3> gridPositions = new List <Vector3> ();	//A list of possible locations to place tiles.

		public int[,] mapWalkableInfoArray;

		public float rewardFlyDuration;

		private BattlePlayerController bpCtr;


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
			}

			MapInstancesToPool ();

			mapItemGenerator = GetComponent<MapItemGenerator> ();

			this.levelData = levelData;

			mapInfo = DataHandler.LoadDataToSingleModelWithPath<MapInfo> (CommonData.mapDataFilePath);

			// 获取地图建模的行数和列数
			rows = mapInfo.height;
			columns = mapInfo.width;

			// 地图上的可行走信息数组
			mapWalkableInfoArray = new int[columns,rows];

			ResetMapWalkableInfoArray ();

			// 重建地图列表（可摆放位置列表）
			ResetGridList ();

			// 初始化地面和墙
			SetUpWallAndFloor();

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


		public Transform GetSkillEffect(Transform parentTrans){
			return skillEffectPool.GetInstance<Transform> (skillEffectModel.gameObject, parentTrans);
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

		private void SetUpPlayer(){

			Transform player = Player.mainPlayer.GetComponentInChildren<BattlePlayerController> ().transform;

			bpCtr = player.GetComponent<BattlePlayerController> ();

			// 随机玩家初始位置
			Vector3 playerOriginPos = RandomPosition ();

			player.position = playerOriginPos;

			bpCtr.singleMoveEndPos = playerOriginPos;

			player.rotation = Quaternion.identity;

			// 视角聚焦到玩家身上
			Camera.main.transform.SetParent (player, false);

			Camera.main.transform.rotation = Quaternion.identity;

			Camera.main.transform.localPosition = new Vector3 (0, 0, -90);

			Camera.main.transform.Find ("Cover").gameObject.SetActive (true);

			// 默认进入关卡后播放的角色动画
			bpCtr.PlayRoleAnim ("stand", 0, null);

		}

		private void SetUpItems(){

			int mapItemCount = Random.Range (levelData.itemCount.minimum, levelData.itemCount.maximum + 1);

//			int mapItemCount = 50;

			List<MapItem> randomMapItems = mapItemGenerator.InitMapItems (levelData.normalItems,levelData.lockedItems, itemPool, itemsContainer, mapItemCount);

			for (int i = 0; i < randomMapItems.Count; i++) {

				MapItem mapItem = randomMapItems [i];

				Vector3 pos = RandomPosition ();

				mapItem.transform.position = pos;

				if (mapItem.mapItemType == MapItemType.Trap) {
					#warning 这里为了测试，先把陷阱做成和普通地板一样的消耗值
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

				Vector3 pos = RandomPosition ();

				mapWalkableInfoArray [(int)pos.x, (int)pos.y] = 0;

				MapNPC mapNpc = npcPool.GetInstance<MapNPC> (mapNpcModel.gameObject, npcsContainer);

				mapNpc.transform.position = pos;

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

				Vector3 pos = RandomPosition ();

				mapWalkableInfoArray [(int)pos.x, (int)pos.y] = 0;

				monster.position = pos;

				monster.gameObject.SetActive (true);

				monster.GetComponent<BattleMonsterController> ().PlayRoleAnim ("wait",0,null);

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
		private void SetUpWallAndFloor ()
		{

			// 获得地图图集名称
			string mapImageName = mapInfo.tilesets[0].image;

			// 创建地面和墙体
			// x代表列
			// y代表行
			// i代表地图层
			for (int i = 0; i < mapInfo.layers.Length; i++) {

				Layer layer = mapInfo.layers [i];

				for(int y = 0; y < rows; y++)
				{
					for(int x = 0; x < columns; x++)
					{

						int tileIndexInMap = x + y * columns;

						Vector3 tilePos = new Vector3 (x,rows - y - 1,0);

						int tileIndex = layer.data [tileIndexInMap] - 1;

						if (tileIndex == -1) {
							continue;
						}

						int walkableInfo = mapInfo.tilesets[0].walkableInfoArray[tileIndex];

						if (walkableInfo < mapWalkableInfoArray [x, rows - y - 1]) {
							mapWalkableInfoArray [x, rows - y - 1] = walkableInfo;
						}

						string tileName = string.Format ("{0}_{1}", mapImageName,tileIndex);

						Sprite tileSprite = GameManager.Instance.gameDataCenter.allMapSprites.Find (delegate(Sprite s) {
							return s.name == tileName;
						});

						// 如果是遮罩层
						if (layer.name == "Mask") {

							Transform wall = outerWallPool.GetInstance<Transform> (wallModel.gameObject, outerWallsContainer);

							wall.position = tilePos;

							wall.name = tileName;

							SpriteRenderer sr = wall.GetComponent<SpriteRenderer> ();

							sr.sprite = tileSprite;

							sr.material = spriteMaterial;

							wall.SetParent (outerWallsContainer, true);

							wall.GetComponent<SpriteRenderer> ().sortingOrder = 2;

							wall.GetComponent<BoxCollider2D> ().enabled = false;

							continue;

						}

						// 非遮罩层的墙体
						if (walkableInfo == -1) {
							
							Transform wall = outerWallPool.GetInstance<Transform> (wallModel.gameObject, outerWallsContainer);

							wall.position = tilePos;

							wall.name = tileName;

							SpriteRenderer sr = wall.GetComponent<SpriteRenderer> ();

							sr.sprite = tileSprite;

							sr.material = spriteMaterial;

							wall.SetParent (outerWallsContainer, true);

							wall.GetComponent<SpriteRenderer> ().sortingOrder = 0;

							wall.GetComponent<BoxCollider2D> ().enabled = true;
			
						} 
						// 其他
						else {
							
							Transform floor = floorPool.GetInstance<Transform> (floorModel.gameObject, floorsContainer);

							floor.position = tilePos;

							floor.name = tileName;

							SpriteRenderer sr = floor.GetComponent<SpriteRenderer> ();

							sr.sprite = tileSprite;

							sr.material = spriteMaterial;

							floor.SetParent (floorsContainer, true);
						}
							
					}

				}

			}

		}

		public void SetUpBuildings(){

			// 初始化地图上的工作台
			Vector3 workBenchPos = RandomPosition();

			workBench.position = workBenchPos;

			mapWalkableInfoArray[(int)workBenchPos.x,(int)workBenchPos.y] = 0;


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
					gridPositions.Add (new Vector3(x, y, y));
				}
			}
		}

		private void ResetMapWalkableInfoArray (){
			for (int i = 0; i < columns; i++) {
				for (int j = 0; j < rows; j++) {
					mapWalkableInfoArray [i, j] = 1;
				}
			}
		}
			

		//RandomPosition returns a random position from our list gridPositions.
		private Vector3 RandomPosition ()
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
			

		//LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
		private void LayoutObjectAtRandom<T> (List<T> tileList, Count tileCount,Transform container)
			where T:Component
		{

			int minimum = tileCount.minimum;
			int maximum = tileCount.maximum;
			
			//Choose a random number of objects to instantiate within the minimum and maximum limits
			int objectCount = Random.Range (minimum, maximum+1);

			//Instantiate objects until the randomly chosen limit objectCount is reached
			for(int i = 0; i < objectCount; i++)
			{
				//Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
				Vector3 randomPosition = RandomPosition();

				//Choose a random tile from tileArray and assign it to tileChoice
				GameObject tileChoice = tileList[Random.Range(0,tileList.Count)].gameObject;

				//Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
				GameObject go =  Instantiate(tileChoice, randomPosition, Quaternion.identity);

				go.name = tileChoice.name;

				go.transform.SetParent (container, true);

				mapWalkableInfoArray [(int)randomPosition.x, (int)randomPosition.y] = 0;

			}
		}


		private struct RewardInMap
		{
			public Transform rewardTrans;
			public Item reward;

			public RewardInMap(Transform rewardTrans,Item reward){
				this.rewardTrans = rewardTrans;
				this.reward = reward;
			}


		}

		public void SetUpRewardInMap(Item reward, Vector3 rewardPosition){

			Transform rewardTrans = rewardItemPool.GetInstance<Transform> (rewardItemModel.gameObject, rewardsContainer);

			SpriteRenderer sr = rewardTrans.GetComponent<SpriteRenderer> ();

			Sprite s = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
				return obj.name == reward.spriteName;
			});

			sr.sprite = s;

			rewardTrans.position = rewardPosition;

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

			while (distance > 0.2f && leftTime > 0.01f) {

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

//				Debug.Log (string.Format ("距离{0}，剩余时间{1}", distance, leftTime));

			}

			Player.mainPlayer.AddItem (rewardInMap.reward);

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
