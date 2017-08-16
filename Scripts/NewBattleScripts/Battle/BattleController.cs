using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


namespace WordJourney
{
	public class BattleController : MonoBehaviour {

		// 玩家控制器
		public BattlePlayerController bpController;

		// 玩家角色信息
		private Player player;

		// 玩家英雄角色数组
		private List<Agent> players = new List<Agent>();

		// 怪物数组
		public List<Agent> monsters = new List<Agent>();

		// 怪物缓存池
		private InstancePool monsterViewPool;

		public GameObject monsterViewModel;

		private List<Item> battleGains = new List<Item>();

		// 判断当前动画是否播放完毕
		private bool allAnimationEnd{
			get{
				return true;
//				bool animationEnd = true;
//
//				foreach (Monster m in monsters) {
//					animationEnd = animationEnd && !m.baView.isAnimating;
//				}
//				foreach (Player p in players) {
//					animationEnd = animationEnd && !p.baView.isAnimating;
//				}
//				return animationEnd;
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
		public void SetUpBattleView(Monster[] monsters){
			SetUpPlayer ();
				SetUpMonsters (monsters);
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
		private void SetUpMonsters(Monster[] monsterGroup){

			monsters.Clear ();

			float screenWidth = Screen.width;
				int monsterNum = monsterGroup.Length;

			for(int i = 0;i<monsterNum;i++){

				GameObject mMonsterView = GetMonsterView ();

				Monster mMonster = mMonsterView.GetComponent<Monster> ();
				mMonster.monsterId = i;
				mMonster.CopyAgentStatus (monsters [i]);
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
				mMonsterView.GetComponent<BattleMonsterController>().SetUpMonsterView ((Monster)monsters[i]);
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
			bpController.UpdateSkillButtonsStatus(player);

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
		private void ExcuteEffectToBattleAgents(List<Agent> bas){
			for (int i = 0; i < bas.Count; i++) {
				Agent ba = bas [i];
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


		// 用户点击技能按钮响应
		private void OnSkillClick(int buttonIndex){

			player.currentSkill = player.skillsEquiped [buttonIndex];

			bpController.OnPlayerSelectSkill (buttonIndex);

			OnPlayerSelectSkill(player.skillsEquiped [buttonIndex],buttonIndex);
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


		/// <summary>
		/// 战斗开始前重置战斗
		/// </summary>
		public void OnResetBattle(){

			//重置所有怪物
			foreach (Monster m in monsters) {
				m.ResetBattleAgentProperties (true,true);
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
}
