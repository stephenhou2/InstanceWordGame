using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney{

	[System.Serializable]
	public class MapData
	{
		public int rowCount;
		public int columnCount;
		public int tileWidth;
		public int tileHeight;
		public string floorImageName;
		public string backgroundImageName;
		public Layer[] layers;

//		public Layer floorLayer;
//		public Layer attachedInfoLayer;

		public MapData(int rowCount,int columnCount,int tileWidth,int tileHeight,string floorImageName,string backgroundImageName,Layer[] layers){
			this.rowCount = rowCount;
			this.columnCount = columnCount;
			this.tileWidth = tileWidth;
			this.tileHeight = tileHeight;
			this.floorImageName = floorImageName;
			this.backgroundImageName = backgroundImageName;
			this.layers = layers;
//			this.floorLayer = floorLayer;
//			this.attachedInfoLayer = attachedInfoLayer;
		}

		public static MapData GetMapDataOfLevel(int level){

			string mapDataFilePath = string.Format ("{0}/MapData/Level_{1}.json", CommonData.persistDataPath, level);
//			string mapDataFilePath = string.Format ("{0}/Level_{1}.json", CommonData.persistDataPath, level);

			Debug.Log (mapDataFilePath);

			return DataHandler.LoadDataToSingleModelWithPath<MapData> (mapDataFilePath);

		}


	}



	[System.Serializable]
	public class Layer
	{
		public string name;
		public Tile[] tileDatas;

		public Layer(string name,Tile[] tileDatas){
			this.name = name;
			this.tileDatas = tileDatas;
		}

	}

	[System.Serializable]
	public class Tile{
		
		public Vector2 position;
		public int tileIndex;
		public bool walkable;

		public Tile(Vector2 pos,int index,bool walkable){
			this.position = pos;
			this.tileIndex = index;
			this.walkable = walkable;
		}


		public override string ToString ()
		{
			return string.Format ("[Tile]\npositon:{0},tileIndex:{1},walkable:{2}",position,tileIndex,walkable);
		}

	}


//	[System.Serializable]
//	public struct TileSet{
//		public string image;
//		public int[] walkableInfoArray;
//	}
}
