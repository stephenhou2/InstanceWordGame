using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{

	using System;
	using System.IO;

	public class GameLevelDataHandler{

		public List<MyGameLevelData> gameLevelDatas = new List<MyGameLevelData>();

		public void LoadGameDatas(){
			
			string gameDataSource = DataHandler.LoadDataString ("/Users/houlianghong/Desktop/MyGameData/GameLevelData.csv");

			string[] gameLevelDataStrings = gameDataSource.Split (new string[]{ "\n" },System.StringSplitOptions.RemoveEmptyEntries);

			for(int i = 1;i<gameLevelDataStrings.Length;i++){
				MyGameLevelData gameLevelData = new MyGameLevelData (gameLevelDataStrings [i]);
				gameLevelDatas.Add (gameLevelData);
			}

		}

		public void SaveGameDatas(){

			string gameLevelDatasJson = JsonHelper.ToJson<MyGameLevelData> (gameLevelDatas.ToArray ());

			Debug.Log (gameLevelDatasJson);

			File.WriteAllText (CommonData.originDataPath + "/GameLevelDatas.json",gameLevelDatasJson);

		}

	}

	[System.Serializable]
	public class MyGameLevelDatas{
		public List<MyGameLevelData> Items = new List<MyGameLevelData> ();
	}

	[System.Serializable]
	public class MyGameLevelData{

		// 关卡序号
		public int gameLevelIndex;

		// 所在章节名称(5关一个章节)
		public string chapterName;

		// 关卡中的所有怪物id
		public int[] monsterIds;

		// 关卡中出现的所有可以开出的物品id
		public int[] itemIds;

		// 关卡中所有可能出现的装备配方id
		public int[] formulaIds;

		// 每个itemId对应的物品是否只能由宝箱开出来
		public bool[] itemLockInfoArray;

		// 关卡中所有npc的id
		public int[] npcIds;

		// 关卡中怪物相对与prefab的提升比例
		public float monsterScaler;

		// 关卡中一共出现的地图物品（瓦罐，箱子，宝箱）数量范围
		public Count itemCount;

		// 关卡中一共出现的怪物数量范围
		public Count monsterCount;

		public MyGameLevelData(string dataString){

			string[] dataStrings = dataString.Split (new char[]{ ','},System.StringSplitOptions.RemoveEmptyEntries);

			gameLevelIndex = Convert.ToInt16 (dataStrings [0]);
			chapterName = dataStrings [1];
			monsterIds = InitIntArrayWithString (dataStrings [2]);
			itemIds = InitIntArrayWithString (dataStrings [3]);
			formulaIds = InitIntArrayWithString (dataStrings [4]);
			itemLockInfoArray = InitBoolArrayWithString (dataStrings [5]);
			npcIds = InitIntArrayWithString (dataStrings [6]);
			monsterScaler = Convert.ToSingle (dataStrings [7]);
			itemCount = InitCountWithString (dataStrings [8]);
			monsterCount = InitCountWithString (dataStrings [9]);


		}

		private bool[] InitBoolArrayWithString(string dataString){
			string[] boolStrings = dataString.Split (new char[]{ '_' }, System.StringSplitOptions.RemoveEmptyEntries);
			bool[] boolArray = new bool[boolStrings.Length];
			for (int i = 0; i < boolStrings.Length; i++) {
				boolArray [i] = Convert.ToInt16(boolStrings[i]) == 0 ? false : true;
			}
			return boolArray;
		}

		private int[] InitIntArrayWithString(string dataString){
			
			string[] idStrings = dataString.Split (new char[]{ '_' }, System.StringSplitOptions.RemoveEmptyEntries);
			int[] idArray = new int[idStrings.Length];
			for (int i = 0; i < idStrings.Length; i++) {
				idArray [i] = Convert.ToInt16(idStrings[i]);
			}
			return idArray;
		}

		private Count InitCountWithString(string dataString){
			string[] countStrings = dataString.Split (new char[]{ '_' }, System.StringSplitOptions.RemoveEmptyEntries);
			return new Count (Convert.ToInt16(countStrings [0]), Convert.ToInt16(countStrings [1]));
		}

	}

}
