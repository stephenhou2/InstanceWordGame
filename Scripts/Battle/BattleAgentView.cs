using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public abstract class BattleAgentView : MonoBehaviour {

	public List<Tweener> allTweeners = new List<Tweener>();

	public Slider healthBar;//血量槽
	public Text healthText;//血量值

	public Slider strengthBar;//气力槽
	public Text strengthText;//气力值

	public Text hurtHUD;//血量提示文字
	public GameObject statesPlane;//状态图片容器
	public Image[] stateImages; // 状态图片

	public Image agentIcon; //角色头像
	public Image effectMaskImg; // 攻击效果图片遮罩

	[HideInInspector]public Transform skillsContainer;

	[HideInInspector]public Transform statesContainer;

	public bool firstSetHealthBar = true;
	public bool firstSetStrengthBar = true;



	public bool isAnimating{

		get{ 
			foreach (Tweener t in allTweeners) {
				if (!t.IsComplete())
					return true;
			}
			if (allTweeners.Count == 0) {
				return false;
			}
			return false;
		}
	}

	public void OnTriggerAnim(TriggerType triggerType){
		if (triggerType == TriggerType.BePhysicalHit) {
			agentIcon.transform.DOShakeRotation (0.5f,20f);
			effectMaskImg.DOFillAmount (1.0f, 0.2f).OnComplete(()=>{effectMaskImg.fillAmount = 0f;});
		}
	}

	// 更新血量槽的动画（首次进入设置血量不播放动画，在ResetBattleAgentProperties（BattleAgent）后开启动画）
	public void UpdateHealthBarAnim(BattleAgent ba){
		healthBar.maxValue = ba.maxHealth;
		healthText.text = ba.health + "/" + ba.maxHealth;
		if (firstSetHealthBar) {
			healthBar.value = ba.health;
		} else {
			healthBar.DOValue (ba.health, 0.5f);
		}
	}

	public void UpdateStrengthBarAnim(BattleAgent ba){
		strengthBar.maxValue = ba.maxStrength;
		strengthText.text = ba.strength + "/" + ba.maxStrength;


		if (firstSetStrengthBar) {
			strengthBar.value = ba.strength;
		} else {
			strengthBar.DOValue (ba.strength, 0.5f);
		}

	}

	// 伤害文本动画
	public void PlayHurtHUDAnim(string text){

		hurtHUD.fontSize = 40;

		hurtHUD.text = text;

		hurtHUD.GetComponent<Text>().enabled = true;

		Tweener scaleAnim = hurtHUD.transform.DOScale (2.5f, 0.5f);

		ManageAnimations(scaleAnim,()=>{

			Vector3 newPos = hurtHUD.transform.localPosition + new Vector3 (0, 100, 0);

			Tweener positionAnim = hurtHUD.transform.DOLocalMove (newPos, 1.0f, false);

			Tweener colorAnim = hurtHUD.DOFade (0f, 1.0f);

			ManageAnimations (positionAnim, null);

			TweenCallback tc = OnHurtTextAnimationComplete;

			ManageAnimations (colorAnim, tc);
		});

	}
		
	public void AgentDieAnim(CallBack cb){
		ManageAnimations (agentIcon.DOFade (0f, 1.0f), () => {
			this.gameObject.SetActive (false);
//			this.enabled = false;
			firstSetHealthBar = true;
			firstSetStrengthBar = true;
			cb();
		});

	}

	private void OnHurtTextAnimationComplete(){
		hurtHUD.transform.localScale = new Vector3 (1.0f, 1.0f);
		hurtHUD.GetComponent<Text>().enabled = false;
		hurtHUD.color = Color.red;
		hurtHUD.transform.localPosition = hurtHUD.transform.localPosition + new Vector3 (0, -100, 0);

	}

	// 动画管理方法，复杂回调单独写函数传入，简单回调使用拉姆达表达式
	private void ManageAnimations(Tweener newTweener,TweenCallback tc){
		allTweeners.Add (newTweener);

		newTweener.OnComplete (
			() => {allTweeners.Remove (newTweener);
				if (tc != null) {
					tc ();
				}});
	}

}
