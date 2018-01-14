using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DragonBones;
using Transform = UnityEngine.Transform;


namespace WordJourney
{

	public class BattlePlayerController : BattleAgentController {



		public GameObject playerForward;
		public GameObject playerBackWard;
		public GameObject playerSide;

		// 事件回调
		public ExploreEventHandler enterMonster;
		public ExploreEventHandler enterNpc;
//		public ExploreEventHandler enterWorkBench;
		public ExploreEventHandler enterCrystal;
		public ExploreEventHandler enterTreasureBox;
		public ExploreEventHandler enterObstacle;
		public ExploreEventHandler enterTrapSwitch;
		public ExploreEventHandler enterDoor;
		public ExploreEventHandler enterBillboard;
		public ExploreEventHandler enterHole;
		public ExploreEventHandler enterMovableBox;
		public ExploreEventHandler enterTransport;
		public ExploreEventHandler enterPlant;

		// 移动速度
		public float moveDuration;			

		// 当前的单步移动动画对象
		private Tweener moveTweener;

		// 标记是否在单步移动中
		private bool inSingleMoving;

		// 移动路径点集
		public List<Vector3> pathPosList;

		// 正在前往的节点位置
		public Vector3 singleMoveEndPos;

		// 移动的终点位置
		public Vector3 moveDestination;

		// 玩家UI控制器
		private BattlePlayerUIController bpUICtr;

		// 当前碰到的怪物控制器
		private BattleMonsterController bmCtr;

		// 玩家战斗失败回调
		private CallBack playerLoseCallBack;

		private NavigationHelper navHelper;

		public bool isInFight;

		public bool isAttackActionFinish;


		protected override void Awake(){

			ActiveBattlePlayer (false, false, false);

			agent = GetComponentInParent<Player> ();

			navHelper = GetComponent<NavigationHelper> ();

			isInFight = false;

			isAttackActionFinish = true;

			base.Awake ();

		}
			
		public void InitBattlePlayer(){
			
			Transform canvas = TransformManager.FindTransform ("ExploreCanvas");

			bpUICtr = canvas.GetComponent<BattlePlayerUIController> ();

		}

		private bool onlyStoreDestination;
		private bool thereIsStoredDestination;

		private Vector3 storedDestination;

		public void TempStoreDestinationAndDontMove(){
			onlyStoreDestination = true;
		}

		public override void SetSortingOrder (int order)
		{
			playerForward.GetComponent<UnityArmatureComponent> ().sortingOrder = order;
			playerBackWard.GetComponent<UnityArmatureComponent> ().sortingOrder = order;
			playerSide.GetComponent<UnityArmatureComponent> ().sortingOrder = order;
		}

		public void MoveToStoredDestination(){
			onlyStoreDestination = false;
			if (thereIsStoredDestination) {
				MoveToPosition (storedDestination, exploreManager.GetComponent<MapGenerator> ().mapWalkableInfoArray);
				thereIsStoredDestination = false;
			}
		}


		/// <summary>
		/// 按照指定路径 pathPosList 移动到终点 moveDestination
		/// </summary>
		/// <param name="pathPosList">Path position list.</param>
		/// <param name="moveDestination">End position.</param>
		public bool MoveToPosition(Vector3 moveDestination,int[,] mapWalkableInfoArray){

			if (onlyStoreDestination) {
				storedDestination = moveDestination;
				thereIsStoredDestination = true;
				return navHelper.FindPath (singleMoveEndPos, moveDestination, mapWalkableInfoArray).Count > 0;
			}

			// 计算自动寻路路径
			pathPosList = navHelper.FindPath(singleMoveEndPos,moveDestination,mapWalkableInfoArray);


			StopCoroutine ("MoveWithNewPath");

			this.moveDestination = moveDestination;

			if (pathPosList.Count == 0) {

				// 移动路径中没有点时，说明没有有效移动路径，此时终点设置为当前单步移动的终点
				this.moveDestination = singleMoveEndPos;

				moveTweener.OnComplete (() => {
					inSingleMoving = false;
					PlayRoleAnim ("wait", 0, null);
				});

				Debug.Log ("无有效路径");

				return false;
			}

			StartCoroutine ("MoveWithNewPath");
//			MoveWithNewPath ();

			return pathPosList.Count > 0;

		}

