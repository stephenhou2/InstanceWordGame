using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace WordJourney
{
	public abstract class BattleAgentUIController : MonoBehaviour {

		//血量槽
		public Slider healthBar;
		//血量值
		public Text healthText;

		public Text attackText;
		public Text attackSpeedText;
		public Text armourText;
		public Text manaResistText;
		public Text critText;
		public Text dodgeText;

		//战斗中文本模型
		public GameObject tintTextModel;

		// 文本缓存池
		private InstancePool tintTextPool;

		// 文本在场景中的容器
		public Transform tintTextContainer;

//		public Text hurtText;
//		public Text gainText;


		public bool firstSetHealthBar = true;
		public bool firstSetManaBar = true;



		protected virtual void Awake(){

			tintTextPool = InstancePool.GetOrCreateInstancePool ("TintTextPool");

		}

		// 更新血量槽的动画（首次进入设置血量不播放动画，在ResetBattleAgentProperties（BattleAgent）后开启动画）
		protected void UpdateHealthBarAnim(Agent ba){
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

			float endY = pos.y + 50f;

			gainText.transform.DOLocalMoveY (endY, 1f).OnComplete(()=>{
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

				tintText.transform.localScale = Vector3.one;

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

		public abstract void QuitFight ();

	}
}
