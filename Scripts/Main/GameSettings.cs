using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class GameSettings {

	public bool isPronunciationEnable = true;

	public bool isDownloadEnable = false;

	public int systemVolume = 50;

	public WordType wordType = WordType.CET4;

	public override string ToString ()
	{
		return string.Format ("[GameSettings]-isPronunciationEnable{0},isDownloadEnable{1},systemVolume{3},wordType{4}",isPronunciationEnable,isDownloadEnable,systemVolume,wordType);
	}


}
