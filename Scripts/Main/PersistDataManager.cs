﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	public class PersistDataManager{

		/// <summary>
		/// 保存游戏设置，学习信息，玩家游戏数据到本地
		/// </summary>
		public void SavePersistDatas(){
			SaveGameSettings ();
			SaveLearnInfo ();
			SavePlayerData ();
		}

		/// <summary>
		/// Saves the game settings.
		/// </summary>
		public void SaveGameSettings(){
			
			string gameSettingsPath = string.Format ("{0}/{1}", CommonData.persistDataPath, "GameSettings.json");

			DataHandler.SaveInstanceDataToFile<GameSettings> (GameManager.Instance.gameDataCenter.gameSettings, gameSettingsPath);
		}

		/// <summary>
		/// Saves the learn info.
		/// </summary>
		public void SaveLearnInfo(){
			
			string learnInfoPath = string.Format ("{0}/{1}", CommonData.persistDataPath, "LearnInfo.json");

			DataHandler.SaveInstanceDataToFile<LearningInfo> (GameManager.Instance.gameDataCenter.learnInfo, learnInfoPath);
		}

		/// <summary>
		/// Saves the player data.
		/// </summary>
		public void SavePlayerData(){
			
			string playerDataPath = string.Format ("{0}/{1}", CommonData.persistDataPath, "PlayerData.json");

			PlayerData playerData = new PlayerData (Player.mainPlayer);

			DataHandler.SaveInstanceDataToFile<PlayerData> (playerData, playerDataPath);
		}

		/// <summary>
		/// 从本地加载玩家游戏数据
		/// </summary>
		public PlayerData LoadPlayerData(){
			
			string playerDataPath = string.Format ("{0}/{1}", CommonData.persistDataPath, "PlayerData.json");

			return DataHandler.LoadDataToSingleModelWithPath<PlayerData> (playerDataPath);

		}


		public GameSettings LoadGameSettings(){
			string settingsPath = string.Format ("{0}/{1}", CommonData.persistDataPath, "Settings.json");
			return DataHandler.LoadDataToSingleModelWithPath<GameSettings> (settingsPath);
		}

		public LearningInfo LoadLearnInfo(){
			string learnInfoPath = string.Format ("{0}/{1}", CommonData.persistDataPath, "LearningInfo.json");
			return DataHandler.LoadDataToSingleModelWithPath<LearningInfo> (learnInfoPath);
		}




	}
}