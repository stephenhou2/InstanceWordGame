using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

namespace WordJourney
{
	public class BattlePlayerController : BattleAgentController {

		// 战斗中的玩家UI

		[HideInInspector]public Player player;

		public Button[] skillButtons;

		public Button[] itemButtons;

		private Transform mSkillAndItemDetailPlane;
		public Transform skillAndItemDetailPlane{
			get{
				if (mSkillAndItemDetailPlane == null) {
					mSkillAndItemDetailPlane = GameObject.Find (CommonData.instanceContainerName + "/SkillAndItemDetailPlane").transform;
				}
				return mSkillAndItemDetailPlane;
			}

		}


		private Sequence mSequence;

		public Sprite skillNormalBackImg; // 技能框默认背景图片
		public Sprite skillSelectedBackImg; // 选中技能的背景高亮图片

		private List<Sprite> skillSprites;

		private List<Sprite> itemSprites;


		public Transform battleGainsHUD;

		public Transform battleGainsContainer;

		public GameObject gainItemModel;

		private InstancePool battleGainsPool;

		// 移动速度
		public float moveDuration = 0.5f;			

		// 当前的单步移动动画对象
		private Tweener moveTweener;

		// 标记是否在单步移动中
		private bool inSingleMoving;

		// 移动路径点集
		private List<Vector3> pathPosList;

		// 正在前往的节点位置
		public Vector3 predicatePos;

		// 移动的终点位置
		private Vector3 endPos;



		protected override void Awake ()
		{
			player = Player.mainPlayer;
			
			animator = GetComponent<Animator> ();

			base.Awake ();
		}


