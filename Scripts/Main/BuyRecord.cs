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

		public void PurchaseSuccess(string productId){

			if (productId == PurchaseManager.equipmentSlot_5_id) {
				BuyRecord.Instance.equipmentSlotUnlockedArray [4] = true;
				GameManager.Instance.persistDataManager.SaveBuyRecord ();
			}else if(productId == PurchaseManager.equipmentSlot_6_id){
				BuyRecord.Instance.equipmentSlotUnlockedArray [5] = true;
				GameManager.Instance.persistDataManager.SaveBuyRecord ();
			}

		}

	}
}
