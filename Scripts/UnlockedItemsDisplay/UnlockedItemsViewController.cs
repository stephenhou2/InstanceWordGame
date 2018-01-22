using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class UnlockedItemsViewController : MonoBehaviour {

		public UnlockedItemsView unlockedItemsView;

		[HideInInspector]public ItemModel itemToCreate;

		private UnlockScrollType currentSelectedType;

		public void SetUpUnlockedItemsView(){
//			SoundManager.Instance.PlayAudioClip ("UI/sfx_UI_Click");
//			IEnumerator coroutine = SetUpViewAfterDataReady ();
//			StartCoroutine (coroutine);
//
//		}
//
//
//		private IEnumerator SetUpViewAfterDataReady(){
//
//			bool dataReady = false;
//
//			while (!dataReady) {
//				dataReady = GameManager.Instance.gameDataCenter.CheckDatasReady (new GameDataCenter.GameDataType[] {
//					GameDataCenter.GameDataType.UISprites,
//					GameDataCenter.GameDataType.ItemModels,
//					GameDataCenter.GameDataType.ItemSprites
//				});
//				yield return null;
//			}
			currentSelectedType = UnlockScrollType.Equipment;
			unlockedItemsView.InitUnlockedItemView ();
			unlockedItemsView.SetUpUnlockedItemsView (currentSelectedType);

		}


		public void OnBeginSpellButtonClick(){
			
			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.spellCanvasBundleName, "SpellCanvas", () => {
				TransformManager.FindTransform("SpellCanvas").GetComponent<SpellViewController>().SetUpSpellViewForCreate(itemToCreate,delegate{
					unlockedItemsView.QuitUnlockedItemDetailHUD ();
				});
			}, false, true);

		}

		public void OnUnlockedEqiupmentButtonClick(){
			if(currentSelectedType == UnlockScrollType.Equipment){
				return;
			}
			currentSelectedType = UnlockScrollType.Equipment;
			unlockedItemsView.SetUpUnlockedItemsView (currentSelectedType);
		}

		public void OnUnlockedConsumablesButtonClick(){
			if(currentSelectedType == UnlockScrollType.Consumables){
				return;
			}
			currentSelectedType = UnlockScrollType.Consumables;
			unlockedItemsView.SetUpUnlockedItemsView (currentSelectedType);
		}

		public void DestroyInstances(){
			GameManager.Instance.UIManager.RemoveCanvasCache ("UnlockedItemsCanvas");
			Destroy (this.gameObject);
			MyResourceManager.Instance.UnloadAssetBundle (CommonData.unlockedItemsCanvasBundleName,true);
		}

		public void OnQuitButtonClick(){

			unlockedItemsView.QuitUnlockedItemsView ();

		}

	}
}
