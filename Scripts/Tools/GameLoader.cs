using UnityEngine;
using System;
using System.IO;
using System.Collections;


namespace WordJourney
{
	public class GameLoader : MonoBehaviour {

		private int maxCaching = 100;

//		private bool finishDataLoading = false;

		void Awake(){
			PersistDataAlways ();
//			StartCoroutine ("PersistDataIfFirstLoad");
		}

		private void InitGame(){

			LoadDatas ();

//			TransformManager.FindTransform ("Path").GetComponent<TextMesh> ().text = Application.streamingAssetsPath;

//			StartCoroutine ("EnterGameAfterFinishingLoadData");

			SetUpHomeView ();

		}

		public void SetUpHomeView(){

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.homeCanvasBundleName, "HomeCanvas", () => {

				TransformManager.FindTransform("HomeCanvas").GetComponent<HomeViewController> ().SetUpHomeView ();

				TransformManager.DestroyTransform(transform);

			});
				
		}


		/// <summary>
		/// 初始化游戏基础数据
		/// </summary>
		private void LoadDatas(){
			
			Caching.maximumAvailableDiskSpace = maxCaching * 1024 * 1024;

			PlayerData playerData = GameManager.Instance.persistDataManager.LoadPlayerData ();

			Player.mainPlayer.SetUpPlayerWithPlayerData (playerData);

//			GameManager.Instance.gameDataCenter.InitItemsAndSkillDataByFormula ();

		}

		#warning 测试时每次都将文件本地化，打包时使用下面的方法，保证只有首次进入游戏会进行文件本地化
		private void PersistDataAlways(){

			Debug.Log ("文件本地化");

//			string playerDataPath = CommonData.originDataPath + "/PlayerData.json";
//			Player.mainPlayer.allEquipedEquipments = new Equipment[6]{ null, null, null, null, null, null };
//			PlayerData data = new PlayerData (Player.mainPlayer);
//
//			DataHandler.SaveInstanceDataToFile<PlayerData> (data, playerDataPath);

			DataHandler.CopyDirectory (CommonData.originDataPath, CommonData.persistDataPath, true);

			ResourceManager.Instance.SetUpManifest ();

			InitGame ();
//			TransformManager.FindTransform ("Path").GetComponent<TextMesh> ().text = Application.streamingAssetsPath;

		}

//		private IEnumerator EnterGameAfterFinishingLoadData(){
//
//			yield return new WaitUntil (() => finishDataLoading == true);
//
//			SetUpHomeView ();
//
//		}

		private IEnumerator PersistDataIfFirstLoad(){

			DirectoryInfo persistDi = new DirectoryInfo (CommonData.persistDataPath);

			if (!persistDi.Exists) {
				
				Debug.Log ("文件本地化");

				persistDi.Create ();

				DirectoryInfo originDi = new DirectoryInfo (CommonData.originDataPath);

				while (persistDi.GetFiles ().Length != originDi.GetFiles ().Length) {

					FileInfo[] dataFiles = originDi.GetFiles ();

					for (int i = 0; i < dataFiles.Length; i++) {
						FileInfo fi = dataFiles [i];
						string persistFilePath = string.Format ("{0}/{1}", CommonData.persistDataPath, fi.Name);
						fi.CopyTo (persistFilePath);
					}

					yield return null;
				}
			}

			ResourceManager.Instance.SetUpManifest ();
	
			Debug.Log (CommonData.persistDataPath);

			InitGame ();

		}



	}
}
