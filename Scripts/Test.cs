using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test: MonoBehaviour {

	public Animator effectAnimator;

	public void Tt(){
		StartCoroutine ("TestClick");
	}

	public IEnumerator TestClick(){

		effectAnimator.gameObject.SetActive (true);

		effectAnimator.SetTrigger ("CrossChopEffect");

		Debug.Log ("特效开始");

		yield return null;

//		float normalizedTime = effectAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
//
//		Debug.Log(normalizedTime);
//
//		while (normalizedTime < 1) {
//			Debug.Log (normalizedTime);
//			normalizedTime = effectAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
//			yield return null;
//		}
//
//		Debug.Log ("特效结束");
//
//		effectAnimator.SetBool ("anim", false);



//		effectAnimator.gameObject.SetActive (false);



	}
}
