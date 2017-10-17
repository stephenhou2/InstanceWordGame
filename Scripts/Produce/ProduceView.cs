using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace WordJourney
{
	public class ProduceView : MonoBehaviour {

		public Text itemNameInProduceView;

		public Transform itemDetailsPlane;
		public Transform itemDetailsContainer;

		public Transform[] materialDetailViews;

		public Transform totalView;

		private List<int> indexGrid = new List<int> ();

		private List<Transform> randomMaterialDetailViews = new List<Transform>();

		public Transform fuseStonesDisplayPlane;
		public Transform fuseStonesContainer;
		private InstancePool fuseStonesPool;
		private Transform fuseStoneModel;




		private Tweener produceButtonAnim;



		void Awake(){

			fuseStonesPool = InstancePool.GetOrCreateInstancePool ("FuseStonesPool");
			fuseStoneModel = TransformManager.FindTransform ("FuseStoneModel");

		}



		public void SetUpProduceView(ItemModel itemModel,List<Material> materialsForProduce){

			itemNameInProduceView.text = itemModel.itemName;

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

				int index = materials[i].itemId % indexGrid.Count;

				randomMaterialDetailViews.Add (materialDetailViews [index]);

			}
		}

		private void SetUpMaterialButtons(List<Material> materials){

			int totalValence = 0;
			int totalUnstableness = 0;

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

				materialName.text = material.itemName;
				materialValence.text = material.valence.ToString ();
				materialProperty.text = material.itemDescription;
				materialCount.text = material.itemCount.ToString ();

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

				totalValence += material.valence * material.itemCount;
				totalUnstableness += material.unstableness * material.itemCount;
			}


			totalView.Find ("TotalValence").GetComponent<Text> ().text = totalValence.ToString();
			totalView.Find ("TotalUnstableness").GetComponent<Text> ().text = string.Format ("{0}%", totalUnstableness);

			totalView.gameObject.SetActive (true);

		}

		/// <summary>
		/// Sets up item details.
		/// </summary>
		/// <param name="equipment">Equipment.</param>
		public void SetUpItemDetailsPlane(Item item){

			Text itemName = itemDetailsContainer.Find ("ItemName").GetComponent<Text> ();
			Text levelRequired = itemDetailsContainer.Find ("LevelRequired").GetComponent<Text> ();
			Text itemBaseProperties = itemDetailsContainer.Find ("ItemBaseProperties").GetComponent<Text> ();
			Text itemAttachedProperties = itemDetailsContainer.Find("ItemAttachedProperties").GetComponent<Text> ();
			Image itemIcon = itemDetailsContainer.Find("ItemIcon").GetComponent<Image> ();

			itemName.text = item.itemName;
			itemBaseProperties.text = item.GetItemBasePropertiesString ();

			Sprite s = GameManager.Instance.dataCenter.allItemSprites.Find(delegate(Sprite obj){
				return obj.name == item.spriteName;
			});

			if (s != null) {
				itemIcon.sprite = s;
			}
				
			levelRequired.text = string.Format ("等级要求:{0}", item.levelRequired);

			itemDetailsPlane.gameObject.SetActive (true);

		}

		/// <summary>
		/// 显示库存不足
		/// </summary>
		public void DisplayNotEnoughText(){

		}

		/// <summary>
		/// 点亮制造按钮
		/// </summary>
		public void EnableProduce(){

			Transform produceButton = totalView.Find ("ProduceButton");

			produceButtonAnim = produceButton.GetComponent<Image> ().DOFade (0.5f, 2f).OnComplete (() => {
					produceButton.GetComponent<Image> ().DOFade (1f, 2f);
			});
				
			produceButtonAnim.SetLoops (-1);

			produceButton.gameObject.SetActive (true);
		}

		private void DisableProduceButton(){
			
			Transform produceButton = totalView.Find ("ProduceButton");

			Image produceBackground = produceButton.GetComponent<Image> ();

			Color c = produceBackground.color;

			produceBackground.color = new Color (c.r, c.g, c.b, 1.0f);

			produceButtonAnim.Kill ();

			produceButton.gameObject.SetActive (false);
		}


		public void SetUpFuseStonesDisplayPlane(){

			fuseStonesPool.AddChildInstancesToPool (fuseStonesContainer);

			for (int i = 0; i < Player.mainPlayer.allFuseStonesInBag.Count; i++) {

				FuseStone fuseStone = Player.mainPlayer.allFuseStonesInBag [i];

				Button fuseStoneButton = fuseStonesPool.GetInstance<Button> (fuseStoneModel.gameObject,fuseStonesContainer);

				fuseStoneButton.GetComponent<Text> ().text = fuseStone.itemName;

				fuseStoneButton.transform.Find ("SelectedBorder").gameObject.SetActive (false);

				fuseStoneButton.onClick.RemoveAllListeners ();

				fuseStoneButton.onClick.AddListener (delegate {
					GetComponent<ProduceViewController>().OnFuseStoneButtonClick(fuseStone,fuseStoneButton.transform);
				});

			}

			fuseStonesDisplayPlane.gameObject.SetActive (true);

		}

		public void UpdateFuseStonesDisplayPlane(Transform selectedFuseStoneTrans){

			for (int i = 0; i < fuseStonesContainer.childCount; i++) {
				
				Transform trans = fuseStonesContainer.GetChild (i).Find ("SelectedBorder");
					
				trans.gameObject.SetActive (trans == selectedFuseStoneTrans);

			}
		}

		public void QuitFuseStonesDisplayPlane(){
			fuseStonesDisplayPlane.gameObject.SetActive (false);
		}

		public void QuitItemDetailsPlane(){

			DisableProduceButton ();

			itemDetailsPlane.gameObject.SetActive (false);

		}

		public void QuitProduceView(CallBack cb){



		}

	}
}
