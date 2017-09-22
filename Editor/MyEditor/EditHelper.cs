using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

using System.Linq;
using System.Text;
using CE.iPhone.PList;
using System.IO;
using System.Data;

using UnityEngine.EventSystems;

using WordJourney;

namespace WordJourney{
	public class EditHelper {
		
		[MenuItem("EditHelper/Test")]
		public static void Test(){



		}


		[MenuItem("EditHelper/Execute")]
		public static void Execute(){


			ModifyCharacterButtons ();



		}


		private static void ModifyCharacterButtons(){

			GameObject charactersPlane = GameObject.Find ("CharactersPlane");

			Button[] characterButtons = charactersPlane.GetComponentsInChildren<Button> ();

			for (int i = 0; i < characterButtons.Length - 1; i++) {

				Button btn = characterButtons [i];

				Transform newTintHUD = btn.transform.Find ("TintHUD");

				Text t = newTintHUD.GetComponentInChildren<Text> ();

				char ch = (char)((int)('a') + i);

				t.text = ch.ToString ().ToUpper ();

				newTintHUD.gameObject.SetActive (false);

				btn.gameObject.AddComponent<CharacterButton> ();

//				EventTrigger et = btn.gameObject.GetComponent<EventTrigger> ();
//
//				EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry ();
//
//				pointerDownEntry.eventID = EventTriggerType.PointerDown;
//
//
//				pointerDownEntry.callback.RemoveAllListeners ();
//
////				BaseEventData data = new BaseEventData (es);
////				data.
//
//
//				pointerDownEntry.callback.AddListener ((PointerEventData data) => {
//					spellViewController.CharacterButtonDown (data);
//				});


//				pointerDownEntry.callback.AddListener(
//
//				EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry ();
//
//				pointerUpEntry.eventID = EventTriggerType.PointerUp;
//
//				pointerUpEntry.callback.AddListener ((data) => {
//					OnPointerUpDelegate((PointerEventData)data);
//					spellViewController.CharacterButtonUp ((int)i);
//				});
//
//				et.triggers.Add (pointerDownEntry);
//				et.triggers.Add (pointerUpEntry);

			}

		}
			
		public static void OnPointerDownDelegate(PointerEventData data)
		{
			Debug.Log ("pointerDown");

		}
		public static void OnPointerUpDelegate(PointerEventData data)
		{
			Debug.Log ("pointerup");
		}

		// 将物品csv数据转化为json文件并存储直接使用这个方法
		private static void ConvertItemToJson(){
			
			ItemModel[] allItem = ItemsToJson ();

			AllItemsJson aij = new AllItemsJson ();

			aij.Items = allItem;

			string allItemsJsonStr = JsonUtility.ToJson (aij);

			File.WriteAllText (CommonData.persistDataPath + "/AllItemsJson.txt", allItemsJsonStr);

		}

		static private ItemModel[] ItemsToJson(){

			string csv = DataHandler.LoadDataString ("itemsData.csv");

			string[] dataArray = csv.Split (new string[]{ "\n" }, System.StringSplitOptions.RemoveEmptyEntries);

			List <ItemModel> allItem = new List<ItemModel> ();

			for (int i = 1; i < dataArray.Length; i++) {

				string[] itemStr = dataArray [i].Split (',');
					
				ItemModel itemModel = new ItemModel ();

				itemModel.itemId = System.Convert.ToInt32 (itemStr [0]);
				itemModel.itemName = itemStr[1];
				itemModel.itemDescription = itemStr[2];
				itemModel.spriteName = itemStr[3];
				itemModel.itemType = (ItemType)System.Convert.ToInt16(itemStr[4]);
				itemModel.itemNameInEnglish = itemStr[5];
				itemModel.attackGain = System.Convert.ToInt16(itemStr[6]);
				itemModel.attackSpeedGain = System.Convert.ToInt16(itemStr[7]);
				itemModel.armourGain = System.Convert.ToInt16(itemStr[8]);
				itemModel.manaResistGain = System.Convert.ToInt16(itemStr[9]);
				itemModel.critGain = System.Convert.ToInt16(itemStr[10]);
				itemModel.dodgeGain = System.Convert.ToInt16(itemStr[11]);
				itemModel.healthGain = System.Convert.ToInt16(itemStr[12]);
				itemModel.manaGain = System.Convert.ToInt16 (itemStr [13]);
				itemModel.equipmentType = (EquipmentType)System.Convert.ToInt16 (itemStr [14]);

				allItem.Add (itemModel);

			}

			ItemModel[] ItemArray = new ItemModel[allItem.Count];

			allItem.CopyTo(ItemArray);


			return ItemArray;

		}

		public class AllItemsJson{

			public ItemModel[] Items;

		}



		private void ToLower(){
			MySQLiteHelper sql = MySQLiteHelper.Instance;
			sql.GetConnectionWith (CommonData.dataBaseName);

			for (int i = 0; i < 37336; i++) {

				IDataReader reader = sql.ReadSpecificRowsAndColsOfTable (
					"AllWordsData",
					"Spell",
					new string[]{ string.Format ("Id={0}", i) },
					true);
				reader.Read ();

				string spell = reader.GetString (0);

				string lowerSpell = spell.ToLower ();

				if (lowerSpell == spell) {
					continue;
				}

				lowerSpell = lowerSpell.Replace("'","''");

				sql.UpdateSpecificColsWithValues ("AllWordsData", 
					new string[]{ "Spell" },
					new string[]{ string.Format("'{0}'",lowerSpell) },
					new string[]{string.Format("Id = {0}",i)},
					true);

				reader.Close ();

			}



			sql.CloseConnection (CommonData.dataBaseName);
		}

