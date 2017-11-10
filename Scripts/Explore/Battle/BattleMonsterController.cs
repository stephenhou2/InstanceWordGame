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

			defaultSkill.selfAnimName = "fight";

			modelActive = this.gameObject;

			base.Awake ();

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
		/// 使用技能之后的逻辑
		/// </summary>
		/// <param name="skill">Skill.</param>
		protected override void UseSkill (Skill skill)
		{

			// 播放技能对应的角色动画，角色动画结束后播放技能特效动画，实现技能效果并更新角色状态栏
			this.PlayRoleAnim (skill.selfAnimName, 1, () => {
				AttackerRoleAnimEnd(skill);
			});

			// 播放技能对应的怪物动画
			if (skill.enemyAnimName != string.Empty) {
				bpCtr.PlayRoleAnim (skill.enemyAnimName, 1, null);
			}


		}


		/// <summary>
		/// 怪物角色动画（攻击动作）结束后的逻辑
		/// </summary>
		/// <param name="skill">Skill.</param>
		private void AttackerRoleAnimEnd(Skill skill){

			skill.AffectAgents(this,bpCtr);

			this.UpdateStatusPlane();

			bpCtr.UpdateStatusPlane();

			// 如果战斗没有结束，则默认在攻击间隔时间之后按照默认攻击方式进行攻击
			if(!FightEnd()){
				attackCoroutine = StartCoroutine("InvokeAttack",defaultSkill);
			}

			// 播放技能对应的玩家技能特效动画
			if (skill.selfEffectName != string.Empty) {
				SetEffectAnim (skill.selfEffectName);
			}

			// 播放技能对应的怪物技能特效动画
			if (skill.enemyEffectName != string.Empty) {
				bpCtr.SetEffectAnim (skill.enemyEffectName);
			}

			GameManager.Instance.soundManager.PlayClips (
				GameManager.Instance.gameDataCenter.allExploreAudioClips,
				SoundDetailTypeName.Skill, 
				skill.sfxName);

			Player player = Player.mainPlayer;
			switch (skill.skillType) {
			case SkillType.Physical:
				for (int i = 0; i < player.allEquipedEquipments.Count; i++) {

					Equipment equipment = player.allEquipedEquipments [i];

					// 受到物理攻击时，已装备的护具中随机一个护具的耐久度降低
					EquipmentType damagedEquipmentType = (EquipmentType)Random.Range (1, 5);

					if (equipment.equipmentType == damagedEquipmentType) {

						equipment.durability -= CommonData.durabilityDecreaseWhenBeAttacked;

						if (equipment.durability <= 0) {
							string tint = string.Format("{0}完全损坏",equipment.itemName);
							bmUICtr.GetComponent<ExploreUICotroller> ().SetUpTintHUD (tint);
							player.RemoveItem (equipment);
							equipment = null;
						}
					}

				}
				break;
			case SkillType.Magic:
				for (int i = 0; i < player.allEquipedEquipments.Count; i++) {

					Equipment equipment = player.allEquipedEquipments [i];

					// 受到物理攻击时，已装备的饰品中随机一个的耐久度降低
					EquipmentType damagedEquipmentType = (EquipmentType)Random.Range (5,7);

					if (equipment.equipmentType == damagedEquipmentType) {

						equipment.durability -= CommonData.durabilityDecreaseWhenBeMagicAttacked;

						if (equipment.durability <= 0) {
							string tint = string.Format("{0}完全损坏",equipment.itemName);
							bmUICtr.GetComponent<ExploreUICotroller> ().SetUpTintHUD (tint);
							player.RemoveItem (equipment);
							equipment = null;
						}
					}
				}
				break;
			case SkillType.Passive:
				break;
			}

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
			UseSkill(defaultSkill);
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
			bmUICtr.PlayGainTextAnim (gainStr,this.transform.position);
		}

		/// <summary>
		/// 判断战斗是否结束
		/// </summary>
		private bool FightEnd(){

			if (bpCtr.agent.health <= 0) {
				bpCtr.PlayerDie ();
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
		public void MonsterDie(){

			if (attackCoroutine != null) {
				StopCoroutine (attackCoroutine);
			}
			if (waitRoleAnimEndCoroutine != null) {
				StopCoroutine (waitRoleAnimEndCoroutine);
			}
				

			bpCtr.PlayRoleAnim ("stand", 0, null);

			PlayRoleAnim ("death", 1, () => {
				
				playerWinCallBack (new Transform[]{ transform });

				ExploreUICotroller expUICtr = bmUICtr.GetComponent<ExploreUICotroller> ();

				expUICtr.QuitFight ();

				CollectSkillEffectsToPool();

				gameObject.SetActive(false);
			});

		}



	}
}
