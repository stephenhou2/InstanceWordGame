using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class BattleViewController : MonoBehaviour {

	// 玩家控制器
	public BattlePlayerController bpController;

	// 玩家角色信息
	private Player player;

	// 玩家英雄角色数组
	private List<BattleAgent> players = new List<BattleAgent>();

	// 怪物数组
	public List<BattleAgent> monsters = new List<BattleAgent>();

	// 怪物缓存池
	private InstancePool monsterViewPool;

	public GameObject monsterViewModel;

	private List<Item> battleGains = new List<Item>();

	// 判断当前动画是否播放完毕
	private bool allAnimationEnd{
		get{

			bool animationEnd = true;

			foreach (Monster m in monsters) {
				animationEnd = animationEnd && !m.baView.isAnimating;
			}
			foreach (Player p in players) {
				animationEnd = animationEnd && !p.baView.isAnimating;
			}
			return animationEnd;
		}

	}// 玩家和怪物的动画是否结束

	// 战斗是否结束
	private bool battleEnd;

	// 游戏进度
	public int gameProcess;

	// 技能和物品按钮的点击类型（点击／长按／取消）
	private PressType mPressType;

	// 当前点击的按钮序号
	private int currentClickButtonIndex;

	// 当前点击的按钮类型（true－技能按钮，false－物品按钮）
	private bool isSkillButton;

	// 当前点击的按钮transform组件
	private Transform currentClickButtonTrans;

	// 底部界面控制遮罩
	public Transform playerControlPlane;

	// 上部界面控制遮罩
	public Transform monsterControlPlane;

	public Transform upperPlane;

	public void Awake(){

		monsterViewPool = InstancePool.GetOrCreateInstancePool ("MonsterViewPool");

	}

	// 初始化战斗界面
	public void SetUpBattleView(MonsterGroup monsterGroup){
		SetUpPlayer ();
		SetUpMonsters (monsterGroup);
		OnResetBattle ();

		GetComponent<Canvas> ().worldCamera = Camera.main;
	}


	// 进入战斗场景时初始化玩家数据，初始化底部界面
	private void SetUpPlayer(){
		players.Clear ();
		player = bpController.GetComponent<Player> ();
		player.CopyAgentStatus (Player.mainPlayer);
		bpController.player = player;
		players.Add (player);
		bpController.SetUpBattlePlayerView ();
	}
		
	// 从缓存池中获取怪物view
	private GameObject GetMonsterView(){

		Transform monsterView = monsterViewPool.GetInstance<Transform> (monsterViewModel, upperPlane);

		return monsterView.gameObject;
	}


	// 进入战斗场景时初始化怪物数据，初始化怪物界面
	private void SetUpMonsters(MonsterGroup monsterGroup){

		monsters.Clear ();

		float screenWidth = Screen.width;
		int monsterNum = monsterGroup.monsters.Length;

		for(int i = 0;i<monsterNum;i++){

			GameObject mMonsterView = GetMonsterView ();

			Monster mMonster = mMonsterView.GetComponent<Monster> ();
			mMonster.monsterId = i;
			mMonster.CopyAgentStatus (monsterGroup.monsters [i]);
			monsters.Add (mMonster);
			switch (monsterNum) {
			case 1:
				mMonsterView.transform.localPosition = Vector3.zero;
				break;
			case 2:
				mMonsterView.transform.localPosition = new Vector3 (2 * (i - 0.5f) * 230f, -600f, 0);
				break;
			case 3:
				mMonsterView.transform.localPosition = new Vector3 ((i - 1f) * 350f,
					-600f + (i % 2 == 0 ? 1 : -1) * 50f,
					0);
				break;
			}
			mMonsterView.transform.localRotation = Quaternion.identity;
			mMonsterView.transform.localScale = Vector3.one;
			Debug.Log (monsters [i]);
			mMonsterView.GetComponent<BattleMonsterView>().SetUpMonsterView ((Monster)monsters[i]);
		}
	}


		
	// 玩家选择指定技能后的响应方法
	/// <param name="playerSkill">Player skill.</param>
	/// <param name="buttonIndex">Button index.</param>
	public void OnPlayerSelectSkill(Skill playerSkill,int buttonIndex){

		monsterControlPlane.gameObject.SetActive (true);

		Debug.Log("player 选择了" + playerSkill.skillName);

		// 如果技能不需要指定对象(对象为玩家自己或者敌方全部)
		if (!playerSkill.needSelectEnemy) {

			bpController.baView.KillSelectedSkillAnim ();

			playerSkill.AffectAgents (player,null,null, monsters, playerSkill.skillLevel);

			playerSkill.isAvalible = false;// 使用技能之后该技能暂时进入不可用状态

			player.strength -= playerSkill.strengthConsume;//使用技能使用者减去对应的气力消耗

			UpdatePropertiesByStates ();


			StartCoroutine ("UpdateUIAndCheckAgentAlive");
//			UpdateUIAndCheckAgentAlive ();//更新玩家和怪物状态,判断游戏是否结束

			if (!battleEnd) {
				StartCoroutine ("OnMonsterAction");
			}
			return;
		}

		// 如果是指向型技能，关闭怪物前面的遮罩
		monsterControlPlane.gameObject.SetActive (false);
	}

	// 如果技能需要选择作用对象，则技能效果由该方法触发
	public void OnPlayerSelectMonster(int monsterId){

		if (player.currentSkill == null) {
			player.currentSkill = bpController.DefaultSelectedSkill ();
		}

		if (!player.currentSkill.needSelectEnemy) {
			return;
		}

		// 选择完怪物之后打开遮罩
		monsterControlPlane.gameObject.SetActive (true);
		playerControlPlane.gameObject.SetActive (true);


		bpController.baView.KillSelectedSkillAnim ();

		Monster monster = null;
		foreach (Monster m in monsters) {
			if (m.monsterId == monsterId) {
				monster = m;
			}
		}

		player.currentSkill.AffectAgents (player,players, monster,monsters, player.currentSkill.skillLevel);

		player.currentSkill.isAvalible = false;// 使用技能之后该技能暂时进入不可用状态

		player.strength -= player.currentSkill.strengthConsume;//使用魔法后使用者减去对应的气力消耗

		UpdatePropertiesByStates ();

		StartCoroutine ("UpdateUIAndCheckAgentAlive");
//		UpdateUIAndCheckAgentAlive ();//更新玩家和怪物状态,判断游戏是否结束

		if (!battleEnd) {
			StartCoroutine ("OnMonsterAction");
		}
	}

	/// <summary>
	/// 怪物行动轮
	/// </summary>
	private IEnumerator OnMonsterAction(){

		monsterControlPlane.gameObject.SetActive(true);
		playerControlPlane.gameObject.SetActive (true);

		for(int i = 0;i < monsters.Count;i++){

			Monster monster = monsters[i] as Monster;

			yield return new WaitUntil (() => allAnimationEnd);

			Debug.Log ("monsters action");

			if (!monster.isActive) {
				continue;
			}

			if (!battleEnd) {

				if (!monster.baView.isActiveAndEnabled) {
					i--;
					continue;
				}

				if (monster.validActionType != ValidActionType.None) {

					monster.ManageSkillAvalibility ();

					#warning 这里添加怪物技能逻辑，选择使用的技能
					//		Skill monsterSkill = monster.SkillOfMonster ();
					Skill monsterSkill = monster.attackSkill;

					monsterSkill.AffectAgents (monster, monsters, player, players, monsterSkill.skillLevel);

					monster.strength -= monsterSkill.strengthConsume;

					monsterSkill.isAvalible = false;

					UpdatePropertiesByStates ();

					StartCoroutine ("UpdateUIAndCheckAgentAlive");
//					UpdateUIAndCheckAgentAlive ();//更新玩家和怪物状态,判断游戏是否结束

					if (!battleEnd) {
						if (monster.monsterId == (monsters[monsters.Count - 1] as Monster).monsterId) {
							StartCoroutine ("OnEnterNextTurn");//进入下一回合
						}
					}


					#warning 这里不太对，如果有反击之类的被动还需要执行反击的动画，回合应该在被动执行完成之后结束，后面加上动画之后再修改一下

					continue;
				}
			}
		}
		if (monsters.Count >= 1 && monsters [monsters.Count - 1].validActionType == ValidActionType.None) {

			StartCoroutine ("OnEnterNextTurn");//进入下一回合

		}
	}

	/// <summary>
	/// 进入下一回合
	/// </summary>
	public IEnumerator OnEnterNextTurn(){

		yield return new WaitUntil (() => allAnimationEnd);

		Debug.Log ("Enter Next Turn!");

		player.currentSkill = null;

		UpdatePropertiesByStates ();

		BattleAgentStatesManager.CheckStates (players,monsters);

		// 更新玩家可采取的行动类型
		player.UpdateValidActionType ();

		// 更新按钮是否可以交互
		bpController.baView.UpdateSkillButtonsStatus(player);

		// 如果玩家本轮无法行动，则直接进入怪物行动轮
		if (player.validActionType == ValidActionType.None) {
			OnMonsterAction ();
		} else {
			// 下回合开始时关闭所有遮罩
			monsterControlPlane.gameObject.SetActive(false);
			playerControlPlane.gameObject.SetActive (false);
		}
		bpController.DefaultSelectedSkill ();
	}

		

	/// <summary>
	/// 根据角色身上的现有状态更新角色属性
	/// </summary>
	private void UpdatePropertiesByStates(){
		ExcuteEffectToBattleAgents (players);
		ExcuteEffectToBattleAgents (monsters);
	}

	/// <summary>
	/// 根据角色身上的状态，触发状态所对应的效果
	/// </summary>
	/// <param name="bas">Bas.</param>
	private void ExcuteEffectToBattleAgents(List<BattleAgent> bas){
		for (int i = 0; i < bas.Count; i++) {
			BattleAgent ba = bas [i];
			for(int j = 0;j<ba.states.Count;j++){
				StateSkillEffect sse = ba.states [j];
				sse.ExcuteEffect (ba,null,null,null, sse.skillLevel, TriggerType.None, 0);
			}
		}
	}


	/// <summary>
	/// 更新UI，检查角色是否被击败
	/// </summary>
	private IEnumerator UpdateUIAndCheckAgentAlive(){


		yield return new WaitUntil (() => allAnimationEnd);

		#warning picturs of states toAdd

		if (players [0].health <= 0) {
//			battleEndHUD.gameObject.SetActive (true);
			battleEnd = true;
			StartCoroutine ("OnQuitBattle");
//			battleEndResult.text = "You Lose!";
		} else { 
			for (int i = 0; i < monsters.Count; i++) {
				Monster monster = monsters [i] as Monster;
				CallBack cb = () => {
					monsterViewPool.AddInstanceToPool(monster.baView.gameObject);
					monsters.Remove (monster);

					if (monsters.Count == 0) {
//					battleEndHUD.gameObject.SetActive (true);
						battleEnd = true;
						StartCoroutine ("OnQuitBattle");
//					battleEndResult.text = "you win";
						return;
					}
					battleEnd = false;
				};
				if (monster.health <= 0) {
					monster.AgentDie (cb);
				}
			}
		}

	}



	// 用户点击攻击按钮响应
	public void OnAttack(){

		player.currentSkill = player.attackSkill;

		bpController.OnPlayerSelectSkill (-1);

		OnPlayerSelectSkill(player.attackSkill,-1);
	}
	// 用户点击防御按钮响应
	public void OnDenfence(){

		player.currentSkill = player.defenceSkill;

		bpController.OnPlayerSelectSkill (-1);

		OnPlayerSelectSkill(player.defenceSkill,-1);
	}

	// 用户点击技能按钮响应
	private void OnSkillClick(int buttonIndex){

		player.currentSkill = player.skillsEquiped [buttonIndex];

		bpController.OnPlayerSelectSkill (buttonIndex);

		OnPlayerSelectSkill(player.skillsEquiped [buttonIndex],buttonIndex);
	}

	// 用户长按技能按钮响应
	private void OnSkillPress(int buttonIndex){
		bpController.OnSkillLongPress (buttonIndex);
	}


	// 用户点击物品按钮响应
	private void OnItemClick(int itemIndex){

//		Item item = player.allEquipedItems [itemIndex + 3];

		bpController.OnPlayerUseItem (itemIndex);

		UpdatePropertiesByStates ();

		StartCoroutine ("UpdateUIAndCheckAgentAlive");
//		UpdateUIAndCheckAgentAlive ();//更新玩家和怪物状态,判断游戏是否结束

		if (!battleEnd) {
			StartCoroutine ("OnMonsterAction");
		}

	}

	// 用户长按物品按钮响应
	private void OnItemPress(int buttonIndex){
		bpController.OnItemLongPress (buttonIndex);
	}

	/// <summary>
	/// 技能按钮按下后开始判断本次点击类型
	/// </summary>
	/// <param name="index"> 按钮序号 </param>
	public void OnSkillButtonDown(int index){

		isSkillButton = true;
		currentClickButtonIndex = index;

//		if (!bpController.baView.skillButtons [index].interactable) {
//			return;
//		}
			
		currentClickButtonTrans = bpController.baView.skillButtons [index].transform;

		StartCoroutine ("CheckButtonClickType", bpController.baView.skillButtons [index].interactable);

	}

	/// <summary>
	/// 技能按钮点击结束时判断当前点击位置是否在按钮内部，如果不在则视为取消本次点击事件
	/// </summary>
	/// <param name="index">Index.</param>
	public void OnSkillButtonUp(int index){

		Vector2 mousePos;

		#if UNITY_WINDOWS || UNITY_EDITOR
		mousePos = Input.mousePosition;
		#elif UNITY_IOS || UNITY_ANDROID
		mousePos = Input.GetTouch(0).position;
		#endif

		RectTransform rt = currentClickButtonTrans as RectTransform;

		Vector3[] corners = new Vector3[4];
		rt.GetWorldCorners (corners);

		if (mousePos.x < corners[0].x || mousePos.y < corners[0].y || mousePos.x > corners[2].x || mousePos.y > corners[2].y){
			mPressType = PressType.Cancel;
		}
		bpController.OnSkillButtonUp ();

	}


	/// <summary>
	/// 物品按钮按下后开始判断本次点击类型
	/// </summary>
	/// <param name="index"> 按钮序号 </param>
	public void OnItemButtonDown(int index){

		isSkillButton = false;
		currentClickButtonIndex = index;

//		if (!bpController.baView.itemButtons [index].interactable) {
//			return;
//		}

		currentClickButtonTrans = bpController.baView.itemButtons [index].transform;

		StartCoroutine ("CheckButtonClickType", bpController.baView.itemButtons [index].interactable);

	}


	/// <summary>
	/// 物品按钮点击结束时判断当前点击位置是否在按钮内部，如果不在则视为取消本次点击事件
	/// </summary>
	/// <param name="index">Index.</param>
	public void OnItemButtonUp(int index){

		Vector2 mousePos;

		#if UNITY_WINDOWS || UNITY_EDITOR
		mousePos = Input.mousePosition;
		#elif UNITY_IOS || UNITY_ANDROID
		mousePos = Input.GetTouch(0).position;
		#endif

		RectTransform rt = currentClickButtonTrans as RectTransform;

		mousePos = Camera.main.ScreenToWorldPoint (mousePos);

		Vector3[] corners = new Vector3[4];
		rt.GetWorldCorners (corners);


		if (mousePos.x < corners[0].x || mousePos.y < corners[0].y || mousePos.x > corners[2].x || mousePos.y > corners[2].y){
			mPressType = PressType.Cancel;
		}
		bpController.OnItemButtonUp ();

	}

		
	/// <summary>
	/// 根据本次点击按下的时间判断点击类型
	/// </summary>
	/// <param name="index">Index.</param>
	private IEnumerator CheckButtonClickType(bool btnInteractable){

		int index = currentClickButtonIndex;

		float pressDurationReq = 1.0f;
		float timeLast = pressDurationReq;

		float t1 = Time.time;

		float t2;

		while (timeLast > 0) {
			timeLast -= Time.deltaTime;

			#if UNITY_WINDOWS || UNITY_EDITOR
			if (Input.GetMouseButtonUp (0)) {
				timeLast = 0;
			}
			#elif UNITY_IOS || UNITY_ANDROID
			if(Input.touchCount == 0){
				timeLast = 0;
			}
			#endif
			yield return null;

		}

		t2 = Time.time;

		if (t2 - t1 + 0.01f >= pressDurationReq) {
			mPressType = PressType.LongPress;
		} else if (t2 - t1 < pressDurationReq && btnInteractable) {
			mPressType = PressType.Click;
		} else {
			mPressType = PressType.Cancel;
		}

		Vector2 mousePos;

		#if UNITY_WINDOWS || UNITY_EDITOR
		mousePos = Input.mousePosition;
		#elif UNITY_IOS || UNITY_ANDROID
		mousePos = Input.GetTouch(0).position;
		#endif


		RectTransform rt = currentClickButtonTrans as RectTransform;

		mousePos = Camera.main.ScreenToWorldPoint (mousePos);

		Vector3[] corners = new Vector3[4];
		rt.GetWorldCorners (corners);

		if (mousePos.x < corners[0].x || mousePos.y < corners[0].y || mousePos.x > corners[2].x || mousePos.y > corners[2].y){
			mPressType = PressType.Cancel;
		}

		if (isSkillButton) {
			OnPlayerSelectSkill (mPressType, index);
		} else {
			OnPlayerSelectItem (mPressType, index);
		}

	}

	/// <summary>
	/// 根据点击类型执行相应点击类型对应的响应方法
	/// </summary>
	/// <param name="pressType">Press type.</param>
	/// <param name="index">Index.</param>
	private void OnPlayerSelectSkill(PressType pressType,int index){

		switch (pressType) {
		case PressType.Click:
			OnSkillClick (index);
			break;
		case PressType.LongPress:
			OnSkillPress (index);
			break;
		case PressType.Cancel:
			break;
		}
	}

	/// <summary>
	/// 根据点击类型执行相应点击类型对应的响应方法
	/// </summary>
	/// <param name="pressType">Press type.</param>
	/// <param name="index">Index.</param>
	private void OnPlayerSelectItem(PressType pressType,int index){
		switch (pressType) {
		case PressType.Click:
			OnItemClick (index);
			break;
		case PressType.LongPress:
			OnItemPress (index);
			break;
		case PressType.Cancel:
			break;
		}
	}

	/// <summary>
	/// 战斗开始前重置战斗
	/// </summary>
	public void OnResetBattle(){

		//重置所有怪物
		foreach (Monster m in monsters) {
			m.ResetBattleAgentProperties (true,true);
			m.baView.agentIcon.color = Color.white;
			m.gameObject.SetActive (true);
		}

		for (int i = 0; i < players.Count; i++) {
			players[i].states.Clear ();
		}
		for (int i = 0; i < monsters.Count; i++) {
			monsters[i].states.Clear ();
			monsters [i].gameObject.SetActive (true);
			monsters [i].enabled = true;
		}

		ResetBattleGains ();

		ResetAgentsProperties ();

		playerControlPlane.gameObject.SetActive (false);
		monsterControlPlane.gameObject.SetActive (false);

		battleEnd = false;
		StartCoroutine ("OnEnterNextTurn");

	}


	public void DiscardAllGains(){

		bpController.QuitBattleGainsHUD ();

		BackToExploreView ();

	}

	public void PickUpAllGains(){

		player.allItems.AddRange (battleGains);

		player.ArrangeAllItems ();

		bpController.QuitBattleGainsHUD ();

		BackToExploreView ();

	}

	/// <summary>
	/// 退出战斗场景
	/// </summary>
	public IEnumerator OnQuitBattle(){

		player.ResetBattleAgentProperties (false, true);

		Player.mainPlayer.CopyAgentStatus (player);

		yield return new WaitUntil (() => allAnimationEnd);

		Debug.Log ("quit to main screen");

		if (battleGains.Count > 0) {

			bpController.SetUpBattleGainsHUD (battleGains);

		} else {
			BackToExploreView ();
		}

	}

	private void BackToExploreView(){
		
		GameObject exploreCanvas = GameObject.Find (CommonData.exploreMainCanvas);

		exploreCanvas.GetComponent<Canvas>().enabled = true;

		GameObject.Find (CommonData.battleCanvas).GetComponent<Canvas>().enabled = false;

		exploreCanvas.GetComponent<ExploreMainViewController> ().OnNextEvent ();

	}

	private void ResetBattleGains(){

		battleGains.Clear ();

		for (int i = 0; i < monsters.Count; i++) {

			Monster m = monsters [i] as Monster;

			for (int j = 0; j < m.allItems.Count; i++) {

				Item item = m.allItems [j];

				int itemCount = RandomCount (item);

				if (itemCount > 0) {
					item.itemCount = itemCount;
					battleGains.Add (item);
				}

			}

		}

	}

	private int RandomCount(Item item){

		float i = 0f;
		i = Random.Range (0f, 10f);

		if (item.itemType == ItemType.Consumables) {
			
			if (i >= 0f && i < 5f) {
				return 0;
			} else if (i >= 5f && i < 9f) {
				return 1;
			} else {
				return 2;
			}

		} else {
			if (i >= 0f && i < 5f) {
				return 0;
			} else {
				return 1;
			}
		}
	}

	// 重置所有战斗角色的属性
	private void ResetAgentsProperties(){
		for (int i = 0; i < players.Count; i++) {
			players[i].ResetBattleAgentProperties (false,true);
		}
		for (int i = 0; i < monsters.Count; i++) {
			monsters[i].ResetBattleAgentProperties (true,true);
		}
			
	}

}
