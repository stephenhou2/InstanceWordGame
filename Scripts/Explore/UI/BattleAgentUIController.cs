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
		public Text armorText;
		public Text manaResistText;
		public Text critText;
		public Text dodgeText;

		//战斗中文本模型
		private Transform tintTextModel;

		// 文本缓存池
		private InstancePool tintTextPool;

		// 文本在场景中的容器
		public Transform tintTextContainer;

//		public Text hurtText;
//		public Text gainText;


		public bool firstSetHealthBar = true;
		public bool firstSetManaBar = true;



		protected void Start(){

			Transform poolContainerOfExploreCanvas = TransformManager.FindOrCreateTransform (CommonData.poolContainerName + "/PoolContainerOfExploreCanvas");
			Transform modelContainerOfExploreScene = TransformManager.FindOrCreateTransform(CommonData.instanceContainerName + "/ModelContainerOfExploreScene");

			tintTextPool = InstancePool.GetOrCreateInstancePool ("TintTextPool",poolContainerOfExploreCanvas.name);
			tintTextModel = TransformManager.FindTransform ("TintTextModel");

//			tintTextPool.transform.SetParent (poolContainerOfExploreCanvas);
			tintTextModel.SetParent (modelContainerOfExploreScene);

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
			
		/// <summary>
		/// 世界坐标转2D画布中的坐标
		/// </summary>
		/// <returns>The point in canvas.</returns>
		/// <param name="worldPos">World position.</param>
		private Vector3 ToPointInCanvas(Vector3 worldPos){

			Vector3 posInScreen = Camera.main.WorldToScreenPoint (worldPos);

			Vector3 posInCanvas = new Vector3 (posInScreen.x * CommonData.scalerToPresetResulotion, posInScreen.y * CommonData.scalerToPresetResulotion, posInScreen.z);

			return posInCanvas;

		}

		// 受到伤害文本动画
		public void PlayHurtTextAnim(string hurtStr, Vector3 agentPos, Towards towards, TintTextType tintTextType){

			// 从缓存池获取文本模型
			Text hurtText = tintTextPool.GetInstance<Text> (tintTextModel.gameObject, tintTextContainer);

		
			Vector3 originHurtPos = Vector3.zero;
			Vector3 finalHurtPos = Vector3.zero;
			Vector3 originTintPos = Vector3.zero;
			Vector3 finalTintPos = Vector3.zero;


			// 获得人物在画布中的位置
			Vector3 agentPosInCanvas = ToPointInCanvas (agentPos);


			// originHurtPos: 伤害文本的初始位置
			// finalHurtPos: 伤害文本的最终位置
			// originTintPos: 效果文本的初始位置
			switch(towards){
			case Towards.Left:
				originHurtPos = agentPosInCanvas + new Vector3 (-50f, 50f, 0);
				finalHurtPos = originHurtPos + new Vector3 (-100f, 0, 0);
				originTintPos = originHurtPos + new Vector3 (-100f, 100f, 0);
				break;
			case Towards.Right:
				originHurtPos = agentPosInCanvas + new Vector3 (50f, 50f, 0);
				finalHurtPos = originHurtPos + new Vector3 (100f, 0, 0);
				originTintPos = originHurtPos + new Vector3 (100f, 100f, 0);
				break;
			}
				
			hurtText.transform.localPosition = originHurtPos;

			hurtText.text = hurtStr;

			hurtText.gameObject.SetActive (true);

			// 根据效果类型播放效果文本动画
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

			// 伤害文本跳跃动画
			hurtText.transform.DOLocalJump (finalHurtPos, 100f, 1, 0.35f).OnComplete(()=>{

				switch(towards){
				case Towards.Left:
					finalHurtPos += new Vector3 (-30f, 0, 0);
					break;
				case Towards.Right:
					finalHurtPos += new Vector3 (30f, 0, 0);
					break;
				}

				// 伤害文本二次跳跃
				hurtText.transform.DOLocalJump (finalHurtPos, 20f, 1, 0.15f).OnComplete(()=>{
					hurtText.gameObject.SetActive(false);
					tintTextPool.AddInstanceToPool(hurtText.gameObject);
				});

			});

		}

		/// <summary>
		/// 吸血文本动画
		/// </summary>
		/// <param name="gainStr">Gain string.</param>
		/// <param name="agentPos">Agent position.</param>
		public void PlayGainTextAnim(string gainStr, Vector3 agentPos, Towards towards){
			
			Vector3 pos = Vector3.zero;

			switch (towards) {
			case Towards.Left:
				pos = ToPointInCanvas (agentPos) + new Vector3 (-50f, 150f, 0);
				break;
			case Towards.Right:
				pos = ToPointInCanvas (agentPos) + new Vector3 (50f, 150f, 0);
				break;
			}

			Text gainText = tintTextPool.GetInstance<Text> (tintTextModel.gameObject, tintTextContainer);

			gainText.transform.localPosition = pos;

			gainText.text = gainStr;

			gainText.gameObject.SetActive (true);

			float endY = pos.y + 100f;

			gainText.transform.DOLocalMoveY (endY, 1f).OnComplete(()=>{
				gainText.gameObject.SetActive(false);
				tintTextPool.AddInstanceToPool(gainText.gameObject);
			});
				
		}
			
		/// <summary>
		/// 效果提示文本动画（暴击，闪避）
		/// </summary>
		/// <param name="tintStr">Tint string.</param>
		/// <param name="originPos">Origin position.</param>
		private void PlayTintTextAnim(string tintStr, Vector3 originPos){
			
			Text tintText = tintTextPool.GetInstance<Text> (tintTextModel.gameObject, tintTextContainer);

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
