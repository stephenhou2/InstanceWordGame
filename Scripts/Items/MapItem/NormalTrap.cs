using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class NormalTrap : Trap {

		public int lifeLose;

		// 陷阱打开状态的图片
		public Sprite trapOnSprite;
		// 陷阱关闭状态的图片
		public Sprite trapOffSprite;

		private IEnumerator normalTrapTriggeredCoroutine;

		private Vector3 agentOriPos;
		private Vector3 backgroundOriPos;


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
			SetSortingOrder (-(int)transform.position.y - 1);//普通陷阱紧贴地面，不会挡住人，所以层级再下降一层，防止人在上面走的时候挡住人）
		}

		public override void AddToPool (InstancePool pool)
		{
			bc2d.enabled = false;
			pool.AddInstanceToPool (this.gameObject);
		}



		public override void SetTrapOn ()
		{
//			mExploreManager.GetComponent<MapGenerator> ().mapWalkableInfoArray [(int)transform.position.x, (int)transform.position.y] = 10;
			mapItemRenderer.sprite = trapOnSprite;
			isTrapOn = true;
		}

		public override void SetTrapOff ()
		{
//			mExploreManager.GetComponent<MapGenerator> ().mapWalkableInfoArray [(int)transform.position.x, (int)transform.position.y] = 1;
			mapItemRenderer.sprite = trapOffSprite;
			isTrapOn = false;
		}


		public override void OnTriggerEnter2D (Collider2D col)
		{

			if (!isTrapOn) {
				return;
			}

			SoundManager.Instance.PlayAudioClip("MapEffects/" + audioClipName);

			BattlePlayerController bp = col.GetComponent<BattlePlayerController> ();

			if (bp == null) {
				return;
			}

//			if(MyTool.ApproximatelySamePosition2D(bp.transform.position,this.transform.position)){

			agentOriPos = new Vector3 (Mathf.RoundToInt(col.transform.position.x), Mathf.RoundToInt(col.transform.position.y), 0);

			backgroundOriPos = Camera.main.transform.Find ("Background").position;

//			}else{

//				agentOriPos = new Vector3 (Mathf.RoundToInt(col.transform.position.x), Mathf.RoundToInt(col.transform.position.y), 0);
//
//				backgroundOriPos = Camera.main.transform.Find ("Background").position;

				Debug.Log (backgroundOriPos);

//			}

			Debug.LogFormat ("oriPos:{0}----currentAgentPos:{1}", agentOriPos, col.transform.position);


			bp.StopMove ();
			bp.singleMoveEndPos = agentOriPos;
			bp.InitFightTextDirectionTowards (transform.position);


			bp.propertyCalculator.InstantPropertyChange (bp, PropertyType.Health, -lifeLose, false);


			if (normalTrapTriggeredCoroutine != null) {
				StopCoroutine (normalTrapTriggeredCoroutine);
			}

			normalTrapTriggeredCoroutine = NormalTrapTriggerEffect (bp);

			StartCoroutine (normalTrapTriggeredCoroutine);

		}



		private IEnumerator NormalTrapTriggerEffect(BattleAgentController ba){

			float timer = 0;

			float agentBackDuration = 0.1f;

			Vector3 backgroundBackVector = (ba.transform.position - agentOriPos) * 0.3f;

			while (timer < agentBackDuration) {

				Vector3 agentBackVector = new Vector3 (
					(agentOriPos.x - ba.transform.position.x) / agentBackDuration,
					(agentOriPos.y - ba.transform.position.y) / agentBackDuration, 0) * Time.deltaTime;

				ba.transform.position += agentBackVector;
				timer += Time.deltaTime;
				yield return null;
			}

			ba.transform.position = agentOriPos;

			Camera.main.transform.Find ("Background").position += backgroundBackVector;

			Debug.Log (Camera.main.transform.Find ("Background").position);

		}


	}
}
