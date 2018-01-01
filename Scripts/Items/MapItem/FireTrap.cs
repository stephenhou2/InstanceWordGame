using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class FireTrap : Trap {

		public int lifeLose;
		public int lifeLoseDuration;

		private Vector3 agentOriPos;

		private IEnumerator fireTriggeredCoroutine;

		private IEnumerator lifeLoseCoroutine;

		public override void SetTrapOn ()
		{
			mapItemAnimator.ResetTrigger ("ChangeStatus");
		}

		public override void SetTrapOff ()
		{
			mapItemAnimator.SetTrigger ("ChangeStatus");
		}

		public override void OnTriggerEnter2D (Collider2D col)
		{
//			triggered = !triggered;

			if (!trapOn) {
				return;
			}

			agentOriPos = new Vector3 (Mathf.RoundToInt(col.transform.position.x), Mathf.RoundToInt(col.transform.position.y), 0);

			Debug.LogFormat ("oriPos:{0}----currentAgentPos:{1}", agentOriPos, col.transform.position);

			GameManager.Instance.soundManager.PlayMapEffectClips(audioClipName);

			BattleAgentController ba = col.GetComponent<BattleAgentController> ();

			if (ba is BattlePlayerController) {
				BattlePlayerController bp = ba as BattlePlayerController;
				bp.StopMove ();
				bp.singleMoveEndPos = agentOriPos;
				bp.InitFightTextDirectionTowards (transform.position);
			}

			if (fireTriggeredCoroutine != null) {
				StopCoroutine (fireTriggeredCoroutine);
			}

			fireTriggeredCoroutine = FireTriggerEffect (ba);

			StartCoroutine (fireTriggeredCoroutine);


			if (lifeLoseCoroutine != null) {
				StopCoroutine (lifeLoseCoroutine);
			}

			lifeLoseCoroutine = LoseLifeContinous (ba);

			StartCoroutine (lifeLoseCoroutine);




		}

		private IEnumerator FireTriggerEffect(BattleAgentController ba){

			float timer = 0;

			float burnBackDuration = 0.1f;


			while (timer < burnBackDuration) {

				Vector3 burnBackVector = new Vector3 (
					(agentOriPos.x - ba.transform.position.x) / burnBackDuration,
					(agentOriPos.y - ba.transform.position.y) / burnBackDuration, 0) * Time.deltaTime;



				ba.transform.position += burnBackVector;
				timer += Time.deltaTime;
				yield return null;
			}

		}


		private IEnumerator LoseLifeContinous(BattleAgentController target){

			float timer = 0;

			while (timer < lifeLoseDuration) {

				target.propertyCalculator.InstantPropertyChange (target, PropertyType.Health, -lifeLose, false);

				yield return new WaitForSeconds (1);

				timer += 1;

			}

		}

	}
}
