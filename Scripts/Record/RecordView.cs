using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace WordJourney
{
	public class RecordView : MonoBehaviour {

		public Text wordType;

		public Transform wordsPlane;



		public Image completionImage;

		public Text completionPercentage;

		public Text learnedWordsCount;

		public Text unGraspedWordsCount;

		// 选项卡选中图片
//		private Sprite typeBtnNormalSprite;
//		// 选项卡未选中图片
//		private Sprite typeBtnSelectedSprite;

		// 单词cell模型
		private Transform wordModel;

		// 复用缓存池
		private InstancePool wordPool;

		public Transform wordContainer;

		private LearningInfo learnInfo;


		public void SetUpRecordView(LearningInfo learnInfo){

			this.learnInfo = learnInfo;

			Transform poolContainerOfRecordCanvas = TransformManager.FindOrCreateTransform (CommonData.poolContainerName + "/PoolContainerOfRecordCanvas");
			Transform modelContainerOfRecordCanvas = TransformManager.FindOrCreateTransform (CommonData.instanceContainerName + "/ModelContainerOfRecordCanvas");

			if (poolContainerOfRecordCanvas.childCount == 0) {
				// 创建缓存池
				wordPool = InstancePool.GetOrCreateInstancePool ("WordItemPool",poolContainerOfRecordCanvas.name);
			}

			if (modelContainerOfRecordCanvas.childCount == 0) {
				// 获得单词展示模型
				wordModel = TransformManager.FindTransform ("WordModel");
				wordModel.SetParent (modelContainerOfRecordCanvas);
			}

			SetUpGeneralLearningInfo ();

			GetComponent<Canvas>().enabled = true;

		}

		/// <summary>
		/// 初始化学习记录页
		/// </summary>
		/// <param name="learnInfo">Learn info.</param>
		public void SetUpGeneralLearningInfo(){

			wordsPlane.gameObject.SetActive (false);

			string wordTypeStr = null;

			switch (learnInfo.currentWordType) {
			case WordType.CET4:
				wordTypeStr = "四级核心词汇";
				break;
			case WordType.CET6:
				wordTypeStr = "六级核心词汇";
				break;
			case WordType.Bussiness:
				wordTypeStr = "商务英语词汇";
				break;
			case WordType.Daily:
				wordTypeStr = "日常英语词汇";
				break;
			}

			wordType.text = wordTypeStr;

			float percentage = 0;

			if (learnInfo.totalWordCount != 0) {
				percentage = learnInfo.learnedWordCount / learnInfo.totalWordCount;
			}
			 

			completionImage.fillAmount = percentage;

			completionPercentage.text = ((int)(percentage * 100)).ToString() + "%";

			learnedWordsCount.text = learnInfo.learnedWordCount.ToString ();

			unGraspedWordsCount.text = learnInfo.ungraspedWordCount.ToString ();

			GetComponent<Canvas> ().enabled = true;

		}

		/// <summary>
		/// 初始化已学习页
		/// </summary>
		/// <param name="learnInfo">Learn info.</param>
		public void SetUpAllLearnedWords(){

			wordsPlane.gameObject.SetActive (true);

			List<LearnWord> allLearnedWords = learnInfo.GetAllLearnedWords ();

			for (int i = 0; i < allLearnedWords.Count; i++) {

				LearnWord word = allLearnedWords [i];

				Transform wordItem = wordPool.GetInstance <Transform> (wordModel.gameObject, wordContainer);

				wordItem.GetComponent<WordItemView> ().SetUpCellDetailView (word);

			}

		}

		/// <summary>
		/// 初始化未学习页
		/// </summary>
		/// <param name="learnInfo">Learn info.</param>
		public void SetUpAllUngraspedWords(){

			wordsPlane.gameObject.SetActive (true);

			List<LearnWord> allUngraspedWords = learnInfo.GetAllUngraspedWords ();


			for (int i = 0; i < allUngraspedWords.Count; i++) {

				LearnWord word = allUngraspedWords [i];

				Transform wordItem = wordPool.GetInstance <Transform> (wordModel.gameObject, wordContainer);

				wordItem.GetComponent<WordItemView> ().SetUpCellDetailView (word);

			}
				
		}

		public void QuitWordsPlane(){

			wordsPlane.gameObject.SetActive (false);

			wordPool.AddChildInstancesToPool (wordContainer);

		}



		/// <summary>
		/// 退出整个单词记录几面
		/// </summary>
		/// <param name="cb">Cb.</param>
		public void OnQuitRecordPlane(){

			wordModel = null;

			wordPool = null;

			GetComponent<Canvas> ().enabled = false;

		}



	}
}
