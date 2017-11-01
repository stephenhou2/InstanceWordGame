using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class RecordViewController : MonoBehaviour {


		public RecordView recordView;

		// 单词学习信息
		private LearningInfo learnInfo;

		// 当前选择的选项卡序号
		private int currentSelectTitleIndex = -1;

		/// <summary>
		/// 初始化学习记录界面
		/// </summary>
		public void SetUpRecordView(){

			learnInfo = GameManager.Instance.dataCenter.learnInfo;

			recordView.SetUpRecordView ();

			OnTitleButtonClick (0);

			GetComponent<Canvas>().enabled = true; 
		}

		/// <summary>
		/// 选择选项卡按钮后的响应方法
		/// </summary>
		/// <param name="index">选项卡序号.</param>
		public void OnTitleButtonClick(int index){

			if (currentSelectTitleIndex == index) {
				return;
			}

			currentSelectTitleIndex = index;

			recordView.OnSelectTitleButton (index);

			switch (index) {
			case 0:
				OnGeneralInfoButtonClick (); // 选择了学习记录选项卡
				break;
			case 1:
				OnLearnedButtonClick (); // 选择了已学习选项卡
				break;
			case 2:
				OnUnlearnedButtonClick (); // 选择了未学习选项卡
				break;
			}

		}

		/// <summary>
		/// 选择学习记录选项卡的响应方法
		/// </summary>
		private void OnGeneralInfoButtonClick(){

			recordView.OnGeneralInfoButtonClick (learnInfo);
		}


		/// <summary>
		/// 选择以学习选项卡的响应方法
		/// </summary>
		private void OnLearnedButtonClick(){

			recordView.OnQuitWordsRecordPlane ();

			recordView.OnLearnedButtonClick (learnInfo);
		}

		/// <summary>
		///  选择未学习选项卡的响应方法
		/// </summary>
		private void OnUnlearnedButtonClick(){

			recordView.OnQuitWordsRecordPlane ();
			
			recordView.OnUnlearnedButtonClick (learnInfo);
		}


		/// <summary>
		/// 退出学习记录界面
		/// </summary>
		public void QuitRecordPlane(){

			recordView.OnQuitRecordPlane ();

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.homeCanvasBundleName, "HomeCanvas", () => {
				TransformManager.FindTransform("HomeCanvas").GetComponent<HomeViewController>().SetUpHomeView();
			});

			TransformManager.DestroyTransfromWithName ("PoolContainerOfRecordCanvas", TransformRoot.PoolContainer);

			GameManager.Instance.dataCenter.ReleaseDataWithNames (new string[]{ "LearnInfo" });

		}

		/// <summary>
		/// 清理内存
		/// </summary>
		public void DestroyInstances(){

			learnInfo = null;

			GameManager.Instance.UIManager.DestroryCanvasWith (CommonData.recordCanvasBundleName, "RecordCanvas","PoolContainerOfRecordCanvas","ModelContainerOfRecordCanvas");

		}



	}
}
