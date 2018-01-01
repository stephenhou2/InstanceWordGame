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

		// 普通攻击技能（角色自带）
		public Skill defaultSkill;

		// 事件回调
		public ExploreEventHandler enterMonster;
		public ExploreEventHandler enterNpc;
		public ExploreEventHandler enterWorkBench;
		public ExploreEventHandler enterCrystal;
		public ExploreEventHandler enterTreasureBox;
		public ExploreEventHandler enterObstacle;
		public ExploreEventHandler enterTrapSwitch;
		public ExploreEventHandler enterDoor;
		public ExploreEventHandler enterBillboard;
		public ExploreEventHandler enterHole;
		public ExploreEventHandler enterMovableBox;
		public ExploreEventHandler enterTransport;

		// 移动速度
		public float moveDuration;			

		// 当前的单步移动动画对象
		private Tweener moveTweener;

		// 标记是否在单步移动中
		private bool inSingleMoving;

		// 移动路径点集
		private List<Vector3> pathPosList;

		// 正在前往的节点位置
		public Vector3 singleMoveEndPos;

		// 移动的终点位置
		private Vector3 endPos;

		// 玩家UI控制器
		private BattlePlayerUIController bpUICtr;

		// 当前碰到的怪物控制器
		private BattleMonsterController bmCtr;

		// 玩家战斗失败回调
		private CallBack playerLoseCallBack;

