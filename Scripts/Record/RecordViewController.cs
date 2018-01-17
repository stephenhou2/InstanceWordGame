﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class RecordViewController : MonoBehaviour {


		public RecordView recordView;

		// 单词学习信息
//		private LearningInfo learnInfo;




		/// <summary>
		/// 初始化学习记录界面
		/// </summary>
		public void SetUpRecordView(){
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
			

		public void OnLearnedWordButtonClick(){
			recordView.SetUpAllLearnedWords ();
		}


		public void OnWrongWordsButtonClick(){
			recordView.SetUpAllUngraspedWords ();
		}

		public void OnQuitWordsPlaneButtonClick(){
			recordView.QuitWordsPlane ();
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

			GameManager.Instance.UIManager.DestroryCanvasWith (CommonData.recordCanvasBundleName, "RecordCanvas","PoolContainerOfRecordCanvas","ModelContainerOfRecordCanvas");

		}



	}
}
