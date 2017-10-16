using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class ProduceViewController : MonoBehaviour {

		private ProduceView produceView;

		private ItemModel itemModel;

		private List<Material> materialsForProduce = new List<Material> ();


		void Awake(){

			produceView = GetComponent<ProduceView> ();
		

		}




		public void SetUpProduceView(ItemModel itemModel){

			this.itemModel = itemModel;

			for (int i = 0; i < itemModel.materials.Count; i++) {
				
				Material m = itemModel.materials [i];

				materialsForProduce.Add (new Material (m, 1));
			}

			produceView.SetUpProduceView (itemModel,materialsForProduce);

		}

		public void MaterialCountPlusOne(Material material){
			material.materialCount += 1;
			produceView.SetUpProduceView (itemModel, materialsForProduce);
		}

		public void MaterialCountMinusOne(Material material){
			if (material.materialCount <= 1) {
				return;
			}
			material.materialCount -= 1;
			produceView.SetUpProduceView (itemModel, materialsForProduce);
		}
	}
}
