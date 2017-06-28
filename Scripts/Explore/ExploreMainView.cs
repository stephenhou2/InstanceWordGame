using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ExploreMainView: MonoBehaviour {



	public GameObject chapterEventView;

	private GameObject chapterEventsContainer;

//	public GameObject mChapterEvent;


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

//		LoadAssets ();

		LoadEventSprites ();
	}

	private void LoadAssets(){
		
		ResourceManager.Instance.LoadAssetWithName ("explore/explore",LoadEventSprites);

	}

	private void LoadEventSprites(){

		ResourceManager.Instance.LoadAssetWithName ("explore/icons",SetUpScene);
	}
		

	public void SetUpScene(){

		InitUI ();

		SetUpChapterEventsPlane ();

		SetUpTopBar ();






	}

	private void InitUI(){

		playerLevelText = GameObject.Find ("PlayerLevelText").GetComponent<Text>();
		stepsLeftText = GameObject.Find ("StepsLeftText").GetComponent<Text>();
		chapterLocationText = GameObject.Find ("ChapterLocationText").GetComponent<Text>();
		playerHealthBar = GameObject.Find ("PlayerHealth").GetComponent<Slider>();

//		chapterEventView = GameObject.Find ("ChapterEvent");
		chapterEventsContainer = GameObject.Find ("ChapterEventsContainer");


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

		GameObject mChapterEvent = Instantiate (chapterEventView, chapterEventsContainer.transform);

		ExploreMainViewController exploreMainViewController = GetComponent<ExploreMainViewController> ();

		Image eventIcon = mChapterEvent.transform.Find ("ChapterEventView/EventIcon").GetComponent<Image>();
		Text eventTitle = mChapterEvent.transform.Find ("ChapterEventView/EventTitle").GetComponent<Text>();
		Text eventDescription = mChapterEvent.transform.Find ("ChapterEventView/EventDescription").GetComponent<Text>();
		Image eventConfirmIcon = mChapterEvent.transform.Find("ChapterEventView/EventSelectButton/EventConfirmIcon").GetComponent<Image>();
		Button eventSelectButton = mChapterEvent.transform.Find ("ChapterEventView/EventSelectButton").GetComponent<Button> ();

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

			eventSelectButton.onClick.AddListener (delegate {
				exploreMainViewController.OnEnterBattle (monsterGroup);
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

			eventSelectButton.onClick.AddListener (delegate{
				exploreMainViewController.OnEnterNPC(npc);
			});
			break;
		case EventType.Item:
			Item item = RandomReturn<Item> (detailInfo.items);
			eventTitle.text = "木箱";
			eventDescription.text = "一个被人遗弃的箱子";
			eventIcon.sprite = sprites.Find (delegate(Sprite obj) {
				return obj.name == "boxIcon";
			});
			eventConfirmIcon.sprite = sprites.Find (delegate(Sprite obj) {
				return obj.name == "watchIcon";
			});

			eventSelectButton.onClick.AddListener (delegate{
				exploreMainViewController.OnEnterItem(item);
			});
			break;
		default:
			break;
		}
	}


	// 初始化顶部bar
	private void SetUpTopBar(){

		Player player = Player.mainPlayer;
	
		playerLevelText.text = player.agentLevel.ToString();
		stepsLeftText.text = detailInfo.totalSteps.ToString();
		chapterLocationText.text = detailInfo.chapterLocation;
		playerHealthBar.maxValue = player.maxHealth;
		playerHealthBar.value = player.health;
		playerHealthBar.transform.FindChild ("HealthText").GetComponent<Text> ().text = player.health + "/" + Player.mainPlayer.maxHealth;



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
