using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	using DG.Tweening;

	public class MovableFloor : MapItem {

		public float moveSpeedX;

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
			bc2d.enabled = true;
			SetSortingOrder (1);
		}

		public override void AddToPool (InstancePool pool)
		{
			bc2d.enabled = false;
			pool.AddInstanceToPool (this.gameObject);
		}

		public void OnTriggerEnter2D(Collider2D other){

			BattlePlayerController battlePlayer = other.GetComponent<BattlePlayerController> ();

			if (battlePlayer == null) {
				return;
			}
			if(!MyTool.ApproximatelySamePosition2D (battlePlayer.moveDestination, this.transform.position)) {
				return;
			}

			MoveToNearestPosition (other.transform);

		}

		public void MoveToNearestPosition(Transform other){

			BattlePlayerController bp = other.GetComponent<BattlePlayerController> ();

//			bp.StopMoveAtEndOfCurrentStep ();

			if (bp == null) {
				return;
			}

			MapGenerator mapGenerator = exploreManager.GetComponent<MapGenerator> ();

			Vector3 nearestMovableFloorPos = mapGenerator.FindNearestMovableFloor (this.transform.position);

			if (nearestMovableFloorPos == transform.position) {
				return;
			}

			bp.singleMoveEndPos = nearestMovableFloorPos;
			bp.TempStoreDestinationAndDontMove ();


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

			float moveDuration = Mathf.Abs ((endPos.x - startPos.x) / moveSpeedX);

			// y轴方向的移动速度
			float myMoveSpeedY = (endPos.y-startPos.y)/moveDuration;


			float timer = 0;
			bool endPosInit = false;


			while (timer < moveDuration) {

				Vector3 moveVector = new Vector3 (myMoveSpeedX * Time.deltaTime, myMoveSpeedY * Time.deltaTime, 0);

				timer += Time.deltaTime;

				this.transform.position += moveVector;
				 
				bp.transform.position += moveVector;

				// 走到一半时终点位置开始初始化（地板和物品，怪物出现）
				if (timer / moveDuration > 0.5 && !endPosInit) {
					exploreManager.GetComponent<ExploreManager> ().ItemsAroundAutoIntoLifeWithBasePoint (endPos);
					endPosInit = true;
				}

				yield return null;

			}

			transform.position = endPos;
			bp.transform.position = endPos;

			bp.SetSortingOrder (-(int)endPos.y);


			// 如果要自动走到一个可行走点，则开启下面的代码
//			Vector3 walkablePositionAround = exploreManager.GetComponent<MapGenerator>().GetAWalkablePositionAround (endPos);
//
//			yield return new WaitUntil (()=>realMapWalkableInfo [(int)walkablePositionAround.x, (int)walkablePositionAround.y] == 1);
//
			realMapWalkableInfo [(int)endPos.x, (int)endPos.y] = 10;

			bp.MoveToStoredDestination ();

		}


		public void AddIntoPool(){

		}



	}
}
