using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	// 游戏控制器单例
	public static GameManager gameManager;

	public BattleManager battleManager;

	[HideInInspector]public BattleManager m_battleManager;

	[HideInInspector]public Player player;

	public int gameProcess;

	public SkillGenerator skillGenerator;

	private EffectData[] mEffectsData;

	[HideInInspector]public EffectData[] effectsData{
		get{
			if (mEffectsData == null || mEffectsData.Length == 0) {
				mEffectsData = DataInitializer.LoadDataToModelWithPath <EffectData>(CommonData.JsonFileDirectoryPath, CommonData.effectsDataFileName);
			}
			return mEffectsData;
		}
	}

	void Awake(){

		if (gameManager == null) {
			gameManager = this;
		} else if (gameManager != this) {
			Destroy (gameObject);
		}

		player = Player.mainPlayer;

		DontDestroyOnLoad (gameObject);
	}

	//初始化战斗
	public void OnEnterBattle(){

		SceneManager.LoadSceneAsync ("BattleTestScene", LoadSceneMode.Additive);



	}

	// 初始化生成技能的界面
	public void OnGenerateSkill(){

		SceneManager.LoadSceneAsync ("GenerateSkillScene", LoadSceneMode.Additive);
//		if (effectsData.Length == 0) {
//			effectsData = DataInitializer.LoadDataWithPath<EffectData> (CommonData.effectsFilePath, CommonData.effectsFileName);
//		}
//		skillGenerator.SetupSkillGenerator (effectsData);
	}

	// 使用effectName生成特定效果，如果effectName==null，则生成所有状态类效果
	public StateSkillEffect[] GenerateStates(string effectName){
		return skillGenerator.GenerateStateSkillEffects (effectsData,effectName);
	}





	//初始化战斗场景
	public void SetupBattleScene(){
		Debug.Log ("进入了战斗场景");
	}

	public void OnSelectEffects(int id_1,int id_2){
		skillGenerator.GenerateSkillWithIds (id_1, id_2,player);
	}

	public void OnConfirm(){
		Debug.Log (player);
	}

	public void OnQuit(){
		skillGenerator.QuitSkillGenerate ();
	}


}
