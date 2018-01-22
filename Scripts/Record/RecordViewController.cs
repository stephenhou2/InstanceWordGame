using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class RecordViewController : MonoBehaviour {


		public RecordView recordView;

		private int currentTabIndex;


		/// <summary>
		/// 初始化学习记录界面
		/// </summary>
		public void SetUpRecordView(){
			currentTabIndex = 0;
			recordView.SetUpRecordView (LearningInfo.Instance);
//			SoundManager.Instance.PlayAudioClip ("UI/sfx_UI_Click");
//			StartCoroutine ("SetUpViewAfterDataReady");
		}

//		private IEnumerator SetUpViewAfterDataReady(){
//
//			bool dataReady = false;
//
//			while (!dataReady) {
//
//				dataReady = GameManager.Instance.gameDataCenter.CheckDatasReady (new GameDataCenter.GameDataType[] {
//					GameDataCenter.GameDataType.LearnInfo,
//				});
//
//				yield return null;
//
//			}
//
//			recordView.SetUpRecordView (LearningInfo.Instance);
//
//
//		}
			

		public void OnGeneralRecordButtonClick(){
			if (currentTabIndex == 0) {
				return;
			}
			currentTabIndex = 0;
			recordView.SetUpGeneralLearningInfo ();
		}


		public void OnWrongWordsButtonClick(){
			if (currentTabIndex == 1) {
				return;
			}
			currentTabIndex = 1;
			recordView.SetUpAllUngraspedWords ();
		}

		/// <summary>
		/// 退出学习记录界面
		/// </summary>
		public void QuitRecordPlane(){

			GameManager.Instance.pronounceManager.ClearPronunciationCache ();

			recordView.OnQuitRecordPlane ();

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.homeCanvasBundleName, "HomeCanvas", () => {
				TransformManager.FindTransform("HomeCanvas").GetComponent<HomeViewController>().SetUpHomeView();
			});

			TransformManager.DestroyTransfromWithName ("PoolContainerOfRecordCanvas", TransformRoot.PoolContainer);

//			GameManager.Instance.gameDataCenter.ReleaseDataWithDataTypes (new GameDataCenter.GameDataType[]{ 
//				GameDataCenter.GameDataType.LearnInfo
//			});

		}

		/// <summary>
		/// 清理内存
		/// </summary>
		public void DestroyInstances(){

//			learnInfo = null;

			GameManager.Instance.UIManager.RemoveCanvasCache ("RecordCanvas");

			Destroy (this.gameObject);

			MyResourceManager.Instance.UnloadAssetBundle (CommonData.recordCanvasBundleName, true);

		}



	}
}
