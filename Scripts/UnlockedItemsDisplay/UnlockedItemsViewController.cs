using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class UnlockedItemsViewController : MonoBehaviour {

		public UnlockedItemsView unlockedItemsView;

		private ItemModel itemToCreate;

		public void SetUpUnlockedItemsView(){

			IEnumerator coroutine = SetUpViewAfterDataReady ();
			StartCoroutine (coroutine);

		}


		private IEnumerator SetUpViewAfterDataReady(){

			bool dataReady = false;

			while (!dataReady) {
				dataReady = GameManager.Instance.gameDataCenter.CheckDatasReady (new GameDataCenter.GameDataType[] {
					GameDataCenter.GameDataType.UISprites,
					GameDataCenter.GameDataType.ItemModels,
					GameDataCenter.GameDataType.ItemSprites
				});
				yield return null;
			}

			unlockedItemsView.InitUnlockedItemView (itemToCreate);
			unlockedItemsView.SetUpUnlockedItemsView (FormulaType.Equipment);

		}


		public void OnBeginSpellButtonClick(){
			unlockedItemsView.QuitUnlockedItemDetailHUD ();
			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.spellCanvasBundleName, "SpellCanvas", () => {
				TransformManager.FindTransform("SpellCanvas").GetComponent<SpellViewController>().SetUpSpellViewForCreate(itemToCreate);
			}, false, true);

		}

		public void OnUnlockedEqiupmentButtonClick(){
			unlockedItemsView.SetUpUnlockedItemsView (FormulaType.Equipment);
		}

		public void OnUnlockedConsumablesButtonClick(){
			unlockedItemsView.SetUpUnlockedItemsView (FormulaType.Consumables);
		}

		public void DestroyInstances(){
			GameManager.Instance.UIManager.DestroryCanvasWith (CommonData.spellCanvasBundleName, "UnlockedItemsCanvas", null, null);
		}

		public void OnQuitButtonClick(){

			unlockedItemsView.QuitUnlockedItemsView ();

		}

	}
}
