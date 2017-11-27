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

		public Transform failMaterialHUD;


		private Tweener produceButtonAnim;



		/// <summary>
		/// 初始化制造界面
		/// </summary>
		/// <param name="itemModel">Item model.</param>
		/// <param name="materialsForProduce">Materials for produce.</param>
		public void SetUpProduceView(ItemModel itemModel,List<Material> materialsForProduce,int totalValence,int totalUnstableness){

			Transform poolContainerOfProduceCanvas = TransformManager.FindOrCreateTransform (CommonData.poolContainerName + "/PoolCotainerOfProduceCanvas");
			Transform modelContainerOfProduceCanvas = TransformManager.FindOrCreateTransform (CommonData.instanceContainerName + "/ModelContainerOfProduceCanvas");

			if (poolContainerOfProduceCanvas.childCount == 0) {
				// 创建缓存池
				fuseStonesPool = InstancePool.GetOrCreateInstancePool ("FuseStonesPool",poolContainerOfProduceCanvas.name);
//				fuseStonesPool.transform.SetParent (poolContainerOfProduceCanvas);
			}


			if (modelContainerOfProduceCanvas.childCount == 0) {
				// 获得融合石模型
				fuseStoneModel = TransformManager.FindTransform ("FuseStoneModel");
				fuseStoneModel.SetParent (modelContainerOfProduceCanvas);
			}


			itemNameInProduceView.text = itemModel.itemName;

			GetRandomMaterialButtons (materialsForProduce);

			SetUpMaterialButtons (materialsForProduce,totalValence,totalUnstableness);

//			GetComponent<Canvas>().enabled = true;

		}

		/// <summary>
		/// 获得材料在炼成阵中对应的按钮位置
		/// </summary>
		/// <param name="materials">Materials.</param>
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

		/// <summary>
		/// 初始化炼金阵中的材料按钮
		/// </summary>
		/// <param name="materials">Materials.</param>
		private void SetUpMaterialButtons(List<Material> materials,int totalValence,int totalUnstableness){

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

				Sprite s = GameManager.Instance.gameDataCenter.allMaterialSprites.Find (delegate(Sprite obj) {
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


			}


			totalView.Find ("TotalValence").GetComponent<Text> ().text = totalValence.ToString();
			UpdateTotalUnstableness (totalUnstableness);

			totalView.gameObject.SetActive (true);

		}

		/// <summary>
		/// 初始化制造出的物品显示界面
		/// </summary>
		/// <param name="equipment">Equipment.</param>
		public void SetUpItemDetailsPlane(Item item){

			if (item.itemType == ItemType.Equipment) {

				Text itemName = itemDetailsContainer.Find ("ItemName").GetComponent<Text> ();
				Text itemBaseProperties = itemDetailsContainer.Find ("ItemBaseProperties").GetComponent<Text> ();
				Text itemAttachedProperties = itemDetailsContainer.Find ("ItemAttachedProperties").GetComponent<Text> ();
				Image itemIcon = itemDetailsContainer.Find ("ItemIcon").GetComponent<Image> ();

				itemName.text = item.itemName;
				itemBaseProperties.text = item.GetItemBasePropertiesString ();

				Sprite s = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
					return obj.name == item.spriteName;
				});

				if (s != null) {
					itemIcon.sprite = s;
				}
				
//				levelRequired.text = string.Format ("等级要求:{0}", (item as Equipment).levelRequired);

				itemDetailsPlane.gameObject.SetActive (true);

			} else {

				Material failMaterial = item as Material;

				Text materialName = failMaterialHUD.Find ("MaterialName").GetComponent<Text> ();
				Text materialProperty = failMaterialHUD.Find ("MaterialProperty").GetComponent<Text> ();
				Text materialValence = failMaterialHUD.Find ("MaterialValence").GetComponent<Text> ();
				Text materialCount = failMaterialHUD.Find ("MaterialCount").GetComponent<Text> ();
				Image materialIcon = failMaterialHUD.Find ("MaterialIcon").GetComponent<Image> ();

				materialName.text = failMaterial.itemName;
				materialProperty.text = failMaterial.itemDescription;
				materialValence.text = failMaterial.valence.ToString ();
				materialCount.text = string.Format ("数量：{0}", failMaterial.itemCount);

				Sprite s = GameManager.Instance.gameDataCenter.allMaterialSprites.Find (delegate(Sprite obj) {
					return obj.name == failMaterial.spriteName;
				});

				if (s != null) {
					materialIcon.sprite = s;
				}

				failMaterialHUD.gameObject.SetActive (true);
			}

		}

		public void QuitFailMaterialHUD (){

			failMaterialHUD.gameObject.SetActive (false);

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

		/// <summary>
		/// 隐藏制造按钮
		/// </summary>
		private void DisableProduceButton(){
			
			Transform produceButton = totalView.Find ("ProduceButton");

			Image produceBackground = produceButton.GetComponent<Image> ();

			Color c = produceBackground.color;

			produceBackground.color = new Color (c.r, c.g, c.b, 1.0f);

			produceButtonAnim.Kill ();

			produceButton.gameObject.SetActive (false);
		}

		/// <summary>
		/// 初始化融合石选择界面
		/// </summary>
		public void SetUpFuseStonesDisplayPlane(){

			fuseStonesPool.AddChildInstancesToPool (fuseStonesContainer);

			for (int i = 0; i < Player.mainPlayer.allFuseStonesInBag.Count; i++) {

				FuseStone fuseStone = Player.mainPlayer.allFuseStonesInBag [i];

				Button fuseStoneButton = fuseStonesPool.GetInstance<Button> (fuseStoneModel.gameObject,fuseStonesContainer);

				Text fusetStoneName = fuseStoneButton.transform.Find ("FuseStoneName").GetComponent<Text> ();

				fusetStoneName.text = fuseStone.itemName;

				fuseStoneButton.onClick.RemoveAllListeners ();

				fuseStoneButton.onClick.AddListener (delegate {

					for (int j = 0; j < fuseStonesContainer.childCount; j++) {

						Transform trans = fuseStonesContainer.GetChild (j);

						if(trans != fuseStoneButton.transform){
							trans.Find ("SelectedBorder").GetComponent<Image>().enabled = false;
						}else{
							Image selectBorder = trans.Find ("SelectedBorder").GetComponent<Image>();
							selectBorder.enabled = !selectBorder.enabled;
							GetComponent<ProduceViewController>().SelectFuseStone(selectBorder.enabled ? fuseStone : null);
						}
					}
				});

			}

			fuseStonesDisplayPlane.gameObject.SetActive (true);

		}

		public void UpdateTotalUnstableness(int totalUnstableness){

			totalView.Find ("TotalUnstableness").GetComponent<Text> ().text = string.Format ("{0}%", totalUnstableness < 0 ? 0 : totalUnstableness);

		}

//		/// <summary>
//		/// 更新所有融合石的选框
//		/// </summary>
//		/// <param name="selectedFuseStoneTrans">Selected fuse stone trans.</param>
//		public void UpdateFuseStonesDisplayPlane(Transform selectedFuseStoneTrans){
//
//			for (int i = 0; i < fuseStonesContainer.childCount; i++) {
//				
//				Transform trans = fuseStonesContainer.GetChild (i);
//					
//				trans.Find ("SelectedBorder").gameObject.SetActive (trans == selectedFuseStoneTrans);
//
//			}
//		}

		/// <summary>
		/// 退出融合石选择界面
		/// </summary>
		public void QuitFuseStonesDisplayPlane(){
			fuseStonesDisplayPlane.gameObject.SetActive (false);
		}

		/// <summary>
		/// 退出制造出的物品显示界面
		/// </summary>
		public void QuitItemDetailsPlane(){

			DisableProduceButton ();

//			itemDetailsPlane.gameObject.SetActive (false);

		}


	}
}
