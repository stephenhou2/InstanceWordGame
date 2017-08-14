using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;


public class SpellViewController : MonoBehaviour {

	public SpellView spellView;

	// 已输入的所有字母
	private StringBuilder enteredCharacters = new StringBuilder();

	// 字母a的ASCII码
//	private int aInAscii = (int)('a'); 

	private SpellPurpose spellPurpose;

	// 容器，记录所有输入的字母及数量
	private int[] enteredCharactersArray = new int[26];
//	// 容器，记录字母碎片不足的字母以及缺少的数量
//	private int[] unsufficientCharacters = new int[26];

	// 最小制造数量
	private int minCreateCount = 1;

	// 目标物品的英文名称（仅当从图鉴界面进入时不为空）
	private string itemNameInEnglish;

	// 目标物品（从图鉴系统中选择的物品）
	private Item itemToSpell;

	// 根据玩家已有字母碎片数量计算出得目标物品最大制造数
	private int maxCreateCount{

		get{
			int myMaxCreateCount = int.MaxValue;

			for(int i = 0;i<enteredCharacters.Length;i++){

				char character = enteredCharacters [i];

				int characterIndex = (int)character - CommonData.aInASCII;

				int characterCount = Player.mainPlayer.charactersCount [characterIndex];

				if (characterCount < myMaxCreateCount) {
					myMaxCreateCount = characterCount;
				}

			}

			return myMaxCreateCount;

		}
	}

	// 目标物品制造数量，默认为1个
	private int createCount = 1;

	// 要制造的物品列表（因为武器装备的属性各不相同，每个物品制造后需单独存储数据，故使用列表作为容器）
	// 列表中相同类型的消耗品放在同一个条目中
	private List<Item> createdItems = new List<Item> ();


	/// <summary>
	/// 初始化拼写界面
	/// </summary>
	public void SetUpSpellView(Item item,SpellPurpose spellPurpose){

		if (item != null) {
			this.itemNameInEnglish = item.itemNameInEnglish;
		}

		this.spellPurpose = spellPurpose;

		if (spellPurpose == SpellPurpose.Strengthen) {
			this.itemToSpell = item;
		}

		spellView.SetUpSpellView (item,spellPurpose);

		GetComponent<Canvas>().enabled = true;

		ClearUnsufficientCharacters ();

	}

	/// <summary>
	/// Clears the unsufficient characters.
	/// </summary>
	private void ClearUnsufficientCharacters(){
		for(int i = 0; i<enteredCharactersArray.Length;i++){
			enteredCharactersArray [i] = 0;
		}
	}

	/// <summary>
	/// 拼写界面输入字母时响应方法
	/// </summary>
	/// <param name="character">输入的字母</param>
	public void EnterCharacter(string character){

		if (enteredCharacters.Length < spellView.characterTexts.Length) {

			int characterIndex = (int)character[0] - CommonData.aInASCII;

			enteredCharacters.Append (character);

			enteredCharactersArray [characterIndex]++;

			string characterWithColor = string.Empty;

			// 字母碎片足够的字母使用绿色，不足的字母使用红色
			if (Player.mainPlayer.charactersCount [characterIndex] >= enteredCharactersArray[characterIndex]) {
				characterWithColor = "<color=green>" + character + "</color>";
			} else {
				characterWithColor = "<color=red>" + character + "</color>";
			}
				
			// 更新拼写界面已输入字母界面ui
			spellView.OnEnterCharacter (enteredCharacters, characterWithColor);

			if (spellPurpose == SpellPurpose.Strengthen) {

				if (enteredCharacters.ToString () != itemToSpell.itemNameInEnglish) {
					return;
				}

				if (!CheckCharactersSufficient (1)) {
					return;
				}

				StartCoroutine ("StrengthenItem");
//				StrengthenItem (itemToSpell);
			}

		}

	}

	/// <summary>
	///  delete按钮点击响应方法
	/// </summary>
	public void Backspace(){

		if (enteredCharacters.Length > 0) {
			
			// 记录删除的字母对应在字母表中的位置
			int removedCharacterIndex = (int)enteredCharacters[enteredCharacters.Length - 1] - CommonData.aInASCII;

			// 删除已输入字母数组的最后一位
			enteredCharacters.Remove (enteredCharacters.Length - 1, 1);


			// 如果删除的字母在不足数组中，对应字母的不足数－1
//			if (unsufficientCharacters [removedCharacterIndex] > 0) {
//				unsufficientCharacters [removedCharacterIndex]--;
//			}

			enteredCharactersArray[removedCharacterIndex]--;
			if (enteredCharactersArray [removedCharacterIndex] < 0) {
				throw new System.Exception ("字母删除越界");
			}


			// 更新ui
			spellView.OnBackspace (enteredCharacters.Length);
		}
	}


