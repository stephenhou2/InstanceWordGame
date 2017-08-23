using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

namespace WordJourney
{
	public class BattlePlayerController : MonoBehaviour {


		[HideInInspector]public Player player;

		// 事件回调
		public ExploreEventHandler enterMonster;
		public ExploreEventHandler enterItem;
		public ExploreEventHandler enterNpc;

		// 移动速度
		public float moveDuration = 0.5f;			

		// 当前的单步移动动画对象
		private Tweener moveTweener;

		// 标记是否在单步移动中
		private bool inSingleMoving;

		// 移动路径点集
		private List<Vector3> pathPosList;

		// 正在前往的节点位置
		public Vector3 singleMoveEndPos;

		// 移动的终点位置
		private Vector3 endPos;

		public Animator animator;

		// 碰撞检测层
		public LayerMask collosionLayer;			//Layer on which collision will be checked.

		private BoxCollider2D boxCollider; 		//The BoxCollider2D component attached to this object.
		private Rigidbody2D rb2D;				//The Rigidbody2D component attached to this object.

		private void Awake(){

			player = Player.mainPlayer;

			animator = GetComponent<Animator> ();

			//Get a component reference to this object's BoxCollider2D
			boxCollider = GetComponent <BoxCollider2D> ();

			//Get a component reference to this object's Rigidbody2D
			rb2D = GetComponent <Rigidbody2D> ();

		}

		/// <summary>
		/// 按照指定路径 pathPosList 移动到终点 endPos
		/// </summary>
		/// <param name="pathPosList">Path position list.</param>
		/// <param name="endPos">End position.</param>
		public void MoveToEndByPath(List<Vector3> pathPosList,Vector3 endPos){

			this.pathPosList = pathPosList;

			this.endPos = endPos;


			if (pathPosList.Count == 0) {

				// 移动路径中没有点时，说明没有有效移动路径，此时终点设置为当前单步移动的终点
				this.endPos = singleMoveEndPos;

				moveTweener.OnComplete (() => {
					inSingleMoving = false;

				});

				Debug.Log ("无有效路径");

				return;
			}

			StartCoroutine ("MoveWithNewPath");

		}

		/// <summary>
		/// 按照新路径移动
		/// </summary>
		/// <returns>The with new path.</returns>
		private IEnumerator MoveWithNewPath(){

			// 如果移动动画不为空，则等待当前移动结束
			if (moveTweener != null) {

				// 动画结束时标记isSingleMoving为false（单步行动结束），不删除路径点（因为该单步行动是旧路径上的行动点）
				moveTweener.OnComplete (() => {
					inSingleMoving = false;
				});

				// 等待单步行动结束
				while (inSingleMoving) {
					yield return null;
				}

			} 

			// 移动到新路径上的下一个节点
			MoveToNextPosition ();

		}


		/// <summary>
		/// 匀速移动到指定节点位置
		/// </summary>
		/// <param name="targetPos">Target position.</param>
		private void MoveToPosition(Vector3 targetPos){

//			Debug.Log (string.Format ("Player pos:[{0},{1}],target pos:[{2},{3}]", transform.position.x, transform.position.y,targetPos.x,targetPos.y));

			moveTweener =  transform.DOMove (targetPos, moveDuration).OnComplete (() => {

				// 动画结束时已经移动到指定节点位置，标记单步行动结束
				inSingleMoving = false;

				// 将当前节点从路径点中删除
				pathPosList.RemoveAt(0);

				// 移动到下一个节点位置
				MoveToNextPosition();
			});

			// 设置匀速移动
			moveTweener.SetEase (Ease.Linear);

		}
			
		/// <summary>
		/// 判断当前是否已经走到了终点位置，位置度容差0.05
		/// </summary>
		/// <returns><c>true</c>, if end point was arrived, <c>false</c> otherwise.</returns>
		private bool ArriveEndPoint(){

			if(Mathf.Abs(transform.position.x - endPos.x) <= 0.05f &&
				Mathf.Abs(transform.position.y - endPos.y) <= 0.05f){
				return true;
			}

			return false;

		}

		/// <summary>
		/// 继续当前未完成的单步移动
		/// </summary>
		public void ContinueMove(){

			moveTweener.Play ();

			boxCollider.enabled = true;

		}

