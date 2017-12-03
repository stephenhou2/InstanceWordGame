using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using System.Data;

	public class LearnViewController : MonoBehaviour {

		private class Pronunciation
		{
			public LearnWord word;
			public AudioClip pronunciation;

			public Pronunciation(LearnWord word,AudioClip pronunciation){
				this.word = word;
				this.pronunciation = pronunciation;
			}
		}

		// 单词学习view
		public LearnView learnView;

		// 一次学习的单词数量（单个水晶学习单词数量）
		private int singleLearnWordsCount;

		// 背多少组循环一次
		private int recycleGroupBase;

		// 背诵多少次进行一次大循环
		private int recycleLearnTimeBase;


		/*背诵总次数	 0		 1		 2		 3
		 * 			【0】	【0】	【0】	【0】
		 * 
		 *背诵总次数 	 4		 5		 6		 7		
		 * 			【1】	【1】	【1】	【1】
		 * 
		 *背诵总次数 	 8		 9	  	 10		 11		
		 * 			【0】	【0】	【0】	【0】
		 * 
		 *背诵总次数 	 12		 13		 14		 15		
		 * 			【1】	【1】	【1】	【1】-----到这里8个单词刚好都背完两遍
		 * 
		 *背诵总次数 	 16		 17		 18		 19	
		 * 			【2】	【2】	【2】	【2】-----重新开始循环
		 * 
		 *背诵总次数 	 20		 21		 22		 23	
		 * 			【3】	【3】	【3】	【3】
		 * 
		 * 			 ............
		 *			 ............
		 *
		 * 带【】的数字表示当前使用的是背诵过几次的单词
		 * 上例中假设一共有8个单词，则是以4组为循环基数，以2次为背诵次数循环基数
		 */ 

		// 测试时选择正确的字母奖励数
		private int singleRewardCharactersCount;
		// 测试时选择错误的字母损失数
		private int singleLoseCharactersCount;

		// 下载发音的超时时长
		public float wwwTimeOutInterval;

		// 当前应该学习的单词组的学习次数
		private int currentWordsLearnedTime;

		// 当前应该学习的单词组的首个单词id
		private int firstIdOfCurrentLearningWords;

		// 本次所有需要记忆的单词数组
		private LearnWord[] wordsToLearnArray;

		// 未掌握的单词列表
		private List<LearnWord> ungraspedWordsList;

		// 当前正在学习的单词（未掌握单词列表的首项）
		private LearnWord currentLearningWord{
			get{
				if (ungraspedWordsList.Count > 0) {
					return ungraspedWordsList [0];
				} else if (wordsExaminationsList.Count > 0) {
					return wordsExaminationsList [0].question;
				} else {
					return null;
				}
			}
		}

		// 单词测试列表
		private List<Examination> wordsExaminationsList;

		// 当前正在进行的单词测试（单词测试列表的首项）
		private Examination currentExamination{
			get{
				if (wordsExaminationsList.Count > 0) {
					return wordsExaminationsList [0];
				} else {
					return null;
				}
			}
		}

		// 当前正在学习的单词的发音
		private AudioClip pronunciationOfCurrentWord;

		// 读音缓存
		private List<Pronunciation> pronunciationCache;

		// 是否自动发音
		private bool autoPronounce;

		void Awake(){
			singleLearnWordsCount = 9;
			singleRewardCharactersCount = 1;
			singleLoseCharactersCount = 2;
			recycleGroupBase = 8;
			recycleLearnTimeBase = 2;
			wordsToLearnArray = new LearnWord[singleLearnWordsCount];
			wordsExaminationsList = new List<Examination> ();
			ungraspedWordsList = new List<LearnWord> ();
			pronunciationCache = new List<Pronunciation> ();
		}

		/// <summary>
		/// 初始化学习界面
		/// </summary>
		public void SetUpLearnView(){

			// 查询是否允许自动发音
			autoPronounce = GameManager.Instance.gameDataCenter.gameSettings.autoPronounce;

			InitWordsToLearn ();

			learnView.SetUpLearnViewWithWord(currentLearningWord);

			if (autoPronounce) {
				OnPronunciationButtonClick ();
			}
		}

		/// <summary>
		/// 初始化本次要学习的单词数组
		/// </summary>
		private void InitWordsToLearn(){

			MySQLiteHelper mySql = MySQLiteHelper.Instance;

			mySql.GetConnectionWith (CommonData.dataBaseName);

			int totalLearnTimeCount = GameManager.Instance.gameDataCenter.learnInfo.totalLearnTimeCount;

			int totalWordsCount = mySql.GetItemCountOfTable (CommonData.CET4Table);

			// 大循环的次数
			int bigCycleCount = totalLearnTimeCount * singleLearnWordsCount / (totalWordsCount * recycleLearnTimeBase);

			currentWordsLearnedTime = totalLearnTimeCount % (recycleLearnTimeBase * recycleGroupBase) / recycleGroupBase + recycleLearnTimeBase * bigCycleCount;



			mySql.BeginTransaction ();

			// 边界条件
			string condition = string.Format ("learnedTimes={0}",currentWordsLearnedTime);

			IDataReader reader = mySql.ReadSpecificRowsOfTable (CommonData.CET4Table, null, new string[]{ condition }, true);

			int flag = 0;

			// 从数据库中读取当前要学习的单词
			for(int i = 0;i<singleLearnWordsCount;i++){

				reader.Read ();
				
				int wordId = reader.GetInt32 (0);

				string spell = reader.GetString (1);

				string phoneticSymble = reader.GetString (2);

				string explaination = reader.GetString (3);

				string example = reader.GetString (4);

				int learnedTimes = reader.GetInt16 (5);

				LearnWord word = new LearnWord (wordId, spell, phoneticSymble, explaination, example);

				Debug.LogFormat ("{0}---{1}次",word,learnedTimes);

				wordsToLearnArray [flag] = word;

				flag++;
			}

			mySql.EndTransaction ();

			mySql.CloseConnection (CommonData.dataBaseName);

			// 当前要学习的单词全部加入到未掌握单词列表中，用户选择掌握或者学习过该单词后从未掌握单词列表中移除
			for (int i = 0; i < wordsToLearnArray.Length; i++) {
				ungraspedWordsList.Add (wordsToLearnArray [i]);
			}

			firstIdOfCurrentLearningWords = wordsToLearnArray [0].wordId;

		}

		/// <summary>
		/// 用户点击了发音按钮
		/// </summary>
		public void OnPronunciationButtonClick(){

			LearnWord word = currentLearningWord;

			if (word == null) {
				return;
			}

			Pronunciation pro = pronunciationCache.Find (delegate(Pronunciation obj) {
				return obj.word.wordId == word.wordId;
			});

			if (pro == null) {
				
				string firstLetter = word.spell.Substring (0, 1);

				string url = string.Format ("https://wordsound.b0.upaiyun.com/voice/{0}/{1}.wav", firstLetter, word.spell);

				WWW www = new WWW (url);

				StartCoroutine ("PlayPronunciationWhenFinishDownloading", www);
			} else {
				
				AudioSource pronunciationAS = GameManager.Instance.soundManager.pronunciationAS;

				pronunciationAS.clip = pro.pronunciation;

				pronunciationAS.Play ();
			}
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

				AudioClip pronunciationClip = WWWAudioExtensions.GetAudioClip (www);

				Pronunciation pro = new Pronunciation (currentLearningWord, pronunciationClip);

				pronunciationCache.Add (pro);

				pronunciationAS.clip = pronunciationClip;

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
			Examination exam = new Examination (currentLearningWord, wordsToLearnArray);

			// 单词测试加入到测试列表中
			wordsExaminationsList.Add (exam);

			// 将当前学习中的单词从未掌握单词列表中删除
			ungraspedWordsList.RemoveAt (0);

			// 如果还有未掌握单词，则继续向用户查询后续单词的掌握情况
			if (ungraspedWordsList.Count > 0) {
				learnView.SetUpLearnViewWithWord (currentLearningWord);
				if (autoPronounce) {
					OnPronunciationButtonClick ();
				}
				return;
			}

			// 如果本次需要记忆的单词已全部掌握，则进入单词测试
			Examination.ExaminationType examType = currentExamination.GetCurrentExamType();
			learnView.SetUpLearnViewWithExamination (currentExamination,examType);

		}

		/// <summary>
		/// 用户点击了未掌握按钮
		/// </summary>
		public void OnHaveNotGraspedButtonClick(){
			// 开始单词学习过程
			learnView.SetUpLearningProgress ();

		}

		/// <summary>
		/// 显示单词释义
		/// </summary>
		public void OnShowExplainationButtonClick(){
			learnView.ShowExplaination ();
		}

		/// <summary>
		/// 用户点击了不熟悉按钮
		/// </summary>
		public void OnUnfamiliarButtonClick(){

			// 将该单词移至未掌握单词列表的尾部
			LearnWord unfamiliarWord = currentLearningWord;

			ungraspedWordsList.RemoveAt (0);

			ungraspedWordsList.Add (unfamiliarWord);

			learnView.SetUpLearnViewWithWord (currentLearningWord);

			if (autoPronounce) {
				OnPronunciationButtonClick ();
			}

		}
			

		/// <summary>
		/// 用户点击了测试界面中的答案选项卡
		/// </summary>
		/// <param name="selectWord">Select word.</param>
		public void OnAnswerChoiceButtonClick(LearnWord selectWord){

			// 如果选择正确，则将该单词的测试从测试列表中移除
			if (selectWord.wordId == currentExamination.question.wordId) {
				Debug.Log ("选择正确");

				// 奖励字母碎片
				int characterIndex = Random.Range (0, currentExamination.question.spell.Length);
				char characterFragmentGain = currentExamination.question.spell.ToCharArray () [characterIndex];
				Player.mainPlayer.AddCharacterFragment(characterFragmentGain,singleRewardCharactersCount);
				Debug.LogFormat ("获得字母碎片{0}{1}个", characterFragmentGain, singleRewardCharactersCount);

				currentExamination.RemoveCurrentExamType ();

				// 如果当前单词测试的中译英和英译中都已经完成，则从测试列表中删除该测试
				bool currentExamFinished = currentExamination.CheckCurrentExamFinished();
				if (currentExamFinished) {
					wordsExaminationsList.RemoveAt (0);
				}else{
					// 当前单词测试未完成
					Examination exam = currentExamination;
					wordsExaminationsList.RemoveAt (0);
					wordsExaminationsList.Add (exam);
				}

			} else {
				// 如果选择错误，则将该单词的测试移至测试列表的尾部
				Debug.Log ("选择错误");
				Examination exam = currentExamination;
				wordsExaminationsList.RemoveAt (0);
				wordsExaminationsList.Add (exam);

				// 扣除用户字母碎片
				int characterIndex = Random.Range (0, currentExamination.question.spell.Length);
				char characterFragmentGain = currentExamination.question.spell.ToCharArray () [characterIndex];
				Player.mainPlayer.RemoveCharacterFragment(characterFragmentGain,singleRewardCharactersCount);
				Debug.LogFormat ("扣除字母碎片{0}{1}个", characterFragmentGain, singleLoseCharactersCount);


			}

			// 单词测试环节结束
			if (wordsExaminationsList.Count <= 0) {

				MySQLiteHelper sql = MySQLiteHelper.Instance;

				sql.GetConnectionWith (CommonData.dataBaseName);

				// 当前单词的背诵次数+1（转为字符串，用于写入数据库）
				string newLearnedTime = (++currentWordsLearnedTime).ToString();

				// 边界条件
				string condition1 = string.Format ("wordId>={0}",firstIdOfCurrentLearningWords);
				string condition2 = string.Format ("wordId<{0}", firstIdOfCurrentLearningWords + 9);

				// 更新数据库中当前背诵单词的背诵次数
				sql.UpdateValues (CommonData.CET4Table,new string[]{"learnedTimes"}, new string[]{ newLearnedTime }, new string[]{ condition1, condition2 }, true);

				sql.CloseConnection (CommonData.dataBaseName);

				CurrentWordsLearningFinished ();

				return;

			}
			Examination.ExaminationType examType = currentExamination.GetCurrentExamType();
			learnView.SetUpLearnViewWithExamination (currentExamination,examType);

		}

		/// <summary>
		/// 当前需要学习的单词组内的单词已经全部学习完毕
		/// </summary>
		private void CurrentWordsLearningFinished(){

			// 清理内存
			for (int i = 0; i < singleLearnWordsCount; i++) {
				wordsToLearnArray [i] = null;
			}

			ungraspedWordsList.Clear ();

			wordsExaminationsList.Clear ();

			// 总背诵次数++
			GameManager.Instance.gameDataCenter.learnInfo.totalLearnTimeCount++;

			// 退出单词学习界面
			learnView.QuitLearnView ();

			TransformManager.FindTransform ("ExploreManager").GetComponent<ExploreManager> ().FinishLearning ();

		}


		
	}

	/// <summary>
	/// 单词测试类
	/// </summary>
	public class Examination{

		// 测试类型
		public enum ExaminationType
		{
			EngToChn,//给拼写，选释义
			ChnToEng//给释义，选拼写
		}
		// 测试题目（测试的单词）
		public LearnWord question;
		// 题目备选答案（测试单词+2个混淆单词）
		public LearnWord[] answers;
		// 正确答案在答案中的序号
		public int correctAnswerIndex;
		// 当前测试的测试类型
		private ExaminationType currentExamType;
		// 测试类型列表（中-英&英-中）
		private List<ExaminationType> examTypeList = new List<ExaminationType>(){ExaminationType.EngToChn,ExaminationType.ChnToEng};

		private LearnWord[] allCurrentLearningWords;

		public ExaminationType GetCurrentExamType(){

			int examTypeIndex = Random.Range (0, examTypeList.Count);

			currentExamType = examTypeList [examTypeIndex];

			return currentExamType;

		}

		/// <summary>
		/// 查询当前单词的测试是否完全完成（中-英和英-中两种测试已经全部答对）
		/// </summary>
		/// <returns><c>true</c>, if current exam finished was checked, <c>false</c> otherwise.</returns>
		public bool CheckCurrentExamFinished(){
			return examTypeList.Count <= 0;
		}

		/// <summary>
		/// 从单词测试类型列表中移除测试单词的当前测试类型
		/// </summary>
		public void RemoveCurrentExamType(){
			examTypeList.Remove (currentExamType);
			//如果还有测试未完成，则重新生成备选答案
			if (examTypeList.Count > 0) {
				RandomAnswersFromLearningWords ();
			}
		}

		/// <summary>
		/// 从当前学习中的所有单词列表中生成备选答案（当前学习的单词+2个混淆单词）
		/// </summary>
		private void RandomAnswersFromLearningWords(){

			answers = new LearnWord[3];

			List<int> indexList = new List<int>{ 0, 1, 2 };

			int questionWordIndex = Random.Range (0, indexList.Count);

			answers [questionWordIndex] = question;

			indexList.Remove (questionWordIndex);

			LearnWord confuseWord1 = GetConfuseWordFromArray (allCurrentLearningWords, new LearnWord[]{ question });

			int confuseWord1Index = indexList [Random.Range (0, indexList.Count)];

			answers [confuseWord1Index] = confuseWord1;

			indexList.Remove (confuseWord1Index);

			int confuseWord2Index = indexList [Random.Range (0, indexList.Count)];

			LearnWord confuseWord2 = GetConfuseWordFromArray (allCurrentLearningWords, new LearnWord[]{ question, confuseWord1 });

			answers [confuseWord2Index] = confuseWord2;

			this.correctAnswerIndex = questionWordIndex;

		}


		/// <summary>
		/// 初始化测试数据
		/// </summary>
		/// <param name="question">Question.</param>
		/// <param name="answers">Answers.</param>
		/// <param name="correctAnswerIndex">Correct answer index.</param>
		/// <param name="examType">Exam type.</param>
		public Examination(LearnWord questionWord, LearnWord[] wordsToLearnArray){

			this.question = questionWord;
			this.allCurrentLearningWords = wordsToLearnArray;

			RandomAnswersFromLearningWords ();

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
