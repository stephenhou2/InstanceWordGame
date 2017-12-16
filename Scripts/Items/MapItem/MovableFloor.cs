using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
//	using DG.Tweening;

	public class MovableFloor : MapItem {

		public Vector2 originPos;
		public Vector2 targetPos;

		public bool inMove;

		public float moveSpeed;

		private float moveDuration;

		public override void InitMapItem ()
		{
			inMove = false;
			bc2d.enabled = true;
			moveDuration = (targetPos - originPos).magnitude / moveSpeed;
		}

		public void MoveToAnotherPosition(BattlePlayerController player){

			Vector2 direction = Vector2.zero;

			Vector2 currentPos = new Vector2 ((int)transform.position.x, (int)transform.position.y);

			if (currentPos == originPos) {
				direction = targetPos - originPos;
			} else {
				direction = originPos - targetPos;
			}

			IEnumerator moveWithFloor = SmoothMoveToPos (direction, player.transform);

			StartCoroutine (moveWithFloor);

		}


		private IEnumerator SmoothMoveToPos(Vector2 direction,Transform playerTrans){

//			mapWalkableInfo [(int)transform.position.x, (int)(transform.position.y)] = -1;

			inMove = true;

			float timer = 0;

			Vector3 moveVector = new Vector3 (direction.x * Time.deltaTime, direction.y * Time.deltaTime, 0);

			while (timer < moveDuration) {
				
				timer += Time.deltaTime;

				this.transform.position += moveVector;
				 
				playerTrans.position += moveVector;

				yield return null;

			}

//			mapWalkableInfo [(int)transform.position.x, (int)(transform.position.y)] = 0;

			inMove = false;


		}
	}
}
