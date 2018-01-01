using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WordJourney
{
	public class MaterialDisplayView : MonoBehaviour {

		public Transform materialsScrollView;// 材料展示scrollView;
		public Transform materialsContainer;//材料展示模型容器
		private Transform materialBtnModel;//材料展示模型

		private InstancePool materialBtnPool;//材料模型缓存池

		private List<Sprite> materialSprites;//所有材料图片



		public void Initialize(){
			
			// 获取所有材料图片
			this.materialSprites = GameManager.Instance.gameDataCenter.allMaterialSprites;

			// 创建材料展示模型缓存池

			Transform poolContainerOfMaterialDisplayCanvas = TransformManager.FindOrCreateTransform (CommonData.poolContainerName + "/PoolContainerOfMaterialDisplayCanvas");
			Transform modelContainerOfMaterialDisplayCanvas = TransformManager.FindOrCreateTransform (CommonData.instanceContainerName + "/ModelContainerOfMaterialDisplayCanvas");


			if (poolContainerOfMaterialDisplayCanvas.childCount == 0) {
				// 创建材料展示模型缓存池
				materialBtnPool = InstancePool.GetOrCreateInstancePool ("MaterialBtnPool",poolContainerOfMaterialDisplayCanvas.name);
//				materialBtnPool.transform.SetParent (poolContainerOfMaterialDisplayCanvas);
			}

			if (modelContainerOfMaterialDisplayCanvas.childCount == 0) {
				// 获取材料展示模型
				materialBtnModel = TransformManager.FindTransform ("MaterialBtnModel");
				materialBtnModel.transform.SetParent (modelContainerOfMaterialDisplayCanvas);
			}
			GetComponent<Canvas>().enabled = true;

		}


		/// <summary>
		/// 按照给定材料种类设置图鉴界面
		/// </summary>
		/// <param name="mt">Mt.</param>
//		public void SetUpMaterials(List<Material> materialsOfTargetType){
//
//			// 将容器中的所有材料模型放入缓存池
//			materialBtnPool.AddChildInstancesToPool (materialsContainer);
//
//			materialsScrollView.GetComponent<ScrollRect> ().velocity = Vector2.zero;
//
//			materialsContainer.localPosition = Vector3.zero;
//
//			// 更新图鉴界面
//			for (int i = 0; i < materialsOfTargetType.Count; i++) {
//				
//				Material m = materialsOfTargetType [i];
//
//				Sprite s = materialSprites.Find (delegate(Sprite obj) {
//					return obj.name == m.spriteName;
//				});
//
//				Transform materialBtn = materialBtnPool.GetInstance<Transform> (materialBtnModel.gameObject, materialsContainer);
//
//				Image materialIcon = materialBtn.Find ("MaterialIcon").GetComponent<Image>();
//				Text materialName = materialBtn.Find ("MaterialName").GetComponent<Text>();
//				Text materialValence = materialBtn.Find ("MaterialValence").GetComponent<Text>();
//				Text materialProperty = materialBtn.Find ("MaterialProperty").GetComponent<Text>();
//
//
//				if (s != null) {
//					materialIcon.sprite = s;
//				}
//
//				materialName.text = m.itemName;
//
//				materialValence.text = m.valence.ToString ();
//
//				materialProperty.text = m.itemDescription;
//
//				materialBtn.GetComponent<Button> ().onClick.RemoveAllListeners ();
//
//				materialBtn.GetComponent<Button> ().onClick.AddListener (delegate {
//
//					GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.spellCanvasBundleName,"SpellCanvas",()=>{
//
//						TransformManager.FindTransform("SpellCanvas").GetComponent<SpellViewController>().SetUpSpellViewForCreateMaterial(m);
//
//					});
//				});
//
//			}
//		}



	}
}
