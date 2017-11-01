using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;
using Transform = UnityEngine.Transform;

namespace WordJourney{


	public class DrTest : MonoBehaviour {

		public UnityArmatureComponent armatureCom;

		public void ChangeBone(){

				UnityFactory.factory.ReplaceSlotDisplay (null, "playerSide", 
					"weapon1", "staff", armatureCom.armature.GetSlot ("weapon1"));
				UnityFactory.factory.ReplaceSlotDisplay(null,"playerSide",
					"weapon2", "staff", armatureCom.armature.GetSlot ("weapon2"));

		}

			public void ChangeBoneBack(){

				UnityFactory.factory.ReplaceSlotDisplay (null, "playerSide", 
					"weapon1", "axe", armatureCom.armature.GetSlot ("weapon1"));
				UnityFactory.factory.ReplaceSlotDisplay(null,"playerSide",
					"weapon2", "axe2", armatureCom.armature.GetSlot ("weapon2"));

			}


	}
}
