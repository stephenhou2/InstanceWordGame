using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class LearnViewController : MonoBehaviour {

		// 单词学习view
		public LearnView learnView;

		// 一次学习的单词数量（单个水晶学习单词数量）
		public int singleLearnWordsCount;

		// 下载发音的超时时长
		public float wwwTimeOutInterval;

		// 本次所有需要记忆的单词数组
		private LearnWord[] wordsToLearnArray;

		// 未掌握的单词列表
		private List<LearnWord> ungraspedWordsList;

		// 当前正在学习的单词（未掌握单词列表的首项）
		private LearnWord currentLearningWord{
			get{
				return ungraspedWordsList [0];
			}
		}

		// 单词测试列表
		private List<Examination> wordsExaminationsList;

		// 当前正在进行的单词测试（单词测试列表的首项）
		private Examination currentExamination{
			get{
				return wordsExaminationsList [0];
			}
		}

		// 当前正在学习的单词的发音
		private AudioClip pronunciationOfCurrentWord;

		void Awake(){
			wordsToLearnArray = new LearnWord[singleLearnWordsCount];
			wordsExaminationsList = new List<Examination> ();
			ungraspedWordsList = new List<LearnWord> ();
		}


		public void SetUpLearnView(){
			InitWordsToLearn ();
			learnView.SetUpLearnViewWithWord(currentLearningWord);
		}

		/// <summary>
		/// 初始化本次要学习的单词数组
		/// </summary>
		private void InitWordsToLearn(){

			MySQLiteHelper mySql = MySQLiteHelper.Instance;

			mySql.GetConnectionWith (CommonData.dataBaseName);

			mySql.BeginTransaction ();

//			mySql.ReadSpecificRowsAndColsOfTable(CommonData.CET4Table,"learnedTimes",


			mySql.EndTransaction ();


			mySql.CloseConnection (CommonData.dataBaseName);

			for (int i = 0; i < wordsToLearnArray.Length; i++) {
				ungraspedWordsList.Add (wordsToLearnArray [i]);
			}

		}

		/// <summary>
		/// 用户点击了发音按钮
		/// </summary>
		public void OnPronunciationButtonClick(){

			string firstLetter = currentLearningWord.spell.Substring (0, 1);

			string url = string.Format ("https://wordsound.b0.upaiyun.com/voice/{0}/{1}.wav", firstLetter, currentLearningWord.spell);

			WWW www = new WWW (url);

			StartCoroutine ("PlayPronunciationWhenFinishDownloading", www);
		}

		/// <summary>
		/// 下载读音文件并在下载完成后播放单词读音的协程
		/// </summary>
		/// <returns>The pronunciation when finish downloading.</returns>
		/// <param name="www">Www.</param>
		private IEnumerator PlayPronunciationWhenFinishDownloading(WWW www){

			AudioSource pronunciationAS = GameManager.Instance.soundManager.pronunciationAS;

			float timer = 0;

			while (!www.isDone && timer < wwwTimeOutInterval) {
				timer += Time.deltaTime;
				yield return null;
			}


			if (www.isDone) {

				AudioClip pronunciation = WWWAudioExtensions.GetAudioClip (www);

				pronunciationAS.clip = pronunciation;

				pronunciationAS.Play ();
			} else {
				// 下载超时时不播放读音,并关闭下载任务
				www.Dispose ();
			}

		}

		/// <summary>
		/// 用户点击了已掌握按钮
		/// </summary>
		public void OnHaveGraspedButtonClick(){

			// 使用当前学习中的单词（在这时已掌握）生成对应的单词测试
			Examination exam = new Examination (currentLearningWord, wordsToLearnArray, Examination.ExaminationType.EngToChn);

			// 单词测试加入到测试列表中
			wordsExaminationsList.Add (exam);

			// 将当前学习中的单词从未掌握单词列表中删除
			ungraspedWordsList.RemoveAt (0);

			// 如果还有未掌握单词，则继续向用户查询后续单词的掌握情况
			if (ungraspedWordsList.Count > 0) {
				learnView.SetUpLearnViewWithWord (currentLearningWord);
				return;
			}

			// 如果本次需要记忆的单词已全部掌握，则进入单词测试
			learnView.SetUpLearnViewWithExamination (currentExamination);

		}

		/// <summary>
		/// 用户点击了未掌握按钮
		/// </summary>
		public void OnHaveNotGraspedButtonClick(){
			// 开始单词学习过程
			learnView.SetUpLearningProgress ();

		}

		/// <summary>
		/// 用户点击了不熟悉按钮
		/// </summary>
		public void OnUnfamiliarButtonClick(){

			// 将该单词移至未掌握单词列表的尾部
			LearnWord unfamiliarWord = currentLearningWord;

			ungraspedWordsList.RemoveAt (0);

			ungraspedWordsList.Add (unfamiliarWord);

		}
			

		/// <summary>
		/// 用户点击了测试界面中的答案选项卡
		/// </summary>
		/// <param name="selectWord">Select word.</param>
		public void OnAnswerChoiceButtonClick(LearnWord selectWord){

			// 如果选择正确，则将该单词的测试从测试列表中移除
			if (selectWord.wordId == currentExamination.question.wordId) {
				Debug.Log ("选择正确");
				wordsExaminationsList.RemoveAt(0);
				learnView.SetUpLearnViewWithExamination (currentExamination);
			} else {
				// 如果选择错误，则将该单词的测试移至测试列表的尾部
				Debug.Log ("选择错误");
				Examination exam = currentExamination;
				wordsExaminationsList.RemoveAt (0);
				wordsExaminationsList.Add (exam);
			}

		}

		public void OnQuitLearnView(){

		}
		
	}


	public class Examination{

		public enum ExaminationType
		{
			EngToChn,
			ChnToEng
		}

		public LearnWord question;
		public LearnWord[] answers;
		public int correctAnswerIndex;
		public ExaminationType examType;

		/// <summary>
		/// 初始化测试数据
		/// </summary>
		/// <param name="question">Question.</param>
		/// <param name="answers">Answers.</param>
		/// <param name="correctAnswerIndex">Correct answer index.</param>
		/// <param name="examType">Exam type.</param>
		public Examination(LearnWord questionWord, LearnWord[] wordsToLearnArray, ExaminationType examType){

			this.question = questionWord;

			answers = new LearnWord[3];

			List<int> indexList = new List<int>{ 0, 1, 2 };

			int questionWordIndex = Random.Range (0, indexList.Count);

			answers [questionWordIndex] = questionWord;

			indexList.Remove (questionWordIndex);

			LearnWord confuseWord1 = GetConfuseWordFromArray (wordsToLearnArray, new LearnWord[]{ questionWord });

			int confuseWord1Index = indexList [Random.Range (0, indexList.Count)];

			answers [confuseWord1Index] = confuseWord1;

			indexList.Remove (confuseWord1Index);

			int confuseWord2Index = indexList [Random.Range (0, indexList.Count)];

			LearnWord confuseWord2 = GetConfuseWordFromArray (wordsToLearnArray, new LearnWord[]{ questionWord, confuseWord1 });

			answers [confuseWord2Index] = confuseWord2;

			this.correctAnswerIndex = questionWordIndex;

			this.examType = examType;
		}


		private LearnWord GetConfuseWordFromArray(LearnWord[] wordsToLearnArray,LearnWord[] existWords){

			LearnWord learnWord = null;

			int randomWordId = Random.Range (0, wordsToLearnArray.Length);

			learnWord = wordsToLearnArray [randomWordId];

			for (int i = 0; i < existWords.Length; i++) {
				if (learnWord.wordId == existWords [i].wordId) {
					return GetConfuseWordFromArray (wordsToLearnArray, existWords);
				}
			}

			return learnWord;

		}


	}

}