//		public Trap trapTriggered;


		protected override void Awake(){

			ActiveBattlePlayer (false, false, false);

			agent = GetComponentInParent<Player> ();

			base.Awake ();

		}



		/// <summary>
		/// 按照指定路径 pathPosList 移动到终点 endPos
		/// </summary>
		/// <param name="pathPosList">Path position list.</param>
		/// <param name="endPos">End position.</param>
		public void MoveToEndByPath(List<Vector3> pathPosList,Vector3 endPos){



			StopCoroutine ("MoveWithNewPath");

			this.pathPosList = pathPosList;

//			for (int i = 0; i < pathPosList.Count; i++) {
//				Debug.LogFormat ("[{0},{1}]", pathPosList [i].x,pathPosList[i].y);
//			}

			this.endPos = endPos;

			if (pathPosList.Count == 0) {

				// 移动路径中没有点时，说明没有有效移动路径，此时终点设置为当前单步移动的终点
				this.endPos = singleMoveEndPos;

				moveTweener.OnComplete (() => {
					inSingleMoving = false;
					PlayRoleAnim ("wait", 0, null);
				});
					
				Debug.Log ("无有效路径");

				return;
			}

			StartCoroutine ("MoveWithNewPath");

		}

		/// <summary>
		/// 按照新路径移动
		/// </summary>
		/// <returns>The with new path.</returns>
		private IEnumerator MoveWithNewPath(){

			// 如果移动动画不为空，则等待当前移动结束
			if (moveTweener != null) {

				// 动画结束时标记isSingleMoving为false（单步行动结束），不删除路径点（因为该单步行动是旧路径上的行动点）
				moveTweener.OnComplete (() => {
					inSingleMoving = false;
				});

				// 等待单步行动结束
				while (inSingleMoving) {
					yield return null;
				}

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

			SetSortingOrder(-(int)targetPos.y);

			moveTweener =  transform.DOMove (targetPos, moveDuration).OnComplete (() => {

				// 动画结束时已经移动到指定节点位置，标记单步行动结束
				inSingleMoving = false;

				// 将当前节点从路径点中删除
				pathPosList.RemoveAt(0);

//				if(trapTriggered != null && !trapTriggered.transform.position.Equals(singleMoveEndPos)){
//					trapTriggered.GetComponent<BoxCollider2D>().enabled = true;
//					trapTriggered = null;
//				}

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

			if(Mathf.Abs(transform.position.x - endPos.x) <= 0.05f &&
				Mathf.Abs(transform.position.y - endPos.y) <= 0.05f){
				return true;
			}

			return false;

		}

		/// <summary>
		/// 继续当前未完成的单步移动
		/// </summary>
		public void ContinueMove(){

			MoveToNextPosition ();

			boxCollider.enabled = true;

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

			if (pathPosList.Count > 0) {
				
				nextPos = pathPosList [0];

				if (nextPos.x == transform.position.x && nextPos.y == transform.position.y) {
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

				// 禁用自身包围盒（否则射线检测时监测到的是自己的包围盒）
				boxCollider.enabled = false;

				RaycastHit2D r2d = Physics2D.Linecast (transform.position, pathPosList [0], collosionLayer);


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

						PlayRoleAnim ("wait", 0, null);

						enterNpc (r2d.transform);

						boxCollider.enabled = true;

						return;

					case "workbench":

						StopWalkBeforeEvent ();

						PlayRoleAnim ("wait", 0, null);

						enterWorkBench (r2d.transform);

						boxCollider.enabled = true;

						return;
					case "crystal":

						StopWalkBeforeEvent ();
						
						PlayRoleAnim ("wait", 0, null);

						enterCrystal (r2d.transform);

						boxCollider.enabled = true;

						return;
					case "billboard":

						StopWalkBeforeEvent ();

						PlayRoleAnim ("wait", 0, null);

						enterBillboard (r2d.transform);

						boxCollider.enabled = true;

						return;
					case "movablefloor":
						break;
					case "firetrap":
						break;
					case "hole":
						StopWalkBeforeEvent ();

						PlayRoleAnim ("wait", 0, null);

						enterHole (r2d.transform);
						return;
					case "movablebox":
						StopWalkBeforeEvent ();
						PlayRoleAnim ("wait", 0, null);
						enterMovableBox (r2d.transform);
						return;
					case "obstacle":

						StopWalkBeforeEvent ();

						PlayRoleAnim ("wait", 0, null);

						enterObstacle (r2d.transform);

						boxCollider.enabled = true;

						return;
					case "treasurebox":

						StopWalkBeforeEvent ();

						PlayRoleAnim ("wait", 0, null);

						enterTreasureBox (r2d.transform);

						boxCollider.enabled = true;

						return;
					
					case "switch":

						StopWalkBeforeEvent ();

						PlayRoleAnim ("wait", 0, null);

						enterTrapSwitch (r2d.transform);

						boxCollider.enabled = true;

						return;
					case "trap":
						break;
					case "transport":
						StopWalkBeforeEvent ();
						PlayRoleAnim ("wait", 0, null);

						boxCollider.enabled = true;
						return;
					case "door":
						StopWalkBeforeEvent ();
						PlayRoleAnim ("wait", 0, null);
						enterDoor (r2d.transform);
						return;
					case "launcher":
						StopWalkBeforeEvent ();
						PlayRoleAnim ("wait", 0, null);
						boxCollider.enabled = true;
						return;
					}

				}

				boxCollider.enabled = true;

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
					Debug.Log (string.Format("actual pos:{0}/ntarget pos:{1},predicat pos{2}",transform.position,endPos,singleMoveEndPos));
					throw new System.Exception ("路径走完但是未到终点");
				}
				return;
			}

			// 如果还没有走到终点
			if (!ArriveEndPoint ()) {

				GameManager.Instance.soundManager.PlayFootStepClips ();

//				GameManager.Instance.soundManager.PlayClips (
//					GameManager.Instance.gameDataCenter.allExploreAudioClips,
//					SoundDetailTypeName.Steps, 
//					null);

				// 记录下一节点位置
				singleMoveEndPos = nextPos;

				// 向下一节点移动
				MoveToPosition (nextPos);


				// 标记单步移动中
				inSingleMoving = true;

				return;

			}

		}

		private void StopWalkBeforeEvent(){

			moveTweener.Kill (false);

			singleMoveEndPos = transform.position;

			inSingleMoving = false;

		}

		public void StopMove(){
			StopCoroutine ("MoveWithNewPath");
//			endPos = new Vector3 (Mathf.RoundToInt(transform.position.x),
//				Mathf.RoundToInt(transform.position.y),
//				Mathf.RoundToInt(transform.position.z));
			moveTweener.Kill (false);
			inSingleMoving = false;
			PlayRoleAnim ("wait", 0, null);
		}

//		private void FinishCurrentStepAndWait(){
//			StartCoroutine ("WaitCurrentStepFinishAndPlayWaitAnim");
//		}
//
//		private IEnumerator WaitCurrentStepFinishAndPlayWaitAnim(){
//			yield return new WaitUntil (() => inSingleMoving);
//			StopWalkBeforeEvent ();
//			PlayRoleAnim ("wait", 0, null);
//		}

		public void TowardsLeft(){
			if (modelActive == playerSide) {
				playerSide.transform.localScale = new Vector3 (-1, 1, 1);
			}
		}

		public void TowardsRight(){
			if (modelActive == playerSide) {
				playerSide.transform.localScale = Vector3.one;
			}
		}
			

		/// <summary>
		///  初始化探索界面中的玩家UI
		/// </summary>
		public void SetUpExplorePlayerUI(){

			Transform canvas = TransformManager.FindTransform ("ExploreCanvas");

			bpUICtr = canvas.GetComponent<BattlePlayerUIController> ();

			#warning 这里技能选择回调暂时传了null
			bpUICtr.SetUpExplorePlayerView (agent as Player, null);

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

//		public override void ShowFightTextInOrder ()
//		{
//			bpUICtr.fightTextManager.ShowFightTextInOrder ();
//		}


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
				UseSkill (defaultSkill);
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
				attackCoroutine = InvokeAttack (defaultSkill);
			}
		}

		public void UseDefaultSkill(){
			UseSkill (defaultSkill);
		}

		/// <summary>
		/// 使用技能
		/// </summary>
		/// <param name="skill">Skill.</param>
		protected override void UseSkill (Skill skill)
		{
			// 停止播放当前的等待动画
			this.armatureCom.animation.Stop ();

			currentSkill = skill;

			string skillRoleAnimName = string.Empty;
			string skillIntervalRoleAnimName = string.Empty;

			skillRoleAnimName = "attackSword";
			skillIntervalRoleAnimName = "intervalSword";

//			if (skill.selfAnimName == "AttackOnce") {
//
//				Equipment weapon = agent.allEquipedEquipments.Find (delegate(Equipment obj) {
//					return obj.equipmentType == EquipmentType.Weapon;
//				});
//
//				if (weapon == null) {
//					skillRoleAnimName = "attackDefault";
//					skillIntervalRoleAnimName = "intervalDefault";
//				} else {
//					switch (weapon.detailType) {
//					case "剑":
//						skillRoleAnimName = "attackSword";
//						skillIntervalRoleAnimName = "intervalSword";
//						break;
//					case "刀":
//						skillRoleAnimName = "attackBlade";
//						skillIntervalRoleAnimName = "intervalBlade";
//						break;
//					case "斧子":
//						skillRoleAnimName = "attackAxe";
//						skillIntervalRoleAnimName = "intervalAxe";
//						break;
//					case "锤":
//						skillRoleAnimName = "attackHammer";
//						skillIntervalRoleAnimName = "intervalHammer";
//						break;
//					case "法杖":
//						skillRoleAnimName = "attackStaff";
//						skillIntervalRoleAnimName = "intervalStaff";
//						break;
//					case "匕首":
//						skillRoleAnimName = "attackDragger";
//						skillIntervalRoleAnimName = "intervalDragger";
//						break;
//					}
//				}
//
//			} else {
//				skillRoleAnimName = skill.selfAnimName;
//				skillIntervalRoleAnimName = skill.selfIntervalAnimName;
//			}



			// 播放技能对应的角色动画，角色动画结束后播放技能特效动画，实现技能效果并更新角色状态栏
			this.PlayRoleAnim (skillRoleAnimName, 1, () => {
				// 播放等待动画
				this.PlayRoleAnim(skillIntervalRoleAnimName,0,null);
			});

		}
			

		/// <summary>
		/// 玩家选择技能后的响应方法
		/// </summary>
		/// <param name="btnIndex">Button index.</param>
