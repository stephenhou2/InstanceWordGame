using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;


[System.Serializable]
public class LearningInfo:Singleton<LearningInfo> {

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

	// 已学习时间
	public int learnTime;

	// 装载所有已学习单词的列表容器
	public List<Word> learnedWords = new List<Word> ();
	// 装载所有未学习单词的列表容器
	public List<Word> unlearnedWords = new List<Word>();

	// 当前设置状态下的单词类型
	public WordType wordType{

		get{
			return GameManager.Instance.gameSettings.wordType;
		}

	}

	// 单词是否已经学习过, 根据需要决定是否扩展'不熟悉'
	private enum WordStatus
	{
		Learned,
		Unlearned
	}

	// 空构造函数
	private LearningInfo(){

	}

	/// <summary>
	/// 从数据库中读取对应类型的单词
	/// </summary>
	public void SetUpWords(){

		string tableName = string.Empty;

		switch (wordType) {
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

		// 检查存放指定单词类型的表是否存在（目前只做了测试用的CET4这一个表，添加表使用参考editor文件夹下的DataBaseManager）
		if (!sql.CheckTableExist (tableName)) {
			Debug.Log ("查询的表不存在");
			return;
		}

		// 检查表中字段名称（目前设定表中字段为：单词id，拼写，释义，例句，是否学习过）
		sql.CheckFiledNames (tableName, new string[]{ "wordId", "spell", "explaination", "example","learned" });

		// 读取器
		IDataReader reader = sql.ReadFullTable (tableName);

		// 从表中读取数据
		while (reader.Read ()) {

			int wordId = reader.GetInt32 (0);
			string spell = reader.GetString (1);
			string explaination = reader.GetString (2);
			string example = reader.GetString (3);
			bool learned = reader.GetBoolean (4);

			Word w = new Word (wordId, spell, explaination, example);

			if (learned) {
				learnedWords.Add (w);
			} else {
				unlearnedWords.Add (w);
			}
		}
	}

}



// 单词模型  
[System.Serializable]
public class Word{

	public int wordId;

	public string spell;

	public string explaination;

	public string example;


	public Word(int wordId,string spell,string explaination,string example){
		this.wordId = wordId;
		this.spell = spell;
		this.explaination = explaination;
		this.example = example;
	}

}