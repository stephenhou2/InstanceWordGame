using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using UnityEditor;

 
public class DataBaseManager {

	private static string[] fieldNames;

	private static List<string[]> itemsProperties = new List<string[]> ();

	[MenuItem("Assets/BuildItemsDataBase")]
	public static void BuildItemsDataBase(){


		MySQLiteHelper sql = MySQLiteHelper.Instance;

		sql.CreateDatabase (CommonData.dataBaseName);

		sql.GetConnectionWith (CommonData.dataBaseName);

//		sql.CreatTable (CommonData.itemsTable,
//			new string[] {"itemId","itemName","itemDescription","spriteName","itemType","itemNameInEnglish",
//				"attackGain","powerGain","magicGain","critGain","amourGain","magicResistGain",
//				"agilityGain","healthGain","strengthGain"},
//			new string[] {"PRIMARY Key","UNIQUE NOT NULL","NOT NULL","NOT NULL","","UNIQUE","","","","","","","","",""},
//			new string[] {"INTEGER","TEXT","TEXT","TEXT","INTEGER","TEXT","INTEGER","INTEGER","INTEGER",
//				"INTEGER","INTEGER","INTEGER","INTEGER","INTEGER","INTEGER" });
//
//		int[] stringTypeCols = new int[]{ 1, 2, 3, 5 };
//
//		itemsProperties.Clear ();
//
//		LoadItemsData ("itemsData.csv");
//
//		sql.CheckFiledNames (CommonData.itemsTable,fieldNames);
//
//		sql.DeleteAllDataFromTable (CommonData.itemsTable);
//
//		for(int i = 0;i<itemsProperties.Count;i++){
//			string[] values = itemsProperties [i];
//
//			foreach (int j in stringTypeCols) {
//				values [j] = "'" + values[j] + "'";
//
//			}
//
//			sql.InsertValues (CommonData.itemsTable, values);
//		}
//

		sql.DeleteTable ("CET4");

		sql.CreateTable ("CET4",
			new string[]{ "wordId", "spell", "explaination", "example","learned" },
			new string[]{ "PRIMARY KEY", "UNIQUE NOT NULL", "NOT NULL", "","" },
			new string[]{ "INTEGER", "TEXT", "TEXT", "TEXT","INTEGER" });

		int[] stringTypeCols = new int[]{ 1, 2, 3 };

		itemsProperties.Clear ();

		LoadItemsData ("wordTest.csv");

		sql.CheckFiledNames ("CET4", fieldNames);

		for(int i = 0;i<itemsProperties.Count;i++){

			string[] values = itemsProperties [i];

			foreach (int j in stringTypeCols) {
				values [j] = "'" + values[j] + "'";

			}
				 
			sql.InsertValues ("CET4", values);
		}

		sql.CloseConnection (CommonData.dataBaseName);


	}

	// 从指定文件（txt／csv等文本文件）中读取数据 csv为从excel中导出的文本文件，导入unity之后需要选择结尾格式（mono里是这样的，在mono中打开csv文件后会有提示），否则在读取数据库时会报字段名不同的错误
	private static void LoadItemsData(string dataFileName){

		string itemsString = DataInitializer.LoadDataString (CommonData.jsonFileDirectoryPath, dataFileName);

		string[] stringsByLine = itemsString.Split (new string[]{ "\n" }, System.StringSplitOptions.RemoveEmptyEntries);

		fieldNames = stringsByLine [0].Split (new char[]{ ',' });

		for (int i = 1; i < stringsByLine.Length; i++) {
			itemsProperties.Add(stringsByLine [i].Split (new char[]{ ',' }));
		}

	}
}
