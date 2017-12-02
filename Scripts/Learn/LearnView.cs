using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;
	using DG.Tweening;

	public class LearnView : MonoBehaviour {

		// 考察的单词时的英文单词文本 或者 考察从中文释义到英文单词时的释义文本
		public Text questionText;

		// 音标文本
		public Text phoneticSymbolText;

		// 考察单词时对应的中文释义文本
		public Text explainationText;

		// 自查单词掌握情况的按钮容器
		public Transform graspConditionButtonsContainer;

		// 学习单词时学习操作按钮容器
		public Transform wordsLearnOperationButtonsContainer;

		// 单词问题的答案按钮容器
		public Transform answerButtonsContainer;

		// 单词答案选项按钮数组
		public Button[] answerButtons;

		// 单词释义淡出时间
		public float explainationFadeOutDuration;



		public void SetUpLearnViewWithWord(LearnWord word){

			questionText.text = word.spell;

			phoneticSymbolText.text = word.phoneticSymbol;

			explainationText.text = word.explaination;

			explainationText.enabled = false;

			ShowButtonsContainers (true, false, false);



		}



		public void SetUpLearningProgress(){

			explainationText.enabled = true;
			explainationText.color = new Color (1, 1, 1, 1);

			ShowButtonsContainers (false, false, false);

			StartCoroutine ("ExplainationShowAndHideAnim");

		}


		private IEnumerator ExplainationShowAndHideAnim(){

			yield return new WaitForSeconds (2.5f);

			explainationText.DOColor (new Color (1, 1, 1, 0), explainationFadeOutDuration).OnComplete (delegate {
				wordsLearnOperationButtonsContainer.gameObject.SetActive (true);
				explainationText.enabled = false;
				explainationText.color = new Color(1,1,1,1);
			});

		}

		public void ShowExplaination(){
			explainationText.enabled = true;
		}
			

		public void SetUpLearnViewWithExamination(Examination exam){

			switch (exam.examType) {

			case Examination.ExaminationType.EngToChn:

				questionText.text = exam.question.spell;

				for (int i = 0; i < answerButtons.Length; i++) {

					Button answerButton = answerButtons [i];

					LearnWord answer = exam.answers [i];

					answerButton.transform.Find ("AnswerText").GetComponent<Text> ().text = answer.explaination;

					answerButton.onClick.RemoveAllListeners ();

					answerButton.onClick.AddListener (delegate {
						GetComponent<LearnViewController>().OnAnswerChoiceButtonClick(answer);
					});

				}
				break;
			case Examination.ExaminationType.ChnToEng:

				questionText.text = exam.question.explaination;

				for (int i = 0; i < answerButtons.Length; i++) {

					Button answerButton = answerButtons [i];

					LearnWord answer = exam.answers [i];

					answerButton.transform.Find ("AnswerText").GetComponent<Text> ().text = answer.spell;

					answerButton.onClick.RemoveAllListeners ();

					answerButton.onClick.AddListener (delegate {
						GetComponent<LearnViewController>().OnAnswerChoiceButtonClick(answer);
					});

				}
				break;
			}

			ShowButtonsContainers (false, false, true);
		}

		public void ShowButtonsContainers(bool graspCondition,bool wordsLearnOperation,bool answers){

			graspConditionButtonsContainer.gameObject.SetActive (graspCondition);
			wordsLearnOperationButtonsContainer.gameObject.SetActive (wordsLearnOperation);
			answerButtonsContainer.gameObject.SetActive (answers);

		}

		public void QuitLearnView(){

		}


	}
}
