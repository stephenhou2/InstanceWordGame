using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class PressSwitch : MapItem {

		private bool isPressOn;

		private Sprite m_SwitchOffSprite;
		private Sprite switchOffSprite{
			get{
				if (m_SwitchOffSprite == null) {
					m_SwitchOffSprite = GameManager.Instance.gameDataCenter.allMapSprites.Find (delegate(Sprite obj) {
						return obj.name == "press_switch_off";
					});
				}
				return m_SwitchOffSprite;
			}
		}

		private Sprite m_SwitchOnSprite;
		private Sprite switchOnSprite{
			get{
				if (m_SwitchOnSprite == null) {
					m_SwitchOnSprite = GameManager.Instance.gameDataCenter.allMapSprites.Find (delegate(Sprite obj) {
						return obj.name == "press_switch_on";
					});
				}
				return m_SwitchOnSprite;
			}
		}

		private Transform m_controlledDoor;
		private Transform controlledDoor{
			get{
				if (m_controlledDoor == null) {
					m_controlledDoor = exploreManager.GetComponent<MapGenerator> ().GetDoor();
				}
				return m_controlledDoor;
			}
			set{ m_controlledDoor = value; }
		}

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


		private float cameraMoveDuration = 1.0f;
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

		public void ResetPressSwitch(Transform door){
			mapItemRenderer.sprite = switchOffSprite;
			controlledDoor = door;
		}

		void OnTriggerEnter2D(Collider2D other){

			if (other.GetComponent<BattleAgentController> () == null && other.GetComponent<MovableBox>() == null) {
				return;
			}
				
//			isPressOn = true;
//
//			playerOriginalDestination = battlePlayer.moveDestination;
//
//			battlePlayer.StopMoveAtEndOfCurrentStep ();
//
//			exploreManager.GetComponent<ExploreManager> ().DisableInteractivity ();
//
//			StartCoroutine ("ShowDoorPosAndChangeDoorStatus");

			PressOnSwitch ();

		}

		private void PressOnSwitch(){
			mapItemRenderer.sprite = switchOffSprite;
			controlledDoor.GetComponent<Door> ().OpenTheDoor ();
		}

		private void PressOffSwich(){
			mapItemRenderer.sprite = switchOnSprite;
			controlledDoor.GetComponent<Door> ().CloseTheDoor ();
		}

		void OnTriggerExit2D(Collider2D other){

//			if (battlePlayer.isInFight) {
//				StartCoroutine ("WaitFightEndAndExitPressSwitch");
//			} else {
//				ExitPressSwitch ();
//			}
			PressOffSwich();

		}

		private IEnumerator WaitFightEndAndExitPressSwitch(){
			yield return new WaitUntil (() => !battlePlayer.isInFight);
			ExitPressSwitch ();
		}

		private void ExitPressSwitch(){

			playerOriginalDestination = battlePlayer.moveDestination;

			isPressOn = false;

			battlePlayer.StopMoveAtEndOfCurrentStep ();

			exploreManager.GetComponent<ExploreManager> ().DisableInteractivity ();

			StartCoroutine ("ShowDoorPosAndChangeDoorStatus");
		}

		private IEnumerator ShowDoorPosAndChangeDoorStatus(){

			if (isPressOn) {
				mapItemRenderer.sprite = switchOffSprite;
			}

			yield return new WaitUntil (() => battlePlayer.isIdle);

			if (!isPressOn) {
				mapItemRenderer.sprite = switchOnSprite;
			}

			Camera mainCam = Camera.main;

			float timer = 0;

			float camSpeedX = (controlledDoor.position.x - mainCam.transform.position.x) / cameraMoveDuration;
			float camSpeedY = (controlledDoor.position.y - mainCam.transform.position.y) / cameraMoveDuration;

			while (timer < cameraMoveDuration) {

				Vector3 camMoveVector = new Vector3 (camSpeedX * Time.deltaTime, camSpeedY * Time.deltaTime, 0);

				mainCam.transform.position += camMoveVector;

				timer += Time.deltaTime;

				yield return null;
			}

			if (isPressOn) {
				controlledDoor.GetComponent<Door> ().OpenTheDoor ();
			} else {
				controlledDoor.GetComponent<Door> ().CloseTheDoor ();
			}

			yield return new WaitForSeconds (0.5f);

			timer = 0;

			while (timer < cameraMoveDuration) {

				Vector3 camMoveVector = new Vector3 (camSpeedX * Time.deltaTime, camSpeedY * Time.deltaTime, 0);

				mainCam.transform.position -= camMoveVector;

				timer += Time.deltaTime;

				yield return null;
			}

			mainCam.transform.localPosition = new Vector3 (0, 0, -10);

			exploreManager.GetComponent<ExploreManager> ().EnableInteractivity ();

			int[,] mapWalkableInfoArray = exploreManager.GetComponent<MapGenerator> ().mapWalkableInfoArray;

			battlePlayer.MoveToPosition (playerOriginalDestination, mapWalkableInfoArray);

		}
			

	}
}
