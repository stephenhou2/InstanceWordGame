using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace WordJourney
{
	public abstract class BattleAgentUIController : MonoBehaviour {

		public List<Tweener> allTweeners = new List<Tweener>();

		//血量槽
		public Slider healthBar;
		//血量值
		public Text healthText;

		public Text attackText;
		public Text attackSpeedText;
		public Text amourText;
		public Text manaResistText;
		public Text critText;
		public Text agilityText;

		//战斗中文字HUD
		public GameObject tintTextModel;

		private InstancePool tintTextPool;

		public Transform tintTextContainer;

//		public Text hurtText;
//		public Text gainText;


		public bool firstSetHealthBar = true;
		public bool firstSetStrengthBar = true;

		protected virtual void Awake(){

			tintTextPool = InstancePool.GetOrCreateInstancePool ("TintTextPool");

		}

		//The virtual keyword means AttemptMove can be overridden by inheriting classes using the override keyword.
		//AttemptMove takes a generic parameter T to specify the type of component we expect our unit to interact with if blocked (Player for Enemies, Wall for Player).
//		protected virtual void AttemptMove <T> (int xDir, int yDir)
//			where T : Component
//		{
//			//Hit will store whatever our linecast hits when Move is called.
//			RaycastHit2D hit;
//
//			//Set canMove to true if Move was successful, false if failed.
//			bool canMove = Move (xDir, yDir, out hit);
//
//			//Check if nothing was hit by linecast
//			if(hit.transform == null)
//				//If nothing was hit, return and don't execute further code.
//				return;
//
//			//Get a component reference to the component of type T attached to the object that was hit
//			T hitComponent = hit.transform.GetComponent <T> ();
//
//			//If canMove is false and hitComponent is not equal to null, meaning MovingObject is blocked and has hit something it can interact with.
//			if(!canMove && hitComponent != null)
//
//				//Call the OnCantMove function and pass it hitComponent as a parameter.
//				OnCantMove (hitComponent);
//		}


		//The abstract modifier indicates that the thing being modified has a missing or incomplete implementation.
		//OnCantMove will be overriden by functions in the inheriting classes.
//		protected abstract void OnCantMove <T> (T component)
//			where T : Component;
		



//		private IEnumerator PlayEffectAnimation(BaseSkillEffect bse){

//			effectAnimator.SetTrigger ("Thunder");
//
//			float effectAnimTime = effectAnimator.GetCurrentAnimatorStateInfo (0).length + 0.5f;
//
//			while (effectAnimTime > 0) {
//				effectAnimTime -= Time.deltaTime;
//				yield return null;
//			}

//			Debug.Log ("EffectAnimEnd");
//
//			effectAnimator.ResetTrigger ("Thunder");
//
//			effectAnimator.gameObject.SetActive (false);

//		}

		// 角色头像抖动动画
		public void PlayShakeAnim(){
	//		Tweener shakeAnim = agentIcon.transform.DOShakeRotation (0.5f, 10f);
	//		ManageAnimations(shakeAnim,null);
		}

		// 更新血量槽的动画（首次进入设置血量不播放动画，在ResetBattleAgentProperties（BattleAgent）后开启动画）
		public void UpdateHealthBarAnim(Agent ba){
			healthBar.maxValue = ba.maxHealth;
			healthText.text = ba.health + "/" + ba.maxHealth;
			if (firstSetHealthBar) {
				healthBar.value = ba.health;
			} else {
				healthBar.DOValue (ba.health, 0.2f);
			}
		}



		// 受到伤害文本动画
		public void PlayHurtTextAnim(string hurtStr, Vector3 agentPos, Towards towards, TintTextType tintTextType){


			Text hurtText = tintTextPool.GetInstance<Text> (tintTextModel, tintTextContainer);

			Vector3 originPos = Vector3.zero;
			Vector3 offset = Vector3.zero;
			Vector3 originTintPos = Vector3.zero;

			switch(towards){
			case Towards.Left:
				originPos = Camera.main.WorldToScreenPoint (agentPos) + new Vector3 (0, 100, 0);
				offset = new Vector3 (-100, 0, 0);
				originTintPos = originPos + new Vector3 (-100, 100, 0);
				break;
			case Towards.Right:
				originPos = Camera.main.WorldToScreenPoint (agentPos) + new Vector3 (100, 100, 0);
				offset = new Vector3(100, 0, 0);
				originTintPos = originPos + new Vector3 (100, 100, 0);
				break;
			}

			hurtText.transform.localPosition = originPos;

			Vector3 newPos = hurtText.transform.localPosition + offset;

			hurtText.text = hurtStr;

			hurtText.gameObject.SetActive (true);

			switch (tintTextType) {
			case TintTextType.None:
				break;
			case TintTextType.Crit:
				string tintStr = "<color=red>暴击</color>";
				PlayTintTextAnim (tintStr, originTintPos);
				break;
			case TintTextType.Miss:
				tintStr = "<color=gray>miss</color>";
				PlayTintTextAnim (tintStr, originTintPos);
				return;
			}
					
			hurtText.transform.DOLocalJump (newPos, 100f, 1, 0.35f).OnComplete(()=>{

				switch(towards){
				case Towards.Left:
					offset = new Vector3 (-30, 0, 0);
					break;
				case Towards.Right:
					offset = new Vector3(30, 0, 0);
					break;
				}

				newPos = hurtText.transform.localPosition + offset;

				hurtText.transform.DOLocalJump (newPos, 20f, 1, 0.15f).OnComplete(()=>{
					hurtText.gameObject.SetActive(false);
					tintTextPool.AddInstanceToPool(hurtText.gameObject);
				});

			});

		}

		public void PlayGainTextAnim(string gainStr, Vector3 agentPos){

			Vector3 pos = Camera.main.WorldToScreenPoint (agentPos) + new Vector3(50,100,0);

			Text gainText = tintTextPool.GetInstance<Text> (tintTextModel, tintTextContainer);

			gainText.transform.localPosition = pos;

			gainText.text = gainStr;

			gainText.gameObject.SetActive (true);

			gainText.transform.DOLocalMoveY (100, 0.5f).OnComplete(()=>{
				gainText.gameObject.SetActive(false);
				tintTextPool.AddInstanceToPool(gainText.gameObject);
			});
				
		}
			
		private void PlayTintTextAnim(string tintStr, Vector3 originPos){
			
			Text tintText = tintTextPool.GetInstance<Text> (tintTextModel, tintTextContainer);

			tintText.transform.localPosition = originPos;

			tintText.text = tintStr;

			tintText.gameObject.SetActive (true);

			tintText.transform.DOScale(new Vector3(1.5f,1.5f,1.5f),0.5f).OnComplete (() => {

				tintText.gameObject.SetActive(false);

				tintTextPool.AddInstanceToPool(tintText.gameObject);

			});
		}
			
//		// 动画管理方法，复杂回调单独写函数传入，简单回调使用拉姆达表达式
//		private void ManageAnimations(Tweener newTweener,CallBack tc){
//			allTweeners.Add (newTweener);
//
//			newTweener.OnComplete (
//				() => {
//					allTweeners.Remove (newTweener);
//					if (tc != null) {
//						tc ();
//					}
//				});
//
//		}

	}
}
