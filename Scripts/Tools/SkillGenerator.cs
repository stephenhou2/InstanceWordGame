using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillGenerator : MonoBehaviour {

	public Skill skill;

	public EffectGenerator effectGenerator;

	private Transform mEffectsAndStatesContainer;

	public Text effectText;

	public Transform effectsContainer;

	private List<int> effectsIndexArray = new List<int>();

	public Player player;

	// *********** for test use **********//
	void Awake(){
		EffectData[] skillEffectsData = DataInitializer.LoadDataToModelWithPath<EffectData> (CommonData.jsonFileDirectoryPath, CommonData.effectsDataFileName);
		SetupSkillGenerator (skillEffectsData);
	}

	public void SetupSkillGenerator(EffectData[] skillEffectsData){
		
		mEffectsAndStatesContainer = TransformManager.NewTransform ("Effects", null);

		//所有的技能效果数据转为技能效果对象,存放在effectGenerator的skillEffectsList中
		effectGenerator.GenerateEffectObjects (skillEffectsData,false,null,mEffectsAndStatesContainer);

		//初始化创造技能界面
		SetupSkillGeneratorScene ();

	}

	// 初始化所有的状态类技能效果
	public StateSkillEffect[] GenerateStateSkillEffects(EffectData[] skillEffectsData,string effectName){

		mEffectsAndStatesContainer = TransformManager.NewTransform ("States", null);

		effectGenerator.GenerateEffectObjects (skillEffectsData, true, effectName,mEffectsAndStatesContainer);

		StateSkillEffect[] sseArray = null;

		effectGenerator.skillEffectsList.CopyTo (sseArray);

		return sseArray;
	}

	// 根据所选技能效果创造技能
	public Skill GenerateSkillWithIds(int id_1,int id_2,BattleAgent ba){

		Skill mySkill = Instantiate(skill,ba.baView.skillsContainer);

		BaseSkillEffect skillEffect_1 = effectGenerator.skillEffectsList [id_1];
		effectGenerator.skillEffectsList.RemoveAt (id_1);
		skillEffect_1.name = skillEffect_1.effectName;

		BaseSkillEffect skillEffect_2 = effectGenerator.skillEffectsList [id_2 - 1];
		effectGenerator.skillEffectsList.RemoveAt (id_2 - 2);
		skillEffect_2.name = skillEffect_2.effectName;

		mySkill.needSelectEnemy = skillEffect_1.needSelectEnemy || skillEffect_2.needSelectEnemy;

		mySkill.skillEffects = new BaseSkillEffect[] {
			skillEffect_1,
			skillEffect_2
		};

		mySkill.strengthConsume = skillEffect_1.strengthConsume + skillEffect_2.strengthConsume;
		mySkill.actionConsume = skillEffect_1.actionConsume + skillEffect_2.actionConsume;

		ba.skillsEquiped.Add (mySkill);

		return mySkill;

	}



	// 初始化创造技能界面
	private void SetupSkillGeneratorScene(){
		int effectsCount = effectGenerator.skillEffectsList.Count;
		for (int i = 0; i<effectsCount; i++) {

			BaseSkillEffect bse = effectGenerator.skillEffectsList [i];
			Text mEffectText = Instantiate (effectText,effectsContainer);
			mEffectText.text = bse.effectName + ":" + bse.description;
			mEffectText.GetComponent<EffectText> ().effectIndex = i;

		}
		Debug.Log ("进入了技能生成场景");
	}

	public void QuitSkillGenerate(){
		TransformManager.DestroyTransform (mEffectsAndStatesContainer);
	}

	public void AddEffectIndex(int effectIndex){
		
		effectsIndexArray.Add (effectIndex);


		if (effectsIndexArray.Count == 2) {

			skill = GenerateSkillWithIds (effectsIndexArray [0], effectsIndexArray [1],player);

			Debug.Log ("generate skill :" + skill);
		}

	}

}