		/// <summary>
		/// 按照新路径移动
		/// </summary>
		/// <returns>The with new path.</returns>
		private IEnumerator MoveWithNewPath(){

			// 如果移动动画不为空，则等待当前移动结束
			if (moveTweener != null) {

				// 原来有移动动画，则将当步的动画结束回调改为只标记已走完，而不删除路径点（因为新的路径就是根据当步的结束点计算的，改点不在新路径内）
				moveTweener.OnComplete (() => {
					inSingleMoving = false;
				});

				yield return new WaitUntil (() => !inSingleMoving);

			} 

			// 移动到新路径上的下一个节点
			MoveToNextPosition ();

		}


		/// <summary>
		/// 匀速移动到指定节点位置
		/// </summary>
		/// <param name="targetPos">Target position.</param>
		private void MoveToPosition(Vector3 targetPos){

			exploreManager.GetComponent<ExploreManager>().ItemsAroundAutoIntoLifeWithBasePoint (targetPos);

//			Debug.Log (string.Format ("Player pos:[{0},{1}],target pos:[{2},{3}]", transform.position.x, transform.position.y,targetPos.x,targetPos.y));

			Vector3 moveVector = transform.position - targetPos;

			moveTweener =  transform.DOMove (targetPos, moveDuration).OnComplete (() => {

				// 动画结束时已经移动到指定节点位置，标记单步行动结束
				inSingleMoving = false;

				SetSortingOrder(-(int)transform.position.y);

				// 将当前节点从路径点中删除
				pathPosList.RemoveAt(0);

				// 移动到下一个节点位置
				MoveToNextPosition();
			});

			// 设置匀速移动
			moveTweener.SetEase (Ease.Linear);

			// 背景图片按照比例移动相应的位移

			Transform background = Camera.main.transform.Find ("Background");

			Vector3 backgroundImageTargetPos = background.localPosition + new Vector3 (moveVector.x * 0.3f, moveVector.y * 0.2f, 0);

//			Debug.LogFormat ("背景层移动目标位置[{0},{1}]", backgroundImageTargetPos.x, backgroundImageTargetPos.y);

			Tweener backgroundMoveTweener = background.DOLocalMove(backgroundImageTargetPos,moveDuration);

			backgroundMoveTweener.SetEase (Ease.Linear);


		}

		/// <summary>
		/// 判断当前是否已经走到了终点位置，位置度容差0.05
		/// </summary>
		/// <returns><c>true</c>, if end point was arrived, <c>false</c> otherwise.</returns>
		private bool ArriveEndPoint(){


			if(Mathf.Abs(transform.position.x - moveDestination.x) <= 0.05f &&
				Mathf.Abs(transform.position.y - moveDestination.y) <= 0.05f){
				return true;
			}

			return false;

		}
			

		public void ActiveBattlePlayer(bool forward,bool backward,bool side){

			playerForward.SetActive (forward);
			playerBackWard.SetActive (backward);
			playerSide.SetActive (side);

			if (forward) {
				modelActive = playerForward;
			} else if (backward) {
				modelActive = playerBackWard;
			} else if (side) {
				modelActive = playerSide;
			}
		}



