using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;


namespace WordJourney
{

//	using UnityEngine.SceneManagement;
	using System.IO;

	public class GameManager : MonoBehaviour {

		private static volatile GameManager instance;  
		private static object objectLock = new System.Object();  
		public static  GameManager Instance {  
			get {  
				if (instance == null) {  
					lock (objectLock) {  
						GameManager[] instances = FindObjectsOfType<GameManager> ();  
						if (instances != null) {  
							for (var i = 0; i < instances.Length; i++) {  
								Destroy (instances [i].gameObject);  
							}  
						} 

						ResourceLoader gameManagerLoader = ResourceLoader.CreateNewResourceLoader ();

						ResourceManager.Instance.LoadAssetsWithBundlePath (gameManagerLoader, CommonData.mainStaticBundleName, ()=>{
							instance = gameManagerLoader.gos[0].GetComponent<GameManager>();
							instance.transform.SetParent(null);
						}, true,"GameManager");
					}  

					DontDestroyOnLoad (instance);
				}  
				return instance;  
			}  
		}

		public SoundManager soundManager;

		public GameDataCenter gameDataCenter;

		public UIManager UIManager;

		public PersistDataManager persistDataManager;




		void Awake(){

			gameDataCenter = new GameDataCenter ();

			UIManager = new UIManager ();

			persistDataManager = new PersistDataManager ();

		}
			
		#warning 如果决定使用scene来进行场景转换打开下面的代码
//		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
//		static public void CallbackInitialization()
//		{
//			//register the callback to be called everytime the scene is loaded
//			SceneManager.sceneLoaded += OnSceneLoaded;
//		}
//
//		static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
//		{
//			switch (arg0.name) {
//			case "GameScene":
//				TransformManager.FindTransform ("GameLoader").GetComponent<GameLoader> ().SetUpHomeView ();
//				break;
//			case "ExploreScene":
//				int currentExploreLevel = GameManager.Instance.unlockedMaxChapterIndex;
//
//				ResourceLoader exploreSceneLoader = ResourceLoader.CreateNewResourceLoader ();
//
//				ResourceManager.Instance.LoadAssetsWithBundlePath (exploreSceneLoader, "explore/scene", () => {
//
//					TransformManager.FindTransform ("ExploreManager").GetComponent<ExploreManager> ().SetupExploreView (currentExploreLevel);
//
//				}, true);
//				break;
//
//			}
//
//		}


	}
}
