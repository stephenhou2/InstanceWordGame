using UnityEngine;
using System;
using System.IO;
using System.Collections;


namespace WordJourney
{
	public class GameLoader : MonoBehaviour {

		private int maxCaching = 100;

		void Awake(){

			StartCoroutine ("PersistDataIfFirstLoad");

		}

		private void InitGame(){

			SetUpSystemSettings ();
			
			DontDestroyOnLoad (Player.mainPlayer);

			DontDestroyOnLoad (GameManager.Instance);

			GameManager.Instance.SetUpHomeView (Player.mainPlayer);



		}

		/// <summary>
		/// 初始化系统设置
		/// </summary>
		private void SetUpSystemSettings(){
			
			Caching.maximumAvailableDiskSpace = maxCaching * 1024 * 1024;

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
