﻿using System.Collections;
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

		private int[] charactersInsufficientArray = new int[26];

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
		public void SetUpSpellViewForCreateMaterial(Material material){

			if (material != null) {
				this.spell = material.itemNameInEnglish;
				spellView.SetUpSpellViewWith (material.itemName,SpellPurpose.CreateMaterial);
			} else {
				spellView.SetUpSpellViewWith (null,SpellPurpose.CreateMaterial);
			}

			this.spellPurpose = SpellPurpose.CreateMaterial;

			ClearUnsufficientCharacters ();

		}

		public void SetUpSpellViewForCreateFuseStone(){

			this.spellPurpose = SpellPurpose.CreateFuseStone;

			spellView.SetUpSpellViewWith (null,SpellPurpose.CreateFuseStone);

		}

		/// <summary>
		/// 初始化拼写界面（强化）
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetUpSpellViewForFix(Equipment equipment, Word word){
			
			this.spellPurpose = SpellPurpose.Fix;

			this.spell = word.spell;

			this.equipmentToFix = equipment;

			spellView.SetUpSpellViewWith (word.explaination,SpellPurpose.Fix);

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
				if (Player.mainPlayer.charactersCount [index] < charactersEnteredArray[index]) {

					charactersInsufficientArray [index] = 1;

				}

			}

			// 更新拼写界面已输入字母界面ui
			spellView.UpdateCharactersEntered(charactersEntered.ToString(),charactersInsufficientArray);

//			if (spellPurpose == SpellPurpose.Fix) {
//
//				if (charactersEntered.ToString () != equipmentToFix.itemNameInEnglish) {
//					return;
//				}
//
//				if (!CheckCharactersSufficient (1)) {
//					return;
//				}
//
//				StartCoroutine ("FixItem");
//			}

		}


		public void OnCharacterButtonDown(int index){
			spellView.ShowCharacterTintHUD (index);
		}

		public void OnCharacterButtonUp(int index){
			spellView.HideCharacterTintHUD (index);
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
				
				return obj.itemNameInEnglish == charactersEntered.ToString ();

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

			spellView.UpdateCharactersPlane ();

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
		private void FixItem(){

			// 修复1次
			equipmentToFix.FixEquipment();

			// 更新玩家字母碎片数量
			UpdateOwnedCharacters ();

			// 更新UI
//			spellView.UpdateFixedItemDetailHUD (equipmentToFix);

			spellView.SetUpFixedItemDetailHUD (equipmentToFix);

		}

		/// <summary>
		/// 玩家点击强化按钮的响应方法
		/// </summary>
//		public void ConfirmFixItem(){
//
//			// 检查字母碎片是否足够1次强化
//			if (CheckCharactersSufficient (1)) {
//
//
//			}
//		}

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


		private bool CheckSpellCorrect(){
			return charactersEntered.ToString () == spell;
		}

		public void OnConfirmButtonClick(){

			switch (spellPurpose) {
			case SpellPurpose.CreateFuseStone:
				FuseStone fuseStone = FuseStone.CreateFuseStoneIfExist (charactersEntered.ToString ());
				if (fuseStone != null) {
					Player.mainPlayer.allFuseStonesInBag.Add (fuseStone);
				
					// 更新剩余字母碎片
					UpdateOwnedCharacters ();

					// 初始化制造的物品列表界面
					spellView.SetUpCreateMaterialDetailHUD (fuseStone);

					spellView.UpdateCharactersPlane ();

					// 清除制造信息
					ClearSpellInfos ();

				}
				break;
			case SpellPurpose.Fix:
				if (CheckSpellCorrect ()) {
					FixItem ();
				}
				break;


			}


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
		public void QuitFixItemDetailHUD(){
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
				case SpellPurpose.CreateMaterial:
					GameObject produceCanvas = GameObject.Find (CommonData.instanceContainerName + "/ProduceCanvas");
					if (produceCanvas != null) {
						produceCanvas.GetComponent<Canvas> ().enabled = true;
					}
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
