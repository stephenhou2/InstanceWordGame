using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	using DragonBones;
	using Transform = UnityEngine.Transform;
		
	public class BattleMonsterController : BattleAgentController {

		// 怪物UI控制器
		private BattleMonsterUIController bmUICtr{
			get{
				Transform canvas = TransformManager.FindTransform ("ExploreCanvas");

				return canvas.GetComponent<BattleMonsterUIController> ();
			}
		}

		// 玩家控制器
		private BattlePlayerController bpCtr;

		// 玩家战斗胜利回调
		private CallBack<Transform> playerWinCallBack;

		public Vector3 originalPos;


		protected override void Awake(){

			agent = GetComponent<Monster> ();

			modelActive = this.gameObject;

			base.Awake ();

		}

		public void KillRoleAnim(){
			armatureCom.animation.Stop ();
		}

		public void SetAlive(){
//			UnityFactory.factory.
			boxCollider.enabled = true;
			PlayRoleAnim ("wait", 0, null);
			SetSortingOrder (-(int)transform.position.y);
			originalPos = transform.position;
		}


		/// <summary>
		/// 初始化碰到的怪物
		/// </summary>
		public void InitMonster(Transform monsterTrans){

			Monster monster = agent as Monster;

			bmUICtr.monster = monster;

			// 初始化怪物状态栏
			bmUICtr.SetUpMonsterStatusPlane (monster);

		}
			





		/// <summary>
		/// 怪物进入战斗
		/// </summary>
		/// <param name="bpCtr">Bp ctr.</param>
		/// <param name="playerWinCallBack">Player window call back.</param>
		public void StartFight(BattlePlayerController bpCtr,CallBack<Transform> playerWinCallBack){

			this.bpCtr = bpCtr;

			this.playerWinCallBack = playerWinCallBack;

			// 怪物比玩家延迟0.5s进入战斗状态
			Invoke ("Fight", 0.5f);

		}
			
		/// <summary>
		/// 角色战斗逻辑
		/// </summary>
		public override void Fight(){
			currentSkill = InteligentAttackSkill ();
			UseSkill (currentSkill);
		}


			
		/// <summary>
		/// 使用技能
		/// </summary>
		/// <param name="skill">Skill.</param>
		protected override void UseSkill (ActiveSkill skill)
		{

			// 播放技能对应的角色动画，角色动画结束后播放攻击间隔动画
			this.PlayRoleAnim (skill.selfRoleAnimName, 1, () => {
				// 播放等待动画
				this.PlayRoleAnim("interval",0,null);
			});

		}
			


		protected override void AgentExcuteHitEffect ()
		{
			// 播放技能对应的怪物技能特效动画
			if (currentSkill.selfEffectAnimName != string.Empty) {
				SetEffectAnim (currentSkill.selfEffectAnimName);
			}

			// 播放技能对应的玩家技能特效动画
			if (currentSkill.enemyEffectAnimName != string.Empty) {
				bpCtr.SetEffectAnim (currentSkill.enemyEffectAnimName);
			}

			SoundManager.Instance.PlayAudioClip ("Skill/" + currentSkill.sfxName);

			currentSkill.AffectAgents(this,bpCtr);

			// 如果战斗没有结束，则默认在攻击间隔时间之后按照默认攻击方式进行攻击
			if(!CheckFightEnd()){
				currentSkill = InteligentAttackSkill ();
//				Debug.Log (currentSkill);
				attackCoroutine = InvokeAttack (currentSkill);
				StartCoroutine (attackCoroutine);
			}
				

//			this.UpdateStatusPlane();
//
//			bpCtr.UpdateStatusPlane();

//			Player player = Player.mainPlayer;

//			switch (currentSkill.hurtType) {
//			case HurtType.Physical:
//
//				// 玩家受到物理攻击，已装备的护具中随机一个护具的耐久度降低
//				List<Equipment> allEquipedProtector = player.allEquipedEquipments.FindAll (delegate (Equipment obj) {
//					int equipmentTypeToInt = (int)obj.equipmentType;
//					return equipmentTypeToInt >= 1 && equipmentTypeToInt <= 5;
//				});
//
//				if (allEquipedProtector.Count == 0) {
//					break;
//				}
//
//				int randomIndex = Random.Range (0, allEquipedProtector.Count);
//
//				Equipment damagedEquipment = allEquipedProtector [randomIndex];
//
//				bool completeDamaged = damagedEquipment.EquipmentDamaged (EquipmentDamageSource.BePhysicalAttacked);
//
//				if (completeDamaged) {
//					string tint = string.Format("{0}完全损坏",damagedEquipment.itemName);
//					bmUICtr.GetComponent<ExploreUICotroller> ().SetUpTintHUD (tint);
//
//				}
//
//				break;
//			case HurtType.Magical:
//
//				List<Equipment> allEquipedOrnaments = player.allEquipedEquipments.FindAll (delegate(Equipment obj) {
//					int equipmentTypeToInt = (int)obj.equipmentType;
//					return equipmentTypeToInt >= 5 && equipmentTypeToInt <= 6;
//				});
//
//				if (allEquipedOrnaments.Count == 0) {
//					break;
//				}
//
//				randomIndex = Random.Range (0, allEquipedOrnaments.Count);
//
//				damagedEquipment = allEquipedOrnaments [randomIndex];
//
//				completeDamaged = damagedEquipment.EquipmentDamaged (EquipmentDamageSource.BeMagicAttacked);
//
//				if (completeDamaged) {
//					string tint = string.Format("{0}完全损坏",damagedEquipment.itemName);
//					bmUICtr.GetComponent<ExploreUICotroller> ().SetUpTintHUD (tint);
//				}
//				break;
//			default:
//				break;
//			}
		}

		public override void InitFightTextDirectionTowards (Vector3 position)
		{
			bmUICtr.fightTextManager.SetUpFightTextManager (transform.position, position);
		}

		public override void AddFightTextToQueue (string text, SpecialAttackResult specialAttackType)
		{
			FightText ft = new FightText (text, specialAttackType);

			bmUICtr.fightTextManager.AddFightText (ft);
		}

//		public override void ShowFightTextInOrder ()
//		{
//			bmUICtr.fightTextManager.ShowFightTextInOrder ();
//		}


		/// <summary>
		/// 判断战斗是否结束
		/// </summary>
		public override bool CheckFightEnd(){

			if (bpCtr.agent.health <= 0) {
				bpCtr.AgentDie ();
				return true;
			} else if (agent.health <= 0) {
				return true;
			}else {
				return false;
			}

		}

		public override void UpdateStatusPlane(){
			bmUICtr.UpdateAgentStatusPlane ();
		}




		/// <summary>
		/// 怪物死亡
		/// </summary>
		override public void AgentDie(){

			StopCoroutinesWhenFightEnd ();

			bpCtr.StopCoroutinesWhenFightEnd ();

			this.armatureCom.animation.Stop ();

			CancelInvoke ();

			boxCollider.enabled = false;

			playerWinCallBack (new Transform[]{ transform });

			ExploreUICotroller expUICtr = bmUICtr.GetComponent<ExploreUICotroller> ();

			expUICtr.QuitFight ();

			PlayRoleAnim ("die", 1, delegate {
				exploreManager.GetComponent<MapGenerator>().AddMonsterToPool(this);
			});



		}

		public override void TowardsLeft ()
		{
			modelActive.transform.localScale = new Vector3 (-1, 1, 1);
		}

		public override void TowardsRight ()
		{
			modelActive.transform.localScale = Vector3.one;
		}

		public void AddToPool(InstancePool pool){

			boxCollider.enabled = false;
			gameObject.SetActive (false);
			pool.AddInstanceToPool (this.gameObject);

		}

	}
}
