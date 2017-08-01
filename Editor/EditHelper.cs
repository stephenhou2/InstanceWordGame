using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

using System.Linq;
using System.Text;
using CE.iPhone.PList;
using System.IO;

public class EditHelper {




	[MenuItem("EditHelper/Execute")]
	public static void Execute(){



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


				Texture2D myimage = new Texture2D (width, height);


				for (int x = lowerLeftCornerX; x < lowerLeftCornerX + width; x++) {
					for (int y = lowerLeftCornerY; y < lowerLeftCornerY + height; y++) {
						myimage.SetPixel (x - lowerLeftCornerX, y - lowerLeftCornerY, texture.GetPixel (x, y));
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