		private void MoveData(){
			MySQLiteHelper sql = MySQLiteHelper.Instance;

			sql.GetConnectionWith (CommonData.dataBaseName);

			sql.CreateTable ("AllWordsData",
				new string[]{ "Id", "Spell", "Explaination", "Type", "Valid" },
				new string[]{ "PRIMARY KEY NOT NULL", "UNIQUE NOT NULL", "NOT NULL", "NOT NULL", "NOT NULL" },
				new string[]{ "INTEGER", "TEXT", "TEXT", "INTEGER", "INTEGER" });

			sql.DeleteAllDataFromTable ("AllWordsData");

			IDataReader reader = null;
			int pad = 0;

			for (int i = 0; i < 39286; i++) {

				if (i == 34250) {
					pad++;
					continue;
				}

				reader = sql.ReadSpecificRowsAndColsOfTable ("AllWords", "*",
					new string[]{ string.Format ("ID={0}", i) },
					true);

				reader.Read ();



				int id = i - pad;
				string spell = reader.GetString (1);
				string explaination = reader.GetString (2);
				int type = 0;
				int valid = 1;

				if (spell == string.Empty || explaination == string.Empty || spell == null || explaination == null) {
					pad++;
					continue;
				}

				spell = spell.Replace ("'", "''");
				explaination = explaination.Replace ("'", "''");

				sql.InsertValues ("AllWordsData",
					new string[] {id.ToString (),
						"'" + spell + "'",
						"'" + explaination + "'",
						type.ToString (),
						valid.ToString ()
					});

				reader.Close ();
			}


			Debug.Log ("Finished");

			sql.CloseConnection (CommonData.dataBaseName);

		}



		[MenuItem("EditHelper/SeperateAnimPics")]
		public static void SeperateAnimPics(){

			for (int index = 1; index < 23; index++) {

				string plistName = string.Format("Assets/Resources/Effect/ef{0}.plist",index);

				PListRoot root = PListRoot.Load (plistName);

				PListDict dic = (PListDict)root.Root;  

				PListDict frames = (PListDict)dic["frames"];  

				string picName = string.Format("Effect/ef{0}",index);

				Texture2D texture = Resources.Load<Texture2D> (picName);


				for(int i = 0;i<frames.Count;i++){

					KeyValuePair<string,IPListElement> kv = frames.ElementAt (i);

					PListDict picDict = kv.Value as PListDict;

					PListString frame = (PListString)picDict ["frame"];


					PListBool rotated = (PListBool)picDict["rotated"];

					int bigPicHeight = texture.height;

					FrameStruct fs = new FrameStruct (frame.ToString (),rotated,bigPicHeight);



					int lowerLeftCornerX= fs.lowerLeftCornerX;
					int lowerLeftCornerY = fs.lowerLeftCornerY;

					int width = fs.width;
					int	height = fs.height;

					Texture2D myimage = null;

					if (!rotated) {
						myimage = new Texture2D (width, height);
					} else {
						myimage = new Texture2D (height, width);
					}


					for (int x = lowerLeftCornerX; x < lowerLeftCornerX + width; x++) {
						for (int y = lowerLeftCornerY; y < lowerLeftCornerY + height; y++) {

							if (!rotated) {
								myimage.SetPixel (x - lowerLeftCornerX, y - lowerLeftCornerY, texture.GetPixel (x, y));
							} else {
								myimage.SetPixel (height + y - lowerLeftCornerY, x - lowerLeftCornerX, texture.GetPixel (x, y));
				
							}
						}
					}  


					//转换纹理到EncodeToPNG兼容格式  
					if(myimage.format != TextureFormat.ARGB32 && myimage.format != TextureFormat.RGB24){  
						Texture2D newTexture = new Texture2D(myimage.width, myimage.height);  
						newTexture.SetPixels(myimage.GetPixels(0),0);  
						myimage = newTexture;  
					}  
					var pngData = myimage.EncodeToPNG();  



					string targetPath = "Assets/MyEffect/" + kv.Key;

					int lastBsIndex = targetPath.LastIndexOf ('/');
					string directoryPath = targetPath.Remove (lastBsIndex, targetPath.Length - lastBsIndex);

					if (!Directory.Exists(directoryPath)) {

						Directory.CreateDirectory (directoryPath);
					}

					FileStream stream = new FileStream (targetPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

					stream.Write (pngData, 0, pngData.Length);

					stream.Close ();



					//			File.Create(targetPath,

					//			File.WriteAllBytes(targetPath, pngData);  

				}
			}
			Debug.Log ("finished");
			AssetDatabase.Refresh ();

		}


	}





	public struct FrameStruct{


		public int lowerLeftCornerX;
		public int lowerLeftCornerY;



		public int width;
		public int height;

		public FrameStruct(string frameStr,bool rotated,int bigPicHeight){

			string frame = frameStr.Replace ("{", string.Empty).Replace ("}", string.Empty);

			string[] values = frame.Split (new char[]{ ',' });


			if (!rotated) {

				width = System.Convert.ToInt32 (values [2]);

				height = System.Convert.ToInt32 (values [3]);

			} else {
				width = System.Convert.ToInt32 (values [3]);

				height = System.Convert.ToInt32 (values [2]);

			}

			lowerLeftCornerX = System.Convert.ToInt32 (values [0]);
			lowerLeftCornerY = bigPicHeight - System.Convert.ToInt32 (values [1]) - height;

		}

	}
}