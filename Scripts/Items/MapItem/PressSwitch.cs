using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class PressSwitch : MapItem {

		private bool isPressOn;

		public Sprite switchOffSprite;

		public Sprite switchOnSprite;


		public Door controlledDoor;

		private Transform m_exploreManager;
		private Transform exploreManager{
			get{
				if (m_exploreManager == null) {
					m_exploreManager = TransformManager.FindTransform ("ExploreManager");
				}
				return m_exploreManager;
			}
		}

		private BattlePlayerController m_battlePlayer;
		private BattlePlayerController battlePlayer{
			get{
				if (m_battlePlayer == null) {
					m_battlePlayer = Player.mainPlayer.transform.Find ("BattlePlayer").GetComponent<BattlePlayerController> ();
				}
				return m_battlePlayer;
			}
		}


//		private float cameraMoveDuration = 1.0f;
		private Vector3 playerOriginalDestination;

		public override void InitMapItem ()
		{
			isPressOn = false;
			bc2d.enabled = true;
			mapItemRenderer.sprite = switchOnSprite;
			SetSortingOrder (1);
		}

		public override void AddToPool (InstancePool pool)
		{
			bc2d.enabled = false;
			pool.AddInstanceToPool (this.gameObject);
		}

		public void ResetPressSwitch(Door door){
			mapItemRenderer.sprite = switchOffSprite;
			controlledDoor = door;
		}

		void OnTriggerEnter2D(Collider2D other){

			if (other.GetComponent<BattleAgentController> () == null && other.GetComponent<MovableBox>() == null) {
				return;
			}

			PressOnSwitch ();

		}

		private void PressOnSwitch(){
			mapItemRenderer.sprite = switchOffSprite;
			controlledDoor.GetComponent<Door> ().OpenTheDoor ();
		}

		private void PressOffSwitch(){
			mapItemRenderer.sprite = switchOnSprite;
			controlledDoor.GetComponent<Door> ().CloseTheDoor ();
		}

		void OnTriggerExit2D(Collider2D other){

			PressOffSwitch();

		}

	}
}
