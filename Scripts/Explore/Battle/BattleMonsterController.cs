using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{

	public class BattleMonsterController : BattleAgentController {



		// 怪物UI控制器
		private BattleMonsterUIController bmUICtr;

		// 玩家控制器
		private BattlePlayerController bpCtr;

		// 玩家战斗胜利回调
		private CallBack<Transform> playerWinCallBack;


		protected override void Awake(){

			agent = GetComponent<Monster> ();

			modelActive = this.gameObject;

			base.Awake ();

		}

		public void SetAlive(){
			armatureCom.enabled = true;
			boxCollider.enabled = true;
		}

		/// <summary>
		/// 初始化碰到的怪物
		/// </summary>
		public void InitMonster(Transform monsterTrans){

			Transform canvas = TransformManager.FindTransform ("ExploreCanvas");

			bmUICtr = canvas.GetComponent<BattleMonsterUIController> ();

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

		#warning 后面这里要加入怪物战斗逻辑,现在默认一直使用普通攻击
		/// <summary>
		/// 角色战斗逻辑
		/// </summary>
		public override void Fight(){
			currentSkill = (agent as Monster).InteligentSelectSkill ();
			UseSkill (currentSkill);
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

			// 播放技能对应的角色动画，角色动画结束后播放技能特效动画，实现技能效果并更新角色状态栏
			this.PlayRoleAnim (skill.selfAnimName, 1, () => {
				// 播放等待动画
				this.PlayRoleAnim("interval",0,null);
			});

		}


		protected override void AgentExcuteHitEffect ()
		{

			// 播放技能对应的玩家技能特效动画
			if (currentSkill.selfEffectName != string.Empty) {
				SetEffectAnim (currentSkill.selfEffectName);
			}

			// 播放技能对应的怪物技能特效动画
			if (currentSkill.enemyEffectName != string.Empty) {
				bpCtr.SetEffectAnim (currentSkill.enemyEffectName);
			}

			GameManager.Instance.soundManager.PlaySkillEffectClips (currentSkill.sfxName);

//			GameManager.Instance.soundManager.PlayClips (
//				GameManager.Instance.gameDataCenter.allExploreAudioClips,
//				SoundDetailTypeName.Skill, 
//				currentSkill.sfxName);

			currentSkill.AffectAgents(this,bpCtr);

			// 如果战斗没有结束，则默认在攻击间隔时间之后按照默认攻击方式进行攻击
			if(!FightEnd()){
				currentSkill = (agent as Monster).InteligentSelectSkill ();
				attackCoroutine = InvokeAttack (currentSkill);
				StartCoroutine (attackCoroutine);
			}

			this.UpdateStatusPlane();

			bpCtr.UpdateStatusPlane();



			Player player = Player.mainPlayer;

			switch (currentSkill.skillType) {
			case SkillType.Physical:

				// 玩家受到物理攻击，已装备的护具中随机一个护具的耐久度降低
				List<Equipment> allEquipedProtector = player.allEquipedEquipments.FindAll (delegate (Equipment obj) {
					int equipmentTypeToInt = (int)obj.equipmentType;
					return equipmentTypeToInt >= 1 && equipmentTypeToInt <= 5;
				});

				if (allEquipedProtector.Count == 0) {
					break;
				}

				int randomIndex = Random.Range (0, allEquipedProtector.Count);

				Equipment damagedEquipment = allEquipedProtector [randomIndex];

				bool completeDamaged = damagedEquipment.EquipmentDamaged (EquipmentDamageSource.BePhysicalAttacked);

				if (completeDamaged) {
					string tint = string.Format("{0}完全损坏",damagedEquipment.itemName);
					bmUICtr.GetComponent<ExploreUICotroller> ().SetUpTintHUD (tint);

				}

				break;
			case SkillType.Magical:

				List<Equipment> allEquipedOrnaments = player.allEquipedEquipments.FindAll (delegate(Equipment obj) {
					int equipmentTypeToInt = (int)obj.equipmentType;
					return equipmentTypeToInt >= 5 && equipmentTypeToInt <= 6;
				});

				if (allEquipedOrnaments.Count == 0) {
					break;
				}

				randomIndex = Random.Range (0, allEquipedOrnaments.Count);

				damagedEquipment = allEquipedOrnaments [randomIndex];

				completeDamaged = damagedEquipment.EquipmentDamaged (EquipmentDamageSource.BeMagicAttacked);

				if (completeDamaged) {
					string tint = string.Format("{0}完全损坏",damagedEquipment.itemName);
					bmUICtr.GetComponent<ExploreUICotroller> ().SetUpTintHUD (tint);
				}
				break;
			default:
				break;
			}
		}


		/// <summary>
		/// 伤害文本动画
		/// </summary>
		/// <param name="hurtStr">Hurt string.</param>
		/// <param name="tintTextType">伤害类型 [TintTextType.Crit:暴击伤害 ,TintTextType.Miss: 伤害闪避]</param>
		public override void PlayHurtTextAnim (string hurtStr,TintTextType tintTextType)
		{
			if (this.transform.position.x < bpCtr.transform.position.x) {
				bmUICtr.PlayHurtTextAnim (hurtStr, this.transform.position, Towards.Left, tintTextType);
			} else {
				bmUICtr.PlayHurtTextAnim (hurtStr, this.transform.position, Towards.Right, tintTextType);
			}

		}

		/// <summary>
		/// 血量提升文本动画
		/// </summary>
		/// <param name="gainStr">Gain string.</param>
		public override void PlayGainTextAnim (string gainStr)
		{
			if (this.transform.position.x < bpCtr.transform.position.x) {
				bmUICtr.PlayGainTextAnim (gainStr, this.transform.position, Towards.Left);
			} else {
				bmUICtr.PlayGainTextAnim (gainStr, this.transform.position, Towards.Right);
			}
		}

		/// <summary>
		/// 判断战斗是否结束
		/// </summary>
		private bool FightEnd(){

			if (bpCtr.agent.health <= 0) {
				bpCtr.AgentDie ();
				return true;
			} else if (agent.health <= 0) {
				return true;
			}else {
				return false;
			}

		}

		public void UpdateStatusPlane(){
			bmUICtr.UpdateMonsterStatusPlane ();
		}

		/// <summary>
		/// 怪物死亡
		/// </summary>
		override public void AgentDie(){

			Debug.Log ("monsterDie");

			this.armatureCom.animation.Stop ();

			bpCtr.PlayRoleAnim ("wait", 0, null);

			CancelInvoke ();

//			StopAllCoroutines ();

			if (attackCoroutine != null) {
				StopCoroutine (attackCoroutine);
			}

			if (waitRoleAnimEndCoroutine != null) {
				StopCoroutine (waitRoleAnimEndCoroutine);
			}
				
			if (bpCtr.attackCoroutine != null) {
				StopCoroutine (bpCtr.attackCoroutine);
			}
				
			if (bpCtr.waitRoleAnimEndCoroutine != null) {
				StopCoroutine (bpCtr.waitRoleAnimEndCoroutine);
			}
				
			boxCollider.enabled = false;

			playerWinCallBack (new Transform[]{ transform });

//			Debug.LogFormat ("怪物位置信息[{0},{1}]", transform.position.x, transform.position.y);

			ExploreUICotroller expUICtr = bmUICtr.GetComponent<ExploreUICotroller> ();

			expUICtr.QuitFight ();

//			CollectSkillEffectsToPool();

			exploreManager.GetComponent<MapGenerator> ().PlayMapOtherAnim ("Death", transform.position);

			Invoke ("MoveAgentToDieZone", 0.2f);

		}





	}
}
