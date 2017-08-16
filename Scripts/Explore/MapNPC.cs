using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney{
	
	public class MapNPC : MonoBehaviour {

		private NPC mNpc;
		[HideInInspector]public NPC npc{
			get{
				return mNpc;
			}
			set{
				mNpc = value;
				GetComponent<SpriteRenderer> ().sprite = GameManager.Instance.allMapSprites.Find (delegate(Sprite s) {
					return s.name == mNpc.spriteName;
				});

			}
		}


	}
}
