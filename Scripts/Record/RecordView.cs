using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RecordView : MonoBehaviour {


	public Text wordType;

	public Transform recordViewContainer;

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

	// 选项卡选中图片
	private Sprite typeBtnNormalSprite;
	// 选项卡未选中图片
	private Sprite typeBtnSelectedSprite;

	// 单词cell模型
	public GameObject wordItemModel;

	// 复用缓存池
	private InstancePool wordItemPool;


	public void SetUpRecordView(){

		wordItemPool = InstancePool.GetOrCreateInstancePool ("WordItemPool");

		typeBtnNormalSprite = GameManager.Instance.allUIIcons.Find (delegate(Sprite obj) {
			return obj.name == "typeButtonNormal";
		});

		typeBtnSelectedSprite = GameManager.Instance.allUIIcons.Find (delegate(Sprite obj) {
			return obj.name == "typeButtonSelected";
		});

	}

	/// <summary>
	/// 初始化学习记录页
	/// </summary>
	/// <param name="learnInfo">Learn info.</param>
	public void OnGeneralInfoButtonClick(LearningInfo learnInfo){

		generalRecordPlane.gameObject.SetActive (true);

		wordsRecordPlane.gameObject.SetActive (false);

		for (int i = 0; i < titleButtons.Length; i++) {

			titleButtons [i].GetComponent<Image> ().sprite = (i == 0 ? typeBtnSelectedSprite : typeBtnNormalSprite);

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

	/// <summary>
	/// 初始化已学习页
	/// </summary>
	/// <param name="learnInfo">Learn info.</param>
	public void OnLearnedButtonClick(LearningInfo learnInfo){


		
		generalRecordPlane.gameObject.SetActive (false);

		wordsRecordPlane.gameObject.SetActive (true);

		for (int i = 0; i < titleButtons.Length; i++) {

			titleButtons [i].GetComponent<Image> ().sprite = (i == 1 ? typeBtnSelectedSprite : typeBtnNormalSprite);

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

	/// <summary>
	/// 初始化未学习页
	/// </summary>
	/// <param name="learnInfo">Learn info.</param>
	public void OnUnlearnedButtonClick(LearningInfo learnInfo){
		
		generalRecordPlane.gameObject.SetActive (false);

		wordsRecordPlane.gameObject.SetActive (true);

		for (int i = 0; i < titleButtons.Length; i++) {

			titleButtons [i].GetComponent<Image> ().sprite = (i == 2 ? typeBtnSelectedSprite : typeBtnNormalSprite);

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


	/// <summary>
	/// 选择选项卡后更新选项卡图片
	/// </summary>
	/// <param name="index">Index.</param>
	public void OnSelectTitleButton(int index){

		for (int i = 0; i < titleButtons.Length; i++) {
			Button titleButton = titleButtons [i];
			titleButton.GetComponent<Image> ().sprite = (i == index ? typeBtnSelectedSprite : typeBtnNormalSprite);

		}


	}

	/// <summary>
	/// 退出已学习／未学习页时将cell放入缓存池
	/// </summary>
	public void OnQuitWordsRecordPlane(){

		wordItemPool.AddChildInstancesToPool (wordItemsContainer);

	}

	/// <summary>
	/// 退出整个单词记录几面
	/// </summary>
	/// <param name="cb">Cb.</param>
	public void OnQuitRecordPlane(CallBack cb){

		recordViewContainer.GetComponent<Image> ().color = new Color (0, 0, 0, 0);

		recordPlane.DOLocalMoveY (-Screen.height, 0.5f).OnComplete(()=>{
			cb();
		});


	}



}
