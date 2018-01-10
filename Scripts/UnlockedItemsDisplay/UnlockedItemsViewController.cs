using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class UnlockedItemsViewController : MonoBehaviour {

		public UnlockedItemsView unlockedItemsView;

		[HideInInspector]public ItemModel itemToCreate;

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

			unlockedItemsView.InitUnlockedItemView ();
			unlockedItemsView.SetUpUnlockedItemsView (UnlockScrollType.Equipment);

		}


		public void OnBeginSpellButtonClick(){
			
			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.spellCanvasBundleName, "SpellCanvas", () => {
				TransformManager.FindTransform("SpellCanvas").GetComponent<SpellViewController>().SetUpSpellViewForCreate(itemToCreate,delegate{
					unlockedItemsView.QuitUnlockedItemDetailHUD ();
				});
			}, false, true);

		}

		public void OnUnlockedEqiupmentButtonClick(){
			unlockedItemsView.SetUpUnlockedItemsView (UnlockScrollType.Equipment);
		}

		public void OnUnlockedConsumablesButtonClick(){
			unlockedItemsView.SetUpUnlockedItemsView (UnlockScrollType.Consumables);
		}

		public void DestroyInstances(){
			GameManager.Instance.UIManager.DestroryCanvasWith (CommonData.unlockedItemsCanvasBundleName, "UnlockedItemsCanvas", null, null);
		}

		public void OnQuitButtonClick(){

			unlockedItemsView.QuitUnlockedItemsView ();

		}

	}
}
