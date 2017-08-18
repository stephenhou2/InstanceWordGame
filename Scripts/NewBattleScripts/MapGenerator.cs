using UnityEngine;
using System;
using System.Collections.Generic; 		//Allows us to use Lists.
using Random = UnityEngine.Random; 		//Tells Random to use the Unity Engine random number generator.


namespace WordJourney	
{
	
	public class MapGenerator : SingletonMono<MapGenerator>
	{
		
		public int columns = 8; 										//Number of columns in our game board.
		public int rows = 8;											//Number of rows in our game board.


		public Transform exit;	

		public Transform player;

		public List<Transform> floorTiles;
		public List<Transform> outerWallTiles;

		public MapItem mapItemModel;
		public MapNPC mapNpcModel;

		private List<MapItem> mapItems = new List<MapItem> ();
		private List<MapNPC>mapNpcs = new List<MapNPC>();
		private List<Monster>monsters;

		public Transform monsterModelsContainer;

		public Transform outerWallsContainer;
		public Transform floorsContainer;
		public Transform itemsContainer;
		public Transform npcsContainer;
		public Transform monstersContainer;


		public Animator destinationAnimator;

		private List <Vector3> gridPositions = new List <Vector3> ();	//A list of possible locations to place tiles.

		public int[,] mapWalkableInfoArray;


		//SetupScene initializes our level and calls the previous functions to lay out the game board
		public void SetUpMap (ChapterDetailInfo chapterDetail)
		{

			mapWalkableInfoArray = new int[rows, columns];

			for (int i = 0; i < rows; i++) {
				for (int j = 0; j < columns; j++) {
					mapWalkableInfoArray [i, j] = 1;
				}
			}

			ResetGridList ();

			SetUpPlayer ();

			SetUpOuterWallAndFloor ();

			List<Item> currentChapterItems = chapterDetail.GetCurrentChapterItems ();

			List<NPC> currentChapterNpcs = chapterDetail.GetCurrentChapterNpcs ();

			int count = Random.Range (chapterDetail.itemCount.minimum, chapterDetail.itemCount.maximum + 1);

			for (int i = 0; i < count; i++) {

				Item item = RandomEvent<Item> (currentChapterItems);

				Vector3 pos = RandomPosition ();

				mapWalkableInfoArray [(int)pos.x, (int)pos.y] = 0;

				MapItem mapItem = Instantiate (mapItemModel, pos, Quaternion.identity);

				mapItem.transform.SetParent(itemsContainer,true);

				mapItem.rewardItem = item;

				mapItem.name = item.itemName;

				mapItems.Add (mapItem);

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

			foreach (Monster m in monsters) {
				m.transform.SetParent (monsterModelsContainer, false);
			}

			LayoutObjectAtRandom (monsters, chapterDetail.monsterCount,monstersContainer);

		}
			
		private void SetUpPlayer(){
			
			Vector3 playerOriginPos = RandomPosition ();

			player.position = playerOriginPos;

			player.GetComponent<BattlePlayerController> ().predicatePos = playerOriginPos;

			player.rotation = Quaternion.identity;

			player.SetParent (null,true);

			Camera.main.transform.position = new Vector3 (0, 0, -10);

			Camera.main.transform.rotation = Quaternion.identity;

			Camera.main.transform.SetParent (player,false);
		}

		private T RandomEvent<T>(List<T> eventsList){

			int index = Random.Range (0, eventsList.Count);

			return eventsList [index];

		}

		//Sets up the outer walls and floor (background) of the game board.
		void SetUpOuterWallAndFloor ()
		{

			//Loop along x axis, starting from -1 (to fill corner) with floor or outerwall edge tiles.
			for(int x = 0; x < columns; x++)
			{
				//Loop along y axis, starting from -1 to place floor or outerwall tiles.
				for(int y = 0; y < rows; y++)
				{

					GameObject toInstantiate = null;
					GameObject instance = null;

					//Check if we current position is at board edge, if so choose a random outer wall prefab from our array of outer wall tiles.
					if (x == 0 || x == columns - 1 || y == 0 || y == rows - 1) {
						toInstantiate = outerWallTiles [Random.Range (0, outerWallTiles.Count)].gameObject;
						instance = Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
						instance.transform.SetParent (outerWallsContainer, true);


						mapWalkableInfoArray [x, y] = -1;

					} else {
						
						toInstantiate = floorTiles[Random.Range (0,floorTiles.Count)].gameObject;
						instance = Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
						instance.transform.SetParent (floorsContainer, true);

						mapWalkableInfoArray [x, y] = 1;

					}

					instance.name = toInstantiate.name;

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






		//RandomPosition returns a random position from our list gridPositions.
		Vector3 RandomPosition ()
		{
			//Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
			int randomIndex = Random.Range (0, gridPositions.Count);

			//Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
			Vector3 randomPosition = gridPositions[randomIndex];

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
	}



}
