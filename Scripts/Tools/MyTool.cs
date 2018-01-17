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

	}
}
