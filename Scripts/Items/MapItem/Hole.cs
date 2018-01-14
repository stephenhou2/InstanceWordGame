using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using DG.Tweening;

	public class Hole : MapItem {

//		private Transform mExploreManager;
//		private Transform exploreManager{
//			get{
//				if (mExploreManager == null) {
//					mExploreManager = TransformManager.FindTransform ("ExploreManager");
//				}
//				return mExploreManager;
//			}
//		}

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
			
		public void EnterHole(MapGenerator mapGenerator,BattlePlayerController bp){

			bp.ActiveBattlePlayer (false, false, false);

			List<Hole> allHolesInMap = mapGenerator.GetAllHolesInMap ();

			Hole randomOtherHole = GetRandomOtherHole (allHolesInMap);

			Debug.LogFormat ("hole found,time:{0}", Time.realtimeSinceStartup);

			mapGenerator.GetComponent<ExploreManager> ().DisableInteractivity ();

			Vector3 otherHolePosition = new Vector3(randomOtherHole.transform.position.x,randomOtherHole.transform.position.y,0);

			Vector3 walkablePositionAround = mapGenerator.GetARandomWalkablePositionAround (otherHolePosition);

			mapGenerator.PlayMapOtherAnim ("HoleFog", this.transform.position);


			if(randomOtherHole.transform.position.z != 0){
				mapGenerator.DirectlyShowSleepingTilesAtPosition(otherHolePosition);
			}

			mapGenerator.ItemsAroundAutoIntoLifeWithBasePoint(otherHolePosition);

			bp.transform.DOMove(otherHolePosition,1).OnComplete(delegate{

				IEnumerator WalkOutOfHoleCoroutine = WalkOutOfHole(mapGenerator,walkablePositionAround,bp,randomOtherHole);

				StartCoroutine(WalkOutOfHoleCoroutine);

			});



		}

		private IEnumerator WalkOutOfHole(MapGenerator mapGenerator,Vector3 walkablePositionAround,BattlePlayerController bp,Hole targetHole){

			targetHole.bc2d.enabled = false;

			int[,] mapWalkableInfo = mapGenerator.mapWalkableInfoArray;

			yield return new WaitUntil(()=> mapWalkableInfo[(int)walkablePositionAround.x,(int)walkablePositionAround.y] == 1);

			mapGenerator.PlayMapOtherAnim("HoleFog",targetHole.transform.position);

			Debug.LogFormat ("around items come to life,time:{0}", Time.realtimeSinceStartup);

			bp.MoveToPosition(walkablePositionAround,mapGenerator.mapWalkableInfoArray);

			yield return new WaitUntil (() => bp.isIdle);

			Debug.LogFormat ("player move End:{0}", Time.realtimeSinceStartup);

			targetHole.bc2d.enabled = true;

			mapGenerator.GetComponent<ExploreManager> ().EnableInteractivity ();

		}

		private Hole GetRandomOtherHole(List<Hole> allHolesInMap){

			int randomHoleIndex = Random.Range (0, allHolesInMap.Count);

			if (allHolesInMap [randomHoleIndex].transform.position == this.transform.position) {
				return GetRandomOtherHole (allHolesInMap);
			}

			return allHolesInMap[randomHoleIndex];

		}


	}
}
