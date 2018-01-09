﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class Launcher : MapItem {

		public Bullet bulletModel;
		private InstancePool bulletPool;
		public Transform bulletsContainer;

		public float fireInterval;

		private Vector2 range;

		private MyTowards towards;

		private IEnumerator launcherFireCoroutine;

//		new void Awake(){
//			mapItemAnimator = GetComponent<Animator> ();
//
//			mapItemRenderer = GetComponent<SpriteRenderer> ();
//
//			bc2d = GetComponent<BoxCollider2D> ();
//			this.range = new Vector2 (10, 10);
//			this.towards = MyTowards.Right;
//			bulletPool = InstancePool.GetOrCreateInstancePool ("BulletPool", CommonData.poolContainerName);
//			SetUpLauncher ();
//		}

		public override void InitMapItem ()
		{
//			gameObject.SetActive (true);
//			bc2d.enabled = true;
//			mapItemAnimator.enabled = true;
			SetUpLauncher ();
			SetSortingOrder (-(int)transform.position.y);
			bulletPool = InstancePool.GetOrCreateInstancePool ("BulletPool", CommonData.poolContainerName);

		}

		public override void AddToPool (InstancePool pool)
		{
//			bc2d.enabled = false;
//			mapItemAnimator.enabled = false;
			gameObject.SetActive(false);
			pool.AddInstanceToPool (this.gameObject);
		}

		public void SetRange(float width,float height){
			range = new Vector2 (width, height);
		}

		public void SetTowards(MyTowards towards){
			this.towards = towards;
		}

		private void SetUpLauncher(){
			switch(towards){	
			case MyTowards.Up:
				mapItemAnimator.SetTrigger ("Back");
				break;
			case MyTowards.Down:
				mapItemAnimator.SetTrigger ("Front");
				break;
			case MyTowards.Left:
				mapItemAnimator.SetTrigger ("Side");
				transform.localScale = new Vector3 (1, 1, 1);
				break;
			case MyTowards.Right:
				mapItemAnimator.SetTrigger ("Side");
				transform.localScale = new Vector3 (-1, 1, 1);
				break;
			}
			launcherFireCoroutine = Fire ();
			StartCoroutine (launcherFireCoroutine);
		}

		public void LauncherStopFire(){
			if (launcherFireCoroutine != null) {
				StopCoroutine (launcherFireCoroutine);
				mapItemAnimator.SetTrigger ("Reset");
			}
		}



		private IEnumerator Fire(){

			while (true) {
				
				yield return new WaitForSeconds (fireInterval);

				Bullet bullet = bulletPool.GetInstance<Bullet> (bulletModel.gameObject, bulletsContainer);

				bullet.BulletMove (this.transform.position, towards, range, bulletPool);
			}
		}


	}
}