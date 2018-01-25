using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;
	using UnityEngine.EventSystems;

	public class EquipedItemDropControl : ItemDropControl {


		private BagView mBagView;
		private BagView bagView{
			get{
				if (mBagView == null) {
					mBagView = TransformManager.FindInParents<BagView> (this.gameObject);
				}
				return mBagView;
			}
		}

		public int equipmentIndexInPanel;

		protected override void OnUserPointerEnter (PointerEventData eventData)
		{

			if (!eventData.dragging) {
				return;
			}

			ItemDragControl dragControl = eventData.pointerDrag.GetComponent<ItemDragControl> ();

			Item draggedItem = GetDraggedItem (eventData);

			if (dragControl == null || draggedItem == null || draggedItem.itemId < 0) {
				return;
			}

			dragControl.detectReceiver = true;

			bool unlocked = BuyRecord.Instance.equipmentSlotUnlockedArray [equipmentIndexInPanel];

			if (draggedItem.itemType == ItemType.Equipment && unlocked) {
				tintImage.enabled = true;
				tintImage.color = new Color (0f, 1f, 0f, 0.2f);
			} else {
				tintImage.enabled = true;
				tintImage.color = new Color (1f, 0f, 0f, 0.2f);
			}
		}

		protected override void OnUserPointerExit (PointerEventData eventData){


			if (!eventData.dragging) {
				tintImage.enabled = false;
				return;
			}

			ItemDragControl dragControl = eventData.pointerDrag.GetComponent<ItemDragControl> ();

//			Item draggedItem = GetDraggedItem (eventData);

			if (dragControl == null) {
				tintImage.enabled = false;
				return;
			}

			dragControl.detectReceiver = false;

			tintImage.enabled = false;
		}



		protected override void OnUserDrop (PointerEventData eventData)
		{
			// 获取拖拽中的物品
			Item itemInBag = GetDraggedItem (eventData);

			bool unlocked = BuyRecord.Instance.equipmentSlotUnlockedArray [equipmentIndexInPanel];

			// 没有拖拽中的物品或者该装备栏没有解锁或者物品类型不是装备H直接返回
			if (itemInBag == null || !unlocked || itemInBag.itemType != ItemType.Equipment) {
				SetDropResult (eventData, false);
				tintImage.enabled = false;
				return;
			}

			GameObject draggedObject = GetDraggedObject (eventData);

			// 准备换下的装备
			Equipment equipmentPrepareToUnload = GetComponent<ItemDragControl>().item as Equipment;

			// 准备装上的装备
			Equipment equipmentPrepareToLoad = itemInBag as Equipment;


			// 如果是从装备栏中拖拽出来的物品
			if (draggedObject.GetComponent<EquipedItemDragControl>() != null) {
//				// 装备标记为拖拽过来的物品的装备标记
//				equipmentPrepareToUnload.equiped = true;
				// 装上背包中拖拽过来的装备标记为已装备
				equipmentPrepareToLoad.equiped = true;

				int unloadEquipmentIndex = equipmentIndexInPanel;

				int loadEquipmentIndex = GetEquipmentIndexInPanel (equipmentPrepareToLoad);

				Debug.LogFormat ("装备互换{0}-{1}", unloadEquipmentIndex, loadEquipmentIndex);

				// 互换装备位置（空的格子原来使用空装备占位，也应该进行移动）
				Player.mainPlayer.allEquipedEquipments[unloadEquipmentIndex] = equipmentPrepareToLoad;
				Player.mainPlayer.allEquipedEquipments[loadEquipmentIndex] = equipmentPrepareToUnload;

				// 对应格子中的装备数据更换
				GetComponent<EquipedItemDragControl>().item = equipmentPrepareToLoad;
				GetDraggedObject (eventData).GetComponent<EquipedItemDragControl> ().item = equipmentPrepareToUnload;

				bagView.SetUpEquipedEquipmentsPlane ();

				SetDropResult (eventData, true);

				tintImage.enabled = false;
				return;
			}

			// 如果是从背包中拖拽出来的物品
			if (draggedObject.GetComponent<ItemInBagDragControl>() != null) {

				int equipmentIndexInPanel = GetEquipmentIndexInPanel (equipmentPrepareToUnload);

				Agent.PropertyChange propertyChangeFromUnload = new Agent.PropertyChange();

				int oriItemIndexInBag = Player.mainPlayer.GetItemIndexInBag (equipmentPrepareToLoad);

				Debug.Log (oriItemIndexInBag);

				if (oriItemIndexInBag == -1) {
					Debug.LogError("背包中没有找到该物品");
				}

				if (equipmentPrepareToUnload.itemId >= 0) {
					// 该装备移入背包中
					propertyChangeFromUnload = Player.mainPlayer.UnloadEquipment (equipmentPrepareToUnload, equipmentIndexInPanel,oriItemIndexInBag);

					Debug.Log (equipmentPrepareToUnload.itemName + "UNLOAD");

					bagView.AddBagItem (equipmentPrepareToUnload, oriItemIndexInBag,true);

				}
					
				// 背包中的装备移入已装备列表
				Agent.PropertyChange propertyChangeFromLoad = Player.mainPlayer.EquipEquipment(equipmentPrepareToLoad,equipmentIndexInPanel);

				Agent.PropertyChange propertyChange = Agent.PropertyChange.MergeTwoPropertyChange (propertyChangeFromLoad,propertyChangeFromUnload);

				bagView.SetUpEquipedEquipmentsPlane ();

				bagView.SetUpPlayerStatusPlane (propertyChange);

				// 对应格子中的装备数据更换
				GetComponent<EquipedItemDragControl>().item = equipmentPrepareToLoad;

				Player.mainPlayer.ResetBattleAgentProperties (false);

				bagView.RemoveBagItemAt (oriItemIndexInBag + (equipmentPrepareToUnload.itemId >= 0 ? 1 : 0));

				SetDropResult (eventData, true);

				tintImage.enabled = false;

				return;

			}

			tintImage.enabled = false;

		}



		/// <summary>
		/// 获取装备在人物装备栏内的序号，如果不在人物装备栏内，返回-1
		/// </summary>
		private int GetEquipmentIndexInPanel(Equipment equipment){
			for (int i = 0; i < Player.mainPlayer.allEquipedEquipments.Length; i++) {
				if (equipment == Player.mainPlayer.allEquipedEquipments [i]) {
					return i;
				}
			}
			return - 1;
		}



	}
}
