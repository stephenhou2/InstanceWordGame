using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RecordView : MonoBehaviour {


	public Text wordType;

	public Transform recordPlane;

	public Transform generalRecordPlane;
	public Transform wordsRecordPlane;

	public Transform wordItemsContainer;

	public Button[] titleButtons;

	public Image completionImage;

	public Text completionPercentage;

	public Text learnedTime;

	public Text learnedCount;

	public Text unlearnedCount;

	public Sprite buttonNormalSprite;
	public Sprite buttonSelectedSprite;

	public GameObject wordItemModel;

	private InstancePool wordItemPool;


	public void SetUpRecordView(){

		wordItemPool = InstancePool.GetOrCreateInstancePool ("WordItemPool");

	}

	public void OnGeneralInfoButtonClick(LearningInfo learnInfo){

		generalRecordPlane.gameObject.SetActive (true);

		wordsRecordPlane.gameObject.SetActive (false);

		for (int i = 0; i < titleButtons.Length; i++) {

			titleButtons [i].GetComponent<Image> ().sprite = (i == 0 ? buttonSelectedSprite : buttonNormalSprite);

		}

		string wordTypeStr = string.Empty;

		switch (learnInfo.wordType) {
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

		float percentage = learnInfo.learnedWordCount / learnInfo.totalWordCount;

		completionImage.fillAmount = percentage;

		completionPercentage.text = ((int)(percentage * 100)).ToString() + "%";

		learnedTime.text = learnInfo.learnTime.ToString();

		learnedCount.text = learnInfo.learnedWordCount.ToString ();

		unlearnedCount.text = (learnInfo.totalWordCount - learnInfo.learnedWordCount).ToString ();

	}

	public void OnLearnedButtonClick(LearningInfo learnInfo){
		
		generalRecordPlane.gameObject.SetActive (false);

		wordsRecordPlane.gameObject.SetActive (true);

		for (int i = 0; i < titleButtons.Length; i++) {

			titleButtons [i].GetComponent<Image> ().sprite = (i == 1 ? buttonSelectedSprite : buttonNormalSprite);

		}
			

		for (int i = 0; i < learnInfo.learnedWordCount; i++) {

			Word w = learnInfo.learnedWords [i];

			Transform wordItem = wordItemPool.GetInstance <Transform> (wordItemModel, wordsRecordPlane);

			Text word = wordItem.FindChild ("Word").GetComponent<Text>();

			Text explaination = wordItem.FindChild ("Explaination").GetComponent<Text> ();

			word.text = w.spell;

			explaination.text = w.example;

		}



	}

	public void OnUnlearnedButtonClick(LearningInfo learnInfo){
		
		generalRecordPlane.gameObject.SetActive (false);

		wordsRecordPlane.gameObject.SetActive (true);

		for (int i = 0; i < titleButtons.Length; i++) {

			titleButtons [i].GetComponent<Image> ().sprite = (i == 2 ? buttonSelectedSprite : buttonNormalSprite);

		}

		int unlearnedWordsCount = learnInfo.totalWordCount - learnInfo.learnedWordCount;

		for (int i = 0; i < unlearnedWordsCount; i++) {

			Word w = learnInfo.unlearnedWords [i];

			Transform wordItem = wordItemPool.GetInstance <Transform> (wordItemModel, wordItemsContainer);

			Text word = wordItem.FindChild ("Word").gameObject.GetComponent<Text>();

			Text explaination = wordItem.FindChild ("Explaination").GetComponent<Text> ();

			word.text = w.spell;

			explaination.text = w.explaination;

		}



	}


	public void OnSelectTitleButton(int index){

		for (int i = 0; i < titleButtons.Length; i++) {
			Button titleButton = titleButtons [i];
			titleButton.GetComponent<Image> ().sprite = (i == index ? buttonSelectedSprite : buttonNormalSprite);

		}


	}

	public void OnQuitWordsRecordPlane(){

		wordItemPool.AddChildInstancesToPool (wordItemsContainer);

	}

	public void OnQuitRecordPlane(CallBack cb){

		recordPlane.DOLocalMoveY (-Screen.height, 0.5f).OnComplete(()=>{
			cb();
		});


	}



}
