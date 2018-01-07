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

		public FightTextManager fightTextManager;

		public Transform statusTintContainer;
		protected Transform statusTintModel;
		protected InstancePool statusTintPool;

		protected void Start(){

			Transform poolContainerOfExploreCanvas = TransformManager.FindOrCreateTransform (CommonData.poolContainerName + "/PoolContainerOfExploreCanvas");
//			Transform modelContainerOfExploreScene = TransformManager.FindOrCreateTransform(CommonData.instanceContainerName + "/ModelContainerOfExploreScene");

			fightTextPool = InstancePool.GetOrCreateInstancePool ("FightTextPool",poolContainerOfExploreCanvas.name);

			fightTextPool.transform.SetParent (poolContainerOfExploreCanvas);
//			fightTextModel.SetParent (modelContainerOfExploreScene);

			fightTextManager.InitFightTextManager (fightTextPool, fightTextModel, fightTextContainer);

		}

		public abstract void UpdateAgentStatusPlane ();

		// 更新血量槽的动画（首次进入设置血量不播放动画，在ResetBattleAgentProperties（BattleAgent）后开启动画）
		protected void UpdateHealthBarAnim(Agent agent){
			healthBar.maxValue = agent.maxHealth;
			healthText.text = agent.health + "/" + agent.maxHealth;
			if (firstSetHealthBar) {
				healthBar.value = agent.health;
			} else {
				healthBar.DOValue (agent.health, 0.2f);
			}
		}
			
		protected void UpdateSkillStatusPlane(Agent agent){

			statusTintPool.AddChildInstancesToPool (statusTintContainer);

			for (int i = 0; i < agent.allStatus.Count; i++) {
				string status = agent.allStatus [i];
				Transform statusTint = statusTintPool.GetInstance<Transform> (statusTintModel.gameObject, statusTintContainer);
				statusTint.GetComponent<Image> ().sprite = GameManager.Instance.gameDataCenter.allSkillSprites.Find (delegate(Sprite obj) {
					return obj.name == status;
				});
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
