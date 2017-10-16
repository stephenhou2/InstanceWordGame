using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace WordJourney
{
	public class ProduceView : MonoBehaviour {

		public Image itemIcon;
		public Text itemName;
		public Text levelRequired;
		public Text itemBaseProperties;
		public Text itemAttachedProperties;

		public Transform[] materialDetailViews;

		public Transform totalView;

		private List<int> indexGrid = new List<int> ();

		private List<Transform> randomMaterialDetailViews = new List<Transform>();



		public void SetUpProduceView(ItemModel itemModel,List<Material> materialsForProduce){

			SetUpItemDetails (itemModel);

			GetRandomMaterialButtons (materialsForProduce);

			SetUpMaterialButtons (materialsForProduce);

		}

		private void GetRandomMaterialButtons(List<Material> materials){

			indexGrid.Clear ();

			randomMaterialDetailViews.Clear();

			for (int i = 0; i < materialDetailViews.Length; i++) {
				indexGrid.Add (i);
			}

			for (int i = 0; i < materials.Count; i++) {

				int index = materials[i].id % indexGrid.Count;

				randomMaterialDetailViews.Add (materialDetailViews [index]);

			}
		}

		private void SetUpMaterialButtons(List<Material> materials){

			int totalValence = 0;
			float totalUnstableness = 0f;

			for (int i = 0; i < randomMaterialDetailViews.Count; i++) {

				Material material = materials [i];
				Transform materialDetailView = randomMaterialDetailViews [i];

				Image materialIcon = materialDetailView.Find ("MaterialIcon").GetComponent<Image> ();
				Text materialName = materialDetailView.Find ("MaterialName").GetComponent<Text> ();
				Text materialValence = materialDetailView.Find ("MaterialValence").GetComponent<Text> ();
				Text materialProperty = materialDetailView.Find ("MaterialProperty").GetComponent<Text> ();
				Text materialCount = materialDetailView.Find ("MaterialCount").GetComponent<Text> ();


				Button plusButton = materialDetailView.Find ("PlusButton").GetComponent<Button> ();
				Button minusButton = materialDetailView.Find ("MinusButton").GetComponent<Button> ();

				materialName.text = material.materialName;
				materialValence.text = material.valence.ToString ();
				materialProperty.text = material.propertyString;
				materialCount.text = material.materialCount.ToString ();

				Sprite s = GameManager.Instance.dataCenter.allMaterialSprites.Find (delegate(Sprite obj) {
					return obj.name == material.spriteName;
				});

				if (s != null) {
					materialIcon.sprite = s;
				}

				plusButton.onClick.RemoveAllListeners ();
				minusButton.onClick.RemoveAllListeners ();

				plusButton.onClick.AddListener (delegate {

					GetComponent<ProduceViewController>().MaterialCountPlusOne(material);

				});

				minusButton.onClick.AddListener (delegate {
					
					GetComponent<ProduceViewController>().MaterialCountMinusOne(material);

				});

				materialDetailView.gameObject.SetActive (true);

				totalValence += material.valence * material.materialCount;
				totalUnstableness += material.unstableness * material.materialCount;
			}


			totalView.Find ("TotalValence").GetComponent<Text> ().text = totalValence.ToString();
			totalView.Find ("TotalUnstableness").GetComponent<Text> ().text = string.Format ("{0}%", (int)(totalUnstableness * 100));

			totalView.gameObject.SetActive (true);

		}

		private void SetUpItemDetails(ItemModel itemModel){

			itemName.text = itemModel.itemName;
			levelRequired.text = string.Format ("等级要求:{0}", itemModel.levelRequired);
				
//			if (itemModel.itemType == ItemType.Equipment) {
//				Equipment equipment = new Equipment (itemModel);
//			}


		}


	}
}
