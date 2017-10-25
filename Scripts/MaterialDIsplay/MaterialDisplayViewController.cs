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
		private List<Material> bossMaterials = new List<Material>();//boss材料列表

		/// <summary>
		/// 初始化图鉴界面
		/// </summary>
		public void SetUpMaterialView(){
			
			List<Material> materials = GameManager.Instance.dataCenter.allMaterials;

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
				case MaterialType.Boss:
					bossMaterials.Add (m);
					break;
				}

			}

			materialDisplayView = GetComponent<MaterialDisplayView> ();

			materialDisplayView.Initialize ();

			//默认进入材料图鉴界面展示的是基础材料
			materialDisplayView.SetUpMaterials(baseMaterials);

			GetComponent<Canvas> ().enabled = true;
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
			case MaterialType.Boss:
				materialsOfTargetType = bossMaterials;
				break;
			default:
				materialsOfTargetType = new List<Material> ();
				break;
			}

			currentMaterialType = mt;

			materialDisplayView.SetUpMaterials (materialsOfTargetType);

		}


		private void backup(){

//			if (material == null) {
//				ResourceLoader.CreateNewResourceLoader().LoadAssetWithBundlePath ("spell/canvas", () => {
//					spellCanvas = GameObject.Find(CommonData.instanceContainerName + "/SpellCanvas");
//					spellCanvas.GetComponent<SpellViewController>().SetUpSpellViewForCreate(null);
//				});
//				return;
//			}
//
//
//			List<char> unsufficientCharacters = Player.mainPlayer.CheckUnsufficientCharacters (material.spell);
//
//			if (unsufficientCharacters.Count > 0) {
//				foreach (char c in unsufficientCharacters) {
//					Debug.Log (string.Format ("字母{0}数量不足", c.ToString ()));
//				}
//				return;
//			}
//
//			// 如果玩家字母碎片足够，则进入拼写界面
//			ResourceLoader.CreateNewResourceLoader().LoadAssetWithBundlePath ("spell/canvas", () => {
//				spellCanvas = GameObject.Find(CommonData.instanceContainerName + "/SpellCanvas");
//				spellCanvas.GetComponent<SpellViewController>().SetUpSpellViewForCreate(material);
//			});
		}

	}
}
