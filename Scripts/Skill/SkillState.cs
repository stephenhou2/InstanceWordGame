using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class SkillState {

		public Skill sourceSkill;

		public string stateName;

		public bool removeWhenQuitFight;

		public IEnumerator durativeSkillEffect;


		public SkillState(Skill sourceSkill, string stateName, bool removeWhenQuitFight, IEnumerator durativeSkillEffect){

			this.sourceSkill = sourceSkill;
			this.stateName = stateName;
			this.removeWhenQuitFight = removeWhenQuitFight;
			this.durativeSkillEffect = durativeSkillEffect;

		}
	}
}
