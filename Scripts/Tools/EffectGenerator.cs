using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EffectGenerator : MonoBehaviour {

	public List<BaseSkillEffect> skillEffectsList = new List<BaseSkillEffect>();


	// allStateEffects == true 只创建状态类效果数组，effectName != null表示创建指定效果 
	public void GenerateEffectObjects(EffectData[] edArray,bool allStateEffects,string effectName,Transform effectsContainer){
		
		skillEffectsList.Clear ();

		foreach (EffectData ed in edArray) {

			GameObject skillEffect = TransformManager.NewTransform ("effects", effectsContainer).gameObject;

			if (effectName != null && ed.effectName != effectName) {
				continue;
			}
			// 如果只获取状态类型效果，并且读取的这个效果不是状态类的，则直接进入下轮遍历
			if (allStateEffects && !ed.isStateEffect) {
				continue;
			}

			//添加对应技能脚本
			BaseSkillEffect bse = AddComponentWithEffectId (skillEffect, ed.id);

			// 技能效果初始化
			bse.effectName = ed.effectName;
			bse.description = ed.description;
			bse.id = ed.id;
			bse.strengthConsume = ed.strengthConsume;
			bse.effectDuration = ed.effectDuration;
			bse.actionConsume = ed.actionConsume;
			bse.effectTarget = ed.effectTarget;
			bse.scaler = ed.scaler;
			bse.isStateEffect = ed.isStateEffect;
			bse.canOverlay = ed.canOverlay;
			bse.startTurn = ed.startTurn;
			bse.effectType = ed.effectType;

			
			skillEffectsList.Add (bse);

		}
	}
	public static BaseSkillEffect AddComponentWithEffectId(GameObject go,int id){
		switch (id) {
		case 0:
			return go.AddComponent<WeaponExpert> ();
		case 1:
			return go.AddComponent<Thunder>();
		case 2:
			return go.AddComponent<PowerOfElement> ();
		case 3:
			return go.AddComponent<ThunderChain> ();
		case 4:
			return go.AddComponent<Paralysis> ();
		case 5:
			return go.AddComponent<FireBlade> ();
		case 6:
			return go.AddComponent<Burn>();
		case 7:
			return go.AddComponent<Treat> ();
		case 8:
			return go.AddComponent<Roar> ();
		case 9:
			return go.AddComponent<DecreaseAmour> ();
		case 10:
			return go.AddComponent<BloodThirsty> ();
		case 11:
			return go.AddComponent<StrikeBack>();
		case 12:
			return go.AddComponent<Stonelize> ();
		default:
			return null;
		}

	}

//	public static BaseSkillEffect AddComponentWithEffectId(GameObject go,int id){
//		switch (id) {
//		case 0:
//			return go.AddComponent<MagicArrow> ();	 
//		case 1:
//			return go.AddComponent<PowerOfMagicElement> (); 
//		case 2:
//			return go.AddComponent<Roar>(); 
//		case 3:
//			return go.AddComponent<BloodThirsty>();
//		case 4:
//			return go.AddComponent<Treat> (); 
//		case 5:
//			return go.AddComponent<HealthRecover>(); 
//		case 6:
//			return go.AddComponent<Blink>(); 
//		case 7:
//			return go.AddComponent<GrowOld>();
//		case 8:
//			return go.AddComponent<IronSkin>();
//		case 9:
//			return go.AddComponent<ElementShield>();
//		case 10:
//			return go.AddComponent<DecreaseAmour>();
//		case 11:
//			return go.AddComponent<Weaken>();
//		case 12:
//			return go.AddComponent<Sacrifice>(); 
//		case 13:
//			return go.AddComponent<Overload>();
//		case 14:
//			return go.AddComponent<DoubleAttack>(); 
//		case 15:
//			return go.AddComponent<StrikeBack>();
//		case 16:
//			return go.AddComponent<Nothingness>();
//		case 17:
//			return go.AddComponent<HurtReflect>();
//		case 18:
//			return go.AddComponent<DisArm>();
//		case 19:
//			return go.AddComponent<Silience>();
//		case 20:
//			return go.AddComponent<Sneak>();
//		case 21:
//			return go.AddComponent<DreamLand>();
//		case 22:
//			return go.AddComponent<Tearing>();
//		case 23:
//			return go.AddComponent<MagicMark>();
//		case 24:
//			return go.AddComponent<IronBody>();
//		case 25:
//			return go.AddComponent<ElementBody>(); 
//		case 26:
//			return go.AddComponent<Burn>();
//		case 27:
//			return go.AddComponent<Bleed>(); 
//		case 28:
//			return go.AddComponent<Poision>();
//		case 29:
//			return go.AddComponent<Stonelize>();
//		case 30:
//			return go.AddComponent<Sorcery>();
//		case 31:
//			return go.AddComponent<Infect>();
//		case 32:
//			return go.AddComponent<Confuse>(); 
//		case 33:
//			return go.AddComponent<Taunt>(); 
//		case 34:
//			return go.AddComponent<Fury>();
//		case 35:
//			return go.AddComponent<Provoke>(); 
//		case 36:
//			return go.AddComponent<Learn>();
//		case 37:
//			return go.AddComponent<Yield>();
//		case 38:
//			return go.AddComponent<StrongWill>();
//		case 39:
//			return go.AddComponent<Faith>();
//		case 40:
//			return go.AddComponent<Gambler>(); 
//		case 41:
//			return go.AddComponent<Scare>();
//		case 42:
//			return go.AddComponent<Wrestle>();
//		case 43:
//			return go.AddComponent<Magic>();
//		case 44:
//			return go.AddComponent<Steal>();
//		case 45:
//			return go.AddComponent<Seal> (); 
//		case 46:
//			return go.AddComponent<PhysicalAttack> ();
//		case 47:
//			return go.AddComponent<Defence> ();
//		default:
//			return null; 
//		}
//	}
}
