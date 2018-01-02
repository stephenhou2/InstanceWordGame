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

//		public Text attackText;
//		public Text attackSpeedText;
//		public Text armorText;
//		public Text manaResistText;
//		public Text critText;
//		public Text dodgeText;

		//战斗中文本模型
		public Transform fightTextModel;

		// 文本缓存池
		private InstancePool fightTextPool;

		// 文本在场景中的容器
		public Transform fightTextContainer;


		public bool firstSetHealthBar = true;
//		public bool firstSetManaBar = true;

		public FightTextManager fightTextManager;

		protected void Start(){

			Transform poolContainerOfExploreCanvas = TransformManager.FindOrCreateTransform (CommonData.poolContainerName + "/PoolContainerOfExploreCanvas");
//			Transform modelContainerOfExploreScene = TransformManager.FindOrCreateTransform(CommonData.instanceContainerName + "/ModelContainerOfExploreScene");

			fightTextPool = InstancePool.GetOrCreateInstancePool ("TintTextPool",poolContainerOfExploreCanvas.name);

			fightTextPool.transform.SetParent (poolContainerOfExploreCanvas);
//			fightTextModel.SetParent (modelContainerOfExploreScene);

			fightTextManager.InitFightTextManager (fightTextPool, fightTextModel, fightTextContainer);

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