		/// <summary>
		/// 移动到下一个节点
		/// </summary>
		private void MoveToNextPosition ()
		{
			Vector3 nextPos = Vector3.zero;

			boxCollider.enabled = true;

			if (pathPosList.Count > 0) {

				nextPos = pathPosList [0];

				if (ArriveEndPoint() && armatureCom.animation.lastAnimationName != "wait") {
					PlayRoleAnim ("wait", 0, null);
					return;
				}

				bool resetWalkAnim = false;

				if (Mathf.RoundToInt(nextPos.x) == Mathf.RoundToInt(transform.position.x)) {

					if (nextPos.y < transform.position.y) {
						if (modelActive != playerForward) {
							resetWalkAnim = true;
						}
						ActiveBattlePlayer (true, false, false);
					} else if (nextPos.y > transform.position.y) {
						if (modelActive != playerBackWard) {
							resetWalkAnim = true;
						}
						ActiveBattlePlayer (false, true, false);
					}

				}

				if(Mathf.RoundToInt(nextPos.y) == Mathf.RoundToInt(transform.position.y)){
					if (nextPos.x > transform.position.x) {
						if (modelActive != playerSide || (modelActive == playerSide && playerSide.transform.localScale != new Vector3 (1, 1, 1))) {
							resetWalkAnim = true;
						}
						playerSide.transform.localScale = new Vector3 (1, 1, 1);

					} else if (nextPos.x < transform.position.x) {
						if (modelActive != playerSide || (modelActive == playerSide && playerSide.transform.localScale != new Vector3 (-1, 1, 1))) {
							resetWalkAnim = true;
						}

						playerSide.transform.localScale = new Vector3 (-1, 1, 1);
					} 
					ActiveBattlePlayer (false, false, true);
				}

				if (isIdle){
					resetWalkAnim = true;
				}

				if (resetWalkAnim) {
					PlayRoleAnim ("walk", 0, null);
				} 
			}

			// 到达终点前的单步移动开始前进行碰撞检测
			// 1.如果碰撞体存在，则根据碰撞体类型给exploreManager发送消息执行指定回调
			// 2.如果未检测到碰撞体，则开始本次移动
			if (pathPosList.Count == 1) {

				Vector3 rayStartPos = (transform.position + pathPosList [0]) / 2;

				RaycastHit2D r2d = Physics2D.Linecast (rayStartPos, pathPosList [0], collosionLayer);


				if (r2d.transform != null) {

					switch (r2d.transform.tag) {

					case "monster":
						StopWalkBeforeEvent ();
						if (modelActive != playerSide) {
							ActiveBattlePlayer (false, false, true);
						}
						r2d.transform.localScale = new Vector3 (-modelActive.transform.localScale.x, 1, 1);
						enterMonster (r2d.transform);
						return;
					case "npc":
						StopWalkBeforeEvent ();
						enterNpc (r2d.transform);
						return;
//					case "workbench":
//						StopWalkBeforeEvent ();
//						enterWorkBench (r2d.transform);
//						return;
					case "crystal":
						StopWalkBeforeEvent ();
						enterCrystal (r2d.transform);
						return;
					case "billboard":
						StopWalkBeforeEvent ();
						enterBillboard (r2d.transform);
						return;
					case "movablefloor":
						break;
					case "firetrap":
						break;
					case "hole":
						StopWalkBeforeEvent ();
						enterHole (r2d.transform);
						return;
					case "movablebox":
						StopWalkBeforeEvent ();
						enterMovableBox (r2d.transform);
						return;
					case "obstacle":

						StopWalkBeforeEvent ();

						enterObstacle (r2d.transform);

						return;
					case "treasurebox":

						StopWalkBeforeEvent ();

						enterTreasureBox (r2d.transform);

						return;

					case "normalswitch":

						StopWalkBeforeEvent ();

						enterTrapSwitch (r2d.transform);

						return;
					case "normaltrap":
						break;
					case "transport":
						StopWalkBeforeEvent ();
						TransformManager.FindTransform ("ExploreManager").GetComponent<ExploreManager> ().EnterNextLevel ();
						return;
					case "door":
						StopWalkBeforeEvent ();
						enterDoor (r2d.transform);
						return;
					case "launcher":
						StopWalkBeforeEvent ();
						return;
					case "plant":
						StopWalkBeforeEvent ();
						enterPlant (r2d.transform);
						return;
					case "pressswitch":
						break;
					}

				}
			}
			// 路径中没有节点
			// 按照行动路径已经将所有的节点走完
			if (pathPosList.Count == 0) {

				// 走到了终点
				if (ArriveEndPoint ()) {
					moveTweener.Kill (true);
					PlayRoleAnim ("wait", 0, null);
					Debug.Log ("到达终点");
				} else {
					Debug.Log (string.Format("actual pos:{0}/ntarget pos:{1},predicat pos{2}",transform.position,moveDestination,singleMoveEndPos));
					throw new System.Exception ("路径走完但是未到终点");
				}
				return;
			}

			// 如果还没有走到终点
			if (!ArriveEndPoint ()) {

				SoundManager.Instance.PlayAudioClip ("MapEffects/footstep");

				// 记录下一节点位置
				singleMoveEndPos = nextPos;

				// 向下一节点移动
				MoveToPosition (nextPos);

				// 标记单步移动中
				inSingleMoving = true;

			} else {
				moveTweener.Kill (true);
				PlayRoleAnim ("wait", 0, null);
			}

		}



		public void StopMoveAtEndOfCurrentStep(){
			this.moveDestination = pathPosList [0];
			pathPosList = new List<Vector3>{ moveDestination };
		}


		private void StopWalkBeforeEvent(){

			moveTweener.Kill (false);

			PlayRoleAnim ("wait", 0, null);

			singleMoveEndPos = transform.position;

			inSingleMoving = false;

		}

		public void StopMove(){
			StopCoroutine ("MoveWithNewPath");
			moveTweener.Kill (false);
			inSingleMoving = false;
			PlayRoleAnim ("wait", 0, null);
		}

