using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	public class DataCenter {

//		public Dictionary<string,List<object>> cacheDic = new Dictionary<string, List<object>> ();

//		public void UnloadCaches(string[] cacheNames,Transform[] cacheContainers){
//
//			for(int i = 0;i<cacheNames.Length;i++){
//				string cacheName = cacheNames [i];
//				cacheDic [cacheName].Clear ();
//				cacheDic.Remove (cacheName);
//			}
//
//			if (cacheContainers != null) {
//				for (int i = 0; i < cacheContainers.Length; i++) {
//					GameObject.Destroy (cacheContainers[i]);
//
//				}
//			}
//
//			Resources.UnloadUnusedAssets ();
//
//			System.GC.Collect ();
//
//
//		}


		private GameSettings mGameSettings;
		public GameSettings gameSettings{

			get{

				if (mGameSettings == null) {
					mGameSettings = new GameSettings ();
				}
				return mGameSettings;

			}
			set{
				mGameSettings = value;
			}
		}

		private LearningInfo mLearnInfo;

		public LearningInfo learnInfo{
			get{
				if (mLearnInfo == null) {
					mLearnInfo = new LearningInfo ();
				}
				mLearnInfo.SetUpWords ();
				return mLearnInfo;
			}
			set{
				mLearnInfo = value;
			}
		}

		private List<Material> mAllMaterials = new List<Material> ();
		public List<Material> allMaterials{
			get{
				if (mAllMaterials.Count == 0) {
					Material[] materials = DataHandler.LoadDataToModelWithPath<Material> (CommonData.materialsDataFilePath);
					for (int i = 0; i < materials.Length; i++) {
						mAllMaterials.Add (materials [i]);
					}

//					cacheDic.Add ("allMaterials",mAllMaterials);
				}
				return mAllMaterials;
			}
		}

		private List<Sprite> mAllMaterialSprites = new List<Sprite> ();
		public List<Sprite> allMaterialSprites{
			get{
				if (mAllMaterialSprites.Count == 0) {
					ResourceManager.Instance.LoadAssetWithBundlePath<Sprite>("material/icons",()=>{
						for(int i = 0;i<ResourceManager.Instance.sprites.Count;i++){
							mAllMaterialSprites.Add(ResourceManager.Instance.sprites[i]);
						}
					},true);
//					cacheDic.Add ("allMaterialSprites", mAllMaterialSprites);
				}
				return mAllMaterialSprites;
			}
		}

		private List<ItemModel> mAllItemModels = new List<ItemModel> ();
		public List<ItemModel> allItemModels{
			get{
				if (mAllItemModels.Count == 0) {
					ItemModel[] itemModels = DataHandler.LoadDataToModelWithPath<ItemModel> (CommonData.itemsDataFilePath);
					for (int i = 0; i < itemModels.Length; i++) {
						mAllItemModels.Add (itemModels [i]);
					}
//					cacheDic.Add ("allItemModels", mAllItemModels);
				}
				return mAllItemModels;
			}

		}

		private List<Sprite> mAllItemSprites = new List<Sprite>();
		public List<Sprite> allItemSprites{

			get{
				if (mAllItemSprites.Count == 0) {
					ResourceManager.Instance.LoadAssetWithBundlePath<Sprite> ("item/icons", () => {
						// 获取所有游戏物品的图片
						for(int i = 0;i<ResourceManager.Instance.sprites.Count;i++){
							mAllItemSprites.Add(ResourceManager.Instance.sprites[i]);
						}
					},true);
//					cacheDic.Add ("allItemSprites", mAllItemSprites);
				}

				return mAllItemSprites;
			}

		}




		private List<Sprite> mAllMapSprites = new List<Sprite> ();
		public List<Sprite> allMapSprites{
			get{
				if (mAllMapSprites.Count == 0) {

					ResourceManager.Instance.LoadAssetWithBundlePath<Sprite> ("mapicons", () => {

						foreach (Sprite s in ResourceManager.Instance.sprites) {
							mAllMapSprites.Add (s);
						}
					},true);
//					cacheDic.Add ("allMapSprites", mAllMapSprites);
				}
				return mAllMapSprites;
			}

		}

		private List<Skill> mAllSkills = new List<Skill>();
		public List<Skill> allSkills{

			get{
				if(mAllSkills.Count == 0){
					Transform allSkillsContainer = TransformManager.NewTransform("AllSkills",GameObject.Find(CommonData.instanceContainerName).transform);

					ResourceManager.Instance.gos.Clear ();

					ResourceManager.Instance.LoadAssetWithBundlePath ("skills/skill", () => {
						for(int i = 0;i<ResourceManager.Instance.gos.Count;i++){
							Skill skill = ResourceManager.Instance.gos[i].GetComponent<Skill>();
							mAllSkills.Add(skill);
							skill.transform.SetParent(allSkillsContainer);
						}
					},true);

					SortSkillsById (mAllSkills);

//					cacheDic.Add ("allSkills", mAllSkills);

				}

				return mAllSkills;
			}

		}

		// 技能按照id排序方法
		private void SortSkillsById(List<Skill> skills){
			Skill temp;
			for (int i = 0; i < skills.Count - 1; i++) {
				for (int j = 0; j < skills.Count - 1 - i; j++) {
					Skill sBefore = skills [j];
					Skill sAfter = skills [j + 1];
					if (sBefore.skillId > sAfter.skillId) {
						temp = sBefore;
						skills [j] = sAfter;
						skills [j + 1] = temp; 
					}
				}
			}
		}


		private List<Sprite> mAllSkillSprites = new List<Sprite>();
		public List<Sprite> allSkillSprites{
			get{
				if (mAllSkillSprites.Count == 0) {
					ResourceManager.Instance.LoadAssetWithBundlePath<Sprite> ("skills/icons", () => {
						// 获取所有游戏物品的图片
						for(int i = 0;i<ResourceManager.Instance.sprites.Count;i++){
							mAllSkillSprites.Add(ResourceManager.Instance.sprites[i]);
						}
					},true);
//					cacheDic.Add ("allSkillSprites", mAllSkillSprites);
				}

				return mAllSkillSprites;
			}
		}

		private List<Sprite> mAllUIIcons = new List<Sprite> ();
		public List<Sprite> allUIIcons{
			get{
				if (mAllUIIcons.Count == 0) {
					ResourceManager.Instance.LoadAssetWithBundlePath<Sprite>("ui_icons",()=>{
						for(int i = 0;i<ResourceManager.Instance.sprites.Count;i++){
							mAllUIIcons.Add(ResourceManager.Instance.sprites[i]);
						}
					},true);
//					cacheDic.Add ("allUIIcons", mAllUIIcons);
				}
				return mAllUIIcons;
			}
		}

		private List<Transform> mAllMonsters = new List<Transform>();
		public List<Transform> allMonsters{
			get{
				if (mAllMonsters.Count == 0) {
					ResourceManager.Instance.LoadAssetWithBundlePath ("monsters", () => {
						for(int i = 0;i<ResourceManager.Instance.gos.Count;i++){
							Transform monster = ResourceManager.Instance.gos[i].transform;
							mAllMonsters.Add(monster);
						};
					}, true);
//					cacheDic.Add ("allMonsters", allMonsters);
				}
				return mAllMonsters;
			}
		}
	}
}
