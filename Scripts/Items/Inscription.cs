﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System.Text;


namespace WordJourney
{
	
	public class Inscription : Item {

		public int attackGain;//攻击力增益
		public int attackSpeedGain;//攻速增益
		public int critGain;//暴击增益
		public int armourGain;//护甲增益
		public int manaResistGain;//魔抗增益
		public int dodgeGain;//闪避增益

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="itemModel">Item model.</param>
		public Inscription(ItemModel itemModel){

			this.itemType = ItemType.Inscription;


			/************从单词数据库中获取物品名称**************/
			MySQLiteHelper sql = MySQLiteHelper.Instance;

			sql.GetConnectionWith (CommonData.dataBaseName);

			IDataReader reader =  sql.ReadSpecificRowsAndColsOfTable ("AllWordsData", "*",
				new string[]{ string.Format ("Spell='{0}'", itemModel.itemNameInEnglish)},
				true);

			if (!reader.Read ()) {
				Debug.Log ("不存在");
				return;
			}

			bool valid = reader.GetBoolean (4);

			if (!valid) {
				Debug.Log ("已使用");
				return;
			}

			int id = reader.GetInt32 (0);

			string explaination = reader.GetString (2);

			string inscriptionName = explaination.Split (new string[]{ ".", "，" }, System.StringSplitOptions.RemoveEmptyEntries)[1];

			inscriptionName = inscriptionName.Replace ("的", string.Empty);

			this.itemName = string.Format ("{0}之石", inscriptionName);

			this.RandomProperties ();

			sql.UpdateSpecificColsWithValues ("AllWordsData",
				new string[]{ "Valid" },
				new string[]{ "0" },
				new string[]{ string.Format("Id={0}",id) },
				true);

			sql.CloseConnection (CommonData.dataBaseName);
		}

		/// <summary>
		/// 获取物品属性字符串
		/// </summary>
		/// <returns>The item properties string.</returns>
		public override string GetItemPropertiesString(){

			StringBuilder itemProperties = new StringBuilder ();

			List<string> propertiesList = new List<string> ();

			if (attackGain > 0) {
				string str = string.Format ("攻击: {0}", attackGain);
				propertiesList.Add (str);
			}
			if (attackSpeedGain > 0) {
				string str = string.Format ("攻速: {0}", attackSpeedGain);
				propertiesList.Add (str);
			}
			if (critGain > 0) {
				string str = string.Format ("暴击: {0}", critGain);
				propertiesList.Add (str);
			}
			if (armourGain > 0) {
				string str = string.Format ("护甲: {0}", armourGain);
				propertiesList.Add (str);
			}
			if (manaResistGain > 0) {
				string str = string.Format ("抗性: {0}", manaResistGain);
				propertiesList.Add (str);
			}
			if (dodgeGain > 0) {
				string str = string.Format ("闪避: {0}", dodgeGain);
				propertiesList.Add (str);
			} 
//			if (healthGain > 0) {
//				string str = string.Format ("体力+{0}",healthGain);
//				propertiesList.Add (str);
//			}
//			if (manaGain > 0) {
//				string str = string.Format ("魔法+{0}",manaGain);
//				propertiesList.Add (str);
//			}

			if (propertiesList.Count > 0) {
				itemProperties.Append (propertiesList [0]);

				for (int i = 1; i < propertiesList.Count; i++) {

					itemProperties.AppendFormat ("\n{0}", propertiesList [i]);

				}

			}

			return itemProperties.ToString ();

		}

		/// <summary>
		/// 铭文随机获得值为1～9的两个属性
		/// </summary>
		protected void RandomProperties(){

			int seed1 = Random.Range (0, 6);
			int seed2 = 0;
			do {
				seed2 = Random.Range (0, 6);
			} while(seed2 == seed1);

			foreach (int seed in new int[]{seed1,seed2}) {
				switch (seed) {
				case 0:
					attackGain = Random.Range (1, 10);
					break;
				case 1:
					attackSpeedGain = Random.Range (1, 10);
					break;
				case 2:
					armourGain = Random.Range (1, 10);
					break;
				case 3:
					manaResistGain = Random.Range (1, 10);
					break;
				case 4:
					critGain = Random.Range (1, 10);
					break;
				case 5:
					dodgeGain = Random.Range (1, 10);
					break;
//				case 6:
//					healthGain = Random.Range (10, 100);
//					break;
//				case 7:
//					manaGain = Random.Range (1, 5);
//					break;
				}
			}
		}


		/// <summary>
		/// 获取物品类型字符串
		/// </summary>
		/// <returns>The item type string.</returns>
		public override string GetItemTypeString ()
		{
			return "类型: 铭文";
		}

		/// <summary>
		/// 获取物品品质字符串
		/// </summary>
		/// <returns>The item quality string.</returns>
		public override string GetItemQualityString ()
		{
			return string.Empty;
		}

	}
}