using UnityEngine;
using System.Collections;
using System.IO;
using Mono.Data.Sqlite;
using System.Data;

public class MyTest : MonoBehaviour 
{

	void Start ()
	{
//		SQLiteHelper sql = SQLiteHelper.Instance;
//
//		sql.SetCurrentDatabaseConnection ("test.db");

//		sql.CreateTable ("table1", new string[]{ "age", "name" }, new string[] {
//			"INTEGER",
//			"TEXT"
//		});
//
//		sql.InsertValues ("table1", new string[]{ "10", "'xiaoxiao'" });
//
//		IDataReader reader = sql.ReadFullTable ("table1");
//
//		while(reader.Read()){
//			Debug.Log(reader.GetString(reader.GetOrdinal("name")));
//		}

		MySQLiteHelper sql = MySQLiteHelper.Instance;

		sql.CreateDatabase ("test2.db");

		sql.GetConnectionWith ("test2.db");


//		sql.CreatTable ("test", new string[]{ "age","name" }, new string[]{ "INTEGER","TEXT" });

		sql.InsertValues("test",new string[] {"18","'haha'"});

		sql.InsertValues("test",new string[]{"30","'laowang'"});

		IDataReader reader = sql.ReadFullTable ("test");

		while (reader.Read ()) {
			Debug.Log (reader.GetString (1));
		}

		sql.UpdateSpecificColsWithValues ("test",
			new string[]{"age","name" },
			new string[]{ "20","'a'"},
			new string[]{ "age=18"},
			true);

		reader = sql.ReadSpecificRowsAndColsOfTable ("test", "name", null, true);

		while (reader.Read ()) {
			Debug.Log (reader.GetString (0));
		}

		sql.DeleteSpecificRows ("test", new string[]{ "age =18" }, true);

		reader = sql.ReadFullTable ("test");

		while (reader.Read ()) {
			Debug.Log (reader.GetString (1));
		}

		sql.CloseAllConnections ();
	}
}
