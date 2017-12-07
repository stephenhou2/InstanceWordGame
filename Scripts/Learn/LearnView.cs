using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;
	using DG.Tweening;

	public class LearnView : MonoBehaviour {

		// 遮罩
		public Transform mask;

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
		public Transform choicesContainer;

		// 单词答案选项按钮数组
		public Transform[] choices;

		// 单词释义淡出时间
		public float explainationFadeOutDuration;

		private Transform currentSelectChoice;

		public Transform energySliderContainer;
		// 单词能量条
		public Image energyFill;

		public Text characterFragment;

		private Tweener characterFragmentAnim;


		public void SetUpLearnViewWithWord(LearnWord word){

			questionText.text = word.spell;

			phoneticSymbolText.text = word.phoneticSymbol;

			explainationText.text = word.explaination;

			explainationText.enabled = false;

			ShowContainers (true, false, false,false);

		}



		public void SetUpLearningProgress(){

			explainationText.enabled = true;
			explainationText.color = new Color (1, 1, 1, 1);

			ShowContainers (false, false, false,false);

			mask.gameObject.SetActive (true);

			StartCoroutine ("ExplainationShowAndHideAnim");

		}


		private IEnumerator ExplainationShowAndHideAnim(){

			yield return new WaitForSeconds (2.5f);

			explainationText.DOColor (new Color (1, 1, 1, 0), explainationFadeOutDuration).OnComplete (delegate {
				wordsLearnOperationButtonsContainer.gameObject.SetActive (true);
				explainationText.enabled = false;
				explainationText.color = new Color(1,1,1,1);
			});

			mask.gameObject.SetActive (false);
		}

		public void ShowExplaination(){
			explainationText.enabled = true;
		}
			

		public void SetUpLearnViewWithLearnExam(Examination exam){

			explainationText.enabled = false;

			questionText.text = exam.question.spell;

			questionText.fontSize = 100;

			for (int i = 0; i < choices.Length; i++) {

				Transform choice = choices [i];

				Button choiceButton = choice.Find("ChoiceButton").GetComponent<Button>();
				Transform accordAnswer = choice.Find ("AccordAnswer");

				LearnWord answer = exam.answers [i];

				choiceButton.GetComponentInChildren<Text>().text = answer.explaination;
				accordAnswer.GetComponentInChildren<Text>().text = answer.spell;

				Debug.Log (accordAnswer.localPosition);

				accordAnswer.localPosition = Vector3.zero;

				Debug.Log (accordAnswer.localPosition);

				choiceButton.onClick.RemoveAllListeners ();

				int currentSelectChoiceIndex = i;

				choiceButton.onClick.AddListener (delegate {
					currentSelectChoice = choices[currentSelectChoiceIndex];
					GetComponent<LearnViewController>().OnAnswerChoiceButtonOfLearnExamsClick(answer);
				});

			}

			ShowContainers (false, false, true,false);

		}

		public void SetUpLearnViewWithFinalExam(Examination exam,Examination.ExaminationType examType){

			switch (examType) {

			case Examination.ExaminationType.EngToChn:

				questionText.text = exam.question.spell;

				questionText.fontSize = 100;

				phoneticSymbolText.enabled = true;

				for (int i = 0; i < choices.Length; i++) {

					Transform choice = choices [i];

					Button choiceButton = choice.Find("ChoiceButton").GetComponent<Button>();
					Transform accordAnswer = choice.Find ("AccordAnswer");

					LearnWord answer = exam.answers [i];

					choiceButton.GetComponentInChildren<Text>().text = answer.explaination;
					accordAnswer.GetComponentInChildren<Text>().text = answer.spell;

					accordAnswer.localPosition = Vector3.zero;

					choiceButton.onClick.RemoveAllListeners ();

					int currentSelectChoiceIndex = i;

					choiceButton.onClick.AddListener (delegate {
						currentSelectChoice = choices[currentSelectChoiceIndex];
						GetComponent<LearnViewController>().OnAnswerChoiceButtonOfFinalExamsClick(answer);
					});

				}
				break;
			case Examination.ExaminationType.ChnToEng:

				questionText.text = exam.question.explaination;

				questionText.fontSize = 60;

				phoneticSymbolText.enabled = false;

				for (int i = 0; i < choices.Length; i++) {
					
					Transform choice = choices [i];

					Button choiceButton = choices [i].Find("ChoiceButton").GetComponent<Button>();
					Transform accordAnswer = choice.Find ("AccordAnswer");

					LearnWord answer = exam.answers [i];

					choiceButton.GetComponentInChildren<Text>().text = answer.spell;
					accordAnswer.GetComponentInChildren<Text>().text = answer.explaination;

					accordAnswer.localPosition = Vector3.zero;

					choiceButton.onClick.RemoveAllListeners ();

					int currentSelectChoiceIndex = i;

					choiceButton.onClick.AddListener (delegate {
						currentSelectChoice = choices[currentSelectChoiceIndex];
						GetComponent<LearnViewController>().OnAnswerChoiceButtonOfFinalExamsClick(answer);
					});

				}
				break;
			}

			ShowContainers (false, false, true,true);
		}

		public void ShowContainers(bool graspCondition,bool wordsLearnOperation,bool answers,bool energySlider){

			graspConditionButtonsContainer.gameObject.SetActive (graspCondition);
			wordsLearnOperationButtonsContainer.gameObject.SetActive (wordsLearnOperation);
			choicesContainer.gameObject.SetActive (answers);
			energySliderContainer.gameObject.SetActive (energySlider);

		}

		public void ShowAccordAnswerOfCurrentSelectedChoice(){
			currentSelectChoice.Find("AccordAnswer").DOLocalMoveY (55, 1.0f);
		}

		public void ResetEnergySlider(char character){

			characterFragment.text = character.ToString ();

		}

		public void UpdateWordEnergySlider(int currentEnergyCount,int energyFullCount){
			float fillAmount = (float)currentEnergyCount / energyFullCount;;
			energyFill.fillAmount = fillAmount;
			if (fillAmount == 1f) {
				StartCoroutine ("ResetWordEnergySliderAfterDelay", 0.5f);
			}
		}

		private IEnumerator ResetWordEnergySliderAfterDelay(float delay){

			yield return new WaitForSeconds (delay);

			energyFill.fillAmount = 0;

		}

	}
}
