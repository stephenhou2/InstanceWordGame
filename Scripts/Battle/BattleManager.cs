using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;


public class BattleManager : MonoBehaviour {

//	private Player player;
//
//	public Monster monsterModel;
//
//	private Monster monster;

	// ********for test use**********//
	public BattlePlayerController bpController;

	private Player player;

	private List<BattleAgent> players = new List<BattleAgent>();

	public List<BattleAgent> monsters = new List<BattleAgent>();

	private List<GameObject> monsterViewPool = new List<GameObject> ();

//	public List<BattleAgent> monstersModel;

	// ********for test use**********//

	private bool allAnimationEnd{
		get{

			bool animationEnd = true;

			foreach (Monster m in monsters) {
				animationEnd = animationEnd && !m.baView.isAnimating;
			}
			foreach (Player p in players) {
				animationEnd = animationEnd && !p.baView.isAnimating;
			}
			Debug.Log (animationEnd);
			return animationEnd;
		}

	}// 玩家和怪物的动画是否结束

	private bool battleEnd;// 战斗是否结束


	public int gameProcess;

	public Text description;

	public Transform playerControlPlane;

	public Transform monsterControlPlane;

	public Transform upperPlane;

	/************ for test *************/
	public GameObject battleEndHUD;

	public Text battleEndResult;

	public Button quit;

	public Button reset;

	public void OnEnterBattle(MonsterGroup monsterGroup){
		SetUpPlayer ();
		SetUpMonsters (monsterGroup);
		OnResetGame ();
	}


	// 进入战斗场景时初始化玩家数据
	private void SetUpPlayer(){
		player = bpController.GetComponent<Player> ();
		player.CopyAgentStatus (Player.mainPlayer);
		bpController.player = player;

		players.Add (player);
	}

	private GameObject GetMonsterView(){
		GameObject monsterView = null;
		if (monsterViewPool.Count != 0) {
			monsterView = monsterViewPool [0];
			monsterView.SetActive (true);
			monsterViewPool.RemoveAt (0);
		} else {
			ResourceManager.Instance.LoadAssetWithName ("battle/monster_view", ()=>{
				monsterView = ResourceManager.Instance.gos [0];
			}, true);
		}

		return monsterView;
	}


