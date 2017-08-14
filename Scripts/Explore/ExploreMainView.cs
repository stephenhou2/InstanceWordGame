using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using DG.Tweening;

public class ExploreMainView: MonoBehaviour {

	public Transform chapterListsViewContainer;

	public Transform chapterListsPlane;

	public Transform chapterListsView;

//	private InstancePool chapterItemsPool;

	public GameObject chapterItemModel;



	public Transform exploreMainViewPlane;

	private InstancePool chapterEventsPool;

	public GameObject chapterEventModel;

	public Transform chapterEventsPlane;

	private List<Transform> chapterEventsVisible = new List<Transform> (); // 可见的eventView


	public int maxEventCountForOnce = 4; // 一次显示的事件数 

	public Text playerLevelText;
	public Text stepsLeftText;
	public Text chapterLocationText;
	public Slider playerHealthBar;


	private List<Sprite> exploreIcons;

	private ChapterDetailInfo chapterDetailInfo;


	float chapterEventViewHeight = 270.0f;

	float padding = 65.0f;

	/// <summary>
	/// 初始化章节列表界面
	/// </summary>
	/// <param name="chapterLists">章节信息数组</param>
	/// <param name="unlockedMaxChapter">已解锁的最大章节数</param>
	public void SetUpChapterListPlane(ChapterList[] chapterLists,ChapterDetailInfo[] chapterDetails,int unlockedMaxChapter){

//		chapterItemsPool = InstancePool.GetOrCreateInstancePool ("ChapterItemsPool");

		for (int i = 0; i < unlockedMaxChapter; i++) {

//			Transform chapterItem = chapterItemsPool.GetInstance (chapterItemModel, chapterListsContainer);

			Transform chapterItem = Instantiate (chapterItemModel, chapterListsView).transform;

			ChapterList chapterList = chapterLists [i];

			Image chapterIcon = chapterItem.FindChild ("ChapterIcon").GetComponent<Image> ();

			Text chapterName = chapterItem.FindChild ("ChapterName").GetComponent<Text> ();

			Text chapterDesc = chapterItem.FindChild ("ChapterDesc").GetComponent<Text> ();

			Text chapterStatus = chapterItem.FindChild ("ChapterStatus").GetComponent<Text> ();

			Text chapterUnlockReq = chapterItem.FindChild ("UnlockRequirement").GetComponent<Text> ();

			chapterIcon.sprite = null;

			chapterIcon.enabled = true;

			chapterName.text = chapterList.chapterName;

			chapterDesc.text = chapterList.chapterDescription;

			ChapterDetailInfo chapterDetail = chapterDetails [i];

			int finishStatus = 100 - chapterDetail.stepsLeft * 100 / chapterDetail.totalSteps;

			chapterStatus.text = "完成度: " + finishStatus.ToString() + "%";

			if (Player.mainPlayer.agentLevel < chapterList.unlockLevel) {
				
				chapterUnlockReq.text = "等级≧" + chapterList.unlockLevel.ToString() + "解锁";
				chapterUnlockReq.gameObject.SetActive (true);

			}



			int chapterIndex = i;

			chapterItem.GetComponent<Button> ().onClick.RemoveAllListeners ();

			chapterItem.GetComponent<Button> ().onClick.AddListener (delegate {
				GetComponent<ExploreMainViewController>().OnChapterListClick(chapterIndex);
			});

		}

		chapterListsViewContainer.gameObject.SetActive (true);
		exploreMainViewPlane.gameObject.SetActive (false);

	}
		

	public void SetUpExploreMainViewPlane(ChapterDetailInfo chapterDetail,List<Sprite> exploreIcons){


		chapterDetailInfo = chapterDetail;

		this.exploreIcons = exploreIcons;

		chapterEventsPool = InstancePool.GetOrCreateInstancePool ("ChapterEventsPool");

		InitChapterEventsPlane ();

		SetUpTopBar ();

		exploreMainViewPlane.gameObject.SetActive (true);
		chapterListsViewContainer.gameObject.SetActive (false);


	}
		

	// 初始化事件面板
	public void InitChapterEventsPlane(){
		
		for (int i = 0; i < maxEventCountForOnce; i++) {
			AddNewChapterEvent (i);
		}
	}


	// 初始化顶部bar
	private void SetUpTopBar(){

		Player player = Player.mainPlayer;

		playerLevelText.text = player.agentLevel.ToString();
		stepsLeftText.text = chapterDetailInfo.totalSteps.ToString();
		chapterLocationText.text = chapterDetailInfo.chapterLocation;
		playerHealthBar.maxValue = player.maxHealth;
		playerHealthBar.value = player.health;
		playerHealthBar.transform.FindChild ("HealthText").GetComponent<Text> ().text = player.health + "/" + Player.mainPlayer.maxHealth;

	}


