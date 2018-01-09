using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;



namespace WordJourney
{


	public class SpellViewController : MonoBehaviour {


		public SpellView spellView;

		// 已输入的所有字母
		private StringBuilder charactersEntered = new StringBuilder();


		// 容器，记录所有输入的字母及数量
		private int[] charactersEnteredArray = new int[26];

		private int[] charactersInsufficientArray = new int[26];

		// 目标物品（从图鉴系统中选择的物品）
		[HideInInspector]public ItemModel itemToCreate;

		// 制造出的物品
		private Item itemCreated;

		// 最小制造数量
		private int minCreateCount = 1;

		// 根据玩家已有字母碎片数量计算出得目标物品最大制造数
		private int maxCreateCount{

			get{
				int myMaxCreateCount = int.MaxValue;

				for(int i = 0;i<charactersEnteredArray.Length;i++){

					int characterNeed = charactersEnteredArray [i];

					if (characterNeed > 0) {
						int maxCreateCount = Player.mainPlayer.charactersCount [i] / characterNeed;
						if (maxCreateCount < myMaxCreateCount) {
							myMaxCreateCount = maxCreateCount;
						}
					}
				}

				return myMaxCreateCount;

			}
		}

		// 目标物品制造数量，默认为1个
		private int createCount = 1;


		/// <summary>
		/// 初始化拼写界面（制造）
		/// </summary>
		public void SetUpSpellViewForCreate(ItemModel itemModel){
			itemToCreate = itemModel;
			IEnumerator coroutine = SetUpViewAfterDataReady (itemModel);
			StartCoroutine (coroutine);
		}


		private IEnumerator SetUpViewAfterDataReady(ItemModel itemModel){

			bool dataReady = false;

			while (!dataReady) {
				dataReady = GameManager.Instance.gameDataCenter.CheckDatasReady (new GameDataCenter.GameDataType[] {
					GameDataCenter.GameDataType.UISprites,
					GameDataCenter.GameDataType.ItemModels,
					GameDataCenter.GameDataType.ItemSprites
				});
				yield return null;
			}

			#warning 这里测试拼写，人物字母碎片全都初始化为10个，后面去掉
			for (int i = 0; i < 26; i++) {
				Player.mainPlayer.charactersCount [i] = 10;
			}
				
			spellView.SetUpSpellViewWith (itemModel);

			ClearUnsufficientCharacters ();

		}
			

		/// <summary>
		/// Clears the unsufficient characters.
		/// </summary>
		private void ClearUnsufficientCharacters(){
			for(int i = 0; i<charactersEnteredArray.Length;i++){
				charactersEnteredArray [i] = 0;
			}
		}

		/// <summary>
		/// 拼写界面输入字母时响应方法
		/// </summary>
		/// <param name="character">输入的字母 character==null代表输入退格键</param>
		public void EnterCharacter(string charEntered){

			if (charactersEntered.Length >= 15) {
				return;
			}

			if(charEntered != null){
				
				int characterIndex = (int)charEntered[0] - CommonData.aInASCII;

				charactersEntered.Append (charEntered);

				charactersEnteredArray [characterIndex]++;
			}

			for (int i = 0; i < charactersInsufficientArray.Length; i++) {
				charactersInsufficientArray [i] = 0;
			}

			for (int i = 0; i < charactersEntered.Length; i++) {

				int index = (int)charactersEntered[i] - CommonData.aInASCII;

				// 将字母碎片不足的字母记录到不足字母列表中
				if (Player.mainPlayer.charactersCount [index] <= charactersEnteredArray[index]) {

					charactersInsufficientArray [index] = 1;

				}

			}

			// 更新拼写界面已输入字母界面ui
			spellView.UpdateCharactersEntered(charactersEntered.ToString(),charactersInsufficientArray);


		}


		public void OnCharacterButtonDown(int index){
			spellView.ShowCharacterTintHUD (index);
		}

		public void OnCharacterButtonUp(int index){
			spellView.HideCharacterTintHUD (index);
		}

		public void OnShowCharactersInBagButtonClick(){
			spellView.ShowAllCharactersInBagPlane ();
		}

		public void QuitCharactersInBagPlane(){
			spellView.QuitAllCharactersInBagPlane ();
		}


		/// <summary>
		///  delete按钮点击响应方法
		/// </summary>
		public void Backspace(){

			if (charactersEntered.Length > 0) {
				
				// 记录删除的字母对应在字母表中的位置
				int removedCharacterIndex = (int)charactersEntered[charactersEntered.Length - 1] - CommonData.aInASCII;

				// 删除已输入字母数组的最后一位
				charactersEntered.Remove (charactersEntered.Length - 1, 1);

				charactersEnteredArray[removedCharacterIndex]--;

				if (charactersEnteredArray [removedCharacterIndex] < 0) {
					throw new System.Exception ("字母删除越界");
				}

				EnterCharacter (null);
			}
		}


		/// <summary>
		/// 制造一个物品
		/// </summary>
		public void CreateOne(){

			int createCount = 1;

			if (!CheckCharactersSufficient(createCount)) {
				return;
			}

			bool isValidWord = CheckEnteredWord ();

			if (!isValidWord) {
				string tint = "拼写不正确或物品未解锁";
				spellView.SetUpTintHUD (tint);
				return;
			}



			CreateItem (itemToCreate,createCount);

		}