//		public void PlayerSelectSkill(int[] btnIndexArray){
//
//			int btnIndex = btnIndexArray [0];
//
//			// 获得选择的技能
//			Skill skill = (agent as Player).validSkills [btnIndex];
//
//			// 停止自动攻击的协程
//			if (attackCoroutine != null) {
//				StopCoroutine (attackCoroutine);
//			}
//
//			UseSkill (skill);
//		}



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
			GameManager.Instance.soundManager.PlaySkillEffectClips(currentSkill.sfxName);

//			GameManager.Instance.soundManager.PlayClips (
//				GameManager.Instance.gameDataCenter.allExploreAudioClips, 
//				SoundDetailTypeName.Skill, 
//				currentSkill.sfxName);
			

			// 技能效果影响玩家和怪物
			currentSkill.AffectAgents(this,bmCtr);

			// 如果战斗没有结束，则默认在攻击间隔时间之后按照默认攻击方式进行攻击
			if (!FightEnd ()) {
				if (autoFight) {
					attackCoroutine = InvokeAttack (defaultSkill);
					StartCoroutine (attackCoroutine);
				} else {
//					TransformManager.FindTransform("ExploreCanvas").GetComponent<ExploreUICotroller> ().ResetAttackCheckPosition ();
				}

			} 

			// 更新玩家状态栏
