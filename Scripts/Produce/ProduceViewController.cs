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

		private int totalValence;
		private int totalUnstableness;

		void Awake(){

			produceView = GetComponent<ProduceView> ();
	
		}
			

		public void SetUpProduceView(ItemModel itemModel){

			this.itemModel = itemModel;

			for (int i = 0; i < itemModel.materials.Count; i++) {
				
				Material material = itemModel.materials [i];

				materialsForProduce.Add (new Material (material, 0));

			}

			CalculateTotalValenceAndUnstableness ();

			produceView.SetUpProduceView (itemModel,materialsForProduce,totalValence,totalValence);

		}

		public void MaterialCountPlusOne(Material material){
			
			Material materialInBag = Player.mainPlayer.GetMaterialInBagWithId (material.itemId);

			if (material.itemCount >= materialInBag.itemCount) {
				produceView.DisplayNotEnoughText ();
				return;
			}
			material.itemCount += 1;

			CalculateTotalValenceAndUnstableness ();

			produceView.SetUpProduceView (itemModel, materialsForProduce, totalValence, totalUnstableness);

			CheckProduceQualification ();
		}

		public void MaterialCountMinusOne(Material material){
			if (material.itemCount <= 0) {
				return;
			}
			material.itemCount -= 1;

			CalculateTotalValenceAndUnstableness ();

			produceView.SetUpProduceView (itemModel, materialsForProduce, totalValence, totalUnstableness);

			CheckProduceQualification ();
		}

		private void CheckProduceQualification(){

			for (int i = 0; i < materialsForProduce.Count; i++) {
				if (materialsForProduce [i].itemCount < 1) {
					return;
				}
			}

			if(totalValence == 0){
				produceView.EnableProduce ();
			}
		}


		public void OnAddFuseStoneButtonClick(){
			produceView.SetUpFuseStonesDisplayPlane ();
		}

		public void SelectFuseStone(FuseStone fuseStone){
			fuseStoneSelected = fuseStone;
			CalculateTotalValenceAndUnstableness ();
			produceView.UpdateTotalUnstableness (totalUnstableness);
		}

		public void OnConfirmFuseStoneButtonClick(){
			produceView.QuitFuseStonesDisplayPlane ();
		}

		private void UseFuseStone(){
			if (fuseStoneSelected != null){
				Player.mainPlayer.allFuseStonesInBag.Remove (fuseStoneSelected);
				fuseStoneSelected = null;
			}
		}


		private bool ProduceSuccess(int unstableness){
			return Random.Range (0, 100) > unstableness;
		}

		private Material RandomFailMaterial(List<Material> materials){
			int index = Random.Range (0, materials.Count - 1);
			return materials [index];
		}

		/// <summary>
		/// 制造按钮点击响应
		/// </summary>
		public void OnProduceButtonClick(){

			Item item = null;

			if (fuseStoneSelected != null) {
				totalUnstableness -= fuseStoneSelected.successGain;
			}

			switch (itemModel.itemType) {
			case ItemType.Equipment:
				if (ProduceSuccess(totalUnstableness)) {
					item = new Equipment (itemModel,fuseStoneSelected);
				} else {
					Material failMaterial = RandomFailMaterial (itemModel.failMaterials);
					item = new Material (failMaterial, 1);
				}
				break;
//			case ItemType.Consumables:
//				item = new Consumables (itemModel);
//				break;
			default:
				break;
			}

			Player.mainPlayer.AddItem (item);

			Player.mainPlayer.RemoveMaterials (materialsForProduce);

			UseFuseStone ();

			produceView.SetUpItemDetailsPlane (item);

		}

		public void QuitItemDetailsPlane(){

			produceView.QuitItemDetailsPlane ();

			for (int i = 0; i < materialsForProduce.Count; i++) {
				materialsForProduce [i].itemCount = 0;
			}

			produceView.SetUpProduceView (itemModel, materialsForProduce,totalValence,totalUnstableness);

		}

		public void QuitFailMaterialHUD(){

			produceView.QuitFailMaterialHUD ();

		}

		private void CalculateTotalValenceAndUnstableness(){

			totalValence = 0;
			totalUnstableness = 0;
			
			for (int i = 0; i < materialsForProduce.Count; i++) {
				Material material = materialsForProduce [i];
				totalValence += material.valence * material.itemCount;
				totalUnstableness += material.unstableness * material.itemCount;
			}

			if (fuseStoneSelected != null) {
				totalUnstableness -= fuseStoneSelected.successGain;
			}

		}

		public void QuitProduceView(){
			
			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.itemDisplayCanvasBundleName, "ItemDisplayCanvas", () => {
				TransformManager.FindTransform ("ItemDisplayCanvas").GetComponent<Canvas> ().enabled = true;
			});

			gameObject.SetActive(false);

			TransformManager.DestroyTransfromWithName ("PoolContainerOfProduceCanvas", TransformRoot.PoolContainer);

		}

		public void DestroyInstances(){

			GameManager.Instance.UIManager.DestroryCanvasWith (CommonData.produceCanvasBundleName, "ProduceCanvas", "PoolContainerOfProduceCanvas", "ModelContainerOfProduceCanvas");

		}

	}
}
