using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	public class DataCenter {



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

					ResourceLoader materialSpritesLoader = ResourceLoader.CreateNewResourceLoader ();

					ResourceManager.Instance.LoadAssetsWithBundlePath<Sprite>(materialSpritesLoader,"material/icons",()=>{
						for(int i = 0;i<materialSpritesLoader.sprites.Count;i++){
							mAllMaterialSprites.Add(materialSpritesLoader.sprites[i]);
						}
					},true);

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


					ResourceLoader itemSpritesLoader = ResourceLoader.CreateNewResourceLoader ();

					ResourceManager.Instance.LoadAssetsWithBundlePath<Sprite> (itemSpritesLoader, "item/icons", () => {
						// 获取所有游戏物品的图片
						for(int i = 0;i<itemSpritesLoader.sprites.Count;i++){
							mAllItemSprites.Add(itemSpritesLoader.sprites[i]);
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

					ResourceLoader mapSpritesLoader = ResourceLoader.CreateNewResourceLoader ();

					ResourceManager.Instance.LoadAssetsWithBundlePath<Sprite> (mapSpritesLoader, "mapicons", () => {

						for(int i = 0;i<mapSpritesLoader.sprites.Count;i++){
							mAllMapSprites.Add (mapSpritesLoader.sprites[i]);
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

					Transform allSkillsContainer = TransformManager.FindOrCreateTransform ("AllSkills");

					ResourceLoader skillsLoader = ResourceLoader.CreateNewResourceLoader ();

					ResourceManager.Instance.LoadAssetsWithBundlePath (skillsLoader, "skills/skill", () => {
						for(int i = 0;i<skillsLoader.gos.Count;i++){
							Skill skill = skillsLoader.gos[i].GetComponent<Skill>();
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

					ResourceLoader skillSpritesLoader = ResourceLoader.CreateNewResourceLoader ();

					ResourceManager.Instance.LoadAssetsWithBundlePath<Sprite> (skillSpritesLoader, "skills/icons", () => {
						// 获取所有游戏物品的图片
						for(int i = 0;i<skillSpritesLoader.sprites.Count;i++){
							mAllSkillSprites.Add(skillSpritesLoader.sprites[i]);
						}
					},true);
//					cacheDic.Add ("allSkillSprites", mAllSkillSprites);
				}

				return mAllSkillSprites;
			}
		}

		private List<Sprite> mAllUISprites = new List<Sprite> ();
		public List<Sprite> allUISprites{
			get{
				if (mAllUISprites.Count == 0) {

					ResourceLoader UISpritesLoader = ResourceLoader.CreateNewResourceLoader ();

					ResourceManager.Instance.LoadAssetsWithBundlePath<Sprite>(UISpritesLoader, "ui_icons",()=>{
						for(int i = 0;i<UISpritesLoader.sprites.Count;i++){
							mAllUISprites.Add(UISpritesLoader.sprites[i]);
						}
					},true);
//					cacheDic.Add ("allUIIcons", mAllUIIcons);
				}
				return mAllUISprites;
			}
		}

		private List<Transform> mAllMonsters = new List<Transform>();
		public List<Transform> allMonsters{
			get{
				if (mAllMonsters.Count == 0) {

					ResourceLoader monstersLoader = ResourceLoader.CreateNewResourceLoader ();

					ResourceManager.Instance.LoadAssetsWithBundlePath (monstersLoader, "monsters", () => {
						for(int i = 0;i<monstersLoader.gos.Count;i++){
							Transform monster = monstersLoader.gos[i].transform;
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
