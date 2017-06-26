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


	public void UpdateHealthAndStrengthBarAnim(BattleAgent ba){
		healthBar.maxValue = ba.maxHealth;
		healthBar.DOValue (ba.health, 0.5f);
		healthText.text = ba.health + "/" + ba.maxHealth;

		strengthBar.maxValue = ba.maxStrength;
		strengthBar.DOValue (ba.strength, 0.5f);
		strengthText.text = ba.strength + "/" + ba.maxStrength;

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
		
	public void AgentDieAnim(){
		agentIcon.DOFade (0f, 1.0f).OnComplete(()=>{
			this.gameObject.SetActive (false);
			this.enabled = false;
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
