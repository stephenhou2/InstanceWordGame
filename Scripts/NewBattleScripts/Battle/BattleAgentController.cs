using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Transform = UnityEngine.Transform;
using DragonBones;


namespace WordJourney
{
	public abstract class BattleAgentController : MonoBehaviour {

		public Agent agent;

		public Skill physicalAttack;

		private Skill currentSelectedSkill;

		public Animator animator;

		// 碰撞检测层
		public LayerMask collosionLayer;			//Layer on which collision will be checked.

		protected BoxCollider2D boxCollider; 		//The BoxCollider2D component attached to this object.
	

		protected UnityArmatureComponent armature;




		protected virtual void Awake(){
			
			armature = GetComponent<UnityArmatureComponent> ();

			//Get a component reference to this object's BoxCollider2D
			boxCollider = GetComponent <BoxCollider2D> ();
		}


		// 状态效果触发执行的方法
//		public void OnTrigger(BattleAgentController triggerAgent, TriggerType triggerType){
//
//			foreach(StateSkillEffect sse in agent.states){
//				sse.AffectAgents (this,triggerAgent, sse.skillLevel, triggerType);
//			}
//
//		}

		public abstract void PlayHurtTextAnim (string hurtStr, TintTextType tintTextType);

		public abstract void PlayGainTextAnim(string gainStr);

		public void PlayIdleAnim(){
			armature.animation.Play ("stand",0);
		}
			
//		public void PlayFightAnim(CallBack cb){
//			armature.animation.Play ("battle",1);
//			StartCoroutine ("ExcuteCallBack", cb);
//
//		}

		private IEnumerator ExcuteCallBack(CallBack cb){
			yield return new WaitUntil (() => armature.animation.isCompleted);
			cb ();
		}

	}
}
