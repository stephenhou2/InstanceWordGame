using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	public class PersistDataManager{

		public void SavePersistDatas(){

			string gameSettingsPath = string.Format ("{0}/{1}", CommonData.persistDataPath, "GameSettings.json");
			string learnInfoPath = string.Format ("{0}/{1}", CommonData.persistDataPath, "LearnInfo.json");
			string playerDataPath = string.Format ("{0}/{1}", CommonData.persistDataPath, "PlayerData.json");


			DataHandler.SaveInstanceDataToFile<GameSettings> (GameManager.Instance.gameDataCenter.gameSettings, gameSettingsPath);
			DataHandler.SaveInstanceDataToFile<LearningInfo> (GameManager.Instance.gameDataCenter.learnInfo, learnInfoPath);

			PlayerData playerData = new PlayerData (Player.mainPlayer);
			DataHandler.SaveInstanceDataToFile<PlayerData> (playerData, playerDataPath);


		}

		// 系统设置更改后更新相关设置
		public void OnSettingsChanged(){

			GameManager.Instance.soundManager.effectAS.volume = GameManager.Instance.gameDataCenter.gameSettings.systemVolume;
			GameManager.Instance.soundManager.bgmAS.volume = GameManager.Instance.gameDataCenter.gameSettings.systemVolume;

			GameManager.Instance.soundManager.pronunciationAS.enabled = GameManager.Instance.gameDataCenter.gameSettings.isPronunciationEnable;


			#warning 离线下载和更改词库的代码后续补充
			// 保存游戏设置到本地文件
			DataHandler.SaveInstanceDataToFile <GameSettings>(GameManager.Instance.gameDataCenter.gameSettings, CommonData.settingsFilePath);

		}


	}
}
