using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectText : MonoBehaviour {

	public int effectIndex = 0;

	public void OnClick(){

		Debug.Log ("click" + gameObject.GetComponent<Text> ().text + "index :" + effectIndex);

		gameObject.GetComponent<Text> ().color = Color.red;

		SkillGenerator sg = GameObject.Find ("SkillGenerator").GetComponent<SkillGenerator>();

		sg.AddEffectIndex (effectIndex);

	}
}