		/// <summary>
		/// 选择制造多个物品
		/// </summary>
		public void SelectCreateCount(){

			bool isValidWord = CheckEnteredWord ();

			if (!isValidWord) {
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
				

			CreateItem (itemToCreate,createCount);

			QuitCreateCountHUD ();

		
		}

		/// <summary>
		/// 如果从图鉴系统进入，检查输入的单词是否和对应物品一致
		/// 检查拼写的单词是否存在
		/// </summary>
		/// return 返回对应的物品，不一致或不存在返回null，其余返回对应的item
		private bool CheckEnteredWord(){

			// 从图鉴接口进入，目标物品名称不为空
			if (itemToCreate.itemNameInEnglish != null) {
				
				if (!charactersEntered.ToString ().Equals (itemToCreate.itemNameInEnglish)) {
					Debug.Log ("请输入正确的单词");
					return false;
				} 

			}

			ItemModel item = GameManager.Instance.gameDataCenter.allItemModels.Find (delegate(ItemModel obj) {
				return obj.itemNameInEnglish == charactersEntered.ToString ();
			});

			if (item == null || !Player.mainPlayer.CheckItemUnlocked(item.itemId)) {
				return false;
			}

			return true;

		}
			
		/// <summary>
		/// 检查玩家字母碎片数量是否足够
		/// </summary>
		/// 数量足够返回true，不足返回false
		private bool CheckCharactersSufficient(int count){

			bool sufficient = true;

			StringBuilder unsufficientCharactesStr= new StringBuilder ();

			for (int i = 0; i < charactersEnteredArray.Length; i++) {

				int numNeed = charactersEnteredArray [i] * count;

				if (Player.mainPlayer.charactersCount [i] < numNeed) {

					string unsufficientCharacter = ((char)(i + CommonData.aInASCII)).ToString ();
					unsufficientCharactesStr.AppendFormat ("字母碎片{0}缺少{1}个、",unsufficientCharacter,numNeed - Player.mainPlayer.charactersCount[i]);

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
		private void CreateItem(ItemModel itemModel,int createCount){

			// 生成物品
			itemCreated = Item.NewItemWith (itemModel.itemId, createCount);

			// 更新剩余字母碎片
			UpdateOwnedCharacters ();

			// 初始化制造的物品列表界面
			spellView.SetUpCreatedItemDetailHUD (itemCreated);

			spellView.UpdateCharactersPlane ();

			// 清除制造信息
			ClearSpellInfos ();

		}
			

		/// <summary>
		/// 清除本次制造信息
		/// </summary>
		public void ClearSpellInfos(){

			charactersEntered = new StringBuilder ();

			for (int i = 0; i < charactersEnteredArray.Length; i++) {

				charactersEnteredArray [i] = 0;

			}

			createCount = 1;

		}


		// 数量加减按钮点击响应
		public void CreateCountPlus(int plus){
			
			int targetCount = createCount + plus;
			// 最大或最小值直接返回
			if (targetCount > maxCreateCount || targetCount <minCreateCount) {
				return;
			}

			spellView.UpdateCreateCountHUD (targetCount);

			createCount = targetCount;
		
		}

		/// <summary>
		/// 选择数量的slider拖动时响应方法
		/// </summary>
		public void CreateSliderDrag(){

			createCount = (int)spellView.countSlider.value;

			spellView.UpdateCreateCountHUD (createCount);

		}
			
			
		public void QuitCreateCountHUD(){

			createCount = 1;

			spellView.QuitSpellCountHUD ();

		}

		// 制造出新物品后更新玩家剩余字母碎片
		private void UpdateOwnedCharacters(){
			
			for (int i = 0; i < charactersEntered.Length; i++) {
				int characterIndex = (int)charactersEntered [i] - CommonData.aInASCII;
				Player.mainPlayer.charactersCount [characterIndex] -= 1;
			}
		}

		public void AddItemToBag(){

			// 更新玩家物品数据
			Player.mainPlayer.AddItem (itemCreated);

			spellView.QuitCreateDetailHUD ();

			string tint = string.Format ("获得{0} x {1}", itemCreated.itemName, itemCreated.itemCount);

			spellView.SetUpTintHUD (tint);

			itemCreated = null;

		}

		public void OnUnlockedItemsButtonClick(){

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.unlockedItemsCanvasBundleName, "UnlockedItemsCanvas", () => {

				TransformManager.FindTransform("UnlockedItemsCanvas").GetComponent<UnlockedItemsViewController>().SetUpUnlockedItemsView();

			},false,true);

		}


		/// <summary>
		/// Quits the spell plane.
		/// </summary>
		public void QuitSpellView(){

			spellView.OnQuitSpellPlane ();

			Transform exploreCanvas = TransformManager.FindTransform ("ExploreCanvas");

			if (exploreCanvas != null) {
				exploreCanvas.GetComponent<BattlePlayerUIController> ().SetUpExplorePlayerView (Player.mainPlayer);
			}

			GameManager.Instance.UIManager.HideCanvas ("SpellCanvas");

			ClearSpellInfos ();

		}
			

		/// <summary>
		/// 退出拼写界面时清除内存
		/// </summary>
		public void DestroyInstances(){

			GameManager.Instance.UIManager.DestroryCanvasWith (CommonData.spellCanvasBundleName, "SpellCanvas", null, null);

		}

	}
}
