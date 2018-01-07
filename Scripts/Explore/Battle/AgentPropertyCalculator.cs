using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public enum SpecialAttackResult{
		None,
		Crit,
		Miss,
		Gain
	}

	public class AgentPropertyCalculator {

		private float critSeed = 0.0035f; //计算暴击时的种子数

		private float armorSeed = 0.01f; //计算护甲抵消伤害的种子数

		private float magicResistSeed = 0.01f; //计算魔抗抵消伤害的种子数

		public SpecialAttackResult specialAttackResult = SpecialAttackResult.None;

		public BattleAgentController self;

		public BattleAgentController enemy;

		public int maxHealth;
		public int health;
		public int mana;

		public int attack;
		public int attackSpeed;
		public int armor;
		public int magicResist;
		public int dodge;
		public int crit;
		public int hit;

		public int physicalHurtFromNomalAttack;

		public float physicalHurtScaler;
		public float magicalHurtScaler;

		public float critHurtScaler;//暴击倍率



		public int physicalHurtToEnemy;
		public int magicalHurtToEnemy;

//		public int totalPhysicalHurt;
//		public int totalMagicalHurt;
//		public int instantPhysicalHurtToEnemy;
//		public int instantMagicalHurtToEnemy;

		public int healthAbsorb;

//		private int healthChange;

		public float critFixScaler;
		public float dodgeFixScaler;

		public int hurtReflect;

		public int maxHealthChangeFromTriggeredSkill;
		public int hitChangeFromTriggeredSkill;
		public int manaChangeFromTriggeredSkill;
		public int attackChangeFromTriggeredSkill;
		public int attackSpeedChangeFromTriggeredSkill;
		public int armorChangeFromTriggeredSkill;
		public int magicResistChangeFromTriggeredSkill;
		public int dodgeChangeFromTriggeredSkill;
		public int critChangeFromTriggeredSkill;

		public float maxHealthChangeScalerFromTriggeredSkill;
		public float hitChangeScalerFromTriggeredSkill;
		public float manaChangeScalerFromTriggeredSkill;
		public float attackChangeScalerFromTriggeredSkill;
		public float attackSpeedChangeScalerFromTriggeredSkill;
		public float armorChangeScalerFromTriggeredSkill;
		public float magicResistChangeScalerFromTriggeredSkill;
		public float dodgeChangeScalerFromTriggeredSkill;
		public float critChangeScalerFromTriggeredSkill;
		public float physicalHurtScalerChangeFromTriggeredSkill;
		public float magicalHurtScalerChangeFromTriggeredSkill;
		public float critHurtScalerChangeFromTriggeredSkill;

		/// <summary>
		/// 如果是即时性的属性变化，需要使用这个方法
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="propertyType">Property type.</param>
		/// <param name="change">Change.</param>
		public Agent.PropertyChange InstantPropertyChange(BattleAgentController target,PropertyType propertyType,float change,bool fromTriggeredSkill = true){

			string instantChange = string.Format ("即时性属性变化: 角色名称：{0}，类型：{1}，变化：{2}",
				target.agent.agentName, propertyType,change);

			Debug.Log (instantChange);

			if (change == 0) {
				return new Agent.PropertyChange();
			}

			AgentPropertyChange (propertyType, change, fromTriggeredSkill);

			Agent.PropertyChange propretyChange = self.agent.ResetBattleAgentProperties (false);

			target.UpdateFightStatus ();

			target.UpdateStatusPlane ();

			if (propertyType == PropertyType.Health) {
				if (change > 0) {
					string gainText = string.Format("<color=green>{0}</color>",(int)change);
					target.AddFightTextToQueue(gainText,SpecialAttackResult.Gain);
				} else {
					string hurtText = string.Format ("<color=red>{0}</color>", (int)(-change));
					target.AddFightTextToQueue (hurtText, SpecialAttackResult.None);
				}
			}

			return propretyChange;
			
		}


		public void CalculateAttackHurt(){

			float critProbability = critSeed * crit / (1 + critSeed * crit) - enemy.propertyCalculator.critFixScaler;

			float tempCritScaler = 1.0f;

			if (isEffective (critProbability)) {
				specialAttackResult = SpecialAttackResult.Crit;
				tempCritScaler = critHurtScaler;
			} else {
				specialAttackResult = SpecialAttackResult.None;
			}

			int oriPhysicalHurtToEnemy = (int)(physicalHurtFromNomalAttack * physicalHurtScaler * tempCritScaler);

			physicalHurtToEnemy = (int)(oriPhysicalHurtToEnemy / (1 + armorSeed * enemy.propertyCalculator.armor)) + enemy.propertyCalculator.hurtReflect;

			magicalHurtToEnemy = (int)(magicalHurtToEnemy * magicalHurtScaler / (1 + magicResistSeed * enemy.propertyCalculator.magicResist));

//			string fightResult = string.Format ("战斗结果:角色名称：{0}，造成的物理伤害：{1}，造成的魔法伤害：{2},暴击系数：{3},原始物理伤害：{4},",
//				self.agent.agentName, physicalHurtToEnemy, magicalHurtToEnemy,tempCritScaler,oriPhysicalHurtToEnemy);

//			Debug.Log (fightResult);	

		}

		public void CalculateAgentHealth(){

			health += healthAbsorb - enemy.propertyCalculator.physicalHurtToEnemy - enemy.propertyCalculator.magicalHurtToEnemy;

		}

		public void ResetAllHurt(){
			physicalHurtFromNomalAttack = 0;
			physicalHurtToEnemy = 0;
			magicalHurtToEnemy = 0;
			hurtReflect = 0;
			healthAbsorb = 0;
		}

		public List<TriggeredSkill> triggeredSkills = new List<TriggeredSkill>();
		public List<ConsumablesSkill> consumablesSkills = new List<ConsumablesSkill> ();

		public void AddSkill<T>(T skill){

			if (typeof(T) == typeof(TriggeredSkill)) {
				
				TriggeredSkill trigSkill = skill as TriggeredSkill;

				if (trigSkill.statusName == "") {
					return;
				}
				triggeredSkills.Add (trigSkill);

				self.agent.allStatus.Add (trigSkill.statusName);

				self.UpdateStatusPlane ();

			} else if (typeof(T) == typeof(ConsumablesSkill)) {
				ConsumablesSkill consSkill = skill as ConsumablesSkill;
				if (consSkill.statusName == "") {
					return;
				}
				consumablesSkills.Add (consSkill);
				self.agent.allStatus.Add (consSkill.statusName);
				self.UpdateStatusPlane ();
			}
		}

		public void RemoveTriggeredSkill<T>(T skill){
			if (typeof(T) == typeof(TriggeredSkill) && triggeredSkills.Contains(skill as TriggeredSkill)) {
				TriggeredSkill trigSkill = skill as TriggeredSkill;

				if (trigSkill.statusName == "") {
					return;
				}

				triggeredSkills.Remove (trigSkill);

				self.agent.allStatus.Remove (trigSkill.statusName);

				self.UpdateStatusPlane ();

			} else if (typeof(T) == typeof(ConsumablesSkill) && consumablesSkills.Contains(skill as ConsumablesSkill)) {

				ConsumablesSkill consSkill = skill as ConsumablesSkill;

				if (consSkill.statusName != "") {
					return;
				}

				consumablesSkills.Remove (consSkill);

				self.agent.allStatus.Remove (consSkill.statusName);

				self.UpdateStatusPlane ();
			}
		}

		public List<TriggeredSkill> GetTriggeredSkillsWithSameStatus(string statusName){
			
			if (statusName == "") {
				string error = "技能的状态名不能为空";
				Debug.LogError (error);
				return null;
			}

			List<TriggeredSkill> sameStatusSkills = new List<TriggeredSkill> ();

			for (int i = 0; i < triggeredSkills.Count; i++) {
				if (triggeredSkills [i].statusName == statusName) {
					sameStatusSkills.Add (triggeredSkills [i]);
				}
			}

			return sameStatusSkills;

		}

		public List<ConsumablesSkill> GetConsumablesSkillsWithSameStatus(string statusName){
			
			List<ConsumablesSkill> sameStatusSkills = new List<ConsumablesSkill> ();

			for (int i = 0; i < triggeredSkills.Count; i++) {
				if (consumablesSkills [i].statusName == statusName) {
					sameStatusSkills.Add (consumablesSkills [i]);
				}
			}

			return sameStatusSkills;
		}

		public void ClearAllSkills<T>(){
			if (typeof(T) == typeof(TriggeredSkill)) {
				triggeredSkills.Clear ();
			} else if (typeof(T) == typeof(ConsumablesSkill)) {
				consumablesSkills.Clear ();
			}
		}
			





		/// <summary>
		/// 属性变更
		/// </summary>
		/// <param name="propertyType">Property type.</param>
		/// <param name="change">change.</param>
		public void AgentPropertyChange(PropertyType propertyType,float change,bool fromTriggeredSkill = true){

			self.agent.AddPropertyChangeFromOther (propertyType, change);

			switch (propertyType) {
			case PropertyType.MaxHealth:
				if (change > -1 && change < 1) {
					maxHealth = (int)(maxHealth * (1 + change));
					health = (int)(health * (1 + change));
					if (fromTriggeredSkill) {
						maxHealthChangeScalerFromTriggeredSkill += change;
					}
				} else {
					maxHealth += (int)change;
					health += (int)(health * change / maxHealth);
					if (fromTriggeredSkill) {
						maxHealthChangeFromTriggeredSkill += (int)change;
					}
				}
				break;
			case PropertyType.Health:
				if (change > -1 && change < 1) {
					health = (int)(health * (1 + change));
				} else {
					health += (int)(change);
				}
				break;
			case PropertyType.Hit:
				if (change > -1 && change < 1) {
					hit = (int)(mana * (1 + change));
					if (fromTriggeredSkill) {
						hitChangeScalerFromTriggeredSkill += (int)change;
					}
				} else {
					hit += (int)change;
					if (fromTriggeredSkill) {
						hitChangeFromTriggeredSkill += (int)change;
					}
				}
				break;
			case PropertyType.Mana:
				if (change > -1 && change < 1) {
					mana = (int)(mana * (1 + change));
					if (fromTriggeredSkill) {
						manaChangeScalerFromTriggeredSkill += change;
					}
				} else {
					mana += (int)change;
					if (fromTriggeredSkill) {
						manaChangeFromTriggeredSkill += (int)change;
					}
				}

				break;
			case PropertyType.Attack:
				if (change > -1 && change < 1) {
					attack = (int)(attack * (1 + change));
					if (fromTriggeredSkill) {
						attackChangeScalerFromTriggeredSkill += change;
					}
				} else {
					attack += (int)change;
					if (fromTriggeredSkill) {
						attackChangeFromTriggeredSkill += (int)change;
					}
				}

				break;
			case PropertyType.AttackSpeed:
				if (change > -1 && change < 1) {
					attackSpeed = (int)(attackSpeed * (1 + change));
					if (fromTriggeredSkill) {
						attackSpeedChangeScalerFromTriggeredSkill += change;
					}
				} else {
					attackSpeed += (int)change;
					if (fromTriggeredSkill) {
						attackSpeedChangeFromTriggeredSkill += (int)change;
					}
				}
				break;
			case PropertyType.Armor:
				if (change > -1 && change < 1) {
					armor = (int)(armor * (1 + change));
					if (fromTriggeredSkill) {
						armorChangeScalerFromTriggeredSkill += change;
					}
				} else {
					armor += (int)change;
					if (fromTriggeredSkill) {
						armorChangeFromTriggeredSkill += (int)change;
					}
				}

				break;
			case PropertyType.MagicResist:
				if (change > -1 && change < 1) {
					magicResist = (int)(magicResist * (1 + change));
					if (fromTriggeredSkill) {
						magicResistChangeScalerFromTriggeredSkill += change;
					}
				} else {
					magicResist += (int)change;
					if (fromTriggeredSkill) {
						magicResistChangeFromTriggeredSkill += (int)change;
					}
				}

				break;
			case PropertyType.Dodge:
				if (change > -1 && change < 1) {
					dodge = (int)(dodge * (1 + change));
					if (fromTriggeredSkill) {
						dodgeChangeScalerFromTriggeredSkill += change;
					}
				} else {
					dodge += (int)change;
					if (fromTriggeredSkill) {
						dodgeChangeFromTriggeredSkill += (int)change;
					}
				}

				break;
			case PropertyType.Crit:
				if (change > -1 && change < 1) {
					crit = (int)(crit * (1 + change));
					if (fromTriggeredSkill) {
						critChangeScalerFromTriggeredSkill += change;
					}
				} else {
					crit += (int)change;
					if (fromTriggeredSkill) {
						critChangeFromTriggeredSkill += (int)change;
					}
				}

				break;
			case PropertyType.PhysicalHurtScaler:
				physicalHurtScaler += change;
				if (fromTriggeredSkill) {
					physicalHurtScalerChangeFromTriggeredSkill += change;
				}
				break;
			case PropertyType.MagicalHurtScaler:
				magicalHurtScaler += change;
				if (fromTriggeredSkill) {
					magicalHurtScalerChangeFromTriggeredSkill += change;
				}
				break;
			case PropertyType.CritHurtScaler:
				critHurtScaler += change;
				if (fromTriggeredSkill) {
					critHurtScalerChangeFromTriggeredSkill += change;
				}
				break;
			case PropertyType.WholeProperty:
				if (change > -1 && change < 1) {
					maxHealth = (int)(health * (1 + change));
					mana = (int)(mana * (1 + change));
					attack = (int)(attack * (1 + change));
					attackSpeed = (int)(attackSpeed * (1 + change));
					armor = (int)(armor * (1 + change));
					magicResist = (int)(magicResist * (1 + change));
					dodge = (int)(dodge * (1 + change));
					crit = (int)(crit * (1 + change));
					hit = (int)(hit * (1 + change));
					if (fromTriggeredSkill) {
						maxHealthChangeScalerFromTriggeredSkill += change;
						manaChangeScalerFromTriggeredSkill += change;
						attackChangeScalerFromTriggeredSkill += change;
						attackSpeedChangeScalerFromTriggeredSkill += change;
						armorChangeScalerFromTriggeredSkill += change;
						magicResistChangeScalerFromTriggeredSkill += change;
						dodgeChangeScalerFromTriggeredSkill += change;
						critChangeScalerFromTriggeredSkill += change;
						hitChangeScalerFromTriggeredSkill += change;
					}
				} else {
					health += (int)change;
					mana += (int)change;
					attack += (int)change;
					attackSpeed += (int)change;
					armor += (int)change;
					magicResist += (int)change;
					dodge += (int)change;
					crit += (int)change;
					hit += (int)change;
					if (fromTriggeredSkill) {
						maxHealthChangeFromTriggeredSkill += (int)change;
						manaChangeFromTriggeredSkill += (int)change;
						attackChangeFromTriggeredSkill += (int)change;
						attackSpeedChangeFromTriggeredSkill += (int)change;
						armorChangeFromTriggeredSkill += (int)change;
						magicResistChangeFromTriggeredSkill += (int)change;
						dodgeChangeFromTriggeredSkill += (int)change;
						critChangeFromTriggeredSkill += (int)change;
						hitChangeFromTriggeredSkill += (int)change;
					}
				}

				break;
			}

		}

		public void AgentPropertySetToValue(PropertyType propertyType,float propertyValue){

			switch (propertyType) {
			case PropertyType.MaxHealth:
				maxHealth = (int)propertyValue;
				break;
			case PropertyType.Mana:
				mana = (int)propertyValue;
				break;
			case PropertyType.Attack:
				attack = (int)propertyValue;
				break;
			case PropertyType.AttackSpeed:
				attackSpeed = (int)propertyValue;
				break;
			case PropertyType.Hit:
				hit = (int)propertyValue;
				break;
			case PropertyType.Armor:
				armor = (int)propertyValue;
				break;
			case PropertyType.MagicResist:
				magicResist = (int)propertyValue;
				break;
			case PropertyType.Dodge:
				dodge = (int)propertyValue;
				break;
			case PropertyType.Crit:
				crit = (int)propertyValue;
				break;
			case PropertyType.PhysicalHurtScaler:
				physicalHurtScaler = propertyValue;
				break;
			case PropertyType.MagicalHurtScaler:
				magicalHurtScaler = propertyValue;
				break;
			case PropertyType.CritHurtScaler:
				critHurtScaler = propertyValue;
				break;
			
			}

		}

		public float GetAgentPropertyWithType(PropertyType propertyType){

			float propertyValue = 0;

			switch (propertyType) {
			case PropertyType.MaxHealth:
				propertyValue = maxHealth;
				break;
			case PropertyType.Health:
				propertyValue = health;
				break;
			case PropertyType.Mana:
				propertyValue = mana;
				break;
			case PropertyType.Attack:
				propertyValue = attack;
				break;
			case PropertyType.AttackSpeed:
				propertyValue = attackSpeed;
				break;
			case PropertyType.Hit:
				propertyValue = hit;
				break;
			case PropertyType.Armor:
				propertyValue = armor;
				break;
			case PropertyType.MagicResist:
				propertyValue = magicResist;
				break;
			case PropertyType.Dodge:
				propertyValue = dodge;
				break;
			case PropertyType.Crit:
				propertyValue = crit;
				break;
			case PropertyType.PhysicalHurtScaler:
				propertyValue = physicalHurtScaler;
				break;
			case PropertyType.MagicalHurtScaler:
				propertyValue = magicalHurtScaler;
				break;
			case PropertyType.CritHurtScaler:
				propertyValue = critHurtScaler;
				break;
			}

			return propertyValue;
		}




		// 判断概率性技能是否生效
		protected bool isEffective(float chance){
			float randomNum = Random.Range (0, 100)/100f;
			return randomNum <= chance;
		}

	}
}
