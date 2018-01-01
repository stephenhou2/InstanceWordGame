using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	using System.IO;
	using System;
	using UnityEditor;
	using System.Text;

	public class NewItemHelper  {

		[MenuItem("EditHelper/NewItemHelper")]
		public static void LoadNewItemDataFromLocalFile(){

			string newItemDataPath = "/Users/houlianghong/Desktop/MyGameData/物品设计.csv";

			string fullItemDataString = DataHandler.LoadDataString (newItemDataPath);

			NewItemDataLoader loader = new NewItemDataLoader ();

			loader.LoadAllItemsWithFullDataString (fullItemDataString);

			for (int i = 0; i < loader.newItemModels.Count; i++) {

				ItemModel im = loader.newItemModels [i];

				Debug.Log (im);

			}

			string newItemModelsDataPath = CommonData.originDataPath + "/NewItemDatas.json";

			DataHandler.SaveInstanceListToFile<ItemModel> (loader.newItemModels,newItemModelsDataPath);

		}
	
	}

	public class TempItemModel{
		public ItemModel itemModel;
		public List<string> itemNamesForProduce;

		public TempItemModel(){
			itemModel = new ItemModel ();
			itemNamesForProduce = new List<string> ();
		}
	}

	public class NewItemDataLoader{

		private List<TempItemModel> tempItemModels = new List<TempItemModel>();

		public List<ItemModel> newItemModels= new List<ItemModel>();


		public void LoadAllItemsWithFullDataString(string fullItemDataString){

			string[] seperateItemDataArray = fullItemDataString.Split (new string[]{ "\n" }, StringSplitOptions.RemoveEmptyEntries);


			for (int i = 2; i < seperateItemDataArray.Length; i++) {

				if (i == 60) {
					Debug.Log ("here");
				}

				string itemDataString = seperateItemDataArray [i].Replace("\r","");

				string[] itemDataArray = itemDataString.Split (new char[]{ ',' }, StringSplitOptions.None);

				int dataLength = itemDataArray.Length;

				TempItemModel tempItemModel = new TempItemModel ();

				tempItemModels.Add (tempItemModel);

				ItemModel im = tempItemModel.itemModel;

				im.itemId = FromStringToInt16 (itemDataArray [0]);

				im.itemName = itemDataArray [1];

				im.itemType = GetItemTypeWithString (itemDataArray [2]);

				im.itemNameInEnglish = itemDataArray [3];

				im.itemDescription = itemDataArray [4];

				im.spriteName = itemDataArray [5];

				im.healthGain = FromStringToSingle (itemDataArray [6]);

				im.manaGain = FromStringToSingle (itemDataArray [7]);

				im.attackGain = FromStringToSingle (itemDataArray [8]);

				im.hitGain = FromStringToSingle (itemDataArray [9]);

				im.armorGain = FromStringToSingle (itemDataArray [10]);

				im.magicResistGain = FromStringToSingle (itemDataArray [11]);

				im.dodgeGain = FromStringToSingle (itemDataArray [12]);

				im.critGain = FromStringToSingle (itemDataArray [13]);

				im.wholePropertyGain = FromStringToSingle (itemDataArray [14]);

				im.physicalHurtScalerGain = FromStringToSingle (itemDataArray [15]);

				im.magicalHurtScalerGain = FromStringToSingle (itemDataArray [16]);

				im.critHurtScalerGain = FromStringToSingle (itemDataArray [17]);

				int attachedSkillColCount = dataLength - 22;

				if (attachedSkillColCount % 13 != 0) {
					string error = string.Format ("数据长度不正确,总数据长度{0}，装备属性数据长度{1}，附加技能数据长度{2}，配方数据长度{3}", dataLength, 18, attachedSkillColCount, 4);
					Debug.LogError (error);
					return;
				}

				int attachedSkillCount = attachedSkillColCount / 13;

				List<SkillInfo> attachedSkillInfos = new List<SkillInfo> ();

				im.attachedSkillInfos = new SkillInfo[attachedSkillCount];

				for (int j = 0; j < attachedSkillCount; j++) {

					SkillInfo attachedSkillInfo = new SkillInfo ();
					int offset = j * 13;

//					string itemSkillType = itemDataArray [17 + offset];

					if (itemDataArray [18 + offset] == "") {
						continue;
					}

					attachedSkillInfo.skillType = GetSkillTypeWithString (itemDataArray [18 + offset]);
					attachedSkillInfo.hurtType = GetHurtTypeWithString (itemDataArray [19 + offset]);
					attachedSkillInfo.triggerSource = GetTargetWithString (itemDataArray [20 + offset]);
					attachedSkillInfo.triggeredCondition = GetTriggeredConditionWithString (itemDataArray [21 + offset]);
					attachedSkillInfo.triggerTarget = GetTargetWithString (itemDataArray [22 + offset]);
					attachedSkillInfo.selfEffectAnimName = itemDataArray [23 + offset];
					attachedSkillInfo.enemyEffectAnimName = itemDataArray [24 + offset];
					attachedSkillInfo.statusName = itemDataArray [25 + offset];
					attachedSkillInfo.triggeredProbability = FromStringToSingle (itemDataArray [26 + offset]);
					attachedSkillInfo.canOverlay = FromStringToBool (itemDataArray [27 + offset]);
					attachedSkillInfo.skillSourceValue = FromStringToSingle (itemDataArray [28 + offset]);
					attachedSkillInfo.excuteOnce = FromStringToBool (itemDataArray [29 + offset]);
					attachedSkillInfo.duration = FromStringToSingle (itemDataArray [30 + offset]);

					attachedSkillInfos.Add(attachedSkillInfo);

				}

//				im.attachedSkillInfos = new SkillInfo[attachedSkillInfos.Count];
				im.attachedSkillInfos =	attachedSkillInfos.ToArray ();

//				List<int> itemIdsForProduce = new List<int> ();

				for (int j = 4; j > 0; j--) {
					int columnIndex = dataLength - j;
					string itemName = itemDataArray [columnIndex];
					if (itemName != "" && itemName != null) {
						tempItemModel.itemNamesForProduce.Add (itemName);
					}
				}

			}

			InitAllItemNameToItemId ();

			for (int i = 0; i < tempItemModels.Count; i++) {
				newItemModels.Add(tempItemModels[i].itemModel);
			}

		}

		private int FromStringToInt16(string str){
//			Debug.Log (str);
			return str == "" ? 0 : Convert.ToInt16 (str);
		}

		private int FromStringToInt32(string str){
			return str == "" ? 0 : Convert.ToInt32 (str);
		}

		private bool FromStringToBool(string str){
			return str == "" ? false : Convert.ToBoolean (str);
		}

		private float FromStringToSingle(string str){
			return str == "" ? 0 : Convert.ToSingle (str);
		}

		private ItemType GetItemTypeWithString(string str){
			ItemType type = ItemType.Equipment;
			switch (str) {
			case "装备":
				type = ItemType.Equipment;
				break;
			case "消耗品":
				type = ItemType.Consumables;
				break;
			}

			return type;

		}

		private MySkillType GetSkillTypeWithString(string str){

			MySkillType type = MySkillType.None;

			switch (str) {
			case "":
				type = MySkillType.None;
				break;
			case "最大生命":
				type = MySkillType.MaxHealth;
				break;
			case "生命":
				type = MySkillType.Health;
				break;
			case "魔法":
				type = MySkillType.Mana;
				break;
			case "攻击":
				type = MySkillType.Attack;
				break;
			case "攻速":
				type = MySkillType.AttackSpeed;
				break;
			case "护甲":
				type = MySkillType.Armor;
				break;
			case "抗性":
				type = MySkillType.MagicResist;
				break;
			case "闪避":
				type = MySkillType.Dodge;
				break;
			case "暴击":
				type = MySkillType.Crit;
				break;
			case "命中":
				type = MySkillType.Hit;
				break;
			case "物理伤害系数":
				type = MySkillType.PhysicalHurtScaler;
				break;
			case "魔法伤害系数":
				type = MySkillType.MagicalHurtScaler;
				break;
			case "暴击伤害系数":
				type = MySkillType.CritHurtScaler;
				break;
			case "全属性":
				type = MySkillType.WholeProperty;
				break;
			case "吸血":
				type = MySkillType.HealthAbsorb;
				break;
			case "附魔":
				type = MySkillType.AttachMagicalHurt;
				break;
			case "击晕":
				type = MySkillType.Fizzy;
				break;
			case "伤害反弹":
				type = MySkillType.ReflectHurt;
				break;
			}

			return type;

		}		

		private HurtType GetHurtTypeWithString(string str){

			HurtType type = HurtType.None;

			switch (str) {
			case "":
				type = HurtType.None;
				break;
			case "物理":
				type = HurtType.Physical;
				break;
			case "魔法":
				type = HurtType.Magical;
				break;

			}

			return type;

		}

		private SkillEffectTarget GetTargetWithString(string str){
			SkillEffectTarget target = SkillEffectTarget.Self;
			switch (str) {
			case "":
			case "己方":
				target = SkillEffectTarget.Self;
				break;
			case "敌方":
				target = SkillEffectTarget.Enemy;
				break;
			}
			return target;
		}

		private TriggeredCondition GetTriggeredConditionWithString(string str){
			TriggeredCondition condition = TriggeredCondition.None;
			switch (str) {
			case "":
				condition = TriggeredCondition.None;
				break;
			case "进入战斗时":
				condition = TriggeredCondition.BeforeFight;
				break;
			case "进入攻击动作时":
				condition = TriggeredCondition.Attack;
				break;
			case "攻击命中时":
				condition = TriggeredCondition.Hit;
				break;
			case "被攻击时":
				condition = TriggeredCondition.BeAttacked;
				break;
			case "被攻击命中时":
				condition = TriggeredCondition.BeHit;
				break;
			}
			return condition;
		}


		private void InitAllItemNameToItemId(){

			for (int i = 0; i < tempItemModels.Count; i++) {

				TempItemModel temp = tempItemModels [i];

				int itemCountForProduce = temp.itemNamesForProduce.Count;

				temp.itemModel.itemIdsForProduce = new int[itemCountForProduce];

				for (int j = 0; j < itemCountForProduce; j++) {

					string itemName = temp.itemNamesForProduce [j];

					TempItemModel itemForProduce = tempItemModels.Find (delegate(TempItemModel obj) {
						return obj.itemModel.itemName == itemName;
					});

					if (itemForProduce == null) {
						Debug.LogError(string.Format("{0}配方中的{1}名称不正确",temp.itemModel.itemName,itemName));
					}

					temp.itemModel.itemIdsForProduce [j] = itemForProduce.itemModel.itemId;

				}
			}

		}


	}
}
