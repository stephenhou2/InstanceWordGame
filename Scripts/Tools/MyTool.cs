using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class MyTool {

		public static bool ApproximatelySamePosition2D(Vector3 pos1,Vector3 pos2){

			int pos1_x = Mathf.RoundToInt (pos1.x);
			int pos1_y = Mathf.RoundToInt (pos1.y);

			int pos2_x = Mathf.RoundToInt (pos2.x);
			int pos2_y = Mathf.RoundToInt (pos2.y);

			return pos1_x == pos2_x && pos1_y == pos2_y;

		}

		private static string[] chineseNums = new string[] {"一","二","三","四","五","六","七","八","九","十","十一","十二","十三","十四","十五","十六","十七","十八","十九","二十"};


		public static string NumberToChinese(int number){

			if (number == 0 || number > 20) {
				return "";
			}

			return chineseNums [number - 1];

		}

		private static string[] normalMonsterNames = new string[]{
			"01_Pumpkin",
			"02_Snake",
			"03_Turtle",
			"04_Bamboo",
			"05_StoneMan",
			"06_LittleFox",
			"07_RedMouse",
			"08_Spider",
			"09_HornMushroom",
			"10_Monkey",
			"11_Zoombie",
			"12_Slurry",
			"13_BallBat",
			"14_LittleShark",
			"15_RotateMouse",
			"16_Mouth",
			"17_DragonBird",
		};

		private static string[] bossNames = new string[] {
			"50_SkeletonWarrior",
			"51_NeedleMouse",
			"52_LittleDragon",
			"53_EyeGhost",
			"54_NightCat",
			"55_Orc",
			"56_FireFox",
			"57_StrongCow",
			"58_StrongWolf",
			"59_Demon"
		};

		public static string GetMonsterName(int monsterId){
			if (monsterId < 50) {
				return normalMonsterNames [monsterId];
			} else {
				return bossNames [monsterId - 50];
			}
		}


		public static Dictionary<string,string> propertyChangeStrings = new Dictionary<string, string>{
			{"Status_Poison_Durative","中毒"},
			{"Status_Burn_Durative","烧伤"},
			{"Status_DecreaseAttack","攻击降低"},
			{"Status_DecreaseMana","魔法降低"},
			{"Status_DecreaseHit","命中降低"},
			{"Status_DecreaseAttackSpeed","攻速降低"},
			{"Status_DecreaseArmor","护甲降低"},
			{"Status_DecreaseMagicResist","抗性降低"},
			{"Status_DecreaseDodge","闪避提升"},
			{"Status_IncreaseAttack","攻击提升"},
			{"Status_IncreaseMana","魔法提升"},
			{"Status_IncreaseHit","命中提升"},
			{"Status_IncreaseAttackSpeed","攻速提升"},
			{"Status_IncreaseArmor","攻速提升"},
			{"Status_IncreaseMagicResist","攻速提升"},
			{"Status_IncreaseDodge","闪避提升"},
			{"Status_IncreaseCrit","暴击提升"}
		};


	}
}
