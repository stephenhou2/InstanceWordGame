using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ExploreMainView: MonoBehaviour {



	private GameObject chapterEvent;

	private GameObject chapterEventsContainer;


	private Text playerLevelText;
	private Text stepsLeftText;
	private Text chapterLocationText;
	private Slider playerHealthBar;

	private Image eventIcon;
	private Text eventTitle;
	private Text eventDescription;
	private Image eventConfirmIcon;


	private Sprite[] sprites;

	public int maxEventCountForOnce = 4;


	private ChapterDetailInfo detailInfo;

	public void SetUpExploreMainView(ChapterDetailInfo detailInfo){

		this.detailInfo = detailInfo;


		LoadAssets ();




	}

	private void LoadAssets(){
		CallBack callBack = LoadEventSprites;

		ResourceManager.Instance.LoadAssetWithName ("explore",callBack);

	}

	private void LoadEventSprites(){
		CallBack callBack = SetUpScene;

		ResourceManager.Instance.LoadAssetWithName ("event/confirm_icons",callBack,true);
	}
		

	public void SetUpScene(){

		InitUI ();

//		SetUpTopBar ();

		SetUpChapterEvents ();
	}

	private void InitUI(){

		playerLevelText = GameObject.Find ("PlayerLevelText").GetComponent<Text>();
		stepsLeftText = GameObject.Find ("StepsLeftText").GetComponent<Text>();
		chapterLocationText = GameObject.Find ("ChapterLocationText").GetComponent<Text>();
		playerHealthBar = GameObject.Find ("PlayerHealth").GetComponent<Slider>();

		chapterEvent = GameObject.Find ("ChapterEvent");
		chapterEventsContainer = GameObject.Find ("ChapterEventsContainer");


	}



	private void SetUpChapterEvents(){
		sprites = new Sprite[10];
		Debug.Log (ResourceManager.Instance.sprites.Count);
		ResourceManager.Instance.sprites.CopyTo(sprites);

		for (int i = 0; i < maxEventCountForOnce; i++) {
			
			GameObject mChapterEvent = Instantiate (chapterEvent, chapterEventsContainer.transform);

//			Image eventIcon = mChapterEvent.transform.FindChild ("EventIcon").GetComponent<Image>();
//			Text eventTitle = mChapterEvent.transform.FindChild ("EventTitle").GetComponent<Text>();
//			Text eventDescription = mChapterEvent.transform.FindChild ("EventDescription").GetComponent<Text>();
//			Image eventConfirmIcon = mChapterEvent.transform.FindChild ("EventConfirmIcon").GetComponent<Image>();

			Image eventIcon = GameObject.Find ("EventIcon").GetComponent<Image>();
			Text eventTitle = GameObject.Find ("EventTitle").GetComponent<Text>();
			Text eventDescription = GameObject.Find ("EventDescription").GetComponent<Text>();
			Image eventConfirmIcon = GameObject.Find ("EventConfirmIcon").GetComponent<Image>();


//			eventTitle.text = npc.npcName;
//			eventDescription.text = npc.npcDescription;
			eventIcon.sprite = sprites[0];
			eventConfirmIcon.sprite = sprites[1];

//			switch (RandomEvent ()) {
//			case EventType.Monster:
//				Monster[] monsters = RandomReturn<Monster[]> (detailInfo.monstersGroup);
//				break;
//			case EventType.NPC:
//				NPC npc = RandomReturn<NPC> (detailInfo.npcs);
//				eventTitle.text = npc.npcName;
//				eventDescription.text = npc.npcDescription;
//				eventIcon.sprite = GameObject.Find ("people1") as Sprite;
//				eventConfirmIcon.sprite = GameObject.Find("chatIcon") as Sprite;
//				break;
//			case EventType.Item:
//				Item item = RandomReturn<Item> (detailInfo.items);
//				break;
//			default:
//				break;
//			}
		}
	}

	private EventType RandomEvent(){
		float i = 0f;
		i = Random.Range (0f, 10f);
		if (i >= 0f && i < 5f) {
			return EventType.Monster;
		} else if (i >= 5f && i < 7.5f) {
			return EventType.NPC;
		} else {
			return EventType.Item;
		}
	}

	private T RandomReturn<T>(T[] array){
		int randomIndex = (int)(Random.Range (0, array.Length - float.Epsilon));
		return array[randomIndex];
	}


	private void SetUpTopBar(){


		playerLevelText.text = Player.mainPlayer.playerLevel.ToString();
		stepsLeftText.text = detailInfo.totalSteps.ToString();
		chapterLocationText.text = detailInfo.chapterLocation;
		playerHealthBar.maxValue = Player.mainPlayer.maxHealth;
		playerHealthBar.value = Player.mainPlayer.health;
		playerHealthBar.transform.FindChild ("HealthText").GetComponent<Text> ().text = Player.mainPlayer.health + "/" + Player.mainPlayer.maxHealth;

	}

	public void SetUpChapterEventsPlane(ChapterDetailInfo detailInfo){

	}


}
