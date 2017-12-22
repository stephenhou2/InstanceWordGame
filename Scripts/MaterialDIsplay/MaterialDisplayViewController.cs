using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	public class MaterialDisplayViewController : MonoBehaviour {

		private MaterialDisplayView materialDisplayView;//图鉴view

		private MaterialType currentMaterialType;//当前图鉴的材料种类


		private List<Material> baseMaterials = new List<Material>();//基础材料列表
		private List<Material> rareMaterials = new List<Material>();//稀有材料列表
		private List<Material> monsterMaterials = new List<Material>();//怪物材料列表

		/// <summary>
		/// 初始化图鉴界面
		/// </summary>
		public void SetUpMaterialView(){
			
			StartCoroutine ("SetUpViewAfterDataReady");
		}


		private IEnumerator SetUpViewAfterDataReady(){

			bool dataReady = false;

			while (!dataReady) {
				dataReady = GameManager.Instance.gameDataCenter.CheckDatasReady (new GameDataCenter.GameDataType[] {
					GameDataCenter.GameDataType.UISprites,
					GameDataCenter.GameDataType.Materials,
					GameDataCenter.GameDataType.MaterialSprites
				});
				yield return null;
			}

			List<Material> materials = GameManager.Instance.gameDataCenter.allMaterials;

			for (int i = 0; i < materials.Count; i++) {
				Material m = materials [i];
				switch (m.materialType) {
				case MaterialType.Base:
					baseMaterials.Add (m);
					break;
				case MaterialType.Rare:
					rareMaterials.Add (m);
					break;
				case MaterialType.Monster:
					monsterMaterials.Add (m);
					break;
				}

			}

			materialDisplayView = GetComponent<MaterialDisplayView> ();

			materialDisplayView.Initialize ();

			//默认进入材料图鉴界面展示的是基础材料
			materialDisplayView.SetUpMaterials(baseMaterials);


		}


		/// <summary>
		/// 材料类型按钮点击响应
		/// </summary>
		/// <param name="typeIndex">Type index.</param>
		public void OnMaterialTypeButtonClick(int typeIndex){

			MaterialType mt = (MaterialType)(typeIndex);

			if (mt == currentMaterialType) {
				return;
			}

			List<Material> materialsOfTargetType;

			// 获取给定类型下的所有材料
			switch (mt) {
			case MaterialType.Base:
				materialsOfTargetType = baseMaterials;
				break;
			case MaterialType.Rare:
				materialsOfTargetType = rareMaterials;
				break;
			case MaterialType.Monster:
				materialsOfTargetType = monsterMaterials;
				break;
			default:
				materialsOfTargetType = new List<Material> ();
				break;
			}

			currentMaterialType = mt;

			materialDisplayView.SetUpMaterials (materialsOfTargetType);

		}

		public void OnQuitMaterialDisplayView(){

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.homeCanvasBundleName, "HomeCanvas", () => {
				TransformManager.FindTransform("HomeCanvas").GetComponent<HomeViewController>().SetUpHomeView();
			});

//			GetComponent<Canvas>().enabled = false;

			GameManager.Instance.gameDataCenter.ReleaseDataWithDataTypes (new GameDataCenter.GameDataType[]{ 
				GameDataCenter.GameDataType.Materials,
				GameDataCenter.GameDataType.MaterialSprites});

			TransformManager.DestroyTransfromWithName ("PoolContainerOfMaterialDisplayCanvas", TransformRoot.PoolContainer);

		}

		public void DestroyInstances(){

			GameManager.Instance.UIManager.DestroryCanvasWith (CommonData.materialDisplayCanvasBundleName, "MaterialDisplayCanvas", "PoolContainerOfMaterialDisplayCanvas", "ModelContainerOfMaterialDisplayCanvas");

		}

	}
}