	/// <summary>
	/// 制造一个物品
	/// </summary>
	public void CreateOne(){

		createCount = 1;

		if (!CheckCharactersSufficient(createCount)) {
			return;
		}

		itemToSpell = CheckEnteredWord ();

		if (itemToSpell == null) {
			return;
		}

		CreateItem (itemToSpell);

	}

	/// <summary>
	/// 选择制造多个物品
	/// </summary>
	public void SelectCreateCount(){

		itemToSpell = CheckEnteredWord ();

		if (itemToSpell == null) {
			return;
		}
			
		spellView.SetUpCreateCountHUD (minCreateCount, maxCreateCount);


	}

	/// <summary>
	/// 选择数量界面确认按钮点击响应方法
	/// </summary>
	public void ConfirmCreateCount(){

		if (!CheckCharactersSufficient(createCount)) {
			return;
		}

		CreateItem (itemToSpell);
	
	}

	/// <summary>
	/// 如果从图鉴系统进入，检查输入的单词是否和对应物品一致
	/// 检查拼写的单词是否存在
	/// </summary>
	/// return 返回对应的物品，不一致或不存在返回null，其余返回对应的item
	private Item CheckEnteredWord(){

		// 从图鉴接口进入，目标物品名称不为空
		if (itemNameInEnglish != null) {
			
			if (!enteredCharacters.ToString ().Equals (itemNameInEnglish)) {
				Debug.Log ("请输入正确的单词");
				return null;
			} 

		}

		Item item = GameManager.Instance.allItems.Find (delegate(Item obj) {
			
			return obj.itemNameInEnglish == enteredCharacters.ToString ();
		});

		if (item == null) {
			Debug.Log("没有这种物品");
		}

		return item;



	}
		
	/// <summary>
	/// 检查玩家字母碎片数量是否足够
	/// </summary>
	/// 数量足够返回true，不足返回false
	private bool CheckCharactersSufficient(int count){

		bool sufficient = true;

		StringBuilder unsufficientCharactesStr= new StringBuilder ();

		for (int i = 0; i < enteredCharactersArray.Length; i++) {

			int numNeed = enteredCharactersArray [i] * count;

			if (Player.mainPlayer.charactersCount [i] < numNeed) {

				string unsufficientCharacter = ((char)(i + CommonData.aInASCII)).ToString ();
				unsufficientCharactesStr.AppendFormat ("字母碎片{0}缺少{1}个、",unsufficientCharacter,numNeed - Player.mainPlayer.charactersCount[i] * count);

				sufficient = false;

			}

		}
			
		if (!sufficient) {
			
			unsufficientCharactesStr.Remove (unsufficientCharactesStr.Length - 1, 1);

			Debug.Log (unsufficientCharactesStr);

		}

		return sufficient;

	}


	/// <summary>
	/// Creats the item.
	/// </summary>
	/// <param name="item">Item.</param>
	private void CreateItem(Item item){

		Item itemInBag = null;

		// 如果制造的物品是消耗品
		if (item.itemType == ItemType.Consumables) {

			Item newItem = new Item (item,true);

			newItem.itemCount = createCount;

			// 在玩家所有已有物品中查找指定名称的物品
			itemInBag = Player.mainPlayer.allItems.Find (delegate(Item obj) {
				return obj.itemNameInEnglish == enteredCharacters.ToString ();	
			});

			if (itemInBag != null) {

				// 如果玩家背包中存在对应消耗品，则对应消耗品数量 ＋＝ 制造数量
				itemInBag.itemCount += createCount;

				createdItems.Add (newItem);		

				// 更新剩余字母碎片
				UpdateOwnedCharacters ();

				// 初始化制造的物品列表界面
				spellView.SetUpCreateItemDetailHUD (createdItems);

				// 清除制造信息
				ClearSpellInfos ();

				return;

			}

			// 如果玩家背包中不存在对应消耗品，则背包中添加该物品
			Player.mainPlayer.allItems.Add (newItem);

			createdItems.Add (newItem);

			UpdateOwnedCharacters ();

			spellView.SetUpCreateItemDetailHUD (createdItems);

			ClearSpellInfos ();

		} 
		// 如果制造的物品不是消耗品，则每个物品都不一样，根据制造数量单独生成每一个物品
		else {

			for (int i = 0; i < createCount; i++) {

				Item newItem = new Item (item,false);

				newItem.itemCount = 1;

				Player.mainPlayer.allItems.Add (newItem);

				createdItems.Add (newItem);

			}

			UpdateOwnedCharacters ();

			spellView.SetUpCreateItemDetailHUD (createdItems);

			ClearSpellInfos ();

		}

	}

