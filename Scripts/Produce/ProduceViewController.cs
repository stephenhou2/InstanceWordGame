using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class ProduceViewController : MonoBehaviour {

		private ProduceView produceView;

		private ItemModel itemModel;

		private List<Material> materialsForProduce = new List<Material> ();

		private FuseStone fuseStoneSelected;

		void Awake(){

			produceView = GetComponent<ProduceView> ();
		

		}
			

		public void SetUpProduceView(ItemModel itemModel){

			this.itemModel = itemModel;

			for (int i = 0; i < itemModel.materials.Count; i++) {
				
				Material m = itemModel.materials [i];

				materialsForProduce.Add (new Material (m, 0));
			}

			produceView.SetUpProduceView (itemModel,materialsForProduce);

		}

		public void MaterialCountPlusOne(Material material){
			
			Material materialInBag = Player.mainPlayer.GetMaterialInBagWithId (material.itemId);

			if (material.itemCount >= materialInBag.itemCount) {
				produceView.DisplayNotEnoughText ();
				return;
			}
			material.itemCount += 1;
			produceView.SetUpProduceView (itemModel, materialsForProduce);
			CheckProduceQualification ();
		}

		public void MaterialCountMinusOne(Material material){
			if (material.itemCount <= 1) {
				return;
			}
			material.itemCount -= 1;
			produceView.SetUpProduceView (itemModel, materialsForProduce);
			CheckProduceQualification ();
		}

		private void CheckProduceQualification(){
			
			int totalValence = 0;

			for (int i = 0; i < materialsForProduce.Count; i++) {
				if (materialsForProduce [i].itemCount < 1) {
					return;
				}
			}

			for (int i = 0; i < materialsForProduce.Count; i++) {
				Material material = materialsForProduce [i];
				totalValence += material.valence * material.itemCount;
			}

			if(totalValence == 0){
				produceView.EnableProduce ();
			}
		}


		public void OnAddFuseStoneButtonClick(){
			produceView.SetUpFuseStonesDisplayPlane ();
		}

		public void OnFuseStoneButtonClick(FuseStone fuseStone,Transform fuseStoneTrans){

			fuseStoneSelected = fuseStone;

			produceView.UpdateFuseStonesDisplayPlane (fuseStoneTrans);

		}

		public void OnConfirmFuseStoneButtonClick(){

			produceView.QuitFuseStonesDisplayPlane ();

		}

		private void UseFuseStone(){
			Player.mainPlayer.allFuseStonesInBag.Remove (fuseStoneSelected);
			fuseStoneSelected = null;
		}


		/// <summary>
		/// 制造按钮点击响应
		/// </summary>
		public void OnProduceButtonClick(){

			Item item = null;

			switch (itemModel.itemType) {
			case ItemType.Equipment:
				item = new Equipment (itemModel);
				break;
			case ItemType.Consumables:
				item = new Consumables (itemModel);
				break;
			default:
				break;
			}

			Player.mainPlayer.AddItem (item);

			for(int i = 0;i<materialsForProduce.Count;i++){

				Material material = materialsForProduce [i];
				
				Material materialInBag = Player.mainPlayer.allMaterialsInBag.Find (delegate(Material obj) {
					return obj.itemId == material.itemId;
				});

				materialInBag.itemCount -= material.itemCount;

				if (materialInBag.itemCount <= 0) {
					Player.mainPlayer.allMaterialsInBag.Remove (materialInBag);
				}

			}

			UseFuseStone ();

			produceView.SetUpItemDetailsPlane (item);

		}

		public void QuitItemDetailsPlane(){

			produceView.QuitItemDetailsPlane ();

			for (int i = 0; i < materialsForProduce.Count; i++) {
				materialsForProduce [i].itemCount = 0;
			}

			produceView.SetUpProduceView (itemModel, materialsForProduce);

		}


		public void QuitProduceView(){
			
			produceView.QuitProduceView (DestroyInstances);

			TransformManager.FindTransform ("ItemDisplayCanvas").GetComponent<Canvas> ().enabled = true;

		}

		private void DestroyInstances(){

			TransformManager.DestroyTransform (this.transform);

			Resources.UnloadUnusedAssets ();

			System.GC.Collect ();

		}

	}
}