		/// <summary>
		/// 按照指定路径 pathPosList 移动到终点 endPos
		/// </summary>
		/// <param name="pathPosList">Path position list.</param>
		/// <param name="endPos">End position.</param>
		public void MoveToEndByPath(List<Vector3> pathPosList,Vector3 endPos){

			this.pathPosList = pathPosList;

			this.endPos = endPos;
		
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
		/// 移动到下一个节点
		/// </summary>
		private void MoveToNextPosition(){

			// 路径中没有节点，有以下两种情况 
			// 1.原始路径中就没有节点，即没有有效的行动路径
			// 2.按照行动路径已经将所有的节点走完
			if (pathPosList.Count == 0) {

				// 走到了终点
				if (ArriveEndPoint ()) {
					Debug.Log ("到达终点");
				}
				return;
			}

			// 如果还没有走到终点
			if (!ArriveEndPoint ()) {
				
				Vector3 nextPos = pathPosList [0];

				// 记录下一节点位置
				predicatePos = nextPos;

				// 向下一节点移动
				MoveToPosition (nextPos);

				// 标记单步移动中
				inSingleMoving = true;

				return;

			}



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


		public void SetUpUI(Player player,List<Sprite> skillSprites,List<Sprite> itemSprites){

			battleGainsPool = InstancePool.GetOrCreateInstancePool ("BattleGainsPool");
			
			this.skillSprites = skillSprites;
			this.itemSprites = itemSprites;


//			SetUpSkillButtonsStatus (player);
//			SetUpItemButtonsStatus (player);

		}

		private void SetUpSkillButtonsStatus(Player player){
			
			for (int i = 0; i < player.skillsEquiped.Count; i++) {

				Button skillButton = skillButtons [i];
				Skill skill = player.skillsEquiped [i];

				Image skillIcon = skillButton.GetComponent<Image> ();

				Text strengthConsumeText = skillButton.transform.parent.FindChild ("StrengthConsumeText").GetComponent<Text>();

				Text skillNameText = skillButton.transform.FindChild ("SkillName").GetComponent<Text> ();

				skillIcon.sprite = skillSprites.Find (delegate(Sprite obj) {
					return obj.name == skill.skillIconName;
				});
				skillIcon.enabled = true;
				skillButton.interactable = true;
				strengthConsumeText.text = skill.strengthConsume.ToString();
				skillNameText.text = skill.skillName;
				skillButton.transform.GetComponentInChildren<Text> ().text = "";
			}

			for (int i = player.skillsEquiped.Count; i < skillButtons.Length; i++) {
				skillButtons [i].interactable = false;
			}
		}

		public void SetUpItemButtonsStatus(Player player){

			for (int i = 3; i < player.allEquipedItems.Count; i++) {
				
				Item consumable = player.allEquipedItems [i];

				Button itemButton = itemButtons [i - 3];

				Image itemIcon = itemButton.GetComponent<Image> ();

				Text itemCount = itemButton.transform.FindChild ("Text").GetComponent<Text> ();

				if (consumable == null) {
					
					itemButton.interactable = false;
					itemIcon.enabled = false;

					itemIcon.sprite = null;
					itemCount.text = string.Empty;

					continue;
				}
					
				itemIcon.sprite = itemSprites.Find (delegate(Sprite obj) {
					return obj.name == consumable.spriteName;
				});
				if (itemIcon.sprite != null) {
					itemIcon.enabled = true;
					itemButton.interactable = true;
					itemCount.text = consumable.itemCount.ToString ();
				}

				itemButton.interactable = (consumable.itemCount > 0) && player.isItemEnable;
			}

		}


		// 更新战斗中玩家UI的状态
		public void UpdateSkillButtonsStatus(Player player){
			
			for (int i = 0;i < player.skillsEquiped.Count;i++) {

				Skill s = player.skillsEquiped [i];
				// 如果是冷却中的技能
				if (s.isAvalible == false) {
					int actionBackCount = s.actionConsume - s.actionCount + 1;
					skillButtons [i].GetComponentInChildren<Text> ().text = actionBackCount.ToString ();
				} else {
					skillButtons [i].GetComponentInChildren<Text> ().text = "";
				}
				skillButtons [i].interactable = s.isAvalible && player.strength >= s.strengthConsume && player.isSkillEnable; 
			}

//			attackButton.interactable = player.isAttackEnable && player.strength >= player.attackSkill.strengthConsume;
//			defenceButton.interactable = player.isDefenceEnable && player.strength >= player.defenceSkill.strengthConsume;


		}


		public void QuitDetailPlane(){

			skillAndItemDetailPlane.gameObject.SetActive (false);

		}


		public void ShowItemDetail(int index,Item item){
			
			skillAndItemDetailPlane.SetParent (itemButtons [index].transform,false);

			skillAndItemDetailPlane.FindChild ("Name").GetComponent<Text> ().text = item.itemName;

			skillAndItemDetailPlane.FindChild ("Description").GetComponent<Text> ().text = item.itemDescription;

			skillAndItemDetailPlane.FindChild ("Detail").GetComponent<Text> ().text = item.GetItemPropertiesString ();

			skillAndItemDetailPlane.gameObject.SetActive (true);
		}
			

		public void SetUpBattleGainsHUD(List<Item> battleGains){

			for (int i = 0; i < battleGains.Count; i++) {

				Item item = battleGains [i];

				Transform gainItem = battleGainsPool.GetInstance<Transform> (gainItemModel, battleGainsContainer);

				Image itemIcon = gainItem.FindChild ("ItemIcon").GetComponent<Image> ();

				Text itemCount = gainItem.FindChild ("ItemCount").GetComponent<Text> ();

				itemIcon.sprite = GameManager.Instance.allItemSprites.Find (delegate(Sprite obj) {
					return obj.name == item.spriteName;
				});

				if (itemIcon.sprite != null) {
					itemIcon.enabled = true;
				}

				itemCount.text = item.itemCount.ToString ();
			}

		}

		public void QuitBattleGainsHUD (){

			foreach (Transform trans in battleGainsContainer) {

				Image itemIcon = trans.FindChild ("ItemIcon").GetComponent<Image> ();

				Text itemCount = trans.FindChild ("ItemCount").GetComponent<Text> ();

				itemIcon.sprite = null;
				itemIcon.enabled = false;
				itemCount.text = string.Empty;

			}

			battleGainsPool.AddChildInstancesToPool (battleGainsContainer);

			battleGainsHUD.gameObject.SetActive (false);

		}

		public void OnQuitBattle(){

			foreach (Button btn in skillButtons) {
				btn.interactable = false;
				btn.GetComponent<Image> ().enabled = false;
				foreach (Text t in btn.GetComponentsInChildren<Text>()) {
					t.text = string.Empty;
				}
			}

			foreach (Button btn in itemButtons) {
				btn.interactable = false;
				btn.GetComponent<Image> ().enabled = false;
				btn.GetComponentInChildren<Text> ().text = string.Empty;
			}
		}

		private BattlePlayerController mBaPlayerController;



		private List<Sprite> skillIcons = new List<Sprite>();

		//	private List<Item> consumables = new List<Item> ();

		// 角色UIView
		public BattlePlayerController baView{

			get{
				if (mBaPlayerController == null) {
					mBaPlayerController = GetComponent<BattlePlayerController> ();
				}
				return mBaPlayerController;
			}

		}


		public void SetUpBattlePlayerView(){

			//		for(int i = 3;i<player.allEquipedItems.Count;i++){
			//			Item consumable = player.allEquipedItems [i];
			//			consumables.Add (consumable);
			//		}

			List<Sprite> allItemSprites = GameManager.Instance.allItemSprites;

			if (skillIcons.Count != 0) {
				baView.SetUpUI (player,skillIcons,allItemSprites);
				return;
			}

			ResourceManager.Instance.LoadSpritesAssetWithFileName("skills/skills", () => {

				foreach(Sprite s in ResourceManager.Instance.sprites){
					skillIcons.Add(s);
				}
				baView.SetUpUI (player,skillIcons,allItemSprites);
			},true);

		}

		public void OnPlayerSelectSkill(int skillIndex){


//			baView.SelectedSkillAnim (player.currentSkill == player.attackSkill,
//				player.currentSkill == player.defenceSkill,
//				skillIndex);

		}

		public void OnPlayerUseItem(int itemIndex){


			Item item = player.allEquipedItems[itemIndex + 3];

			if (item == null) {
				return;
			}

			item.itemCount--;


			if (item.itemCount <= 0) {
				player.allEquipedItems [itemIndex + 3] = null;
				player.allItems.Remove (item);
				baView.SetUpItemButtonsStatus (player);
			}



			if (item.healthGain != 0 && item.manaGain != 0) {
				player.health += item.healthGain;
				player.strength += item.manaGain;
				baView.UpdateHealthBarAnim (player);
				baView.UpdateStrengthBarAnim (player);

				string hurtText = "<color=green>+" + item.healthGain.ToString() + "体力</color>" 
					+ "\t\t\t\t\t" 
					+ "<color=orange>+" + item.manaGain.ToString() + "魔法</color>";
				baView.PlayHurtHUDAnim(hurtText);

			}else if (item.healthGain != 0) {
				player.health += item.healthGain;
				baView.UpdateHealthBarAnim (player);
				string hurtText = "<color=green>+" + item.healthGain.ToString() + "体力</color>";
				baView.PlayHurtHUDAnim(hurtText);

			}else if (item.manaGain != 0) {
				player.strength += item.manaGain;
				baView.UpdateStrengthBarAnim (player);
				string hurtText = "<color=orange>+" + item.manaGain.ToString() + "魔法</color>";
				baView.PlayHurtHUDAnim(hurtText);
			}

			baView.SetUpItemButtonsStatus (player);

		}
			

		public void OnSkillButtonUp(){

			baView.QuitDetailPlane ();
		}

		public void OnItemLongPress(int index){
			Item i = player.allEquipedItems [3 + index];
			baView.ShowItemDetail (index, i);
		}

		public void OnItemButtonUp(){

			baView.QuitDetailPlane ();
		}

		public Skill DefaultSelectedSkill(){

			// 如果气力大于普通攻击所需的气力值，则默认选中普通攻击
			if (player.strength >= player.attackSkill.strengthConsume && player.validActionType != ValidActionType.PhysicalExcption) {
				player.currentSkill = player.attackSkill;
//				baView.SelectedSkillAnim (true, false, -1);
				return player.attackSkill;

			}

			// 如果玩家没有被沉默，默认选中可以第一个可以使用的技能
			if (player.validActionType != ValidActionType.MagicException) {
				foreach (Skill s in player.skillsEquiped) {
					if (s.isAvalible && player.strength >= s.strengthConsume) {
						player.currentSkill = s;
//						baView.SelectedSkillAnim (false, false, s.skillId);
						return s;
					}
				}
			}


			// 如果其他技能都无法使用，则默认选中防御
			player.currentSkill = player.defenceSkill;
//			baView.SelectedSkillAnim(false,true,-1);
			return player.defenceSkill;

		}

		public float restartLevelDelay = 1f;		//Delay time in seconds to restart level.

		public AudioClip moveSound1;				//1 of 2 Audio clips to play when player moves.
		public AudioClip moveSound2;				//2 of 2 Audio clips to play when player moves.
		public AudioClip eatSound1;					//1 of 2 Audio clips to play when player collects a food object.
		public AudioClip eatSound2;					//2 of 2 Audio clips to play when player collects a food object.
		public AudioClip drinkSound1;				//1 of 2 Audio clips to play when player collects a soda object.
		public AudioClip drinkSound2;				//2 of 2 Audio clips to play when player collects a soda object.
		public AudioClip gameOverSound;				//Audio clip to play when player dies.

		private Animator animator;					//Used to store a reference to the Player's animator component.

		#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
		private Vector2 touchOrigin = -Vector2.one;	//Used to store location of screen touch origin for mobile controls.
		#endif


		private void OnTriggerEnter2D(Collider2D other){

			Debug.Log ("peng");


		}
			
		//AttemptMove overrides the AttemptMove function in the base class MovingObject
		//AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
//		protected void AttemptMove <T> (int xDir, int yDir)
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
//		protected void OnCantMove <T> (T component)
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
