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
			
			Transform canvas = TransformManager.FindTransform ("ExploreCanvas");

			bmUICtr = canvas.GetComponent<BattleMonsterUIController> ();

			physicalAttack.selfAnimName = "fight";

			base.Awake ();

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
		/// Uses the skill.
		/// </summary>
		/// <param name="skill">Skill.</param>
		protected override void UseSkill (Skill skill)
		{
			this.PlayRoleAnim (skill.selfAnimName, 1, () => {
				skill.AffectAgents(this,bpCtr);
				this.UpdateMonsterStatusPlane();
				bpCtr.UpdatePlayerStatusPlane();
				if(!FightEnd()){
					StopCoroutine("InvokeAttack");
					StartCoroutine("InvokeAttack",skill);
				}
			});
			if (skill.enemyAnimName != string.Empty) {
				bpCtr.PlayRoleAnim (skill.enemyAnimName, 1, null);
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

			// 怪物比玩家延迟0.2s进入战斗状态
			Invoke ("Fight", 0.2f);

		}

		#warning 后面这里要加入怪物战斗逻辑,现在默认一直使用普通攻击
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

		public void UpdateMonsterStatusPlane(){
			bmUICtr.UpdateMonsterStatusPlane ();
		}

		/// <summary>
		/// 怪物死亡
		/// </summary>
		public void MonsterDie(){
			bpCtr.StopCoroutine ("InvokeAttack");
			StopCoroutine ("InvokeAttack");

			bmUICtr.GetComponent<ExploreUICotroller> ().QuitFight ();

			bpCtr.PlayRoleAnim ("stand", 0, null);
			PlayRoleAnim ("death", 1, () => {
				playerWinCallBack (new Transform[]{ transform });
				gameObject.SetActive(false);
			});
		}

	}
}
