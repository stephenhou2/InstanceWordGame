using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	// 地图物品类型枚举
	public enum MapItemType{
		Buck,
		Pot,
		TreasureBox,
		Tree,
		Stone,
		NormalTrapOn,
		NormalTrapOff,
		Switch,
		Door,
		MovableFloor,
		Transport,
		Billboard,
		FireTrap,
		Hole,
		MovableBox,
		LauncherTowardsUp,
		LauncherTowardsDown,
		LauncherTowardsLeft,
		LauncherTowardsRight,
		Plant,
		PressSwitch
	}

	public abstract class MapItem : MonoBehaviour {

		public string audioClipName;

		protected Animator mapItemAnimator;

		protected SpriteRenderer mapItemRenderer;

		protected CallBack animEndCallBack;

		public MapItemType mapItemType;

		protected BoxCollider2D bc2d;

		protected virtual void Awake(){

			mapItemAnimator = GetComponent<Animator> ();

			mapItemRenderer = GetComponent<SpriteRenderer> ();

			bc2d = GetComponent<BoxCollider2D> ();

//			InitMapItem ();

		}

		public abstract void InitMapItem ();

		public void SetSortingOrder(int order){
			mapItemRenderer.sortingOrder = order;
		}


//		public void SetSortingLayer(string layerName){
//			mapItemRenderer.sortingLayerName = layerName;
//		}


	}


}
