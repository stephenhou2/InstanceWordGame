using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour {

//	private Player player;
//
//	public Monster monsterModel;
//
//	private Monster monster;

	// ********for test use**********//
	public Player player;

	private List<BattleAgent> players = new List<BattleAgent>();

	public List<BattleAgent> monsters = new List<BattleAgent>();

	public List<BattleAgent> monstersModel;

	// ********for test use**********//


	#warning 动画后面添加一下
	private bool allAnimationEnd{
		get{

			bool animationEnd = true;

			foreach (Monster m in monsters) {
				animationEnd = animationEnd && !m.isAnimating;
			}
			foreach (Player p in players) {
				animationEnd = animationEnd && !p.isAnimating;
			}
			return animationEnd;
		}

	}// 玩家和怪物的动画是否结束

	private bool battleEnd;// 战斗是否结束


	public int gameProcess;

	private Skill playerSkill;

	public Button[] skillButtons;

	public Button attackButton;

	public Button defenceButton;

	public Button[] itemButtons;

	public Slider playerHealth;

	public Slider playerStrength;

	public Slider monsterHealth;

	public Slider monsterStrength;

	public Text description;

	public GameObject controlPlane;

	public GameObject battleEndHUD;

	public Text battleEndResult;

	public Button quit;

	public Button reset;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	static private void CallBackAfterBattleSceneLoaded(){
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
	{

		BattleManager bm = GameObject.Find ("BattleManager").GetComponent<BattleManager> ();

		int gameProcess = GameManager.gameManager.gameProcess;

		Player[] players = new Player[]{GameManager.gameManager.player};

//		bm.SetupBattleManager (player, gameProcess);
	}

	// **************for test use************//
	// 后面改为scene加载后执行的函数（上面的）
	void Awake(){

		SetupBattleManager (player, gameProcess);
		players.Add (player);
		OnResetGame ();
	}

	// 进入战斗场景时初始化玩家和怪物数据
	public void SetupBattleManager(Player player,int gameProcess){

		// 每一关根据游戏进度创建新的怪物
//		monster = Instantiate (monsterModel);

//		monster.SetupMonster (gameProcess);

//		player = Player.mainPlayer;

		#warning 这里是否会产生循环引用？注意一下
//		foreach (Player player in players) {
//			player.enemies = monsters;
//		}
//
//		foreach (Monster monster in monsters) {
//			monster.enemies = players;
//		}




	}

	public void PlayerSelectSkill(int skillId){

		controlPlane.gameObject.SetActive (true);// 遮罩，下回合开始前用户无法再点击按钮

		playerSkill = player.skills [skillId];// 用户选择的技能

		description.text = "player 使用了" + playerSkill.skillName;

		Debug.Log("player 使用了" + playerSkill.skillName);

		// 如果技能不需要指定对象(对象为玩家自己或者敌方全部)
		if (!playerSkill.needSelectEnemy) {
			
			playerSkill.AffectAgents (player,null,null, monsters, playerSkill.skillLevel);

			playerSkill.isAvalible = false;// 使用技能之后该技能暂时进入不可用状态

			player.strength -= playerSkill.strengthConsume;//使用技能使用者减去对应的气力消耗

			try {
				StartCoroutine ("PlayerAnimation");
			} catch (System.Exception e) {
				Debug.Log (e.Message);
			}
		}
	}

	// 如果技能需要选择作用对象，则技能效果由该方法触发
	public void PlayerSelectMonster(int monsterId){
		
		Monster monster = monsters [monsterId] as Monster;

		playerSkill.AffectAgents (player,players, monster,monsters, playerSkill.skillLevel);

		playerSkill.isAvalible = false;// 使用技能之后该技能暂时进入不可用状态

		player.strength -= playerSkill.strengthConsume;//使用魔法后使用者减去对应的气力消耗

		try {
			StartCoroutine ("PlayerAnimation");
		} catch (System.Exception e) {
			Debug.Log (e.Message);
		}
	}


	public IEnumerator PlayerAnimation(){

		#warning player animation
		yield return new WaitForSeconds (1.0f);//动画暂时由延时1s代替

		UpdatePropertiesByStates ();

		UpdateUIAndBattleResult ();//更新玩家和怪物状态,判断游戏是否结束

		if (!battleEnd) {
			StartCoroutine ("MonsterAction");
		}
		battleEnd = false;



	}


	private IEnumerator MonsterAction(){

		Debug.Log ("monsters action");

		foreach (Monster monster in monsters) {

			yield return new WaitUntil (() => allAnimationEnd);

			if (!battleEnd) {
				if (monster.validActionType != ValidActionType.None) {

					monster.ManageSkillAvalibility ();

					#warning 这里添加怪物技能逻辑，选择使用的技能
					//		Skill monsterSkill = monster.SkillOfMonster ();
					Skill monsterSkill = monster.skills [0];//怪物技能暂时都使用第一个技能

					monsterSkill.AffectAgents (monster, monsters, player, players, monsterSkill.skillLevel);

					monster.strength -= monsterSkill.strengthConsume;

					monsterSkill.isAvalible = false;

					StartCoroutine ("MonsterAnimation", monster);

					#warning 这里不太对，如果有反击之类的被动还需要执行反击的动画，回合应该在被动执行完成之后结束，后面加上动画之后再修改一下

					continue;
				}
			}
		}
		if (monsters.Count >= 1 && monsters [monsters.Count - 1].validActionType == ValidActionType.None) {

			controlPlane.gameObject.SetActive (false);

			StartCoroutine ("EnterNextTurn");//进入下一回合

		}
		battleEnd = false;
	}

	public IEnumerator MonsterAnimation(Monster monster){

		#warning player animation
		yield return new WaitForSeconds (1.0f);

		UpdatePropertiesByStates ();

		UpdateUIAndBattleResult ();//更新玩家和怪物状态,判断游戏是否结束

		if (monster.monsterId == monsters.Count - 1) {

			controlPlane.gameObject.SetActive (false);

			StartCoroutine ("EnterNextTurn");//进入下一回合

		}
			
	}


	public IEnumerator EnterNextTurn(){

		yield return new WaitUntil (() => allAnimationEnd);

		Debug.Log ("Enter Next Turn!");

		UpdatePropertiesByStates ();

		BattleAgentStatesManager.CheckStates (players,monsters);

		// 根据玩家可采取的行动类型设定技能按钮是否可以交互
		ValidActionForPlayer ();


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


	public void UpdateUIAndBattleResult(){
		
		UpdateStatesUI (players);
		UpdateStatesUI (monsters);

		#warning picturs of states toAdd



		if (players[0].health <= 0) {
			battleEndHUD.gameObject.SetActive (true);
			battleEnd = true;
			battleEndResult.text = "You Lose!";
			return;
		} 
		for (int i = 0; i < monsters.Count; i++) {
			Monster monster = monsters [i] as Monster;
			if (monster.health <= 0) {
				monsters.Remove (monster);
				i--;
			}
		}
		if (monsters.Count == 0) {
			battleEndHUD.gameObject.SetActive (true);
			battleEnd = true;
			battleEndResult.text = "you win";
		}

	}

	public void UpdateStatesUI(List<BattleAgent> bas){
		for (int i = 0; i < bas.Count; i++) {
			BattleAgent ba = bas [i];
			ba.healthBar.value = ba.health;
			ba.strengthBar.value = ba.strength;
		}
//		playerHealth.value = player.health;
//		playerStrength.value = player.strength;
//		monsterHealth.value = monster.health;
//		monsterStrength.value = monster.strength;

	}

	public void ValidActionForPlayer(){

		switch (players[0].validActionType) {

		case ValidActionType.All:
			break;
		case ValidActionType.PhysicalExcption:
			EnableOrDisableButtons (false, true, true, true);
			break;
		case ValidActionType.MagicException:
			EnableOrDisableButtons (true, false, true, true);
			break;
		case ValidActionType.None:
			EnableOrDisableButtons (false, false, false, false);
			break;
		case ValidActionType.PhysicalOnly:
			EnableOrDisableButtons (true, false, false, true);
			break;
		case ValidActionType.MagicOnly:
			EnableOrDisableButtons (false, true, false, true);
			break;
		default:
			break;
		}
		// 如果技能还在冷却中或者玩家气力值小于技能消耗的气力值，则相应按钮不可用
		Player player = players[0] as Player;
		for (int i = 0;i < player.skills.Count;i++) {
			
			Skill s = player.skills [i];

			// 如果是冷却中的技能
			if (s.isAvalible == false) {
				s.actionCount++;
				Debug.Log (s.skillName + "从使用开始经过了" + s.actionCount + "回合");
				if (s.actionCount >= s.actionConsume && player.strength >= s.strengthConsume) {
					skillButtons [i].interactable = true;
					s.isAvalible = true;
					s.actionCount = 0;
				} else {
					skillButtons [i].interactable = false;
				}
			}
			// 如果不是冷却中的技能
			else {
				skillButtons [i].interactable = player.strength >= s.strengthConsume;
			} 
		}

	}

	//根据玩家状态判断按钮是否可以交互
	private void EnableOrDisableButtons(bool isAttackEnable,bool isSkillEnable,bool isItemEnable,bool isDefenceEnable){
		attackButton.interactable = isAttackEnable && attackButton.interactable;
		defenceButton.interactable = isDefenceEnable && defenceButton.interactable;
		foreach (Button btn in skillButtons) {
			btn.interactable = isSkillEnable && btn.interactable;
		}
		foreach (Button btn in itemButtons) {
			btn.interactable = isItemEnable && btn.interactable;
		}
	}



	public void OnAttack(){
		PlayerSelectSkill (3);
	}

	public void OnDenfence(){
		PlayerSelectSkill (4);
	}

	public void OnSkill(int skillIndex){
		PlayerSelectSkill (skillIndex);
	}

	public void OnItem(int itemIndex){

	}

	//*********** for skill test use **************//
	public void OnResetGame(){
		for (int i = 0; i < players.Count; i++) {
			players[i].states.Clear ();
		}
		for (int i = 0; i < monsters.Count; i++) {
			monsters[i].states.Clear ();
		}
		OnReset ();

		battleEndHUD.SetActive (false);
		UpdateStatesUI (players);
		UpdateStatesUI (monsters);
		foreach (Button btn in skillButtons) {
			btn.interactable = true;
		}
		controlPlane.gameObject.SetActive (false);

		/****************测试用，重置所有怪物****************/
		foreach (Monster m in monstersModel) {
			m.ResetBattleAgentProperties (true);

			monsters.Add (m);
		}
		UpdateStatesUI (monsters);

		/****************测试用，重置所有怪物****************/
	}

	public void OnQuitBattle(){
		Debug.Log ("quit to main screen");
		battleEndHUD.gameObject.SetActive (false);
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
