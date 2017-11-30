using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public abstract class TriggeredPassiveSkill : PassiveSkill {

		[System.Serializable]
		public struct TriggerInfo{
			public bool triggered;
			public SkillEffectTarget excutor;
		}


		public TriggerInfo beforeFightTriggerInfo;
		public TriggerInfo attackTriggerInfo;
		public TriggerInfo attackFinishTriggerInfo;
		public TriggerInfo beAttackedTriggerInfo;
		public TriggerInfo fightEndTriggerInfo;

		public string stateName;

		protected virtual void Awake(){
			this.skillType = SkillType.TriggeredPassive;
		}


		public override void AffectAgents (BattleAgentController self, BattleAgentController enemy)
		{
			base.AffectAgents (self, enemy);

			BattleAgentController targetBa = null;

			if (beforeFightTriggerInfo.triggered) {
				switch (beforeFightTriggerInfo.excutor) {
				case SkillEffectTarget.Self:
					targetBa = self;
					break;
				case SkillEffectTarget.Enemy:
					targetBa = enemy;
					break;
				}
				targetBa.beforeFightTriggerCallBacks.Add (BeforeFightTriggerCallBack);
			}
			if (attackTriggerInfo.triggered) {
				switch (attackTriggerInfo.excutor) {
				case SkillEffectTarget.Self:
					targetBa = self;
					break;
				case SkillEffectTarget.Enemy:
					targetBa = enemy;
					break;
				}
				targetBa.attackTriggerCallBacks.Add (AttackTriggerCallBack);
			}
			if (attackFinishTriggerInfo.triggered) {
				switch (attackFinishTriggerInfo.excutor) {
				case SkillEffectTarget.Self:
					targetBa = self;
					break;
				case SkillEffectTarget.Enemy:
					targetBa = enemy;
					break;
				}
				targetBa.attackFinishTriggerCallBacks.Add (AttackFinishTriggerCallBack);
			}
			if (beAttackedTriggerInfo.triggered) {
				switch (beAttackedTriggerInfo.excutor) {
				case SkillEffectTarget.Self:
					targetBa = self;
					break;
				case SkillEffectTarget.Enemy:
					targetBa = enemy;
					break;
				}
				targetBa.beAttackedTriggerCallBacks.Add (BeAttackedTriggerCallBack);
			}
			if (fightEndTriggerInfo.triggered) {
				switch (fightEndTriggerInfo.excutor) {
				case SkillEffectTarget.Self:
					targetBa = self;
					break;
				case SkillEffectTarget.Enemy:
					targetBa = enemy;
					break;
				}
				targetBa.fightEndTriggerCallBacks.Add (FightEndTriggerCallBack);
			}
		}

		protected virtual void BeforeFightTriggerCallBack(BattleAgentController self,BattleAgentController enemy){

		}

		protected virtual void AttackTriggerCallBack(BattleAgentController self,BattleAgentController enemy){

		}

		protected virtual void AttackFinishTriggerCallBack(BattleAgentController self,BattleAgentController enemy){

		}

		protected virtual void BeAttackedTriggerCallBack(BattleAgentController self,BattleAgentController enemy){

		}

		protected virtual void FightEndTriggerCallBack(BattleAgentController self,BattleAgentController enemy){

		}


		protected void SetEffectAnims(TriggerInfo triggerInfo,BattleAgentController self,BattleAgentController enemy){

			// 播放技能对应的玩家技能特效动画
			switch(triggerInfo.excutor){
			case SkillEffectTarget.Self://如果被动逻辑由自己执行
				if (selfEffectName != string.Empty) {
					self.SetEffectAnim (selfEffectName);
				}
				if (enemyEffectName != string.Empty) {
					enemy.SetEffectAnim (enemyEffectName);
				}
				break;

			case SkillEffectTarget.Enemy:// 如果被动逻辑由对方执行
				if (selfEffectName != string.Empty) {
					enemy.SetEffectAnim (selfEffectName);
				}
				if (enemyEffectName != string.Empty) {
					self.SetEffectAnim (enemyEffectName);
				}
				break;
			}

		}

	}
}
