using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	[System.Serializable]
	public class EquipmentAttachedProperty {

		public int attachedPropertyId;
		public int attackGain;
		public int attackSpeedGain;
		public int armorGain;
		public int manaResistGain;
		public int dodgeGain;
		public int critGain;
		public int healthGain;
		public int manaGain;
		public int physicalHurtGain;
		public int magicHurtGain;
		public int healthAbsorbGain;
		public int hardBeatGain;
		public int brambleShiledGain;
		public int magicShieldGain;
		public int allPropertiesGain;

		public EquipmentAttachedProperty(int attachedPropertyId,bool randomGain){

			List<EquipmentAttachedProperty> test = GameManager.Instance.gameDataCenter.allEquipmentAttachedProperties;

			EquipmentAttachedProperty originalAttachedProperty = GameManager.Instance.gameDataCenter.allEquipmentAttachedProperties.Find (delegate(EquipmentAttachedProperty obj) {
				return obj.attachedPropertyId == attachedPropertyId;
			});

			if (randomGain) {
				attackGain = ResetToRandomGain (originalAttachedProperty.attackGain);
				attackSpeedGain = ResetToRandomGain (originalAttachedProperty.attackSpeedGain);
				armorGain = ResetToRandomGain (originalAttachedProperty.armorGain);
				manaResistGain = ResetToRandomGain (originalAttachedProperty.manaResistGain);
				dodgeGain = ResetToRandomGain (originalAttachedProperty.dodgeGain);
				critGain = ResetToRandomGain (originalAttachedProperty.critGain);
				healthGain = ResetToRandomGain (originalAttachedProperty.healthGain);
				manaGain = ResetToRandomGain (originalAttachedProperty.manaGain);
				physicalHurtGain = ResetToRandomGain (originalAttachedProperty.physicalHurtGain);
				magicHurtGain = ResetToRandomGain (originalAttachedProperty.magicHurtGain);
				healthAbsorbGain = ResetToRandomGain (originalAttachedProperty.healthAbsorbGain);
				hardBeatGain = ResetToRandomGain (originalAttachedProperty.hardBeatGain);
				brambleShiledGain = ResetToRandomGain (originalAttachedProperty.brambleShiledGain);
				magicShieldGain = ResetToRandomGain (originalAttachedProperty.magicShieldGain);
				allPropertiesGain = ResetToRandomGain (originalAttachedProperty.allPropertiesGain);
			}
		}

		private int ResetToRandomGain(int originalGain){

			if (originalGain > 0) {
				return Random.Range (1, originalGain);
			}

			return 0;

		}


		public void RebuildPropertiesOf(Agent agent){

			if (attackGain > 0) {
//				agent.attack = (int)(agent.attack * (1 + attackGain / 100f));
				agent.attackGainScaler += attackGain / 100f;
			}
			if (attackSpeedGain > 0) {
//				agent.attackSpeed = (int)(agent.attackSpeed * (1 + attackSpeedGain / 100f));
				agent.attackSpeedGainScaler += attackSpeedGain / 100f;
			}
			if (armorGain > 0) {
//				agent.armor = (int)(agent.armor * (1 + armorGain / 100f));
				agent.armorGainScaler += armorGain / 100f;
			}
			if (manaResistGain > 0) {
//				agent.manaResist = (int)(agent.manaResist * (1 + manaResistGain / 100f));
				agent.manaResistGainScaler += manaResistGain / 100f;
			}
			if (dodgeGain > 0) {
//				agent.dodge = (int)(agent.dodge * (1 + dodgeGain / 100f));
				agent.dodgeGainScaler += dodgeGain / 100f;
			}
			if (critGain > 0) {
//				agent.crit = (int)(agent.crit * (1 + critGain / 100f));
				agent.critGainScaler += critGain / 100f;
			}
			if (healthGain > 0) {
//				agent.maxHealth = (int)(agent.maxHealth * (1 + healthGain / 100f));
//				agent.health = (int)(agent.health * (1 + healthGain / 100f));
				agent.maxHealthGainScaler += healthGain / 100f;
			}
			if (manaGain > 0) {
//				agent.maxMana = (int)(agent.maxMana * (1 + manaGain / 100f));
//				agent.mana = (int)(agent.mana * (1 + manaGain / 100f));
				agent.maxManaGainScaler += manaGain / 100f;
			}
			if (physicalHurtGain > 0) {
				agent.physicalHurtScaler += physicalHurtGain / 100f;
			}
			if (magicHurtGain > 0) {
				agent.magicalHurtScaler += magicHurtGain / 100f;
			}
			if (healthAbsorbGain > 0) {
				agent.healthAbsorbScalser += healthAbsorbGain / 100f;
			}
			if (hardBeatGain > 0) {
				agent.hardBeatProbability += hardBeatGain / 100f;
			}
			if (brambleShiledGain > 0) {
				agent.reflectScaler += brambleShiledGain / 100f;
			}
			if (magicShieldGain > 0) {
				agent.decreaseHurtScaler += magicShieldGain / 100f;
			}
			if (allPropertiesGain > 0) {
				agent.originalAttack += allPropertiesGain;
				agent.originalAttackSpeed += allPropertiesGain;
				agent.originalArmor += allPropertiesGain;
				agent.originalManaResist += allPropertiesGain;
				agent.originalDodge += allPropertiesGain;
				agent.originalCrit += allPropertiesGain;
			}
		}

	}
}
