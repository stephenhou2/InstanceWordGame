using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class Test : MonoBehaviour {

		public GameObject go;
		public InstancePool pool{
			get{
				return InstancePool.GetOrCreateInstancePool ("Pool",null);
			}
		}
		public Transform container;

		public Animator animator;

		public void Play(){
			go.GetComponent<SpriteRenderer> ().enabled = false;
			animator.gameObject.SetActive (true);
			animator.SetTrigger ("Play");
		}

		public void AddToPool(){
			go.SetActive (false);
			animator.gameObject.SetActive (false);
			pool.AddInstanceToPool (go);
		}

		public void Reset(){
			go.SetActive (true);
			go.GetComponent<SpriteRenderer> ().enabled = true;
			go.transform.SetParent (container);
		}

		public void AddToPoolAndReset(){
			AddToPool ();
			Reset ();
		}

	}


}
