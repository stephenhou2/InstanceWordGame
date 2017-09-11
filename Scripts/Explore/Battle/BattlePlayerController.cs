﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;



namespace WordJourney
{
	public class BattlePlayerController : BattleAgentController {

		// 事件回调
		public ExploreEventHandler enterMonster;
		public ExploreEventHandler enterItem;
		public ExploreEventHandler enterNpc;



		// 移动速度
		public float moveDuration = 0.5f;			

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



		protected override void Awake(){

			agent = GetComponentInParent<Player> ();

			physicalAttack.selfAnimName = "battle";

			base.Awake ();

		}

		/// <summary>
		/// 按照指定路径 pathPosList 移动到终点 endPos
		/// </summary>
		/// <param name="pathPosList">Path position list.</param>
		/// <param name="endPos">End position.</param>
		public void MoveToEndByPath(List<Vector3> pathPosList,Vector3 endPos){

			this.pathPosList = pathPosList;

			this.endPos = endPos;


			if (pathPosList.Count == 0) {

				// 移动路径中没有点时，说明没有有效移动路径，此时终点设置为当前单步移动的终点
				this.endPos = singleMoveEndPos;

				moveTweener.OnComplete (() => {
					inSingleMoving = false;

				});

				PlayRoleAnim ("stand", 0, null);

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

//			Debug.Log (string.Format ("Player pos:[{0},{1}],target pos:[{2},{3}]", transform.position.x, transform.position.y,targetPos.x,targetPos.y));

			moveTweener =  transform.DOMove (targetPos, moveDuration).OnComplete (() => {

				// 动画结束时已经移动到指定节点位置，标记单步行动结束
				inSingleMoving = false;

				// 将当前节点从路径点中删除
				pathPosList.RemoveAt(0);

				// 移动到下一个节点位置
				MoveToNextPosition();
			});


			// 设置匀速移动
			moveTweener.SetEase (Ease.Linear);

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
//		public void ContinueMove(){
//
//			MoveToNextPosition ();
//
//			boxCollider.enabled = true;
//
//		}

		/// <summary>
		/// 移动到下一个节点
		/// </summary>
		private void MoveToNextPosition ()
		{
			Vector3 nextPos = Vector3.zero;
			if (pathPosList.Count > 0) {
				
				nextPos = pathPosList [0];

				if (nextPos.x > transform.position.x) {
					transform.localScale = new Vector3 (1, 1, 1);
				} else if (nextPos.x < transform.position.x) {
					transform.localScale = new Vector3 (-1, 1, 1);
				}

			}

			// 到达终点前的单步移动开始前进行碰撞检测
			// 1.如果碰撞体存在，则根据碰撞体类型给exploreManager发送消息执行指定回调
			// 2.如果未检测到碰撞体，则开始本次移动
			if (pathPosList.Count == 1) {

				// 禁用自身包围盒
				boxCollider.enabled = false;

				RaycastHit2D r2d = Physics2D.Linecast (transform.position, pathPosList [0], collosionLayer);

				if (r2d.transform != null) {

					switch (r2d.transform.tag) {

					case "monster":

						moveTweener.Kill(false);

						enterMonster (r2d.transform);

						return;

					case "item":

						moveTweener.Kill (false);

						PlayRoleAnim ("stand", 0, null);

						singleMoveEndPos = transform.position;

						inSingleMoving = false;

						enterItem (r2d.transform);

						boxCollider.enabled = true;

						return;

					case "npc":

						PlayRoleAnim ("stand", 0, null);

						moveTweener.Kill (false);

						singleMoveEndPos = transform.position;

						inSingleMoving = false;

						enterNpc (r2d.transform);

						boxCollider.enabled = true;

						return;

					default:
						boxCollider.enabled = true;
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
					PlayRoleAnim ("stand", 0, null);
					Debug.Log ("到达终点");
				} else {
					Debug.Log (string.Format("actual pos:{0}/ntarget pos:{1},predicat pos{2}",transform.position,endPos,singleMoveEndPos));
					throw new System.Exception ("路径走完但是未到终点");
				}
				return;
			}

			// 如果还没有走到终点
			if (!ArriveEndPoint ()) {

				// 记录下一节点位置
				singleMoveEndPos = nextPos;

				// 向下一节点移动
				MoveToPosition (nextPos);

				PlayRoleAnim ("walk", 0, null);

				// 标记单步移动中
				inSingleMoving = true;

				return;

			}

		}

		/// <summary>
		///  初始化探索界面中的玩家UI
		/// </summary>
		public void SetUpExplorePlayerUI(){

			Transform canvas = TransformManager.FindTransform ("ExploreCanvas");

			bpUICtr = canvas.GetComponent<BattlePlayerUIController> ();

			bpUICtr.SetUpExplorePlayerView (agent as Player, GameManager.Instance.allSkillSprites, GameManager.Instance.allItemSprites,PlayerSelectSkill);

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
			bpUICtr.SetUpPlayerSkillPlane (agent as Player);

			// 开始战斗
			Fight();

		}

		/// <summary>
		/// 角色战斗逻辑
		/// </summary>
		public override void Fight(){
			UseSkill(physicalAttack);
		}


		/// <summary>
		/// 伤害文本动画
		/// </summary>
		/// <param name="hurtStr">Hurt string.</param>
		/// <param name="tintTextType">伤害类型 [TintTextType.Crit:暴击伤害 ,TintTextType.Miss: 伤害闪避]</param>
		public override void PlayHurtTextAnim (string hurtStr, TintTextType tintTextType)
		{
			// 伤害文字的动画方向根据玩家和怪物的位置信息进行调整
			if (this.transform.position.x <= bmCtr.transform.position.x) {
				bpUICtr.PlayHurtTextAnim (hurtStr, this.transform.position, Towards.Left, tintTextType);
			} else {
				bpUICtr.PlayHurtTextAnim (hurtStr, this.transform.position, Towards.Right, tintTextType);
			}


		}

		/// <summary>
		/// 血量提升文本动画
		/// </summary>
		/// <param name="gainStr">Gain string.</param>
		public override void PlayGainTextAnim (string gainStr)
		{
			bpUICtr.PlayGainTextAnim (gainStr,this.transform.position);
		}
			

		/// <summary>
		/// 玩家选择技能后的响应方法
		/// </summary>
		/// <param name="btnIndex">Button index.</param>
		public void PlayerSelectSkill(int[] btnIndexArray){

			int btnIndex = btnIndexArray [0];

			Skill skill = agent.equipedSkills [btnIndex];

			StopCoroutine ("InvokeAttack");

			UseSkill (skill);
		}

		/// <summary>
		/// 使用技能
		/// </summary>
		/// <param name="skill">Skill.</param>
		protected override void UseSkill (Skill skill)
		{
			// 技能对应的角色动画，动画结束后执行技能效果并更新角色状态栏
			this.PlayRoleAnim (skill.selfAnimName, 1, () => {
				skill.AffectAgents(this,bmCtr);
				this.UpdatePlayerStatusPlane();
				bmCtr.UpdateMonsterStatusPlane();
				if(!FightEnd()){
					StopCoroutine("InvokeAttack");
					StartCoroutine("InvokeAttack",physicalAttack);
				}
			});
				
			// 播放技能对应的怪物动画
			if (skill.enemyAnimName != string.Empty) {
				bmCtr.PlayRoleAnim (skill.enemyAnimName, 1, null);
			}

			// 播放技能对应的玩家技能特效动画
			if (skill.selfEffectName != string.Empty) {
				SetEffectAnim (skill.selfEffectName);
			}

			// 播放技能对应的怪物技能特效动画
			if (skill.enemyEffectName != string.Empty) {
				bmCtr.SetEffectAnim (skill.enemyEffectName);
			}
		}

		/// <summary>
		/// 判断本次战斗是否结束,如果怪物死亡则执行怪物死亡对应的方法
		/// </summary>
		/// <returns><c>true</c>, if end was fought, <c>false</c> otherwise.</returns>
		private bool FightEnd(){

			if (bmCtr.agent.health <= 0) {
				bmCtr.MonsterDie ();
				agent.ResetBattleAgentProperties ();
				return true;
			} else if (agent.health <= 0) {
				return true;
			}else {
				return false;
			}

		}
			
		/// <summary>
		/// 玩家死亡
		/// </summary>
		public void PlayerDie(){

			// 停止玩家和怪物的攻击
			this.StopCoroutine ("InvokeAttack");
			bmCtr.StopCoroutine ("InvokeAttack");

//			playerLoseCallBack ();
//			bpUICtr.GetComponent<ExploreUICotroller> ().QuitFight ();
//			gameObject.SetActive(false);

			ExploreUICotroller expUICtr = bpUICtr.GetComponent<ExploreUICotroller> ();

			expUICtr.HideFightPlane ();

			PlayRoleAnim("die", 1, () => {
				playerLoseCallBack ();
				expUICtr.QuitFight ();
				gameObject.SetActive(false);
			});


		}

		/// <summary>
		/// 更新玩家状态栏
		/// </summary>
		public void UpdatePlayerStatusPlane(){
			bpUICtr.UpdatePlayerStatusPlane ();
		}


		public void OnItemButtonClick(int ButtonIndex){

			Item item = null;

			switch (ButtonIndex) {
			case 0:
				item = agent.allItems.Find (delegate (Item obj) {
					return obj.itemNameInEnglish == "health";
				});
				break;
			case 1:
				item = agent.allItems.Find (delegate (Item obj) {
					return obj.itemNameInEnglish == "mana";
				});
				break;
			case 2:
				item = agent.allItems.Find (delegate (Item obj) {
					return obj.itemNameInEnglish == "antiDebuff";
				});
				break;
			default:
				break;

			}


			if (item == null) {
				return;
			}

			agent.health += (item as Consumables).healthGain;
			agent.mana += (item as Consumables).manaGain;


			item.itemCount--;

			if (item.itemCount <= 0) {
				agent.allItems.Remove (item);
			}

			bpUICtr.UpdateItemButtonsAndStatusPlane ();

		}


	}
}