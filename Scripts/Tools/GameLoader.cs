using UnityEngine;
using System;
using System.IO;
using System.Collections;


namespace WordJourney
{
	public class GameLoader : MonoBehaviour {

		public bool alwaysPersistData;

		void Awake(){
			PersistData();
		}

		private IEnumerator InitData(){

			yield return new WaitUntil(()=> MyResourceManager.Instance.isManifestReady);

			LoadDatas ();

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.homeCanvasBundleName, "HomeCanvas", () => {

				TransformManager.FindTransform("HomeCanvas").GetComponent<HomeViewController> ().SetUpHomeView ();

			});
				
		}


		/// <summary>
		/// 初始化游戏基础数据
		/// </summary>
		private void LoadDatas(){
			
			GameManager.Instance.gameDataCenter.InitPersistentGameData ();

			PlayerData playerData = GameManager.Instance.persistDataManager.LoadPlayerData ();

			Player.mainPlayer.SetUpPlayerWithPlayerData (playerData);

		}



		private void PersistData(){

			Debug.Log (CommonData.persistDataPath);

			DirectoryInfo persistDi = new DirectoryInfo (CommonData.persistDataPath);

			if (!persistDi.Exists) {
				DataHandler.CopyDirectory (CommonData.originDataPath, CommonData.persistDataPath, true);
				StartCoroutine ("InitData");
				return;
			}

			if (alwaysPersistData) {
				DataHandler.CopyDirectory (CommonData.originDataPath, CommonData.persistDataPath, true);
			}

			StartCoroutine ("InitData");

		}



	}
}