	// 进入战斗场景时初始化怪物数据
	private void SetUpMonsters(MonsterGroup monsterGroup){

		float screenWidth = Screen.width;
		int monsterNum = monsterGroup.monsters.Length;

		for(int i = 0;i<monsterNum;i++){

			GameObject mMonsterView = GetMonsterView ();

			mMonsterView.transform.SetParent (upperPlane, false);

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


		
	// 玩家选择技能后的响应方法
	public void OnPlayerSelectSkill(Skill playerSkill){

		monsterControlPlane.gameObject.SetActive (true);

		player.currentSkill = playerSkill;

		bpController.baView.SelectedSkillAnim (player.currentSkill == player.attackSkill,
			player.currentSkill == player.defenceSkill,
			player.currentSkill.skillId);

		description.text = "player 使用了" + playerSkill.skillName;

		Debug.Log("player 选择了" + playerSkill.skillName);

		// 如果技能不需要指定对象(对象为玩家自己或者敌方全部)
		if (!playerSkill.needSelectEnemy) {

			bpController.baView.KillSelectedSkillAnim ();

			playerSkill.AffectAgents (player,null,null, monsters, playerSkill.skillLevel);

			playerSkill.isAvalible = false;// 使用技能之后该技能暂时进入不可用状态

			player.strength -= playerSkill.strengthConsume;//使用技能使用者减去对应的气力消耗

			UpdatePropertiesByStates ();

			UpdateUIAndCheckAgentAlive ();//更新玩家和怪物状态,判断游戏是否结束

			if (!battleEnd) {
				StartCoroutine ("OnMonsterAction");
			}
			return;
		}

		// 如果是指向型技能，关闭怪物前面的遮罩
		monsterControlPlane.gameObject.SetActive (false);
	}

	public void OnPlayerSelectItem(Item item){

	}

	// 如果技能需要选择作用对象，则技能效果由该方法触发
	public void OnPlayerSelectMonster(int monsterId){

		if (player.currentSkill == null) {
			player.currentSkill = DefaultSelectedSkill ();
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

		UpdateUIAndCheckAgentAlive ();//更新玩家和怪物状态,判断游戏是否结束

		if (!battleEnd) {
			StartCoroutine ("OnMonsterAction");
		}
	}


	private IEnumerator OnMonsterAction(){

		monsterControlPlane.gameObject.SetActive(true);
		playerControlPlane.gameObject.SetActive (true);

		Debug.Log ("monsters action");

		for(int i = 0;i < monsters.Count;i++){

			Monster monster = monsters[i] as Monster;

			yield return new WaitUntil (() => allAnimationEnd);

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

					UpdateUIAndCheckAgentAlive ();//更新玩家和怪物状态,判断游戏是否结束

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

	public IEnumerator OnEnterNextTurn(){

		yield return new WaitUntil (() => allAnimationEnd);

		Debug.Log ("Enter Next Turn!");

		player.currentSkill = null;

		UpdatePropertiesByStates ();

		BattleAgentStatesManager.CheckStates (players,monsters);

		// 更新玩家可采取的行动类型
		player.UpdateValidActionType ();

		// 更新按钮是否可以交互
		bpController.baView.UpdateUIStatus(player);

		// 如果玩家本轮无法行动，则直接进入怪物行动轮
		if (player.validActionType == ValidActionType.None) {
			OnMonsterAction ();
		} else {
			// 下回合开始时关闭所有遮罩
			monsterControlPlane.gameObject.SetActive(false);
			playerControlPlane.gameObject.SetActive (false);
		}
		DefaultSelectedSkill ();
	}

		

	private Skill DefaultSelectedSkill(){

		// 如果气力大于普通攻击所需的气力值，则默认选中普通攻击
		if (player.strength >= player.attackSkill.strengthConsume && player.validActionType != ValidActionType.PhysicalExcption) {
			player.currentSkill = player.attackSkill;
			bpController.baView.SelectedSkillAnim (true, false, -1);
			return player.attackSkill;

		}

		// 如果玩家没有被沉默，默认选中可以第一个可以使用的技能
		if (player.validActionType != ValidActionType.MagicException) {
			foreach (Skill s in player.skills) {
				if (s.isAvalible && player.strength >= s.strengthConsume) {
					player.currentSkill = s;
					bpController.baView.SelectedSkillAnim (false, false, s.skillId);
					return s;
				}
			}
		}


		// 如果其他技能都无法使用，则默认选中防御
		player.currentSkill = player.defenceSkill;
		bpController.baView.SelectedSkillAnim(false,true,-1);
		return player.defenceSkill;

	}




	private void UpdatePropertiesByStates(){
		ExcuteEffectToBattleAgents (players);
		ExcuteEffectToBattleAgents (monsters);
	}

	private void ExcuteEffectToBattleAgents(List<BattleAgent> bas){
		for (int i = 0; i < bas.Count; i++) {
			BattleAgent ba = bas [i];
			for(int j = 0;j<ba.states.Count;j++){
				StateSkillEffect sse = ba.states [j];
				sse.ExcuteEffect (ba,null,null,null, sse.skillLevel, TriggerType.None, 0);
			}
		}
	}


	private void UpdateUIAndCheckAgentAlive(){

		#warning picturs of states toAdd

		if (players[0].health <= 0) {
			battleEndHUD.gameObject.SetActive (true);
			battleEnd = true;
			battleEndResult.text = "You Lose!";
			return;
		} 

		for (int i = 0; i < monsters.Count; i++) {
			Monster monster = monsters [i] as Monster;
			CallBack cb = () => {
				monsterViewPool.Add(monster.baView.gameObject);
				monster.baView.transform.SetParent(ContainerManager.FindContainer(CommonData.poolContainerName));
				monsters.Remove (monster);

				if (monsters.Count == 0) {
					battleEndHUD.gameObject.SetActive (true);
					battleEnd = true;
					battleEndResult.text = "you win";
					return;
				}
				battleEnd = false;
			};
			if (monster.health <= 0) {
				monster.AgentDie (cb);
			}
		}

	}



	// 用户点击攻击按钮响应
	public void OnAttack(){
		OnPlayerSelectSkill(player.attackSkill);
	}
	// 用户点击防御按钮响应
	public void OnDenfence(){
		OnPlayerSelectSkill(player.defenceSkill);
	}
	// 用户点击技能按钮响应
	public void OnSkill(int skillIndex){
		OnPlayerSelectSkill(player.skills [skillIndex]);
	}
	// 用户点击物品按钮响应
	public void OnItem(int itemIndex){

	}

	//*********** for skill test use **************//
	public void OnResetGame(){
		

		/****************测试用，重置所有怪物****************/
		foreach (Monster m in monsters) {
			m.ResetBattleAgentProperties (true);
			m.baView.agentIcon.color = Color.white;
			m.gameObject.SetActive (true);
//			monsters.Add (m);
		}

		for (int i = 0; i < players.Count; i++) {
			players[i].states.Clear ();
		}
		for (int i = 0; i < monsters.Count; i++) {
			monsters[i].states.Clear ();
			monsters [i].gameObject.SetActive (true);
			monsters [i].enabled = true;
		}

		OnReset ();

		battleEndHUD.SetActive (false);

		bpController.baView.ResetPlayerUI();


		playerControlPlane.gameObject.SetActive (false);
		monsterControlPlane.gameObject.SetActive (false);

		battleEnd = false;
		StartCoroutine ("OnEnterNextTurn");

		/****************测试用，重置所有怪物****************/
	}

	public void OnQuitBattle(){

		Player.mainPlayer.CopyAgentStatus (player);

		Debug.Log ("quit to main screen");

		battleEndHUD.gameObject.SetActive (false);

		GameObject exploreCanvas = GameObject.Find ("ExploreCanvas");

		exploreCanvas.GetComponent<Canvas>().enabled = true;

		GameObject.Find ("BattleCanvas").GetComponent<Canvas>().enabled = false;

		exploreCanvas.GetComponent<ExploreMainViewController> ().OnNextEvent ();

	}

	public void OnReset(){
		for (int i = 0; i < players.Count; i++) {
			players[i].ResetBattleAgentProperties (true);
		}
		for (int i = 0; i < monsters.Count; i++) {
			monsters[i].ResetBattleAgentProperties (true);
		}
			
	}

}
