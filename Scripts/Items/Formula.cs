using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public enum FormulaType{
		Equipment,
		Skill
	}

	/*************attention****************

	itemId：2000-3000代表物品配方，3000-3100代表技能卷轴

	*************attention****************/
	public class Formula:Item {

		public FormulaType formulaType;

		public int itemOrSkillId;

		public Formula(FormulaType formulaType,int itemOrSkillId){

			this.itemType = ItemType.Formula;

			this.formulaType = formulaType;

			this.itemOrSkillId = itemOrSkillId;

			#warning 配方图片只有配方和卷轴两种，直接在构造函数里赋值
			this.spriteName = "";

			switch (formulaType) {
			case FormulaType.Equipment:
				
				ItemModel itemModel = GameManager.Instance.gameDataCenter.allItemModels.Find (delegate (ItemModel obj) {
					return obj.itemId == itemOrSkillId;
				});

				this.itemName = itemModel.itemName;
					
				this.itemId = itemModel.itemId + 2000;

				this.itemDescription = itemModel.itemDescription;

				GetItemModelUnlock (itemModel);

				break;
			case FormulaType.Skill:
				
				Skill skill = GameManager.Instance.gameDataCenter.allSkills.Find (delegate(Skill obj) {
					return obj.skillId == itemOrSkillId;
				});

				this.itemName = skill.skillName;

				this.itemId = skill.skillId + 3000;

				this.itemDescription = skill.skillDescription;

				GetSkillUnlock (skill);

				break;
			}
		}



		/// <summary>
		/// 解锁该配方对应的物品，并返回该物品信息
		/// </summary>
		/// <returns>The item model unlock.</returns>
		public ItemModel GetItemModelUnlock(){

			ItemModel itemModel = GameManager.Instance.gameDataCenter.allItemModels.Find (delegate (ItemModel obj) {
				return obj.itemId == itemOrSkillId;
			});

			GetItemModelUnlock (itemModel);

			return itemModel;
		}

		private void GetItemModelUnlock(ItemModel itemModel){

			// 如果背包中已经有这种配方，则不添加到背包中
			for (int i = 0; i < Player.mainPlayer.allFormulasInBag.Count; i++) {
				Formula formulaInBag = Player.mainPlayer.allFormulasInBag [i];
				if (formulaInBag.formulaType == FormulaType.Equipment && formulaInBag.itemOrSkillId == itemOrSkillId) {
					return;
				}
			}

			// 如果背包中没有这种配方，则解锁该配方，并把配方放进背包中
			itemModel.unlocked = true;

			Player.mainPlayer.allFormulasInBag.Add (this);

		}

		/// <summary>
		/// 解锁该技能卷轴对应的技能，并返回技能信息
		/// </summary>
		/// <returns>The skill unlock.</returns>
		public Skill GetSkillUnlock(){

			Skill skill = GameManager.Instance.gameDataCenter.allSkills.Find (delegate(Skill obj) {
				return obj.skillId == itemOrSkillId;
			});

			GetSkillUnlock (skill);

			return skill;
		}

		private void GetSkillUnlock(Skill skill){
			// 如果背包中已经有这种技能卷轴，则不添加到背包中
			for (int i = 0; i < Player.mainPlayer.allFormulasInBag.Count; i++) {
				Formula formulaInBag = Player.mainPlayer.allFormulasInBag [i];
				if (formulaInBag.formulaType == FormulaType.Skill && formulaInBag.itemOrSkillId == itemOrSkillId) {
					return;
				}
			}

			// 如果背包中没有这种技能卷轴，则解锁该技能卷轴，并把配方放进背包中
			skill.unlocked = true;

			Player.mainPlayer.allFormulasInBag.Add (this);
		}


		public override string GetItemBasePropertiesString ()
		{
			return itemDescription;
		}

		public override string GetItemTypeString ()
		{
			string formulaTypeString = string.Empty;
			switch (formulaType) {
			case FormulaType.Equipment:
				formulaTypeString = "配方";
				break;
			case FormulaType.Skill:
				formulaTypeString = "卷轴";
				break;
			}
			return formulaTypeString;
		}

		public override string ToString ()
		{
			return string.Format ("[Formula]");
		}
	}
}
