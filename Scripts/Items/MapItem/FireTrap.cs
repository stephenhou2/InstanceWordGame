using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class FireTrap : Trap {

		public int lifeLose;
		public int lifeLoseDuration;

		public Animator mapItemAnimator;

		private MapGenerator mMapGenerator;
		private MapGenerator mapGenerator{
			get{
				if (mMapGenerator == null) {
					mMapGenerator = TransformManager.FindTransform ("ExploreManager").GetComponent<MapGenerator> ();
				}
				return mMapGenerator;
			}
		}

		private IEnumerator lifeLoseCoroutine;


		public override void InitMapItem ()
		{
			bc2d.enabled = true;
			SetTrapOn ();
			SetSortingOrder (-(int)transform.position.y);
		}


		public override void AddToPool(InstancePool pool){
			bc2d.enabled = false;
			pool.AddInstanceToPool (this.gameObject);
		}

		public override void SetTrapOn ()
		{
			mapItemAnimator.SetBool ("Play",false);
			bc2d.enabled = true;
			isTrapOn = true;
			mapGenerator.mapWalkableInfoArray [(int)transform.position.x, (int)transform.position.y] = 10;
		}

		public override void SetTrapOff ()
		{
			mapItemAnimator.SetBool ("Play",true);
			bc2d.enabled = false;
			isTrapOn = false;
			mapGenerator.mapWalkableInfoArray [(int)transform.position.x, (int)transform.position.y] = 1;
			mapGenerator.AddMapItemInPool (this.transform);
			
		}

		public override void OnTriggerEnter2D (Collider2D col)
		{
//			triggered = !triggered;

			if (!isTrapOn) {
				return;
			}

		
			SoundManager.Instance.PlayAudioClip("MapEffects/" + audioClipName);

			BattleAgentController ba = col.GetComponent<BattleAgentController> ();

			if (ba is BattlePlayerController) {
				ba.InitFightTextDirectionTowards (transform.position);
			}

			if (lifeLoseCoroutine != null) {
				StopCoroutine (lifeLoseCoroutine);
			}

			lifeLoseCoroutine = LoseLifeContinous (ba);

			StartCoroutine (lifeLoseCoroutine);


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
