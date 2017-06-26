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


	private List<Sprite> sprites = new List<Sprite>();

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

		ResourceManager.Instance.LoadAssetWithName ("event/icons",callBack,true);
	}
		

	public void SetUpScene(){

		InitUI ();

//		SetUpTopBar ();

		SetUpChapterEventsPlane ();
	}

	private void InitUI(){

		playerLevelText = GameObject.Find ("PlayerLevelText").GetComponent<Text>();
		stepsLeftText = GameObject.Find ("StepsLeftText").GetComponent<Text>();
		chapterLocationText = GameObject.Find ("ChapterLocationText").GetComponent<Text>();
		playerHealthBar = GameObject.Find ("PlayerHealth").GetComponent<Slider>();

		chapterEvent = GameObject.Find ("ChapterEvent");
		chapterEventsContainer = GameObject.Find ("ChapterEventsContainer");


	}


	// 初始化顶部bar
	private void SetUpTopBar(){

		playerLevelText.text = Player.mainPlayer.playerLevel.ToString();
		stepsLeftText.text = detailInfo.totalSteps.ToString();
		chapterLocationText.text = detailInfo.chapterLocation;
		playerHealthBar.maxValue = Player.mainPlayer.maxHealth;
		playerHealthBar.value = Player.mainPlayer.health;
		playerHealthBar.transform.FindChild ("HealthText").GetComponent<Text> ().text = Player.mainPlayer.health + "/" + Player.mainPlayer.maxHealth;

	}

	// 初始化事件面板
	public void SetUpChapterEventsPlane(){
		
		sprites = ResourceManager.Instance.sprites;

		for (int i = 0; i < maxEventCountForOnce; i++) {
			SetUpChapterEvents ();
		}
	}

	// 初始化单个事件控件
	private void SetUpChapterEvents(){

		GameObject mChapterEvent = Instantiate (chapterEvent, chapterEventsContainer.transform);

		Image eventIcon = mChapterEvent.transform.Find ("ChapterEventView/EventIcon").GetComponent<Image>();
		Text eventTitle = mChapterEvent.transform.Find ("ChapterEventView/EventTitle").GetComponent<Text>();
		Text eventDescription = mChapterEvent.transform.Find ("ChapterEventView/EventDescription").GetComponent<Text>();
		Image eventConfirmIcon = mChapterEvent.transform.Find("ChapterEventView/EventSelectButton/EventConfirmIcon").GetComponent<Image>();


		switch (RandomEvent ()) {
		case EventType.Monster:
			MonsterGroup monsterGroup = RandomReturn<MonsterGroup> (detailInfo.monsterGroups);
			eventTitle.text = monsterGroup.monsterGroupName;
			eventDescription.text = monsterGroup.monsterGroupDescription;
			eventIcon.sprite = sprites.Find (delegate(Sprite obj) {
				return obj.name == monsterGroup.spriteName; 
			});
			eventConfirmIcon.sprite = sprites.Find (delegate(Sprite obj) {
				return obj.name == "battleIcon"; 
			});
			break;
		case EventType.NPC:
			NPC npc = RandomReturn<NPC> (detailInfo.npcs);
			eventTitle.text = npc.npcName;
			eventDescription.text = npc.npcDescription;
			eventIcon.sprite = sprites.Find (delegate(Sprite obj) {
				return obj.name == npc.spriteName;
			});
			eventConfirmIcon.sprite = sprites.Find (delegate(Sprite obj) {
				return obj.name == "chatIcon";
			});
			break;
		case EventType.Item:
			Item item = RandomReturn<Item> (detailInfo.items);
			eventTitle.text = item.itemName;
			eventDescription.text = item.itemDescription;
			eventIcon.sprite = sprites.Find (delegate(Sprite obj) {
				return obj.name == item.spriteName;
			});
			eventConfirmIcon.sprite = sprites.Find (delegate(Sprite obj) {
				return obj.name == "watchIcon";
			});
			break;
		default:
			break;
			}
	}

	// 返回随机事件
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
	// 随机返回当前事件中的npc／怪物组／物品
	private T RandomReturn<T>(T[] array){
		int randomIndex = (int)(Random.Range (0, array.Length - float.Epsilon));
		return array[randomIndex];
	}



}