		public override void TowardsLeft(){
			ActiveBattlePlayer (false, false, true);
			PlayRoleAnim ("wait", 0, null);
			playerSide.transform.localScale = new Vector3 (-1, 1, 1);
		}

		public override void TowardsRight(){
			ActiveBattlePlayer (false, false, true);
			PlayRoleAnim ("wait", 0, null);
			playerSide.transform.localScale = Vector3.one;
		}
			
		/// <summary>
		/// 战斗结束之后玩家移动到怪物原来的位置
		/// </summary>
		public void PlayerMoveToEnemyPosAfterFight(){

			PlayRoleAnim ("wait", 0, null);

			Vector3 targetPos = pathPosList [0];

			// 玩家角色位置和原来的怪物位置之间间距大于0.5（玩家是横向进入战斗的），则播放跑的动画到指定位置
			if (Mathf.Abs (targetPos.x - transform.position.x) > 0.5) {
				MoveToNextPosition ();

			} else {// 玩家角色位置和原来的怪物位置之间间距小于0.5（玩家是纵向进入战斗的），则角色直接移动到指定位置，不播动画

				transform.position = targetPos;

				singleMoveEndPos = targetPos;

				pathPosList.Clear ();

				exploreManager.GetComponent<ExploreManager>().ItemsAroundAutoIntoLifeWithBasePoint (targetPos);

			}

		}



		public override void InitFightTextDirectionTowards (Vector3 position)
		{
			bpUICtr.fightTextManager.SetUpFightTextManager (transform.position, position);
		}

		public override void AddFightTextToQueue (string text, SpecialAttackResult specialAttackType)
		{
			if (bpUICtr != null) {
				FightText ft = new FightText (text, specialAttackType);
				bpUICtr.fightTextManager.AddFightText (ft);
			}
		}


		/// <summary>
		/// Starts the fight.
		/// </summary>
		/// <param name="bmCtr">怪物控制器</param>
		/// <param name="playerLoseCallBack"> 玩家战斗失败回调 </param>
		public void StartFight(BattleMonsterController bmCtr,CallBack playerLoseCallBack){

			this.bmCtr = bmCtr;

			this.playerLoseCallBack = playerLoseCallBack;

			// 初始化玩家战斗UI（技能界面）
//			bpUICtr.SetUpPlayerSkillPlane (agent as Player);

			if (autoFight) {
				// 默认玩家在战斗中将先出招，首次攻击不用等待
				currentSkill = InteligentAttackSkill();
				UseSkill (currentSkill);
			} else {
				PlayRoleAnim ("wait", 0, null);
			}

		}

		public bool autoFight = false;


		/// <summary>
		/// 角色默认战斗逻辑
		/// </summary>
		public override void Fight(){
			if (!autoFight) {
				PlayRoleAnim ("wait", 0, null);
			} else {
				currentSkill = InteligentAttackSkill ();
				attackCoroutine = InvokeAttack (currentSkill);
			}
		}

		public void UseDefaultSkill(){
			currentSkill = InteligentAttackSkill ();
			UseSkill (currentSkill);
		}

		/// <summary>
		/// 使用技能
		/// </summary>
		/// <param name="skill">Skill.</param>
		protected override void UseSkill (ActiveSkill skill)
		{
			isAttackActionFinish = false;

			// 播放技能对应的角色动画，角色动画结束后播放攻击间隔动画
			this.PlayRoleAnim (skill.selfRoleAnimName, 1, () => {
				// 播放等待动画
				this.PlayRoleAnim("interval",0,null);
			});

		}
			

		protected override void AgentExcuteHitEffect ()
		{
			// 播放技能对应的玩家技能特效动画
			if (currentSkill.selfEffectAnimName != string.Empty) {
				SetEffectAnim (currentSkill.selfEffectAnimName);
			}

			// 播放技能对应的怪物技能特效动画
			if (currentSkill.enemyEffectAnimName != string.Empty) {
				bmCtr.SetEffectAnim (currentSkill.enemyEffectAnimName);
			}

			// 播放技能对应的音效
			SoundManager.Instance.PlayAudioClip("Skill/" + currentSkill.sfxName);


			// 技能效果影响玩家和怪物
			currentSkill.AffectAgents(this,bmCtr);

			isAttackActionFinish = true;

			// 如果战斗没有结束，则默认在攻击间隔时间之后按照默认攻击方式进行攻击
			if (!FightEnd ()) {
				if (autoFight) {
					currentSkill = InteligentAttackSkill ();
					attackCoroutine = InvokeAttack (currentSkill);
					StartCoroutine (attackCoroutine);
				} else {
//					TransformManager.FindTransform("ExploreCanvas").GetComponent<ExploreUICotroller> ().ResetAttackCheckPosition ();
				}

			} 

		}