	public void SetUpNextStepChapterEventPlane(Transform currentSelectedEventView,int stepsLeft){

		stepsLeftText.text = stepsLeft.ToString();
		Player player = Player.mainPlayer;

		playerHealthBar.maxValue = player.maxHealth;
		playerHealthBar.value = player.health;
		playerHealthBar.transform.FindChild ("HealthText").GetComponent<Text> ().text = player.health + "/" + Player.mainPlayer.maxHealth;


		Sequence mSequence = DOTween.Sequence ();

		mSequence.Append (currentSelectedEventView.DOLocalMoveX (Screen.width, 0.5f, false).
			OnComplete(()=>{

				AddNewChapterEvent(maxEventCountForOnce);

				float currentSelectedEventViewY = currentSelectedEventView.transform.localPosition.y;

				currentSelectedEventView.gameObject.SetActive(false);
				chapterEventsVisible.Remove(currentSelectedEventView);
				chapterEventsPool.AddInstanceToPool(currentSelectedEventView.gameObject);

				for(int i = 0;i<chapterEventsVisible.Count;i++)
				{
					Transform eventView = chapterEventsVisible[i];
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


		Transform mChapterEventView = GetChapterEvent ();

		ChapterEventView mChapterEventViewScript = mChapterEventView.GetComponent<ChapterEventView> ();

		mChapterEventView.localPosition = new Vector3 (0, -(chapterEventViewHeight + padding) * eventIndex, 0);

		chapterEventsVisible.Add (mChapterEventView);

		ExploreMainViewController exploreMainViewController = GetComponent<ExploreMainViewController> ();


		switch (RandomEvent ()) {
		case EventType.Monster:
			MonsterGroup monsterGroup = RandomReturn<MonsterGroup> (chapterDetailInfo.monsterGroups);
			mChapterEventViewScript.eventTitle.text = monsterGroup.monsterGroupName;
			mChapterEventViewScript.eventDescription.text = monsterGroup.monsterGroupDescription;
			mChapterEventViewScript.eventIcon.sprite = exploreIcons.Find (delegate(Sprite obj) {
				return obj.name == monsterGroup.spriteName; 
			});
			mChapterEventViewScript.eventConfirmIcon.sprite = exploreIcons.Find (delegate(Sprite obj) {
				return obj.name == "battleIcon"; 
			});
			mChapterEventViewScript.eventSelectButton.onClick.RemoveAllListeners ();
			mChapterEventViewScript.eventSelectButton.onClick.AddListener (delegate {
				exploreMainViewController.OnEnterBattle (monsterGroup,mChapterEventView);
			});
			break;
		case EventType.NPC:
			NPC npc = RandomReturn<NPC> (chapterDetailInfo.npcs);
			mChapterEventViewScript.eventTitle.text = npc.npcName;
			mChapterEventViewScript.eventDescription.text = npc.npcDescription;
			Sprite npcSprite = exploreIcons.Find (delegate(Sprite obj) {
				return obj.name == npc.spriteName;
			});
			mChapterEventViewScript.eventIcon.sprite = npcSprite;
			mChapterEventViewScript.eventConfirmIcon.sprite = exploreIcons.Find (delegate(Sprite obj) {
				return obj.name == "chatIcon";
			});
			mChapterEventViewScript.eventSelectButton.onClick.RemoveAllListeners ();
			mChapterEventViewScript.eventSelectButton.onClick.AddListener (delegate{
				exploreMainViewController.OnEnterNPC(npc,mChapterEventView,npcSprite);
			});
			break;
		case EventType.Item:
			
			Item item = RandomReturn<Item> (chapterDetailInfo.GetItems());
			mChapterEventViewScript.eventTitle.text = "木箱";
			mChapterEventViewScript.eventDescription.text = "一个被人遗弃的箱子";
			mChapterEventViewScript.eventIcon.sprite = exploreIcons.Find (delegate(Sprite obj) {
				return obj.name == "boxIcon";
			});
			mChapterEventViewScript.eventConfirmIcon.sprite = exploreIcons.Find (delegate(Sprite obj) {
				return obj.name == "watchIcon";
			});
			Sprite itemSprite = GameManager.Instance.allItemSprites.Find (delegate(Sprite obj) {
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

		mChapterEventView.gameObject.SetActive (true);
	}




	private Transform GetChapterEvent(){
		
		Transform mChapterEventView = chapterEventsPool.GetInstance<Transform> (chapterEventModel, chapterEventsPlane);

		ChapterEventView mChapterEventViewScript = mChapterEventView.GetComponent<ChapterEventView> ();

		mChapterEventViewScript.eventIcon = mChapterEventView.transform.Find ("ChapterEventView/EventIcon").GetComponent<Image>();
		mChapterEventViewScript.eventTitle = mChapterEventView.transform.Find ("ChapterEventView/EventTitle").GetComponent<Text>();
		mChapterEventViewScript.eventDescription = mChapterEventView.transform.Find ("ChapterEventView/EventDescription").GetComponent<Text>();
		mChapterEventViewScript.eventConfirmIcon = mChapterEventView.transform.Find("ChapterEventView/EventSelectButton/EventConfirmIcon").GetComponent<Image>();
		mChapterEventViewScript.eventSelectButton = mChapterEventView.transform.Find ("ChapterEventView/EventSelectButton").GetComponent<Button> ();

		return mChapterEventView;
	}

	public void OnQuitExploreChapterView(CallBack cb){

		chapterListsViewContainer.GetComponent<Image> ().color = new Color (0, 0, 0, 0);

		chapterListsPlane.DOLocalMoveY (-Screen.height, 0.5f).OnComplete (() => {

			cb();

		});
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
