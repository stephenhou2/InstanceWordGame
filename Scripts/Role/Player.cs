using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Player : BattleAgent {

	private static Player mPlayerSingleton;

	private static object objectLock = new object();

//	[HideInInspector]public int[] charactersCount = new int[26];

	private int[] mCharactersCount;

	[HideInInspector]public int[] charactersCount{

		get{
			if (mCharactersCount == null) {
				mCharactersCount = new int[26];
				for(int i = 0;i<mCharactersCount.Length;i++){
					mCharactersCount[i] = 10;
				}
			}
			return mCharactersCount;
		}

	}


	// 玩家角色单例
	public static Player mainPlayer{
		get{
			if (mPlayerSingleton == null) {
				lock (objectLock) {
					ResourceManager.Instance.LoadAssetWithFileName("player",()=>{
						mPlayerSingleton = GameObject.Find ("Player").GetComponent<Player>();
						mPlayerSingleton.transform.SetParent(null);
						mPlayerSingleton.ResetBattleAgentProperties (true,false);
						DontDestroyOnLoad (mPlayerSingleton);
					},true);
				}
			}
//			else{
//				mPlayerSingleton.ResetBattleAgentProperties (false,false);
//			}

			return mPlayerSingleton;
		}
//		set{
//			mPlayerSingleton = value;
//		}

	}


	public List<Skill> allLearnedSkills = new List<Skill>();

	public int skillPointsLeft;


	public override void Awake(){

		base.Awake ();
	
		#warning 这里用来玩家信息初始化
	}




	public void UpdateValidActionType(){

		switch (validActionType) {

		case ValidActionType.All:
			break;
		case ValidActionType.PhysicalExcption:
			SetUpPlayerValidAction(false, true, true, true);
			break;
		case ValidActionType.MagicException:
			SetUpPlayerValidAction(true, false, true, true);
			break;
		case ValidActionType.None:
			SetUpPlayerValidAction(false, false, false, false);
			break;
		case ValidActionType.PhysicalOnly:
			SetUpPlayerValidAction(true, false, false, true);
			break;
		case ValidActionType.MagicOnly:
			SetUpPlayerValidAction(false, true, false, true);
			break;
		default:
			break;
		}
		// 如果技能还在冷却中或者玩家气力值小于技能消耗的气力值，则相应按钮不可用
		for (int i = 0;i < skillsEquiped.Count;i++) {

			Skill s = skillsEquiped [i];
			// 如果是冷却中的技能
			if (s.isAvalible == false) {
				s.actionCount++;
//				int actionBackCount = s.actionConsume - s.actionCount + 1;
				Debug.Log (s.skillName + "从使用开始经过了" + s.actionCount + "回合");
				if (s.actionCount > s.actionConsume) {
					s.isAvalible = true;
					s.actionCount = 0;
				} 
			}
		}
	}

	//根据玩家的可用行动状态
	private void SetUpPlayerValidAction(bool isAttackEnable,bool isSkillEnable,bool isItemEnable,bool isDefenceEnable){
		
		this.isAttackEnable = isAttackEnable;
		this.isSkillEnable = isSkillEnable;
		this.isItemEnable = isItemEnable;
		this.isDefenceEnable = isDefenceEnable;

	}

	// 获取玩家已学习的技能
	public Skill GetPlayerLearnedSkill(string skillName){
		Skill s = null;
		s = allLearnedSkills.Find (delegate(Skill obj) {
			return obj.skillName == skillName;	
		});
		return s;
	}

	/// <summary>
	/// 分解物品
	/// </summary>
	/// <returns>分解后获得的字母碎片</returns>
	/// <param name="item">Item.</param>
	public List<char> ResolveItem(Item item,int resolveCount){

		List<char> charactersReturn = new List<char> ();

		int charactersReturnCount = item.itemNameInEnglish.Length / 2;

		char[] charArray = item.itemNameInEnglish.ToCharArray ();

		List<char> charList = new List<char> ();

		for (int i = 0; i < charArray.Length; i++) {
			charList.Add (charArray [i]);
		}

		for (int j = 0; j < resolveCount; j++) {
			
			for (int i = 0; i < charactersReturnCount; i++) {

				char character = ReturnRandomCharacters (ref charList);

				int charIndex = (int)character - CommonData.aInASCII;

				charactersCount [charIndex]++;

				charactersReturn.Add (character);
			}
		}

		item.itemCount -= resolveCount;

		if (item.itemCount <= 0) {
			allItems.Remove (item);

			if (item.equiped) {

				int itemIndex = allEquipedItems.IndexOf (item);

				allEquipedItems [itemIndex] = null;
			}

		}


		return charactersReturn;

	}

	public void ArrangeAllItems(){

		for (int i = 0; i < allItems.Count - 1; i++) {

			Item item = allItems [i];

			if (item.itemType == ItemType.Consumables) {

				for (int j = i + 1; j < allItems.Count; j++) {

					Item itemBackwords = allItems [j];

					if (item.itemId == itemBackwords.itemId) {

						item.itemCount += itemBackwords.itemCount;

						allItems.Remove (itemBackwords);

						j--;

					}

				}

			}

		}


	}

	public List<char> CheckUnsufficientCharacters(string itemNameInEnglish){

		char[] charactersArray = itemNameInEnglish.ToCharArray ();

		int[] charactersNeed = new int[26];

		List<char> unsufficientCharacters = new List<char> ();

		foreach (char c in charactersArray) {
			int index = (int)c - CommonData.aInASCII;
			charactersNeed [index]++;
		}

		// 判断玩家字母碎片是否足够
		for(int i = 0;i<charactersNeed.Length;i++){

			if (charactersNeed [i] > Player.mainPlayer.charactersCount[i]) {

				char c = (char)(i + CommonData.aInASCII);

				unsufficientCharacters.Add (c);

			}

		}

		return unsufficientCharacters;

	}

	/// <summary>
	/// 从单词的字母组成中随机返回一个字母
	/// </summary>
	/// <returns>The random characters.</returns>
	private char ReturnRandomCharacters(ref List<char> charList){

		int charIndex = (int)Random.Range (0, charList.Count - float.Epsilon);

		char character = charList [charIndex];

		charList.RemoveAt (charIndex);

		return character;

	}

}