	private IEnumerator StrengthenItem(){

		yield return new WaitForSeconds (0.05f);
		spellView.SetUpStrengthenItemDetailHUD (itemToSpell);

	}

	public void ConfirmStrengthenItem(){

		if (CheckCharactersSufficient (1)) {

			string strengthenGainStr = itemToSpell.StrengthenItem ();

			UpdateOwnedCharacters ();

			spellView.UpdateStrengthenItemDetailHUD (itemToSpell, strengthenGainStr);
		}
	}

	/// <summary>
	/// 清除本次制造信息
	/// </summary>
	private void ClearSpellInfos(){

		enteredCharacters = new StringBuilder ();

		for (int i = 0; i < enteredCharactersArray.Length; i++) {

			enteredCharactersArray [i] = 0;

		}

		createdItems.Clear ();

		createCount = 1;

		itemToSpell = null;
	}


	// 数量加减按钮点击响应
	public void CreateCountPlus(int plus){
		
		int targetCount = createCount + plus;
		// 最大或最小值直接返回
		if (targetCount > maxCreateCount || targetCount <minCreateCount) {
			return;
		}

		spellView.UpdateCreateCountHUD (targetCount,spellPurpose);

		createCount = targetCount;
	
	}

	/// <summary>
	/// 选择数量的slider拖动时响应方法
	/// </summary>
	public void CreateSliderDrag(){

		createCount = (int)spellView.countSlider.value;

		spellView.UpdateCreateCountHUD (createCount,spellPurpose);

	}
		
	public void QuitCreateCountHUD(){

		createCount = 1;

		spellView.QuitSpellCountHUD ();

	}

	// 制造出新物品后更新玩家剩余字母碎片
	private void UpdateOwnedCharacters(){
		
		for (int i = 0; i < enteredCharacters.Length; i++) {
			int characterIndex = (int)enteredCharacters [i] - CommonData.aInASCII;
			Player.mainPlayer.charactersCount [characterIndex] -= createCount;
		}
	}

	public void QuitCreateDetailHUD(){

		spellView.OnQuitCreateDetailHUD ();
	}

	public void QuitStrengthenItemDetailHUD(){
		ClearSpellInfos ();
		spellView.QuitStrengthenItemDetailHUD();
		QuitSpellPlane ();
	}

	public void QuitSpellPlane(){
		
		spellView.OnQuitSpellPlane ();

		if (itemNameInEnglish != null) {

			switch (spellPurpose) {
			case SpellPurpose.Create:
				GameObject produceCanvas = GameObject.Find (CommonData.instanceContainerName + "/ProduceCanvas");
				produceCanvas.GetComponent<Canvas> ().enabled = true;
				break;
			case SpellPurpose.Strengthen:
				GameObject bagCanvas = GameObject.Find (CommonData.instanceContainerName + "/BagCanvas");
				bagCanvas.GetComponent<BagViewController> ().SetUpBagView ();
				break;
			default:
				break;
			}


			DestroyInstances ();

			return;

		}

		GameObject homeCanvas = GameObject.Find (CommonData.instanceContainerName + "/HomeCanvas");

		if (homeCanvas != null) {
			homeCanvas.GetComponent<HomeViewController> ().SetUpHomeView ();
			DestroyInstances ();
		}

	}

	/// <summary>
	/// 退出拼写界面时清除内存
	/// </summary>
	private void DestroyInstances(){
		TransformManager.DestroyTransform (gameObject.transform);
		TransformManager.DestroyTransfromWithName ("CreatedItemDetailModel", TransformRoot.InstanceContainer);
		TransformManager.DestroyTransfromWithName ("CreatedItemDetailPool", TransformRoot.PoolContainer);
		TransformManager.DestroyTransfromWithName ("StrengthenGainTextModel", TransformRoot.InstanceContainer);
		TransformManager.DestroyTransfromWithName ("StrengthenGainTextPool", TransformRoot.PoolContainer);

	}

}
