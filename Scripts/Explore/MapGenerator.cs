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
		[HideInInspector]public int columns; 										//Number of columns in our game board.
		[HideInInspector]public int rows;											//Number of rows in our game board.

		private MapItemGenerator mapItemGenerator;

		private MapInfo mapInfo;
//		private TileInfo tileInfo;

		public Transform exit;	

		public Transform wallModel;
		public Transform floorModel;
		public Transform mapNpcModel;
		public Transform skillEffectModel;

		private List<MapItem> mapItems = new List<MapItem> ();
		private List<MapNPC> mapNpcs = new List<MapNPC>();
		private List<Transform>monsters;

		public Transform monsterModelsContainer;

		public Transform outerWallsContainer;
		public Transform floorsContainer;
		public Transform itemsContainer;
		public Transform npcsContainer;
		public Transform monstersContainer;

		private InstancePool outerWallPool;
		private InstancePool floorPool;
		private InstancePool npcPool;
		private InstancePool itemPool;
		private InstancePool monsterPool;
		private InstancePool skillEffectPool;

		public Animator destinationAnimator;

		private GameLevelData levelData;

		public UnityEngine.Material spriteMaterial; 

		private List <Vector3> gridPositions = new List <Vector3> ();	//A list of possible locations to place tiles.

		public int[,] mapWalkableInfoArray;


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

//				outerWallPool.transform.SetParent (poolContainerOfExploreScene);
//				floorPool.transform.SetParent (poolContainerOfExploreScene);
//				npcPool.transform.SetParent (poolContainerOfExploreScene);
//				itemPool.transform.SetParent (poolContainerOfExploreScene);
//				monsterPool.transform.SetParent (poolContainerOfExploreScene);
//				skillEffectPool.transform.SetParent (poolContainerOfExploreScene);

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

			ClearPools ();

		}
			
		public void CreateSkillEffect(Transform parentTrans){

			skillEffectPool.GetInstance<Transform> (skillEffectModel.gameObject, parentTrans);

		}

		public void AddSkillEffectToPool(Transform skillEffect){
			skillEffectPool.AddInstanceToPool (skillEffect.gameObject);
		}

		private void SetUpPlayer(){

			Transform player = Player.mainPlayer.GetComponentInChildren<BattlePlayerController> ().transform;

			BattlePlayerController bpCtr = player.GetComponent<BattlePlayerController> ();

//			CreateSkillEffect (bpCtr.transform);

			Vector3 playerOriginPos = RandomPosition ();

			player.position = playerOriginPos;

			bpCtr.singleMoveEndPos = playerOriginPos;

			player.rotation = Quaternion.identity;

			Camera.main.transform.SetParent (player, false);

			Camera.main.transform.rotation = Quaternion.identity;

			Camera.main.transform.localPosition = new Vector3 (0, 0, -10);

			bpCtr.PlayRoleAnim ("stand", 0, null);

		}

		private void SetUpItems(){
			
			List<Item> currentChapterItems = levelData.GetCurrentChapterItems ();

//			int mapItemCount = Random.Range (chapterDetail.itemCount.minimum, chapterDetail.itemCount.maximum + 1);

			int mapItemCount = 50;

			List<MapItem> randomMapItems = mapItemGenerator.RandomMapItems (currentChapterItems, itemPool, itemsContainer, mapItemCount);

			for (int i = 0; i < randomMapItems.Count; i++) {

				MapItem mapItem = randomMapItems [i];

				Vector3 pos = RandomPosition ();

				mapItem.transform.position = pos;

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
			
			List<NPC> currentChapterNpcs = levelData.GetCurrentChapterNpcs ();

			for (int i = 0; i < currentChapterNpcs.Count; i++) {

				NPC npc = currentChapterNpcs [i];

				Vector3 pos = RandomPosition ();

				mapWalkableInfoArray [(int)pos.x, (int)pos.y] = 0;

				MapNPC mapNpc = npcPool.GetInstance<MapNPC> (mapNpcModel.gameObject, npcsContainer);

				mapNpc.transform.position = pos;

				mapNpc.npc = npc;

				mapNpc.name = npc.npcName;

//				SpriteRenderer sr = mapNpc.GetComponent<SpriteRenderer> ();
//
//				sr.sprite = GameManager.Instance.gameDataCenter.allMapSprites.Find (delegate(Sprite obj) {
//					return obj.name == mapNpc.npc.
//				});

				mapNpc.GetComponent<BoxCollider2D> ().enabled = true;

				mapNpcs.Add (mapNpc);

			}
		}

		private void SetUpMonsters(){
			
			monsters = levelData.GetCurrentChapterMonsters ();

			for(int i = 0;i<monsters.Count;i++){
				Transform monster = monsters [i].transform;
				monster.SetParent (monsterModelsContainer, false);
			}

			#warning for test
			LayoutObjectAtRandom (monsters, new Count(30,40),monstersContainer);

			for (int i = 0; i < monstersContainer.transform.childCount; i++) {
				Transform monsterTrans = monstersContainer.transform.GetChild (i);
				monsterTrans.GetComponent<BattleMonsterController> ().PlayRoleAnim ("stand",0);
			}
//			LayoutObjectAtRandom (monsters, chapterDetail.monsterCount,monstersContainer);
		}


		private T RandomEvent<T>(List<T> eventsList){

			int index = Random.Range (0, eventsList.Count);

			return eventsList [index];

		}

		//Sets up the outer walls and floor (background) of the game board.
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
					gridPositions.Add (new Vector3(x, y, 0f));
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
