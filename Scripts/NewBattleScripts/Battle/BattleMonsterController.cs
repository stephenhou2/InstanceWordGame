using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	public class BattleMonsterController : BattleAgentController {

		[HideInInspector]public Monster monster;

		private BattleMonsterUIController bmUICtr;

		private BattlePlayerController bpCtr;

		private CallBack<Transform> playerWinCallBack;

		protected override void Awake(){

			monster = GetComponent<Monster> ();
			
			Transform canvas = TransformManager.FindTransform ("ExploreCanvas");

			bmUICtr = canvas.GetComponent<BattleMonsterUIController> ();

			agent = monster;

			base.Awake ();

		}

		public void InitMonster(Transform monsterTrans){

			Monster monster = agent as Monster;

			bmUICtr.monster = monster;

			bmUICtr.SetUpMonsterStatusPlane (monster);

		}

		public void StartFight(BattlePlayerController bpCtr,CallBack<Transform> playerWinCallBack){

			this.bpCtr = bpCtr;

			this.playerWinCallBack = playerWinCallBack;

			Invoke ("PhysicalAttack", 0.2f);
//			StartCoroutine ("InvokePhysicalAttack");

		}

		private void PhysicalAttack(){

			physicalAttack.AffectAgents (this, bpCtr);

			bpCtr.UpdatePlayerStatusPlane ();

			if (!FightEnd ()) {
				StartCoroutine ("InvokePhysicalAttack");
			}

		}

		private IEnumerator InvokePhysicalAttack(){

			float timePassed = 0;

			while (timePassed < monster.attackInterval) {

				timePassed += Time.deltaTime;

				yield return null;

			}

			PhysicalAttack ();

		}

		public override void PlayHurtTextAnim (string hurtStr)
		{
			if (this.transform.position.x < bpCtr.transform.position.x) {
				bmUICtr.PlayHurtTextAnim (hurtStr, this.transform.position, Towards.Left);
			} else {
				bmUICtr.PlayHurtTextAnim (hurtStr, this.transform.position, Towards.Right);
			}

		}

		public override void PlayGainTextAnim (string gainStr)
		{
			bmUICtr.PlayGainTextAnim (gainStr,this.transform.position);
		}

		private bool FightEnd(){

			if (bpCtr.agent.health <= 0) {
				bpCtr.PlayerDie ();
				return true;
			} else if (monster.health <= 0) {
				return true;
			}else {
				return false;
			}

		}

		public void UpdateMonsterStatusPlane(){
			bmUICtr.UpdateMonsterStatusPlane ();
		}

		public void MonsterDie(){
			bmUICtr.GetComponent<ExploreUICotroller> ().QuitFight ();
			bmUICtr.PlayMonsterDieAnim (this, playerWinCallBack, new Transform[]{transform});
		}

	}
}
