using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public abstract class ActiveSkill : Skill {


		public int baseManaConsume;//基础魔法消耗

		public int manaConsumeGain;//魔法消耗增长值

		//技能的魔法消耗
		public int manaConsume{
			get{
				return baseManaConsume + (skillLevel / 5) * manaConsumeGain;
			}
		}

	}
}