		/// <summary>
		/// 判断本次战斗是否结束,如果怪物死亡则执行怪物死亡对应的方法
		/// </summary>
		/// <returns><c>true</c>, if end was fought, <c>false</c> otherwise.</returns>
		private bool FightEnd(){

			if (bmCtr.agent.health <= 0) {
				bmCtr.AgentDie ();
				agent.ResetBattleAgentProperties (false);
				isInFight = false;
				return true;
			} else if (agent.health <= 0) {
				isInFight = false;
				agent.ResetBattleAgentProperties (true);
				exploreManager.GetComponent<ExploreManager> ().QuitExploreScene (false);
				return true;
			}else {
				return false;
			}

		}
			


		/// <summary>
		/// 更新玩家状态栏
		/// </summary>
		public override void UpdateStatusPlane(){
			if (bpUICtr != null) {
				bpUICtr.UpdateAgentStatusPlane ();
			}
		}


//		private IEnumerator ConsumablesEffectOn(ConsumablesEffectState consumablesEffectState){
//
//			Player player = agent as Player;
//
//			Consumables consumables = consumablesEffectState.consumables;
//
//			int healthGain = (int)(consumables.healthGain * player.maxHealth / consumables.effectDuration);
//			int manaGain = (int)(consumables.manaGain * player.maxMana / consumables.effectDuration);
//
//			player.attack = (int)(player.attack * (1 + consumables.attackGain));
//			player.attackSpeed = (int)(player.attackSpeed * (1 + consumables.attackSpeedGain));
//			player.armor = (int)(player.armor * (1 + consumables.armorGain));
//			player.manaResist = (int)(player.manaResist * (1 + consumables.manaResistGain));
//			player.dodge = (int)(player.dodge * (1 + consumables.dodgeGain));
//			player.crit = (int)(player.crit * (1 + consumables.critGain));
//
//			player.physicalHurtScaler = 1 + consumables.physicalHurtScaler;
//			player.magicalHurtScaler = 1 + consumables.magicHurtScaler;
//
//			bpUICtr.UpdatePlayerStatusPlane ();
//
//			while (consumablesEffectState.effectTime < consumables.effectDuration) {
//				yield return new WaitForSeconds (1.0f);
//				consumablesEffectState.effectTime++;
//				player.health += healthGain;
//				player.mana += manaGain;
////				if (player.health >= player.maxHealth) {
////					player.health = player.maxHealth;
////				}
////				if (player.mana >= player.maxMana) {
////					player.mana = player.maxMana;
////				}
//
//				bpUICtr.UpdatePlayerStatusPlane ();
//			}
//
//			player.ResetBattleAgentProperties (false);
//			player.physicalHurtScaler -= consumables.physicalHurtScaler;
//			player.magicalHurtScaler -= consumables.magicHurtScaler;
//
//		}

		/// <summary>
		/// 清理引用
		/// </summary>
		public void ClearReference(){


			enterMonster = null;
			enterNpc = null;
//			enterWorkBench = null;
			enterCrystal = null;
			enterTreasureBox = null;
			enterObstacle = null;
			enterTrapSwitch = null;
			enterDoor = null;
			enterBillboard = null;
			enterHole = null;
			enterMovableBox = null;
			enterTransport = null;
			enterPlant = null;

			ClearAllEffectStatesAndSkillCallBacks ();

//			trapTriggered = null;
			bmCtr = null;

			playerLoseCallBack = null;

			bpUICtr = null;

		}


		public void QuitExplore(){

//			CollectSkillEffectsToPool ();

			ClearReference ();

			gameObject.SetActive (false);

		}


		/// <summary>
		/// 玩家死亡
		/// </summary>
		override public void AgentDie(){

			StopCoroutinesWhenFightEnd ();

			bmCtr.StopCoroutinesWhenFightEnd ();

//			exploreManager.GetComponent<MapGenerator> ().PlayMapOtherAnim ("Death", transform.position);
				
			PlayRoleAnim("die", 1, () => {

				playerLoseCallBack ();

//				CollectSkillEffectsToPool();

				gameObject.SetActive(false);


			});


		}

	}
}
