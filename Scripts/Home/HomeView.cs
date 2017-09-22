using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace WordJourney
{
	public class HomeView : MonoBehaviour {


		public Text playerLevelText;

		public Slider playerHealthBar;

		public Transform votexImage;

		private Tweener votexRotate;

		public Image maskImage;

		public void SetUpHomeView(){

			GetComponent<Canvas> ().enabled = true;

			SetUpTopBar ();

			VotexRotate ();

		}
		public void ShowMaskImage (){
			maskImage.gameObject.SetActive (true);
		}

		private void HideMaskImage(){
			maskImage.gameObject.SetActive (false);
		}

		public void VotexRotate(){

			if (votexRotate != null) {
				votexRotate.Play ();
				return;
			}

			votexRotate = votexImage.DOLocalRotate (new Vector3 (0, 0, 360),10.0f, RotateMode.FastBeyond360);
			votexRotate.SetLoops(-1);
			votexRotate.SetEase (Ease.Linear);
		}


		// 初始化顶部bar
		private void SetUpTopBar(){

			Player player = Player.mainPlayer;

			playerLevelText.text = player.agentLevel.ToString();

			playerHealthBar.maxValue = player.maxHealth;
			playerHealthBar.value = player.health;
			playerHealthBar.transform.Find ("HealthText").GetComponent<Text> ().text = player.health + "/" + Player.mainPlayer.maxHealth;

		}

		public void OnQuitHomeView(CallBack cb = null){

			if (cb == null) {
				GetComponent<Canvas> ().enabled = false;

				if (votexRotate != null) {
					votexRotate.Pause ();
				}

				HideMaskImage ();
			} else {
				votexRotate.Kill (false);
				cb ();
			}

		}

	}
}
