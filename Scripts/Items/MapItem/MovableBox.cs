using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace WordJourney
{
	using DG.Tweening;

	public class MovableBox : MapItem {

		public override void InitMapItem ()
		{
			bc2d.enabled = true;
		}

		public void OnAgentPushBox(BattlePlayerController bp,MapGenerator mapGenerator){

			ExploreManager em = mapGenerator.GetComponent<ExploreManager> ();
			em.DisableInteractivity ();

			int[,] mapWalkableInfo = mapGenerator.mapWalkableInfoArray;

			int playerPosX = Mathf.RoundToInt (bp.transform.position.x);
			int playerPosY = Mathf.RoundToInt (bp.transform.position.y);

			int boxPosX = Mathf.RoundToInt (this.transform.position.x);
			int boxPosY = Mathf.RoundToInt (this.transform.position.y);


			if (playerPosX < boxPosX && CanMove(mapWalkableInfo, boxPosX + 1, boxPosY)) {
				this.transform.DOMove (new Vector3 (boxPosX + 1, boxPosY),1.0f).OnComplete (delegate {
					mapWalkableInfo[boxPosX,boxPosY] = 0;
					mapWalkableInfo[boxPosX + 1,boxPosY] = 1;
					em.EnableInteractivity();
				});

			} else if (playerPosX > boxPosX && CanMove(mapWalkableInfo, boxPosX - 1, boxPosY)) {
				this.transform.DOMove (new Vector3 (boxPosX - 1, boxPosY),1.0f).OnComplete (delegate {
					mapWalkableInfo[boxPosX,boxPosY] = 0;
					mapWalkableInfo[boxPosX - 1,boxPosY] = 1;
					em.EnableInteractivity();
				});

			} else if (playerPosY < boxPosY && CanMove(mapWalkableInfo, boxPosX, boxPosY + 1)) {
				this.transform.DOMove (new Vector3 (boxPosX, boxPosY + 1),1.0f).OnComplete (delegate {
					mapWalkableInfo[boxPosX,boxPosY] = 0;
					mapWalkableInfo[boxPosX,boxPosY + 1] = 1;
					em.EnableInteractivity();
				});

			} else if (playerPosY > boxPosY && CanMove(mapWalkableInfo, boxPosX, boxPosY - 1)) {

				this.transform.DOMove (new Vector3 (boxPosX, boxPosY - 1),1.0f).OnComplete (delegate {
					mapWalkableInfo[boxPosX,boxPosY] = 0;
					mapWalkableInfo[boxPosX,boxPosY - 1] = 1;
					em.EnableInteractivity();
				});
			}

			em.EnableInteractivity ();

		}

		private bool CanMove(int[,] walkableInfo,int posX,int posY){
			return walkableInfo [posX, posY] == 1;
		}

	}
}
