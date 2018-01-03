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


	}
}