		/// <summary>
		/// 移动到下一个节点
		/// </summary>
		private void MoveToNextPosition ()
		{

			// 到达终点前的单步移动开始前进行碰撞检测
			// 1.如果碰撞体存在，则根据碰撞体类型给exploreManager发送消息执行指定回调
			// 2.如果未检测到碰撞体，则开始本次移动
			if (pathPosList.Count == 1) {

				boxCollider.enabled = false;

				RaycastHit2D r2d = Physics2D.Linecast (transform.position, pathPosList [0], collosionLayer);

				if (r2d.transform != null) {

					switch (r2d.transform.tag) {

					case "monster":

						moveTweener.Pause ();

						enterMonster (r2d.transform);

						break;

					case "item":

						moveTweener.Kill (false);

						singleMoveEndPos = transform.position;

						inSingleMoving = false;

						enterItem (r2d.transform);

						boxCollider.enabled = true;

						return;

					case "npc":

						moveTweener.Kill (false);

						singleMoveEndPos = transform.position;

						inSingleMoving = false;

						enterNpc (r2d.transform);

						boxCollider.enabled = true;

						return;

					default:
						boxCollider.enabled = true;
						break;

					}
				}
			}


			// 路径中没有节点
			// 按照行动路径已经将所有的节点走完
			if (pathPosList.Count == 0) {

				// 走到了终点
				if (ArriveEndPoint ()) {
					Debug.Log ("到达终点");
				} else {
					Debug.Log (string.Format("actual pos:{0}/ntarget pos:{1},predicat pos{2}",transform.position,endPos,singleMoveEndPos));
					throw new System.Exception ("路径走完但是未到终点");
				}
				return;
			}

			// 如果还没有走到终点
			if (!ArriveEndPoint ()) {

				Vector3 nextPos = pathPosList [0];

				// 记录下一节点位置
				singleMoveEndPos = nextPos;

				// 向下一节点移动
				MoveToPosition (nextPos);

				// 标记单步移动中
				inSingleMoving = true;

				return;

			}

		}

//		private IEnumerator SmoothMovement (Vector3 end,CallBack cb)
//		{
//			inSingleMoving = true;
//
//			float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
//
//			while(sqrRemainingDistance > float.Epsilon)
//			{
//				Vector3 newPostion = Vector3.MoveTowards(rb2D.position, end, 1f / moveDuration * Time.deltaTime);
//
//				rb2D.MovePosition (newPostion);
//
//				sqrRemainingDistance = (transform.position - end).sqrMagnitude;
//
//				yield return null;
//			}
//
//			inSingleMoving = false;
//
//			cb ();
//		}




		public float restartLevelDelay = 1f;		//Delay time in seconds to restart level.

		public AudioClip moveSound1;				//1 of 2 Audio clips to play when player moves.
		public AudioClip moveSound2;				//2 of 2 Audio clips to play when player moves.
		public AudioClip eatSound1;					//1 of 2 Audio clips to play when player collects a food object.
		public AudioClip eatSound2;					//2 of 2 Audio clips to play when player collects a food object.
		public AudioClip drinkSound1;				//1 of 2 Audio clips to play when player collects a soda object.
		public AudioClip drinkSound2;				//2 of 2 Audio clips to play when player collects a soda object.
		public AudioClip gameOverSound;				//Audio clip to play when player dies.

		#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
		private Vector2 touchOrigin = -Vector2.one;	//Used to store location of screen touch origin for mobile controls.
		#endif


			
		//AttemptMove overrides the AttemptMove function in the base class MovingObject
		//AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
//		private void AttemptMove <T> (int xDir, int yDir)
//		{
//
//			//Hit will store whatever our linecast hits when Move is called.
//			RaycastHit2D hit;
//
//			//Set canMove to true if Move was successful, false if failed.
//			bool canMove = Move (xDir, yDir, out hit);
//
//			//Check if nothing was hit by linecast
//			if(hit.transform == null)
//				//If nothing was hit, return and don't execute further code.
//				return;
//
//			//Get a component reference to the component of type T attached to the object that was hit
//			T hitComponent = hit.transform.GetComponent <T> ();
//
//			//If canMove is false and hitComponent is not equal to null, meaning MovingObject is blocked and has hit something it can interact with.
//			if(!canMove && hitComponent != null)
//
//				//Call the OnCantMove function and pass it hitComponent as a parameter.
//				OnCantMove (hitComponent);
//
//			//If Move returns true, meaning Player was able to move into an empty space.
//			if (Move (xDir, yDir, out hit)) 
//			{
//				//Call RandomizeSfx of SoundManager to play the move sound, passing in two audio clips to choose from.
//				SoundManager.instance.RandomizeSfx (moveSound1, moveSound2);
//			}
//
//			//Since the player has moved and lost food points, check if the game has ended.
//			CheckIfGameOver ();
//
//		}
//

		//OnCantMove overrides the abstract function OnCantMove in MovingObject.
		//It takes a generic parameter T which in the case of Player is a Wall which the player can attack and destroy.
//		private void OnCantMove <T> (T component)
//		{
//			//Set hitWall to equal the component passed in as a parameter.
//			Wall hitWall = component as Wall;
//
//			//Set the attack trigger of the player's animation controller in order to play the player's attack animation.
//			animator.SetTrigger ("playerChop");
//		}
//

		//OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
//		private void OnTriggerEnter2D (Collider2D other)
//		{
//			//Check if the tag of the trigger collided with is Exit.
//			if(other.tag == "Exit")
//			{
//				//Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
//				Invoke ("Restart", restartLevelDelay);
//
//				//Disable the player object since level is over.
//				enabled = false;
//			}
//
//			//Check if the tag of the trigger collided with is Food.
//			else if(other.tag == "Food")
//			{
//
//				//Call the RandomizeSfx function of SoundManager and pass in two eating sounds to choose between to play the eating sound effect.
//				SoundManager.instance.RandomizeSfx (eatSound1, eatSound2);
//
//				//Disable the food object the player collided with.
//				other.gameObject.SetActive (false);
//			}
//
//			//Check if the tag of the trigger collided with is Soda.
//			else if(other.tag == "Soda")
//			{
//
//				//Call the RandomizeSfx function of SoundManager and pass in two drinking sounds to choose between to play the drinking sound effect.
//				SoundManager.instance.RandomizeSfx (drinkSound1, drinkSound2);
//
//				//Disable the soda object the player collided with.
//				other.gameObject.SetActive (false);
//			}
//		}
			
	}
}
