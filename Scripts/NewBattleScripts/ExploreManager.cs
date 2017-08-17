using UnityEngine;
using System.Collections;
using UnityEngine.AI;


namespace WordJourney
{
	using System.Collections.Generic;		//Allows us to use Lists. 
	using UnityEngine.UI;					//Allows us to use UI.
	
	public class ExploreManager : MonoBehaviour
	{
		// 探索控制器单例
//		private static ExploreManager mExploreManager;
//		private static object mLock = new System.Object();
//		public static ExploreManager Instance{
//			get{
//				if (mExploreManager == null) {
//					lock (mLock) {
//						ResourceManager.Instance.LoadAssetWithFileName ("explore/scene", () => {
//							mExploreManager = ResourceManager.Instance.gos.Find (delegate(GameObject obj) {
//								return obj.name == "ExploreManager";
//							}).GetComponent<ExploreManager>();
//						},true);
//					}
//				}
//				return mExploreManager;
//			}
//		}
			
		// 所有转场间隔时间(转场时背景全黑)
		public float TransitionDelay = 0.5f;					
			
		// 地图生成器
		private MapGenerator battleMap;						

		// 当前地图id
		private int currentMapIndex = 0;	
	

		// 当前关卡所有怪物
		private List<BattleMonsterController> battleMonsters;	

		// 玩家控制器
		private BattlePlayerController battlePlayer;

		// 图片遮罩
		public Image maskImage;

		private NavigationHelper navHelper;

		private List<Vector3> pathPosList;

		public GameObject cube;
		
		//Awake is always called before any Start functions
		void Awake()
		{
//			TransitionDelay = 0.5f;
//			currentMapIndex = 0;

			//Assign enemies to a new List of Enemy objects.
			battleMonsters = new List<BattleMonsterController>();

			navHelper = GetComponent<NavigationHelper> ();



		}
			

		 

		private void Update(){

			#if UNITY_STANDALONE || UNITY_EDITOR
			{
				if(Input.GetMouseButtonDown(0)){

					Vector3 mousePosInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

					Vector3 targetPos = new Vector3((int)(mousePosInWorld.x + 0.5f),(int)(mousePosInWorld.y + 0.5f),0);

					Vector3 endPos = targetPos + new Vector3(0,0,15);

					RaycastHit2D r2d = Physics2D.Linecast(targetPos,endPos,battlePlayer.blockingLayer);

					if(r2d.transform != null){
						
						Debug.Log(r2d.transform.gameObject);

						if(battlePlayer != null){

							pathPosList = navHelper.FindPath(battlePlayer.transform.position,targetPos,battleMap.mapWalkableInfoArray);

							if(pathPosList.Count == 0){
								Debug.Log("something wrong");
							}else{

								for(int i = 0;i<pathPosList.Count;i++){

									Debug.Log(pathPosList[i]);

									Instantiate(cube,pathPosList[i],Quaternion.identity);

								}

							}
						}


					}

				}


			}
			#elif UNITY_ANDROID || UNITY_IOS
			{
				if (Input.touchCount != 0) {

					Touch t = Input.GetTouch (0);

					Vector3 touchPos = t.position;

					Vector3 startPos = Camera.main.ScreenToWorldPoint(new Vector3((int)touchPos.x,(int)touchPos.y,0));

					Vector3 endPos = startPos + new Vector3(0,0,15);

					RaycastHit2D r2d = Physics2D.Linecast(startPos,endPos,battlePlayer.blockingLayer);

					if(r2d.transform != null){

						Debug.Log(r2d.transform.gameObject);

						if(battlePlayer != null){

							battlePlayer.transform.position = r2d.transform.position;

						}
					}
				}
			}
			#endif
		}


		private void test(){



		}

		//Initializes the game for each level.
		public void SetupExploreView(int chapterIndex)
		{

			battleMap = TransformManager.FindTransform ("BattleMap").GetComponent<MapGenerator>();

			battlePlayer = TransformManager.FindTransform ("BattlePlayer").GetComponent<BattlePlayerController> ();

			//Call the HideLevelImage function with a delay in seconds of levelStartDelay.
			Invoke("HideLevelImage", TransitionDelay);
			
			//Clear any Enemy objects in our List to prepare for next level.
			battleMonsters.Clear();

			ChapterDetailInfo chapterDetail = DataInitializer.LoadDataToModelWithPath<ChapterDetailInfo> (CommonData.jsonFileDirectoryPath, CommonData.chapterDataFileName)[chapterIndex];
			
			//Call the SetupScene function of the BoardManager script, pass it current level number.
			battleMap.SetUpMap(chapterDetail);
			
		}
		
		
		//Hides black image used between levels
		private void HideLevelImage()
		{
			maskImage.enabled = false;
		}

	}
}

