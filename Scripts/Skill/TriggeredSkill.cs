using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
 
	public abstract class TriggeredSkill : Skill {

//		public int skillPriority;//技能执行的优先级

		[System.Serializable]
		public struct TriggerInfo{
			public bool triggered;
			public SkillEffectTarget triggerSource;
			public SkillEffectTarget triggerTarget;
		}

		public bool canOverlay;// 技能效果是否可以叠加

		public TriggerInfo beforeFightTriggerInfo; 	// 进入战斗触发信息
		public TriggerInfo attackTriggerInfo;		// 攻击动作触发信息
		public TriggerInfo hitTriggerInfo;			// 攻击命中触发信息
//		public TriggerInfo attackFinishTriggerInfo;	// 攻击动作完成触发信息
		public TriggerInfo beAttackedTriggerInfo;	// 被攻击时触发信息
		public TriggerInfo beHitTriggerInfo;		// 被攻击命中触发信息
		public TriggerInfo fightEndTriggerInfo;		// 结束战斗触发信息

		public string statusName;//状态名称

		public float triggeredProbability;//触发概率

		public float skillSourceValue;//技能结算数据源

		public HurtType hurtType;



		protected BattleAgentController GetAffectedBattleAgent(TriggerInfo triggerInfo,BattleAgentController self,BattleAgentController enemy){
			BattleAgentController affectedBa = null;
			if (triggerInfo.triggerTarget == SkillEffectTarget.Self) {
				affectedBa = self;
			} else if (triggerInfo.triggerTarget == SkillEffectTarget.Enemy) {
				affectedBa = enemy;
			}
			return affectedBa;
		}

		protected BattleAgentController GetSkillUser(TriggerInfo triggerInfo,BattleAgentController self,BattleAgentController enemy){

			BattleAgentController skillUser = null;

			if (triggerInfo.triggerSource == SkillEffectTarget.Self) {
				skillUser = self;
			} else if (triggerInfo.triggerSource == SkillEffectTarget.Enemy) {
				skillUser = enemy;
			}

			return skillUser;

		}


		//技能持续事件或状态持续时间
		//单次型技能代表技能对应状态的持续时间（如 ： 减少10点护甲，【持续】90秒）
		//连续型技能代表技能持续时间(如 ：每秒损失10点生命，【持续】3秒）
		public float duration;

		/// <summary>
		/// 技能作用效果
		/// 【！子类重写AffectAgents方法时，必须调用base.AffectAgents，否则必须重写技能触发回调逻辑！】
		/// </summary>
		public override void AffectAgents (BattleAgentController self, BattleAgentController enemy)
		{
			base.AffectAgents (self, enemy);

			BattleAgentController triggerSource = null;

			if (beforeFightTriggerInfo.triggered) {
				switch (beforeFightTriggerInfo.triggerSource) {
				case SkillEffectTarget.Self:
					triggerSource = self;
					break;
				case SkillEffectTarget.Enemy:
					triggerSource = enemy;
					break;
				}

				TriggeredSkillExcutor excutor = new TriggeredSkillExcutor (beforeFightTriggerInfo.triggerTarget, BeforeFightTriggerCallBack);

				triggerSource.beforeFightTriggerExcutors.Add (excutor);
			}

			if (attackTriggerInfo.triggered) {
				switch (attackTriggerInfo.triggerSource) {
				case SkillEffectTarget.Self:
					triggerSource = self;
					break;
				case SkillEffectTarget.Enemy:
					triggerSource = enemy;
					break;
				}
				TriggeredSkillExcutor excutor = new TriggeredSkillExcutor (attackTriggerInfo.triggerTarget, AttackTriggerCallBack);
				triggerSource.attackTriggerExcutors.Add (excutor);
			}

			if (hitTriggerInfo.triggered) {
				switch (attackTriggerInfo.triggerSource) {
				case SkillEffectTarget.Self:
					triggerSource = self;
					break;
				case SkillEffectTarget.Enemy:
					triggerSource = enemy;
					break;
				}
				TriggeredSkillExcutor excutor = new TriggeredSkillExcutor (hitTriggerInfo.triggerTarget, HitTriggerCallBack);
				triggerSource.hitTriggerExcutors.Add (excutor);
			}

//			if (attackFinishTriggerInfo.triggered) {
//				switch (attackFinishTriggerInfo.excutor) {
//				case SkillEffectTarget.Self:
//					targetBa = self;
//					break;
//				case SkillEffectTarget.Enemy:
//					targetBa = enemy;
//					break;
//				}
//				targetBa.attackFinishTriggerCallBacks.Add (AttackFinishTriggerCallBack);
//			}

			if (beAttackedTriggerInfo.triggered) {
				switch (beAttackedTriggerInfo.triggerSource) {
				case SkillEffectTarget.Self:
					triggerSource = self;
					break;
				case SkillEffectTarget.Enemy:
					triggerSource = enemy;
					break;
				}
				TriggeredSkillExcutor excutor = new TriggeredSkillExcutor (beAttackedTriggerInfo.triggerTarget, BeAttackedTriggerCallBack);
				triggerSource.beAttackedTriggerExcutors.Add (excutor);
			}

			if (beHitTriggerInfo.triggered) {
				switch (beHitTriggerInfo.triggerSource) {
				case SkillEffectTarget.Self:
					triggerSource = self;
					break;
				case SkillEffectTarget.Enemy:
					triggerSource = enemy;
					break;
				}
				TriggeredSkillExcutor excutor = new TriggeredSkillExcutor (beHitTriggerInfo.triggerTarget, BeHitTriggerCallBack);
				triggerSource.beHitTriggerExcutors.Add (excutor);
			}
				
			TriggeredSkillExcutor FightEndExcutor = new TriggeredSkillExcutor (fightEndTriggerInfo.triggerTarget, FightEndTriggerCallBack);


			self.fightEndTriggerExcutors.Add (FightEndExcutor);
		}



		protected virtual void BeforeFightTriggerCallBack(BattleAgentController self,BattleAgentController enemy){

		}

		protected virtual void AttackTriggerCallBack(BattleAgentController self,BattleAgentController enemy){

		}

		protected virtual void HitTriggerCallBack(BattleAgentController self,BattleAgentController enemy){

		}

		protected virtual void BeAttackedTriggerCallBack(BattleAgentController self,BattleAgentController enemy){

		}

		protected virtual void BeHitTriggerCallBack(BattleAgentController self,BattleAgentController enemy){

		}

		/// <summary>
		/// 战斗结束后的逻辑回调
		/// 用于重置角色状态，该方法一定会加入战斗结束的回调队列中
		/// 如果不需要重置，重写一个空的方法即可
		/// </summary>
		/// <param name="self">Self.</param>
		/// <param name="enemy">Enemy.</param>
		protected virtual void FightEndTriggerCallBack (BattleAgentController self, BattleAgentController enemy){
		
		}

		/// <summary>
		/// 技能效果触发后执行的逻辑函数
		/// 该方法必须在战斗的各种状态（进入战斗／攻击动作／攻击命中／被攻击／被攻击命中）的回调中调用才能执行
		/// 也可以不用这个函数，在战斗的各种状态回调中单独描述回调逻辑
		/// </summary>
		/// <param name="triggerInfo">状态触发信息.</param>
		/// <param name="self">攻击方.</param>
		/// <param name="enemy">被攻击方.</param>
		protected virtual void ExcuteTriggeredSkillLogic(TriggerInfo triggerInfo, BattleAgentController self,BattleAgentController enemy){

		}

		/// <summary>
		/// 强制取消技能效果
		/// 【注意：取消技能效果的同时需要将技能从战斗结算器中删除】
		/// </summary>
		/// <returns><c>true</c> if this instance cancel skill effect; otherwise, <c>false</c>.</returns>
		public virtual void CancelSkillEffect (){

		}

		protected void SetEffectAnims(TriggerInfo triggerInfo,BattleAgentController self,BattleAgentController enemy){

			// 播放技能对应的己方（释放技能方）技能特效动画
			switch(triggerInfo.triggerTarget){
			case SkillEffectTarget.Self://如果被动逻辑由自己执行
				if (selfEffectAnimName != string.Empty) {
					self.SetEffectAnim (selfEffectAnimName);
				}
				if (enemyEffectAnimName != string.Empty) {
					enemy.SetEffectAnim (enemyEffectAnimName);
				}
				break;

			// 播放技能对应的敌方（技能目标方）的技能特效动画
			case SkillEffectTarget.Enemy:// 如果被动逻辑由对方执行
				if (selfEffectAnimName != string.Empty) {
					enemy.SetEffectAnim (selfEffectAnimName);
				}
				if (enemyEffectAnimName != string.Empty) {
					self.SetEffectAnim (enemyEffectAnimName);
				}
				break;
			}

		}
			


	}
}
