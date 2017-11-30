using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace WordJourney
{

	using System.Data;

	// 普通单词模型  
	[System.Serializable]
	public struct GeneralWord{

		// 单词id
		public int wordId;
		// 单词拼写
		public string spell;
		// 单词释义
		public string explaination;
		// 单词是否可用于制造融合石
		public bool valid;


		public GeneralWord(int wordId, string spell, string explaination,bool valid){
			this.wordId = wordId;
			this.spell = spell;
			this.explaination = explaination;
			this.valid = valid;
		}
			


		public static GeneralWord RandomGeneralWord(){

			string tableName = "AllWordsData";

			MySQLiteHelper sql = MySQLiteHelper.Instance;

			// 连接数据库
			sql.GetConnectionWith (CommonData.dataBaseName);

			int wordsCount = sql.GetItemCountOfTable (tableName);

			int wordId = Random.Range (0, wordsCount);

			string[] conditions = new string[]{string.Format ("wordId={0}", wordId)};

			IDataReader reader = sql.ReadSpecificRowsAndColsOfTable (tableName, null, conditions, true);

			reader.Read ();

			string spell = reader.GetString (1);

			string explaination = reader.GetString (2);

			string example = reader.GetString (3);

			bool valid = reader.GetBoolean (4);

			return new GeneralWord (wordId, spell, explaination, valid);

		}
			

	}

	// 普通单词模型  
	[System.Serializable]
	public class LearnWord{

		// 单词id
		public int wordId;
		// 单词拼写
		public string spell;
		// 单词音标
		public string phoneticSymbol;
		// 单词释义
		public string explaination;
		// 例句
		public string example;
		// 单词已学次数
		public int learnedTimes;


		public LearnWord(int wordId, string spell, string phoneticSymbol, string explaination,string example,int learnedTimes){
			this.wordId = wordId;
			this.spell = spell;
			this.phoneticSymbol = phoneticSymbol;
			this.explaination = explaination;
			this.example = example;
			this.learnedTimes = learnedTimes;
		}


		public static LearnWord RandomWord(){

			LearningInfo learnInfo = GameManager.Instance.gameDataCenter.learnInfo;

			int wordId = 0;

			if (learnInfo.learnedWords.Count != 0) {

				wordId = Random.Range (0, learnInfo.learnedWords.Count);

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

			wordId = Random.Range (0, wordsCount);

			string[] conditions = new string[]{string.Format ("wordId={0}", wordId)};

			IDataReader reader = sql.ReadSpecificRowsAndColsOfTable (tableName, null, conditions, true);

			reader.Read ();

			string spell = reader.GetString (1);

			string explaination = reader.GetString (2);

			string phoneticSymbol = reader.GetString (3);

			string example = reader.GetString (4);

			int learnedTimes = reader.GetInt16 (5);

			return new LearnWord (wordId, spell, explaination, phoneticSymbol, example, learnedTimes);

		}


	}


}
