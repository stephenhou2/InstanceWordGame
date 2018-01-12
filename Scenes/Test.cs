using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DragonBones;

public class Test : MonoBehaviour {



	public Animator anim;

	public UnityArmatureComponent arm;

	private GameObject myMonster;

	public void ChangeStatus(){

		anim.SetBool ("Play", true);
	}

	public void ChangeToOrig(){
		anim.SetBool ("Play", false);
	}

	public void PlaySkill(){
		arm.animation.Play ("skill", 1);
	}

	public void PlayWait(){
		arm.animation.Play ("wait", 0);
	}

	public void LoadArm(){
		GameObject monster = Resources.Load ("FireFox") as GameObject;
		myMonster = Instantiate (monster);
		myMonster.transform.position = Vector3.zero;
		arm = myMonster.GetComponent<UnityArmatureComponent> ();

	}

	public void ActiveMonster(){
		myMonster.SetActive (true);
		arm.animation.Play ("wait", 0);
	}
}
