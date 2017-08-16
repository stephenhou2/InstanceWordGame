using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Net;
using System.Text;
using System.Data;
using UnityEngine.UI;

namespace WordJourney{
	public class Test: MonoBehaviour {

		public Animator effectAnimator;

		public Image image;


		public void PicTest(){

			Texture2D t = image.mainTexture as Texture2D;

			Debug.Log (t.width);
			Debug.Log (t.height);


		}

		public void Tt(){
	//		StartCoroutine ("TestClick");

	//		Item inscription = Item.CreateInscription ("abandon");
	//
	//		if (inscription != null) {
	//			Debug.Log (inscription.itemName);
	//			Debug.Log (inscription.GetItemPropertiesString ());
	//		}

			StartCoroutine ("ResetWordsData");

		}

		private void ResetWordsData(){

			MySQLiteHelper sql = MySQLiteHelper.Instance;



			sql.GetConnectionWith (CommonData.dataBaseName);



				sql.UpdateSpecificColsWithValues ("AllWordsData",
					new string[]{ "Valid" },
					new string[]{ "1" },
					null,
					true);

			sql.CloseConnection (CommonData.dataBaseName);

		}

		public IEnumerator TestClick(){

			effectAnimator.gameObject.SetActive (true);

			effectAnimator.SetTrigger ("HealEffect");

			Debug.Log ("特效开始");

			yield return null;

	//		float normalizedTime = effectAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
	//
	//		Debug.Log(normalizedTime);
	//
	//		while (normalizedTime < 1) {
	//			Debug.Log (normalizedTime);
	//			normalizedTime = effectAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
	//			yield return null;
	//		}
	//
	//		Debug.Log ("特效结束");
	//
	//		effectAnimator.SetBool ("anim", false);



	//		effectAnimator.gameObject.SetActive (false);



		}



		private List<string> wordsArray = new List<string>();


		public void DataLoader(){

			string tempString = LoadStringWithPath ("/Users/houlianghong/Downloads/美式英语单词发音完整版/content.txt");

			SeperateString(tempString);

			BuildDataBase ();

		}

		public string LoadStringWithPath(string path){

			StreamReader stream = null;

			try{

				stream = new StreamReader (path);

				string tempString = stream.ReadToEnd ();

				return tempString;


			}catch(Exception e){
				Debug.Log (e);
				return null;
			}

		}

		private void SeperateString(string str){

			string[] pathArray = str.Split (new string[]{ "\n" },StringSplitOptions.RemoveEmptyEntries);

			for (int i = 0; i < pathArray.Length; i++) {

				string[] tempStringArray = pathArray[i].Split(new char[]{ '\\', '.' });

				string tempStr = tempStringArray [tempStringArray.Length - 2];

				if (tempStr.Contains ("(")) {
					continue;
				}

				wordsArray.Add (tempStr);

			}
		}

		private void BuildDataBase(){

			MySQLiteHelper sql = MySQLiteHelper.Instance;

			sql.GetConnectionWith (CommonData.dataBaseName);

			if (!sql.CheckTableExist ("AllWords")) {
				sql.CreateTable ("AllWords", 
					new string[]{ 
						"ID", 
						"SPELL", 
						"EXPLAINATION" 
				}, new string[] {
					"PRIMARY KEY NOT NULL",
					"NOT NULL",
					"NOT NULL"
				}, new string[]{ 
						"INTEGER",
						"TEXT",
						"TEXT" });
				return;
			}

	//		sql.DeleteAllDataFromTable ("AllWords");


			for (int i = 0; i < 2294; i++) {

				string word = wordsArray [i];



				string explaination = GetTranslation (word);

				if (explaination == null) {
					Debug.Log (word);
					explaination = string.Empty;
				}


				sql.CheckFiledNames ("AllWords", new string[]{ "ID", "SPELL", "EXPLAINATION" });

				word = word.Replace ("'", "''");

				sql.InsertValues ("AllWords", new string[]{ i.ToString (), "'" + word + "'", "'" + explaination + "'"});

			}


			sql.CloseAllConnections();

		}

		public string GetTranslation(string word){

			try {

				string url = string.Format("http://www.iciba.com/{0}",word);

				HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create (new Uri (url));

				webRequest.Method = "GET";

				webRequest.CachePolicy = new System.Net.Cache.HttpRequestCachePolicy(System.Net.Cache.HttpRequestCacheLevel.NoCacheNoStore);

				// Set some reasonable limits on resources used by this request
				webRequest.MaximumAutomaticRedirections = 4;
				webRequest.MaximumResponseHeadersLength = 4;
				// Set credentials to use for this request.
				webRequest.Credentials = CredentialCache.DefaultCredentials;
				HttpWebResponse response = null;

				try{
					response = (HttpWebResponse)webRequest.GetResponse ();
				}catch(WebException we){
					Debug.Log(we);
				}

				if(response == null){
					return null;
				}

				//				Debug.Log ("Content length is {0}", response.ContentLength);
				//				Debug.Log ("Content type is {0}", response.ContentType);

				// Get the stream associated with the response.
				Stream receiveStream = response.GetResponseStream ();

				// Pipes the stream to a higher level stream reader with the required encoding format. 
				StreamReader readStream = new StreamReader (receiveStream, System.Text.Encoding.UTF8);

				//				Debug.Log ("Response stream received.");

				string dataStr = readStream.ReadToEnd ();

				response.Close ();
				readStream.Close ();


				string[] strArray = dataStr.Split(new string[]{"<li class=\"clearfix\">","<li class=\"change clearfix\">"},StringSplitOptions.RemoveEmptyEntries);

				List<string> targetStrList = new List<string>();

				if(strArray.Length == 2){

					string targetStr = (strArray[1].Split(new string[]{"<!-- 一元好课 广告 临时 end -->"},StringSplitOptions.RemoveEmptyEntries))[0];
					targetStrList.Add(targetStr);

				}else{
					for(int i = 1;i<strArray.Length - 1;i++){
						targetStrList.Add(strArray[i]);
					}
				}

				StringBuilder finalExplaination = new StringBuilder();

				for(int i = 0;i<targetStrList.Count;i++){

					targetStrList[i] = RemoveTargetStrings(targetStrList[i],new string[]{"\n"," ","</span>","<span>","<p>","</p>","<spanclass=\"prop\">","<li>","</li>","<ul>","</ul>"});

					targetStrList[i] = targetStrList[i].Replace("释义",string.Empty);

					targetStrList[i] = targetStrList[i].Replace("'","''");

					if(i == 0){
						finalExplaination.Append(targetStrList[i]);
					}else{
						finalExplaination.AppendFormat("\n{0}",targetStrList[i]);
					}

				}

				return finalExplaination.ToString();


			} catch (Exception e) {

				Debug.Log (e);

				return null;
			}
		}

		private string RemoveTargetStrings(string oriStr,string[] remStrs){

			StringBuilder str = new StringBuilder ();
			str.Append (oriStr);

			foreach (string s in remStrs) {
				str = str.Replace (s, string.Empty);
			}

			return str.ToString ();
		}

	//	private void testFunc(){
	//		
	//		MySQLiteHelper sql = MySQLiteHelper.Instance;
	//
	//		sql.GetConnectionWith (CommonData.dataBaseName);
	//
	//		sql.ReadSpecificRowsAndColsOfTable ("CET4", "wordId", new string[]{"wordId=0"}, true);
	//
	//	}


	}
}