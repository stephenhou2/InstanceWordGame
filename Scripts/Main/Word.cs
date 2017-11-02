using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;


namespace WordJourney
{



	// 单词模型  
	[System.Serializable]
	public class Word{

		public int wordId;

		public string spell;

		public string explaination;

		public string example;

		public bool valid;


		public Word(int wordId,string spell,string explaination,string example,bool valid){
			this.wordId = wordId;
			this.spell = spell;
			this.explaination = explaination;
			this.example = example;
			this.valid = valid;
		}

//		public static Word FindWordInAllWords(string spell){
//
//			string tableName = "AllWordsData";
//
//			MySQLiteHelper sql = MySQLiteHelper.Instance;
//
//			// 连接数据库
//			sql.GetConnectionWith (CommonData.dataBaseName);
//
//			string[] conditions = new string[]{ string.Format ("Spell={0}",spell) };
//
//			IDataReader reader =  sql.ReadSpecificRowsAndColsOfTable (tableName, null, conditions, true);
//
//			reader.Read ();
//
//			if (reader == null) {
//				Debug.Log ("没有对应单词");
//				return null;
//			}
//
//			int wordId = reader.GetInt32 (0);
//			string explaination = reader.GetString (2);
//			int type = reader.GetInt16 (3);
//			bool valid = reader.GetBoolean (4);

//			sql.CloseConnection (CommonData.dataBaseName);
//
//			return new Word (wordId, spell, explaination,string.Empty,valid);

//		}


		public static Word RandomWord(){

			LearningInfo learnInfo = GameManager.Instance.gameDataCenter.learnInfo;

			int wordId = 0;

			if (learnInfo.learnedWords.Count != 0) {

				wordId = Random.Range (0, learnInfo.learnedWords.Count - 1);

				return learnInfo.learnedWords [wordId];

			}


			string tableName = string.Empty;

			WordType wt = GameManager.Instance.gameDataCenter.learnInfo.wordType;

			switch (wt) {
			case WordType.CET4:
				tableName = "CET4";
				break;
			case WordType.CET6:
				tableName = "CET6";
				break;
			case WordType.Daily:
				tableName = "Daily";
				break;
			case WordType.Bussiness:
				tableName = "Bussiness";
				break;
			}

			MySQLiteHelper sql = MySQLiteHelper.Instance;

			// 连接数据库
			sql.GetConnectionWith (CommonData.dataBaseName);

			int wordsCount = sql.GetItemCountOfTable (tableName);

			wordId = Random.Range (0, wordsCount - 1);

			string[] conditions = new string[]{string.Format ("wordId={0}", wordId)};

			IDataReader reader = sql.ReadSpecificRowsAndColsOfTable (tableName, null, conditions, true);

			reader.Read ();

			string spell = reader.GetString (1);

			string explaination = reader.GetString (2);

			string example = reader.GetString (3);

			return new Word (wordId, spell, explaination, example, true);

		}
			

	}

}
