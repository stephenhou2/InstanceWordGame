using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class Bullet : MonoBehaviour {

		public float speed;

		public int hurt;

		private Animator bulletAnimator;

		private InstancePool bulletPool;

		private IEnumerator bulletMoveCoroutine;

		private BoxCollider2D bc2d;


		void Awake(){
			bulletAnimator = GetComponent<Animator> ();
			bc2d = GetComponent<BoxCollider2D> ();
		}
			

		public void BulletMove(Vector3 launcherPos,MyTowards towards,Vector2 range,InstancePool bulletPool){

			this.bulletPool = bulletPool;


			Vector3 bulletLaunchPos = Vector3.zero;

			switch (towards) {
			case MyTowards.Left:
				bulletLaunchPos = launcherPos + new Vector3 (-0.5f, 0, 0);
				break;
			case MyTowards.Right:
				bulletLaunchPos = launcherPos + new Vector3 (0.5f, 0, 0);
				break;
			case MyTowards.Up:
				bulletLaunchPos = launcherPos + new Vector3 (0, 0.5f, 0);
				break;
			case MyTowards.Down:
				bulletLaunchPos = launcherPos + new Vector3 (0, -0.5f, 0);
				break;

			}

			this.transform.position = bulletLaunchPos;

			bc2d.enabled = true;

			bulletMoveCoroutine = BulletMoveWithMoveVectorAndRange (towards,range);

			StartCoroutine (bulletMoveCoroutine);

		}

		private IEnumerator BulletMoveWithMoveVectorAndRange(MyTowards towards,Vector2 range){

			float distanceX = 0;
			float distanceY = 0;

			Vector3 moveVector = Vector3.zero;

			while (distanceX < range.x && distanceY < range.y) {

				switch (towards) {
				case MyTowards.Left:
					moveVector = new Vector3 (-speed * Time.deltaTime, 0, 0);
					break;
				case MyTowards.Right:
					moveVector = new Vector3 (speed * Time.deltaTime, 0, 0);
					break;
				case MyTowards.Up:
					moveVector = new Vector3 (0,speed * Time.deltaTime, 0);
					break;
				case MyTowards.Down:
					moveVector = new Vector3 (0,-speed * Time.deltaTime, 0);
					break;

				}

				this.transform.position += moveVector;

				distanceX += moveVector.x;
				distanceY += moveVector.y;

//				Debug.Log (this.transform.position);

				yield return null;
			}

//			Debug.LogFormat("before : {0}",bulletAnimator.GetCurrentAnimatorClipInfo(0).
//			bulletAnimator.SetBool ("Play",false);


			bc2d.enabled = false;

			bulletPool.AddInstanceToPool (this.gameObject);

		}

		void OnTriggerEnter2D(Collider2D other){

			if (other.GetComponent<Launcher> () != null) {
				return;
			}

			Debug.Log ("子弹碰到物体");

			StopCoroutine (bulletMoveCoroutine);

			bc2d.enabled = false;

			BattleAgentController ba = other.GetComponent<BattleAgentController> ();

			if (ba != null) {
				ba.InitFightTextDirectionTowards (this.transform.position);
				ba.propertyCalculator.InstantPropertyChange (ba, PropertyType.Health, -hurt, false);
				Debug.Log ("子弹命中");
			}

			bulletAnimator.SetBool ("Play",true);

			StartCoroutine ("ColletBulletAfterExplosion");

		}

		private IEnumerator ColletBulletAfterExplosion(){

			yield return null;

			AnimatorStateInfo info = bulletAnimator.GetCurrentAnimatorStateInfo (0);

			while (info.normalizedTime < 1.0f) {
				yield return null;
				info = bulletAnimator.GetCurrentAnimatorStateInfo (0);
			}


			bc2d.enabled = false;

			bulletAnimator.SetBool ("Play", false);

			this.transform.position = new Vector3 (0, 0, 100);

			bulletPool.AddInstanceToPool (this.gameObject);
		}



	}
}
