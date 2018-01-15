using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class NormalTrap : Trap {

		public int lifeLose;

		// 陷阱打开状态的图片
		public Sprite trapOnSprite;
		// 陷阱关闭状态的图片
		public Sprite trapOffSprite;

		private IEnumerator normalTrapTriggeredCoroutine;

		private Vector3 agentOriPos;

		public override void InitMapItem ()
		{
			bc2d.enabled = true;
			SetSortingOrder (-(int)transform.position.y);
		}

		public override void AddToPool (InstancePool pool)
		{
			bc2d.enabled = false;
			pool.AddInstanceToPool (this.gameObject);
		}



		public override void SetTrapOn ()
		{
			mapItemRenderer.sprite = trapOnSprite;
			isTrapOn = true;
		}

		public override void SetTrapOff ()
		{
			mapItemRenderer.sprite = trapOffSprite;
			isTrapOn = false;
		}


		public override void OnTriggerEnter2D (Collider2D col)
		{

			if (!isTrapOn) {
				return;
			}

			SoundManager.Instance.PlayAudioClip("MapEffects/" + audioClipName);

			BattleAgentController ba = col.GetComponent<BattleAgentController> ();

			agentOriPos = new Vector3 (Mathf.RoundToInt(col.transform.position.x), Mathf.RoundToInt(col.transform.position.y), 0);

			Debug.LogFormat ("oriPos:{0}----currentAgentPos:{1}", agentOriPos, col.transform.position);

			if (ba is BattlePlayerController) {
				BattlePlayerController bp = ba as BattlePlayerController;
				bp.StopMove ();
				bp.singleMoveEndPos = agentOriPos;
				bp.InitFightTextDirectionTowards (transform.position);
			}

			ba.propertyCalculator.InstantPropertyChange (ba, PropertyType.Health, -lifeLose, false);


			if (normalTrapTriggeredCoroutine != null) {
				StopCoroutine (normalTrapTriggeredCoroutine);
			}

			normalTrapTriggeredCoroutine = NormalTrapTriggerEffect (ba);

			StartCoroutine (normalTrapTriggeredCoroutine);

		}



		private IEnumerator NormalTrapTriggerEffect(BattleAgentController ba){

			float timer = 0;

			float agentBackDuration = 0.1f;


			while (timer < agentBackDuration) {

				Vector3 agentBackVector = new Vector3 (
					(agentOriPos.x - ba.transform.position.x) / agentBackDuration,
					(agentOriPos.y - ba.transform.position.y) / agentBackDuration, 0) * Time.deltaTime;

				ba.transform.position += agentBackVector;
				timer += Time.deltaTime;
				yield return null;
			}

			ba.transform.position = agentOriPos;

		}


	}
}
