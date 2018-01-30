using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace WordJourney
{

//	using UnityEngine.Rendering;
	using UnityEngine.Rendering;
	using UnityEngine.UI;
	using System;

	using System.IO;

	public class Test : MonoBehaviour {

		public Sprite s;

		public RawImage img;
		public RawImage img2;
		public Text text;


		public Font font;
		public Material numFontMat;
		public TextAsset ta;
		public float totalWidth = 97f;
		public float totalHeight = 107f;


		public Transform shareContainer;
		public Text shareText;
		public Button shareButton;

		public Button weChatShareButton;

		public void OnWeCharShareButtonClick(){


			shareText.text = "share test!!";

			shareContainer.gameObject.SetActive (true);

			shareButton.gameObject.SetActive (true);
		}

		public void OnShareButtonClick(){

			shareButton.gameObject.SetActive (false);

			StartCoroutine ("TrimScreenShotAndShare");

		}

		private IEnumerator TrimScreenShotAndShare(){

			yield return new WaitForEndOfFrame();

			Texture2D t2d = ScreenCapture.CaptureScreenshotAsTexture (1);

			shareContainer.gameObject.SetActive (false);

//			yield return new WaitForSeconds (0.5f);

//			byte[] data = File.ReadAllBytes (Application.persistentDataPath + "/screenShot.png");
//
//			Texture2D t2d = new Texture2D (1080, 1920);

//			bool success = t2d.LoadImage (data);

//			Debug.Log (success);

			Texture2D newT2d = new Texture2D (480, 1320);

			for (int i = 300; i < t2d.width-300; i++) {
				for (int j = 300; j < t2d.height-300; j++) {

					Color c = t2d.GetPixel(i,j);

					newT2d.SetPixel (i - 300, j - 300, c);

				}
			}

			newT2d.Apply ();

			img.texture = newT2d;

		}


		private CharacterInfo[] GetCharacterInfosFromCharacter(char[] charArray){

			CharacterInfo[] ciArray = new CharacterInfo[charArray.Length];

			for(int i = 0;i<charArray.Length;i++){
				CharacterInfo ci;
				font.GetCharacterInfo (charArray [i], out ci);
				ciArray [i] = ci;
			}

			return ciArray;

		}

		public void GenerateTextureTest(){

			Texture2D newT2d = Instantiate (s.texture) as Texture2D;

			CharacterInfo[] ciArray = GetCharacterInfosFromCharacter (new char[]{ '1', '0' });

			Texture2D numT2d = numFontMat.mainTexture as Texture2D;

			for (int i = 0; i < ciArray.Length; i++) {

				CharacterInfo ci = ciArray [i];

				for (int j = ci.minX; j < ci.maxX; j++) {

					for (int k = ci.maxY; k < ci.minY; k++) {

						Color c = numT2d.GetPixel (j, k);

						if (c.a == 0) {
							continue;
						}

						newT2d.SetPixel (j + 100, k + 100, c);

					}

				}

			}

			newT2d.Apply ();



			img.texture = newT2d;


//			Sprite newSprite = Sprite.Create (newT2d, new Rect(Vector2.zero,new Vector2(newT2d.width,newT2d.height)), new Vector2 (0.5f, 0.5f));
//
//			img.sprite = newSprite;

		}




		public void Import(TextAsset rects, Font font, Vector3 size)
		{
			string[] lines = rects.text.Split(new string[]{"\n"}, StringSplitOptions.None);
			CharacterInfo[] info = new CharacterInfo[lines.Length];
			for (var i = 0; i < lines.Length; i++)
			{
				string[] line = lines[i].Split(new string[]{"="," "}, StringSplitOptions.None);
				int id = Convert.ToInt32 (line [2]);
				int x = Convert.ToInt32(line[4]);
				int y = Convert.ToInt32(line[6]);
				int width = Convert.ToInt32(line[8]);
				int height = Convert.ToInt32(line[10]);
				int offsetX = Convert.ToInt32(line[12]);
				int offsetY = Convert.ToInt32 (line [14]);
				int advance = Convert.ToInt32 (line [16]);

				info[i].uv.x = x / size.x;
				info[i].uv.y = 1 - y / size.y;
				info[i].uv.width = width / size.x;
				info[i].uv.height = -height / size.y;
				info[i].vert.x = offsetX;
				info[i].vert.y = -offsetY;
				info[i].vert.width = width;
				info[i].vert.height = height;
				info[i].width = advance;
				info[i].index = id;
			}
			font.characterInfo = info;
			AssetDatabase.SaveAssets();
		}



	}


}
