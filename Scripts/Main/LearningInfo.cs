using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;



namespace WordJourney
{
	
	[System.Serializable]
	public class LearningInfo {

		// 当前单词类型下所有单词的数量
		public int totalWordCount{
			get{
				return learnedWordCount + unlearnedWords.Count;
			}

		}

		// 当前单词类型下所有已学习过的单词数量
		public int learnedWordCount{
			get{
				return learnedWords.Count;
			}
		}

		// 总学习时长
		public int totalLearnDuration;

		// 总背诵次数
		public int totalLearnTimeCount;

		// 装载所有已学习单词的列表容器
		public List<LearnWord> learnedWords = new List<LearnWord> ();
		// 装载所有未学习单词的列表容器
		public List<LearnWord> unlearnedWords = new List<LearnWord>();

		// 当前设置状态下的单词类型
		public WordType wordType{

			get{
				return GameManager.Instance.gameDataCenter.gameSettings.wordType;
			}

		}

		/// <summary>
		/// 从数据库中读取对应类型的单词
		/// </summary>
		public void GetCurrentLearningTypeWords(){

			string tableName = string.Empty;

			switch (wordType) {
			case WordType.CET4:
				tableName = CommonData.CET4Table;
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

			// 检查存放指定单词类型的表是否存在（目前只做了测试用的CET4这一个表，添加表使用参考editor文件夹下的DataBaseManager）
			if (!sql.CheckTableExist (tableName)) {
				Debug.Log ("查询的表不存在");
				return;
			}

			// 检查表中字段名称（目前设定表中字段为：单词id，拼写，释义，例句，是否学习过）
			sql.CheckFiledNames (tableName, new string[]{ "wordId", "spell", "phoneticSymbol", "explaination", "example","learnedTimes", "ungraspTimes" });

			// 读取器
			IDataReader reader = sql.ReadFullTable (tableName);

			// 从表中读取数据
			while (reader.Read ()) {

				if (reader == null) {
					return;
				}

				int wordId = reader.GetInt32 (0);
				string spell = reader.GetString (1);
				string explaination = reader.GetString (2);
				string phoneticSymble = reader.GetString (3);
				string example = reader.GetString (4);
				int learnedTimes = reader.GetInt16 (5);
				int ungraspTimes = reader.GetInt16 (6);

				LearnWord w = new LearnWord (wordId, spell, explaination, phoneticSymble, example,learnedTimes,ungraspTimes);

				#warning 学习记录中应该展示哪些单词？
			}
		}

	}

}