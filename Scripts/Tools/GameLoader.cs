using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Object = UnityEngine.Object;


namespace WordJourney
{
	public class GameLoader : MonoBehaviour {

		private bool hasInitialized;

		void Awake(){

			PersistDataIfFirstLoad ();

			DontDestroyOnLoad (Player.mainPlayer);

			DontDestroyOnLoad (GameManager.Instance);

		}

		private void PersistDataIfFirstLoad(){

			DirectoryInfo persistDi = new DirectoryInfo (CommonData.persistDataPath);

			if (!persistDi.Exists) {
				persistDi.Create ();
				DirectoryInfo originDi = new DirectoryInfo (CommonData.originDataPath);
				FileInfo[] dataFiles = originDi.GetFiles ();
				for (int i = 0; i < dataFiles.Length; i++) {
					FileInfo fi = dataFiles [i];
					string persistFilePath = string.Format ("{0}/{1}", CommonData.persistDataPath, fi.Name);
					fi.CopyTo (persistFilePath);
				}
			}
	
			Debug.Log (CommonData.persistDataPath);

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
