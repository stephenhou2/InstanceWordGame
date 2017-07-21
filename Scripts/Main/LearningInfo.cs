using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;


[System.Serializable]
public class LearningInfo:Singleton<LearningInfo> {

	public int totalWordCount{
		get{
			return learnedWordCount + unlearnedWords.Count;
		}

	}

	public int learnedWordCount{
		get{
			return learnedWords.Count;
		}
	}

	public int learnTime;

	public List<Word> learnedWords = new List<Word> ();
	public List<Word> unlearnedWords = new List<Word>();

	public WordType wordType{

		get{
			return GameManager.Instance.gameSettings.wordType;
		}

	}

	private enum WordStatus
	{
		Learned,
		Unlearned
	}


	private LearningInfo(){

	}

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

		sql.GetConnectionWith (CommonData.dataBaseName);

		if (!sql.CheckTableExist (tableName)) {
			Debug.Log ("查询的表不存在");
			return;
		}

		sql.CheckFiledNames (tableName, new string[]{ "wordId", "spell", "explaination", "example","learned" });

		IDataReader reader = sql.ReadFullTable (tableName);

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