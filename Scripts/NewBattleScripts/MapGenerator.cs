using UnityEngine;
using System;
using System.Collections.Generic; 		//Allows us to use Lists.
using Random = UnityEngine.Random; 		//Tells Random to use the Unity Engine random number generator.
using Transform = UnityEngine.Transform;
using DragonBones;

namespace WordJourney	
{
	public class MapGenerator : SingletonMono<MapGenerator>
	{
		[HideInInspector]public int columns; 										//Number of columns in our game board.
		[HideInInspector]public int rows;											//Number of rows in our game board.


		private MapInfo mapInfo;
		private TileInfo tileInfo;

		public Transform exit;	

		private Transform player;

//		public List<Transform> floorTiles;
//		public List<Transform> outerWallTiles;

		public GameObject wallModel;
		public GameObject floorModel;
		public MapItem mapItemModel;
		public MapNPC mapNpcModel;

		private List<MapItem> mapItems = new List<MapItem> ();
		private List<MapNPC>mapNpcs = new List<MapNPC>();
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


		public Animator destinationAnimator;

		private List <Vector3> gridPositions = new List <Vector3> ();	//A list of possible locations to place tiles.

		public int[,] mapWalkableInfoArray;


		void Awake(){

			outerWallPool = InstancePool.GetOrCreateInstancePool ("OuterWallPool");
			floorPool = InstancePool.GetOrCreateInstancePool ("FloorPool");
			npcPool = InstancePool.GetOrCreateInstancePool ("NPCPool");
			itemPool = InstancePool.GetOrCreateInstancePool ("ItemPool");
			monsterPool = InstancePool.GetOrCreateInstancePool ("MonsterPool");

		}



		//SetupScene initializes our level and calls the previous functions to lay out the game board
		public void SetUpMap (ChapterDetailInfo chapterDetail)
		{

			mapInfo = DataInitializer.LoadDataToSingleModelWithPath<MapInfo> (CommonData.jsonFileDirectoryPath, "MapJson.json");

			rows = mapInfo.height;
			columns = mapInfo.width;

			tileInfo = DataInitializer.LoadDataToSingleModelWithPath<TileInfo> (CommonData.jsonFileDirectoryPath, "TileJson.json");

			mapWalkableInfoArray = new int[rows, columns];

			ResetGridList ();

			SetUpWallAndFloor ();

			SetUpPlayer ();

			List<Item> currentChapterItems = chapterDetail.GetCurrentChapterItems ();

			List<NPC> currentChapterNpcs = chapterDetail.GetCurrentChapterNpcs ();

			int mapItemcount = Random.Range (chapterDetail.itemCount.minimum, chapterDetail.itemCount.maximum + 1);

			for (int i = 0; i < mapItemcount; i++) {

				int rewardItemCount = Random.Range (2,4);

				Vector3 pos = RandomPosition ();

				mapWalkableInfoArray [(int)pos.x, (int)pos.y] = 0;

				MapItem mapItem = Instantiate (mapItemModel, pos, Quaternion.identity);

				mapItem.transform.SetParent(itemsContainer,true);

				mapItem.rewardItems = new Item[rewardItemCount];

				for (int j = 0; j < rewardItemCount; j++) {
					
					Item item = RandomEvent<Item> (currentChapterItems);

					mapItem.rewardItems [j] = item;
				}

				if (mapItem != null) {
					mapItems.Add (mapItem);
				}


			}

			for (int i = 0; i < currentChapterNpcs.Count; i++) {

				NPC npc = currentChapterNpcs [i];

				Vector3 pos = RandomPosition ();

				mapWalkableInfoArray [(int)pos.x, (int)pos.y] = 0;

				MapNPC mapNpc = Instantiate (mapNpcModel, pos, Quaternion.identity);

				mapNpc.transform.SetParent (npcsContainer, true);

				mapNpc.npc = npc;

				mapNpc.name = npc.npcName;

				mapNpcs.Add (mapNpc);

			}
				

			monsters = chapterDetail.GetCurrentChapterMonsters ();

			foreach (Transform monster in monsters) {
				monster.SetParent (monsterModelsContainer, false);
				monster.GetComponent<BattleMonsterController> ().PlayRoleAnim ("stand", 0, null);
			}

			LayoutObjectAtRandom (monsters, chapterDetail.monsterCount,monstersContainer);

		}
			
		private void SetUpPlayer(){

			player = Player.mainPlayer.GetComponentInChildren<BattlePlayerController> ().transform;

			BattlePlayerController bpCtr = player.GetComponent<BattlePlayerController> ();
			
			Vector3 playerOriginPos = RandomPosition ();

			player.position = playerOriginPos;

			bpCtr.singleMoveEndPos = playerOriginPos;

			player.rotation = Quaternion.identity;

			Camera.main.transform.position = new Vector3 (0, 0, -10);

			Camera.main.transform.rotation = Quaternion.identity;

			Camera.main.transform.SetParent (player,false);

			bpCtr.PlayRoleAnim ("stand", 0, null);

		}

		private T RandomEvent<T>(List<T> eventsList){

			int index = Random.Range (0, eventsList.Count);

			return eventsList [index];

		}

		//Sets up the outer walls and floor (background) of the game board.
		void SetUpWallAndFloor ()
		{

			for(int y = 0; y < columns; y++)
			{
				for(int x = 0; x < rows; x++)
				{

					int tileIndexInMap = columns * y + x;

					Vector3 tilePos = new Vector3 (x,columns - y - 1,0);

					for (int i = 0; i < mapInfo.layers.Length; i++) {

						Layer layer = mapInfo.layers [i];

						int tileIndex = layer.data [tileIndexInMap] - 1;

						if (tileIndex == -1) {
							mapWalkableInfoArray [x,columns - y - 1] = 1;
							continue;
						}

						string tileName = string.Format ("map1_{0}", tileIndex);

						Sprite tileSprite = GameManager.Instance.allMapSprites.Find (delegate(Sprite s) {
							return s.name == tileName;
						});

						int walkableInfo = tileInfo.walkableInfoArray[tileIndex];

						mapWalkableInfoArray [x,columns - y - 1] = walkableInfo;

						if (walkableInfo == -1) {
							GameObject wall = Instantiate (wallModel, tilePos, Quaternion.identity);
						
							wall.name = tileName;

							wall.GetComponent<SpriteRenderer> ().sprite = tileSprite;

							wall.transform.SetParent (outerWallsContainer, true);
						} else {
							GameObject floor = Instantiate (floorModel, tilePos, Quaternion.identity);

							floor.name = tileName;

							floor.GetComponent<SpriteRenderer> ().sprite = tileSprite;

							floor.transform.SetParent (floorsContainer, true);
						}
					}

				}

			}

			Debug.Log (mapWalkableInfoArray);

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


		//RandomPosition returns a random position from our list gridPositions.
		private Vector3 RandomPosition ()
		{

			//Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
			int randomIndex = Random.Range (0, gridPositions.Count);

			//Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
			Vector3 randomPosition = gridPositions[randomIndex];

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

		public void QuitCurrentMap(){
			outerWallPool.AddChildInstancesToPool (outerWallsContainer);
			floorPool.AddChildInstancesToPool (floorsContainer);
			npcPool.AddChildInstancesToPool (npcsContainer);
			itemPool.AddChildInstancesToPool (itemsContainer);
			monsterPool.AddChildInstancesToPool (monstersContainer);
		}

	}
		
}
