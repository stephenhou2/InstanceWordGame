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

		// 制造还是修复
		private SpellPurpose spellPurpose;

		// 容器，记录所有输入的字母及数量
		private int[] charactersEnteredArray = new int[26];

		// 最小制造数量
		private int minCreateCount = 1;

		// 指定拼写的单词（材料图鉴进入或者修复装备进入时不为null）
		private string spell;

		// 目标物品（从图鉴系统中选择的物品）
		private Material materialToCreate;

		// 制造出的材料
		private Material materialCreated;

		// 想要修复的装备
		private Equipment equipmentToFix;

		// 根据玩家已有字母碎片数量计算出得目标物品最大制造数
		private int maxCreateCount{

			get{
				int myMaxCreateCount = int.MaxValue;

				for(int i = 0;i<charactersEntered.Length;i++){

					char character = charactersEntered [i];

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


		/// <summary>
		/// 初始化拼写界面（制造）
		/// </summary>
		public void SetUpSpellViewForCreate(Material material){

			if (material != null) {
				this.spell = material.spell;
				spellView.SetUpSpellViewWith (material.materialName);
			} else {
				spellView.SetUpSpellViewWith (null);
			}

			this.spellPurpose = SpellPurpose.Create;

			ClearUnsufficientCharacters ();

		}

		/// <summary>
		/// 初始化拼写界面（强化）
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetUpSpellViewForFix(Equipment equipment, Word word){
			
			this.spellPurpose = SpellPurpose.Fix;

			this.spell = word.spell;

			this.equipmentToFix = equipment;

			spellView.SetUpSpellViewWith (word.explaination);

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
		/// <param name="character">输入的字母</param>
		public void EnterCharacter(string charEntered){

			if (charactersEntered.Length < 15) {

				int characterIndex = (int)charEntered[0] - CommonData.aInASCII;

				charactersEntered.Append (charEntered);

				charactersEnteredArray [characterIndex]++;

				StringBuilder charactersWithColor = new StringBuilder();

				for (int i = 0; i < charactersEntered.Length; i++) {

					int index = (int)charactersEntered[i] - CommonData.aInASCII;

					// 字母碎片足够的字母使用绿色，不足的字母使用红色
					if (Player.mainPlayer.charactersCount [index] >= charactersEnteredArray[index]) {
						charactersWithColor.AppendFormat("<color=green>{0}</color>",charactersEntered[i]);
					} else {
						charactersWithColor.AppendFormat("<color=red>{0}</color>",charactersEntered[i]);
					}

				}


					
				// 更新拼写界面已输入字母界面ui
				spellView.UpdateCharactersEntered(charactersWithColor.ToString());

				if (spellPurpose == SpellPurpose.Fix) {

					if (charactersEntered.ToString () != equipmentToFix.itemNameInEnglish) {
						return;
					}

					if (!CheckCharactersSufficient (1)) {
						return;
					}

					StartCoroutine ("FixItem");
				}

			}

		}

		public void CharacterButtonDown(int buttonIndex){
			spellView.ShowCharacterTintHUD (buttonIndex);
		}

		public void CharacterButtonUp(int buttonIndex){
			spellView.HideCharacterTintHUD (buttonIndex);
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


				// 如果删除的字母在不足数组中，对应字母的不足数－1
	//			if (unsufficientCharacters [removedCharacterIndex] > 0) {
	//				unsufficientCharacters [removedCharacterIndex]--;
	//			}

				charactersEnteredArray[removedCharacterIndex]--;


				if (charactersEnteredArray [removedCharacterIndex] < 0) {
					throw new System.Exception ("字母删除越界");
				}

				StringBuilder charactersWithColor = new StringBuilder();

				for (int i = 0; i < charactersEntered.Length; i++) {

					int index = (int)charactersEntered[i] - CommonData.aInASCII;

					// 字母碎片足够的字母使用绿色，不足的字母使用红色
					if (Player.mainPlayer.charactersCount [index] >= charactersEnteredArray[index]) {
						charactersWithColor.AppendFormat("<color=green>{0}</color>",charactersEntered[i]);
					} else {
						charactersWithColor.AppendFormat("<color=red>{0}</color>",charactersEntered[i]);
					}

				}


				// 更新拼写界面已输入字母界面ui
				spellView.UpdateCharactersEntered(charactersWithColor.ToString());

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

			materialToCreate = CheckEnteredWord ();

			if (materialToCreate == null) {
				return;
			}

			CreateMaterial (materialToCreate,createCount);

		}

		/// <summary>
		/// 选择制造多个物品
		/// </summary>
		public void SelectCreateCount(){

			materialToCreate = CheckEnteredWord ();

			if (materialToCreate == null) {
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

			CreateMaterial (materialToCreate,createCount);
		
		}

		/// <summary>
		/// 如果从图鉴系统进入，检查输入的单词是否和对应物品一致
		/// 检查拼写的单词是否存在
		/// </summary>
		/// return 返回对应的物品，不一致或不存在返回null，其余返回对应的item
		private Material CheckEnteredWord(){

			// 从图鉴接口进入，目标物品名称不为空
			if (spell != null) {
				
				if (!charactersEntered.ToString ().Equals (spell)) {
					Debug.Log ("请输入正确的单词");
					return null;
				} 

			}

			Material material = GameManager.Instance.dataCenter.allMaterials.Find (delegate(Material obj) {
				
				return obj.spell == charactersEntered.ToString ();

			});

			if (material == null) {
				Debug.Log("没有这种材料");
			}

			return material;

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
		private void CreateMaterial(Material material,int createCount){

			// 生成材料
			materialCreated = new Material (material, createCount);

			// 更新玩家材料数据
			Player.mainPlayer.AddMaterial (materialCreated);

			// 更新剩余字母碎片
			UpdateOwnedCharacters ();

			// 初始化制造的物品列表界面
			spellView.SetUpCreateMaterialDetailHUD (materialCreated);

			// 清除制造信息
			ClearSpellInfos ();

//			// 如果制造的物品不是消耗品，则每个物品都不一样，根据制造数量单独生成每一个物品
//			else {
//
//				for (int i = 0; i < createCount; i++) {
//
//					Item newItem = null;
//					switch(itemModel.itemType){
//					case ItemType.Equipment:
//						newItem = new Equipment (itemModel);
//						break;
//					case ItemType.Inscription:
//						newItem = new Inscription (itemModel);
//						break;
//					case ItemType.Task:
//						newItem = new TaskItem (itemModel);
//						break;
//
//					}
//
//					newItem.itemCount = 1;
//
//					Player.mainPlayer.allItems.Add (newItem);
//
//					createdItems.Add (newItem);
//
//				}

//				UpdateOwnedCharacters ();
//
//				spellView.SetUpCreateItemDetailHUD (createdItems);
//
//				ClearSpellInfos ();
//
//			}

		}

		/// <summary>
		/// Strengthens the item.
		/// </summary>
		/// <returns>The item.</returns>
		private IEnumerator FixItem(){

			yield return new WaitForSeconds (0.05f);

			spellView.SetUpFixedItemDetailHUD (equipmentToFix);

		}

		/// <summary>
		/// 玩家点击强化按钮的响应方法
		/// </summary>
		public void ConfirmFixItem(){

			// 检查字母碎片是否足够1次强化
			if (CheckCharactersSufficient (1)) {

				// 修复1次
				equipmentToFix.FixEquipment();

				// 更新玩家字母碎片数量
				UpdateOwnedCharacters ();

				// 更新UI
				spellView.UpdateFixedItemDetailHUD (equipmentToFix);
			}
		}

		/// <summary>
		/// 清除本次制造信息
		/// </summary>
		private void ClearSpellInfos(){

			charactersEntered = new StringBuilder ();

			for (int i = 0; i < charactersEnteredArray.Length; i++) {

				charactersEnteredArray [i] = 0;

			}

			materialCreated = null;

			createCount = 1;

			equipmentToFix = null;
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
			
			for (int i = 0; i < charactersEntered.Length; i++) {
				int characterIndex = (int)charactersEntered [i] - CommonData.aInASCII;
				Player.mainPlayer.charactersCount [characterIndex] -= createCount;
			}
		}

		/// <summary>
		/// 退出制造出的物品描述界面
		/// </summary>
		public void QuitCreateDetailHUD(){

			spellView.OnQuitCreateDetailHUD ();
		}


		/// <summary>
		/// 退出强化物品描述界面
		/// </summary>
		public void QuitStrengthenItemDetailHUD(){
			ClearSpellInfos ();
			spellView.QuitFixedItemDetailHUD();
			QuitSpellPlane ();
		}

		/// <summary>
		/// Quits the spell plane.
		/// </summary>
		public void QuitSpellPlane(){
			
			spellView.OnQuitSpellPlane ();


			if (spell == null) {

				// 如果从制造接口的任意制造接口进入
//				GameObject produceCanvas = GameObject.Find (CommonData.instanceContainerName + "/ProduceCanvas");
//				produceCanvas.GetComponent<ItemDisplayViewController> ().QuitItemDisplayView ();

				GameObject homeCanvas = GameObject.Find (CommonData.instanceContainerName + "/HomeCanvas");
				homeCanvas.GetComponent<HomeViewController> ().SetUpHomeView ();
				DestroyInstances ();

			} else {
				switch (spellPurpose) {

				case SpellPurpose.Create:
					GameObject produceCanvas = GameObject.Find (CommonData.instanceContainerName + "/ProduceCanvas");
					produceCanvas.GetComponent<Canvas> ().enabled = true;
					break;
				case SpellPurpose.Fix:
					GameObject bagCanvas = GameObject.Find (CommonData.instanceContainerName + "/BagCanvas");
					bagCanvas.GetComponent<BagViewController> ().SetUpBagView ();
					break;
				default:
					break;
				}

				DestroyInstances ();
			}


		}

		/// <summary>
		/// 退出拼写界面时清除内存
		/// </summary>
		private void DestroyInstances(){
			
			TransformManager.DestroyTransform (gameObject.transform);
//			TransformManager.DestroyTransfromWithName ("FixGainTextModel", TransformRoot.InstanceContainer);
//			TransformManager.DestroyTransfromWithName ("FixGainTextPool", TransformRoot.PoolContainer);

			Resources.UnloadUnusedAssets ();

			System.GC.Collect ();

		}

	}
}
