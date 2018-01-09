using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class AttackCheckController : MonoBehaviour {

		private enum AttackCheckType
		{
			Rect,
			Circle
		}

		public Transform rectAttackCheckPlane;
		public RectTransform rectAttackZone;
		public RectTransform rectAttackCheck;
		public RectTransform rectValidZone;
		public float rectAttackCheckMoveSpeed;
		public float rectAttackZoneMoveSpeed;
		public Image healthInRectAttackCheck;

		public Transform circleAttackCheckPlane;
		public RectTransform circleAttackZone;
		public RectTransform circleAttackCheck;
		public RectTransform circleValidZone;
		public float circleAttackCheckRotateSpeed;
		public float circleAttackZoneRotateSpeed;
		public Image healthInCircleAttackCheck;

	
		private float validZoneEdgeLeft{
			get{ return rectValidZone.rect.center.x - rectValidZone.rect.width / 2; }
		}
		private float validZoneEdgeRight{
			get{ return rectValidZone.rect.center.x + rectValidZone.rect.width / 2;}
		}

		private AttackCheckType attackCheckType;

		private float rectAttackZoneSize;
		private float circleAttackZoneSize;


		public void StartRectAttackCheck(){
			attackCheckType = AttackCheckType.Rect;
			rectAttackCheckPlane.gameObject.SetActive (true);
			circleAttackCheckPlane.gameObject.SetActive (false);
			rectAttackCheck.localPosition = Vector3.zero;
			rectAttackZoneSize = rectAttackZone.sizeDelta.x;
			rectAttackZone.localPosition = new Vector3 (Random.Range (validZoneEdgeLeft, validZoneEdgeRight), 0, 0);
			StartCoroutine ("RectAttackCheckMove");
			StartCoroutine ("RectAttackZoneMove");

		}

		public void StartCircleAttackCheck(){
			attackCheckType = AttackCheckType.Circle;
			rectAttackCheckPlane.gameObject.SetActive (false);
			circleAttackCheckPlane.gameObject.SetActive (true);
			circleAttackCheck.localRotation = Quaternion.identity;
			circleAttackZoneSize = circleAttackZone.GetComponent<Image> ().fillAmount * 360;
			float originalRotationOffset = Random.Range (circleAttackZoneSize -  90, 90);
			circleAttackZone.localRotation = Quaternion.Euler(new Vector3(0,0,originalRotationOffset));
			StartCoroutine ("CircleAttackCheckRotate");
			StartCoroutine ("CircleAttackZoneRotate");
		}

//		public void ResetRectAttackCheckPosition(){
//			rectAttackCheck.localPosition = Vector3.zero;
//		}

		private IEnumerator RectAttackCheckMove(){

			int direction = 1;

			while (true) {

				if (rectAttackCheck.localPosition.x > validZoneEdgeRight) {
					direction = -1;
				} else if (rectAttackCheck.localPosition.x < validZoneEdgeLeft) {
					direction = 1;
				}

				Vector3 attackCheckPositionFix = new Vector3 (rectAttackCheckMoveSpeed * Time.deltaTime * direction, 0, 0);

				rectAttackCheck.localPosition += attackCheckPositionFix;

				yield return null;
			}

		}


		private IEnumerator RectAttackZoneMove(){

			int direction = 1;

			while (true) {

				if (rectAttackZone.localPosition.x > validZoneEdgeRight - rectAttackZoneSize) {
					direction = -1;
				} else if (rectAttackZone.localPosition.x < validZoneEdgeLeft) {
					direction = 1;
				}

				bool inAttackZone = RectCheckInAttackZone ();

				if (inAttackZone) {
					rectAttackZone.GetComponent<Image> ().color = Color.green;
				} else {
					rectAttackZone.GetComponent<Image> ().color = Color.blue;
				}

				Vector3 attackZonePositionFix = new Vector3 (rectAttackZoneMoveSpeed * Time.deltaTime * direction, 0, 0);

				rectAttackZone.position += attackZonePositionFix;

				yield return null;
			}

		}


		private IEnumerator CircleAttackCheckRotate(){

			int direction = 1;

			while (true) {

				float formerRotation = circleAttackCheck.localRotation.eulerAngles.z;

				if (formerRotation > 90 && formerRotation < 180) {
					direction = 1;
				} else if (formerRotation < 270 && formerRotation > 180) {
					direction = -1;
				}

				float rotation = circleAttackCheckRotateSpeed * Time.deltaTime * direction;

				circleAttackCheck.localRotation = Quaternion.Euler (new Vector3(0,0,formerRotation - rotation));

				yield return null;

			}

		}

		private IEnumerator CircleAttackZoneRotate(){

			int direction = 1;

			while (true) {

				float formerRotation = circleAttackZone.localRotation.eulerAngles.z;

				if (formerRotation > 90 && formerRotation < 180) {
					direction = 1;
				} else if (formerRotation < 270 + circleAttackZoneSize && formerRotation > 180) {
					direction = -1;
				}

				bool inAttackZone = CircleCheckInAttackZone ();

				if (inAttackZone) {
					circleAttackZone.GetComponent<Image> ().color = Color.green;
				} else {
					circleAttackZone.GetComponent<Image> ().color = Color.blue;
				}

				float rotation = circleAttackZoneRotateSpeed * Time.deltaTime * direction;

				circleAttackZone.localRotation = Quaternion.Euler (new Vector3 (0, 0, formerRotation - rotation));

				yield return null;

			}

		}


		public void RectAttackButtonClicked(){
			if (RectCheckInAttackZone()) {
				BattlePlayerController bpCtr = Player.mainPlayer.transform.Find ("BattlePlayer").GetComponent<BattlePlayerController> ();
				bpCtr.UseDefaultSkill ();
			}
		}

		private bool RectCheckInAttackZone(){

			Vector2 checkCenter = rectAttackCheck.localPosition;

//			Debug.LogFormat ("check center x:{0}", checkCenter.x);

			float attackZoneLeftEdge = rectAttackZone.localPosition.x;

			float attackZoneRightEdgeX = attackZoneLeftEdge + rectAttackZone.rect.width;

//			Debug.LogFormat ("attack zone left:{0}  attack zone right:{1}", attackZoneLeftEdge, attackZoneRightEdgeX);

			return checkCenter.x >= attackZoneLeftEdge && checkCenter.x <= attackZoneRightEdgeX;

		}



		public void CircleAttackButtonClicked(){
			if (CircleCheckInAttackZone()) {
				BattlePlayerController bpCtr = Player.mainPlayer.transform.Find ("BattlePlayer").GetComponent<BattlePlayerController> ();
				bpCtr.UseDefaultSkill ();
			}
		}

		private bool CircleCheckInAttackZone(){

			float circleAttackCheckRotation = GetRotationWithTransformToMyAxis (circleAttackCheck);

			float circleAttackZoneRotation = GetRotationWithTransformToMyAxis (circleAttackZone);

			if (circleAttackCheckRotation - circleAttackZoneRotation > 0 &&
				circleAttackCheckRotation - circleAttackZoneRotation < circleAttackZoneSize) {
				return true;
			}
				
//			Debug.LogFormat ("未命中： 指针位置：{0}，命中区域位置：[{1},{2}]", circleAttackCheckRotation, circleAttackZoneRotation, circleAttackZoneRotation + circleAttackZoneSize);

			return false;

		}

		private float GetRotationWithTransformToMyAxis(Transform trans){

			float rotation = trans.localRotation.eulerAngles.z;

			if (rotation >= 0 && rotation <= 90) {
				rotation = 90 - rotation;
			} else if (rotation > 90 && rotation <= 180) {
				rotation = 0;
			} else if (rotation > 180 && rotation < 270) {
				rotation = 180;
			} else if (rotation >= 270 && rotation <= 360) {
				rotation = 450 - rotation;
			} 

			return rotation;

		}

		public void UpdateHealth(){

			switch (attackCheckType) {
			case AttackCheckType.Circle:
				healthInCircleAttackCheck.fillAmount = 0.5f * Player.mainPlayer.health / Player.mainPlayer.maxHealth;
				break;
			case AttackCheckType.Rect:
				healthInRectAttackCheck.fillAmount = Player.mainPlayer.health / Player.mainPlayer.maxHealth;
				break;
			}



		}



		public void QuitAttackCheck(){

			switch (attackCheckType) {
			case AttackCheckType.Rect:
				StopCoroutine ("RectAttackCheckMove");
				StopCoroutine ("RectAttackZoneMove");
				break;
			case AttackCheckType.Circle:
				StopCoroutine ("CircleAttackCheckRotate");
				StopCoroutine ("CircleAttackZoneRotate");
				break;
			}

			rectAttackCheckPlane.gameObject.SetActive (false);
			circleAttackCheckPlane.gameObject.SetActive (false);

		}

	}
}
