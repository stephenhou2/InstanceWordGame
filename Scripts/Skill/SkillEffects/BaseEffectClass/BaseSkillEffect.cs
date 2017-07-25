using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BaseSkillEffect:MonoBehaviour {

	//*************需要从本地读取的技能效果数据属性***************//
	public string effectName;//魔法效果名称

	public string description;//魔法效果描述

	public int id;//魔法效果id

	public int strengthConsume;//消耗的气力数

	public int effectDuration;//效果持续回合数

	public int actionConsume;//效果消耗行动数

	public SkillEffectTarget effectTarget;//效果目标

	public float scaler;//影响系数

	public bool isStateEffect;//是否是状态类效果

	public bool canOverlay;//是否可以叠加

	public StartTurn startTurn;//效果开始轮

	public EffectType effectType;//物理攻击、魔法攻击、混乱攻击
	//*************需要从本地读取的技能效果数据属性***************//

	public float seed = 0.01f;//计算伤害和闪避时的种子数

	public int actionCount = 1;//该效果已持续的回合数

	public int skillLevel;//效果等级

	public bool needSelectEnemy;

	// 技能特效对应的效果图名称
	public string spriteName;

	// 技能效果图的填充方式
	public UnityEngine.UI.Image.FillMethod fillMethod;

	// 技能效果图的填充起点
	public int fillOrigin;

//	public TriggerType triggerType;//触发效果的trigger类型

	 
	//技能实际效果
	public abstract void AffectAgents (BattleAgent self,List<BattleAgent> friends, BattleAgent targetEnemy,
		List<BattleAgent> enemies,
		int skillLevel,
		TriggerType triggerType,
		int attachedInfo);

	// 判断概率性技能是否生效
	protected bool isEffective(float chance){
		float randomNum = Random.Range (0, 100)/100f;
		return randomNum <= chance;
	}

}


[System.Serializable]
public class EffectData{
	public string effectName;//魔法效果名称

	public string description;//魔法效果描述

	public int id;//魔法效果id

	public int strengthConsume;//消耗的气力数

	public int effectDuration;//效果持续回合数

	public int actionConsume;//效果消耗行动数

	public SkillEffectTarget effectTarget;//效果目标

	public float scaler;//影响系数

	public bool isStateEffect;//是否是状态类效果

	public bool canOverlay;//是否可以叠加

	public StartTurn startTurn;//效果开始轮

	public EffectType effectType;//物理攻击、魔法攻击、混乱攻击

	public override string ToString ()
	{
		return string.Format ("[EffectDataInitializer]" + "\n[effectName]:" + effectName + "\n[description]:" + description + "\n[id]:" + id);
	}

}