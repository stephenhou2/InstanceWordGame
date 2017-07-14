using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using DG.Tweening;

public class ExploreMainView: MonoBehaviour {



	public GameObject chapterEventView;

	public GameObject chapterEventsPlane;

	private List<GameObject> chapterEventViewsVisible = new List<GameObject> (); // 可见的eventView

	private List<GameObject> chapterEventViewPool = new List<GameObject> (); // eventView 缓存池

	public int maxEventCountForOnce = 4; // 一次显示的事件数 

	public Text playerLevelText;
	public Text stepsLeftText;
	public Text chapterLocationText;
	public Slider playerHealthBar;


	private List<Sprite> sprites = new List<Sprite>();

	private ChapterDetailInfo detailInfo;


	float chapterEventViewHeight = 270.0f;

	float padding = 65.0f;



	public void SetUpExploreMainView(ChapterDetailInfo chapterDetail){

		detailInfo = chapterDetail;

		InitChapterEventsPlane ();

		SetUpTopBar ();


	}
		

	// 初始化事件面板
	public void InitChapterEventsPlane(){
		foreach (Sprite s in ResourceManager.Instance.sprites) {
			sprites.Add (s);
		}
		for (int i = 0; i < maxEventCountForOnce; i++) {
			AddNewChapterEvent (i);
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


	public void SetUpNextStepChapterEventPlane(GameObject currentSelectedEventView,int stepsLeft){

		stepsLeftText.text = stepsLeft.ToString();
		Player player = Player.mainPlayer;

		playerHealthBar.maxValue = player.maxHealth;
		playerHealthBar.value = player.health;
		playerHealthBar.transform.FindChild ("HealthText").GetComponent<Text> ().text = player.health + "/" + Player.mainPlayer.maxHealth;


		Sequence mSequence = DOTween.Sequence ();

		mSequence.Append (currentSelectedEventView.transform.DOLocalMoveX (Screen.width, 0.5f, false).
			OnComplete(()=>{

				AddNewChapterEvent(maxEventCountForOnce);

				float currentSelectedEventViewY = currentSelectedEventView.transform.localPosition.y;

				currentSelectedEventView.gameObject.SetActive(false);
				chapterEventViewsVisible.Remove(currentSelectedEventView);
				chapterEventViewPool.Add(currentSelectedEventView);

				for(int i = 0;i<chapterEventViewsVisible.Count;i++)
				{
					GameObject eventView = chapterEventViewsVisible[i];
					if (eventView.transform.localPosition.y < currentSelectedEventViewY){

						eventView.transform.DOLocalMoveY((chapterEventViewHeight + padding) * (-i),1.0f);

//						TweenCallback tcb = ()=>{
//							currentSelectedEventView.gameObject.SetActive(false);
//							currentSelectedEventView.transform.position = new Vector3(0,-Screen.height,0);
//							chapterEventViewsVisible.Remove(currentSelectedEventView);
//							chapterEventViewPool.Add(currentSelectedEventView);
//						};
//
//						if(i == chapterEventViewsVisible.Count - 1){
//							myTween.OnComplete(tcb);
//						}
					}
				}

//				currentSelectedEventView.gameObject.SetActive(false);
//				chapterEventViewPool.Add(currentSelectedEventView);
			}));



	}


	// 初始化单个事件控件
	private void AddNewChapterEvent(int eventIndex){

//		bool newChapterEventView;

		GameObject mChapterEventView = GetChapterEventView ();

		ChapterEventView mChapterEventViewScript = mChapterEventView.GetComponent<ChapterEventView> ();

		mChapterEventView.transform.SetParent(chapterEventsPlane.transform);

		mChapterEventView.transform.localPosition = new Vector3 (0, -(chapterEventViewHeight + padding) * eventIndex, 0);

		chapterEventViewsVisible.Add (mChapterEventView);

		ExploreMainViewController exploreMainViewController = GetComponent<ExploreMainViewController> ();


//		Image eventIcon = mChapterEventView.transform.Find ("ChapterEventView/EventIcon").GetComponent<Image>();
//		Text eventTitle = mChapterEventView.transform.Find ("ChapterEventView/EventTitle").GetComponent<Text>();
//		Text eventDescription = mChapterEventView.transform.Find ("ChapterEventView/EventDescription").GetComponent<Text>();
//		Image eventConfirmIcon = mChapterEventView.transform.Find("ChapterEventView/EventSelectButton/EventConfirmIcon").GetComponent<Image>();
//		Button eventSelectButton = mChapterEventView.transform.Find ("ChapterEventView/EventSelectButton").GetComponent<Button> ();

		switch (RandomEvent ()) {
		case EventType.Monster:
			MonsterGroup monsterGroup = RandomReturn<MonsterGroup> (detailInfo.monsterGroups);
			mChapterEventViewScript.eventTitle.text = monsterGroup.monsterGroupName;
			mChapterEventViewScript.eventDescription.text = monsterGroup.monsterGroupDescription;
			mChapterEventViewScript.eventIcon.sprite = sprites.Find (delegate(Sprite obj) {
				return obj.name == monsterGroup.spriteName; 
			});
			mChapterEventViewScript.eventConfirmIcon.sprite = sprites.Find (delegate(Sprite obj) {
				return obj.name == "battleIcon"; 
			});
			mChapterEventViewScript.eventSelectButton.onClick.RemoveAllListeners ();
			mChapterEventViewScript.eventSelectButton.onClick.AddListener (delegate {
				exploreMainViewController.OnEnterBattle (monsterGroup,mChapterEventView);
			});
			break;
		case EventType.NPC:
			NPC npc = RandomReturn<NPC> (detailInfo.npcs);
			mChapterEventViewScript.eventTitle.text = npc.npcName;
			mChapterEventViewScript.eventDescription.text = npc.npcDescription;
			Sprite npcSprite = sprites.Find (delegate(Sprite obj) {
				return obj.name == npc.spriteName;
			});
			mChapterEventViewScript.eventIcon.sprite = npcSprite;
			mChapterEventViewScript.eventConfirmIcon.sprite = sprites.Find (delegate(Sprite obj) {
				return obj.name == "chatIcon";
			});
			mChapterEventViewScript.eventSelectButton.onClick.RemoveAllListeners ();
			mChapterEventViewScript.eventSelectButton.onClick.AddListener (delegate{
				exploreMainViewController.OnEnterNPC(npc,mChapterEventView);
			});
			break;
		case EventType.Item:
			Item item = RandomReturn<Item> (detailInfo.items);
			mChapterEventViewScript.eventTitle.text = "木箱";
			mChapterEventViewScript.eventDescription.text = "一个被人遗弃的箱子";
			mChapterEventViewScript.eventIcon.sprite = sprites.Find (delegate(Sprite obj) {
				return obj.name == "boxIcon";
			});
			mChapterEventViewScript.eventConfirmIcon.sprite = sprites.Find (delegate(Sprite obj) {
				return obj.name == "watchIcon";
			});
			Sprite itemSprite = sprites.Find (delegate(Sprite obj) {
				return obj.name == item.spriteName;
			});
			mChapterEventViewScript.eventSelectButton.onClick.RemoveAllListeners ();
			mChapterEventViewScript.eventSelectButton.onClick.AddListener (delegate{
				exploreMainViewController.OnEnterItem(item,mChapterEventView,itemSprite);
			});
			break;
		default:
			break;
		}
	}




	private GameObject GetChapterEventView(){
		GameObject mChapterEventView = null;
		if (chapterEventViewPool.Count != 0) {
			mChapterEventView = chapterEventViewPool [0];
			mChapterEventView.SetActive (true);
			chapterEventViewPool.RemoveAt (0);
		} else {
			mChapterEventView = Instantiate (chapterEventView);

			ChapterEventView mChapterEventViewScript = mChapterEventView.GetComponent<ChapterEventView> ();

			mChapterEventViewScript.eventIcon = mChapterEventView.transform.Find ("ChapterEventView/EventIcon").GetComponent<Image>();
			mChapterEventViewScript.eventTitle = mChapterEventView.transform.Find ("ChapterEventView/EventTitle").GetComponent<Text>();
			mChapterEventViewScript.eventDescription = mChapterEventView.transform.Find ("ChapterEventView/EventDescription").GetComponent<Text>();
			mChapterEventViewScript.eventConfirmIcon = mChapterEventView.transform.Find("ChapterEventView/EventSelectButton/EventConfirmIcon").GetComponent<Image>();
			mChapterEventViewScript.eventSelectButton = mChapterEventView.transform.Find ("ChapterEventView/EventSelectButton").GetComponent<Button> ();
		}
		return mChapterEventView;
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
