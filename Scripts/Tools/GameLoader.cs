using UnityEngine;
using System;
using System.IO;
using System.Collections;


namespace WordJourney
{
	public class GameLoader : MonoBehaviour {

		private int maxCaching = 100;

		void Awake(){

			PersistDataAlways ();
//			StartCoroutine ("PersistDataIfFirstLoad");

		}

		private void InitGame(){

			LoadDatas ();

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

			GameManager.Instance.persistDataManager.SavePlayerData ();

			PlayerData playerData = GameManager.Instance.persistDataManager.LoadPlayerData ();

			Player.mainPlayer.SetUpPlayerWithPlayerData (playerData);

			GameManager.Instance.gameDataCenter.InitItemsAndSkillDataByFormula ();

		}

		#warning 测试时每次都将文件本地化，打包时使用下面的方法，保证只有首次进入游戏会进行文件本地化
		private void PersistDataAlways(){



			Debug.Log ("文件本地化");

			DataHandler.CopyDirectory (CommonData.originDataPath, CommonData.persistDataPath, true);

			InitGame ();

		}

		private void CopySubFiles(){

		}

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
	
			Debug.Log (CommonData.persistDataPath);

			InitGame ();

		}


//		private bool BinaryFileSave(string name,object obj)  
//		{  
//			Stream flstr=null;  
//			BinaryWriter binaryWriter=null;  
//			try  
//			{  
//				flstr = new FileStream(name, FileMode.Create);  
//				binaryWriter = new BinaryWriter(flstr);  
//				var buff = FormatterObjectBytes(obj);  
//				binaryWriter.Write(buff);  
//			}  
//			catch (System.Exception er)  
//			{  
//				throw new System.Exception(er.Message);  
//			}  
//			finally  
//			{  
//				if (binaryWriter != null) binaryWriter.Close();  
//				if (flstr != null) flstr.Close();  
//			}  
//			return true;  
//		}  
//
//
//		private byte[] FormatterObjectBytes(object obj)  
//		{  
//			if(obj==null)  
//				throw new ArgumentNullException("obj");  
//			byte[] buff;  
//			try  
//			{  
//				using (var ms = new MemoryStream())  
//				{  
//					IFormatter iFormatter = new BinaryFormatter();  
//					iFormatter.Serialize(ms, obj);  
//					buff = ms.GetBuffer();  
//				}  
//			}  
//			catch (Exception er)  
//			{  
//				throw new Exception(er.Message);  
//			}  
//			return buff;  
//		}
	}
}
