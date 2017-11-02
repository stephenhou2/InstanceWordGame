using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


namespace WordJourney
{
	public static class DataHandler{

		// 数据转模型
		public static T[] LoadDataToModelWithPath<T>(string fileName){

			string jsonStr = LoadDataString (fileName);

			T[] dataArray = null;

			//模型转换
			try{
				dataArray = JsonHelper.FromJson<T> (jsonStr);
			}catch(System.Exception e){
				Debug.Log (e.Message);
			}
			return dataArray;
		}

		public static T LoadDataToSingleModelWithPath<T>(string fileName){

			string jsonStr = LoadDataString (fileName);

			T instance = default (T);

			if (jsonStr == string.Empty) {
				return instance;
			}

			//模型转换
			try{
				instance = JsonUtility.FromJson<T> (jsonStr);
			}catch(System.Exception e){
				Debug.Log (e.Message);
			}
			return instance;

		}


		// 加载指定路径的文件数据
		public static string LoadDataString(string fileName){
			
			StreamReader sr = null;

			if (!File.Exists (fileName)) {
				Debug.Log(string.Format("can not find file {0}",fileName));
				return string.Empty;
			}

			//读取文件
			try{
				sr = File.OpenText (fileName);
				string dataString = sr.ReadToEnd ();
				sr.Dispose();
				return dataString;

			}catch(System.Exception e){
				Debug.Log (e.Message);
				return null;
			}

		}
			

		public static void SaveInstanceDataToFile<T>(T instance,string filePath){
			
			try{

				string stringData = JsonUtility.ToJson(instance);

				StreamWriter sw = new StreamWriter(filePath,false);

				sw.Write(stringData);

				sw.Dispose();

			}catch(System.Exception e){
				
				Debug.Log (e);

			}

		}

	}
}

