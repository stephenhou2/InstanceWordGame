using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.EventSystems;


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

		void Awake()
		{
//			TransitionDelay = 0.5f;
//			currentMapIndex = 0;

			//Assign enemies to a new List of Enemy objects.
			battleMonsters = new List<BattleMonsterController>();

			navHelper = GetComponent<NavigationHelper> ();


		}
			

		 

		private void Update(){


			Vector3 clickPos = Vector3.zero;

#if UNITY_STANDALONE || UNITY_EDITOR

			if(Input.GetMouseButtonDown(0)){

				if(EventSystem.current.IsPointerOverGameObject()){
					Debug.Log("点击在UI上");
					return;
				}

				clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			}

#elif UNITY_ANDROID || UNITY_IOS
			if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended){

				if(EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)){
						Debug.Log("点击在UI上");
						return;
					}

				clickPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
			}
#endif
			// 未检测到点击位置
			//（初始设置点击位置在世界坐标原点，如果检测到点击点并初始化点击点之后，z一定会和camera的z保持一致，为－10，如果为0表示没有重设点击点）
			if (clickPos.z == 0) {
				return;
			}

			// 点击位置在地图有效区之外，直接返回
			if(clickPos.x + 0.5f >= battleMap.rows 
				|| clickPos.y + 0.5f >= battleMap.columns 
				|| clickPos.x + 0.5f < 0 
				|| clickPos.y + 0.5f < 0){
				Debug.Log ("点击在地图有效区外部");
				return;
			}


			// 由于地图贴图 tile时是以中心点为参考，宽高为1，所以如果以实际拼出的地图左下角为坐标原点，则点击位置需要进行如下坐标转换
			int targetX = (int)(clickPos.x + 0.5f);

			int targetY = (int)(clickPos.y + 0.5f);

			// 以地图左下角为坐标原点时的点击位置
			Vector3 targetPos = new Vector3(targetX, targetY, 0);

			Vector3 rayEndPos = targetPos + new Vector3(0,0,15);

			// 检测点击到的碰撞体
			RaycastHit2D r2d = Physics2D.Linecast(targetPos,rayEndPos,battlePlayer.blockingLayer);


			if(r2d.transform != null){

				if(battlePlayer != null){

					// 计算自动寻路路径
					pathPosList = navHelper.FindPath(battlePlayer.predicatePos,targetPos,battleMap.mapWalkableInfoArray);

				}
			}else{
				// 点击位置没有有效碰撞体（不在地图有效范围内），则清空寻路路径
				pathPosList.Clear();
			}

			// 地图上点击位置生成提示动画
			battleMap.PlayDestinationAnim(targetPos,pathPosList.Count > 0);

			// 游戏角色按照自动寻路路径移动到点击位置
			battlePlayer.MoveToEndByPath (pathPosList, targetPos);

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

