using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	[System.Serializable]
	public class BuyRecord{

		private static BuyRecord mInstance;
		public static BuyRecord Instance{
			get{
				if (mInstance == null) {
					mInstance = DataHandler.LoadDataToSingleModelWithPath<BuyRecord> (CommonData.buyRecordFilePath);
				}

				return mInstance;
			}
		}

		public bool[] equipmentSlotUnlockedArray;

		public bool extraBagUnlocked;



	}
}
