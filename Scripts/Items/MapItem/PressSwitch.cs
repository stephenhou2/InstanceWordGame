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

//		private Transform m_controlledDoor;
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
