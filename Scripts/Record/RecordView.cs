﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace WordJourney
{
	public class RecordView : MonoBehaviour {


		public Transform generalInfoPlane;
		public Transform wordsPlane;


		public Button recordTitle;
		public Button wrongWordsTitle;


		public Text wordType;

		public Image completionImage;

		public Text completionPercentage;

		public Text learnedWordsCount;

		public Text unGraspedWordsCount;

		// 单词cell模型
		public Transform wordModel;

		// 复用缓存池
		private InstancePool wordPool;

		public Transform wordContainer;

		private LearningInfo learnInfo;


		/// <summary>
		/// 初始化记录页面
		/// </summary>
		/// <param name="learnInfo">Learn info.</param>
		/// <param name="tabIndex">选项卡序号 【0:基本信息 1:错误单词】.</param>
		public void SetUpRecordView(LearningInfo learnInfo){

			this.learnInfo = learnInfo;

			Transform poolContainerOfRecordCanvas = TransformManager.FindOrCreateTransform (CommonData.poolContainerName + "/PoolContainerOfRecordCanvas");

			if (poolContainerOfRecordCanvas.childCount == 0) {
				// 创建缓存池
				wordPool = InstancePool.GetOrCreateInstancePool ("WordItemPool",poolContainerOfRecordCanvas.name);
			}


			SetUpGeneralLearningInfo ();

			GetComponent<Canvas>().enabled = true;

		}

		/// <summary>
		/// 初始化学习记录页
		/// </summary>
		/// <param name="learnInfo">Learn info.</param>
		public void SetUpGeneralLearningInfo(){

			string wordTypeStr = GameManager.Instance.gameDataCenter.gameSettings.GetWordTypeString ();

			wordType.text = wordTypeStr;

			float percentage = 0;

			if (learnInfo.totalWordCount != 0) {
				percentage = learnInfo.learnedWordCount / learnInfo.totalWordCount;
			}
			 

			completionImage.fillAmount = percentage;

			completionPercentage.text = ((int)(percentage * 100)).ToString() + "%";

			learnedWordsCount.text = learnInfo.learnedWordCount.ToString ();

			unGraspedWordsCount.text = learnInfo.ungraspedWordCount.ToString ();

			generalInfoPlane.gameObject.SetActive (true);
			wordsPlane.gameObject.SetActive (false);

			recordTitle.Select ();

			recordTitle.GetComponentInChildren<Text>().color = new Color (
				CommonData.selectedColor.x, 
				CommonData.selectedColor.y, 
				CommonData.selectedColor.z);
			wrongWordsTitle.GetComponentInChildren<Text>().color = new Color (
				CommonData.deselectedColor.x, 
				CommonData.deselectedColor.y, 
				CommonData.deselectedColor.z);

			GetComponent<Canvas> ().enabled = true;

		}

		/// <summary>
		/// 初始化已学习页
		/// </summary>
		/// <param name="learnInfo">Learn info.</param>
//		public void SetUpAllLearnedWords(){
//
//			wordsPlane.gameObject.SetActive (true);
//
//			List<LearnWord> allLearnedWords = learnInfo.GetAllLearnedWords ();
//
//			for (int i = 0; i < allLearnedWords.Count; i++) {
//
//				LearnWord word = allLearnedWords [i];
//
//				Transform wordItem = wordPool.GetInstance <Transform> (wordModel.gameObject, wordContainer);
//
//				wordItem.GetComponent<WordItemView> ().SetUpCellDetailView (word);
//
//			}
//
//		}

		/// <summary>
		/// 初始化未学习页
		/// </summary>
		/// <param name="learnInfo">Learn info.</param>
		public void SetUpAllUngraspedWords(){
			
			List<LearnWord> allUngraspedWords = learnInfo.GetAllUngraspedWords ();
		
			wordPool.AddChildInstancesToPool (wordContainer);

			for (int i = 0; i < allUngraspedWords.Count; i++) {

				LearnWord word = allUngraspedWords [i];

				Transform wordItem = wordPool.GetInstance <Transform> (wordModel.gameObject, wordContainer);

				wordItem.GetComponent<WordItemView> ().SetUpCellDetailView (word);

			}

			generalInfoPlane.gameObject.SetActive (false);
			wordsPlane.gameObject.SetActive (true);

			wrongWordsTitle.Select ();

			wrongWordsTitle.GetComponentInChildren<Text>().color = new Color (
				CommonData.selectedColor.x, 
				CommonData.selectedColor.y, 
				CommonData.selectedColor.z);
			recordTitle.GetComponentInChildren<Text>().color = new Color (
				CommonData.deselectedColor.x, 
				CommonData.deselectedColor.y, 
				CommonData.deselectedColor.z);

			GetComponent<Canvas> ().enabled = true;
				
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
