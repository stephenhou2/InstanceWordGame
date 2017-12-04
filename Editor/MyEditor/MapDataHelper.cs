using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	using UnityEditor;
	using System.IO;

	public class MapDataHelper {

		[System.Serializable]
		public struct OriginalMapData
		{
			public OriginalTileSet[] tilesets;
			public int width;
			public int height;
			public int tilewidth;
			public int tileheight;
			public OriginalLayer[] layers;

			public int floorTileFirstGid;
			public int attachedInfoFirstGid;

		}

		[System.Serializable]
		public struct OriginalLayer
		{
			public int[] data;
			public string name;
			public AttachedProperty properties;

		}

		[System.Serializable]
		public struct AttachedProperty{
//			public string image;
			public string backgroundImageName;
			public string floorImageName;
		}

		[System.Serializable]
		public struct OriginalTileSet{
			public int firstgid;
			public string source;
		}

		[System.Serializable]
		public class FloorTilesInfo
		{
			public string floorImageName;
			public int[] walkableInfoArray;
		}


		[MenuItem("EditHelper/MapDataHelper")]
		public static void SetUpMapData(){

			string originalMapDataDirectory = "/Users/houlianghong/Desktop/MyGameData/Map/MapDatas";

			DirectoryInfo directory = new DirectoryInfo (originalMapDataDirectory);

			FileInfo[] originalMapDatasFiles = directory.GetFiles ();

			string floorTilesInfoPath = "/Users/houlianghong/Desktop/MyGameData/Map/FloorTilesInfo.json";

			FloorTilesInfo[] floorTilesInfoArray = DataHandler.LoadDataToModelsWithPath<FloorTilesInfo> (floorTilesInfoPath);

			FloorTilesInfo tileInfo = null;

			for (int i = 0; i < originalMapDatasFiles.Length; i++) {

				FileInfo fi = originalMapDatasFiles [i];

				if (fi.Extension != ".json") {
					continue;
				}

				OriginalMapData oriMapData = DataHandler.LoadDataToSingleModelWithPath<OriginalMapData> (fi.FullName);

				for (int j = 0; j < oriMapData.tilesets.Length; j++) {

					OriginalTileSet ts = oriMapData.tilesets [j];

					if (ts.source.Contains ("Floor")) {
						oriMapData.floorTileFirstGid = ts.firstgid;
					} else if (ts.source.Contains ("AttachedInfo")) {
						oriMapData.attachedInfoFirstGid = ts.firstgid;
					} else {
						Debug.LogError(string.Format("未查询到地图／附加信息的原始贴图数据"));
					}


				}


				Layer[] newLayers = new Layer[oriMapData.layers.Length];
				string backgroundImageName = null;
				string floorImageName = null;


				for (int j = 0; j < oriMapData.layers.Length; j++) {
					
					OriginalLayer layer = oriMapData.layers [j];

					int firstGid = 0;


					switch (layer.name) {
					case "FloorLayer":
						firstGid = oriMapData.floorTileFirstGid;
						floorImageName = layer.properties.floorImageName;
						for (int m = 0; m < floorTilesInfoArray.Length; m++) {
							if (floorTilesInfoArray [m].floorImageName == floorImageName) {
								tileInfo = floorTilesInfoArray [m];
							}
						}
						if (tileInfo == null) {
							Debug.LogError (string.Format ("未查询到对应的地板图块信息--地图名称：{0}", fi.Name));
						}

						break;
					case "AttachedInfoLayer":
						firstGid = oriMapData.attachedInfoFirstGid;
						backgroundImageName = layer.properties.backgroundImageName;
						break;
					}

					int row = 0;
					int col = 0;

					List<Tile> tileDatas = new List<Tile> ();

					for (int k = 0; k < layer.data.Length; k++) {
						row = oriMapData.height - k / oriMapData.width - 1;
						col = k % oriMapData.width;
						int tileIndex = layer.data [k] - firstGid;
						if (tileIndex >= 0) {
							bool walkable = tileInfo.walkableInfoArray [tileIndex] == 1;
							Tile tile = new Tile (new Vector2 (col, row), tileIndex ,walkable);
							tileDatas.Add (tile);
							Debug.LogFormat ("{0}-{1}",k,tile);
						}

					}


					newLayers[j] = new Layer (layer.name, tileDatas.ToArray()); 

				}

				MapData newMapData = new MapData (oriMapData.height, oriMapData.width, oriMapData.tilewidth, oriMapData.tileheight, floorImageName, backgroundImageName, newLayers);

				string newMapDataDirectory = "/Users/houlianghong/Desktop/Unityfolder/TestOnSkills/Assets/StreamingAssets/Data/MapData";

				SaveNewMapData (newMapData, newMapDataDirectory, fi.Name);
			}

		}
			
		private static void SaveNewMapData(MapData mapData, string directory, string fileName){

			if (!Directory.Exists (directory)) {
				Directory.CreateDirectory (directory);
			}

			string path = string.Format ("{0}/{1}", directory, fileName);

			string mapDataJson = JsonUtility.ToJson (mapData);

			File.WriteAllText (path, mapDataJson);

		}
		
	}
}
