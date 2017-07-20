using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class DataInitializer{


	// 数据转模型
	public static T[] LoadDataToModelWithPath<T>(string filePath,string fileName){

		string jsonStr = LoadDataString (filePath, fileName);

		T[] dataArray = null;

		//模型转换
		try{
			dataArray = JsonHelper.FromJson<T> (jsonStr);
		}catch(System.Exception e){
			Debug.Log (e.Message);
		}
		return dataArray;
	}

	public static T LoadDataToSingleModelWithPath<T>(string filePath,string fileName){

		string jsonStr = LoadDataString (filePath, fileName);

		T instance = default (T);

		//模型转换
		try{
			instance = JsonUtility.FromJson<T> (jsonStr);
		}catch(System.Exception e){
			Debug.Log (e.Message);
		}
		return instance;

	}


	// 加载指定路径的文件数据
	public static string LoadDataString(string filePath,string fileName){
		
		StreamReader sr = null;

		//读取文件
		try{
			sr = File.OpenText (filePath + "/" + fileName);
			string dataString = sr.ReadToEnd ();
			Debug.Log(dataString);
			return dataString;

		}catch(System.Exception e){
			Debug.Log (e.Message);
			return null;
		}
	}


}

