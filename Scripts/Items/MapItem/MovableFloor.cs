using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	using DG.Tweening;

	public class MovableFloor : MapItem {

//		public Vector2 originPos;
//		public Vector2 targetPos;

//		private bool inMove;

		public float moveSpeedX;

		private bool triggered;

//		private float moveDuration;

		private Transform mExploreManager;
		private Transform exploreManager{
			get{
				if (mExploreManager == null) {
					mExploreManager = TransformManager.FindTransform ("ExploreManager");
				}
				return mExploreManager;
			}
		}

		public override void InitMapItem ()
		{
//			inMove = false;
			bc2d.enabled = true;
			triggered = false;
//			moveDuration = (targetPos - originPos).magnitude / moveSpeed;
		}

		public void OnTriggerEnter2D(Collider2D other){

			triggered = !triggered;

			if (triggered) {

				exploreManager.GetComponent<ExploreManager> ().DisableInteractivity ();

				MoveToNearestPosition (other.transform);
			}

		}
			

		public void MoveToNearestPosition(Transform other){

			BattlePlayerController bp = other.GetComponent<BattlePlayerController> ();

			if (bp == null) {
				exploreManager.GetComponent<ExploreManager> ().EnableInteractivity ();
				return;
			}

			bp.boxCollider.enabled = false;
			bc2d.enabled = false;

			MapGenerator mapGenerator = exploreManager.GetComponent<MapGenerator> ();

			Vector3 nearestMovableFloorPos = mapGenerator.FindNearestMovableFloor (this.transform.position);

			if (nearestMovableFloorPos == transform.position) {
				exploreManager.GetComponent<ExploreManager> ().EnableInteractivity ();
				return;
			}

			IEnumerator floorMoveAnim = SmoothMoveToPos (this.transform.position,nearestMovableFloorPos, bp);

			StartCoroutine (floorMoveAnim);

		}


		/// <summary>
		/// 角色随地板一起移动到最近的可移动地板目的地
		/// </summary>
		/// <returns>The move to position.</returns>
		/// <param name="startPos">Start position.</param>
		/// <param name="endPos">End position.</param>
		/// <param name="ba">Ba.</param>
		private IEnumerator SmoothMoveToPos(Vector3 startPos, Vector3 endPos, BattlePlayerController bp){

			yield return new WaitUntil (() => bp.isIdle);

			int[,] realMapWalkableInfo = exploreManager.GetComponent<MapGenerator> ().mapWalkableInfoArray;

			realMapWalkableInfo [(int)startPos.x, (int)startPos.y] = -1;


			bp.ActiveBattlePlayer (false, false, true);

			bp.PlayRoleAnim ("wait", 0, null);

			float myMoveSpeedX = 0;

			// x轴方向的移动速度
			if (endPos.x > startPos.x) {
				myMoveSpeedX = moveSpeedX;
				bp.TowardsRight ();
			} else {
				myMoveSpeedX = -moveSpeedX;
				bp.TowardsLeft ();
			}
//			float myMoveSpeedX = (endPos.x > startPos.x ? 1 : -1) * moveSpeedX;
			// 总移动时长
			float moveDuration = Mathf.Abs ((endPos.x - startPos.x) / moveSpeedX);

			// y轴方向的移动速度
			float myMoveSpeedY = (endPos.y-startPos.y)/moveDuration;



			float timer = 0;


			while (timer < moveDuration) {

				Vector3 moveVector = new Vector3 (myMoveSpeedX * Time.deltaTime, myMoveSpeedY * Time.deltaTime, 0);

				timer += Time.deltaTime;

				this.transform.position += moveVector;
				 
				bp.transform.position += moveVector;

				yield return null;

			}

			transform.position = endPos;
			bp.transform.position = endPos;
			bp.singleMoveEndPos = endPos;

			bp.SetSortingOrder (-(int)endPos.y);

//			bc2d.enabled = true;
			bp.boxCollider.enabled = true;
			StartCoroutine ("LatelyEnableBoxCollider",bp);

			exploreManager.GetComponent<ExploreManager> ().ItemsAroundAutoIntoLifeWithBasePoint (endPos);

			// 如果要自动走到一个可行走点，则开启下面的代码
//			Vector3 walkablePositionAround = exploreManager.GetComponent<MapGenerator>().GetAWalkablePositionAround (endPos);
//
//			yield return new WaitUntil (()=>realMapWalkableInfo [(int)walkablePositionAround.x, (int)walkablePositionAround.y] == 1);
//
			realMapWalkableInfo [(int)endPos.x, (int)endPos.y] = 10;
//
//			bp.MoveToEndByPath(new List<Vector3>{walkablePositionAround},walkablePositionAround);

			exploreManager.GetComponent<ExploreManager> ().EnableInteractivity ();
		}

		private IEnumerator LatelyEnableBoxCollider(BattlePlayerController bp){
			yield return new WaitUntil (()=>!bp.isIdle);
			bc2d.enabled = true;
		}






	}
}
