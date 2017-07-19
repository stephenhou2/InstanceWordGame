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

		sql.CreatDatabase (CommonData.dataBaseName);

		sql.GetConnectionWith (CommonData.dataBaseName);

		sql.CreatTable (CommonData.itemsTable,
			new string[] {"itemId","itemName","itemDescription","spriteName","itemType","itemNameInEnglish",
				"attackGain","powerGain","magicGain","critGain","amourGain","magicResistGain",
				"agilityGain","healthGain","strengthGain"},
			new string[] {"PRIMARY Key","UNIQUE","NOT NULL","NOT NULL","","UNIQUE","","","","","","","","",""},
			new string[] {"INTEGER","TEXT","TEXT","TEXT","INTEGER","TEXT","INTEGER","INTEGER","INTEGER",
				"INTEGER","INTEGER","INTEGER","INTEGER","INTEGER","INTEGER" });

		int[] stringTypeCols = new int[]{ 1, 2, 3, 5 };

		LoadItemsData ();

		sql.CheckFiledNames (CommonData.itemsTable,fieldNames);

		sql.DeleteAllDataFromTable (CommonData.itemsTable);

		for(int i = 0;i<itemsProperties.Count;i++){
			string[] values = itemsProperties [i];

			foreach (int j in stringTypeCols) {
				values [j] = "'" + values[j] + "'";

			}

			sql.InsertValues (CommonData.itemsTable, values);
		}

		sql.CloseConnection (CommonData.dataBaseName);


	}

	private static void LoadItemsData(){

		string itemsString = DataInitializer.LoadDataString (CommonData.jsonFileDirectoryPath, "itemsData.csv");

		string[] stringsByLine = itemsString.Split (new string[]{ "\n" }, System.StringSplitOptions.RemoveEmptyEntries);

		fieldNames = stringsByLine [0].Split (new char[]{ ',' });

		for (int i = 1; i < stringsByLine.Length; i++) {
			itemsProperties.Add(stringsByLine [i].Split (new char[]{ ',' }));
		}

	}
}
