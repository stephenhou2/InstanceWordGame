using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class PicTool {
	
	[MenuItem("EditHelper/PicTool")]
	public static void GeneratePic(){

		ShowPicTool ();




	}

	private static void ShowPicTool(){
		
//		PicToolWindow tool = EditorWindow.GetWindow<PicToolWindow> (true);

		PicToolWindow tool = PicToolWindow.instance;

		tool.Show ();



	}

}

public class PicToolWindow:EditorWindow{

	private static PicToolWindow mInstance;

	public static PicToolWindow instance{

		get{
			if (mInstance == null) {
				mInstance = ScriptableObject.CreateInstance<PicToolWindow> ();
				mInstance.ShowUtility ();
			}

			return mInstance;
		}


	}

	private string picName;

	private int width;

	private int height;

	public void OnGUI(){

		//绘制一个文本框
		picName=EditorGUILayout.TextField("picName:",picName);


		GUILayout.BeginHorizontal (new GUILayoutOption[] {
			GUILayout.Height(20),
			GUILayout.Width(100),
		});

		EditorGUILayout.LabelField ("width:");

		EditorGUILayout.LabelField (width.ToString());

		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal (new GUILayoutOption[] {
			GUILayout.Height(20),
			GUILayout.Width(100),
		});

		EditorGUILayout.LabelField ("height:");

		EditorGUILayout.LabelField (height.ToString());

		GUILayout.EndHorizontal ();

		//绘制一个按钮
		if (GUILayout.Button ("Generate", GUILayout.Height (20))) {
			if (picName != string.Empty) {
				GeneratePicWithName (picName);
			}
		}

	}

	private void GeneratePicWithName(string picName){

		GameObject go = Selection.gameObjects [0];
		Debug.Log (go);

		Image image = go.GetComponent<Image> ();



		Texture t2d = image.mainTexture as Texture;



		if (t2d != null) {

			width = t2d.width;

			height = t2d.height;

			Repaint ();
		}



	}



}