//			this.UpdateStatusPlane();
//
//			// 更新怪物状态栏
//			bmCtr.UpdateStatusPlane();

			// 如果该次攻击是物理攻击，对应减少当前武器的耐久度
//			switch (currentSkill.hurtType) {
//			case HurtType.Physical:
//
//				Equipment equipment = agent.allEquipedEquipments.Find (delegate(Equipment obj) {
//					return obj.equipmentType == EquipmentType.Weapon;
//				});
//
//				if (equipment == null) {
//					break;
//				}
//
//				bool completeDamaged = equipment.EquipmentDamaged (EquipmentDamageSource.PhysicalAttack);
//
//				if (completeDamaged) {
//					string tint = string.Format("{0}完全损坏",equipment.itemName);
//					bpUICtr.GetComponent<ExploreUICotroller>().SetUpTintHUD(tint);
//				}
//				break;
//			default:
//				break;
//			}

		}


		/// <summary>
		/// 判断本次战斗是否结束,如果怪物死亡则执行怪物死亡对应的方法
		/// </summary>
		/// <returns><c>true</c>, if end was fought, <c>false</c> otherwise.</returns>
		private bool FightEnd(){

			if (bmCtr.agent.health <= 0) {
				bmCtr.AgentDie ();
				agent.ResetBattleAgentProperties ();
				return true;
			} else if (agent.health <= 0) {
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
				bpUICtr.UpdatePlayerStatusPlane ();
			}
			Transform bagCanvas = TransformManager.FindTransform ("BagCanvas");
//			if (bagCanvas != null) {
//				bagCanvas.GetComponent<BagView>().SetUpPlayerStatusPlane(
//			}
		}


		public void UseItem(Consumables consumables){

			return;

//			Player player = agent as Player;

//			switch (consumables.consumablesType) {
//			case ConsumablesType.MedicineAndScroll:
//				if (consumables.effectDuration == 0) {
//					player.health += (int)(consumables.healthGain * agent.maxHealth);
//					player.mana += (int)(consumables.manaGain * agent.maxMana);
//				} else {
//					ConsumablesEffectState consumablesEffectState = player.allConsumablesEffectStates.Find (delegate(ConsumablesEffectState obj) {
//						return obj.consumables.itemId == consumables.itemId;
//					});
//					if (consumablesEffectState == null) {
//						consumablesEffectState = new ConsumablesEffectState (consumables);
//						player.allConsumablesEffectStates.Add (consumablesEffectState);
//						StartCoroutine ("ConsumablesEffectOn", consumablesEffectState);
//					}
//				}
//				break;
//			}
//
//			consumables.itemCount--;
//
//			if (consumables.itemCount <= 0) {
//				player.allConsumablesInBag.Remove (consumables);
//			}
//
//			bpUICtr.UpdateItemButtonsAndStatusPlane ();

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
			enterWorkBench = null;
			enterCrystal = null;
			enterTreasureBox = null;
			enterObstacle = null;
			enterTrapSwitch = null;
			enterDoor = null;
			enterBillboard = null;
			enterHole = null;
			enterMovableBox = null;
			enterTransport = null;

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



			StopAllCoroutines ();

			// 停止怪物的攻击
			if (bmCtr.attackCoroutine != null) {
				bmCtr.StopCoroutine (bmCtr.attackCoroutine);
			}

			if (bmCtr.waitRoleAnimEndCoroutine != null) {
				StopCoroutine (bmCtr.waitRoleAnimEndCoroutine);
			}

			exploreManager.GetComponent<MapGenerator> ().PlayMapOtherAnim ("Death", transform.position);
				
			PlayRoleAnim("die", 1, () => {

				playerLoseCallBack ();

//				CollectSkillEffectsToPool();

				gameObject.SetActive(false);


			});


		}

	}
}
