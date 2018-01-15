using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using System.Data;


	public class LearnViewController : MonoBehaviour {

		// 单词学习view
		public LearnView learnView;

		// 是否从学习过程开始
		public bool beginWithLearn;

		// 背错的单词是否加到队列尾部继续学习
		private bool addToTrailIfWrong;

		// 一次学习的单词数量（单个水晶学习单词数量）
		private int singleLearnWordsCount;


		//***********不再按照记忆曲线对已测试过的单词进行复习,以下代码注释掉**************//

		// 背多少组循环一次
//		private int recycleGroupBase;

		// 背诵多少次进行一次大循环
//		private int recycleLearnTimeBase;

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

		//***********不再按照记忆曲线对已测试过的单词进行复习,以上代码注释掉**************//



		// 本次所有需要记忆的单词数组
		private LearnWord[] wordsToLearnArray;

		// 未掌握的单词列表
		private List<LearnWord> ungraspedWordsList;

		private List<LearnWord> graspedWordsList;

		private int learnedWordCount;

		private int correctWordCount;

		private int coinGain;

//		private bool hasFinishWholeCurrentTypeWords;


		private MySQLiteHelper mySql;

		private string currentWordsTableName;


		private Examination.ExaminationType examType;

		// 当前正在学习的单词（未掌握单词列表的首项）
		private LearnWord currentLearningWord{
			get{
				if (beginWithLearn && ungraspedWordsList.Count > 0) {
					return ungraspedWordsList [0];
				} else if (currentExamination != null) {
					return currentExamination.question;
				} else {
					return null;
				}
			}
		}

		// 单词测试列表
		private List<Examination> finalExaminationsList ;

		private List<Examination> learnExaminationsList;

		// 当前正在进行的单词测试（单词测试列表的首项）
		private Examination currentExamination{
			get{
				if (learnExaminationsList.Count > 0) {
					return learnExaminationsList [0];
				} else if (finalExaminationsList.Count > 0) {
					return finalExaminationsList [0];
				} else {
					return null;
				}
			}
		}
			
		// 是否自动发音
		private bool autoPronounce;



		void Awake(){
			singleLearnWordsCount = 10;
//			recycleGroupBase = 8;
//			recycleLearnTimeBase = 2;
			wordsToLearnArray = new LearnWord[singleLearnWordsCount];
			finalExaminationsList = new List<Examination> ();
			learnExaminationsList = new List<Examination> ();
			ungraspedWordsList = new List<LearnWord> ();
			graspedWordsList = new List<LearnWord> ();

		}

		/// <summary>
		/// 初始化学习界面
		/// </summary>
		public void SetUpLearnView(){
//			SoundManager.Instance.PlayAudioClip ("UI/sfx_UI_Click");
			currentWordsTableName = GameManager.Instance.gameDataCenter.learnInfo.GetCurrentLearningWordsTabelName();
			Time.timeScale = 0;
			SoundManager.Instance.PauseBgm ();
			StartCoroutine ("SetUpViewAfterDataReady");
		}


		private IEnumerator SetUpViewAfterDataReady(){

			bool dataReady = false;

			while (!dataReady) {

				dataReady = GameManager.Instance.gameDataCenter.CheckDatasReady (new GameDataCenter.GameDataType[] {
					GameDataCenter.GameDataType.UISprites,
				});
				yield return null;
			}


//			hasFinishWholeCurrentTypeWords = false;

			// 查询是否允许自动发音
			autoPronounce = GameManager.Instance.gameDataCenter.gameSettings.autoPronounce;

			GameSettings.LearnMode learnMode = GameManager.Instance.gameDataCenter.gameSettings.learnMode;

			switch (learnMode) {
			case GameSettings.LearnMode.Test:
				#warning 这里暂时只做英->中
				this.examType = Examination.ExaminationType.EngToChn;
				addToTrailIfWrong = false;
				beginWithLearn = false;
				break;
			case GameSettings.LearnMode.Learn:
				this.examType = Examination.ExaminationType.Both;
				addToTrailIfWrong = true;
				beginWithLearn = true;
				break;
			}

			learnedWordCount = 0;
			coinGain = 0;
			correctWordCount = 0;

			InitWordsToLearn ();

			learnView.InitLearnView (wordsToLearnArray.Length);

			if (beginWithLearn) {
				learnView.SetUpLearnViewWithWord (currentLearningWord);
				if (autoPronounce) {
					OnPronunciationButtonClick ();
				}
			} else {
				GenerateFinalExams (examType);
				learnView.SetUpLearnViewWithFinalExam (finalExaminationsList [0], examType);
			}

			GetComponent<Canvas> ().enabled = true;
		}

		/// <summary>
		/// 初始化本次要学习的单词数组
		/// </summary>
		private void InitWordsToLearn(){

			ungraspedWordsList.Clear ();

			mySql = MySQLiteHelper.Instance;

			mySql.GetConnectionWith (CommonData.dataBaseName);

//			int totalLearnTimeCount = GameManager.Instance.gameDataCenter.learnInfo.totalLearnTimeCount;
//
//			int totalWordsCount = mySql.GetItemCountOfTable (CommonData.CET4Table,null,true);
//
//			// 大循环的次数
//			int bigCycleCount = totalLearnTimeCount * singleLearnWordsCount / (totalWordsCount * recycleLearnTimeBase);
//
//			currentWordsLearnedTime = totalLearnTimeCount % (recycleLearnTimeBase * recycleGroupBase) / recycleGroupBase + recycleLearnTimeBase * bigCycleCount;

			mySql.BeginTransaction ();

			// 边界条件
			string[] condition = new string[]{ "learnedTimes=0" };


			IDataReader reader = mySql.ReadSpecificRowsOfTable (currentWordsTableName, null, condition, true);


			// 从数据库中读取当前要学习的单词
			for(int i = 0;i<singleLearnWordsCount;i++){

				reader.Read ();

				if (reader == null) {

					string[] colFields = new string[]{ "learnedTimes" };
					string[] values = new string[]{ "0" };
					string[] conditions = new string[]{"learnedTimes=1"};

					mySql.UpdateValues (currentWordsTableName, colFields, values, conditions, true);

					mySql.EndTransaction ();

					mySql.CloseAllConnections ();

					InitWordsToLearn ();

					return;
				}
				
				int wordId = reader.GetInt32 (0);

				string spell = reader.GetString (1);

				string phoneticSymble = reader.GetString (2);

				string explaination = reader.GetString (3);

				string example = reader.GetString (4);

				int learnedTimes = reader.GetInt16 (5);

				int ungraspTimes = reader.GetInt16 (6);

				LearnWord word = new LearnWord (wordId, spell, phoneticSymble, explaination, example, learnedTimes, ungraspTimes);

				Debug.LogFormat ("{0}---{1}次",word,learnedTimes);

				wordsToLearnArray [i] = word;

			}

			mySql.EndTransaction ();

			mySql.CloseAllConnections ();

			// 当前要学习的单词全部加入到未掌握单词列表中，用户选择掌握或者学习过该单词后从未掌握单词列表中移除
			for (int i = 0; i < wordsToLearnArray.Length; i++) {
				ungraspedWordsList.Add (wordsToLearnArray [i]);
			}

//			firstIdOfCurrentLearningWords = wordsToLearnArray [0].wordId;

		}

		/// <summary>
		/// 直接从本次学习单词生成最终测试列表
		/// </summary>
		private void GenerateFinalExams(Examination.ExaminationType examType){

			for (int i = 0; i < ungraspedWordsList.Count; i++) {

				LearnWord word = ungraspedWordsList [i];

				Examination finalExam = new Examination (word, wordsToLearnArray, examType);

				finalExaminationsList.Add (finalExam);

			}

		}


		/// <summary>
		/// 用户点击了发音按钮
		/// </summary>
		public void OnPronunciationButtonClick(){

			LearnWord word = currentLearningWord;

			if (word == null) {
				return;
			}

			GameManager.Instance.pronounceManager.PronounceWord (word);

		}



		/// <summary>
		/// 用户点击了已掌握按钮
		/// </summary>
		public void OnHaveGraspedButtonClick(){

			GameManager.Instance.pronounceManager.CancelPronounce ();

			// 使用当前学习中的单词（在这时已掌握）生成对应的单词测试
			Examination exam = new Examination (currentLearningWord, wordsToLearnArray,examType);

			// 单词测试加入到最终测试列表中
			finalExaminationsList.Add (exam);

			// 将当前学习中的单词从未掌握单词列表中删除
			LearnWord word = ungraspedWordsList[0];
			ungraspedWordsList.RemoveAt (0);
			graspedWordsList.Add (word);

			// 如果已经有新的三个单词选择了已掌握
			if (graspedWordsList.Count % 3 == 0) {
				
				LearnWord[] wordsArray = new LearnWord[3];

				for (int i = 0; i < 3; i++) {
					int graspedWordsCount = graspedWordsList.Count;
					wordsArray[i] = graspedWordsList [graspedWordsCount - 3 + i];
				}

				for (int i = 0; i < 3; i++) {
					Examination learnExam = new Examination (wordsArray [i], wordsArray, examType);  
					learnExaminationsList.Add (learnExam); 
				}
					

				learnView.SetUpLearnViewWithLearnExam (learnExaminationsList [0]);

				return;

			}

			// 如果还有未掌握单词，则继续向用户查询后续单词的掌握情况
			if (ungraspedWordsList.Count > 0) {
				learnView.SetUpLearnViewWithWord (currentLearningWord);
				if (autoPronounce) {
					OnPronunciationButtonClick ();
				}
			}

//			// 如果本次需要记忆的单词已全部掌握，则进入单词测试
//			Examination.ExaminationType examType = currentExamination.GetCurrentExamType();
//			learnView.SetUpLearnViewWithFinalExam (currentExamination,examType);

		}

		/// <summary>
		/// 用户点击了未掌握按钮
		/// </summary>
		public void OnHaveNotGraspedButtonClick(){
			if (ungraspedWordsList.Count == 1) {
				OnShowExplainationButtonClick ();
				return;
			}
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

		private LearnWord GetWordFromWordsToLearnArrayWith(int wordId){

			for (int i = 0; i < wordsToLearnArray.Length; i++) {
				LearnWord word = wordsToLearnArray [i];
				if (word.wordId == wordId) {
					return word;
				}
			}

			return null;

		}
			
		public void OnAnswerChoiceButtonOfLearnExamsClick(LearnWord selectWord){

			// 如果选择正确，则将该单词的测试从测试列表中移除
			if (selectWord.wordId == currentExamination.question.wordId) {
				Debug.Log ("选择正确");

				learnExaminationsList.RemoveAt (0);

				// 本次学习测试还未完成
				if (learnExaminationsList.Count > 0) {
					learnView.SetUpLearnViewWithLearnExam (currentExamination);
				}
				// 整个学习过程结束，接下来应该进入最终测试环节
				else if (learnExaminationsList.Count <= 0 && ungraspedWordsList.Count <= 0) {

					// 随机字母
//					char characterFragmentGain = (char)(Random.Range (0, 26) + CommonData.aInASCII);
//
//					characterAsReward = characterFragmentGain;

//					learnView.ResetEnergySlider (characterAsReward);

					// 更新单词能量条
//					learnView.UpdateLearningProgress (wordEnergyCount,energyFullCount);

					Examination.ExaminationType examType = currentExamination.GetCurrentExamType();
					learnView.SetUpLearnViewWithFinalExam (currentExamination,examType);
				}
				// 本次学习测试结束，但是还有未学习的单词
				else {
					learnView.SetUpLearnViewWithWord (currentLearningWord);
				}
			} else {
				// 如果选择错误,则显示错误选项的意思，并更新该单词的背错次数
				Debug.Log ("选择错误");

				Examination exam = currentExamination;

//				learnExaminationsList.RemoveAt (0);
//
//				learnExaminationsList.Add (exam);

				learnView.ShowAccordAnswerOfCurrentSelectedChoice ();

				// 单词的背错次数+1
				GetWordFromWordsToLearnArrayWith(exam.question.wordId).ungraspTimes++;

			}

		}
			

		/// <summary>
		/// 用户点击了最终测试界面中的答案选项卡
		/// </summary>
		public void OnAnswerChoiceButtonOfFinalExamsClick(LearnWord selectWord){

			learnedWordCount++;

			learnView.UpdateLearningProgress (learnedWordCount, wordsToLearnArray.Length, true);

			// 如果选择正确，则将该单词的测试从测试列表中移除
			if (selectWord.wordId == currentExamination.question.wordId) {
				Debug.Log ("选择正确");

				SoundManager.Instance.PlayAudioClip ("UI/sfx_UI_RightTint");

				correctWordCount++;

				coinGain++;

				learnView.UpdateCrystalHarvest (coinGain);

				currentExamination.RemoveCurrentExamType ();

				// 如果当前单词测试的所有测试类型都已经完成（根据设置，测试类型有 英译中，英译中+中译英）都已经完成，则从测试列表中删除该测试
				bool currentExamFinished = currentExamination.CheckCurrentExamFinished();
				if (currentExamFinished) {
					currentExamination.question.learnedTimes++;
					finalExaminationsList.RemoveAt (0);
				}else{
					// 当前单词测试未完成
					Examination exam = currentExamination;
					finalExaminationsList.RemoveAt (0);
					finalExaminationsList.Add (exam);
				}
					
			} else {
				// 如果选择错误
				Debug.Log ("选择错误");

				SoundManager.Instance.PlayAudioClip ("UI/sfx_UI_WrongTint");

				// 单词的背错次数+1
				GetWordFromWordsToLearnArrayWith(currentExamination.question.wordId).ungraspTimes++;

				// 当前测试加入到测试列表尾部
				Examination exam = currentExamination;

				finalExaminationsList.RemoveAt (0);

				if (addToTrailIfWrong) {
					finalExaminationsList.Add (exam);
					coinGain--;
				}
					
				learnView.ShowRightAnswerAndEnterNextExam (exam.correctAnswerIndex ,currentExamination);
			}

			// 单词测试环节结束
			if (finalExaminationsList.Count <= 0) {

				for (int i = 0; i < singleLearnWordsCount; i++) {
					LearnWord word = wordsToLearnArray [i];
					string condition = string.Format ("wordId={0}", word.wordId);
					string newLearnedTime = (word.learnedTimes).ToString ();
					string newUngraspTime = (word.ungraspTimes).ToString ();

					mySql.GetConnectionWith (CommonData.dataBaseName);

					// 更新数据库中当前背诵单词的背诵次数和背错次数
					mySql.UpdateValues (currentWordsTableName, new string[]{ "learnedTimes", "ungraspTimes" }, new string[] {
						newLearnedTime,
						newUngraspTime
					}, new string[] {
						condition
					}, true);
				}

				CurrentWordsLearningFinished ();
			} else {
				// 测试环节还没有结束，则初始化下一个单词的测试
				Examination.ExaminationType examType = currentExamination.GetCurrentExamType ();
				learnView.SetUpLearnViewWithFinalExam (currentExamination, examType);
			}

		}


		/// <summary>
		/// 当前需要学习的单词组内的单词已经全部学习完毕
		/// </summary>
		private void CurrentWordsLearningFinished(){

			// 清理内存
			for (int i = 0; i < singleLearnWordsCount; i++) {
				wordsToLearnArray [i] = null;
			}

//			ungraspedWordsList.Clear ();
//
//			finalExaminationsList.Clear ();

			GameManager.Instance.pronounceManager.ClearPronunciationCache ();

			GameManager.Instance.persistDataManager.SaveLearnInfo ();


			Player.mainPlayer.totalCoins += coinGain;

			learnView.ShowFinishLearningHUD (coinGain,correctWordCount);


		}


		public void OnQuitButtonClick(){
			learnView.ShowQuitQueryHUD ();
		}

		public void OnCancelQuitButtonClick(){
			learnView.HideQuitQueryHUD ();
		}
			




		public void DestroyInstances(){
			GameManager.Instance.UIManager.DestroryCanvasWith (CommonData.learnCanvasBundleName, "LearnCanvas", null,null);
		}

		public void QuitLearnView(bool finishLearning){

			ungraspedWordsList.Clear ();

			finalExaminationsList.Clear ();

			Time.timeScale = 1f;

			mySql.CloseConnection (CommonData.dataBaseName);

			learnView.QuitLearnView ();

			Transform em = TransformManager.FindTransform ("ExploreManager");

			if (em != null) {
				GameManager.Instance.UIManager.HideCanvas ("LearnCanvas");
				SoundManager.Instance.ResumeBgm ();
				if (finishLearning) {
					ExploreManager exploreManager = em.GetComponent<ExploreManager> ();
					exploreManager.ChangeCrystalStatus ();
					exploreManager.expUICtr.UpdatePlayerStatusBar ();
				}
			} else {
				GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.homeCanvasBundleName, "HomeCanvas", () => {
					TransformManager.FindTransform("HomeCanvas").GetComponent<HomeViewController>().SetUpHomeView();
				});
			}

		}

	}

}
